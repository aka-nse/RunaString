using System.Collections.Immutable;
using System.Text;

namespace RunaString.Test;  

public partial class RuneEnumeratorTest
{
    public static TheoryData<string, int, int, string> SliceTestCases() =>
        new()
        {
            { "Hello, world!", 0, 0, "" },
            { "Hello, world!", 0, 5, "Hello" },
            { "Hello, world!", 7, 12, "world" },
            { "Hello, world!", 0, 13, "Hello, world!" },
        };

    private static void SliceTestCore<TEnumerable, TEnumerator, TIndex, TBuffer>(TEnumerable value, int start, int end, string expected)
        where TEnumerable : struct, IRuneString<TEnumerable, TEnumerator, TIndex>, allows ref struct
        where TEnumerator : struct, IRuneEnumerator<TEnumerator, TIndex, TBuffer>, allows ref struct
        where TIndex : ISeekIndex
        where TBuffer : struct, allows ref struct
    {
        var startEnumerator = TEnumerator.Empty;
        var endEnumerator = TEnumerator.Empty;
        var enumerator = value.GetEnumerator();
        var i = 0;
        while(true)
        {
            var result = enumerator.MoveNext();
            if (i == start)
            {
                startEnumerator = enumerator;
            }
            if (i == end)
            {
                endEnumerator = enumerator;
            }
            if(!result)
            {
                break;
            }
            ++i;
        }
        var actual = value.Slice(startEnumerator.SeekIndex, endEnumerator.SeekIndex).ToString();
        Assert.Equal(expected, actual);
    }


    [Theory, MemberData(nameof(SliceTestCases))]
    public void SliceUtf8Span(string value, int startIndex, int endIndex, string expected)
    {
        var utf8 = Encoding.UTF8.GetBytes(value);
        var span = Utf8Span.DangerousFromSpan(utf8);
        SliceTestCore<Utf8Span, Utf8SpanEnumerator, Utf8Index, ReadOnlySpan<byte>>(span, startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceTestCases))]
    public void SliceUtf8Memory(string value, int startIndex, int endIndex, string expected)
    {
        var utf8 = Encoding.UTF8.GetBytes(value);
        var text = Utf8String.FromUtf8([.. utf8], 0, utf8.Length);
        SliceTestCore<Utf8String, Utf8MemoryEnumerator, Utf8Index, ReadOnlyMemory<byte>>(text, startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceTestCases))]
    public void SliceCharsSpan(string value, int startIndex, int endIndex, string expected)
    {
        SliceTestCore<CharsSpanString, CharsSpanEnumerator, CharsIndex, ReadOnlySpan<char>>(value.AsSpan().AsRunaString(), startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceTestCases))]
    public void SliceCharsMemory(string value, int startIndex, int endIndex, string expected)
    {
        SliceTestCore<CharsMemoryString, CharsMemoryEnumerator, CharsIndex, ReadOnlyMemory<char>>(value.AsRunaString(), startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceTestCases))]
    public void SliceRunesSpan(string value, int startIndex, int endIndex, string expected)
    {
        var runes = value.EnumerateRunes().ToImmutableArray();
        var span = runes.AsSpan().AsRunaString();
        SliceTestCore<RunesSpanString, RunesSpanEnumerator, RunesIndex, ReadOnlySpan<Rune>>(span, startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceTestCases))]
    public void SliceRunesMemory(string value, int startIndex, int endIndex, string expected)
    {
        var runes = value.EnumerateRunes().ToImmutableArray();
        var memory = runes.AsMemory().AsRunaString();
        SliceTestCore<RunesMemoryString, RunesMemoryEnumerator, RunesIndex, ReadOnlyMemory<Rune>>(memory, startIndex, endIndex, expected);
    }
}
