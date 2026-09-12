using System.Numerics;
using System.Runtime.CompilerServices;
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

    private static readonly Vector256<ushort> _nonAsciiBit256 = Vector256.Create((ushort)0xFF80);
    private static readonly Vector128<ushort> _nonAsciiBit128 = Vector128.Create((ushort)0xFF80);


    public static int CountAsciiCharFromAhead(scoped ReadOnlySpan<char> s)
    {
        var i = 0;
        var us = MemoryMarshal.Cast<char, ushort>(s);
        if(Vector256.IsHardwareAccelerated)
        {
            while (us.Length - i >= Vector256<ushort>.Count)
            {
                var v = Vector256.LoadUnsafe(in us[i]);
                var isNonAscii = Vector256.BitwiseAnd(v, _nonAsciiBit256);
                if (!Vector256.EqualsAll(isNonAscii, Vector256<ushort>.Zero))
                {
                    break;
                }
                i += Vector256<ushort>.Count;
            }
        }
        if(Vector128.IsHardwareAccelerated)
        {
            while (us.Length - i >= Vector128<ushort>.Count)
            {
                var v = Vector128.LoadUnsafe(in us[i]);
                var isNonAscii = Vector128.BitwiseAnd(v, _nonAsciiBit128);
                if (!Vector128.EqualsAll(isNonAscii, Vector128<ushort>.Zero))
                {
                    break;
                }
                i += Vector128<ushort>.Count;
            }
        }
        while(us.Length - i >= sizeof(ulong) / sizeof(ushort))
        {
            unsafe
            {
                var uss = MemoryMarshal.Cast<ushort, byte>(us[i..]);
                var t = Unsafe.ReadUnaligned<ulong>(ref Unsafe.AsRef(in uss[0])) & 0xFF80_FF80_FF80_FF80uL;
                if (t != 0)
                {
                    break;
                }
                i += sizeof(ulong) / sizeof(ushort);
            }
        }
        while (i < s.Length)
        {
            if (!char.IsAscii(s[i]))
            {
                break;
            }

            ++i;
        }
        return i;
    }


    public static int CountNonAsciiCharFromAhead(scoped ReadOnlySpan<char> s)
    {
        var i = 0;
        var us = MemoryMarshal.Cast<char, ushort>(s);
        if(Vector256.IsHardwareAccelerated)
        {
            while (us.Length - i >= Vector256<ushort>.Count)
            {
                var v = Vector256.LoadUnsafe(in us[i]);
                var isNonAscii = Vector256.BitwiseAnd(v, _nonAsciiBit256);
                if (Vector256.EqualsAny(isNonAscii, Vector256<ushort>.Zero))
                {
                    break;
                }
                i += Vector256<ushort>.Count;
            }
        }
        if (Vector128.IsHardwareAccelerated)
        {
            while (us.Length - i >= Vector128<ushort>.Count)
            {
                var v = Vector128.LoadUnsafe(in us[i]);
                var isNonAscii = Vector128.BitwiseAnd(v, _nonAsciiBit128);
                if (Vector128.EqualsAny(isNonAscii, Vector128<ushort>.Zero))
                {
                    break;
                }
                i += Vector128<ushort>.Count;
            }
        }
        while(us.Length - i >= sizeof(ulong) / sizeof(ushort))
        {
            unsafe
            {
                var uss = MemoryMarshal.Cast<ushort, byte>(us[i..]);
                var t = Unsafe.ReadUnaligned<ulong>(ref Unsafe.AsRef(in uss[0])) & 0xFF80_FF80_FF80_FF80uL;
                var z = (t - 0x0001000100010001uL) & ~t & 0x0080008000800080uL;
                if (z != 0)
                {
                    break;
                }
                i += sizeof(ulong) / sizeof(ushort);
            }
        }
        while (i < s.Length)
        {
            if (char.IsAscii(s[i]))
            {
                break;
            }

            ++i;
        }
        return i;
    }


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
        if (Vector256.IsHardwareAccelerated)
        {
            while (charsBuffer.Length > Vector256<byte>.Count)
            {
                var vector = Vector256.LoadUnsafe(in charsBuffer[0]);
                hash.Add(vector);
                charsBuffer = charsBuffer[Vector256<byte>.Count..];
            }
        }
        if (Vector128.IsHardwareAccelerated)
        {
            while (charsBuffer.Length > Vector128<byte>.Count)
            {
                var vector = Vector128.LoadUnsafe(in charsBuffer[0]);
                hash.Add(vector);
                charsBuffer = charsBuffer[Vector128<byte>.Count..];
            }
        }
        foreach (var c in charsBuffer)
        {
            hash.Add(c);
        }
        return hash.ToHashCode();
    }

}
