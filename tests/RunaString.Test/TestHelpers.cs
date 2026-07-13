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
        ];

    public static readonly TheoryData<Utf8TestCase> Utf8TestCases =
        [.. CommonTestStrings.Select(Utf8TestCase.Create)];

    public static readonly TheoryData<CharsTestCase> CharsTestCases =
        [.. CommonTestStrings.Select(CharsTestCase.Create)];

    public static readonly TheoryData<RunesTestCase> RunesTestCases =
        [.. CommonTestStrings.Select(RunesTestCase.Create)];

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    public static extern Utf8Index CreateUtf8Index(int byteIndex, int runePosition);

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    public static extern CharsIndex CreateCharsIndex(int charIndex, int runePosition);

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    public static extern RunesIndex CreateRunesIndex(int runePosition);
}


public record Utf8TestCase(string String, ImmutableArray<byte> Bytes, int RuneLength)
{
    public static Utf8TestCase Create(string s) =>
        new(s, [.. Encoding.UTF8.GetBytes(s)], s.EnumerateRunes().Count());

    public Utf8String GetMemoryString() => Utf8String.DangerousFromUtf8(Bytes, 0, Bytes.Length);
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
    public static CharsTestCase Create(string s) =>
        new(s, s.EnumerateRunes().Count());

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

public record RunesTestCase(ImmutableArray<Rune> Runes, int RuneLength)
{
    public static RunesTestCase Create(string s) =>
        new([.. s.EnumerateRunes()], s.EnumerateRunes().Count());

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