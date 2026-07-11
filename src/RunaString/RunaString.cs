using System.Buffers;
using System.Text;

namespace RunaString;

/// <summary>
/// Extensions for <see cref="ReadOnlySpan{Char}"/>, <see cref="ReadOnlyMemory{Char}"/>,
/// <see cref="ReadOnlySpan{Rune}"/>, and <see cref="ReadOnlyMemory{Rune}"/>.
/// </summary>
public static class RunaString
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
        /// Creates a <see cref="CharsMemoryString"/> from the given read-only span of string.
        /// </summary>
        /// <returns></returns>
        public CharsMemoryString AsRunaString() => source.AsMemory().AsRunaString();
    }

    extension(ReadOnlyMemory<char> source)
    {
        /// <summary>
        /// Creates a <see cref="CharsMemoryString"/> from the given read-only span of <see cref="char"/>.
        /// </summary>
        /// <returns></returns>
        public CharsMemoryString AsRunaString() => new(source);
    }

    extension(ReadOnlySpan<Rune> source)
    {
        /// <summary>
        /// Creates a <see cref="RunesSpanString"/> from the given read-only span of <see cref="Rune"/>.
        /// </summary>
        /// <returns></returns>
        public RunesSpanString AsRunaString() => new(source);
    }

    extension(ReadOnlyMemory<Rune> source)
    {
        /// <summary>
        /// Creates a <see cref="RunesMemoryString"/> from the given read-only span of <see cref="Rune"/>.
        /// </summary>
        /// <returns></returns>
        public RunesMemoryString AsRunaString() => new(source);
    }
}


/// <summary>
/// Represents an enumerable collection of Unicode runes backed by a read-only span of <see cref="char"/>.
/// </summary>
/// <param name="source"></param>
[RuneString]
public readonly ref partial struct CharsSpanString(ReadOnlySpan<char> source)
    : IRuneString<CharsSpanString, CharsSpanEnumerator, CharsIndex>
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
        new (Source.Slice(start.CharIndex, end.CharIndex - start.CharIndex));

    /// <inheritdoc />
    public bool TryGetRune(CharsIndex index, out Rune rune, out int codeUnitConsumed)
    {
        var result = Rune.DecodeFromUtf16(Source.Slice(index.CharIndex), out rune, out codeUnitConsumed);
        return result == OperationStatus.Done;
    }

    /// <inheritdoc />
    public bool TryIncrement(ref CharsIndex index)
    {
        if (index.CharIndex >= Source.Length)
        {
            return false;
        }
        var newRuneIndex = index.RuneIndex + 1;
        var newCharIndex = index.CharIndex + (char.IsHighSurrogate(Source[index.CharIndex]) ? 2 : 1);
        if (Source.Length <= newCharIndex)
        {
            return false;
        }
        index = new(newCharIndex, newRuneIndex);
        return true;
    }

    /// <inheritdoc />
    public bool TryDecrement(ref CharsIndex index)
    {
        if(index.CharIndex == 0)
        {
            return false;
        }
        var newRuneIndex = index.RuneIndex - 1;
        var newCharIndex = index.CharIndex - 1;
        if (char.IsLowSurrogate(Source[newCharIndex]))
        {
            --newCharIndex;
        }
        index = new(newCharIndex, newRuneIndex);
        return true;
    }

    /// <inheritdoc />
    public override string ToString() =>
        Source.ToString();
}


/// <summary>
/// Represents an enumerable collection of Unicode runes backed by a read-only memory of <see cref="char"/>.
/// </summary>
/// <param name="source"></param>
[RuneString]
public readonly partial struct CharsMemoryString(ReadOnlyMemory<char> source)
    : IRuneString<CharsMemoryString, CharsMemoryEnumerator, CharsIndex>
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
    public CharsMemoryString Slice(CharsIndex start, CharsIndex end) =>
        new(Source.Slice(start.CharIndex, end.CharIndex - start.CharIndex));

    /// <inheritdoc />
    public bool TryGetRune(CharsIndex index, out Rune rune, out int codeUnitConsumed)
    {
        var result = Rune.DecodeFromUtf16(Source.Span.Slice(index.CharIndex), out rune, out codeUnitConsumed);
        return result == OperationStatus.Done;
    }

    /// <inheritdoc />
    public bool TryIncrement(ref CharsIndex index)
    {
        if(index.CharIndex >= Source.Length)
        {
            return false;
        }
        var newRuneIndex = index.RuneIndex + 1;
        var newCharIndex = index.CharIndex + (char.IsHighSurrogate(Source.Span[index.CharIndex]) ? 2 : 1);
        if (Source.Length <= newCharIndex)
        {
            return false;
        }
        index = new(newCharIndex, newRuneIndex);
        return true;
    }

    /// <inheritdoc />
    public bool TryDecrement(ref CharsIndex index)
    {
        if (index.CharIndex == 0)
        {
            return false;
        }
        var newRuneIndex = index.RuneIndex - 1;
        var newCharIndex = index.CharIndex - 1;
        if (char.IsLowSurrogate(Source.Span[newCharIndex]))
        {
            --newCharIndex;
        }
        index = new(newCharIndex, newRuneIndex);
        return true;
    }

    /// <inheritdoc />
    public override string ToString() =>
        Source.ToString();
}


