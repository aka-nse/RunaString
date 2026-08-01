using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;

namespace RunaString.Test;

internal static class TestHelpers
{
    public static readonly ImmutableArray<string> CommonTestStrings = [
            "",
            "Hello, world!",
            "The quick brown fox jumps over the lazy dog",
            "CRLF\r\nTab\tEnd",
            "Ångström façade Noël",
            "Привет, мир!",
            "مرحبا بالعالم",
            "色は匂へど　散りぬるを\r\nわが世誰ぞ　常ならむ\r\n有為の奥山　今日越えて\r\n浅き夢見し　酔ひもせず",
            "키스의 고유조건은 입술끼리 만나야 하고 특별한 기술은 필요치 않다",
            "\u1112\u1161\u11AB\u1100\u1173\u11AF",
            "Щётканы фермд пийшин цувъя. Бөгж зогсч хэльюү",
            "نص حكيم له سر قاطع وذو شأن عظيم مكتوب على ثوب أخضر ومغلف بجلد أزرق",
            "เป็นมนุษย์สุดประเสริฐเลิศคุณค่า กว่าบรรดาฝูงสัตว์เดรัจฉาน จงฝ่าฟันพัฒนาวิชาการ อย่าล้างผลาญฤๅเข่นฆ่าบีฑาใคร ไม่ถือโทษโกรธแช่งซัดฮึดฮัดด่า หัดอภัยเหมือนกีฬาอัชฌาสัย ปฏิบัติประพฤติกฎกำหนดใจ พูดจาให้จ๊ะๆ จ๋า น่าฟังเอยฯ",
            "Supplementary: 𝄞",
            "Emoji: 😀😄👍🏽🚀\U0001F3C3\U0001F3FD\U0000200D\U00002640\U0000FE0F",
            "ZWJ Family: 👩‍👩‍👧‍👦",
            "Regional Indicator Symbol: 🇦🇦 🇦🇧 🇦🇨 🇦🇩 🇦🇪 🇦🇫 🇦🇬 🇦🇭 🇦🇮 🇦🇯 🇦🇰 🇦🇱 🇦🇲 🇦🇳 🇦🇴 🇦🇵 🇦🇶 🇦🇷 🇦🇸 🇦🇹 🇦🇺 🇦🇻 🇦🇼 🇦🇽 🇦🇾 🇦🇿 🇧🇦 🇧🇧 🇧🇨 🇧🇩 🇧🇪 🇧🇫 🇧🇬 🇧🇭 🇧🇮 🇧🇯 🇧🇰 🇧🇱 🇧🇲 🇧🇳 🇧🇴 🇧🇵 🇧🇶 🇧🇷 🇧🇸 🇧🇹 🇧🇺 🇧🇻 🇧🇼 🇧🇽 🇧🇾 🇧🇿 🇨🇦 🇨🇧 🇨🇨 🇨🇩 🇨🇪 🇨🇫 🇨🇬 🇨🇭 🇨🇮 🇨🇯 🇨🇰 🇨🇱 🇨🇲 🇨🇳 🇨🇴 🇨🇵 🇨🇶 🇨🇷 🇨🇸 🇨🇹 🇨🇺 🇨🇻 🇨🇼 🇨🇽 🇨🇾 🇨🇿 🇩🇦 🇩🇧 🇩🇨 🇩🇩 🇩🇪 🇩🇫 🇩🇬 🇩🇭 🇩🇮 🇩🇯 🇩🇰 🇩🇱 🇩🇲 🇩🇳 🇩🇴 🇩🇵 🇩🇶 🇩🇷 🇩🇸 🇩🇹 🇩🇺 🇩🇻 🇩🇼 🇩🇽 🇩🇾 🇩🇿 🇪🇦 🇪🇧 🇪🇨 🇪🇩 🇪🇪 🇪🇫 🇪🇬 🇪🇭 🇪🇮 🇪🇯 🇪🇰 🇪🇱 🇪🇲 🇪🇳 🇪🇴 🇪🇵 🇪🇶 🇪🇷 🇪🇸 🇪🇹 🇪🇺 🇪🇻 🇪🇼 🇪🇽 🇪🇾 🇪🇿 🇫🇦 🇫🇧 🇫🇨 🇫🇩 🇫🇪 🇫🇫 🇫🇬 🇫🇭 🇫🇮 🇫🇯 🇫🇰 🇫🇱 🇫🇲 🇫🇳 🇫🇴 🇫🇵 🇫🇶 🇫🇷 🇫🇸 🇫🇹 🇫🇺 🇫🇻 🇫🇼 🇫🇽 🇫🇾 🇫🇿 🇬🇦 🇬🇧 🇬🇨 🇬🇩 🇬🇪 🇬🇫 🇬🇬 🇬🇭 🇬🇮 🇬🇯 🇬🇰 🇬🇱 🇬🇲 🇬🇳 🇬🇴 🇬🇵 🇬🇶 🇬🇷 🇬🇸🇬🇹 🇬🇺 🇬🇻 🇬🇼 🇬🇽 🇬🇾 🇬🇿 🇭🇦 🇭🇧 🇭🇨 🇭🇩 🇭🇪 🇭🇫 🇭🇬 🇭🇭 🇭🇮 🇭🇯 🇭🇰 🇭🇱 🇭🇲 🇭🇳 🇭🇴 🇭🇵 🇭🇶 🇭🇷 🇭🇸 🇭🇹 🇭🇺 🇭🇻 🇭🇼 🇭🇽 🇭🇾 🇭🇿 🇮🇦 🇮🇧 🇮🇨 🇮🇩 🇮🇪 🇮🇫 🇮🇬 🇮🇭 🇮🇮 🇮🇯 🇮🇰 🇮🇱 🇮🇲 🇮🇳 🇮🇴 🇮🇵 🇮🇶 🇮🇷 🇮🇸 🇮🇹 🇮🇺 🇮🇻 🇮🇼 🇮🇽 🇮🇾 🇮🇿 🇯🇦 🇯🇧 🇯🇨 🇯🇩 🇯🇪 🇯🇫 🇯🇬 🇯🇭 🇯🇮 🇯🇯 🇯🇰 🇯🇱 🇯🇲 🇯🇳 🇯🇴 🇯🇵 🇯🇶 🇯🇷 🇯🇸 🇯🇹 🇯🇺 🇯🇻 🇯🇼 🇯🇽 🇯🇾 🇯🇿 🇰🇦 🇰🇧 🇰🇨 🇰🇩 🇰🇪 🇰🇫 🇰🇬 🇰🇭 🇰🇮 🇰🇯 🇰🇰 🇰🇱 🇰🇲 🇰🇳 🇰🇴 🇰🇵 🇰🇶 🇰🇷 🇰🇸 🇰🇹 🇰🇺 🇰🇻 🇰🇼 🇰🇽 🇰🇾 🇰🇿 🇱🇦 🇱🇧 🇱🇨 🇱🇩 🇱🇪 🇱🇫 🇱🇬 🇱🇭 🇱🇮 🇱🇯 🇱🇰 🇱🇱 🇱🇲 🇱🇳 🇱🇴 🇱🇵 🇱🇶 🇱🇷 🇱🇸 🇱🇹 🇱🇺 🇱🇻 🇱🇼 🇱🇽 🇱🇾 🇱🇿 🇲🇦 🇲🇧 🇲🇨 🇲🇩 🇲🇪 🇲🇫 🇲🇬 🇲🇭 🇲🇮 🇲🇯 🇲🇰 🇲🇱 🇲🇲 🇲🇳 🇲🇴 🇲🇵 🇲🇶 🇲🇷 🇲🇸 🇲🇹 🇲🇺 🇲🇻 🇲🇼 🇲🇽 🇲🇾 🇲🇿 🇳🇦 🇳🇧 🇳🇨 🇳🇩 🇳🇪 🇳🇫 🇳🇬 🇳🇭 🇳🇮 🇳🇯 🇳🇰 🇳🇱🇳🇲 🇳🇳 🇳🇴 🇳🇵 🇳🇶 🇳🇷 🇳🇸 🇳🇹 🇳🇺 🇳🇻 🇳🇼 🇳🇽 🇳🇾 🇳🇿 🇴🇦 🇴🇧 🇴🇨 🇴🇩 🇴🇪 🇴🇫 🇴🇬 🇴🇭 🇴🇮 🇴🇯 🇴🇰 🇴🇱 🇴🇲 🇴🇳 🇴🇴 🇴🇵 🇴🇶 🇴🇷 🇴🇸 🇴🇹 🇴🇺 🇴🇻 🇴🇼 🇴🇽 🇴🇾 🇴🇿 🇵🇦 🇵🇧 🇵🇨 🇵🇩 🇵🇪 🇵🇫 🇵🇬 🇵🇭 🇵🇮 🇵🇯 🇵🇰 🇵🇱 🇵🇲 🇵🇳 🇵🇴 🇵🇵 🇵🇶 🇵🇷 🇵🇸 🇵🇹 🇵🇺 🇵🇻 🇵🇼 🇵🇽 🇵🇾 🇵🇿 🇶🇦 🇶🇧 🇶🇨 🇶🇩 🇶🇪 🇶🇫 🇶🇬 🇶🇭 🇶🇮 🇶🇯 🇶🇰 🇶🇱 🇶🇲 🇶🇳 🇶🇴 🇶🇵 🇶🇶 🇶🇷 🇶🇸 🇶🇹 🇶🇺 🇶🇻 🇶🇼 🇶🇽 🇶🇾 🇶🇿 🇷🇦 🇷🇧 🇷🇨 🇷🇩 🇷🇪 🇷🇫 🇷🇬 🇷🇭 🇷🇮 🇷🇯 🇷🇰 🇷🇱 🇷🇲 🇷🇳 🇷🇴 🇷🇵 🇷🇶 🇷🇷 🇷🇸 🇷🇹 🇷🇺 🇷🇻 🇷🇼 🇷🇽 🇷🇾 🇷🇿 🇸🇦 🇸🇧 🇸🇨 🇸🇩 🇸🇪 🇸🇫 🇸🇬 🇸🇭 🇸🇮 🇸🇯 🇸🇰 🇸🇱 🇸🇲 🇸🇳 🇸🇴 🇸🇵 🇸🇶 🇸🇷 🇸🇸 🇸🇹 🇸🇺 🇸🇻 🇸🇼 🇸🇽 🇸🇾 🇸🇿 🇹🇦 🇹🇧 🇹🇨 🇹🇩 🇹🇪 🇹🇫 🇹🇬 🇹🇭 🇹🇮 🇹🇯 🇹🇰 🇹🇱 🇹🇲 🇹🇳 🇹🇴 🇹🇵 🇹🇶 🇹🇷 🇹🇸 🇹🇹 🇹🇺 🇹🇻 🇹🇼 🇹🇽 🇹🇾 🇹🇿 🇺🇦 🇺🇧 🇺🇨 🇺🇩 🇺🇪🇺🇫 🇺🇬 🇺🇭 🇺🇮 🇺🇯 🇺🇰 🇺🇱 🇺🇲 🇺🇳 🇺🇴 🇺🇵 🇺🇶 🇺🇷 🇺🇸 🇺🇹 🇺🇺 🇺🇻 🇺🇼 🇺🇽 🇺🇾 🇺🇿 🇻🇦 🇻🇧 🇻🇨 🇻🇩 🇻🇪 🇻🇫 🇻🇬 🇻🇭 🇻🇮 🇻🇯 🇻🇰 🇻🇱 🇻🇲 🇻🇳 🇻🇴 🇻🇵 🇻🇶 🇻🇷 🇻🇸 🇻🇹 🇻🇺 🇻🇻 🇻🇼 🇻🇽 🇻🇾 🇻🇿 🇼🇦 🇼🇧 🇼🇨 🇼🇩 🇼🇪 🇼🇫 🇼🇬 🇼🇭 🇼🇮 🇼🇯 🇼🇰 🇼🇱 🇼🇲 🇼🇳 🇼🇴 🇼🇵 🇼🇶 🇼🇷 🇼🇸 🇼🇹 🇼🇺 🇼🇻 🇼🇼 🇼🇽 🇼🇾 🇼🇿 🇽🇦 🇽🇧 🇽🇨 🇽🇩 🇽🇪 🇽🇫 🇽🇬 🇽🇭 🇽🇮 🇽🇯 🇽🇰 🇽🇱 🇽🇲 🇽🇳 🇽🇴 🇽🇵 🇽🇶 🇽🇷 🇽🇸 🇽🇹 🇽🇺 🇽🇻 🇽🇼 🇽🇽 🇽🇾 🇽🇿 🇾🇦 🇾🇧 🇾🇨 🇾🇩 🇾🇪 🇾🇫 🇾🇬 🇾🇭 🇾🇮 🇾🇯 🇾🇰 🇾🇱 🇾🇲 🇾🇳 🇾🇴 🇾🇵 🇾🇶 🇾🇷 🇾🇸 🇾🇹 🇾🇺 🇾🇻 🇾🇼 🇾🇽 🇾🇾 🇾🇿 🇿🇦 🇿🇧 🇿🇨 🇿🇩 🇿🇪 🇿🇫 🇿🇬 🇿🇭 🇿🇮 🇿🇯 🇿🇰 🇿🇱 🇿🇲 🇿🇳 🇿🇴 🇿🇵 🇿🇶 🇿🇷 🇿🇸 🇿🇹 🇿🇺 🇿🇻 🇿🇼 🇿🇽 🇿🇾 🇿🇿",
            "Variation Selectors: 葛\U000E0100飾区 葛\U000E0100城市",
            /* UTF-8 code unit boundaries           */ "\u0000\u007F\u0080\u07FF\u0800\uFFFF\U00010000\U0010FFFF",
            /* UTF-16 code unit boundaries          */ "\uFFFD\uFFFE\uFFFF\U00010000\U00010001",
            /* Unicode maximum code point           */ "\U0010FFFF",
            /* Noncharacters                        */ "\uFDD0\uFDEF\uFFFE\uFFFF\U0001FFFE\U0001FFFF\U0010FFFE\U0010FFFF",
            /* Combining marks                      */ "a\u0301\u0323\u0308\u0304",
            /* Combining marks only                 */ "\u0301\u0302\u0303",
            /* Combining marks with base character  */ "A\u0300\u0301\u0302\u0303\u0304\u0305\u0306\u0307\u0308\u0309",
            /* Precomposed character                */ "\u00E9",
            /* Decomposed character                 */ "e\u0301",
            /* Angstrom sign                        */ "\u212B",
            /* Combining ring above                 */ "A\u030A",
            /* Hangul NFC                           */ "한글",
            /* Hangul NFD                           */ "\u1112\u1161\u11AB\u1100\u1173\u11AF",
            /* Hangul NFD with base character       */ "한글\u1112\u1161\u11AB",
            /* Emoji                                */ "❤❤️✈✈️☺☺️",
            /* Keycap                               */ "1️⃣2️⃣3️⃣#️⃣*️⃣",
            /* Flag                                 */ "🏴",
            /* Skin tone                            */ "👍👍🏻👍🏼👍🏽👍🏾👍🏿",
            /* ZWJ sequence                         */ "👩‍❤️‍💋‍👨",
            /* Devanagari                           */ "किंतु",
            /* Hebrew                               */ "שָׁלוֹם",
            /* Right-to-left mark                   */ "ABC\u200FDEF\u200EGHI",
            /* Right-to-left override               */ "abc\u202E123\u202Cxyz",
            /* Invisible separator                  */ "A\u200BB\u200CC\u200DD\u2060E",
            /* Zero-width no-break space            */ "\uFEFFHello",
            /* Zero-width no-break space in middle  */ "Hello\uFEFFWorld",
            /* Control characters                   */ "\u0000\u0001\u0002\u0003\u0007\b\t\n\v\f\r\u001B",
            /* Mixed script                         */ "Aあ한😀𝄞𠀋􏿿",
            /* Various symbols                      */ "A1Ⅷ①½₿™©®§¶‽",
            /* Private use area                     */ "\uE000\uF8FF\U000F0000\U0010FFFD",
            /* Combining grapheme joiner            */ "A\u034FB",
            /* Interlinear annotation               */ "\uFFF9A\uFFFAB\uFFFB",
            /* Miscellaneous                        */ "\u0000é漢𐀀😀𠀋\U000E0100\U0010FFFF",
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
        ];

