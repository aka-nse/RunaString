using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
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

    /// <summary>
    /// Gets a read-only memory of bytes representing the UTF-8 encoded string.
    /// </summary>
    public ReadOnlyMemory<byte> Buffer { get; }

    /// <summary>
    /// Gets the length of the UTF-8 encoded string in bytes.
    /// </summary>
    public int BufferLength => Buffer.Length;

    private Utf8String(ReadOnlyMemory<byte> buffer)
    {
        Buffer = buffer;
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
        var memory = utf8Buffer.AsMemory().Slice(byteStart, byteLength);
        InternalHelpers.ValidateUtf8(memory.Span);
        return new(memory);
    }

    /// <summary>
    /// Creates a new <see cref="Utf8String" />  from a given UTF-8 encoded byte buffer, starting at the specified index and with the specified length, without validating the UTF-8 encoding.
    /// </summary>
    /// <param name="utf8Buffer"></param>
    /// <returns></returns>
    public static Utf8String DangerousFromUtf8(ReadOnlyMemory<byte> utf8Buffer) =>
        new (utf8Buffer);

    /// <inheritdoc />
    public Utf8String Slice(int runeStart, int runeLength)
    {
        var (byteIndex, byteLength) = FileHelpers.GetSliceIndex(this, runeStart, runeLength);
        return new(Buffer.Slice(byteIndex, byteLength));
    }

    /// <inheritdoc />
    public Utf8String Slice(Utf8Index start, Utf8Index end)
    {
        var byteStart = start.ByteIndex;
        var byteLength = end.ByteIndex - start.ByteIndex;
        if (byteStart < 0 || (uint)(byteStart + byteLength) > (uint)Buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }
        return new(Buffer.Slice(byteStart, byteLength));
    }

    internal readonly Utf8String DangerousSlice(int byteStart, int byteLength)
    {
        return new(Buffer.Slice(byteStart, byteLength));
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

    /// <inheritdoc />
    public Utf8MemoryEnumerator GetEnumerator() => new(this);

    /// <inheritdoc />
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

    /// <inheritdoc />
    public static bool Equals(Utf8String x, Utf8String y) =>
        InternalHelpers.Equals(x.Buffer.Span, y.Buffer.Span);

    /// <inheritdoc />
    public static int Compare(Utf8String x, Utf8String y) =>
        InternalHelpers.Compare<Utf8String, Utf8MemoryEnumerator>(x, y);

    /// <inheritdoc />
    public static bool operator ==(Utf8String x, Utf8String y) => Equals(x, y);

    /// <inheritdoc />
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

    /// <inheritdoc />
    public Utf8SpanString Slice(int runeStart, int runeLength)
    {
        var (byteIndex, byteLength) = FileHelpers.GetSliceIndex(this, runeStart, runeLength);
        return new(in Unsafe.Add(ref Unsafe.AsRef(in _reference), byteIndex), byteLength);
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

    /// <inheritdoc />
    public Utf8SpanEnumerator GetEnumerator() => new(this);

    /// <inheritdoc />
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

    /// <inheritdoc />
    public static bool Equals(Utf8SpanString x, Utf8SpanString y) =>
        InternalHelpers.Equals(x.Buffer, y.Buffer);

    /// <inheritdoc />
    public static int Compare(Utf8SpanString x, Utf8SpanString y) =>
        InternalHelpers.Compare<Utf8SpanString, Utf8SpanEnumerator>(x, y);

    /// <inheritdoc />
    public static bool operator ==(Utf8SpanString x, Utf8SpanString y) => Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(Utf8SpanString x, Utf8SpanString y) => !Equals(x, y);

    /// <summary>
    /// Defines an implicit conversion from <see cref="Utf8String" /> to <see cref="Utf8SpanString" /> that creates a new <see cref="Utf8SpanString" /> representing the UTF-8 encoded string.
    /// </summary>
    /// <param name="str"></param>
    public static implicit operator Utf8SpanString(Utf8String str) =>
        FromString(str);
}



/// <summary>
/// Provides methods for comparing UTF-8 encoded strings and spans, as well as computing their hash codes.
/// </summary>
public abstract partial class Utf8Comparer
    : IComparer<Utf8SpanString>
    , IComparer<Utf8String>
    , IEqualityComparer<Utf8SpanString>
    , IEqualityComparer<Utf8String>
{
    /// <summary>
    /// Gets a default instance of the <see cref="Utf8Comparer"/> class that performs ordinal (binary) comparisons of UTF-8 encoded strings and spans.
    /// </summary>
    public static Utf8Comparer Default { get; } = new Default_();

    private sealed class Default_ : Utf8Comparer
    {
        public override int Compare(Utf8SpanString x, Utf8SpanString y) =>
            InternalHelpers.Compare<Utf8SpanString, Utf8SpanEnumerator>(x, y);

        public override bool Equals(Utf8SpanString x, Utf8SpanString y) =>
            InternalHelpers.Equals(x.Buffer, y.Buffer);

        public override int GetHashCode([NotNull] Utf8SpanString obj) =>
            InternalHelpers.GetHashCode(obj.Buffer);
    }

    private Utf8Comparer()
    {
    }

    /// <inheritdoc />
    public abstract int Compare(Utf8SpanString x, Utf8SpanString y);

    /// <inheritdoc />
    public int Compare(Utf8String x, Utf8String y) =>
        Compare((Utf8SpanString)x, (Utf8SpanString)y);

    /// <inheritdoc />
    public abstract bool Equals(Utf8SpanString x, Utf8SpanString y);

    /// <inheritdoc />
    public bool Equals(Utf8String x, Utf8String y) =>
        Equals((Utf8SpanString)x, (Utf8SpanString)y);

    /// <inheritdoc />
    public abstract int GetHashCode([DisallowNull] Utf8SpanString obj);

    /// <inheritdoc />
    public int GetHashCode([DisallowNull] Utf8String obj) =>
        GetHashCode((Utf8SpanString)obj);
}


file static class FileHelpers
{
    public static (int byteIndex, int byteLength) GetSliceIndex(Utf8SpanString str, int runeStart, int runeLength)
    {
        var enumerator = str.GetEnumerator();
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
        return (startByteIndex, endByteIndex - startByteIndex);
    }

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
            var (byteIndex, runeIndex) = index;
            Utf8Helpers.TryGetRuneAndMoveNext(buffer, ref byteIndex, ref runeIndex, out _);
            index = new(byteIndex, runeIndex);
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
