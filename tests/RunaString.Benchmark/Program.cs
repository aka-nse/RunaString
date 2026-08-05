using BenchmarkDotNet.Running;
using RunaString.Benchmark;

// BenchmarkRunner.Run<Utf8EnumeratorHelpersBenchmarkContext>();
// BenchmarkRunner.Run<Utf8RuneEnumeratorBenchmarkContext>();
// BenchmarkRunner.Run<Utf8CompareBenchmarkContext>();
// BenchmarkRunner.Run<Utf8GetRuneCountBenchmarkContext>();
// BenchmarkRunner.Run<Utf8SliceBenchmarkContext>();
BenchmarkRunner.Run<CharsGetRuneCountBenchmarkContext>();
