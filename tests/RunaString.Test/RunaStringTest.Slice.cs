using System.Collections.Immutable;
using System.Text;

namespace RunaString.Test;

public partial class RunaStringTest
{
    public static TheoryData<string, int, string> SliceStartTestCases() =>
        new()
        {
            { "Hello, world!", 0, "Hello, world!" },
            { "Hello, world!", 7, "world!" },
            { "こんにちは、世界！", 0, "こんにちは、世界！"  },
            { "こんにちは、世界！", 6, "世界！" },
            { "👋🌍", 0, "👋🌍" },
            { "👋🌍", 1, "🌍" },
            {
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                335,
                "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            },
            {
                "いろはにほへと　ちりぬるを　わかよたれそ　つねならむ　うゐのおくやま　けふこえて　あさきゆめみし　ゑひもせす",
                41,
                "あさきゆめみし　ゑひもせす"
            }
        };

    private static void SliceStartTestCore<TString, TEnumerator, TIndex, TBuffer>(TString value, int start, string expected)
        where TString : struct, IRunaString<TString, TEnumerator, TIndex>, allows ref struct
        where TEnumerator : struct, IRunaEnumerator<TEnumerator, TIndex, TBuffer>, allows ref struct
        where TIndex : IRunaIndex
        where TBuffer : struct, allows ref struct
    {
        {
            // int index of Rune slicing
            var actual = value.Slice(start).ToString();
            Assert.Equal(expected, actual);
        }
        {
            // TIndex slicing
            var startEnumerator = TEnumerator.Empty;
            var enumerator = value.GetEnumerator();
            var i = 0;
            while (true)
            {
                var result = enumerator.MoveNext();
                if (i == start)
                {
                    startEnumerator = enumerator;
                }
                if (!result)
                {
                    break;
                }
                ++i;
            }
            var actual = value.Slice(startEnumerator.Index).ToString();
            Assert.Equal(expected, actual);
        }
    }

    [Theory, MemberData(nameof(SliceStartTestCases))]
    public void SliceStartUtf8Span(string value, int startIndex, string expected)
    {
        var utf8 = Encoding.UTF8.GetBytes(value);
        var span = Utf8SpanString.DangerousFromSpan(utf8);
        SliceStartTestCore<Utf8SpanString, Utf8SpanEnumerator, Utf8Index, ReadOnlySpan<byte>>(span, startIndex, expected);
    }

    [Theory, MemberData(nameof(SliceStartTestCases))]
    public void SliceStartUtf8Memory(string value, int startIndex, string expected)
    {
        var utf8 = Encoding.UTF8.GetBytes(value);
        var text = Utf8String.FromUtf8([.. utf8], 0, utf8.Length);
        SliceStartTestCore<Utf8String, Utf8MemoryEnumerator, Utf8Index, ReadOnlyMemory<byte>>(text, startIndex, expected);
    }

    [Theory, MemberData(nameof(SliceStartTestCases))]
    public void SliceStartCharsSpan(string value, int startIndex, string expected)
    {
        SliceStartTestCore<CharsSpanString, CharsSpanEnumerator, CharsIndex, ReadOnlySpan<char>>(value.AsSpan().AsRunaString(), startIndex, expected);
    }

    [Theory, MemberData(nameof(SliceStartTestCases))]
    public void SliceStartCharsMemory(string value, int startIndex, string expected)
    {
        SliceStartTestCore<CharsString, CharsMemoryEnumerator, CharsIndex, ReadOnlyMemory<char>>(value.AsRunaString(), startIndex, expected);
    }

    [Theory, MemberData(nameof(SliceStartTestCases))]
    public void SliceStartRunesSpan(string value, int startIndex, string expected)
    {
        var runes = value.EnumerateRunes().ToImmutableArray();
        var span = runes.AsSpan().AsRunaString();
        SliceStartTestCore<RunesSpanString, RunesSpanEnumerator, RunesIndex, ReadOnlySpan<Rune>>(span, startIndex, expected);
    }

    [Theory, MemberData(nameof(SliceStartTestCases))]
    public void SliceStartRunesMemory(string value, int startIndex, string expected)
    {
        var runes = value.EnumerateRunes().ToImmutableArray();
        var memory = runes.AsMemory().AsRunaString();
        SliceStartTestCore<RunesString, RunesMemoryEnumerator, RunesIndex, ReadOnlyMemory<Rune>>(memory, startIndex, expected);
    }


    public static TheoryData<string, int, int, string> SliceStartEndTestCases() =>
        new()
        {
            { "Hello, world!", 0, 0, "" },
            { "Hello, world!", 0, 5, "Hello" },
            { "Hello, world!", 7, 12, "world" },
            { "Hello, world!", 0, 13, "Hello, world!" },
            { "こんにちは、世界！", 0, 5, "こんにちは"  },
            { "こんにちは、世界！", 5, 7, "、世" },
            { "こんにちは、世界！", 7, 9, "界！" },
            { "こんにちは、世界！", 0, 9, "こんにちは、世界！" },
            { "👋🌍", 0, 1, "👋" },
            { "👋🌍", 1, 2, "🌍" },
            { "👋🌍", 0, 2, "👋🌍" },
            {
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                335,
                335 + 9,
                "Excepteur"
            },
            {
                "いろはにほへと　ちりぬるを　わかよたれそ　つねならむ　うゐのおくやま　けふこえて　あさきゆめみし　ゑひもせす",
                41,
                41+ 7,
                "あさきゆめみし"
            }
        };

    private static void SliceStartEndTestCore<TString, TEnumerator, TIndex, TBuffer>(TString value, int start, int end, string expected)
        where TString : struct, IRunaString<TString, TEnumerator, TIndex>, allows ref struct
        where TEnumerator : struct, IRunaEnumerator<TEnumerator, TIndex, TBuffer>, allows ref struct
        where TIndex : IRunaIndex
        where TBuffer : struct, allows ref struct
    {
        {
            // int index of Rune slicing
            var actual = value.Slice(start, end - start).ToString();
            Assert.Equal(expected, actual);
        }
        {
            // TIndex slicing
            var startEnumerator = TEnumerator.Empty;
            var endEnumerator = TEnumerator.Empty;
            var enumerator = value.GetEnumerator();
            var i = 0;
            while (true)
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
                if (!result)
                {
                    break;
                }
                ++i;
            }
            var actual = value.Slice(startEnumerator.Index, endEnumerator.Index).ToString();
            Assert.Equal(expected, actual);
        }
    }

