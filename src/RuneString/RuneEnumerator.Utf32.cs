using System.Text;

namespace RuneString;

/// <summary>
/// A struct that represents a seekable index into a UTF-32 encoded buffer for an <see cref="Utf32SpanEnumerator"/> or <see cref="Utf32MemoryEnumerator"/>.
/// </summary>
/// <remarks>
/// This instance is only for the source sequence which the instance was created from.
/// It is not valid for other sequences, even if they contain the same data.
/// The behavior is undefined if used with a different source sequence.
/// </remarks>
public readonly struct Utf32RuneIndex : ISeekIndex
{
    /// <summary></summary>
    public int RuneIndex { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Utf32RuneIndex"/> struct with the specified byte index and rune position.
    /// </summary>
    /// <param name="runePosition"></param>
    internal Utf32RuneIndex(int runePosition)
    {
        RuneIndex = runePosition;
    }
}

/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a UTF-32 encoded buffer.
/// </summary>
public ref struct Utf32SpanEnumerator
    : IRuneEnumerator<Utf32SpanEnumerator, Utf32RuneIndex, ReadOnlySpan<Rune>>
{
    /// <inheritdoc />
    public static Utf32SpanEnumerator Empty =>
        new([]);

    // NOTE: keep order to save size
    private readonly ReadOnlySpan<Rune> _buffer;
    private int _runeIndex = -1;
    private Rune _current = default;

    /// <inheritdoc />
    public readonly Utf32RuneIndex SeekIndex => new(_runeIndex);

    /// <inheritdoc />
    public readonly Rune Current => _current;

    /// <inheritdoc />
    public readonly ReadOnlySpan<Rune> SourceBuffer => _buffer;

    /// <inheritdoc />
    public readonly ReadOnlySpan<Rune> ConsumedBuffer => _buffer.Slice(0, Math.Max(_runeIndex + 1, _buffer.Length));

    /// <inheritdoc />
    public readonly ReadOnlySpan<Rune> RemainingBuffer => _buffer.Slice(Math.Max(_runeIndex + 1, _buffer.Length));

    private Utf32SpanEnumerator(ReadOnlySpan<Rune> buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Creates a new <see cref="Utf32SpanEnumerator"/> for the specified UTF-32 encoded buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static Utf32SpanEnumerator Create(ReadOnlySpan<Rune> buffer) =>
        new(buffer);

    /// <inheritdoc />
    public bool MoveNext() =>
        Helpers.MoveNext(_buffer, ref _runeIndex, out _current);

    /// <inheritdoc />
    public Utf32SpanEnumerator Seek(Utf32RuneIndex index) =>
        new (_buffer)
        {
            _runeIndex = index.RuneIndex,
        };
}


/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a UTF-32 encoded buffer.
/// </summary>
public struct Utf32MemoryEnumerator
    : IRuneEnumerator<Utf32MemoryEnumerator, Utf32RuneIndex, ReadOnlyMemory<Rune>>
{
    /// <inheritdoc />
    public static Utf32MemoryEnumerator Empty { get; } =
        new(ReadOnlyMemory<Rune>.Empty);

    private readonly ReadOnlyMemory<Rune> _buffer;
    private int _runeIndex = -1;
    private Rune _current = default;

    /// <inheritdoc />
    public readonly Utf32RuneIndex SeekIndex => new(_runeIndex);

    /// <inheritdoc />
    public readonly Rune Current => _current;

    /// <inheritdoc />
    public readonly ReadOnlyMemory<Rune> SourceBuffer => _buffer;

    /// <inheritdoc />
    public readonly ReadOnlyMemory<Rune> ConsumedBuffer => _buffer.Slice(0, Math.Max(_runeIndex + 1, _buffer.Length));

    /// <inheritdoc />
    public readonly ReadOnlyMemory<Rune> RemainingBuffer => _buffer.Slice(Math.Max(_runeIndex + 1, _buffer.Length));

    private Utf32MemoryEnumerator(ReadOnlyMemory<Rune> buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Creates a new <see cref="Utf32MemoryEnumerator"/> for the specified UTF-32 encoded buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static Utf32MemoryEnumerator Create(ReadOnlyMemory<Rune> buffer) =>
        new(buffer);

    /// <inheritdoc />
    public bool MoveNext() =>
        Helpers.MoveNext(_buffer.Span, ref _runeIndex, out _current);

    /// <inheritdoc />
    public Utf32MemoryEnumerator Seek(Utf32RuneIndex index) =>
        new(_buffer)
        {
            _runeIndex = index.RuneIndex,
        };
}


file static class Helpers
{
    public static bool MoveNext(
        ReadOnlySpan<Rune> buffer,
        ref int runeIndex,
        out Rune current)
    {
        ++runeIndex;
        if (runeIndex >= buffer.Length)
        {
            current = default;
            return false;
        }
        current = buffer[runeIndex];
        return true;
    }
}