    public static readonly ImmutableArray<Utf8TestCase> Utf8TestCases =
        [.. CommonTestStrings.Select(Utf8TestCase.Create)];

    public static readonly ImmutableArray<CharsTestCase> CharsTestCases =
        [.. CommonTestStrings.Select(CharsTestCase.Create)];

    public static readonly ImmutableArray<RunesTestCase> RunesTestCases =
        [.. CommonTestStrings.Select(RunesTestCase.Create)];

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    public static extern Utf8Index CreateUtf8Index(int byteIndex, int runeIndex);

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    public static extern CharsIndex CreateCharsIndex(int charIndex, int runeIndex);

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    public static extern RunesIndex CreateRunesIndex(int runeIndex);
}


public record Utf8TestCase(string String, ImmutableArray<byte> Bytes, int RuneLength)
{
    public static Utf8TestCase Create(string s) =>
        new(s, [.. Encoding.UTF8.GetBytes(s)], s.EnumerateRunes().Count());

    public ReadOnlySpan<byte> GetSpan() => Bytes.AsSpan();
    public ReadOnlyMemory<byte> GetMemory() => Bytes.AsMemory();

    public Utf8String GetMemoryString() => Utf8String.DangerousFromUtf8(Bytes.AsMemory());
    public Utf8SpanString GetSpanString() => Utf8SpanString.DangerousFromSpan(Bytes.AsSpan());

