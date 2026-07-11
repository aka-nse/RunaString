using System.Buffers;
using System.Text;

namespace RunaString;

/// <summary>
/// Extensions for <see cref="ReadOnlySpan{Char}"/>, <see cref="ReadOnlyMemory{Char}"/>,
/// <see cref="ReadOnlySpan{Rune}"/>, and <see cref="ReadOnlyMemory{Rune}"/>.
/// </summary>
public static class RuneEnumerable
{
    extension(ReadOnlySpan<char> source)
    {
        /// <summary>
        /// Creates a <see cref="CharsSpanEnumerable"/> from the given read-only span of <see cref="char"/>.
        /// </summary>
        /// <returns></returns>
        public CharsSpanEnumerable AsRuneEnumerable() => new(source);
    }

    extension(string source)
    {
        /// <summary>
        /// Creates a <see cref="CharsMemoryEnumerable"/> from the given read-only span of string.
        /// </summary>
        /// <returns></returns>
        public CharsMemoryEnumerable AsRuneEnumerable() => source.AsMemory().AsRuneEnumerable();
    }

    extension(ReadOnlyMemory<char> source)
    {
        /// <summary>
        /// Creates a <see cref="CharsMemoryEnumerable"/> from the given read-only span of <see cref="char"/>.
        /// </summary>
        /// <returns></returns>
        public CharsMemoryEnumerable AsRuneEnumerable() => new(source);
    }

    extension(ReadOnlySpan<Rune> source)
    {
        /// <summary>
        /// Creates a <see cref="Utf32SpanEnumerable"/> from the given read-only span of UTF-32 characters.
        /// </summary>
        /// <returns></returns>
        public Utf32SpanEnumerable AsRuneEnumerable() => new(source);
    }

    extension(ReadOnlyMemory<Rune> source)
    {
        /// <summary>
        /// Creates a <see cref="Utf32MemoryEnumerable"/> from the given read-only span of UTF-32 characters.
        /// </summary>
        /// <returns></returns>
        public Utf32MemoryEnumerable AsRuneEnumerable() => new(source);
    }
}


