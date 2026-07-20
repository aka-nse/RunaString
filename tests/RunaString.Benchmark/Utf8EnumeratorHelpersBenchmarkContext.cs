using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace RunaString.Benchmark;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 1)]
[HardwareCounters(
        HardwareCounter.BranchMispredictions,
        HardwareCounter.BranchInstructions)]
public class Utf8EnumeratorHelpersBenchmarkContext
{
    [Benchmark(Baseline = true)]
    public int StandardCounting()
    {
        var count = 0;
        var span = BenchmarkHelpers.Utf8Bytes.AsSpan();
        while (span.Length > 0)
        {
            Rune.DecodeFromUtf8(span, out _, out var bytesConsumed);
            span = span.Slice(bytesConsumed);
            count++;
        }
        return count;
    }

    [Benchmark]
    public int Utf8EnumeratorCounting()
    {
        var count = 0;
        var span = BenchmarkHelpers.Utf8Bytes.AsSpan();
        int nextByteIndex = 0;
        int runeIndex = 0;
        while (Utf8Helpers.TryGetRuneAndMoveNext(span, ref nextByteIndex, ref runeIndex, out var _))
        {
            count++;
        }
        return count;
    }

    [Benchmark]
    public int LegacyCounting()
    {
        var count = 0;
        var span = BenchmarkHelpers.Utf8Bytes.AsSpan();
        int nextByteIndex = 0;
        int runeIndex = 0;
        while (MoveNextLegacy(span, ref nextByteIndex, ref runeIndex, out var _))
        {
            count++;
        }
        return count;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool MoveNextLegacy(
        ReadOnlySpan<byte> utf8Buffer,
        ref int nextByteIndex,
        ref int runeIndex,
        out Rune current)
    {
        if (nextByteIndex >= utf8Buffer.Length)
        {
            current = default;
            return false;
        }
        if (utf8Buffer[nextByteIndex] <= 0x7F)
        {
            current = Unsafe.BitCast<uint, Rune>(utf8Buffer[nextByteIndex]);
            ++nextByteIndex;
            ++runeIndex;
            return true;
        }

        decodeMultiBytes(utf8Buffer, nextByteIndex, out current, out var bytesConsumed);
        nextByteIndex += bytesConsumed;
        ++runeIndex;
        return true;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static bool decodeMultiBytes(ReadOnlySpan<byte> utf8Buffer, int byteIndex, out Rune rune, out int bytesConsumed)
        {
            var len = utf8Buffer.Length;
            if ((uint)byteIndex >= (uint)len)
            {
                rune = default;
                bytesConsumed = 0;
                return false;
            }

            uint value;
            var b0 = utf8Buffer[byteIndex];

            // ASCII fast-path  0xxxx-xxxx
            if (b0 <= 0x7F)
            {
                value = b0;
                bytesConsumed = 1;
                goto succeeded;
            }

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
                var b1 = utf8Buffer[byteIndex + 1];
                var b2 = utf8Buffer[byteIndex + 2];
                if ((b1 & 0xC0) != 0x80 || (b2 & 0xC0) != 0x80)
                {
                    goto invalid;
                }
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
                var b1 = utf8Buffer[byteIndex + 1];
                var b2 = utf8Buffer[byteIndex + 2];
                var b3 = utf8Buffer[byteIndex + 3];
                if ((b1 & 0xC0) != 0x80 || (b2 & 0xC0) != 0x80 || (b3 & 0xC0) != 0x80)
                {
                    goto invalid;
                }
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

    }

}
