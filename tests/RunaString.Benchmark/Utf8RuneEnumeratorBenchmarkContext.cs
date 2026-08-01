using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace RunaString.Benchmark;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 1)]
[HardwareCounters(
        HardwareCounter.BranchMispredictions,
        HardwareCounter.BranchInstructions)]
public class Utf8RuneEnumeratorBenchmarkContext
{
    private static readonly byte[] _utf8Bytes =
        Encoding.UTF8.GetBytes(BenchmarkHelpers.TestString_Multilingual);

    [Benchmark]
    public int StandardCounting()
    {
        var count = 0;
        var span = _utf8Bytes.AsSpan();
        while(span.Length > 0)
        {
            Rune.DecodeFromUtf8(span, out _, out var bytesConsumed);
            span = span.Slice(bytesConsumed);
            count++;
        }
        return count;
    }

    [Benchmark]
    public int RuneEnumerator()
    {
        var str = Utf8SpanString.DangerousFromSpan(_utf8Bytes);
        var count = 0;
        foreach (var _ in str)
        {
            count++;
        }
        return count;
    }
}
