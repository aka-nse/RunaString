using System.Text;

namespace RunaString.Test;

public partial class RunaStringTest
{
    public static TheoryData<Utf8TestCase>  FromRunesAndStringUtf8TestCases() => [.. TestHelpers.Utf8TestCases];
    public static TheoryData<CharsTestCase> FromRunesAndStringCharsTestCases() => [.. TestHelpers.CharsTestCases];
    public static TheoryData<RunesTestCase> FromRunesAndStringRunesTestCases() => [.. TestHelpers.RunesTestCases];


    private static void FromRunesCore<TString, TIndex>(ReadOnlySpan<Rune> runes, TString expected, TIndex _)
        where TString : IRunaString<TString, TIndex>, allows ref struct
        where TIndex : unmanaged, IRunaIndex<TIndex>
    {
        var actual = TString.FromRunes(runes);
        AssertEx.Equal(expected, actual);
    }


    [Theory]
    [MemberData(nameof(FromRunesAndStringUtf8TestCases))]
    public void FromRunesUtf8(Utf8TestCase testCase)
    {
        Rune[] runes = [.. testCase.String.EnumerateRunes()];
        FromRunesCore(runes, testCase.GetMemoryString(), testCase.GetMemoryString().IncrementBegin);
        FromRunesCore(runes, testCase.GetSpanString(), testCase.GetSpanString().IncrementBegin);
    }


    [Theory]
    [MemberData(nameof(FromRunesAndStringCharsTestCases))]
    public void FromRunesChars(CharsTestCase testCase)
    {
        Rune[] runes = [.. testCase.String.EnumerateRunes()];
        FromRunesCore(runes, testCase.GetMemoryString(), testCase.GetMemoryString().IncrementBegin);
        FromRunesCore(runes, testCase.GetSpanString(), testCase.GetSpanString().IncrementBegin);
    }


    [Theory]
    [MemberData(nameof(FromRunesAndStringRunesTestCases))]
    public void FromRunesRunes(RunesTestCase testCase)
    {
        Rune[] runes = [.. testCase.String.EnumerateRunes()];
        FromRunesCore(runes, testCase.GetMemoryString(), testCase.GetMemoryString().IncrementBegin);
        FromRunesCore(runes, testCase.GetSpanString(), testCase.GetSpanString().IncrementBegin);
    }


    private static void FromStringCore<TString, TIndex>(string str, TString expected, TIndex _)
        where TString : IRunaString<TString, TIndex>, allows ref struct
        where TIndex : unmanaged, IRunaIndex<TIndex>
    {
        var actual = TString.FromString(str);
        AssertEx.Equal(expected, actual);
    }


    [Theory]
    [MemberData(nameof(FromRunesAndStringUtf8TestCases))]
    public void FromStringUtf8(Utf8TestCase testCase)
    {
        FromStringCore(testCase.String, testCase.GetMemoryString(), testCase.GetMemoryString().IncrementBegin);
        FromStringCore(testCase.String, testCase.GetSpanString(), testCase.GetSpanString().IncrementBegin);
    }


    [Theory]
    [MemberData(nameof(FromRunesAndStringCharsTestCases))]
    public void FromStringChars(CharsTestCase testCase)
    {
        FromStringCore(testCase.String, testCase.GetMemoryString(), testCase.GetMemoryString().IncrementBegin);
        FromStringCore(testCase.String, testCase.GetSpanString(), testCase.GetSpanString().IncrementBegin);
    }


    [Theory]
    [MemberData(nameof(FromRunesAndStringRunesTestCases))]
    public void FromStringRunes(RunesTestCase testCase)
    {
        FromStringCore(testCase.String, testCase.GetMemoryString(), testCase.GetMemoryString().IncrementBegin);
        FromStringCore(testCase.String, testCase.GetSpanString(), testCase.GetSpanString().IncrementBegin);
    }
}