    [Theory, MemberData(nameof(SliceStartEndTestCases))]
    public void SliceStartEndUtf8Span(string value, int startIndex, int endIndex, string expected)
    {
        var utf8 = Encoding.UTF8.GetBytes(value);
        var span = Utf8SpanString.DangerousFromSpan(utf8);
        SliceStartEndTestCore<Utf8SpanString, Utf8SpanEnumerator, Utf8Index, ReadOnlySpan<byte>>(span, startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceStartEndTestCases))]
    public void SliceStartEndUtf8Memory(string value, int startIndex, int endIndex, string expected)
    {
        var utf8 = Encoding.UTF8.GetBytes(value);
        var text = Utf8String.FromUtf8([.. utf8], 0, utf8.Length);
        SliceStartEndTestCore<Utf8String, Utf8MemoryEnumerator, Utf8Index, ReadOnlyMemory<byte>>(text, startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceStartEndTestCases))]
    public void SliceStartEndCharsSpan(string value, int startIndex, int endIndex, string expected)
    {
        SliceStartEndTestCore<CharsSpanString, CharsSpanEnumerator, CharsIndex, ReadOnlySpan<char>>(value.AsSpan().AsRunaString(), startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceStartEndTestCases))]
    public void SliceStartEndCharsMemory(string value, int startIndex, int endIndex, string expected)
    {
        SliceStartEndTestCore<CharsString, CharsMemoryEnumerator, CharsIndex, ReadOnlyMemory<char>>(value.AsRunaString(), startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceStartEndTestCases))]
    public void SliceStartEndRunesSpan(string value, int startIndex, int endIndex, string expected)
    {
        var runes = value.EnumerateRunes().ToImmutableArray();
        var span = runes.AsSpan().AsRunaString();
        SliceStartEndTestCore<RunesSpanString, RunesSpanEnumerator, RunesIndex, ReadOnlySpan<Rune>>(span, startIndex, endIndex, expected);
    }

    [Theory, MemberData(nameof(SliceStartEndTestCases))]
    public void SliceStartEndRunesMemory(string value, int startIndex, int endIndex, string expected)
    {
        var runes = value.EnumerateRunes().ToImmutableArray();
        var memory = runes.AsMemory().AsRunaString();
        SliceStartEndTestCore<RunesString, RunesMemoryEnumerator, RunesIndex, ReadOnlyMemory<Rune>>(memory, startIndex, endIndex, expected);
    }
}
