using System.Buffers;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RunaString;

/// <summary>
/// Represents an immutable UTF-8 encoded string.
/// </summary>
[RuneEnumerable]
public readonly partial struct Utf8String
    : IRuneEnumerable<Utf8String, Utf8MemoryEnumerator, Utf8RuneIndex>
    , IComparable<Utf8String>
    , IEquatable<Utf8String>
{
    #region source generated members

    public partial Rune this[Utf8RuneIndex index] { get; }
    public partial bool TryGetRune(Utf8RuneIndex index, out Rune rune);

    #endregion

    private readonly int _byteStart;

    private readonly int _byteLength;

    private readonly ImmutableArray<byte> _buffer;

    /// <summary>
    /// Gets a read-only memory of bytes representing the UTF-8 encoded string.
    /// </summary>
    public ReadOnlyMemory<byte> Buffer => _buffer.AsMemory().Slice(_byteStart, _byteLength);

    /// <summary>
    /// Gets the length of the UTF-8 encoded string in bytes.
    /// </summary>
    public int BufferLength => _byteLength;

    private Utf8String(ImmutableArray<byte> buffer, int start, int length)
    {
        _buffer = buffer;
        _byteStart = start;
        _byteLength = length;
    }

    /// <summary>
    /// Creates a new <see cref="Utf8String" /> from a given UTF-8 encoded byte buffer, starting at the specified index and with the specified length.
    /// </summary>
    /// <param name="utf8Buffer"></param>
    /// <param name="byteStart"></param>
    /// <param name="byteLength"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException" />
    public static Utf8String FromUtf8(ImmutableArray<byte> utf8Buffer, int byteStart, int byteLength)
    {
        InternalHelpers.ValidateUtf8(utf8Buffer.AsSpan(byteStart, byteLength));
        return new(utf8Buffer, byteStart, byteLength);
    }

    /// <summary>
    /// Creates a new <see cref="Utf8String" />  from a given UTF-8 encoded byte buffer, starting at the specified index and with the specified length, without validating the UTF-8 encoding.
    /// </summary>
    /// <param name="utf8Buffer"></param>
    /// <param name="byteStart"></param>
    /// <param name="byteLength"></param>
    /// <returns></returns>
    public static Utf8String DangerousFromUtf8(ImmutableArray<byte> utf8Buffer, int byteStart, int byteLength) =>
        new(utf8Buffer, byteStart, byteLength);

    /// <summary>
    /// Returns a new <see cref="Utf8String" />  that is a slice of the current string, starting at the specified rune index and with the specified rune length.
    /// </summary>
    /// <param name="runeStart"></param>
    /// <param name="runeLength"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Utf8String Slice(int runeStart, int runeLength)
    {
        var enumerator = GetEnumerator();
        for (var i = 0; i < runeStart; i++)
        {
            if (!enumerator.MoveNext())
            {
                throw new ArgumentOutOfRangeException(nameof(runeStart));
            }
        }
        var startByteIndex = enumerator.NextByteIndex;
        for (var i = 0; i < runeLength; i++)
        {
            if (!enumerator.MoveNext())
            {
                throw new ArgumentOutOfRangeException(nameof(runeLength));
            }
        }
        var endByteIndex = enumerator.NextByteIndex;
        return new(_buffer, _byteStart + startByteIndex, endByteIndex - startByteIndex);
    }

    /// <inheritdoc />
    public Utf8String Slice(Utf8RuneIndex start, Utf8RuneIndex end)
    {
        var byteStart = _byteStart + start.ByteIndex;
        var byteLength = end.ByteIndex - start.ByteIndex;
        if(byteStart < 0 || (uint)(byteStart + byteLength) > (uint)_buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }
        return new(_buffer, byteStart, byteLength);
    }

    internal readonly Utf8String DangerousSlice(int byteStart, int byteLength)
    {
        return new(_buffer, _byteStart + byteStart, byteLength);
    }

    /// <inheritdoc />
    public bool TryGetRune(Utf8RuneIndex index, out Rune rune, out int codeUnitConsumed)
    {
        var result = Rune.DecodeFromUtf8(Buffer.Span.Slice(index.ByteIndex), out rune, out codeUnitConsumed);
        return result == OperationStatus.Done;
    }

    /// <inheritdoc />
    public bool TryIncrement(ref Utf8RuneIndex index)
    {
        Rune.DecodeFromUtf8(Buffer.Span.Slice(index.ByteIndex), out _, out var codeUnitConsumed);
        var newByteIndex = index.ByteIndex + codeUnitConsumed;
        if(_buffer.Length <= newByteIndex)
        {
            return false;
        }
        index = new(newByteIndex, index.RuneIndex + 1);
        return true;
    }

    /// <inheritdoc />
    public bool TryDecrement(ref Utf8RuneIndex index)
    {
        var newByteIndex = index.ByteIndex - 1;
        while(newByteIndex >= 0)
        {
            if((_buffer[newByteIndex] & 0xC0) != 0xC0)
            {
                index = new(newByteIndex, index.RuneIndex - 1);
                return true;
            }
            --newByteIndex;
        }
        return false;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the UTF-8 encoded string as a sequence of Unicode code points (runes).
    /// </summary>
    /// <returns></returns>
    public Utf8MemoryEnumerator GetEnumerator() => new(this);

    /// <summary>
    /// Returns the number of Unicode code points (runes) in the UTF-8 encoded string.
    /// </summary>
    /// <returns></returns>
    public int GetRuneCount() => InternalHelpers.GetRuneCount(GetEnumerator());

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Utf8String other && Equals(this, other);

    /// <inheritdoc />
    public override string ToString() => InternalHelpers.ToString(Buffer.Span);

    /// <inheritdoc />
    public override int GetHashCode() => InternalHelpers.GetHashCode(Buffer.Span);

    /// <inheritdoc />
    public int CompareTo(Utf8String other) => Compare(this, other);

    /// <inheritdoc />
    public bool Equals(Utf8String other) => Equals(this, other);

    /// <summary>
    /// Determines whether two <see cref="Utf8String" />  instances are equal by comparing their UTF-8 encoded byte sequences for equality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equals(Utf8String x, Utf8String y) =>
        InternalHelpers.Equals((Utf8Span)x, (Utf8Span)y);

    /// <summary>
    /// Compares two <see cref="Utf8String" />  instances by comparing their UTF-8 encoded byte sequences in lexicographical order.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(Utf8String x, Utf8String y) =>
        InternalHelpers.Compare((Utf8Span)x, (Utf8Span)y);

    /// <summary>
    /// Determines whether two <see cref="Utf8String" />  instances are equal by comparing their UTF-8 encoded byte sequences for equality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool operator ==(Utf8String x, Utf8String y) => Equals(x, y);

    /// <summary>
    /// Determines whether two <see cref="Utf8String" />  instances are not equal by comparing their UTF-8 encoded byte sequences for inequality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool operator !=(Utf8String x, Utf8String y) => !Equals(x, y);
}

