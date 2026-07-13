using System.Buffers;
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
        public CharsSpanString AsRunaString() => new(source);
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
        public CharsString AsRunaString() => new(source);
    }
}


/// <summary>
/// Represents an enumerable collection of Unicode runes backed by a read-only memory of <see cref="char"/>.
/// </summary>
/// <param name="source"></param>
[RunaString]
public readonly partial struct CharsString(ReadOnlyMemory<char> source)
    : IRunaString<CharsString, CharsMemoryEnumerator, CharsIndex>
{
    #region source generated members

    public partial Rune this[CharsIndex index] { get; }
    public partial bool TryGetRune(CharsIndex index, out Rune rune);

    #endregion

    /// <summary>
    /// Gets the source data as a read-only span of characters, which serves as the underlying buffer for enumerating Unicode runes in this enumerable.
    /// </summary>
    public ReadOnlyMemory<char> Source { get; } = source;

    /// <inheritdoc />
    public CharsMemoryEnumerator GetEnumerator() =>
        CharsMemoryEnumerator.Create(Source);

    /// <inheritdoc />
    public CharsString Slice(CharsIndex start, CharsIndex end) =>
        new(Source.Slice(start.CharIndex, end.CharIndex - start.CharIndex));

    /// <inheritdoc />
    public bool TryGetRune(CharsIndex index, out Rune rune, out int codeUnitConsumed) =>
        FileHelpers.TryGetRune(Source.Span, index, out rune, out codeUnitConsumed);

    /// <inheritdoc />
    public CharsIndex Increment(ref CharsIndex index) =>
        FileHelpers.Increment(Source.Span, ref index);

    /// <inheritdoc />
    public CharsIndex Decrement(ref CharsIndex index) =>
        FileHelpers.Decrement(Source.Span, ref index);

    /// <inheritdoc />
    public bool IsInRange(CharsIndex index) =>
        FileHelpers.IsInRange(Source.Span, index);

    /// <inheritdoc />
    public override string ToString() =>
        Source.ToString();
}


/// <summary>
/// Represents an enumerable collection of Unicode runes backed by a read-only span of <see cref="char"/>.
/// </summary>
/// <param name="source"></param>
[RunaString]
public readonly ref partial struct CharsSpanString(ReadOnlySpan<char> source)
    : IRunaString<CharsSpanString, CharsSpanEnumerator, CharsIndex>
{
    #region source generated members

    public partial Rune this[CharsIndex index] { get; }
    public partial bool TryGetRune(CharsIndex index, out Rune rune);

    #endregion

    /// <summary>
    /// Gets the source data as a read-only span of characters, which serves as the underlying buffer for enumerating Unicode runes in this enumerable.
    /// </summary>
    public ReadOnlySpan<char> Source { get; } = source;

    /// <inheritdoc />
    public CharsSpanEnumerator GetEnumerator() =>
        CharsSpanEnumerator.Create(Source);

    /// <inheritdoc />
    public CharsSpanString Slice(CharsIndex start, CharsIndex end) =>
        new(Source.Slice(start.CharIndex, end.CharIndex - start.CharIndex));

    /// <inheritdoc />
    public bool TryGetRune(CharsIndex index, out Rune rune, out int codeUnitConsumed) =>
        FileHelpers.TryGetRune(Source, index, out rune, out codeUnitConsumed);

    /// <inheritdoc />
    public CharsIndex Increment(ref CharsIndex index) =>
        FileHelpers.Increment(Source, ref index);

    /// <inheritdoc />
    public CharsIndex Decrement(ref CharsIndex index) =>
        FileHelpers.Decrement(Source, ref index);

    /// <inheritdoc />
    public bool IsInRange(CharsIndex index) =>
        FileHelpers.IsInRange(Source, index);

    /// <inheritdoc />
    public override string ToString() =>
        Source.ToString();
}


file static class FileHelpers
{
    public static bool TryGetRune(ReadOnlySpan<char> source, CharsIndex index, out Rune rune, out int codeUnitConsumed)
    {
        var result = Rune.DecodeFromUtf16(source.Slice(index.CharIndex), out rune, out codeUnitConsumed);
        return result == OperationStatus.Done;
    }

    public static CharsIndex Increment(ReadOnlySpan<char> Source, ref CharsIndex index)
    {
        if ((uint)index.CharIndex < (uint)Source.Length)
        {
            var newRuneIndex = index.RuneIndex + 1;
            var newCharIndex = index.CharIndex + (char.IsHighSurrogate(Source[index.CharIndex]) ? 2 : 1);
            index = new(newCharIndex, newRuneIndex);
        }
        return index;
    }

    public static CharsIndex Decrement(ReadOnlySpan<char> source, ref CharsIndex index)
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

    public static bool IsInRange(ReadOnlySpan<char> source, CharsIndex index) =>
        (uint)index.CharIndex < (uint)source.Length;
}