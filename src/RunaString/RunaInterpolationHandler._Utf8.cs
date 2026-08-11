using System.Buffers;
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
[InterpolatedStringHandler]
public ref struct RunaUtf8InterpolationHandler(int literalLength, int formattedCount, IFormatProvider? formatProvider)
{
    private readonly int _literalLength = literalLength;
    private readonly int _formattedCount = formattedCount;
    private readonly IFormatProvider? _formatProvider = formatProvider;
    private byte[] _buffer = ArrayPool<byte>.Shared.Rent(literalLength + formattedCount * 10);
    private int _length = 0;

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
        if(_buffer.Length < requiredLength)
        {
            requiredLength = (int)BitOperations.RoundUpToPowerOf2((uint)requiredLength);
            var newBuffer = ArrayPool<byte>.Shared.Rent(requiredLength);
            _buffer.AsSpan(0, _length).CopyTo(newBuffer);
            (_buffer, newBuffer) = (newBuffer, _buffer);
            ArrayPool<byte>.Shared.Return(newBuffer);
        }
    }

    /// <summary>
    /// Appends a literal string to the UTF-8 encoded string being built.
    /// </summary>
    /// <param name="s"></param>
    public void AppendLiteral(string s)
    {
        var bytesCount = Encoding.UTF8.GetByteCount(s);
        ReserveIfNeed(_length + bytesCount);
        Encoding.UTF8.GetBytes(s, _buffer.AsSpan(_length));
        _length += bytesCount;
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
        int alignment = 0,
        string? format = null,
        OverloadResolutionMarker? marker = null)
    {
        InternalHelpers.NoUse(marker);
        var s = string.Format(_formatProvider, $"{{0{(alignment != 0 ? $",{alignment}" : "")}{(format != null ? $":{format}" : "")}}}", value);
        AppendLiteral(s);
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
