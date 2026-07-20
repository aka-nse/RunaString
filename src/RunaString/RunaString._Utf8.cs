using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace RunaString;

/// <summary>
/// Represents an immutable UTF-8 encoded string.
/// </summary>
[RunaString]
public readonly partial struct Utf8String
    : IRunaString<Utf8String, Utf8MemoryEnumerator, Utf8Index>
    , IComparable<Utf8String>
    , IEquatable<Utf8String>
{
    #region source generated members

    public partial Rune this[Utf8Index index] { get; }
    public partial bool TryGetRune(Utf8Index index, out Rune rune);

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
    /// <returns></returns>
    /// <exception cref="ArgumentException" />
    public static Utf8String FromUtf8(ImmutableArray<byte> utf8Buffer) =>
        FromUtf8(utf8Buffer, 0, utf8Buffer.Length);

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
    /// <returns></returns>
    public static Utf8String DangerousFromUtf8(ImmutableArray<byte> utf8Buffer) =>
        DangerousFromUtf8(utf8Buffer, 0, utf8Buffer.Length);

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
    public Utf8String Slice(Utf8Index start, Utf8Index end)
    {
        var byteStart = _byteStart + start.ByteIndex;
        var byteLength = end.ByteIndex - start.ByteIndex;
        if (byteStart < 0 || (uint)(byteStart + byteLength) > (uint)_buffer.Length)
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
    public bool TryGetRune(Utf8Index index, out Rune rune, out int codeUnitConsumed) =>
        FileHelpers.TryGetRune(Buffer.Span, index, out rune, out codeUnitConsumed);

    /// <inheritdoc />
    public Utf8Index Increment(ref Utf8Index index) =>
        FileHelpers.Increment(Buffer.Span, ref index);

    /// <inheritdoc />
    public Utf8Index Decrement(ref Utf8Index index) =>
        FileHelpers.Decrement(Buffer.Span, ref index);

    /// <inheritdoc />
    public bool IsInRange(Utf8Index index) =>
        FileHelpers.IsInRange(Buffer.Span, index);

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
        InternalHelpers.Equals((Utf8SpanString)x, (Utf8SpanString)y);

    /// <summary>
    /// Compares two <see cref="Utf8String" />  instances by comparing their UTF-8 encoded byte sequences in lexicographical order.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(Utf8String x, Utf8String y) =>
        InternalHelpers.Compare((Utf8SpanString)x, (Utf8SpanString)y);

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


/// <summary>
/// Represents a read-only span of UTF-8 encoded bytes.
/// </summary>
[RunaString]
public readonly ref partial struct Utf8SpanString
    : IRunaString<Utf8SpanString, Utf8SpanEnumerator, Utf8Index>
    , IComparable<Utf8SpanString>
    , IEquatable<Utf8SpanString>
{
    #region source generated members

    public partial Rune this[Utf8Index index] { get; }
    public partial bool TryGetRune(Utf8Index index, out Rune rune);

    #endregion

    private readonly ref readonly byte _reference;

    private readonly int _length;

    /// <summary>
    /// Gets a read-only span of bytes representing the UTF-8 encoded string.
    /// </summary>
    public ReadOnlySpan<byte> Buffer =>
        MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _reference), _length);

    /// <summary>
    /// Gets the length of the UTF-8 encoded string in bytes.
    /// </summary>
    public int BufferLength => _length;

    private Utf8SpanString(in byte reference, int length)
    {
        _reference = ref reference;
        _length = length;
    }

    /// <summary>
    /// Creates a new <see cref="Utf8SpanString" /> from a given <see cref="Utf8String" /> .
    /// </summary>
    /// <param name="str">The <see cref="Utf8String" /> to create the span from.</param>
    /// <returns>A new <see cref="Utf8SpanString" /> representing the UTF-8 encoded string.</returns>
    public static Utf8SpanString FromString(Utf8String str) =>
        DangerousFromSpan(str.Buffer.Span);

    /// <summary>
    /// Creates a new <see cref="Utf8SpanString" /> from a given UTF-8 encoded byte buffer, starting at the specified index and with the specified length, without validating the UTF-8 encoding.
    /// </summary>
    /// <param name="utf8Buffer"></param>
    /// <returns></returns>
    public static Utf8SpanString DangerousFromSpan(ReadOnlySpan<byte> utf8Buffer) =>
        new(in MemoryMarshal.GetReference(utf8Buffer), utf8Buffer.Length);

    /// <summary>
    /// Returns a new <see cref="Utf8SpanString" /> that is a slice of the current string, starting at the specified rune index and with the specified rune length.
    /// </summary>
    /// <param name="runeStart"></param>
    /// <param name="runeLength"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Utf8SpanString Slice(int runeStart, int runeLength)
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
        return new(in Unsafe.Add(ref Unsafe.AsRef(in _reference), startByteIndex), endByteIndex - startByteIndex);
    }

    /// <inheritdoc />
    public Utf8SpanString Slice(Utf8Index start, Utf8Index end)
    {
        var byteStart = start.ByteIndex;
        var byteLength = end.ByteIndex - start.ByteIndex;
        if (byteStart < 0 || (uint)(byteStart + byteLength) > (uint)_length)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }
        return DangerousFromSpan(Buffer.Slice(byteStart, byteLength));
    }

    internal Utf8SpanString DangerousSlice(int byteStart, int byteLength)
    {
        return new(in Unsafe.Add(ref Unsafe.AsRef(in _reference), byteStart), byteLength);
    }

    /// <inheritdoc />
    public bool TryGetRune(Utf8Index index, out Rune rune, out int codeUnitConsumed) =>
        FileHelpers.TryGetRune(Buffer, index, out rune, out codeUnitConsumed);

    /// <inheritdoc />
    public Utf8Index Increment(ref Utf8Index index) =>
        FileHelpers.Increment(Buffer, ref index);

    /// <inheritdoc />
    public Utf8Index Decrement(ref Utf8Index index) =>
        FileHelpers.Decrement(Buffer, ref index);

    /// <inheritdoc />
    public bool IsInRange(Utf8Index index) =>
        FileHelpers.IsInRange(Buffer, index);

    /// <summary>
    /// Returns an enumerator that iterates through the UTF-8 encoded string as a sequence of Unicode code points (runes).
    /// </summary>
    /// <returns></returns>
    public Utf8SpanEnumerator GetEnumerator() => new(this);

    /// <summary>
    /// Returns the number of Unicode code points (runes) in the UTF-8 encoded string.
    /// </summary>
    /// <returns></returns>
    public int GetRuneCount() => InternalHelpers.GetRuneCount(GetEnumerator());

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => false;

    /// <inheritdoc />
    public override string ToString() => InternalHelpers.ToString(Buffer);

    /// <inheritdoc />
    public override int GetHashCode() => InternalHelpers.GetHashCode(Buffer);

    /// <inheritdoc />
    public int CompareTo(Utf8SpanString other) => Compare(this, other);

    /// <inheritdoc />
    public bool Equals(Utf8SpanString other) => Equals(this, other);

    /// <summary>
    /// Determines whether two <see cref="Utf8SpanString" />  instances are equal by comparing their UTF-8 encoded byte sequences for equality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equals(Utf8SpanString x, Utf8SpanString y) =>
        InternalHelpers.Equals(x, y);

    /// <summary>
    /// Compares two <see cref="Utf8SpanString" />  instances by comparing their UTF-8 encoded byte sequences in lexicographical order.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(Utf8SpanString x, Utf8SpanString y) =>
        InternalHelpers.Compare(x, y);

    /// <summary>
    /// Determines whether two <see cref="Utf8SpanString" />  instances are equal by comparing their UTF-8 encoded byte sequences for equality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool operator ==(Utf8SpanString x, Utf8SpanString y) => Equals(x, y);

    /// <summary>
    /// Determines whether two <see cref="Utf8SpanString" />  instances are not equal by comparing their UTF-8 encoded byte sequences for inequality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool operator !=(Utf8SpanString x, Utf8SpanString y) => !Equals(x, y);

    /// <summary>
    /// Defines an implicit conversion from <see cref="Utf8String" /> to <see cref="Utf8SpanString" /> that creates a new <see cref="Utf8SpanString" /> representing the UTF-8 encoded string.
    /// </summary>
    /// <param name="str"></param>
    public static implicit operator Utf8SpanString(Utf8String str) =>
        FromString(str);
}


