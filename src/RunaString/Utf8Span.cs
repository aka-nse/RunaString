using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace RunaString;

/// <summary>
/// Represents a read-only span of UTF-8 encoded bytes.
/// </summary>
[RuneEnumerable]
public readonly ref partial struct Utf8Span
    : IRuneString<Utf8Span, Utf8SpanEnumerator, Utf8RuneIndex>
    , IComparable<Utf8Span>
    , IEquatable<Utf8Span>
{
    #region source generated members

    public partial Rune this[Utf8RuneIndex index] { get; }
    public partial bool TryGetRune(Utf8RuneIndex index, out Rune rune);

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

    private Utf8Span(in byte reference, int length)
    {
        _reference = ref reference;
        _length = length;
    }

    /// <summary>
    /// Creates a new <see cref="Utf8Span" /> from a given <see cref="Utf8String" /> .
    /// </summary>
    /// <param name="str">The <see cref="Utf8String" /> to create the span from.</param>
    /// <returns>A new <see cref="Utf8Span" /> representing the UTF-8 encoded string.</returns>
    public static Utf8Span FromString(Utf8String str) =>
        DangerousFromSpan(str.Buffer.Span);

    /// <summary>
    /// Creates a new <see cref="Utf8Span" /> from a given UTF-8 encoded byte buffer, starting at the specified index and with the specified length, without validating the UTF-8 encoding.
    /// </summary>
    /// <param name="utf8Buffer"></param>
    /// <returns></returns>
    public static Utf8Span DangerousFromSpan(ReadOnlySpan<byte> utf8Buffer) =>
        new(in MemoryMarshal.GetReference(utf8Buffer), utf8Buffer.Length);

    /// <summary>
    /// Returns a new <see cref="Utf8Span" /> that is a slice of the current string, starting at the specified rune index and with the specified rune length.
    /// </summary>
    /// <param name="runeStart"></param>
    /// <param name="runeLength"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Utf8Span Slice(int runeStart, int runeLength)
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
    public Utf8Span Slice(Utf8RuneIndex start, Utf8RuneIndex end)
    {
        var byteStart = start.ByteIndex;
        var byteLength = end.ByteIndex - start.ByteIndex;
        if (byteStart < 0 || (uint)(byteStart + byteLength) > (uint)_length)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }
        return DangerousFromSpan(Buffer.Slice(byteStart, byteLength));
    }

    internal Utf8Span DangerousSlice(int byteStart, int byteLength)
    {
        return new(in Unsafe.Add(ref Unsafe.AsRef(in _reference), byteStart), byteLength);
    }

    /// <inheritdoc />
    public bool TryGetRune(Utf8RuneIndex index, out Rune rune, out int codeUnitConsumed)
    {
        var result = Rune.DecodeFromUtf8(Buffer.Slice(index.ByteIndex), out rune, out codeUnitConsumed);
        return result == OperationStatus.Done;
    }

    /// <inheritdoc />
    public bool TryIncrement(ref Utf8RuneIndex index)
    {
        Rune.DecodeFromUtf8(Buffer.Slice(index.ByteIndex), out _, out var codeUnitConsumed);
        var newByteIndex = index.ByteIndex + codeUnitConsumed;
        if (Buffer.Length <= newByteIndex)
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
        while (newByteIndex >= 0)
        {
            if (Buffer[newByteIndex] < 0x80 || Buffer[newByteIndex] >= 0xC0)
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
    public int CompareTo(Utf8Span other) => Compare(this, other);

    /// <inheritdoc />
    public bool Equals(Utf8Span other) => Equals(this, other);

    /// <summary>
    /// Determines whether two <see cref="Utf8Span" />  instances are equal by comparing their UTF-8 encoded byte sequences for equality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equals(Utf8Span x, Utf8Span y) =>
        InternalHelpers.Equals(x, y);

    /// <summary>
    /// Compares two <see cref="Utf8Span" />  instances by comparing their UTF-8 encoded byte sequences in lexicographical order.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int Compare(Utf8Span x, Utf8Span y) =>
        InternalHelpers.Compare(x, y);

    /// <summary>
    /// Determines whether two <see cref="Utf8Span" />  instances are equal by comparing their UTF-8 encoded byte sequences for equality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool operator ==(Utf8Span x, Utf8Span y) => Equals(x, y);

    /// <summary>
    /// Determines whether two <see cref="Utf8Span" />  instances are not equal by comparing their UTF-8 encoded byte sequences for inequality.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool operator !=(Utf8Span x, Utf8Span y) => !Equals(x, y);

    /// <summary>
    /// Defines an implicit conversion from <see cref="Utf8String" /> to <see cref="Utf8Span" /> that creates a new <see cref="Utf8Span" /> representing the UTF-8 encoded string.
    /// </summary>
    /// <param name="str"></param>
    public static implicit operator Utf8Span(Utf8String str) =>
        FromString(str);
}
