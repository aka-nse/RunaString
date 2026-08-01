namespace RunaString.Test;

public partial class RunaStringTest
{
    public static TheoryData<Utf8TestCase>  GetRuneCountUtf8TestCases() => [.. TestHelpers.Utf8TestCases];
    public static TheoryData<CharsTestCase> GetRuneCountCharsTestCases() => [.. TestHelpers.CharsTestCases];
    public static TheoryData<RunesTestCase> GetRuneCountRunesTestCases() => [.. TestHelpers.RunesTestCases];

    [Theory]
    [MemberData(nameof(GetRuneCountUtf8TestCases))]
    public void TestGetRuneCountUtf8(Utf8TestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        Assert.Equal(testCase.RuneLength, memory.GetRuneCount());
        Assert.Equal(testCase.RuneLength, span.GetRuneCount());
    }

    [Theory]
    [MemberData(nameof(GetRuneCountCharsTestCases))]
    public void TestGetRuneCountChars(CharsTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        Assert.Equal(testCase.RuneLength, memory.GetRuneCount());
        Assert.Equal(testCase.RuneLength, span.GetRuneCount());
    }

    [Theory]
    [MemberData(nameof(GetRuneCountRunesTestCases))]
    public void TestGetRuneCountRunes(RunesTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        Assert.Equal(testCase.RuneLength, memory.GetRuneCount());
        Assert.Equal(testCase.RuneLength, span.GetRuneCount());
    }
}
