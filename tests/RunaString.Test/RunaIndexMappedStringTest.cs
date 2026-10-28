using System;
using System.Collections.Generic;
using System.Text;

namespace RunaString.Test;

public class RunaIndexMappedStringTest
{
    public static TheoryData<string, int, int> TestCases() => new()
    {
        { "Hello, world!", 0, 5 },
        { "Hello, world!", 3, 6 },
        { "こんにちは、世界！", 0, 5 },
        { "こんにちは、世界！", 2, 3 },
        { "👋🌍", 0, 2 },
        { "👋🌍", 1, 1 },
        { "Hello, 世界!", 0, 7 },
        { "Hello, 世界!", 7, 3 },
    };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void TestIndexMappedString(string input, int start, int length)
    {
        var expectedLength = 0;
        var expectedSubstring = new StringBuilder();
        var runes = new List<Rune>();
        {
            var i = 0;
            foreach (var rune in input.EnumerateRunes())
            {
                if(start <= i && i < start + length)
                {
                    expectedSubstring.Append(rune);
                }
                runes.Add(rune);
                ++i;
            }
            expectedLength = i;
        }

        var str = Utf8String.FromUtf8([.. Encoding.UTF8.GetBytes(input)]);
        var mappedString = RunaIndexMappedString.Create<Utf8String, Utf8Index>(str);

        Assert.Equal(expectedLength, mappedString.Length);
        Assert.Equal(input, mappedString.ToString());
        for(var i = 0; i < mappedString.Length; ++i)
        {
            Assert.Equal(runes[i], mappedString[i]);
        }

        var slicedMappedString = mappedString.Slice(start, length);
        Assert.Equal(expectedSubstring.ToString(), slicedMappedString.ToString());
    }
}
