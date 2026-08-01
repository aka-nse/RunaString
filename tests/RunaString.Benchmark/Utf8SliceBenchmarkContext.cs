using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace RunaString.Benchmark;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 1)]
[HardwareCounters(
        HardwareCounter.BranchMispredictions,
        HardwareCounter.BranchInstructions)]
public class Utf8SliceBenchmarkContext
{
    public IEnumerable<(byte[] Bytes, int RuneStart, int RuneLength)> GetUtf8Buffers()
    {
        yield return ("Hello, world!"u8.ToArray(), 7, 5);
        {
            var length = BenchmarkHelpers.TestString_Ascii1.EnumerateRunes().Count();
            yield return (BenchmarkHelpers.Utf8Bytes_Ascii, length / 4, length / 2);
        }
        {
            var length = BenchmarkHelpers.TestString_Multilingual.EnumerateRunes().Count();
            yield return (BenchmarkHelpers.Utf8Bytes_Multilingual, length / 4, length / 2);
        }
    }

    [ParamsSource(nameof(GetUtf8Buffers))]
    public (byte[] Bytes, int RuneStart, int RuneLength) TestData { get; set; } = default!;

    [Benchmark]
    public Utf8String StandardIteration()
    {
        var str = Utf8String.DangerousFromUtf8(TestData.Bytes);
        var (start, end) = InternalHelpers.GetSliceIndex<Utf8String, Utf8MemoryEnumerator, Utf8Index>(str, TestData.RuneStart, TestData.RuneLength);
        return str.Slice(start, end);
    }

    [Benchmark]
    public Utf8String EscapeDecoding()
    {
        var str = Utf8String.DangerousFromUtf8(TestData.Bytes);
        return str.Slice(TestData.RuneStart, TestData.RuneLength);
    }
}
