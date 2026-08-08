using System.Text;

namespace RunaString.Test;

public class Utf8StringTest
{
    public static TheoryData<Utf8TestCase> Utf8StringTestCases() =>
        [.. TestHelpers.Utf8TestCases];

    [Theory]
    [MemberData(nameof(Utf8StringTestCases))]
    public void BufferLengthIsValid(Utf8TestCase test)
    {
        Assert.Equal(test.Bytes.Length, test.GetMemoryString().BufferLength);
        Assert.Equal(test.Bytes.Length, test.GetSpanString().BufferLength);
    }


    [Fact]
    public void InvalidSlice()
    {
        var test = Utf8TestCase.Create("Hello, World!");
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var memstr = test.GetMemoryString();
            memstr.Slice(new Utf8Index(-1, 0), new Utf8Index(0, 0));
        });
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var memstr = test.GetMemoryString();
            memstr.Slice(new Utf8Index(memstr.BufferLength, 0), new Utf8Index(memstr.BufferLength + 1, 0));
        });

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var spanstr = test.GetSpanString();
            spanstr.Slice(new Utf8Index(-1, 0), new Utf8Index(0, 0));
        });
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var spanstr = test.GetSpanString();
            spanstr.Slice(new Utf8Index(spanstr.BufferLength, 0), new Utf8Index(spanstr.BufferLength + 1, 0));
        });
    }


    [Theory]
    [MemberData(nameof(Utf8StringTestCases))]
    public void TryGetRune_CanEnumerateValidly(Utf8TestCase test)
    {
        static void core<TString>(TString srt, StringRuneEnumerator expectedEnumerator)
            where TString : IRunaString<TString, Utf8Index>, allows ref struct
        {
            var byteIndex = 0;
            var runeIndex = 0;
            while (true)
            {
                var expectedMoveNext = expectedEnumerator.MoveNext();
                var actualTryGetRune = srt.TryGetRune(new Utf8Index(byteIndex, runeIndex), out var actualRune, out var codeUnitConsumed);
                Assert.Equal(expectedMoveNext, actualTryGetRune);
                if (!expectedMoveNext)
                {
                    break;
                }
                Assert.Equal(expectedEnumerator.Current, actualRune);
                byteIndex += codeUnitConsumed;
                ++runeIndex;
            }
        }

        core(test.GetMemoryString(), test.String.EnumerateRunes());
        core(test.GetSpanString(), test.String.EnumerateRunes());
    }


    [Fact]
    public void GetHashCode_Consistency()
    {
        var memoryConflictTimes = 0.0;
        var spanConflictTimes = 0.0;
        var totalTimes = 0;
        foreach (var case1 in TestHelpers.Utf8TestCases)
        {
            foreach (var case2 in TestHelpers.Utf8TestCases)
            {
                if (case1.String == case2.String)
                {
                    Assert.Equal(case1.GetMemoryString().GetHashCode(), case2.GetMemoryString().GetHashCode());
                    Assert.Equal(case1.GetSpanString().GetHashCode(), case2.GetSpanString().GetHashCode());
                }
                else
                {
                    if (case1.GetMemoryString().GetHashCode() == case2.GetMemoryString().GetHashCode())
                    {
                        ++memoryConflictTimes;
                    }
                    if (case1.GetSpanString().GetHashCode() == case2.GetSpanString().GetHashCode())
                    {
                        ++spanConflictTimes;
                    }
                    ++totalTimes;
                }
            }
        }
        Assert.True(memoryConflictTimes / totalTimes < 3.0 / int.MaxValue);
        Assert.True(spanConflictTimes / totalTimes < 3.0 / int.MaxValue);
    }

    [Fact]
    public void ObjectEquals_Memory()
    {
        var obj = new object();
        foreach (var case1 in TestHelpers.Utf8TestCases)
        {
            var str1 = case1.GetMemoryString();
            Assert.False(str1.Equals(null));
            Assert.False(str1.Equals(obj));
            foreach (var case2 in TestHelpers.Utf8TestCases)
            {
                var str2 = case2.GetMemoryString();
                if (case1.String == case2.String)
                {
                    Assert.True(str1.Equals((object)str2));
                }
                else
                {
                    Assert.False(str1.Equals((object)str2));
                }
            }
        }
    }

    [Fact]
    public void ObjectEquals_Span()
    {
        var obj = new object();
        foreach (var case1 in TestHelpers.Utf8TestCases)
        {
            var str1 = case1.GetSpanString();
            Assert.False(str1.Equals(null));
            Assert.False(str1.Equals(obj));
        }
    }
}
