using System.Buffers;
using System.Text;

namespace RunaString;

/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a UTF-16 encoded buffer.
/// </summary>
public ref struct Utf16SpanEnumerator
    : IRuneEnumerator<Utf16SpanEnumerator, Utf16RuneIndex, ReadOnlySpan<char>>
{
    /// <inheritdoc />
    public static Utf16SpanEnumerator Empty =>
        new([]);

    // NOTE: keep order to save size
    private int _currCharIndex = -1;
    private int _nextCharIndex = 0;
    private int _runePosition = -1;
    private Rune _current = default;
    private readonly ReadOnlySpan<char> _buffer;

    /// <inheritdoc />
    public Utf16RuneIndex SeekIndex => new(_currCharIndex, _runePosition);

    /// <inheritdoc />
    public Rune Current => _current;

    /// <inheritdoc />
    public ReadOnlySpan<char> SourceBuffer => _buffer;

    /// <inheritdoc />
    public ReadOnlySpan<char> ConsumedBuffer => _buffer.Slice(0, _nextCharIndex);

    /// <inheritdoc />
    public ReadOnlySpan<char> RemainingBuffer => _buffer.Slice(_nextCharIndex);

    private Utf16SpanEnumerator(ReadOnlySpan<char> buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Creates a <see cref="Utf16SpanEnumerator"/> for the specified UTF-16 buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static Utf16SpanEnumerator Create(ReadOnlySpan<char> buffer) => new(buffer);

    /// <inheritdoc />
    public bool MoveNext()
    {
        _currCharIndex = _nextCharIndex;
        return Helpers.MoveNext(_buffer, ref _nextCharIndex, ref _runePosition, out _current);
    }

    /// <inheritdoc />
    public Utf16SpanEnumerator Seek(Utf16RuneIndex index) =>
        new(_buffer)
        {
            _currCharIndex = index.CharIndex,
            _nextCharIndex = Helpers.GetNextCharIndex(_buffer, index.CharIndex),
            _runePosition = index.RuneIndex,
        };
}


/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a UTF-16 encoded buffer.
/// </summary>
public struct Utf16MemoryEnumerator
    : IRuneEnumerator<Utf16MemoryEnumerator, Utf16RuneIndex, ReadOnlyMemory<char>>
{
    /// <inheritdoc />
    public static Utf16MemoryEnumerator Empty { get; } =
        new(ReadOnlyMemory<char>.Empty);

    private int _currCharIndex = -1;
    private int _nextCharIndex = 0;
    private int _runePosition = -1;
    private Rune _current = default;
    private readonly ReadOnlyMemory<char> _buffer;

    /// <inheritdoc />
    public Utf16RuneIndex SeekIndex => new(_currCharIndex, _runePosition);

    /// <inheritdoc />
    public Rune Current => _current;

    /// <inheritdoc />
    public ReadOnlyMemory<char> SourceBuffer => _buffer;

    /// <inheritdoc />
    public ReadOnlyMemory<char> ConsumedBuffer => _buffer.Slice(0, _nextCharIndex);

    /// <inheritdoc />
    public ReadOnlyMemory<char> RemainingBuffer => _buffer.Slice(_nextCharIndex);

    private Utf16MemoryEnumerator(ReadOnlyMemory<char> buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Creates a <see cref="Utf16MemoryEnumerator"/> for the specified UTF-16 buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static Utf16MemoryEnumerator Create(ReadOnlyMemory<char> buffer) => new(buffer);

    /// <inheritdoc />
    public bool MoveNext()
    {
        _currCharIndex = _nextCharIndex;
        return Helpers.MoveNext(_buffer.Span, ref _nextCharIndex, ref _runePosition, out _current);
    }

    /// <inheritdoc />
    public Utf16MemoryEnumerator Seek(Utf16RuneIndex index) =>
        new(_buffer)
        {
            _currCharIndex = index.CharIndex,
            _nextCharIndex = Helpers.GetNextCharIndex(_buffer.Span, index.CharIndex),
            _runePosition = index.RuneIndex,
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
            throw new InvalidOperationException($"Invalid UTF-16 sequence at char index {charIndex}");
        }
        return charIndex + charsConsumed;
    }

    public static bool MoveNext(
        ReadOnlySpan<char> buffer,
        ref int nextCharIndex,
        ref int runePosition,
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
            throw new InvalidOperationException($"Invalid UTF-16 sequence at char index {nextCharIndex}");
        }
        nextCharIndex += charsConsumed;
        ++runePosition;
        return true;
    }
}