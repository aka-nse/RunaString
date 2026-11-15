using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RunaString;

partial class RunaString
{
    extension(ReadOnlySpan<char> source)
    {
        /// <summary>
        /// Creates a <see cref="CharsSpanString"/> from the given read-only span of <see cref="char"/>.
        /// </summary>
        /// <returns></returns>
        public CharsSpanString AsRunaString() => CharsSpanString.FromChars(source);
    }

    extension(string source)
    {
        /// <summary>
        /// Creates a <see cref="CharsString"/> from the given read-only span of string.
        /// </summary>
        /// <returns></returns>
        public CharsString AsRunaString() => source.AsMemory().AsRunaString();
    }

    extension(ReadOnlyMemory<char> source)
    {
        /// <summary>
        /// Creates a <see cref="CharsString"/> from the given read-only span of <see cref="char"/>.
        /// </summary>
        /// <returns></returns>
        public CharsString AsRunaString() => CharsString.FromChars(source);
    }
}


/// <summary>
/// Represents an enumerable collection of Unicode runes backed by a read-only memory of <see cref="char"/>.
/// </summary>
/// <remarks>
/// This API is designed to handle only valid UTF-16 encoded characters.
/// If invalid UTF-16 sequences are provided via <see cref="DangerousFromChars(ReadOnlyMemory{char})"/>,
/// the behavior is undefined and may lead to exceptions or incorrect results.
/// </remarks>
[RunaString]
public readonly partial struct CharsString
    : IRunaString<CharsString, CharsSpanString, CharsMemoryEnumerator, CharsIndex>
{
    #region base members

    /// <summary>
    /// Gets the source data as a read-only span of characters, which serves as the underlying buffer for enumerating Unicode runes in this enumerable.
    /// </summary>
    public ReadOnlyMemory<char> Source { get; }

    /// <inheritdoc />
    public override string ToString() => Source.ToString();

    /// <inheritdoc />
    public override int GetHashCode() => CharHelpers.GetHashCode(Source.Span);


    #region source generated members

    public partial Rune this[CharsIndex index] { get; }
    public partial bool TryGetRune(CharsIndex index, out Rune rune);

    #endregion

    #endregion

    #region creation

    /// <inheritdoc />
    public static CharsString FromRunes(scoped ReadOnlySpan<Rune> runes)
    {
        var charCount = 0;
        foreach (var rune in runes)
        {
            charCount += rune.Utf16SequenceLength;
        }

        var chars = GC.AllocateUninitializedArray<char>(charCount);
        var destination = chars.AsSpan();
        var written = 0;

        foreach(var rune in runes)
        {
            if(!rune.TryEncodeToUtf16(destination.Slice(written), out var writtenCount))
            {
                Guard.ThrowInvalidOperation("Failed to encode rune to UTF-16.");
            }
            written += writtenCount;
        }
        return new(chars);
    }

    /// <inheritdoc />
    public static CharsString FromString(scoped ReadOnlySpan<char> str) =>
        new(str.ToArray());

    /// <summary>
    /// Creates a new <see cref="CharsString"/> from the given read-only memory of <see cref="char"/>. The input is validated to ensure it is valid UTF-16."/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static CharsString FromChars(ReadOnlyMemory<char> source)
    {
        CharHelpers.ValidateUtf16(source.Span);
        return new(source);
    }

    /// <param name="source"></param>
    private CharsString(ReadOnlyMemory<char> source)
    {
        Source = source;
    }

    /// <summary>
    /// Creates a new <see cref="CharsString" />  from a given chars encoded byte buffer.
    /// </summary>
    /// <param name="source"> Must be valid UTF-16 encoded characters. </param>
    /// <returns></returns>
    /// <remarks>
    /// If an invalid UTF-16 character sequence is input, the state of all members of this instance - as well as all members reachable via the return values ​​of those members - becomes undefined.
    /// </remarks>
    public static CharsString DangerousFromChars(ReadOnlyMemory<char> source) => new(source);

    #endregion

    #region slice

    /// <inheritdoc />
    public CharsString Slice(int runeStart)
    {
        var index = GetIndexByRuneIndex(runeStart);
        return new(Source.Slice(index.CharIndex));
    }

    /// <inheritdoc />
    public CharsString Slice(int runeStart, int runeLength)
    {
        var (charIndex, charLength) = FileHelpers.GetSliceIndex(this, runeStart, runeLength);
        return new(Source.Slice(charIndex, charLength));
    }

    /// <inheritdoc />
    public CharsString Slice(CharsIndex start) =>
        new(Source.Slice(start.CharIndex));

    /// <inheritdoc />
    public CharsString Slice(CharsIndex start, CharsIndex end) =>
        new(Source.Slice(start.CharIndex, end.CharIndex - start.CharIndex));

    #endregion

    #region enumerations

    /// <inheritdoc />
    public CharsIndex Increment(ref CharsIndex index) =>
        FileHelpers.Increment(Source.Span, ref index);

    /// <inheritdoc />
    public CharsIndex IncrementBegin => CharsIndex.IncrementBegin;

    /// <inheritdoc />
    public CharsIndex GetIncrementEnd() => new(Source.Length, GetRuneCount());

    /// <inheritdoc />
    public CharsIndex Decrement(ref CharsIndex index) =>
        FileHelpers.Decrement(Source.Span, ref index);

    /// <inheritdoc />
    public CharsIndex DecrementEnd => CharsIndex.DecrementEnd;

    /// <inheritdoc />
    public CharsIndex GetDecrementBegin()
    {
        var index = new CharsIndex(Source.Length, GetRuneCount());
        return Decrement(ref index);
    }

    /// <inheritdoc />
    public CharsMemoryEnumerator GetEnumerator() =>
        CharsMemoryEnumerator.Create(Source);

    #endregion

    #region rune-base manipulations

    /// <inheritdoc />
    public bool TryGetRune(CharsIndex index, out Rune rune, out int codeUnitConsumed) =>
        FileHelpers.TryGetRune(Source.Span, index, out rune, out codeUnitConsumed);

    /// <inheritdoc />
    public CharsIndex GetIndexByRuneIndex(int runeIndex)
    {
        for (CharsIndex i = IncrementBegin, end = GetIncrementEnd(); i != end; Increment(ref i))
        {
            if (i.RuneIndex == runeIndex)
            {
                return i;
            }
        }
        return Guard.ThrowArgument<CharsIndex>(nameof(runeIndex), "The specified rune index is out of range.");
    }

    /// <inheritdoc />
    public bool IsInRange(CharsIndex index) =>
        FileHelpers.IsInRange(Source.Span, index);

    /// <inheritdoc />
    public int GetRuneCount() => CharHelpers.GetRuneCount(Source.Span);

    #endregion

    #region comparison and equality

    /// <inheritdoc />
    public int CodeUnitIndexOf(CharsSpanString pattern) =>
        MemoryExtensions.IndexOf(Source.Span, pattern.Source);

    /// <inheritdoc />
    public bool StartsWith(CharsSpanString pattern) =>
        MemoryExtensions.StartsWith(Source.Span, pattern.Source);

    /// <inheritdoc />
    public bool EndsWith(CharsSpanString pattern) =>
        MemoryExtensions.EndsWith(Source.Span, pattern.Source);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is CharsString other && Equals(this, other);

    /// <inheritdoc />
    public int CompareTo(CharsString other) =>
        Compare(this, other);

    /// <inheritdoc />
    public bool Equals(CharsString other) =>
        Equals(this, other);

    /// <inheritdoc />
    public static bool Equals(CharsString x, CharsString y) =>
        InternalHelpers.Equals(x.Source.Span, y.Source.Span);

    /// <inheritdoc />
    public static bool Equals(CharsString x, CharsSpanString y) =>
        InternalHelpers.Equals(x.Source.Span, y.Source);

    /// <inheritdoc />
    public static bool Equals(CharsSpanString x, CharsString y) =>
        InternalHelpers.Equals(x.Source, y.Source.Span);

    /// <inheritdoc />
    public static int Compare(CharsString x, CharsString y) =>
        InternalHelpers.Compare<CharsString, CharsMemoryEnumerator>(x, y);

    /// <inheritdoc />
    public static int Compare(CharsString x, CharsSpanString y) =>
        InternalHelpers.Compare<CharsSpanString, CharsSpanEnumerator>(x, y);

    /// <inheritdoc />
    public static int Compare(CharsSpanString x, CharsString y) =>
        InternalHelpers.Compare<CharsSpanString, CharsSpanEnumerator>(x, y);

    /// <inheritdoc />
    public static bool operator ==(CharsString x, CharsString y) =>
        Equals(x, y);

    /// <inheritdoc />
    public static bool operator ==(CharsString x, CharsSpanString y) =>
        Equals(x, y);

    /// <inheritdoc />
    public static bool operator ==(CharsSpanString x, CharsString y) =>
        Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(CharsString x, CharsString y) =>
        !Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(CharsString x, CharsSpanString y) =>
        !Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(CharsSpanString x, CharsString y) =>
        !Equals(x, y);

    #endregion

    #region type conversion

    /// <inheritdoc />
    public CharsSpanString AsView() => this;

    #endregion
}


