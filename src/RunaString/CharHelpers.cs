using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace RunaString;

internal static class CharHelpers
{
    internal static void ValidateUtf16(ReadOnlySpan<char> span)
    {
        for (var i = 0; i < span.Length; ++i)
        {
            if (char.IsHighSurrogate(span[i]))
            {
                ++i;
                if (i >= span.Length || !char.IsLowSurrogate(span[i]))
                {
                    throw new ArgumentException("The provided buffer contains an invalid UTF-16 sequence.");
                }
            }
            else if (char.IsLowSurrogate(span[i]))
            {
                throw new ArgumentException("The provided buffer contains an invalid UTF-16 sequence.");
            }
        }
    }


    private static readonly Vector256<ushort> _highSurrogateMin256 = Vector256.Create((ushort)0xD800);
    private static readonly Vector256<ushort> _highSurrogateMax256 = Vector256.Create((ushort)0xDBFF);
    private static readonly Vector128<ushort> _highSurrogateMin128 = Vector128.Create((ushort)0xD800);
    private static readonly Vector128<ushort> _highSurrogateMax128 = Vector128.Create((ushort)0xDBFF);

    public static int GetRuneCount(ReadOnlySpan<char> chars)
    {
        static int countHighSurrogate(ReadOnlySpan<ushort> chars)
        {
            var c = 0;
            if (Vector256.IsHardwareAccelerated)
            {
                while (chars.Length >= Vector256<ushort>.Count)
                {
                    var v = Vector256.LoadUnsafe(in chars[0]);
                    var isHighSurrogate = Vector256.GreaterThanOrEqual(v, _highSurrogateMin256) & Vector256.LessThanOrEqual(v, _highSurrogateMax256);
                    c += BitOperations.PopCount(Vector256.ExtractMostSignificantBits(isHighSurrogate));
                    chars = chars[Vector256<ushort>.Count..];
                }
            }
            if (Vector128.IsHardwareAccelerated)
            {
                while (chars.Length >= Vector128<ushort>.Count)
                {
                    var v = Vector128.LoadUnsafe(in chars[0]);
                    var isHighSurrogate = Vector128.GreaterThanOrEqual(v, _highSurrogateMin128) & Vector128.LessThanOrEqual(v, _highSurrogateMax128);
                    c += BitOperations.PopCount(Vector128.ExtractMostSignificantBits(isHighSurrogate));
                    chars = chars[Vector128<ushort>.Count..];
                }
            }
            foreach (var ch in chars)
            {
                if (ch >= 0xD800 && ch <= 0xDBFF)
                {
                    ++c;
                }
            }
            return c;
        }

        return chars.Length - countHighSurrogate(MemoryMarshal.Cast<char, ushort>(chars));
    }


    public static int GetHashCode(ReadOnlySpan<char> charsBuffer)
    {
        var hash = new HashCode();
        foreach (var x in MemoryMarshal.Cast<char, int>(charsBuffer))
        {
            hash.Add(x);
        }
        return hash.ToHashCode();
    }

}
