using System.Buffers;
using System.Text;

namespace RunaString;

/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a <see cref="char"/> buffer.
/// </summary>
public ref struct CharsSpanEnumerator
    : IRunaEnumerator<CharsSpanEnumerator, CharsIndex, ReadOnlySpan<char>>
{
    /// <inheritdoc />
    public static CharsSpanEnumerator Empty =>
        new([]);

    // NOTE: keep order to save size
    private int _currCharIndex = -1;
    private int _nextCharIndex = 0;
    private int _runeIndex = -1;
    private Rune _current = default;
    private readonly ReadOnlySpan<char> _buffer;

    internal readonly int NextCharIndex => _nextCharIndex;

    /// <inheritdoc />
    public readonly CharsIndex SeekIndex => new(_currCharIndex, _runeIndex);

    /// <inheritdoc />
    public readonly Rune Current => _current;

    /// <inheritdoc />
    public readonly ReadOnlySpan<char> SourceBuffer => _buffer;

    /// <inheritdoc />
    public readonly ReadOnlySpan<char> ConsumedBuffer => _buffer.Slice(0, _nextCharIndex);

    /// <inheritdoc />
    public readonly ReadOnlySpan<char> RemainingBuffer => _buffer.Slice(_nextCharIndex);

    private CharsSpanEnumerator(ReadOnlySpan<char> buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Creates a <see cref="CharsSpanEnumerator"/> for the specified <see cref="char"/> buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static CharsSpanEnumerator Create(ReadOnlySpan<char> buffer) => new(buffer);

    /// <inheritdoc />
    public bool MoveNext()
    {
        _currCharIndex = _nextCharIndex;
        return Helpers.MoveNext(_buffer, ref _nextCharIndex, ref _runeIndex, out _current);
    }

    /// <inheritdoc />
    public CharsSpanEnumerator Seek(CharsIndex index) =>
        new(_buffer)
        {
            _currCharIndex = index.CharIndex,
            _nextCharIndex = Helpers.GetNextCharIndex(_buffer, index.CharIndex),
            _runeIndex = index.RuneIndex,
        };
}


/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a <see cref="char"/> buffer.
/// </summary>
public struct CharsMemoryEnumerator
    : IRunaEnumerator<CharsMemoryEnumerator, CharsIndex, ReadOnlyMemory<char>>
{
    /// <inheritdoc />
    public static CharsMemoryEnumerator Empty { get; } =
        new(ReadOnlyMemory<char>.Empty);

    private int _currCharIndex = -1;
    private int _nextCharIndex = 0;
    private int _runeIndex = -1;
    private Rune _current = default;
    private readonly ReadOnlyMemory<char> _buffer;

    /// <inheritdoc />
    public readonly CharsIndex SeekIndex => new(_currCharIndex, _runeIndex);

    /// <inheritdoc />
    public readonly Rune Current => _current;

    /// <inheritdoc />
    public readonly ReadOnlyMemory<char> SourceBuffer => _buffer;

    /// <inheritdoc />
    public readonly ReadOnlyMemory<char> ConsumedBuffer => _buffer.Slice(0, _nextCharIndex);

    /// <inheritdoc />
    public readonly ReadOnlyMemory<char> RemainingBuffer => _buffer.Slice(_nextCharIndex);

    private CharsMemoryEnumerator(ReadOnlyMemory<char> buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Creates a <see cref="CharsMemoryEnumerator"/> for the specified <see cref="char"/> buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static CharsMemoryEnumerator Create(ReadOnlyMemory<char> buffer) => new(buffer);

    /// <inheritdoc />
    public bool MoveNext()
    {
        _currCharIndex = _nextCharIndex;
        return Helpers.MoveNext(_buffer.Span, ref _nextCharIndex, ref _runeIndex, out _current);
    }

    /// <inheritdoc />
    public CharsMemoryEnumerator Seek(CharsIndex index) =>
        new(_buffer)
        {
            _currCharIndex = index.CharIndex,
            _nextCharIndex = Helpers.GetNextCharIndex(_buffer.Span, index.CharIndex),
            _runeIndex = index.RuneIndex,
        };
}


file static class Helpers
{
    public static int GetNextCharIndex(ReadOnlySpan<char> buffer, int charIndex)
    {
        if (charIndex >= buffer.Length)
        {
            return buffer.Length;
        }
        var status = Rune.DecodeFromUtf16(buffer.Slice(charIndex), out _, out var charsConsumed);
        if (status != OperationStatus.Done)
        {
            throw new InvalidOperationException($"Invalid char sequence at char index {charIndex}");
        }
        return charIndex + charsConsumed;
    }

    public static bool MoveNext(
        ReadOnlySpan<char> buffer,
        ref int nextCharIndex,
        ref int runeIndex,
        out Rune current)
    {
        if (nextCharIndex >= buffer.Length)
        {
            current = default;
            return false;
        }
        var status = Rune.DecodeFromUtf16(buffer.Slice(nextCharIndex), out current, out var charsConsumed);
        if (status != OperationStatus.Done)
        {
            throw new InvalidOperationException($"Invalid char sequence at char index {nextCharIndex}");
        }
        nextCharIndex += charsConsumed;
        ++runeIndex;
        return true;
    }
}