using System.Text;

namespace RunaString.Test;

public partial class Utf8ComparerTest
{
    public static TheoryData<string> Default_EqualsAndCompareTestCases() => [
            "", /* 0-length */
            "abc", /* length less than sizeof(uint) */
            "0123456789ABCDEF",
            """
            Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
            Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
            Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
            Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
            """,
        ];

    [Theory]
    [MemberData(nameof(Default_EqualsAndCompareTestCases))]
    public void Default_EqualsAndCompare(string testText)
    {
        var comparer = Utf8Comparer.Default;
        var x = Utf8String.FromUtf8([.. Encoding.UTF8.GetBytes(testText)]);
        Assert.True(comparer.Equals(x, x));

        {
            // length differs
            var y = Utf8String.FromUtf8([.. Encoding.UTF8.GetBytes(testText + ' ')]);
            Assert.False(comparer.Equals(x, y));
            Assert.Equal(0, comparer.Compare(x, x));
            Assert.True(comparer.Compare(x, y) < 0);
            Assert.True(comparer.Compare(y, x) > 0);
        }

        for(var i = 0; i < testText.Length; ++i)
        {
            var y = Utf8String.FromUtf8([.. Encoding.UTF8.GetBytes(
                testText[0..i] +
                '\u007E' /* larger than every char in testText regarding sort order */ +
                testText[(i+1)..])]);
            Assert.False(comparer.Equals(x, y));
            Assert.Equal(0, comparer.Compare(x, x));
            Assert.True(comparer.Compare(x, y) < 0);
        }
    }
}
