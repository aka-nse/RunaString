namespace RunaString.Test;

public partial class RunaStringTest
{
    private static void EqualityTestCore_Equal<TString, TEnumerator, TIndex>(TString value1, TString value2)
        where TString : struct, IRunaString<TString, TEnumerator, TIndex>, allows ref struct
        where TEnumerator : struct, IRunaEnumerator<TEnumerator, TIndex>, allows ref struct
        where TIndex : IRunaIndex
    {
        Assert.Equal(value1.ToString(), value2.ToString());
        Assert.True(TString.Equals(value1, value2));
        Assert.True(value1.Equals(value2));
        Assert.True(value1 == value2);
        Assert.False(value1 != value2);

        Assert.Equal(0, TString.Compare(value1, value2));
        Assert.Equal(0, value1.CompareTo(value2));
    }

    private static void EqualityTestCore_NotEqual<TString, TEnumerator, TIndex>(TString value1, TString value2)
        where TString : struct, IRunaString<TString, TEnumerator, TIndex>, allows ref struct
        where TEnumerator : struct, IRunaEnumerator<TEnumerator, TIndex>, allows ref struct
        where TIndex : IRunaIndex
    {
        Assert.NotEqual(value1.ToString(), value2.ToString());
        Assert.False(value1.Equals(value2));
        Assert.False(value1 == value2);
        Assert.True(value1 != value2);

        Assert.NotEqual(0, TString.Compare(value1, value2));
        Assert.NotEqual(0, value1.CompareTo(value2));

        Assert.NotEqual(TString.Compare(value1, value2) < 0, TString.Compare(value2, value1) < 0);
        Assert.NotEqual(value1.CompareTo(value2) < 0, value2.CompareTo(value1) < 0);
    }


    [Fact]
    public void EqualityUtf8()
    {
        foreach (var value1 in TestHelpers.Utf8TestCases)
        {
            foreach (var value2 in TestHelpers.Utf8TestCases)
            {
                if (value1.String == value2.String)
                {
                    EqualityTestCore_Equal<Utf8String, Utf8MemoryEnumerator, Utf8Index>(value1.GetMemoryString(), value2.GetMemoryString());
                    EqualityTestCore_Equal<Utf8SpanString, Utf8SpanEnumerator, Utf8Index>(value1.GetSpanString(), value2.GetSpanString());
                }
                else
                {
                    EqualityTestCore_NotEqual<Utf8String, Utf8MemoryEnumerator, Utf8Index>(value1.GetMemoryString(), value2.GetMemoryString());
                    EqualityTestCore_NotEqual<Utf8SpanString, Utf8SpanEnumerator, Utf8Index>(value1.GetSpanString(), value2.GetSpanString());
                }
            }
        }
    }


    [Fact]
    public void EqualityChars()
    {
        foreach(var value1 in TestHelpers.CharsTestCases)
        {
            foreach(var value2 in TestHelpers.CharsTestCases)
            {
                if (value1.String == value2.String)
                {
                    EqualityTestCore_Equal<CharsString, CharsMemoryEnumerator, CharsIndex>(value1.GetMemoryString(), value2.GetMemoryString());
                    EqualityTestCore_Equal<CharsSpanString, CharsSpanEnumerator, CharsIndex>(value1.GetSpanString(), value2.GetSpanString());
                }
                else
                {
                    EqualityTestCore_NotEqual<CharsString, CharsMemoryEnumerator, CharsIndex>(value1.GetMemoryString(), value2.GetMemoryString());
                    EqualityTestCore_NotEqual<CharsSpanString, CharsSpanEnumerator, CharsIndex>(value1.GetSpanString(), value2.GetSpanString());
                }
            }
        }
    }


    [Fact]
    public void EqualityRunes()
    {
        foreach (var value1 in TestHelpers.RunesTestCases)
        {
            foreach (var value2 in TestHelpers.RunesTestCases)
            {
                if (value1.String == value2.String)
                {
                    EqualityTestCore_Equal<RunesString, RunesMemoryEnumerator, RunesIndex>(value1.GetMemoryString(), value2.GetMemoryString());
                    EqualityTestCore_Equal<RunesSpanString, RunesSpanEnumerator, RunesIndex>(value1.GetSpanString(), value2.GetSpanString());
                }
                else
                {
                    EqualityTestCore_NotEqual<RunesString, RunesMemoryEnumerator, RunesIndex>(value1.GetMemoryString(), value2.GetMemoryString());
                    EqualityTestCore_NotEqual<RunesSpanString, RunesSpanEnumerator, RunesIndex>(value1.GetSpanString(), value2.GetSpanString());
                }
            }
        }
    }
}
