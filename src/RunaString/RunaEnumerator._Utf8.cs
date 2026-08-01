using System.Text;

namespace RunaString;

/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a UTF-8 encoded buffer.
/// </summary>
public ref struct Utf8SpanEnumerator
    : IRunaEnumerator<Utf8SpanEnumerator, Utf8Index, ReadOnlySpan<byte>>
{
    /// <inheritdoc />
    public static Utf8SpanEnumerator Empty =>
        new([]);

    // NOTE: keep order to save size
    private int _currByteIndex = -1;
    private int _nextByteIndex = 0;
    private int _nextRuneIndex = 0;
    private Rune _current = default;
    private readonly ReadOnlySpan<byte> _utf8Buffer;

    internal readonly int NextByteIndex => _nextByteIndex;

    /// <inheritdoc />
    public readonly Utf8Index SeekIndex => new(_currByteIndex, _nextRuneIndex - 1);

    /// <inheritdoc />
    public readonly Rune Current => _current;

    /// <inheritdoc />
    public readonly ReadOnlySpan<byte> SourceBuffer => _utf8Buffer;

    /// <inheritdoc />
    public readonly ReadOnlySpan<byte> ConsumedBuffer => _utf8Buffer.Slice(0, _nextByteIndex);

    /// <inheritdoc />
    public readonly ReadOnlySpan<byte> RemainingBuffer => _utf8Buffer.Slice(_nextByteIndex);

    private Utf8SpanEnumerator(ReadOnlySpan<byte> utf8Buffer)
    {
        _utf8Buffer = utf8Buffer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Utf8SpanEnumerator"/> struct with the specified UTF-8 string.
    /// </summary>
    /// <param name="buffer"></param>
    public Utf8SpanEnumerator(Utf8SpanString buffer)
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
        return new(utf8Buffer);
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
        return Utf8Helpers.UnsafeTryGetRuneAndMoveNext(_utf8Buffer, ref _nextByteIndex, ref _nextRuneIndex, out _current);
    }

    /// <inheritdoc />
    public readonly Utf8SpanEnumerator Seek(Utf8Index index)
    {
        var byteIndex = index.ByteIndex;
        var runeIndex = index.RuneIndex;
        Utf8Helpers.UnsafeTryGetRuneAndMoveNext(_utf8Buffer, ref byteIndex, ref runeIndex, out var current);
        return new(_utf8Buffer)
        {
            _currByteIndex = index.ByteIndex,
            _nextByteIndex = byteIndex,
            _nextRuneIndex = runeIndex,
            _current = current,
        };
    }
}


/// <summary>
/// An enumerator that iterates over Unicode scalar values (runes) in a UTF-8 encoded buffer.
/// </summary>
public struct Utf8MemoryEnumerator
    : IRunaEnumerator<Utf8MemoryEnumerator, Utf8Index, ReadOnlyMemory<byte>>
{
    /// <inheritdoc />
    public static Utf8MemoryEnumerator Empty { get; } =
        new(ReadOnlyMemory<byte>.Empty);

    // NOTE: keep order to save size
    private int _currByteIndex = -1;
    private int _nextByteIndex = 0;
    private int _nextRuneIndex = 0;
    private Rune _current = default;
    private readonly ReadOnlyMemory<byte> _utf8Buffer;

    internal readonly int NextByteIndex => _nextByteIndex;

    /// <inheritdoc />
    public readonly Utf8Index SeekIndex => new(_currByteIndex, _nextRuneIndex - 1);

    /// <inheritdoc />
    public readonly Rune Current => _current;

    /// <inheritdoc />
    public readonly ReadOnlyMemory<byte> SourceBuffer => _utf8Buffer;

    /// <inheritdoc />
    public readonly ReadOnlyMemory<byte> ConsumedBuffer => _utf8Buffer.Slice(0, _nextByteIndex);

    /// <inheritdoc />
    public readonly ReadOnlyMemory<byte> RemainingBuffer => _utf8Buffer.Slice(_nextByteIndex);

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
        return new(utf8Buffer);
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
        return Utf8Helpers.UnsafeTryGetRuneAndMoveNext(_utf8Buffer.Span, ref _nextByteIndex, ref _nextRuneIndex, out _current);
    }

    /// <inheritdoc />
    public readonly Utf8MemoryEnumerator Seek(Utf8Index index)
    {
        var byteIndex = index.ByteIndex;
        var runeIndex = index.RuneIndex;
        Utf8Helpers.UnsafeTryGetRuneAndMoveNext(_utf8Buffer.Span, ref byteIndex, ref runeIndex, out var current);
        return new(_utf8Buffer)
        {
            _currByteIndex = index.ByteIndex,
            _nextByteIndex = byteIndex,
            _nextRuneIndex = runeIndex,
            _current = current,
        };
    }
}