/// <summary>
/// Represents an enumerable collection of Unicode runes backed by a read-only span of <see cref="char"/>.
/// </summary>
/// <remarks>
/// This API is designed to handle only valid UTF-16 encoded characters.
/// If invalid UTF-16 sequences are provided via <see cref="DangerousFromChars(ReadOnlySpan{char})"/>,
/// the behavior is undefined and may lead to exceptions or incorrect results.
/// </remarks>
[RunaString]
public readonly ref partial struct CharsSpanString
    : IRunaString<CharsSpanString, CharsSpanString, CharsSpanEnumerator, CharsIndex>
{
    #region base members

    /// <summary>
    /// Gets the source data as a read-only span of characters, which serves as the underlying buffer for enumerating Unicode runes in this enumerable.
    /// </summary>
    public ReadOnlySpan<char> Source { get; }

    /// <inheritdoc />
    public override string ToString() =>
        Source.ToString();

    /// <inheritdoc />
    public override int GetHashCode() =>
        CharHelpers.GetHashCode(Source);


    #region source generated members

    public partial Rune this[CharsIndex index] { get; }
    public partial bool TryGetRune(CharsIndex index, out Rune rune);

    #endregion

    #endregion

    #region creation

    /// <inheritdoc />
    public static CharsSpanString FromRunes(scoped ReadOnlySpan<Rune> runes) =>
        CharsString.FromRunes(runes);

    /// <inheritdoc />
    public static CharsSpanString FromString(scoped ReadOnlySpan<char> str) =>
        CharsString.FromString(str);

    private CharsSpanString(ReadOnlySpan<char> source)
    {
        Source = source;
    }

    /// <summary>
    /// Creates a new <see cref="CharsString"/> from the given read-only memory of <see cref="char"/>. The input is validated to ensure it is valid UTF-16."/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static CharsSpanString FromChars(ReadOnlySpan<char> source)
    {
        CharHelpers.ValidateUtf16(source);
        return new(source);
    }

    /// <summary>
    /// Creates a new <see cref="CharsString" />  from a given chars encoded byte buffer.
    /// </summary>
    /// <param name="source"> Must be valid UTF-16 encoded characters. </param>
    /// <returns></returns>
    /// <remarks>
    /// If an invalid UTF-16 character sequence is input, the state of all members of this instance - as well as all members reachable via the return values ​​of those members - becomes undefined.
    /// </remarks>
    public static CharsSpanString DangerousFromChars(ReadOnlySpan<char> source) => new(source);

    #endregion

    #region slicing

    /// <inheritdoc />
    public CharsSpanString Slice(int runeStart)
    {
        var index = GetIndexByRuneIndex(runeStart);
        return new(Source.Slice(index.CharIndex));
    }

    /// <inheritdoc />
    public CharsSpanString Slice(int runeStart, int runeLength)
    {
        var (charIndex, charLength) = FileHelpers.GetSliceIndex(this, runeStart, runeLength);
        return new(Source.Slice(charIndex, charLength));
    }

    /// <inheritdoc />
    public CharsSpanString Slice(CharsIndex start) =>
        new(Source.Slice(start.CharIndex));

    /// <inheritdoc />
    public CharsSpanString Slice(CharsIndex start, CharsIndex end) =>
        new(Source.Slice(start.CharIndex, end.CharIndex - start.CharIndex));

    #endregion

    #region enumerations

    /// <inheritdoc />
    public CharsIndex Increment(ref CharsIndex index) =>
        FileHelpers.Increment(Source, ref index);

    /// <inheritdoc />
    public CharsIndex IncrementBegin =>
        CharsIndex.IncrementBegin;

    /// <inheritdoc />
    public CharsIndex GetIncrementEnd() =>
        new(Source.Length, GetRuneCount());

    /// <inheritdoc />
    public CharsIndex Decrement(ref CharsIndex index) =>
        FileHelpers.Decrement(Source, ref index);

    /// <inheritdoc />
    public CharsIndex DecrementEnd =>
        CharsIndex.DecrementEnd;

    /// <inheritdoc />
    public CharsIndex GetDecrementBegin()
    {
        var index = new CharsIndex(Source.Length, GetRuneCount());
        return Decrement(ref index);
    }

    /// <inheritdoc />
    public CharsSpanEnumerator GetEnumerator() =>
        CharsSpanEnumerator.Create(Source);

    #endregion

    #region rune-base manipulations

    /// <inheritdoc />
    public int CodeUnitIndexOf(scoped CharsSpanString pattern) =>
        MemoryExtensions.IndexOf(Source, pattern.Source);

    /// <inheritdoc />
    public bool TryGetRune(CharsIndex index, out Rune rune, out int codeUnitConsumed) =>
        FileHelpers.TryGetRune(Source, index, out rune, out codeUnitConsumed);

    /// <inheritdoc />
    public CharsIndex GetIndexByRuneIndex(int runeIndex)
    {
        for (CharsIndex i = IncrementBegin, end = GetIncrementEnd(); i != end; Increment(ref i))
        {
            if (i.RuneIndex == runeIndex)
            {
                return i;
            }
        }
        return Guard.ThrowArgument<CharsIndex>(nameof(runeIndex), "The specified rune index is out of range.");
    }

    /// <inheritdoc />
    public bool IsInRange(CharsIndex index) =>
        FileHelpers.IsInRange(Source, index);

    /// <inheritdoc />
    public int GetRuneCount() => CharHelpers.GetRuneCount(Source);

    #endregion

    #region comparison and equality

    /// <inheritdoc />
    public bool StartsWith(scoped CharsSpanString pattern) =>
        MemoryExtensions.StartsWith(Source, pattern.Source);

    /// <inheritdoc />
    public bool EndsWith(scoped CharsSpanString pattern) =>
        MemoryExtensions.EndsWith(Source, pattern.Source);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => false;
    /// <inheritdoc />
    public int CompareTo(scoped CharsSpanString other) => Compare(this, other);

    /// <inheritdoc />
    public bool Equals(scoped CharsSpanString other) => Equals(this, other);

    /// <inheritdoc />
    public static bool Equals(scoped CharsSpanString x, scoped CharsSpanString y) =>
        InternalHelpers.Equals(x.Source, y.Source);

    /// <inheritdoc />
    public static int Compare(scoped CharsSpanString x, scoped CharsSpanString y) =>
        InternalHelpers.Compare<CharsSpanString, CharsSpanEnumerator>(x, y);

    /// <inheritdoc />
    public static bool operator ==(scoped CharsSpanString x, scoped CharsSpanString y) => Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(scoped CharsSpanString x, scoped CharsSpanString y) => !Equals(x, y);

    #endregion

    #region type conversion

    /// <inheritdoc />
    public CharsSpanString AsView() => this;

    /// <summary>
    /// Defines an implicit conversion from <see cref="Utf8String" /> to <see cref="CharsSpanString" /> that creates a new <see cref="CharsSpanString" /> representing the UTF-8 encoded string.
    /// </summary>
    /// <param name="str"></param>
    public static implicit operator CharsSpanString(CharsString str) =>
        new (str.Source.Span);

    #endregion
}


