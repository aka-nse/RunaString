using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

namespace RunaString.Benchmark;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 1)]
// [HardwareCounters(
//         HardwareCounter.BranchMispredictions,
//         HardwareCounter.BranchInstructions)]
public class Utf8FormatBenchmarkContext : IProfilable
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int GetUnoptimizableInt() =>
        unchecked((int)0xDEADBEEF);
    private static readonly int _unoptimizableInt = GetUnoptimizableInt();

    private record Formattable() : IFormattable
    {
        public string ToString(string? format, IFormatProvider? formatProvider) => $"Formattable.ToString(format={format})";
    }
    private static readonly Formattable _formattable = new();

    private static readonly object _unformattable = new();

    public void InvokeProfileTarget() => Utf8StringFormat_Int();

    [Benchmark(Baseline = true)]
    public Utf8String StringFormat_Int()
    {
        var str = $"""
            Hello, world! {_unoptimizableInt} {_unoptimizableInt:x08} {_unoptimizableInt,16} Hello, world!
            Hello, world! {_unoptimizableInt} {_unoptimizableInt:x08} {_unoptimizableInt,16} Hello, world!
            Hello, world! {_unoptimizableInt} {_unoptimizableInt:x08} {_unoptimizableInt,16} Hello, world!
            """;
        return Utf8String.DangerousFromUtf8(Encoding.UTF8.GetBytes(str));
    }

    [Benchmark]
    public Utf8String Utf8StringFormat_Int() =>
        Utf8String.FromFormat(
            $"""
            Hello, world! {_unoptimizableInt} {_unoptimizableInt:x08} {_unoptimizableInt,16} Hello, world!
            Hello, world! {_unoptimizableInt} {_unoptimizableInt:x08} {_unoptimizableInt,16} Hello, world!
            Hello, world! {_unoptimizableInt} {_unoptimizableInt:x08} {_unoptimizableInt,16} Hello, world!
            """);

    [Benchmark]
    public Utf8String StringFormat_Formattable()
    {
        var str = $"""
            Hello, world! {_formattable} {_formattable:x08} {_formattable,16} Hello, world!
            Hello, world! {_formattable} {_formattable:x08} {_formattable,16} Hello, world!
            Hello, world! {_formattable} {_formattable:x08} {_formattable,16} Hello, world!
            """;
        return Utf8String.DangerousFromUtf8(Encoding.UTF8.GetBytes(str));
    }

    [Benchmark]
    public Utf8String Utf8StringFormat_Formattable() =>
        Utf8String.FromFormat(
            $"""
            Hello, world! {_formattable} {_formattable:x08} {_formattable,16} Hello, world!
            Hello, world! {_formattable} {_formattable:x08} {_formattable,16} Hello, world!
            Hello, world! {_formattable} {_formattable:x08} {_formattable,16} Hello, world!
            """);

    [Benchmark]
    public Utf8String StringFormat_Unformattable()
    {
        var str = $"""
            Hello, world! {_unformattable} {_unformattable:x08} {_unformattable,16} Hello, world!
            Hello, world! {_unformattable} {_unformattable:x08} {_unformattable,16} Hello, world!
            Hello, world! {_unformattable} {_unformattable:x08} {_unformattable,16} Hello, world!
            """;
        return Utf8String.DangerousFromUtf8(Encoding.UTF8.GetBytes(str));
    }

    [Benchmark]
    public Utf8String Utf8StringFormat_Unformattable() =>
        Utf8String.FromFormat(
            $"""
            Hello, world! {_unformattable} {_unformattable:x08} {_unformattable,16} Hello, world!
            Hello, world! {_unformattable} {_unformattable:x08} {_unformattable,16} Hello, world!
            Hello, world! {_unformattable} {_unformattable:x08} {_unformattable,16} Hello, world!
            """);
}
