using System.Text;

namespace RunaString.Benchmark;

internal static class BenchmarkHelpers
{
    public const string TestString_Ascii1 = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
    public const string TestString_Ascii2 = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborun.";
    //                                       ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- benchmark testing difference ^

    public const string TestString_Multilingual = """
            Hello, world!
            The quick brown fox jumps over the lazy dog
            CRLF\r\nTab\tEnd
            Ångström façade Noël
            Привет, мир!
            مرحبا بالعالم
            色は匂へど　散りぬるを\r\nわが世誰ぞ　常ならむ\r\n有為の奥山　今日越えて\r\n浅き夢見し　酔ひもせず
            키스의 고유조건은 입술끼리 만나야 하고 특별한 기술은 필요치 않다
            \u1112\u1161\u11AB\u1100\u1173\u11AF
            Щётканы фермд пийшин цувъя. Бөгж зогсч хэльюү
            نص حكيم له سر قاطع وذو شأن عظيم مكتوب على ثوب أخضر ومغلف بجلد أزرق
            เป็นมนุษย์สุดประเสริฐเลิศคุณค่า กว่าบรรดาฝูงสัตว์เดรัจฉาน จงฝ่าฟันพัฒนาวิชาการ อย่าล้างผลาญฤๅเข่นฆ่าบีฑาใคร ไม่ถือโทษโกรธแช่งซัดฮึดฮัดด่า หัดอภัยเหมือนกีฬาอัชฌาสัย ปฏิบัติประพฤติกฎกำหนดใจ พูดจาให้จ๊ะๆ จ๋า น่าฟังเอยฯ
            Supplementary: 𝄞
            Emoji: 😀😄👍🏽🚀\U0001F3C3\U0001F3FD\U0000200D\U00002640\U0000FE0F
            ZWJ Family: 👩‍👩‍👧‍👦
            Regional Indicator Symbol: 🇦🇦 🇦🇧 🇦🇨 🇦🇩 🇦🇪 🇦🇫 🇦🇬 🇦🇭 🇦🇮 🇦🇯 🇦🇰 🇦🇱 🇦🇲 🇦🇳 🇦🇴 🇦🇵 🇦🇶 🇦🇷 🇦🇸 🇦🇹 🇦🇺 🇦🇻 🇦🇼 🇦🇽 🇦🇾 🇦🇿 🇧🇦 🇧🇧 🇧🇨 🇧🇩 🇧🇪 🇧🇫 🇧🇬 🇧🇭 🇧🇮 🇧🇯 🇧🇰 🇧🇱 🇧🇲 🇧🇳 🇧🇴 🇧🇵 🇧🇶 🇧🇷 🇧🇸 🇧🇹 🇧🇺 🇧🇻 🇧🇼 🇧🇽 🇧🇾 🇧🇿 🇨🇦 🇨🇧 🇨🇨 🇨🇩 🇨🇪 🇨🇫 🇨🇬 🇨🇭 🇨🇮 🇨🇯 🇨🇰 🇨🇱 🇨🇲 🇨🇳 🇨🇴 🇨🇵 🇨🇶 🇨🇷 🇨🇸 🇨🇹 🇨🇺 🇨🇻 🇨🇼 🇨🇽 🇨🇾 🇨🇿 🇩🇦 🇩🇧 🇩🇨 🇩🇩 🇩🇪 🇩🇫 🇩🇬 🇩🇭 🇩🇮 🇩🇯 🇩🇰 🇩🇱 🇩🇲 🇩🇳 🇩🇴 🇩🇵 🇩🇶 🇩🇷 🇩🇸 🇩🇹 🇩🇺 🇩🇻 🇩🇼 🇩🇽 🇩🇾 🇩🇿 🇪🇦 🇪🇧 🇪🇨 🇪🇩 🇪🇪 🇪🇫 🇪🇬 🇪🇭 🇪🇮 🇪🇯 🇪🇰 🇪🇱 🇪🇲 🇪🇳 🇪🇴 🇪🇵 🇪🇶 🇪🇷 🇪🇸 🇪🇹 🇪🇺 🇪🇻 🇪🇼 🇪🇽 🇪🇾 🇪🇿 🇫🇦 🇫🇧 🇫🇨 🇫🇩 🇫🇪 🇫🇫 🇫🇬 🇫🇭 🇫🇮 🇫🇯 🇫🇰 🇫🇱 🇫🇲 🇫🇳 🇫🇴 🇫🇵 🇫🇶 🇫🇷 🇫🇸 🇫🇹 🇫🇺 🇫🇻 🇫🇼 🇫🇽 🇫🇾 🇫🇿 🇬🇦 🇬🇧 🇬🇨 🇬🇩 🇬🇪 🇬🇫 🇬🇬 🇬🇭 🇬🇮 🇬🇯 🇬🇰 🇬🇱 🇬🇲 🇬🇳 🇬🇴 🇬🇵 🇬🇶 🇬🇷 🇬🇸🇬🇹 🇬🇺 🇬🇻 🇬🇼 🇬🇽 🇬🇾 🇬🇿 🇭🇦 🇭🇧 🇭🇨 🇭🇩 🇭🇪 🇭🇫 🇭🇬 🇭🇭 🇭🇮 🇭🇯 🇭🇰 🇭🇱 🇭🇲 🇭🇳 🇭🇴 🇭🇵 🇭🇶 🇭🇷 🇭🇸 🇭🇹 🇭🇺 🇭🇻 🇭🇼 🇭🇽 🇭🇾 🇭🇿 🇮🇦 🇮🇧 🇮🇨 🇮🇩 🇮🇪 🇮🇫 🇮🇬 🇮🇭 🇮🇮 🇮🇯 🇮🇰 🇮🇱 🇮🇲 🇮🇳 🇮🇴 🇮🇵 🇮🇶 🇮🇷 🇮🇸 🇮🇹 🇮🇺 🇮🇻 🇮🇼 🇮🇽 🇮🇾 🇮🇿 🇯🇦 🇯🇧 🇯🇨 🇯🇩 🇯🇪 🇯🇫 🇯🇬 🇯🇭 🇯🇮 🇯🇯 🇯🇰 🇯🇱 🇯🇲 🇯🇳 🇯🇴 🇯🇵 🇯🇶 🇯🇷 🇯🇸 🇯🇹 🇯🇺 🇯🇻 🇯🇼 🇯🇽 🇯🇾 🇯🇿 🇰🇦 🇰🇧 🇰🇨 🇰🇩 🇰🇪 🇰🇫 🇰🇬 🇰🇭 🇰🇮 🇰🇯 🇰🇰 🇰🇱 🇰🇲 🇰🇳 🇰🇴 🇰🇵 🇰🇶 🇰🇷 🇰🇸 🇰🇹 🇰🇺 🇰🇻 🇰🇼 🇰🇽 🇰🇾 🇰🇿 🇱🇦 🇱🇧 🇱🇨 🇱🇩 🇱🇪 🇱🇫 🇱🇬 🇱🇭 🇱🇮 🇱🇯 🇱🇰 🇱🇱 🇱🇲 🇱🇳 🇱🇴 🇱🇵 🇱🇶 🇱🇷 🇱🇸 🇱🇹 🇱🇺 🇱🇻 🇱🇼 🇱🇽 🇱🇾 🇱🇿 🇲🇦 🇲🇧 🇲🇨 🇲🇩 🇲🇪 🇲🇫 🇲🇬 🇲🇭 🇲🇮 🇲🇯 🇲🇰 🇲🇱 🇲🇲 🇲🇳 🇲🇴 🇲🇵 🇲🇶 🇲🇷 🇲🇸 🇲🇹 🇲🇺 🇲🇻 🇲🇼 🇲🇽 🇲🇾 🇲🇿 🇳🇦 🇳🇧 🇳🇨 🇳🇩 🇳🇪 🇳🇫 🇳🇬 🇳🇭 🇳🇮 🇳🇯 🇳🇰 🇳🇱🇳🇲 🇳🇳 🇳🇴 🇳🇵 🇳🇶 🇳🇷 🇳🇸 🇳🇹 🇳🇺 🇳🇻 🇳🇼 🇳🇽 🇳🇾 🇳🇿 🇴🇦 🇴🇧 🇴🇨 🇴🇩 🇴🇪 🇴🇫 🇴🇬 🇴🇭 🇴🇮 🇴🇯 🇴🇰 🇴🇱 🇴🇲 🇴🇳 🇴🇴 🇴🇵 🇴🇶 🇴🇷 🇴🇸 🇴🇹 🇴🇺 🇴🇻 🇴🇼 🇴🇽 🇴🇾 🇴🇿 🇵🇦 🇵🇧 🇵🇨 🇵🇩 🇵🇪 🇵🇫 🇵🇬 🇵🇭 🇵🇮 🇵🇯 🇵🇰 🇵🇱 🇵🇲 🇵🇳 🇵🇴 🇵🇵 🇵🇶 🇵🇷 🇵🇸 🇵🇹 🇵🇺 🇵🇻 🇵🇼 🇵🇽 🇵🇾 🇵🇿 🇶🇦 🇶🇧 🇶🇨 🇶🇩 🇶🇪 🇶🇫 🇶🇬 🇶🇭 🇶🇮 🇶🇯 🇶🇰 🇶🇱 🇶🇲 🇶🇳 🇶🇴 🇶🇵 🇶🇶 🇶🇷 🇶🇸 🇶🇹 🇶🇺 🇶🇻 🇶🇼 🇶🇽 🇶🇾 🇶🇿 🇷🇦 🇷🇧 🇷🇨 🇷🇩 🇷🇪 🇷🇫 🇷🇬 🇷🇭 🇷🇮 🇷🇯 🇷🇰 🇷🇱 🇷🇲 🇷🇳 🇷🇴 🇷🇵 🇷🇶 🇷🇷 🇷🇸 🇷🇹 🇷🇺 🇷🇻 🇷🇼 🇷🇽 🇷🇾 🇷🇿 🇸🇦 🇸🇧 🇸🇨 🇸🇩 🇸🇪 🇸🇫 🇸🇬 🇸🇭 🇸🇮 🇸🇯 🇸🇰 🇸🇱 🇸🇲 🇸🇳 🇸🇴 🇸🇵 🇸🇶 🇸🇷 🇸🇸 🇸🇹 🇸🇺 🇸🇻 🇸🇼 🇸🇽 🇸🇾 🇸🇿 🇹🇦 🇹🇧 🇹🇨 🇹🇩 🇹🇪 🇹🇫 🇹🇬 🇹🇭 🇹🇮 🇹🇯 🇹🇰 🇹🇱 🇹🇲 🇹🇳 🇹🇴 🇹🇵 🇹🇶 🇹🇷 🇹🇸 🇹🇹 🇹🇺 🇹🇻 🇹🇼 🇹🇽 🇹🇾 🇹🇿 🇺🇦 🇺🇧 🇺🇨 🇺🇩 🇺🇪🇺🇫 🇺🇬 🇺🇭 🇺🇮 🇺🇯 🇺🇰 🇺🇱 🇺🇲 🇺🇳 🇺🇴 🇺🇵 🇺🇶 🇺🇷 🇺🇸 🇺🇹 🇺🇺 🇺🇻 🇺🇼 🇺🇽 🇺🇾 🇺🇿 🇻🇦 🇻🇧 🇻🇨 🇻🇩 🇻🇪 🇻🇫 🇻🇬 🇻🇭 🇻🇮 🇻🇯 🇻🇰 🇻🇱 🇻🇲 🇻🇳 🇻🇴 🇻🇵 🇻🇶 🇻🇷 🇻🇸 🇻🇹 🇻🇺 🇻🇻 🇻🇼 🇻🇽 🇻🇾 🇻🇿 🇼🇦 🇼🇧 🇼🇨 🇼🇩 🇼🇪 🇼🇫 🇼🇬 🇼🇭 🇼🇮 🇼🇯 🇼🇰 🇼🇱 🇼🇲 🇼🇳 🇼🇴 🇼🇵 🇼🇶 🇼🇷 🇼🇸 🇼🇹 🇼🇺 🇼🇻 🇼🇼 🇼🇽 🇼🇾 🇼🇿 🇽🇦 🇽🇧 🇽🇨 🇽🇩 🇽🇪 🇽🇫 🇽🇬 🇽🇭 🇽🇮 🇽🇯 🇽🇰 🇽🇱 🇽🇲 🇽🇳 🇽🇴 🇽🇵 🇽🇶 🇽🇷 🇽🇸 🇽🇹 🇽🇺 🇽🇻 🇽🇼 🇽🇽 🇽🇾 🇽🇿 🇾🇦 🇾🇧 🇾🇨 🇾🇩 🇾🇪 🇾🇫 🇾🇬 🇾🇭 🇾🇮 🇾🇯 🇾🇰 🇾🇱 🇾🇲 🇾🇳 🇾🇴 🇾🇵 🇾🇶 🇾🇷 🇾🇸 🇾🇹 🇾🇺 🇾🇻 🇾🇼 🇾🇽 🇾🇾 🇾🇿 🇿🇦 🇿🇧 🇿🇨 🇿🇩 🇿🇪 🇿🇫 🇿🇬 🇿🇭 🇿🇮 🇿🇯 🇿🇰 🇿🇱 🇿🇲 🇿🇳 🇿🇴 🇿🇵 🇿🇶 🇿🇷 🇿🇸 🇿🇹 🇿🇺 🇿🇻 🇿🇼 🇿🇽 🇿🇾 🇿🇿
            Variation Selectors: 葛\U000E0100飾区 葛\U000E0100城市
            \u0000\u007F\u0080\u07FF\u0800\uFFFF\U00010000\U0010FFFF
            \uFFFD\uFFFE\uFFFF\U00010000\U00010001
            \U0010FFFF
            \uFDD0\uFDEF\uFFFE\uFFFF\U0001FFFE\U0001FFFF\U0010FFFE\U0010FFFF
            a\u0301\u0323\u0308\u0304
            \u0301\u0302\u0303
            A\u0300\u0301\u0302\u0303\u0304\u0305\u0306\u0307\u0308\u0309
            \u00E9
            e\u0301
            \u212B
            A\u030A
            한글
            \u1112\u1161\u11AB\u1100\u1173\u11AF
            한글\u1112\u1161\u11AB
            ❤❤️✈✈️☺☺️
            1️⃣2️⃣3️⃣#️⃣*️⃣
            🏴
            👍👍🏻👍🏼👍🏽👍🏾👍🏿
            👩‍❤️‍💋‍👨
            किंतु
            שָׁלוֹם
            ABC\u200FDEF\u200EGHI
            abc\u202E123\u202Cxyz
            A\u200BB\u200CC\u200DD\u2060E
            \uFEFFHello
            Hello\uFEFFWorld
            \u0000\u0001\u0002\u0003\u0007\b\t\n\v\f\r\u001B
            Aあ한😀𝄞𠀋􏿿
            A1Ⅷ①½₿™©®§¶‽
            \uE000\uF8FF\U000F0000\U0010FFFD
            A\u034FB
            \uFFF9A\uFFFAB\uFFFB
            \u0000é漢𐀀😀𠀋\U000E0100\U0010FFFF
            """;

    public static readonly byte[] Utf8Bytes =
        Encoding.UTF8.GetBytes(TestString_Multilingual);
}
