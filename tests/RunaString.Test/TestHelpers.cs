using System.Collections.Immutable;
using System.Runtime.CompilerServices;

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


    public static int GetUtf8CodeUnitCount(string s, int start, int count) =>
        s.EnumerateRunes()
        .Skip(start)
        .Take(count)
        .Select(static x => x.Utf8SequenceLength)
        .Sum();


    public static int GetCharsCodeUnitCount(string s, int start, int count) =>
        s.EnumerateRunes()
        .Skip(start)
        .Take(count)
        .Select(static x => x.Utf16SequenceLength)
        .Sum();

    public static int GetRunesCodeUnitCount(string s, int start, int count) =>
        s.EnumerateRunes()
        .Skip(start)
        .Take(count)
        .Count();

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    public static extern Utf8Index CreateUtf8Index(int byteIndex, int runePosition);

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    public static extern CharsIndex CreateCharsIndex(int charIndex, int runePosition);

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    public static extern RunesIndex CreateRunesIndex(int runePosition);
}
