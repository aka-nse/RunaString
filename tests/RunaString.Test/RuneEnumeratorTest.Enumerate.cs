using System.Text;

namespace RunaString.Test;

public partial class RuneEnumeratorTest
{
    public static TheoryData<string> EnumerateTestCases() => [
            "",
            "Hello, world!",
            "The quick brown fox jumps over the lazy dog",
            "CRLF\r\nTab\tEnd",
            "色は匂へど　散りぬるを\r\nわが世誰ぞ　常ならむ\r\n有為の奥山　今日越えて\r\n浅き夢見し　酔ひもせず",
            "키스의 고유조건은 입술끼리 만나야 하고 특별한 기술은 필요치 않다",
            "Щётканы фермд пийшин цувъя. Бөгж зогсч хэльюү",
            "نص حكيم له سر قاطع وذو شأن عظيم مكتوب على ثوب أخضر ومغلف بجلد أزرق",
            "เป็นมนุษย์สุดประเสริฐเลิศคุณค่า กว่าบรรดาฝูงสัตว์เดรัจฉาน จงฝ่าฟันพัฒนาวิชาการ อย่าล้างผลาญฤๅเข่นฆ่าบีฑาใคร ไม่ถือโทษโกรธแช่งซัดฮึดฮัดด่า หัดอภัยเหมือนกีฬาอัชฌาสัย ปฏิบัติประพฤติกฎกำหนดใจ พูดจาให้จ๊ะๆ จ๋า น่าฟังเอยฯ",
            "Supplementary: 𝄞",
            "Emoji: 😄🚀",
            "ZWJ Family: 👩‍👩‍👧‍👦",
        ];

    private static void EnumerateTestCore<TEnumerator>(string expected, TEnumerator actualEnumerator)
        where TEnumerator : IRuneEnumerator<TEnumerator>, allows ref struct
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
    public void EnumerateUtf16Span(string value)
    {
        EnumerateTestCore(value, Utf16SpanEnumerator.Create(value.AsSpan()));
    }

    [Theory, MemberData(nameof(EnumerateTestCases))]
    public void EnumerateUtf16Memory(string value)
    {
        EnumerateTestCore(value, Utf16MemoryEnumerator.Create(value.AsMemory()));
    }

    [Theory, MemberData(nameof(EnumerateTestCases))]
    public void EnumerateUtf32Span(string value)
    {
        var utf32 = value.EnumerateRunes().ToArray();
        EnumerateTestCore(value, Utf32SpanEnumerator.Create(utf32));
    }
    [Theory, MemberData(nameof(EnumerateTestCases))]
    public void EnumerateUtf32Memory(string value)
    {
        var utf32 = value.EnumerateRunes().ToArray();
        EnumerateTestCore(value, Utf32MemoryEnumerator.Create(utf32));
    }
}
