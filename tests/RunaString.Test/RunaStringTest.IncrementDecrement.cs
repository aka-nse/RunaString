namespace RunaString.Test;

public partial class RunaStringTest
{
    // Provides test cases covering a variety of Unicode inputs (multiple languages, diacritics, combining marks, and emoji sequences)
    // for increment/decrement rune iteration tests. Uses well-known test strings where appropriate.

    public static TheoryData<Utf8TestCase> IncrementDecrementUtf8TestCases() => TestHelpers.Utf8TestCases;
    public static TheoryData<CharsTestCase> IncrementDecrementCharsTestCases() => TestHelpers.CharsTestCases;
    public static TheoryData<RunesTestCase> IncrementDecrementRunesTestCases() => TestHelpers.RunesTestCases;

    [Theory]
    [MemberData(nameof(IncrementDecrementUtf8TestCases))]
    public void TestIncrementUtf8(Utf8TestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        for(var runeIndex = 0; runeIndex < testCase.RuneLength - 1; runeIndex++)
        {
            var index = testCase.GetIndex(runeIndex);
            var nextIndex = testCase.GetIndex(runeIndex + 1);
            IncrementTestCore_True(memory, index, nextIndex);
            IncrementTestCore_True(span, index, nextIndex);
        }
        IncrementTestCore_False(memory, testCase.GetIndex(^1));
        IncrementTestCore_False(span, testCase.GetIndex(^1));
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementUtf8TestCases))]
    public void TestDecrementUtf8(Utf8TestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        for (var runeIndex = testCase.RuneLength; runeIndex > 0; runeIndex--)
        {
            var index = testCase.GetIndex(runeIndex);
            var nextIndex = testCase.GetIndex(runeIndex - 1);
            DecrementTestCore_True(memory, index, nextIndex);
            DecrementTestCore_True(span, index, nextIndex);
        }
        DecrementTestCore_False(memory, testCase.GetIndex(0));
        DecrementTestCore_False(span, testCase.GetIndex(0));
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementCharsTestCases))]
    public void TestIncrementChars(CharsTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        for (var runeIndex = 0; runeIndex < testCase.RuneLength - 1; runeIndex++)
        {
            var index = testCase.GetIndex(runeIndex);
            var nextIndex = testCase.GetIndex(runeIndex + 1);
            IncrementTestCore_True(memory, index, nextIndex);
            IncrementTestCore_True(span, index, nextIndex);
        }
        IncrementTestCore_False(memory, testCase.GetIndex(^1));
        IncrementTestCore_False(span, testCase.GetIndex(^1));
    }


    [Theory]
    [MemberData(nameof(IncrementDecrementCharsTestCases))]
    public void TestDecrementChars(CharsTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        for (var runeIndex = testCase.RuneLength; runeIndex > 0; runeIndex--)
        {
            var index = testCase.GetIndex(runeIndex);
            var nextIndex = testCase.GetIndex(runeIndex - 1);
            DecrementTestCore_True(memory, index, nextIndex);
            DecrementTestCore_True(span, index, nextIndex);
        }
        DecrementTestCore_False(memory, testCase.GetIndex(0));
        DecrementTestCore_False(span, testCase.GetIndex(0));
    }

    [Theory]
    [MemberData(nameof(IncrementDecrementRunesTestCases))]
    public void TestIncrementRunes(RunesTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        for (var runeIndex = 0; runeIndex < testCase.RuneLength - 1; runeIndex++)
        {
            var index = testCase.GetIndex(runeIndex);
            var nextIndex = testCase.GetIndex(runeIndex + 1);
            IncrementTestCore_True(memory, index, nextIndex);
            IncrementTestCore_True(span, index, nextIndex);
        }
        IncrementTestCore_False(memory, testCase.GetIndex(^1));
        IncrementTestCore_False(span, testCase.GetIndex(^1));
    }


    [Theory]
    [MemberData(nameof(IncrementDecrementRunesTestCases))]
    public void TestDecrementRunes(RunesTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        for (var runeIndex = testCase.RuneLength; runeIndex > 0; runeIndex--)
        {
            var index = testCase.GetIndex(runeIndex);
            var nextIndex = testCase.GetIndex(runeIndex - 1);
            DecrementTestCore_True(memory, index, nextIndex);
            DecrementTestCore_True(span, index, nextIndex);
        }
        DecrementTestCore_False(memory, testCase.GetIndex(0));
        DecrementTestCore_False(span, testCase.GetIndex(0));
    }


    private static void IncrementTestCore_True<TStr, TIndex>(TStr input, TIndex index, TIndex expected)
        where TStr : IRunaString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex<TIndex>
    {
        Assert.Equal(expected, input.Increment(ref index));
        Assert.Equal(expected, index);
    }

    private static void IncrementTestCore_False<TStr, TIndex>(TStr input, TIndex index)
        where TStr : IRunaString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex<TIndex>
    {
        input.Increment(ref index);
        Assert.False(input.IsInRange(index));
    }

    private static void DecrementTestCore_True<TStr, TIndex>(TStr input, TIndex index, TIndex expected)
        where TStr : IRunaString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex<TIndex>
    {
        Assert.Equal(expected, input.Decrement(ref index));
        Assert.Equal(expected, index);
    }

    private static void DecrementTestCore_False<TStr, TIndex>(TStr input, TIndex index)
        where TStr : IRunaString<TStr, TIndex>, allows ref struct
        where TIndex : struct, ISeekIndex<TIndex>
    {
        input.Decrement(ref index);
        Assert.False(input.IsInRange(index));
    }
}