file static class FileHelpers
{
    public static (int charIndex, int charLength) GetSliceIndex(scoped CharsSpanString str, int runeStart, int runeLength)
    {
        var enumerator = str.GetEnumerator();
        for (var i = 0; i < runeStart; i++)
        {
            Guard.ValidArgument(enumerator.MoveNext(), "The runeStart index is out of range.");
        }
        var startCharIndex = enumerator.NextCharIndex;
        for (var i = 0; i < runeLength; i++)
        {
            Guard.ValidArgument(enumerator.MoveNext(), "The runeLength is out of range.");
        }
        var endCharIndex = enumerator.NextCharIndex;
        return (startCharIndex, endCharIndex - startCharIndex);
    }

    public static bool TryGetRune(scoped ReadOnlySpan<char> source, CharsIndex index, out Rune rune, out int codeUnitConsumed)
    {
        Rune.DecodeFromUtf16(source.Slice(index.CharIndex), out rune, out codeUnitConsumed);
        return codeUnitConsumed > 0;
    }

    public static CharsIndex Increment(scoped ReadOnlySpan<char> Source, ref CharsIndex index)
    {
        if ((uint)index.CharIndex < (uint)Source.Length)
        {
            var newRuneIndex = index.RuneIndex + 1;
            var newCharIndex = index.CharIndex + (char.IsHighSurrogate(Source[index.CharIndex]) ? 2 : 1);
            index = new(newCharIndex, newRuneIndex);
        }
        return index;
    }

    public static CharsIndex Decrement(scoped ReadOnlySpan<char> source, ref CharsIndex index)
    {
        if (index.CharIndex <= 0)
        {
            return index = CharsIndex.DecrementEnd;
        }
        var newRuneIndex = index.RuneIndex - 1;
        var newCharIndex = index.CharIndex - 1;
        if (char.IsLowSurrogate(source[newCharIndex]))
        {
            --newCharIndex;
        }
        return index = new(newCharIndex, newRuneIndex);
    }

    public static bool IsInRange(scoped ReadOnlySpan<char> source, CharsIndex index) =>
        (uint)index.CharIndex < (uint)source.Length;
}