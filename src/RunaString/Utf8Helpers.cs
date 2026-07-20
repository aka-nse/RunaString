using System.Runtime.CompilerServices;
using System.Text;

namespace RunaString;

internal static class Utf8Helpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int GetNextByteIndex(ReadOnlySpan<byte> utf8Buffer, int currentByteIndex)
    {
        if (currentByteIndex >= utf8Buffer.Length)
        {
            return utf8Buffer.Length;
        }
        Rune.DecodeFromUtf8(utf8Buffer.Slice(currentByteIndex), out _, out var bytesConsumed);
        return currentByteIndex + bytesConsumed;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool TryGetRuneAndMoveNext(
        ReadOnlySpan<byte> utf8Buffer,
        ref int byteIndex,
        ref int runeIndex,
        out Rune current)
    {
        if (byteIndex >= utf8Buffer.Length)
        {
            current = default;
            return false;
        }
        if (utf8Buffer[byteIndex] <= 0x7F)
        {
            current = Unsafe.BitCast<uint, Rune>(utf8Buffer[byteIndex]);
            ++byteIndex;
            ++runeIndex;
            return true;
        }
        else
        {
            decodeMultiBytes(utf8Buffer, byteIndex, out current, out var bytesConsumed);
            byteIndex += bytesConsumed;
            ++runeIndex;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static bool decodeMultiBytes(ReadOnlySpan<byte> utf8Buffer, int byteIndex, out Rune rune, out int bytesConsumed)
        {
            uint value;
            var len = utf8Buffer.Length;
            var b0 = utf8Buffer[byteIndex];

            // 2-byte sequence  110x-xxxx 10xx-xxxx
            if ((b0 & 0xE0) == 0xC0)
            {
                if (byteIndex + 1 >= len)
                {
                    goto invalid;
                }
                var b1 = utf8Buffer[byteIndex + 1];
                if ((b1 & 0xC0) != 0x80)
                {
                    goto invalid;
                }
                if (b0 < 0xC2)
                {
                    // overlong
                    goto invalid;
                }
                value = ((b0 & 0x1Fu) << 6) | (b1 & 0x3Fu);
                bytesConsumed = 2;
                goto succeeded;
            }

            // 3-byte sequence  1110-xxxx 10xx-xxxx 10xx-xxxx
            if ((b0 & 0xF0) == 0xE0)
            {
                if (byteIndex + 2 >= len)
                {
                    goto invalid;
                }

                // ****-**** 10xx-xxxx 10xx-xxxx
                if ((readUnaligned<ushort>(utf8Buffer, byteIndex + 1) & 0xC0C0u) != 0x8080u)
                {
                    goto invalid;
                }

                var b1 = utf8Buffer[byteIndex + 1];
                var b2 = utf8Buffer[byteIndex + 2];
                if (b0 == 0xE0 && b1 < 0xA0)
                {
                    // overlong
                    goto invalid;
                }
                if (b0 == 0xED && b1 >= 0xA0)
                {
                    // surrogate
                    goto invalid;
                }
                value = ((b0 & 0xFu) << 12) | ((b1 & 0x3Fu) << 6) | (b2 & 0x3Fu);
                bytesConsumed = 3;
                goto succeeded;
            }

            // 4-byte sequence  1111-0xxx 10xx-xxxx 10xx-xxxx 10xx-xxxx
            if ((b0 & 0xF8) == 0xF0)
            {
                if (byteIndex + 3 >= len)
                {
                    goto invalid;
                }
                if (b0 > 0xF4)
                {
                    // > U+10FFFF
                    goto invalid;
                }

                // ****-**** 10xx-xxxx 10xx-xxxx 10xx-xxxx
                if ((readUnaligned<uint>(utf8Buffer, byteIndex) & _bitMask4Byte) != _bitPattern4Byte)
                {
                    goto invalid;
                }

                var b1 = utf8Buffer[byteIndex + 1];
                var b2 = utf8Buffer[byteIndex + 2];
                var b3 = utf8Buffer[byteIndex + 3];
                // // previous implementation
                // if ((b1 & 0xC0) != 0x80 || (b2 & 0xC0) != 0x80 || (b3 & 0xC0) != 0x80)
                // {
                //     goto invalid;
                // }
                if (b0 == 0xF0 && b1 < 0x90)
                {
                    // overlong
                    goto invalid;
                }
                if (b0 == 0xF4 && b1 >= 0x90)
                {
                    // > U+10FFFF
                    goto invalid;
                }
                value = ((b0 & 0x7u) << 18) | ((b1 & 0x3Fu) << 12) | ((b2 & 0x3Fu) << 6) | (b3 & 0x3Fu);
                bytesConsumed = 4;
                goto succeeded;
            }

        invalid:
            rune = Rune.ReplacementChar;
            bytesConsumed = 1;
            return true;

        succeeded:
            rune = Unsafe.BitCast<uint, Rune>(value);
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static T readUnaligned<T>(ReadOnlySpan<byte> span, int index)
            where T : unmanaged
        {
            unsafe
            {
                return Unsafe.ReadUnaligned<T>(ref Unsafe.AsRef(in span[index]));
            }
        }
    }
    private static readonly uint _bitMask4Byte = BitConverter.IsLittleEndian ? 0xC0C0C000u : 0x00C0C0C0u;
    private static readonly uint _bitPattern4Byte = BitConverter.IsLittleEndian ? 0x80808000u : 0x00808080u;
}