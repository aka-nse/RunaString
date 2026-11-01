using System;
using System.Collections.Generic;
using System.Text;

namespace RunaString.Test;

public class RunaIndexMappedStringTest
{
    public static TheoryData<string, int, int> TestCases()
    {
        var data = new TheoryData<string, int, int>()
        {
            { "Hello, world!", 0, 5 },
            { "Hello, world!", 3, 6 },
            { "こんにちは、世界！", 0, 5 },
            { "こんにちは、世界！", 2, 3 },
            { "Hello, 世界!", 0, 7 },
            { "Hello, 世界!", 7, 3 },

            { "吾輩は猫である。名前はまだ無い。", 0, 6 },
            { "吾輩は猫である。名前はまだ無い。", 6, 5 },
            { "吾輩は猫である。名前はまだ無い。", "吾輩は猫である。名前はまだ無い。".Length - 3, 3 },

            { "ABCあいう漢字123", 0, 0 },
            { "ABCあいう漢字123", 3, 5 },
            { "ABCあいう漢字123", "ABCあいう漢字123".Length, 0 },
        };

        { // Lorem ipsum (BMP only)
            const string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            data.Add(text, 0, text.Length);
            data.Add(text, text.Length, 0);
            data.Add(text, 3, text.Length - 6);
            data.Add(text, 0, 11);
            data.Add(text, 12, 17);
        }

        { // 吾輩は猫である (BMP only)
            const string text = "　吾輩は猫である。名前はまだ無い。\r\n　どこで生れたかとんと見当がつかぬ。何でも薄暗いじめじめした所でニャーニャー泣いていた事だけは記憶している。吾輩はここで始めて人間というものを見た。しかもあとで聞くとそれは書生という人間中で一番獰悪な種族であったそうだ。この書生というのは時々我々を捕えて煮て食うという話である。";
            data.Add(text, 0, text.Length);
            data.Add(text, text.Length, 0);
            data.Add(text, 3, text.Length - 6);
            data.Add(text, 15, 15);
            data.Add(text, 0, 30);
        }

        { // 平家物語 (contains Supplementary Ideographic Plane)
            const string text = "祇園精舍の鐘の聲、諸行無常の響あり。\r\n娑羅雙樹の花の色、盛者必衰の理をあらはす。\r\n驕れる人も久しからず、ただ春の夜の夢のごとし。\r\n猛き者もつひにはほろびぬ、ひとへに風の前の塵に同じ。";
            int length = text.EnumerateRunes().Count();
            data.Add(text, 0, length);
            data.Add(text, length, 0);
            data.Add(text, 3, length - 6);
            data.Add(text, 15, 15);
            data.Add(text, 0, 30);
        }
        return data;
    }

    [Theory]
    [MemberData(nameof(TestCases))]
    public void TestIndexMappedString(string input, int start, int length)
    {
        int expectedLength;
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
