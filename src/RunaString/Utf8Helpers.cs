// #define VALIDATE_UTF8

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        RuneHelpers.DecodeFromUtf8At(utf8Buffer, currentByteIndex, out _, out var bytesConsumed);
        return currentByteIndex + bytesConsumed;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool UnsafeTryGetRuneAndMoveNext(
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
            unsafe
            {
                current = Unsafe.BitCast<uint, Rune>(utf8Buffer[byteIndex]);
            }
            ++byteIndex;
            ++runeIndex;
            return true;
        }
        else
        {
            int bytesConsumed;
            unsafe
            {
                decodeMultiBytes(utf8Buffer, byteIndex, out current, out bytesConsumed);
            }
            byteIndex += bytesConsumed;
            ++runeIndex;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static unsafe bool decodeMultiBytes(ReadOnlySpan<byte> utf8Buffer, int byteIndex, out Rune rune, out int bytesConsumed)
        {
            Unsafe.SkipInit(out bytesConsumed);
            Unsafe.SkipInit(out uint value);
            var len = utf8Buffer.Length;
            ref var b0 = ref Unsafe.Add(ref MemoryMarshal.GetReference(utf8Buffer), byteIndex);

            // 2-byte sequence  110x-xxxx 10xx-xxxx
            if ((b0 & 0xE0) == 0xC0)
            {
#if VALIDATE_UTF8
                if (byteIndex + 1 >= len)
                {
                    goto invalid;
                }
#endif
                var b1 = Unsafe.Add(ref b0, 1);
#if VALIDATE_UTF8
                if ((b1 & 0xC0) != 0x80)
                {
                    goto invalid;
                }
                if (b0 < 0xC2)
                {
                    // overlong
                    goto invalid;
                }
#endif
                value = ((b0 & 0x1Fu) << 6) | (b1 & 0x3Fu);
                bytesConsumed = 2;
                goto succeeded;
            }

            // 3-byte sequence  1110-xxxx 10xx-xxxx 10xx-xxxx
            if ((b0 & 0xF0) == 0xE0)
            {
#if VALIDATE_UTF8
                if (byteIndex + 2 >= len)
                {
                    goto invalid;
                }
#endif

                ref var b1 = ref Unsafe.Add(ref b0, 1);

#if VALIDATE_UTF8
                // ****-**** 10xx-xxxx 10xx-xxxx
                if ((Unsafe.ReadUnaligned<ushort>(ref b1) & 0xC0C0u) != 0x8080u)
                {
                    goto invalid;
                }
#endif

                var b2 = Unsafe.Add(ref b0, 2);
#if VALIDATE_UTF8
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
#endif
                value = ((b0 & 0xFu) << 12) | ((b1 & 0x3Fu) << 6) | (b2 & 0x3Fu);
                bytesConsumed = 3;
                goto succeeded;
            }

            // 4-byte sequence  1111-0xxx 10xx-xxxx 10xx-xxxx 10xx-xxxx
            if ((b0 & 0xF8) == 0xF0)
            {
#if VALIDATE_UTF8
                if (byteIndex + 3 >= len)
                {
                    goto invalid;
                }
                if (b0 > 0xF4)
                {
                    // > U+10FFFF
                    goto invalid;
                }
#endif

#if VALIDATE_UTF8
                // ****-**** 10xx-xxxx 10xx-xxxx 10xx-xxxx
                if ((Unsafe.ReadUnaligned<uint>(ref b0) & _bitMask4Byte) != _bitPattern4Byte)
                {
                    goto invalid;
                }
#endif

                var b1 = Unsafe.Add(ref b0, 1);
                var b2 = Unsafe.Add(ref b0, 2);
                var b3 = Unsafe.Add(ref b0, 3);
#if VALIDATE_UTF8
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
#endif
                value = ((b0 & 0x7u) << 18) | ((b1 & 0x3Fu) << 12) | ((b2 & 0x3Fu) << 6) | (b3 & 0x3Fu);
                bytesConsumed = 4;
                goto succeeded;
            }

#if VALIDATE_UTF8
        invalid:
            rune = Rune.ReplacementChar;
            bytesConsumed = 1;
            return true;
#endif

        succeeded:
            rune = Unsafe.BitCast<uint, Rune>(value);
            return true;
        }
    }

#if VALIDATE_UTF8
    private static readonly uint _bitMask4Byte = BitConverter.IsLittleEndian ? 0xC0C0C000u : 0x00C0C0C0u;
    private static readonly uint _bitPattern4Byte = BitConverter.IsLittleEndian ? 0x80808000u : 0x00808080u;
#endif
}