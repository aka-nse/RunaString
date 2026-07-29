using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace RunaString.Benchmark;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 1)]
[HardwareCounters(
        HardwareCounter.BranchMispredictions,
        HardwareCounter.BranchInstructions)]
public class Utf8CompareBenchmarkContext
{
    private static readonly byte[] _utf8Bytes1 =
        Encoding.UTF8.GetBytes(BenchmarkHelpers.TestString_Ascii1);
    private static readonly byte[] _utf8Bytes2 =
        Encoding.UTF8.GetBytes(BenchmarkHelpers.TestString_Ascii2);


    [Benchmark(Baseline = true)]
    public int GenericCompare()
    {
        var str1 = Utf8String.DangerousFromUtf8(_utf8Bytes1);
        var str2 = Utf8String.DangerousFromUtf8(_utf8Bytes2);
        return InternalHelpers.Compare<Utf8String, Utf8MemoryEnumerator>(str1, str2);
    }


    [Benchmark]
    public int SpecializedCompare()
    {
        var str1 = Utf8String.DangerousFromUtf8(_utf8Bytes1);
        var str2 = Utf8String.DangerousFromUtf8(_utf8Bytes2);
        return Utf8String.Compare(str1, str2);
    }
}
