using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RunaString;

/// <summary>
/// A struct that represents a seekable index into a UTF-16 encoded buffer for an <see cref="Utf16SpanEnumerator"/> or <see cref="Utf16MemoryEnumerator"/>.
/// </summary>
/// <remarks>
/// This instance is only for the source sequence which the instance was created from.
/// It is not valid for other sequences, even if they contain the same data.
/// The behavior is undefined if used with a different source sequence.
/// </remarks>
public struct Utf16RuneIndex : ISeekIndex<Utf16RuneIndex>
{
    /// <summary></summary>
    public int CharIndex { get; }

    /// <summary></summary>
    public int RuneIndex { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Utf16RuneIndex"/> struct with the specified byte index and rune position.
    /// </summary>
    /// <param name="charIndex"></param>
    /// <param name="runePosition"></param>
    internal Utf16RuneIndex(int charIndex, int runePosition)
    {
        CharIndex = charIndex;
        RuneIndex = runePosition;
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"Utf16RuneIndex {{ CharIndex = {CharIndex}, RuneIndex = {RuneIndex} }}";

    /// <inheritdoc />
    public override int GetHashCode() =>
        CharIndex.GetHashCode() ^ RuneIndex.GetHashCode();

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Utf16RuneIndex other && Equals(this, other);

    /// <inheritdoc />
    public bool Equals(Utf16RuneIndex other) => Equals(this, other);

    /// <inheritdoc />
    /// <exception cref="ArgumentException">
    /// There is an inconsistency between the byte indices and the rune indices.
    /// These instances may have been originated from different strings.
    /// </exception>
    public int CompareTo(Utf16RuneIndex other) => Compare(this, other);

    /// <inheritdoc />
    public static bool Equals(Utf16RuneIndex x, Utf16RuneIndex y) =>
        x.CharIndex == y.CharIndex && x.RuneIndex == y.RuneIndex;

    /// <inheritdoc />
    public static int Compare(Utf16RuneIndex x, Utf16RuneIndex y)
    {
        if (x.CharIndex == y.CharIndex && x.RuneIndex == y.RuneIndex)
        {
            return 0;
        }
        return (x.CharIndex < y.CharIndex, x.RuneIndex < y.RuneIndex) switch
        {
            (true, true) => -1,
            (false, false) => +1,
            _ => throw new ArgumentException($"Inconsistent source hash codes: x and y may be from different source strings."),
        };
    }

    /// <inheritdoc />
    public static bool operator ==(Utf16RuneIndex x, Utf16RuneIndex y) => Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(Utf16RuneIndex x, Utf16RuneIndex y) => !Equals(x, y);

    /// <inheritdoc />
    public static bool operator <(Utf16RuneIndex x, Utf16RuneIndex y) => Compare(x, y) < 0;

    /// <inheritdoc />
    public static bool operator >(Utf16RuneIndex x, Utf16RuneIndex y) => Compare(x, y) > 0;

    /// <inheritdoc />
    public static bool operator <=(Utf16RuneIndex x, Utf16RuneIndex y) => Compare(x, y) <= 0;

    /// <inheritdoc />
    public static bool operator >=(Utf16RuneIndex x, Utf16RuneIndex y) => Compare(x, y) >= 0;
}

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