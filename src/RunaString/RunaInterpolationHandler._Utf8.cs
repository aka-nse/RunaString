using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
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

    private readonly Span<byte> Destination => _buffer.AsSpan(_length);


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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ExtendDouble() => Extend(2 * _buffer.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Extend(int requiredLength)
    {
        Debug.Assert(_buffer.Length < requiredLength);
        requiredLength = (int)BitOperations.RoundUpToPowerOf2((uint)requiredLength);
        var newBuffer = ArrayPool<byte>.Shared.Rent(requiredLength);
        _buffer.AsSpan(0, _length).CopyTo(newBuffer);
        (_buffer, newBuffer) = (newBuffer, _buffer);
        ArrayPool<byte>.Shared.Return(newBuffer);
    }


    private void ExtendIfNeed(int appendLength)
    {
        var newLength = _length + appendLength;
        if (_buffer.Length < newLength)
        {
            Extend(newLength);
        }
    }


    private void AppendCore(scoped ReadOnlySpan<char> s, int alignment)
    {
        if (alignment == 0)
        {
            AppendWithoutPad(s);
            return;
        }

        var runeCount = CharHelpers.GetRuneCount(s);
        var xalign = Math.Abs(alignment);
        if (xalign < runeCount)
        {
            AppendWithoutPad(s);
        }
        else
        {
            var bytesCount = Encoding.UTF8.GetByteCount(s);
            AppendWithPad(s, bytesCount, xalign - runeCount, alignment > 0);
        }
    }


    private void AppendCore(scoped ReadOnlySpan<byte> bytes, int alignment)
    {
        if (alignment == 0)
        {
            AppendWithoutPad(bytes);
            return;
        }

        var runeCount = Utf8Helpers.GetRuneCount(bytes);
        var xalign = Math.Abs(alignment);
        if (xalign < runeCount)
        {
            AppendWithoutPad(bytes);
        }
        else
        {
            AppendWithPad(bytes, xalign - runeCount, alignment > 0);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendAscii(scoped ReadOnlySpan<char> s)
    {
        var length = s.Length;
        var us = MemoryMarshal.Cast<char, ushort>(s);
        ExtendIfNeed(length);
        var dest = Destination;
        if (Vector256.IsHardwareAccelerated)
        {
            while (us.Length > 2 * Vector256<ushort>.Count)
            {
                var v1 = Vector256.LoadUnsafe(in us[0]);
                us = us[Vector256<ushort>.Count..];
                var v2 = Vector256.LoadUnsafe(in us[0]);
                us = us[Vector256<ushort>.Count..];
                var vdst = Vector256.Narrow(v1, v2);
                vdst.CopyTo(dest);
                dest = dest[Vector256<byte>.Count..];
            }
        }
        if (Vector128.IsHardwareAccelerated)
        {
            while (us.Length > 2 * Vector128<ushort>.Count)
            {
                var v1 = Vector128.LoadUnsafe(in us[0]);
                us = us[Vector128<ushort>.Count..];
                var v2 = Vector128.LoadUnsafe(in us[0]);
                us = us[Vector128<ushort>.Count..];
                var vdst = Vector128.Narrow(v1, v2);
                vdst.CopyTo(dest);
                dest = dest[Vector128<byte>.Count..];
            }
        }
        if (Vector64.IsHardwareAccelerated)
        {
            while (us.Length > 2 * Vector64<ushort>.Count)
            {
                var v1 = Vector64.LoadUnsafe(in us[0]);
                us = us[Vector64<ushort>.Count..];
                var v2 = Vector64.LoadUnsafe(in us[0]);
                us = us[Vector64<ushort>.Count..];
                var vdst = Vector64.Narrow(v1, v2);
                vdst.CopyTo(dest);
                dest = dest[Vector64<byte>.Count..];
            }
        }
        for (var i = 0; i < us.Length; i++)
        {
            dest[i] = (byte)us[i];
        }
        _length += length;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendWithoutPad(scoped ReadOnlySpan<char> s)
    {
        var i = 0;
        while (i < s.Length)
        {
            if (MemoryMarshal.Cast<char, byte>(s) is { Length: >= 8 } us)
            {
                unsafe
                {
                    if ((Unsafe.ReadUnaligned<ulong>(ref Unsafe.AsRef(in us[0])) & 0xFF80_FF80_FF80_FF80uL) == 0)
                    {
                        i += 8;
                        continue;
                    }
                }
            }

            if (char.IsAscii(s[i]))
            {
                ++i;
                continue;
            }
            else if (i > 0)
            {
                AppendAscii(s[..i]);
                s = s[i..];
                i = 0;
            }

            // encode non-ASCII character to UTF-8
            var rune = new Rune(s[0]);
            if (!rune.TryEncodeToUtf8(Destination, out var bytesWritten))
            {
                ExtendDouble();
                bytesWritten = rune.EncodeToUtf8(Destination);
            }
            _length += bytesWritten;
            s = s[1..];
        }

        if (i > 0)
        {
            AppendAscii(s);
        }
    }


    private void AppendWithoutPad(scoped ReadOnlySpan<byte> bytes)
    {
        ExtendIfNeed(bytes.Length);
        bytes.CopyTo(Destination);
        _length += bytes.Length;
    }


    private void AppendWithPad(scoped ReadOnlySpan<char> s, int bytesCount, int spaceSize, bool padLeft)
    {
        var appendLength = bytesCount + spaceSize;
        ExtendIfNeed(appendLength);
        var dest = Destination;
        if (padLeft)
        {
            dest[..spaceSize].Fill((byte)' ');
            Encoding.UTF8.GetBytes(s, dest[spaceSize..]);
        }
        else
        {
            Encoding.UTF8.GetBytes(s, dest[..bytesCount]);
            dest[bytesCount..].Fill((byte)' ');
        }
        _length += appendLength;
    }


    private void AppendWithPad(scoped ReadOnlySpan<byte> bytes, int spaceSize, bool padLeft)
    {
        var appendLength = bytes.Length + spaceSize;
        ExtendIfNeed(appendLength);
        var dest = Destination;
        if (padLeft)
        {
            dest[..spaceSize].Fill((byte)' ');
            bytes.CopyTo(dest[spaceSize..]);
        }
        else
        {
            bytes.CopyTo(dest[..bytes.Length]);
            dest[bytes.Length..].Fill((byte)' ');
        }
        _length += appendLength;
    }


    /// <summary>
    /// Appends a literal string to the UTF-8 encoded string being built.
    /// </summary>
    /// <param name="s"></param>
    public void AppendLiteral(string s) => AppendWithoutPad(s);


    /// <inheritdoc cref="AppendFormatted{T}(T, int, string?, OverloadResolutionMarker?)"/>
    [OverloadResolutionPriority(0)]
    public void AppendFormatted<T>(
        T value,
        string? format = null,
        OverloadResolutionMarker? marker = null)
    {
        InternalHelpers.NoUse(marker);
        string s;
        if (_formatProvider is not ICustomFormatter)
        {
            s = value?.ToString() ?? "";
        }
        else
        {
            s = format is { }
                ? string.Format(_formatProvider, $"{{0:{format}}}", value)
                : string.Format(_formatProvider, "{0}", value);
        }
        AppendWithoutPad(s);
    }


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
        int alignment,
        string? format = null,
        OverloadResolutionMarker? marker = null)
    {
        InternalHelpers.NoUse(marker);
        string s;
        if (_formatProvider is not ICustomFormatter)
        {
            s = value?.ToString() ?? "";
        }
        else
        {
            s = format is { }
                ? string.Format(_formatProvider, $"{{0:{format}}}", value)
                : string.Format(_formatProvider, "{0}", value);
        }

        if (alignment == 0)
        {
            AppendWithoutPad(s);
        }
        else
        {
            AppendCore(s, alignment);
        }
    }


    /// <inheritdoc cref="AppendFormatted{T}(T, int, string?, OverloadResolutionMarker?)"/>
    [OverloadResolutionPriority(1)]
    public void AppendFormatted<T>(
        T value,
        string? format = null,
        OverloadResolutionMarker.AssignableFrom<IFormattable>? marker = null)
        where T : IFormattable
    {
        InternalHelpers.NoUse(marker);
        var s = value.ToString(format, _formatProvider);
        AppendWithoutPad(s);
    }


    /// <inheritdoc cref="AppendFormatted{T}(T, int, string?, OverloadResolutionMarker?)"/>
    [OverloadResolutionPriority(1)]
    public void AppendFormatted<T>(
        T value,
        int alignment,
        string? format = null,
        OverloadResolutionMarker.AssignableFrom<IFormattable>? marker = null)
        where T : IFormattable
    {
        InternalHelpers.NoUse(marker);
        var s = value.ToString(format, _formatProvider);
        if (alignment == 0)
        {
            AppendWithoutPad(s);
        }
        else
        {
            AppendCore(s, alignment);
        }
    }


    /// <inheritdoc cref="AppendFormatted{T}(T, int, string?, OverloadResolutionMarker?)"/>
    [OverloadResolutionPriority(2)]
    public void AppendFormatted<T>(
        T value,
        string? format = null,
        OverloadResolutionMarker.AssignableFrom<ISpanFormattable>? marker = null)
        where T : ISpanFormattable
    {
        InternalHelpers.NoUse(marker);
        var stackBuffer = (stackalloc char[StackBufferSize / sizeof(char)]);
        if (value.TryFormat(stackBuffer, out var charsWritten, format, _formatProvider))
        {
            AppendWithoutPad(stackBuffer.Slice(0, charsWritten));
            return;
        }

        var heapBuffer = default(char[]);
        try
        {
            heapBuffer = ArrayPool<char>.Shared.Rent(HeapBufferSize / sizeof(char));
            if (value.TryFormat(heapBuffer, out charsWritten, format, _formatProvider))
            {
                AppendWithoutPad(heapBuffer.AsSpan(0, charsWritten));
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

        AppendFormatted(value, format, (OverloadResolutionMarker.AssignableFrom<IFormattable>?)null);
    }


    /// <inheritdoc cref="AppendFormatted{T}(T, int, string?, OverloadResolutionMarker?)"/>
    [OverloadResolutionPriority(2)]
    public void AppendFormatted<T>(
        T value,
        int alignment,
        string? format = null,
        OverloadResolutionMarker.AssignableFrom<ISpanFormattable>? marker = null)
        where T : ISpanFormattable
    {
        InternalHelpers.NoUse(marker);
        if (alignment == 0)
        {
            AppendFormatted(value, format, marker);
            return;
        }

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
        string? format = null,
        OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>? marker = null)
        where T : IUtf8SpanFormattable
    {
        InternalHelpers.NoUse(marker);
        var i = 0;
        while (true)
        {
            if (value.TryFormat(Destination, out var bytesWritten, format, _formatProvider))
            {
                _length += bytesWritten;
                return;
            }
            if (i >= 10)
            {
                throw new InvalidOperationException($"Failed to format the value of type {typeof(T)} after 10 attempts.");
            }
            i++;
            ExtendDouble();
        }
    }


    /// <inheritdoc cref="AppendFormatted{T}(T, int, string?, OverloadResolutionMarker?)"/>
    [OverloadResolutionPriority(3)]
    public void AppendFormatted<T>(
        T value,
        int alignment,
        string? format = null,
        OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>? marker = null)
        where T : IUtf8SpanFormattable
    {
        InternalHelpers.NoUse(marker);
        var stackBuffer = (stackalloc byte[StackBufferSize]);
        if (value.TryFormat(stackBuffer, out var bytesWritten, format, _formatProvider))
        {
            AppendCore(stackBuffer[..bytesWritten], alignment);
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
            if (heapBuffer is { })
            {
                ArrayPool<byte>.Shared.Return(heapBuffer);
            }
        }

        if (value is IFormattable formattable)
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
