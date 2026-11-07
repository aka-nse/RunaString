using System.Buffers;
using System.Text;

namespace RunaString.Test;

public partial class RunaStringTest
{
    public static TheoryData<Utf8TestCase> StartsWithAndEndWithUtf8TestCases() => [.. TestHelpers.Utf8TestCases];
    public static TheoryData<CharsTestCase> StartsWithAndEndWithCharsTestCases() => [.. TestHelpers.CharsTestCases];
    public static TheoryData<RunesTestCase> StartsWithAndEndWithRunesTestCases() => [.. TestHelpers.RunesTestCases];


    [Theory]
    [MemberData(nameof(StartsWithAndEndWithUtf8TestCases))]
    public void StartsWithAndEndsWithUtf8_Failure(Utf8TestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        var enc = new UTF8Encoding(false, true);
        var testString = enc.GetBytes(TestHelpers.PatternGuaranteedNotToBeContainedWithInCommonTestStrings);
        Assert.False(memory.StartsWith(Utf8String.DangerousFromUtf8(testString)));
        Assert.False(span.StartsWith(Utf8String.DangerousFromUtf8(testString)));
        Assert.False(memory.EndsWith(Utf8String.DangerousFromUtf8(testString)));
        Assert.False(span.EndsWith(Utf8String.DangerousFromUtf8(testString)));
    }


    [Theory]
    [MemberData(nameof(StartsWithAndEndWithUtf8TestCases))]
    public void StartsWithUtf8_Success(Utf8TestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        var enc = new UTF8Encoding(false, true);
        var runes = new StringBuilder();
        foreach (var rune in testCase.String.EnumerateRunes())
        {
            runes.Append(rune);
            var testString = enc.GetBytes(runes.ToString());
            Assert.True(memory.StartsWith(Utf8String.DangerousFromUtf8(testString)));
            Assert.True(span.StartsWith(Utf8String.DangerousFromUtf8(testString)));
        }
    }

    [Theory]
    [MemberData(nameof(StartsWithAndEndWithUtf8TestCases))]
    public void EndsWithWithUtf8_Success(Utf8TestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        var enc = new UTF8Encoding(false, true);
        var runes = new StringBuilder();
        foreach (var rune in testCase.String.EnumerateRunes().Reverse())
        {
            runes.Insert(0, rune);
            var testString = enc.GetBytes(runes.ToString());
            Assert.True(memory.EndsWith(Utf8String.DangerousFromUtf8(testString)));
            Assert.True(span.EndsWith(Utf8SpanString.DangerousFromSpan(testString)));
        }
    }

    [Theory]
    [MemberData(nameof(StartsWithAndEndWithCharsTestCases))]
    public void StartsWithAndEndsWithChars_Failure(CharsTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        var testString = TestHelpers.PatternGuaranteedNotToBeContainedWithInCommonTestStrings.AsMemory();
        Assert.False(memory.StartsWith(CharsString.DangerousFromChars(testString)));
        Assert.False(span.StartsWith(CharsSpanString.DangerousFromChars(testString.Span)));
        Assert.False(memory.EndsWith(CharsString.DangerousFromChars(testString)));
        Assert.False(span.EndsWith(CharsSpanString.DangerousFromChars(testString.Span)));
    }

    [Theory]
    [MemberData(nameof(StartsWithAndEndWithCharsTestCases))]
    public void StartsWithAndEndsWithChars_Success(CharsTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        int charIndex = 0;
        while(Rune.DecodeFromUtf16(testCase.String.AsSpan(charIndex), out _, out var charsConsumed) == OperationStatus.Done)
        {
            charIndex += charsConsumed;
            var startsWithTestString = testCase.String.AsMemory(0, charIndex);
            var endsWithTestString = testCase.String.AsMemory(charIndex);
            Assert.True(memory.StartsWith(CharsString.DangerousFromChars(startsWithTestString)));
            Assert.True(span.StartsWith(CharsSpanString.DangerousFromChars(startsWithTestString.Span)));
            Assert.True(memory.EndsWith(CharsString.DangerousFromChars(endsWithTestString)));
            Assert.True(span.EndsWith(CharsSpanString.DangerousFromChars(endsWithTestString.Span)));
        }
    }

    [Theory]
    [MemberData(nameof(StartsWithAndEndWithRunesTestCases))]
    public void StartsWithAndEndsWithRunes_Failure(RunesTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        var testString = TestHelpers.PatternGuaranteedNotToBeContainedWithInCommonTestStrings.EnumerateRunes().ToArray().AsMemory();
        Assert.False(memory.StartsWith(new RunesString(testString)));
        Assert.False(span.StartsWith(new RunesSpanString(testString.Span)));
        Assert.False(memory.EndsWith(new RunesString(testString)));
        Assert.False(span.EndsWith(new RunesSpanString(testString.Span)));
    }

    [Theory]
    [MemberData(nameof(StartsWithAndEndWithRunesTestCases))]
    public void StartsWithAndEndsWithRunes_Success(RunesTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        for(var runeIndex = 0; runeIndex < testCase.Runes.Length; ++runeIndex)
        {
            var startsWithTestString = testCase.Runes.AsMemory().Slice(0, runeIndex);
            var endsWithTestString = testCase.Runes.AsMemory().Slice(runeIndex);
            Assert.True(memory.StartsWith(new RunesString(startsWithTestString)));
            Assert.True(span.StartsWith(new RunesSpanString(startsWithTestString.Span)));
            Assert.True(memory.EndsWith(new RunesString(endsWithTestString)));
            Assert.True(span.EndsWith(new RunesSpanString(endsWithTestString.Span)));
        }
    }
}
