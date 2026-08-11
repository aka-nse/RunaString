using System.Diagnostics;
using System.Text;

namespace RunaString.Test;

public class RunaUtf8InterpolationHandlerTest
{
    [Fact]
    public void Literal()
    {
        RunaUtf8InterpolationHandler handler = $"Hello, world!";
        var st = handler.MoveToUtf8String().ToString();
        Assert.Equal("Hello, world!", st);
    }

    [Fact]
    public void FormatCommon()
    {
        RunaUtf8InterpolationHandler handler = $"{new TestCommon()}";
        var st = handler.MoveToUtf8String().ToString();
        var expectedMethod = typeof(RunaUtf8InterpolationHandler)
            .GetMethods()
            .Where(static m => m.Name == nameof(RunaUtf8InterpolationHandler.AppendFormatted))
            .First(static m => m.GetParameters().Last().ParameterType == typeof(OverloadResolutionMarker));
        Assert.Equal($"{nameof(TestCommon)}.{nameof(ToString)}[caller={expectedMethod}]", st);
    }

    [Fact]
    public void FormatFormattable()
    {
        RunaUtf8InterpolationHandler handler = $"{new TestFormattable()}";
        var st = handler.MoveToUtf8String().ToString();
        var expectedMethod = typeof(RunaUtf8InterpolationHandler)
            .GetMethods()
            .Where(static m => m.Name == nameof(RunaUtf8InterpolationHandler.AppendFormatted))
            .First(static m => m.GetParameters().Last().ParameterType == typeof(OverloadResolutionMarker.AssignableFrom<IFormattable>));
        Assert.Equal($"{nameof(TestFormattable)}.{nameof(ToString)}[caller={expectedMethod}]", st);
    }

    [Fact]
    public void FormatSpanFormattable()
    {
        RunaUtf8InterpolationHandler handler = $"{new TestSpanFormattable()}";
        var st = handler.MoveToUtf8String().ToString();
        var expectedMethod = typeof(RunaUtf8InterpolationHandler)
            .GetMethods()
            .Where(static m => m.Name == nameof(RunaUtf8InterpolationHandler.AppendFormatted))
            .First(static m => m.GetParameters().Last().ParameterType == typeof(OverloadResolutionMarker.AssignableFrom<ISpanFormattable>));
        Assert.Equal($"{nameof(TestSpanFormattable)}.{nameof(ToString)}[caller={expectedMethod}]", st);
    }

    [Fact]
    public void FormatUtf8SpanFormattable()
    {
        RunaUtf8InterpolationHandler handler = $"{new TestUtf8SpanFormattable()}";
        var st = handler.MoveToUtf8String().ToString();
        var expectedMethod = typeof(RunaUtf8InterpolationHandler)
            .GetMethods()
            .Where(static m => m.Name == nameof(RunaUtf8InterpolationHandler.AppendFormatted))
            .First(static m => m.GetParameters().Last().ParameterType == typeof(OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>));
        Assert.Equal($"{nameof(TestUtf8SpanFormattable)}.{nameof(ToString)}[caller={expectedMethod}]", st);
    }
}


file abstract class TestBase
{
    protected static string GetResolvedAppendFormatted()
    {
        var trace = new StackTrace(0);
        for (var i = 0; trace.GetFrame(i) is { } frame; ++i)
        {
            var method = frame.GetMethod();
            if (method?.DeclaringType == typeof(RunaUtf8InterpolationHandler))
            {
                var caller = method.ToString();
                return $"[caller={caller}]";
            }
        }
        return "";
    }
}


file class TestCommon : TestBase
{
    public override string ToString() =>
        $"{nameof(TestCommon)}.{nameof(ToString)}{GetResolvedAppendFormatted()}";
}


file class TestFormattable : TestBase, IFormattable
{
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        $"{nameof(TestFormattable)}.{nameof(ToString)}{GetResolvedAppendFormatted()}";
}

file class TestSpanFormattable : TestBase, ISpanFormattable
{
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
    {
        var s = $"{nameof(TestSpanFormattable)}.{nameof(TryFormat)}{GetResolvedAppendFormatted()}";
        if (s.Length > destination.Length)
        {
            charsWritten = 0;
            return false;
        }
        s.AsSpan().CopyTo(destination);
        charsWritten = s.Length;
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        $"{nameof(TestSpanFormattable)}.{nameof(ToString)}{GetResolvedAppendFormatted()}";
}

file class TestUtf8SpanFormattable : TestBase, IUtf8SpanFormattable, ISpanFormattable
{
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var s = $"{nameof(TestUtf8SpanFormattable)}.{nameof(TryFormat)}{GetResolvedAppendFormatted()}";
        return Encoding.UTF8.TryGetBytes(s, utf8Destination, out bytesWritten);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
    {
        var s = $"{nameof(TestUtf8SpanFormattable)}.{nameof(TryFormat)}{GetResolvedAppendFormatted()}";
        if (s.Length > destination.Length)
        {
            charsWritten = 0;
            return false;
        }
        s.AsSpan().CopyTo(destination);
        charsWritten = s.Length;
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        $"{nameof(TestUtf8SpanFormattable)}.{nameof(ToString)}{GetResolvedAppendFormatted()}";
}