file static class FileHelpers
{
    public static bool TryGetRune(ReadOnlySpan<byte> buffer, Utf8Index index, out Rune rune, out int codeUnitConsumed)
    {
        var (byteIndex, runeIndex) = index;
        var result = Utf8Helpers.TryGetRuneAndMoveNext(buffer, ref byteIndex, ref runeIndex, out rune);
        codeUnitConsumed = byteIndex - index.ByteIndex;
        return result;
    }

    public static Utf8Index Increment(ReadOnlySpan<byte> buffer, ref Utf8Index index)
    {
        if((uint)index.ByteIndex < (uint)buffer.Length)
        {
            Rune.DecodeFromUtf8(buffer.Slice(index.ByteIndex), out _, out var codeUnitConsumed);
            var newByteIndex = index.ByteIndex + codeUnitConsumed;
            index = new(newByteIndex, index.RuneIndex + 1);
        }
        return index;
    }

    public static Utf8Index Decrement(ReadOnlySpan<byte> buffer, ref Utf8Index index)
    {
        if (index.ByteIndex <= 0)
        {
            return index = new(-1, -1);
        }
        var newByteIndex = index.ByteIndex - 1;
        while (newByteIndex >= 0)
        {
            if (buffer[newByteIndex] < 0x80 || buffer[newByteIndex] >= 0xC0)
            {
                index = new(newByteIndex, index.RuneIndex - 1);
                break;
            }
            --newByteIndex;
        }
        return index;
    }

    public static bool IsInRange(ReadOnlySpan<byte> buffer, Utf8Index index) =>
        (uint)index.ByteIndex < (uint)buffer.Length;
}
