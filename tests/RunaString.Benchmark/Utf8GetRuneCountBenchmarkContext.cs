using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace RunaString.Benchmark;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 1)]
[HardwareCounters(
        HardwareCounter.BranchMispredictions,
        HardwareCounter.BranchInstructions)]
public class Utf8GetRuneCountBenchmarkContext
{
    public IEnumerable<byte[]> GetUtf8Buffers()
    {
        yield return "Hello, world!"u8.ToArray();
        yield return BenchmarkHelpers.Utf8Bytes_Ascii;
        yield return BenchmarkHelpers.Utf8Bytes_Multilingual;
    }

    [ParamsSource(nameof(GetUtf8Buffers))]
    public byte[] TestBytes { get; set; } = default!;

    [Benchmark]
    public int DecodeAllRunes()
    {
        var str = Utf8String.DangerousFromUtf8(TestBytes);
        return InternalHelpers.GetRuneCount(str.GetEnumerator());
    }

    [Benchmark]
    public int Utf8Specific()
    {
        var str = Utf8String.DangerousFromUtf8(TestBytes);
        return str.GetRuneCount();
    }
}
