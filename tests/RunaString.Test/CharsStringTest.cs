using System.Text;

namespace RunaString.Test;

public class CharsStringTest
{
    public static TheoryData<CharsTestCase> CharsStringTestCases() => [..TestHelpers.CharsTestCases];

    public static TheoryData<string, int, int, string> SliceTestCases() =>
        new()
        {
            { "", 0, 0, "" },
            { "Hello, world!", 0, 5, "Hello" },
            { "A😀B𐀀C", 1, 4, "😀B𐀀" },
            { "A😀B𐀀C", 0, 5, "A😀B𐀀C" },
            { "a\u0301b", 0, 2, "a\u0301" },
        };

    [Fact]
    public void Create_Throws_ForInvalidUtf16Sequence()
    {
        var value = "\uD800A";


        Assert.Throws<ArgumentException>(() =>
        {
            value.AsRunaString();
        });
        Assert.Throws<ArgumentException>(() =>
        {
            value.AsSpan().AsRunaString();
        });
    }

    [Theory]
    [MemberData(nameof(CharsStringTestCases))]
    public void ToString_ReturnsOriginalString(CharsTestCase testCase)
    {
        Assert.Equal(testCase.String, testCase.GetMemoryString().ToString());
        Assert.Equal(testCase.String, testCase.GetSpanString().ToString());
    }

    [Theory]
    [MemberData(nameof(CharsStringTestCases))]
    public void Source_PreservesUnderlyingBuffer(CharsTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();

        Assert.True(memory.Source.Span.SequenceEqual(testCase.GetSpan()));
        Assert.True(span.Source.SequenceEqual(testCase.GetSpan()));
    }

    [Theory]
    [MemberData(nameof(CharsStringTestCases))]
    public void GetEnumerator_EnumeratesExpectedRunes(CharsTestCase testCase)
    {
        EnumerateTestCore<CharsString, CharsMemoryEnumerator>(testCase.String, testCase.GetMemoryString());
        EnumerateTestCore<CharsSpanString, CharsSpanEnumerator>(testCase.String, testCase.GetSpanString());
    }

    [Theory]
    [MemberData(nameof(CharsStringTestCases))]
    public void TryGetRune_Indexer_AndIsInRange_WorkAtRuneBoundaries(CharsTestCase testCase)
    {
        ValidateCharsStringCore<CharsString, CharsMemoryEnumerator>(testCase, testCase.GetMemoryString());
        ValidateCharsStringCore<CharsSpanString, CharsSpanEnumerator>(testCase, testCase.GetSpanString());
    }

    [Theory]
    [MemberData(nameof(SliceTestCases))]
    public void Slice_ReturnsExpectedSubstring(string value, int start, int end, string expected)
    {
        var testCase = CharsTestCase.Create(value);

        Assert.Equal(
            expected,
            testCase.GetMemoryString()
                .Slice(testCase.GetIndex(new(start)), testCase.GetIndex(new(end)))
                .ToString());

        Assert.Equal(
            expected,
            testCase.GetSpanString()
                .Slice(testCase.GetIndex(new(start)), testCase.GetIndex(new(end)))
                .ToString());
    }

    private static void EnumerateTestCore<TString, TEnumerator>(string expected, TString value)
        where TString : IRunaEnumerable<TString, TEnumerator>, allows ref struct
        where TEnumerator : IRunaEnumerator<TEnumerator>, allows ref struct
    {
        var expectedEnumerator = expected.EnumerateRunes();
        var actualEnumerator = value.GetEnumerator();

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

    private static void ValidateCharsStringCore<TString, TEnumerator>(CharsTestCase testCase, TString value)
        where TString : IRunaString<TString, TEnumerator, CharsIndex>, allows ref struct
        where TEnumerator : IRunaEnumerator<TEnumerator>, allows ref struct
    {
        var expectedRunes = testCase.String.EnumerateRunes().ToArray();

        for (var i = 0; i < expectedRunes.Length; i++)
        {
            var index = testCase.GetIndex(new(i));
            var expectedRune = expectedRunes[i];

            Assert.True(value.IsInRange(index));
            Assert.True(value.TryGetRune(index, out var rune));
            Assert.Equal(expectedRune, rune);

            Assert.True(value.TryGetRune(index, out var runeWithConsumed, out var codeUnitConsumed));
            Assert.Equal(expectedRune, runeWithConsumed);
            Assert.Equal(expectedRune.Utf16SequenceLength, codeUnitConsumed);

            Assert.Equal(expectedRune, value[index]);
        }

        var endIndex = testCase.GetIndex(^0);

        Assert.False(value.IsInRange(endIndex));
        Assert.False(value.TryGetRune(endIndex, out _));

        Assert.False(value.TryGetRune(endIndex, out _, out var consumedAtEnd));
        Assert.Equal(0, consumedAtEnd);
    }
}
