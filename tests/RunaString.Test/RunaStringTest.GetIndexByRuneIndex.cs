using System.Text;
using System.Text.Unicode;

namespace RunaString.Test;

public partial class RunaStringTest
{
    public static TheoryData<Utf8TestCase> GetIndexByRuneIndexUtf8TestCases() => [.. TestHelpers.Utf8TestCases];
    public static TheoryData<CharsTestCase> GetIndexByRuneIndexCharsTestCases() => [.. TestHelpers.CharsTestCases];
    public static TheoryData<RunesTestCase> GetIndexByRuneIndexRunesTestCases() => [.. TestHelpers.RunesTestCases];


    [Theory]
    [MemberData(nameof(GetIndexByRuneIndexUtf8TestCases))]
    public void TestGetIndexByRuneIndexUtf8(Utf8TestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        for(int byteIndex = 0, runeIndex = 0; byteIndex < testCase.Bytes.Length;)
        {
            {
                var index = memory.GetIndexByRuneIndex(runeIndex);
                Assert.Equal(runeIndex, index.RuneIndex);
                Assert.Equal(byteIndex, index.ByteIndex);
            }
            {
                var index = span.GetIndexByRuneIndex(runeIndex);
                Assert.Equal(runeIndex, index.RuneIndex);
                Assert.Equal(byteIndex, index.ByteIndex);
            }
            Rune.DecodeFromUtf8(testCase.Bytes.AsSpan()[byteIndex..], out _, out var bytesConsumed);
            byteIndex += bytesConsumed;
            ++runeIndex;
        }
    }


    [Theory]
    [MemberData(nameof(GetIndexByRuneIndexCharsTestCases))]
    public void TestGetIndexByRuneIndexChars(CharsTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        for (int charIndex = 0, runeIndex = 0; charIndex < testCase.Chars.Length;)
        {
            {
                var index = memory.GetIndexByRuneIndex(runeIndex);
                Assert.Equal(runeIndex, index.RuneIndex);
                Assert.Equal(charIndex, index.CharIndex);
            }
            {
                var index = span.GetIndexByRuneIndex(runeIndex);
                Assert.Equal(runeIndex, index.RuneIndex);
                Assert.Equal(charIndex, index.CharIndex);
            }
            Rune.DecodeFromUtf16(testCase.Chars.AsSpan()[charIndex..], out _, out var bytesConsumed);
            charIndex += bytesConsumed;
            ++runeIndex;
        }
    }


    [Theory]
    [MemberData(nameof(GetIndexByRuneIndexRunesTestCases))]
    public void TestGetIndexByRuneIndexRunes(RunesTestCase testCase)
    {
        var memory = testCase.GetMemoryString();
        var span = testCase.GetSpanString();
        for (int runeIndex = 0; runeIndex < testCase.Runes.Length;)
        {
            {
                var index = memory.GetIndexByRuneIndex(runeIndex);
                Assert.Equal(runeIndex, index.RuneIndex);
            }
            {
                var index = span.GetIndexByRuneIndex(runeIndex);
                Assert.Equal(runeIndex, index.RuneIndex);
            }
            ++runeIndex;
        }
    }
}
