using System.Text;

namespace RunaString;

/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a <see cref="Rune"/> buffer.
/// </summary>
public ref struct RunesSpanEnumerator
    : IRunaEnumerator<RunesSpanEnumerator, RunesIndex, ReadOnlySpan<Rune>>
{
    /// <inheritdoc />
    public static RunesSpanEnumerator Empty =>
        new([]);

    // NOTE: keep order to save size
    private readonly ReadOnlySpan<Rune> _buffer;
    private int _runeIndex = -1;
    private Rune _current = default;

    /// <inheritdoc />
    public readonly RunesIndex SeekIndex => new(_runeIndex);

    /// <inheritdoc />
    public readonly Rune Current => _current;

    /// <inheritdoc />
    public readonly ReadOnlySpan<Rune> SourceBuffer => _buffer;

    /// <inheritdoc />
    public readonly ReadOnlySpan<Rune> ConsumedBuffer => _buffer.Slice(0, Math.Max(_runeIndex + 1, _buffer.Length));

    /// <inheritdoc />
    public readonly ReadOnlySpan<Rune> RemainingBuffer => _buffer.Slice(Math.Max(_runeIndex + 1, _buffer.Length));

    private RunesSpanEnumerator(ReadOnlySpan<Rune> buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Creates a new <see cref="RunesSpanEnumerator"/> for the specified <see cref="Rune"/> buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static RunesSpanEnumerator Create(ReadOnlySpan<Rune> buffer) =>
        new(buffer);

    /// <inheritdoc />
    public bool MoveNext() =>
        Helpers.MoveNext(_buffer, ref _runeIndex, out _current);

    /// <inheritdoc />
    public RunesSpanEnumerator Seek(RunesIndex index) =>
        new (_buffer)
        {
            _runeIndex = index.RuneIndex,
        };
}


/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a <see cref="Rune"/> buffer.
/// </summary>
public struct RunesMemoryEnumerator
    : IRunaEnumerator<RunesMemoryEnumerator, RunesIndex, ReadOnlyMemory<Rune>>
{
    /// <inheritdoc />
    public static RunesMemoryEnumerator Empty { get; } =
        new(ReadOnlyMemory<Rune>.Empty);

    private readonly ReadOnlyMemory<Rune> _buffer;
    private int _runeIndex = -1;
    private Rune _current = default;

    /// <inheritdoc />
    public readonly RunesIndex SeekIndex => new(_runeIndex);

    /// <inheritdoc />
    public readonly Rune Current => _current;

    /// <inheritdoc />
    public readonly ReadOnlyMemory<Rune> SourceBuffer => _buffer;

    /// <inheritdoc />
    public readonly ReadOnlyMemory<Rune> ConsumedBuffer => _buffer.Slice(0, Math.Max(_runeIndex + 1, _buffer.Length));

    /// <inheritdoc />
    public readonly ReadOnlyMemory<Rune> RemainingBuffer => _buffer.Slice(Math.Max(_runeIndex + 1, _buffer.Length));

    private RunesMemoryEnumerator(ReadOnlyMemory<Rune> buffer)
    {
        _buffer = buffer;
    }

    /// <summary>
    /// Creates a new <see cref="RunesMemoryEnumerator"/> for the specified <see cref="Rune"/> buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static RunesMemoryEnumerator Create(ReadOnlyMemory<Rune> buffer) =>
        new(buffer);

    /// <inheritdoc />
    public bool MoveNext() =>
        Helpers.MoveNext(_buffer.Span, ref _runeIndex, out _current);

    /// <inheritdoc />
    public RunesMemoryEnumerator Seek(RunesIndex index) =>
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