using System.Runtime.CompilerServices;

namespace RunaString.Test;

public partial class RuneIndexTest
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = ".ctor")]
    private static extern void ctor_Utf8Index(ref Utf8Index index, int byteIndex, int runePosition);
    private static Utf8Index CreateUtf8Index(int byteIndex, int runePosition)
    {
        Utf8Index index = default;
        ctor_Utf8Index(ref index, byteIndex, runePosition);
        return index;
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = ".ctor")]
    private static extern void ctor_CharsIndex(ref CharsIndex index, int charIndex, int runePosition);
    private static CharsIndex CreateCharsIndex(int charIndex, int runePosition)
    {
        CharsIndex index = default;
        ctor_CharsIndex(ref index, charIndex, runePosition);
        return index;
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = ".ctor")]
    private static extern void ctor_RunesIndex(ref RunesIndex index, int runePosition);
    private static RunesIndex CreateRunesIndex(int runePosition)
    {
        RunesIndex index = default;
        ctor_RunesIndex(ref index, runePosition);
        return index;
    }

    public static TheoryData<ISeekIndex, ISeekIndex, int> EquatableComparableTestCase() =>
        new()
        {
            { CreateUtf8Index(0, 0), CreateUtf8Index(0, 0), 0 },
            { CreateUtf8Index(1, 1), CreateUtf8Index(1, 1), 0 },
            { CreateUtf8Index(100, 50), CreateUtf8Index(100, 50), 0 },
            { CreateUtf8Index(10, 5), CreateUtf8Index(100, 50), -1 },
            { CreateUtf8Index(100, 50), CreateUtf8Index(10, 5), 1 },

            { CreateCharsIndex(0, 0), CreateCharsIndex(0, 0), 0 },
            { CreateCharsIndex(1, 1), CreateCharsIndex(1, 1), 0 },
            { CreateCharsIndex(60, 50), CreateCharsIndex(60, 50), 0 },
            { CreateCharsIndex(6, 5), CreateCharsIndex(60, 50), -1 },
            { CreateCharsIndex(60, 50), CreateCharsIndex(6, 5), 1 },

            { CreateRunesIndex(0), CreateRunesIndex(0), 0 },
            { CreateRunesIndex(1), CreateRunesIndex(1), 0 },
            { CreateRunesIndex(50), CreateRunesIndex(50), 0 },
            { CreateRunesIndex(5), CreateRunesIndex(50), -1 },
            { CreateRunesIndex(50), CreateRunesIndex(5), 1 },
        };


    [Theory]
    [MemberData(nameof(EquatableComparableTestCase))]
    public void Equal<TIndex>(TIndex x, TIndex y, int expected)
        where TIndex : unmanaged, ISeekIndex<TIndex>
    {
        var exp = expected == 0;
        Assert.Equal(exp, TIndex.Equals(x, y));
        Assert.Equal(exp, x.Equals(y));
        Assert.Equal(exp, y.Equals(x));
        Assert.Equal(exp, x.Equals((object)y));
        Assert.Equal(exp, y.Equals((object)x));
        Assert.Equal(exp, x == y);
        Assert.Equal(!exp, x != y);
        if (exp)
        {
            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }
    }


    [Theory]
    [MemberData(nameof(EquatableComparableTestCase))]
    public void Compare<TIndex>(TIndex x, TIndex y, int expected)
        where TIndex : unmanaged, ISeekIndex<TIndex>
    {
        Assert.Equal(Math.Sign(expected), Math.Sign(TIndex.Compare(x, y)));
        Assert.Equal(Math.Sign(expected), Math.Sign(x.CompareTo(y)));
        Assert.Equal(-Math.Sign(expected), Math.Sign(y.CompareTo(x)));
        Assert.Equal(expected < 0, x < y);
        Assert.Equal(expected > 0, x > y);
        Assert.Equal(expected <= 0, x <= y);
        Assert.Equal(expected >= 0, x >= y);
    }
}
