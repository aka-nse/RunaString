using System.Text;

namespace RunaString.Test;
using static TestHelpers;

public partial class RunaStringTest
{
    /// <summary>
    /// Provides test cases covering a variety of Unicode inputs (multiple languages, diacritics, combining marks, and emoji sequences)
    /// for increment/decrement rune iteration tests. Uses well-known test strings where appropriate.
    /// </summary>
    public static TheoryData<string, int> IncrementDecrementTestCases()
    {
        var retval = new TheoryData<string, int>();

        void core(string input, int runeIndex)
        {
            retval.Add(input, runeIndex);
        }

        foreach(var str in TestHelpers.CommonTestStrings)
        {
            var runeCount = str.EnumerateRunes().Count();
            for (int i = 0; i <= runeCount; i++)
            {
                core(str, i);
            }
        }

        return retval;
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestIncrementUtf8(string input, int runeIndex)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var charIndex = GetUtf8CodeUnitCount(input, 0, runeIndex);
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex + 1;
        var index = CreateUtf8Index(charIndex, runeIndex);
        var memory = Utf8String.DangerousFromUtf8([.. bytes], 0, bytes.Length);
        var span = Utf8Span.DangerousFromSpan(bytes);
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextCharIndex = GetUtf8CodeUnitCount(input, 0, nextRuneIndex);
            var nextIndex = CreateUtf8Index(nextCharIndex, nextRuneIndex);
            IncrementTestCore_True(
                memory,
                index,
                nextIndex);
            IncrementTestCore_True(
                span,
                index,
                nextIndex);
        }
        else
        {
            IncrementTestCore_False(
                memory,
                index);
            IncrementTestCore_False(
                span,
                index);
        }
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestIncrementChars(string input, int runeIndex)
    {
        var charIndex = GetCharsCodeUnitCount(input, 0, runeIndex);
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex + 1;
        var index = CreateCharsIndex(charIndex, runeIndex);
        var memory = new CharsMemoryString(input.AsMemory());
        var span = new CharsSpanString(input.AsSpan());
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextCharIndex = GetCharsCodeUnitCount(input, 0, nextRuneIndex);
            var nextIndex = CreateCharsIndex(nextCharIndex, nextRuneIndex);
            IncrementTestCore_True(memory, index, nextIndex);
            IncrementTestCore_True(span, index, nextIndex);
        }
        else
        {
            IncrementTestCore_False(memory, index);
            IncrementTestCore_False(span, index);
        }
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestIncrementRunes(string input, int runeIndex)
    {
        var runes = input.EnumerateRunes().ToArray();
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex + 1;
        var index = CreateRunesIndex(runeIndex);
        var memory = new RunesMemoryString(runes);
        var span = new RunesSpanString(runes);
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextIndex = CreateRunesIndex(nextRuneIndex);
            IncrementTestCore_True(memory, index, nextIndex);
            IncrementTestCore_True(span, index, nextIndex);
        }
        else
        {
            IncrementTestCore_False(memory, index);
            IncrementTestCore_False(span, index);
        }
    }

    private static void IncrementTestCore_True<TStr, TIndex>(TStr input, TIndex index, TIndex expected)
        where TStr : IRunaString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex
    {
        Assert.True(input.TryIncrement(ref index));
        Assert.Equal(expected, index);
    }

    private static void IncrementTestCore_False<TStr, TIndex>(TStr input, TIndex index)
        where TStr : IRunaString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex
    {
        Assert.False(input.TryIncrement(ref index));
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestDecrementUtf8(string input, int runeIndex)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var charIndex = GetUtf8CodeUnitCount(input, 0, runeIndex);
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex - 1;
        var index = CreateUtf8Index(charIndex, runeIndex);
        var memory = Utf8String.DangerousFromUtf8([.. bytes], 0, bytes.Length);
        var span = Utf8Span.DangerousFromSpan(bytes);
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextCharIndex = GetUtf8CodeUnitCount(input, 0, nextRuneIndex);
            var nextIndex = CreateUtf8Index(nextCharIndex, nextRuneIndex);
            DecrementTestCore_True(memory, index, nextIndex);
            DecrementTestCore_True(span, index, nextIndex);
        }
        else
        {
            DecrementTestCore_False(memory, index);
            DecrementTestCore_False(span, index);
        }
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestDecrementUtf16(string input, int runeIndex)
    {
        var charIndex = GetCharsCodeUnitCount(input, 0, runeIndex);
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex - 1;
        var index = CreateCharsIndex(charIndex, runeIndex);
        var memory = new CharsMemoryString(input.AsMemory());
        var span = new CharsSpanString(input.AsSpan());
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextCharIndex = GetCharsCodeUnitCount(input, 0, nextRuneIndex);
            var nextIndex = CreateCharsIndex(nextCharIndex, nextRuneIndex);
            DecrementTestCore_True(memory, index, nextIndex);
            DecrementTestCore_True(span, index, nextIndex);
        }
        else
        {
            DecrementTestCore_False(memory, index);
            DecrementTestCore_False(span, index);
        }
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementTestCases))]
    public void TestDecrementRunes(string input, int runeIndex)
    {
        var runes = input.EnumerateRunes().ToArray();
        var runeLength = input.EnumerateRunes().Count();
        var nextRuneIndex = runeIndex - 1;
        var index = CreateRunesIndex(runeIndex);
        var memory = new RunesMemoryString(runes);
        var span = new RunesSpanString(runes);
        if ((uint)nextRuneIndex < (uint)runeLength)
        {
            var nextIndex = CreateRunesIndex(nextRuneIndex);
            DecrementTestCore_True(memory, index, nextIndex);
            DecrementTestCore_True(span, index, nextIndex);
        }
        else
        {
            DecrementTestCore_False(memory, index);
            DecrementTestCore_False(span, index);
        }
    }

    private static void DecrementTestCore_True<TStr, TIndex>(TStr input, TIndex index, TIndex expected)
        where TStr : IRunaString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex
    {
        Assert.True(input.TryDecrement(ref index));
        Assert.Equal(expected, index);
    }

    private static void DecrementTestCore_False<TStr, TIndex>(TStr input, TIndex index)
        where TStr : IRunaString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex
    {
        Assert.False(input.TryDecrement(ref index));
    }
}
