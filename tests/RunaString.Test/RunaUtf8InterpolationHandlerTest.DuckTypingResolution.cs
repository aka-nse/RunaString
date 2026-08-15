using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace RunaString.Test;

public partial class RunaUtf8InterpolationHandlerTest
{
    public class DuckTypingNameResolution
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
            Assert.Equal($"{nameof(TestCommon)}.{nameof(ToString)}[caller={expectedMethod.ToDisplayString()}]", st);
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
            Assert.Equal($"{nameof(TestFormattable)}.{nameof(ToString)}[caller={expectedMethod.ToDisplayString()}]", st);
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
            Assert.Equal($"{nameof(TestSpanFormattable)}.{nameof(TestSpanFormattable.TryFormat)}[caller={expectedMethod.ToDisplayString()}]", st);
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
            Assert.Equal($"{nameof(TestUtf8SpanFormattable)}.{nameof(TestUtf8SpanFormattable.TryFormat)}[caller={expectedMethod.ToDisplayString()}]", st);
        }
    }
}


file static class TestHelpers
{
    extension(Type type)
    {
        public string ToDisplayString()
        {
            var sb = new StringBuilder();
            if (type.Namespace is { Length: > 0 } ns)
            {
                sb.Append(ns);
                sb.Append('.');
            }
            type.ToDisplayStringCore(sb);
            return sb.ToString();
        }

        private void ToDisplayStringCore(StringBuilder sb)
        {
            if (type.DeclaringType is { } declaringType)
            {
                declaringType.ToDisplayStringCore(sb);
                sb.Append('+');
            }
            sb.Append(type.Name);
            if (type.IsGenericType)
            {
                sb.Append('<');
                sb.Append(string.Join(", ", type.GetGenericArguments().Select(static ga => ga.ToDisplayString())));
                sb.Append('>');
            }
        }
    }

    extension(MethodBase method)
    {
        public string ToDisplayString()
        {
            var sb = new StringBuilder();
            if (method is MethodInfo mi)
            {
                sb.Append($"{mi.ReturnType.ToDisplayString()} ");
            }
            sb.Append($"{method.DeclaringType?.ToDisplayString()}.{method.Name}");
            if (method.GetGenericArguments() is { Length: > 0 } genericArgs)
            {
                sb.Append('<');
                sb.Append(string.Join(", ", genericArgs.Select(static ga => ga.ToDisplayString())));
                sb.Append('>');
            }
            sb.Append('(');
            sb.Append(string.Join(", ", method.GetParameters().Select(static p => $"{p.ParameterType.ToDisplayString()} {p.Name}")));
            sb.Append(')');
            return sb.ToString();
        }
    }
}


file abstract class TestBase
{
    protected static string GetResolvedAppendFormatted()
    {
        var trace = new StackTrace(0);
        var lastCaller = default(MethodBase);
        for (var i = 0; trace.GetFrame(i) is { } frame; ++i)
        {
            var method = frame.GetMethod();
            if (method?.DeclaringType == typeof(RunaUtf8InterpolationHandler))
            {
                lastCaller = method;
            }
        }
        var caller = lastCaller?.ToDisplayString();
        return $"[caller={caller}]";
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
