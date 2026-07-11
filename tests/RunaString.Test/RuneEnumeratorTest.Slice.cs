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
        SliceTestCore<Utf8Span, Utf8SpanEnumerator, Utf8RuneIndex, ReadOnlySpan<byte>>(span, startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceTestCases))]
    public void SliceUtf8Memory(string value, int startIndex, int endIndex, string expected)
    {
        var utf8 = Encoding.UTF8.GetBytes(value);
        var text = Utf8String.FromUtf8([.. utf8], 0, utf8.Length);
        SliceTestCore<Utf8String, Utf8MemoryEnumerator, Utf8RuneIndex, ReadOnlyMemory<byte>>(text, startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceTestCases))]
    public void SliceUtf16Span(string value, int startIndex, int endIndex, string expected)
    {
        SliceTestCore<Utf16SpanEnumerable, Utf16SpanEnumerator, Utf16RuneIndex, ReadOnlySpan<char>>(value.AsSpan().AsRuneEnumerable(), startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceTestCases))]
    public void SliceUtf16Memory(string value, int startIndex, int endIndex, string expected)
    {
        SliceTestCore<Utf16MemoryEnumerable, Utf16MemoryEnumerator, Utf16RuneIndex, ReadOnlyMemory<char>>(value.AsRuneEnumerable(), startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceTestCases))]
    public void SliceUtf32Span(string value, int startIndex, int endIndex, string expected)
    {
        var utf32 = value.EnumerateRunes().ToImmutableArray();
        var span = utf32.AsSpan().AsRuneEnumerable();
        SliceTestCore<Utf32SpanEnumerable, Utf32SpanEnumerator, Utf32RuneIndex, ReadOnlySpan<Rune>>(span, startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceTestCases))]
    public void SliceUtf32Memory(string value, int startIndex, int endIndex, string expected)
    {
        var utf32 = value.EnumerateRunes().ToImmutableArray();
        var memory = utf32.AsMemory().AsRuneEnumerable();
        SliceTestCore<Utf32MemoryEnumerable, Utf32MemoryEnumerator, Utf32RuneIndex, ReadOnlyMemory<Rune>>(memory, startIndex, endIndex, expected);
    }
}
