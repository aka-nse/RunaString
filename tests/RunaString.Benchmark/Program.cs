// #define USE_PROFILER

#pragma warning disable IDE0005
using BenchmarkDotNet.Running;
using RunaString.Benchmark;


// core<Utf8EnumeratorHelpersBenchmarkContext>();
// core<Utf8RuneEnumeratorBenchmarkContext>();
// core<Utf8CompareBenchmarkContext>();
// core<Utf8GetRuneCountBenchmarkContext>();
// core<Utf8SliceBenchmarkContext>();
// core<CharsGetRuneCountBenchmarkContext>();
// core<Utf8FormatBenchmarkContext>();
core<Utf8FormatBenchmarkContext>();

static void core<T>() where T : IProfilable, new()
{
#if USE_PROFILER
        var cxt = new T();
        for (var i = 0; i < 65536; ++i)
        {
            cxt.InvokeProfileTarget();
        }
#else
    BenchmarkRunner.Run<T>();
#endif
    Console.WriteLine("\a");
}