/// <summary>
/// Represents an enumerable collection of Unicode runes backed by a read-only span of <see cref="Rune"/>.
/// </summary>
/// <param name="source"></param>
[RuneString]
public readonly ref partial struct RunesSpanString(ReadOnlySpan<Rune> source)
    : IRuneString<RunesSpanString, RunesSpanEnumerator, RunesIndex>
{
    #region source generated members

    public partial Rune this[RunesIndex index] { get; }
    public partial bool TryGetRune(RunesIndex index, out Rune rune);

    #endregion

    /// <summary>
    /// Gets the source data as a read-only span of characters, which serves as the underlying buffer for enumerating Unicode runes in this enumerable.
    /// </summary>
    public ReadOnlySpan<Rune> Source { get; } = source;

    /// <inheritdoc />
    public RunesSpanEnumerator GetEnumerator() =>
        RunesSpanEnumerator.Create(Source);

    /// <inheritdoc />
    public RunesSpanString Slice(RunesIndex start, RunesIndex end) =>
        new(Source.Slice(start.RuneIndex, end.RuneIndex - start.RuneIndex));

    /// <inheritdoc />
    public bool TryGetRune(RunesIndex index, out Rune rune, out int codeUnitConsumed)
    {
        if((uint)index.RuneIndex >= (uint)Source.Length)
        {
            rune = default;
            codeUnitConsumed = 0;
            return false;
        }
        rune = Source[index.RuneIndex];
        codeUnitConsumed = 1;
        return true;
    }

    /// <inheritdoc />
    public bool TryIncrement(ref RunesIndex index)
    {
        var newIndex = index.RuneIndex + 1;
        if(Source.Length <= newIndex)
        {
            return false;
        }
        index = new(newIndex);
        return true;
    }

    /// <inheritdoc />
    public bool TryDecrement(ref RunesIndex index)
    {
        if(index.RuneIndex <= 0)
        {
            return false;
        }
        index = new(index.RuneIndex - 1);
        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var len = 0;
        foreach (var rune in Source)
        {
            len += rune.Utf16SequenceLength;
        }
        return string.Create(len, Source, static (span, source) =>
        {
            foreach (var rune in source)
            {
                span = span.Slice(rune.EncodeToUtf16(span));
            }
        });
    }
}


/// <summary>
/// Represents an enumerable collection of Unicode runes backed by a read-only memory of <see cref="Rune"/>.
/// </summary>
/// <param name="source"></param>
[RuneString]
public readonly partial struct RunesMemoryString(ReadOnlyMemory<Rune> source)
    : IRuneString<RunesMemoryString, RunesMemoryEnumerator, RunesIndex>
{
    #region source generated members

    public partial Rune this[RunesIndex index] { get; }
    public partial bool TryGetRune(RunesIndex index, out Rune rune);

    #endregion
    /// <summary>
    /// Gets the source data as a read-only span of characters, which serves as the underlying buffer for enumerating Unicode runes in this enumerable.
    /// </summary>
    public ReadOnlyMemory<Rune> Source { get; } = source;

    /// <inheritdoc />
    public RunesMemoryEnumerator GetEnumerator() =>
        RunesMemoryEnumerator.Create(Source);

    /// <inheritdoc />
    public RunesMemoryString Slice(RunesIndex start, RunesIndex end) =>
        new(Source.Slice(start.RuneIndex, end.RuneIndex - start.RuneIndex));

    /// <inheritdoc />
    public bool TryGetRune(RunesIndex index, out Rune rune, out int codeUnitConsumed)
    {
        if ((uint)index.RuneIndex >= (uint)Source.Length)
        {
            rune = default;
            codeUnitConsumed = 0;
            return false;
        }
        rune = Source.Span[index.RuneIndex];
        codeUnitConsumed = 1;
        return true;
    }

    /// <inheritdoc />
    public bool TryIncrement(ref RunesIndex index)
    {
        var newIndex = index.RuneIndex + 1;
        if (Source.Length <= newIndex)
        {
            return false;
        }
        index = new(newIndex);
        return true;
    }

    /// <inheritdoc />
    public bool TryDecrement(ref RunesIndex index)
    {
        if (index.RuneIndex <= 0)
        {
            return false;
        }
        index = new(index.RuneIndex - 1);
        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var len = 0;
        foreach (var rune in Source.Span)
        {
            len += rune.Utf16SequenceLength;
        }
        return string.Create(len, Source.Span, static (span, source) =>
        {
            foreach (var rune in source)
            {
                span = span.Slice(rune.EncodeToUtf16(span));
            }
        });
    }
}