    public int GetCodeUnitCount(int start, int count)
    {
        if (count < 0)
        {
            return -1;
        }
        return String.EnumerateRunes()
        .Skip(start)
        .Take(count)
        .Sum(static x => x.Utf8SequenceLength);
    }

    public Utf8Index GetIndex(Index runeIndex)
    {
        var i = runeIndex.GetOffset(RuneLength);
        return TestHelpers.CreateUtf8Index(GetCodeUnitCount(0, i), i);
    }
}

public record CharsTestCase(string Chars, int RuneLength)
{
    public string String => Chars;

    public static CharsTestCase Create(string s) =>
        new(s, s.EnumerateRunes().Count());

    public ReadOnlySpan<char> GetSpan() => Chars;
    public ReadOnlyMemory<char> GetMemory() => Chars.AsMemory();

    public CharsString GetMemoryString() => new (Chars.AsMemory());
    public CharsSpanString GetSpanString() => new (Chars.AsSpan());


    public int GetCodeUnitCount(int start, int count)
    {
        if(count < 0)
        {
            return -1;
        }
        return Chars.EnumerateRunes()
        .Skip(start)
        .Take(count)
        .Sum(static x => x.Utf16SequenceLength);
    }

    public CharsIndex GetIndex(Index runeIndex)
    {
        var i = runeIndex.GetOffset(RuneLength);
        return TestHelpers.CreateCharsIndex(GetCodeUnitCount(0, i), i);
    }

}

public record RunesTestCase(string String, ImmutableArray<Rune> Runes, int RuneLength)
{
    public static RunesTestCase Create(string s) =>
        new(s, [.. s.EnumerateRunes()], s.EnumerateRunes().Count());

    public ReadOnlySpan<Rune> GetSpan() => Runes.AsSpan();
    public ReadOnlyMemory<Rune> GetMemory() => Runes.AsMemory();

    public RunesString GetMemoryString() => new (Runes.AsMemory());
    public RunesSpanString GetSpanString() => new (Runes.AsSpan());

    public int GetCodeUnitCount(int start, int count)
    {
        if (count < 0)
        {
            return -1;
        }
        return Runes
            .Skip(start)
            .Take(count)
            .Count();
    }

    public RunesIndex GetIndex(Index runeIndex)
    {
        var i = runeIndex.GetOffset(RuneLength);
        return TestHelpers.CreateRunesIndex(i);
    }
}