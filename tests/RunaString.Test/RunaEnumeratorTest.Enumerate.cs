using System.Text;

namespace RunaString.Test;

public partial class RunaEnumeratorTest
{
    public static TheoryData<string> EnumerateTestCases() => [.. TestHelpers.CommonTestStrings];

    private static void EnumerateTestCore<TEnumerator>(string expected, TEnumerator actualEnumerator)
        where TEnumerator : IRunaEnumerator<TEnumerator>, allows ref struct
    {
        var expectedEnumerator = expected.EnumerateRunes();
        while (true)
        {
            var expectedMoveNext = expectedEnumerator.MoveNext();
            var actualMoveNext = actualEnumerator.MoveNext();
            Assert.Equal(expectedMoveNext, actualMoveNext);
            if (!expectedMoveNext)
            {
                break;
            }
            Assert.Equal(expectedEnumerator.Current, actualEnumerator.Current);
        }
    }

    [Theory, MemberData(nameof(EnumerateTestCases))]
    public void EnumerateUtf8Span(string value)
    {
        var utf8 = Encoding.UTF8.GetBytes(value);
        EnumerateTestCore(value, Utf8SpanEnumerator.Create(utf8));
    }

    [Theory, MemberData(nameof(EnumerateTestCases))]
    public void EnumerateUtf8Memory(string value)
    {
        var utf8 = Encoding.UTF8.GetBytes(value);
        EnumerateTestCore(value, Utf8MemoryEnumerator.Create(utf8));
    }

    [Theory, MemberData(nameof(EnumerateTestCases))]
    public void EnumerateCharsSpan(string value)
    {
        EnumerateTestCore(value, CharsSpanEnumerator.Create(value.AsSpan()));
    }

    [Theory, MemberData(nameof(EnumerateTestCases))]
    public void EnumerateCharsMemory(string value)
    {
        EnumerateTestCore(value, CharsMemoryEnumerator.Create(value.AsMemory()));
    }

    [Theory, MemberData(nameof(EnumerateTestCases))]
    public void EnumerateRunesSpan(string value)
    {
        var utf32 = value.EnumerateRunes().ToArray();
        EnumerateTestCore(value, RunesSpanEnumerator.Create(utf32));
    }
    [Theory, MemberData(nameof(EnumerateTestCases))]
    public void EnumerateRunesMemory(string value)
    {
        var utf32 = value.EnumerateRunes().ToArray();
        EnumerateTestCore(value, RunesMemoryEnumerator.Create(utf32));
    }
}
