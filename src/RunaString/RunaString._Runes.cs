using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace RunaString;

partial class RunaString
{
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
        /// Creates a <see cref="RunesString"/> from the given read-only span of <see cref="Rune"/>.
        /// </summary>
        /// <returns></returns>
        public RunesString AsRunaString() => new(source);
    }
}


/// <summary>
/// Represents an enumerable collection of Unicode runes backed by a read-only memory of <see cref="Rune"/>.
/// </summary>
/// <param name="source"></param>
[RunaString]
public readonly partial struct RunesString(ReadOnlyMemory<Rune> source)
    : IRunaString<RunesString, RunesMemoryEnumerator, RunesIndex>
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
    public RunesString Slice(int runeStart, int runeLength) =>
        new(Source.Slice(runeStart, runeLength));

    /// <inheritdoc />
    public RunesString Slice(RunesIndex start, RunesIndex end) =>
        new(Source.Slice(start.RuneIndex, end.RuneIndex - start.RuneIndex));

    /// <inheritdoc />
    public bool TryGetRune(RunesIndex index, out Rune rune, out int codeUnitConsumed) =>
        FileHelpers.TryGetRune(Source.Span, index, out rune, out codeUnitConsumed);

    /// <inheritdoc />
    public RunesIndex Increment(ref RunesIndex index) =>
        FileHelpers.Increment(Source.Span, ref index);

    /// <inheritdoc />
    public RunesIndex Decrement(ref RunesIndex index) =>
        FileHelpers.Decrement(Source.Span, ref index);

    /// <inheritdoc />
    public bool IsInRange(RunesIndex index) =>
        FileHelpers.IsInRange(Source.Span, index);

    /// <inheritdoc />
    public RunesMemoryEnumerator GetEnumerator() =>
        RunesMemoryEnumerator.Create(Source);

    /// <inheritdoc />
    public int GetRuneCount() => Source.Length;

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Utf8String other && Equals(this, other);

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

    /// <inheritdoc />
    public override int GetHashCode() => InternalHelpers.GetHashCode(Source.Span);

    /// <inheritdoc />
    public int CompareTo(RunesString other) => Compare(this, other);

    /// <inheritdoc />
    public bool Equals(RunesString other) => Equals(this, other);

    /// <inheritdoc />
    public static bool Equals(RunesString x, RunesString y) =>
        InternalHelpers.Equals(x.Source.Span, y.Source.Span);

    /// <inheritdoc />
    public static int Compare(RunesString x, RunesString y) =>
        InternalHelpers.Compare<RunesString, RunesMemoryEnumerator>(x, y);

    /// <inheritdoc />
    public static bool operator ==(RunesString x, RunesString y) => Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(RunesString x, RunesString y) => !Equals(x, y);
}


/// <summary>
/// Represents an enumerable collection of Unicode runes backed by a read-only span of <see cref="Rune"/>.
/// </summary>
/// <param name="source"></param>
[RunaString]
public readonly ref partial struct RunesSpanString(ReadOnlySpan<Rune> source)
    : IRunaString<RunesSpanString, RunesSpanEnumerator, RunesIndex>
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
    public RunesSpanString Slice(int runeStart, int runeLength) =>
        new(Source.Slice(runeStart, runeLength));

    /// <inheritdoc />
    public RunesSpanString Slice(RunesIndex start, RunesIndex end) =>
        new(Source.Slice(start.RuneIndex, end.RuneIndex - start.RuneIndex));

    /// <inheritdoc />
    public bool TryGetRune(RunesIndex index, out Rune rune, out int codeUnitConsumed) =>
        FileHelpers.TryGetRune(Source, index, out rune, out codeUnitConsumed);

    /// <inheritdoc />
    public RunesIndex Increment(ref RunesIndex index) =>
        FileHelpers.Increment(Source, ref index);

    /// <inheritdoc />
    public RunesIndex Decrement(ref RunesIndex index) =>
        FileHelpers.Decrement(Source, ref index);

    /// <inheritdoc />
    public bool IsInRange(RunesIndex index) =>
        FileHelpers.IsInRange(Source, index);

    /// <inheritdoc />
    public RunesSpanEnumerator GetEnumerator() =>
        RunesSpanEnumerator.Create(Source);

    /// <inheritdoc />
    public int GetRuneCount() => Source.Length;

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => false;

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

    /// <inheritdoc />
    public override int GetHashCode() => InternalHelpers.GetHashCode(Source);

    /// <inheritdoc />
    public int CompareTo(RunesSpanString other) => Compare(this, other);

    /// <inheritdoc />
    public bool Equals(RunesSpanString other) => Equals(this, other);

    /// <inheritdoc />
    public static bool Equals(RunesSpanString x, RunesSpanString y) =>
        InternalHelpers.Equals(x.Source, y.Source);

    /// <inheritdoc />
    public static int Compare(RunesSpanString x, RunesSpanString y) =>
        InternalHelpers.Compare<RunesSpanString, RunesSpanEnumerator>(x, y);

    /// <inheritdoc />
    public static bool operator ==(RunesSpanString x, RunesSpanString y) => Equals(x, y);

    /// <inheritdoc />
    public static bool operator !=(RunesSpanString x, RunesSpanString y) => !Equals(x, y);

    /// <summary>
    /// Defines an implicit conversion from <see cref="RunesString" /> to <see cref="RunesSpanString" /> that creates a new <see cref="RunesSpanString" /> representing the UTF-8 encoded string.
    /// </summary>
    /// <param name="str"></param>
    public static implicit operator RunesSpanString(RunesString str) =>
        new(str.Source.Span);
}


file static class FileHelpers
{
    public static bool TryGetRune(ReadOnlySpan<Rune> source, RunesIndex index, out Rune rune, out int codeUnitConsumed)
    {
        if ((uint)index.RuneIndex >= (uint)source.Length)
        {
            rune = default;
            codeUnitConsumed = 0;
            return false;
        }
        rune = source[index.RuneIndex];
        codeUnitConsumed = 1;
        return true;
    }

    public static RunesIndex Increment(ReadOnlySpan<Rune> source, ref RunesIndex index)
    {
        var newIndex = index.RuneIndex + 1;
        if (source.Length < newIndex)
        {
            return index;
        }
        return index = new(newIndex);
    }

    public static RunesIndex Decrement(ReadOnlySpan<Rune> source, ref RunesIndex index)
    {
        InternalHelpers.NoUse(source);

        if ((uint)index.RuneIndex <= 0)
        {
            return index = RunesIndex.DecrementEnd;
        }
        return index = new(index.RuneIndex - 1);
    }

    public static bool IsInRange(ReadOnlySpan<Rune> source, RunesIndex index) =>
        (uint)index.RuneIndex < (uint)source.Length;
}
