using System.Buffers;
using System.Text;

namespace RunaString;

/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a UTF-8 encoded buffer.
/// </summary>
public ref struct Utf8SpanEnumerator
    : IRuneEnumerator<Utf8SpanEnumerator, Utf8Index, ReadOnlySpan<byte>>
{
    /// <inheritdoc />
    public static Utf8SpanEnumerator Empty =>
        new([]);

    // NOTE: keep order to save size
    private int _currByteIndex = -1;
    private int _nextByteIndex = 0;
    private int _runePosition = -1;
    private Rune _current = default;
    private readonly ReadOnlySpan<byte> _utf8Buffer;

    internal readonly int NextByteIndex => _nextByteIndex;

    /// <inheritdoc />
    public Utf8Index SeekIndex => new(_currByteIndex, _runePosition);

    /// <inheritdoc />
    public Rune Current => _current;

    /// <inheritdoc />
    public ReadOnlySpan<byte> SourceBuffer => _utf8Buffer;

    /// <inheritdoc />
    public ReadOnlySpan<byte> ConsumedBuffer => _utf8Buffer.Slice(0, _nextByteIndex);

    /// <inheritdoc />
    public ReadOnlySpan<byte> RemainingBuffer => _utf8Buffer.Slice(_nextByteIndex);

    private Utf8SpanEnumerator(ReadOnlySpan<byte> utf8Buffer)
    {
        _utf8Buffer = utf8Buffer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Utf8SpanEnumerator"/> struct with the specified UTF-8 string.
    /// </summary>
    /// <param name="buffer"></param>
    public Utf8SpanEnumerator(Utf8Span buffer)
        : this(buffer.Buffer)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Utf8SpanEnumerator"/> struct with the specified UTF-8 buffer.
    /// </summary>
    /// <param name="utf8Buffer"></param>
    /// <returns></returns>
    public static Utf8SpanEnumerator Create(ReadOnlySpan<byte> utf8Buffer)
    {
        InternalHelpers.ValidateUtf8(utf8Buffer);
        return new (utf8Buffer);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Utf8SpanEnumerator"/> struct with the specified UTF-8 buffer without validating the UTF-8 data.
    /// </summary>
    /// <param name="utf8Buffer"></param>
    /// <returns></returns>
    public static Utf8SpanEnumerator DangerousCreate(ReadOnlySpan<byte> utf8Buffer)
        => new(utf8Buffer);

    /// <inheritdoc />
    public bool MoveNext()
    {
        _currByteIndex = _nextByteIndex;
        return Helpers.MoveNext(_utf8Buffer, ref _nextByteIndex, ref _runePosition, out _current);
    }

    /// <inheritdoc />
    public Utf8SpanEnumerator Seek(Utf8Index index) =>
        new(_utf8Buffer)
    {
        _currByteIndex = index.ByteIndex,
        _nextByteIndex = Helpers.GetNextByteIndex(_utf8Buffer, index.ByteIndex),
        _runePosition = index.RuneIndex,
    };
}


/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a UTF-8 encoded buffer.
/// </summary>
public struct Utf8MemoryEnumerator
    : IRuneEnumerator<Utf8MemoryEnumerator, Utf8Index, ReadOnlyMemory<byte>>
{
    /// <inheritdoc />
    public static Utf8MemoryEnumerator Empty { get; } =
        new(ReadOnlyMemory<byte>.Empty);

    // NOTE: keep order to save size
    private int _currByteIndex = -1;
    private int _nextByteIndex = 0;
    private int _runePosition = -1;
    private Rune _current = default;
    private readonly ReadOnlyMemory<byte> _utf8Buffer;

    internal readonly int NextByteIndex => _nextByteIndex;

    /// <inheritdoc />
    public Utf8Index SeekIndex => new(_currByteIndex, _runePosition);

    /// <inheritdoc />
    public Rune Current => _current;

    /// <inheritdoc />
    public ReadOnlyMemory<byte> SourceBuffer => _utf8Buffer;

    /// <inheritdoc />
    public ReadOnlyMemory<byte> ConsumedBuffer => _utf8Buffer.Slice(0, _nextByteIndex);

    /// <inheritdoc />
    public ReadOnlyMemory<byte> RemainingBuffer => _utf8Buffer.Slice(_nextByteIndex);

    private Utf8MemoryEnumerator(ReadOnlyMemory<byte> utf8Buffer)
    {
        _utf8Buffer = utf8Buffer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Utf8MemoryEnumerator"/> struct with the specified UTF-8 string.
    /// </summary>
    /// <param name="buffer"></param>
    public Utf8MemoryEnumerator(Utf8String buffer)
        : this(buffer.Buffer)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Utf8MemoryEnumerator"/> struct with the specified UTF-8 buffer.
    /// </summary>
    /// <param name="utf8Buffer"></param>
    /// <returns></returns>
    public static Utf8MemoryEnumerator Create(ReadOnlyMemory<byte> utf8Buffer)
    {
        InternalHelpers.ValidateUtf8(utf8Buffer.Span);
        return new (utf8Buffer);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Utf8MemoryEnumerator"/> struct with the specified UTF-8 buffer without validating the UTF-8 data.
    /// </summary>
    /// <param name="utf8Buffer"></param>
    /// <returns></returns>
    public static Utf8MemoryEnumerator DangerousCreate(ReadOnlyMemory<byte> utf8Buffer)
        => new(utf8Buffer);

    /// <inheritdoc />
    public bool MoveNext()
    {
        _currByteIndex = _nextByteIndex;
        return Helpers.MoveNext(_utf8Buffer.Span, ref _nextByteIndex, ref _runePosition, out _current);
    }

    /// <inheritdoc />
    public Utf8MemoryEnumerator Seek(Utf8Index index) =>
        new(_utf8Buffer)
        {
            _currByteIndex = index.ByteIndex,
            _nextByteIndex = Helpers.GetNextByteIndex(_utf8Buffer.Span, index.ByteIndex),
            _runePosition = index.RuneIndex,
        };
}


file static class Helpers
{
    public static int GetNextByteIndex(ReadOnlySpan<byte> utf8Buffer, int currentByteIndex)
    {
        if (currentByteIndex >= utf8Buffer.Length)
        {
            return -1;
        }
        var status = Rune.DecodeFromUtf8(utf8Buffer.Slice(currentByteIndex), out _, out var bytesConsumed);
        if (status != OperationStatus.Done)
        {
            throw new InvalidOperationException($"Invalid UTF-8 sequence at byte index {currentByteIndex}");
        }
        return currentByteIndex + bytesConsumed;
    }

    public static bool MoveNext(
        ReadOnlySpan<byte> utf8Buffer,
        ref int nextByteIndex,
        ref int runePosition,
        out Rune current)
    {
        if (nextByteIndex >= utf8Buffer.Length)
        {
            current = default;
            return false;
        }
        var status = Rune.DecodeFromUtf8(utf8Buffer.Slice(nextByteIndex), out current, out var bytesConsumed);
        if (status != OperationStatus.Done)
        {
            throw new InvalidOperationException($"Invalid UTF-8 sequence at byte index {nextByteIndex}");
        }
        nextByteIndex += bytesConsumed;
        ++runePosition;
        return true;
    }
}