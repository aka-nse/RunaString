using System;
using System.Collections.Generic;
using System.Text;

namespace RunaString.Test;

public class CharHelpersTest
{
    public static TheoryData<string, int> CountAsciiCharFromAheadTestCases => new()
    {
        { "", 0 },
        { "Hello, world!", 13 },
        { "Hello, 世界!", 7 },
        { "こんにちは、世界！", 0 },
        { "Hello, 世界! こんにちは、世界！", 7 },
        { "Hello, 世界! Hello, 世界!", 7 },
        { "Hello, 世界! Hello, 世界! Hello, 世界!", 7 },
        { "Hello, 世界! Hello, 世界! Hello, 世界! Hello, 世界!", 7 },
        { new string('a', 255) + 'あ', 255 },
        { new string('a', 256) + 'あ', 256 },
        { new string('a', 257) + 'あ', 257 },
        { string.Concat(Enumerable.Range(0, 256).Select(static x => (char)x)), 128 },
    };

    [Theory]
    [MemberData(nameof(CountAsciiCharFromAheadTestCases))]
    public void CountAsciiCharFromAhead_ShouldReturnExpectedCount(string input, int expectedCount)
    {
        var actualCount = CharHelpers.CountAsciiCharFromAhead(input);
        Assert.Equal(expectedCount, actualCount);
    }



    public static TheoryData<string, int> CountNonAsciiCharFromAheadTestCases => new()
    {
        { "", 0  },
        { "Hello, world!", 0 },
        { "Hello, 世界!", 0 },
        { "こんにちは、世界！", 9 },
        { "こんにちは、world!", 6 },
        { "こんにちは、world!こんにちは、world!", 6 },
        { new string('あ', 255) + 'a', 255 },
        { new string('あ', 256) + 'a', 256 },
        { new string('あ', 257) + 'a', 257 },
        { string.Concat(Enumerable.Range(0, 256).Select(static x => (char)(255 - x))), 128 },
    };

    [Theory]
    [MemberData(nameof(CountNonAsciiCharFromAheadTestCases))]
    public void CountNonAsciiCharFromAhead_ShouldReturnExpectedCount(string input, int expectedCount)
    {
        var actualCount = CharHelpers.CountNonAsciiCharFromAhead(input);
        Assert.Equal(expectedCount, actualCount);
    }
}