/// <summary>
/// Represents an enumerable collection of Unicode runes backed by a read-only span of <see cref="char"/>.
/// </summary>
/// <param name="source"></param>
[RuneEnumerable]
public readonly ref partial struct CharsSpanEnumerable(ReadOnlySpan<char> source)
    : IRuneString<CharsSpanEnumerable, CharsSpanEnumerator, CharsRuneIndex>
{
    #region source generated members

    public partial Rune this[CharsRuneIndex index] { get; }
    public partial bool TryGetRune(CharsRuneIndex index, out Rune rune);

    #endregion

    /// <summary>
    /// Gets the source data as a read-only span of characters, which serves as the underlying buffer for enumerating Unicode runes in this enumerable.
    /// </summary>
    public ReadOnlySpan<char> Source { get; } = source;

    /// <inheritdoc />
    public CharsSpanEnumerator GetEnumerator() =>
        CharsSpanEnumerator.Create(Source);

    /// <inheritdoc />
    public CharsSpanEnumerable Slice(CharsRuneIndex start, CharsRuneIndex end) =>
        new (Source.Slice(start.CharIndex, end.CharIndex - start.CharIndex));

    /// <inheritdoc />
    public bool TryGetRune(CharsRuneIndex index, out Rune rune, out int codeUnitConsumed)
    {
        var result = Rune.DecodeFromUtf16(Source.Slice(index.CharIndex), out rune, out codeUnitConsumed);
        return result == OperationStatus.Done;
    }

    /// <inheritdoc />
    public bool TryIncrement(ref CharsRuneIndex index)
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
    public bool TryDecrement(ref CharsRuneIndex index)
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
[RuneEnumerable]
public readonly partial struct CharsMemoryEnumerable(ReadOnlyMemory<char> source)
    : IRuneString<CharsMemoryEnumerable, CharsMemoryEnumerator, CharsRuneIndex>
{
    #region source generated members

    public partial Rune this[CharsRuneIndex index] { get; }
    public partial bool TryGetRune(CharsRuneIndex index, out Rune rune);

    #endregion

    /// <summary>
    /// Gets the source data as a read-only span of characters, which serves as the underlying buffer for enumerating Unicode runes in this enumerable.
    /// </summary>
    public ReadOnlyMemory<char> Source { get; } = source;

    /// <inheritdoc />
    public CharsMemoryEnumerator GetEnumerator() =>
        CharsMemoryEnumerator.Create(Source);

    /// <inheritdoc />
    public CharsMemoryEnumerable Slice(CharsRuneIndex start, CharsRuneIndex end) =>
        new(Source.Slice(start.CharIndex, end.CharIndex - start.CharIndex));

    /// <inheritdoc />
    public bool TryGetRune(CharsRuneIndex index, out Rune rune, out int codeUnitConsumed)
    {
        var result = Rune.DecodeFromUtf16(Source.Span.Slice(index.CharIndex), out rune, out codeUnitConsumed);
        return result == OperationStatus.Done;
    }

    /// <inheritdoc />
    public bool TryIncrement(ref CharsRuneIndex index)
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
    public bool TryDecrement(ref CharsRuneIndex index)
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
/// Represents an enumerable collection of Unicode runes backed by a read-only span of UTF-32 characters.
/// </summary>
/// <param name="source"></param>
[RuneEnumerable]
public readonly ref partial struct Utf32SpanEnumerable(ReadOnlySpan<Rune> source)
    : IRuneString<Utf32SpanEnumerable, Utf32SpanEnumerator, Utf32RuneIndex>
{
    #region source generated members

    public partial Rune this[Utf32RuneIndex index] { get; }
    public partial bool TryGetRune(Utf32RuneIndex index, out Rune rune);

    #endregion

    /// <summary>
    /// Gets the source data as a read-only span of characters, which serves as the underlying buffer for enumerating Unicode runes in this enumerable.
    /// </summary>
    public ReadOnlySpan<Rune> Source { get; } = source;

    /// <inheritdoc />
    public Utf32SpanEnumerator GetEnumerator() =>
        Utf32SpanEnumerator.Create(Source);

    /// <inheritdoc />
    public Utf32SpanEnumerable Slice(Utf32RuneIndex start, Utf32RuneIndex end) =>
        new(Source.Slice(start.RuneIndex, end.RuneIndex - start.RuneIndex));

    /// <inheritdoc />
    public bool TryGetRune(Utf32RuneIndex index, out Rune rune, out int codeUnitConsumed)
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
    public bool TryIncrement(ref Utf32RuneIndex index)
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
    public bool TryDecrement(ref Utf32RuneIndex index)
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
/// Represents an enumerable collection of Unicode runes backed by a read-only memory of UTF-32 characters.
/// </summary>
/// <param name="source"></param>
[RuneEnumerable]
public readonly partial struct Utf32MemoryEnumerable(ReadOnlyMemory<Rune> source)
    : IRuneString<Utf32MemoryEnumerable, Utf32MemoryEnumerator, Utf32RuneIndex>
{
    #region source generated members

    public partial Rune this[Utf32RuneIndex index] { get; }
    public partial bool TryGetRune(Utf32RuneIndex index, out Rune rune);

    #endregion
    /// <summary>
    /// Gets the source data as a read-only span of characters, which serves as the underlying buffer for enumerating Unicode runes in this enumerable.
    /// </summary>
    public ReadOnlyMemory<Rune> Source { get; } = source;

    /// <inheritdoc />
    public Utf32MemoryEnumerator GetEnumerator() =>
        Utf32MemoryEnumerator.Create(Source);

    /// <inheritdoc />
    public Utf32MemoryEnumerable Slice(Utf32RuneIndex start, Utf32RuneIndex end) =>
        new(Source.Slice(start.RuneIndex, end.RuneIndex - start.RuneIndex));

    /// <inheritdoc />
    public bool TryGetRune(Utf32RuneIndex index, out Rune rune, out int codeUnitConsumed)
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
    public bool TryIncrement(ref Utf32RuneIndex index)
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
    public bool TryDecrement(ref Utf32RuneIndex index)
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
