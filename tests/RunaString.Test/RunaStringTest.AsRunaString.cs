namespace RunaString.Test;

public partial class RunaStringTest
{
    public static TheoryData<CharsTestCase> AsRunaStringCharsTestCases() => [..TestHelpers.CharsTestCases];
    public static TheoryData<RunesTestCase> AsRunaStringRunesTestCases() => [..TestHelpers.RunesTestCases];

    [Theory]
    [MemberData(nameof(AsRunaStringCharsTestCases))]
    public void ReadOnlySpanChar_AsRunaString_ReturnsCharsSpanString(CharsTestCase input)
    {
        var span = input.GetSpan();
        var runa = span.AsRunaString();
        Assert.Equal(input.String, runa.ToString());
    }

    [Theory]
    [MemberData(nameof(AsRunaStringCharsTestCases))]
    public void String_AsRunaString_ReturnsCharsString(CharsTestCase input)
    {
        var runa = input.Chars.AsRunaString();
        Assert.Equal(input.String, runa.ToString());
    }

    [Theory]
    [MemberData(nameof(AsRunaStringCharsTestCases))]
    public void ReadOnlyMemoryChar_AsRunaString_ReturnsCharsString(CharsTestCase input)
    {
        var memory = input.Chars.AsMemory();
        var runa = memory.AsRunaString();
        Assert.Equal(input.Chars, runa.ToString());
    }

    [Theory]
    [MemberData(nameof(AsRunaStringRunesTestCases))]
    public void ReadOnlySpanRune_AsRunaString_ReturnsRunesSpanString(RunesTestCase input)
    {
        var span = input.GetSpan();
        var runa = span.AsRunaString();
        Assert.Equal(input.String, runa.ToString());
    }

    [Theory]
    [MemberData(nameof(AsRunaStringRunesTestCases))]
    public void ReadOnlyMemoryRune_AsRunaString_ReturnsRunesString(RunesTestCase input)
    {
        var memory = input.GetMemory();
        var runa = memory.AsRunaString();
        Assert.Equal(input.String, runa.ToString());
    }
}
