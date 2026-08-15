using System.Buffers;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace RunaString;

/// <summary>
/// A custom interpolated string handler for building UTF-8 encoded strings.
/// </summary>
/// <param name="literalLength"></param>
/// <param name="formattedCount"></param>
/// <param name="formatProvider"></param>
/// <remarks>
/// `alignment` of this handler is based on the number of Unicode code points (runes) rather than the number of bytes or chars.
/// </remarks>
[InterpolatedStringHandler]
public ref struct RunaUtf8InterpolationHandler(int literalLength, int formattedCount, IFormatProvider? formatProvider)
{
    internal const int StackBufferSize = 256;
    internal const int HeapBufferSize = 2048;

    private readonly int _literalLength = literalLength;
    private readonly int _formattedCount = formattedCount;
    private readonly IFormatProvider? _formatProvider = formatProvider;
    private byte[] _buffer = ArrayPool<byte>.Shared.Rent(literalLength + formattedCount * 10);
    private int _length = 0;


    /// <summary>
    /// This constructor is not supported and will throw a NotSupportedException.
    /// Use the constructor with parameters instead.
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    [Obsolete("Don't use default constructor", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public RunaUtf8InterpolationHandler()
        : this(0, 0, null)
    {
        throw new NotSupportedException("Do not use default constructor.");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RunaUtf8InterpolationHandler"/> struct with the specified literal length and formatted count.
    /// </summary>
    /// <param name="literalLength"></param>
    /// <param name="formattedCount"></param>
    public RunaUtf8InterpolationHandler(int literalLength, int formattedCount)
        : this(literalLength, formattedCount, null)
    {
    }

    private void ReserveIfNeed(int requiredLength)
    {
        if (_buffer.Length < requiredLength)
        {
            requiredLength = (int)BitOperations.RoundUpToPowerOf2((uint)requiredLength);
            var newBuffer = ArrayPool<byte>.Shared.Rent(requiredLength);
            _buffer.AsSpan(0, _length).CopyTo(newBuffer);
            (_buffer, newBuffer) = (newBuffer, _buffer);
            ArrayPool<byte>.Shared.Return(newBuffer);
        }
    }

    private void AppendCore(scoped ReadOnlySpan<char> s, int alignment)
    {
        var bytesCount = Encoding.UTF8.GetByteCount(s);
        var runeCount = CharHelpers.GetRuneCount(s);
        var xalign = Math.Abs(alignment);
        if (xalign < runeCount)
        {
            ReserveIfNeed(_length + bytesCount);
            Encoding.UTF8.GetBytes(s, _buffer.AsSpan(_length));
            _length += bytesCount;
        }
        else
        {
            AppendWithPadCore(s, bytesCount, xalign - runeCount, alignment > 0);
        }
    }

    private void AppendCore(scoped ReadOnlySpan<byte> bytes, int alignment)
    {
        var bytesWritten = bytes.Length;
        var runeCount = Utf8Helpers.GetRuneCount(bytes);
        var xalign = Math.Abs(alignment);
        if (xalign < runeCount)
        {
            ReserveIfNeed(_length + bytesWritten);
            bytes.CopyTo(_buffer.AsSpan(_length));
            _length += bytesWritten;
        }
        else
        {
            AppendWithPad(bytes, xalign - runeCount, alignment > 0);
        }
    }

    private void AppendWithPadCore(scoped ReadOnlySpan<char> s, int bytesCount, int spaceSize, bool padLeft)
    {
        var newLength = _length + bytesCount + spaceSize;
        ReserveIfNeed(newLength);
        if (padLeft)
        {
            _buffer.AsSpan(_length, spaceSize).Fill((byte)' ');
            Encoding.UTF8.GetBytes(s, _buffer.AsSpan(_length + spaceSize));
        }
        else
        {
            Encoding.UTF8.GetBytes(s, _buffer.AsSpan(_length, bytesCount));
            _buffer.AsSpan(_length + bytesCount, spaceSize).Fill((byte)' ');
        }
        _length = newLength;
    }

    private void AppendWithPad(scoped ReadOnlySpan<byte> bytes, int spaceSize, bool padLeft)
    {
        var newLength = _length + bytes.Length + spaceSize;
        ReserveIfNeed(newLength);
        if (padLeft)
        {
            _buffer.AsSpan(_length, spaceSize).Fill((byte)' ');
            bytes.CopyTo(_buffer.AsSpan(_length + spaceSize));
        }
        else
        {
            bytes.CopyTo(_buffer.AsSpan(_length));
            _buffer.AsSpan(_length + bytes.Length, spaceSize).Fill((byte)' ');
        }
        _length = newLength;
    }

    /// <summary>
    /// Appends a literal string to the UTF-8 encoded string being built.
    /// </summary>
    /// <param name="s"></param>
    public void AppendLiteral(string s) => AppendCore(s, 0);

    /// <summary>
    /// Appends a formatted value to the UTF-8 encoded string being built.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="alignment"></param>
    /// <param name="format"></param>
    /// <param name="marker"></param>
    [OverloadResolutionPriority(0)]
    public void AppendFormatted<T>(
        T value,
        int alignment = 0,
        string? format = null,
        OverloadResolutionMarker? marker = null)
    {
        InternalHelpers.NoUse(marker);
        var s = string.Format(_formatProvider, $"{{0{(format != null ? $":{format}" : "")}}}", value);
        AppendCore(s, alignment);
    }

    /// <inheritdoc cref="AppendFormatted{T}(T, int, string?, OverloadResolutionMarker?)"/>
    [OverloadResolutionPriority(1)]
    public void AppendFormatted<T>(
        T value,
        int alignment = 0,
        string? format = null,
        OverloadResolutionMarker.AssignableFrom<IFormattable>? marker = null)
        where T : IFormattable
    {
        InternalHelpers.NoUse(marker);
        var s = value.ToString(format, _formatProvider);
        AppendCore(s, alignment);
    }


    /// <inheritdoc cref="AppendFormatted{T}(T, int, string?, OverloadResolutionMarker?)"/>
    [OverloadResolutionPriority(2)]
    public void AppendFormatted<T>(
        T value,
        int alignment = 0,
        string? format = null,
        OverloadResolutionMarker.AssignableFrom<ISpanFormattable>? marker = null)
        where T : ISpanFormattable
    {
        InternalHelpers.NoUse(marker);
        var stackBuffer = (stackalloc char[StackBufferSize / sizeof(char)]);
        if (value.TryFormat(stackBuffer, out var charsWritten, format, _formatProvider))
        {
            AppendCore(stackBuffer.Slice(0, charsWritten), alignment);
            return;
        }

        var heapBuffer = default(char[]);
        try
        {
            heapBuffer = ArrayPool<char>.Shared.Rent(HeapBufferSize / sizeof(char));
            if (value.TryFormat(heapBuffer, out charsWritten, format, _formatProvider))
            {
                AppendCore(heapBuffer.AsSpan(0, charsWritten), alignment);
                return;
            }
        }
        finally
        {
            if (heapBuffer is { })
            {
                ArrayPool<char>.Shared.Return(heapBuffer);
            }
        }

        AppendFormatted(value, alignment, format, (OverloadResolutionMarker.AssignableFrom<IFormattable>?)null);
    }


    /// <inheritdoc cref="AppendFormatted{T}(T, int, string?, OverloadResolutionMarker?)"/>
    [OverloadResolutionPriority(3)]
    public void AppendFormatted<T>(
        T value,
        int alignment = 0,
        string? format = null,
        OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>? marker = null)
        where T : IUtf8SpanFormattable
    {
        InternalHelpers.NoUse(marker);
        var stackBuffer = (stackalloc byte[StackBufferSize]);
        if (value.TryFormat(stackBuffer, out var bytesWritten, format, _formatProvider))
        {
            AppendCore(stackBuffer.Slice(0, bytesWritten), alignment);
            return;
        }

        var heapBuffer = default(byte[]);
        try
        {
            heapBuffer = ArrayPool<byte>.Shared.Rent(HeapBufferSize);
            if (value.TryFormat(heapBuffer, out bytesWritten, format, _formatProvider))
            {
                AppendCore(heapBuffer.AsSpan(0, bytesWritten), alignment);
                return;
            }
        }
        finally
        {
            if(heapBuffer is { })
            {
                ArrayPool<byte>.Shared.Return(heapBuffer);
            }
        }

        if(value is IFormattable formattable)
        {
            AppendFormatted(formattable, alignment, format);
            return;
        }

        AppendFormatted(value, alignment, format, (OverloadResolutionMarker?)null);
    }

    /// <summary>
    /// Completes the construction of the UTF-8 encoded string and returns it as a Utf8String.
    /// </summary>
    /// <returns></returns>
    public Utf8String MoveToUtf8String()
    {
        // DO NOT returns `_buffer` to ArrayPool<byte>.Shared,
        // as it is now owned by the Utf8String instance.
        var memory = _buffer.AsMemory(0, _length);
        _buffer = [];
        return Utf8String.DangerousFromUtf8(memory);
    }

    /// <summary>
    /// Completes the construction of the UTF-8 encoded string and returns it as a Utf8SpanString.
    /// </summary>
    /// <returns></returns>
    public Utf8SpanString MoveToUtf8SpanString()
    {
        // DO NOT returns `_buffer` to ArrayPool<byte>.Shared,
        // as it is now owned by the Utf8SpanString instance.
        var span = _buffer.AsSpan(0, _length);
        _buffer = [];
        return Utf8SpanString.DangerousFromSpan(span);
    }
}
