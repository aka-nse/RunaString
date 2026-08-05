using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace RunaString.Benchmark;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 1)]
[HardwareCounters(
        HardwareCounter.BranchMispredictions,
        HardwareCounter.BranchInstructions)]
public class CharsGetRuneCountBenchmarkContext
{
    public IEnumerable<string> GetCharsBuffers()
    {
        yield return "Hello, world!";
        yield return BenchmarkHelpers.TestString_Ascii1;
        yield return BenchmarkHelpers.TestString_Multilingual;
    }

    [ParamsSource(nameof(GetCharsBuffers))]
    public string TestString { get; set; } = default!;

    [Benchmark]
    public int DecodeAllRunes()
    {
        var str = TestString.AsRunaString();
        return InternalHelpers.GetRuneCount(str.GetEnumerator());
    }

    [Benchmark]
    public int CharsSpecific()
    {
        var str = TestString.AsRunaString();
        return str.GetRuneCount();
    }
}
