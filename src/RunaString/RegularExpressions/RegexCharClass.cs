using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace RunaString.RegularExpressions;

[RegexSourceMigration(
    "System/Text/RegularExpressions/RegexCharClass.cs",
    ["System", "System.Text", "System.Text.RegularExpressions"])]
[RegexSourceMigration(
    "System/Text/RegularExpressions/RegexCharClass.Tables.cs",
    ["System", "System.Text", "System.Text.RegularExpressions"])]
internal partial class RegexCharClass
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CharInClass(Rune rune, string set, ref uint[]? asciiLazyCache)
    {
        const int cacheArrayLength = 8;
        Debug.Assert(asciiLazyCache is null || asciiLazyCache.Length == cacheArrayLength);
        if (asciiLazyCache is uint[] cache)
        {
            var index = rune.Value >> 4;
            if ((uint)index < (uint)cache.Length)
            {
                Debug.Assert(rune.Value < 128);
                var current = cache[index];
                var bit = 1u << ((rune.Value & 0xF) << 1);
                if ((current & bit) != 0)
                {
                    return (current & (bit << 1)) != 0;
                }
            }
        }

        return rune.Value < 128
            ? InitializeValue(rune, set, ref asciiLazyCache)
            : CharInClassIterative(rune, set, 0);

        static bool InitializeValue(Rune rune, string set, ref uint[]? asciiLazyCache)
        {
            Debug.Assert(rune.Value < 128);
            var isInClass = CharInClass(rune, set);
            var bitsToSet = 1u << ((rune.Value & 0xF) << 1);
            if (isInClass)
            {
                bitsToSet |= bitsToSet << 1;
            }

            var cache = asciiLazyCache
                ?? Interlocked.CompareExchange(ref asciiLazyCache, new uint[cacheArrayLength], null)
                ?? asciiLazyCache;
            Interlocked.Or(ref cache[rune.Value >> 4], bitsToSet);
            return isInClass;
        }
    }


    public static bool CharInClass(Rune ch, string set) =>
        CharInClassIterative(ch, set, 0);


    private static bool CharInClassIterative(Rune rune, string set, int start)
    {
        var inClass = false;
        while (true)
        {
            var setLength = set[start + SetLengthIndex];
            var categoryLength = set[start + CategoryLengthIndex];
            var endPosition = start + SetStartIndex + setLength + categoryLength;
            if (CharInClassInternal(rune, set, start, setLength, categoryLength) == IsNegated(set, start))
            {
                break;
            }

            inClass = !inClass;
            if (set.Length <= endPosition)
            {
                break;
            }

            start = endPosition;
        }

        return inClass;
    }


    private static bool CharInClassInternal(Rune rune, string set, int start, int setLength, int categoryLength)
    {
        var min = start + SetStartIndex;
        var max = min + setLength;
        while (min != max)
        {
            var mid = (min + max) >> 1;
            if (rune.Value < set[mid])
            {
                max = mid;
            }
            else
            {
                min = mid + 1;
            }
        }

        Debug.Assert((SetStartIndex & 0x1) == 1);
        if ((min & 0x1) == (start & 0x1))
        {
            return true;
        }

        if (categoryLength == 0)
        {
            return false;
        }

        return CharInCategory(rune, set.AsSpan(SetStartIndex + start + setLength, categoryLength));
    }


    private static bool CharInCategory(Rune rune, ReadOnlySpan<char> categorySetSegment)
    {
        var chcategory = Rune.GetUnicodeCategory(rune);
        for (var i = 0; i < categorySetSegment.Length; i++)
        {
            var curcat = (short)categorySetSegment[i];
            if (curcat == 0)
            {
                if (CharInCategoryGroup(chcategory, categorySetSegment, ref i))
                {
                    return true;
                }
            }
            else if (curcat > 0)
            {
                if (curcat == SpaceConst)
                {
                    if (Rune.IsWhiteSpace(rune))
                    {
                        return true;
                    }
                }
                else if (chcategory == (UnicodeCategory)(curcat - 1))
                {
                    return true;
                }
            }
            else
            {
                if (curcat == NotSpaceConst)
                {
                    if (!Rune.IsWhiteSpace(rune))
                    {
                        return true;
                    }
                }
                else if (chcategory != (UnicodeCategory)(-1 - curcat))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
