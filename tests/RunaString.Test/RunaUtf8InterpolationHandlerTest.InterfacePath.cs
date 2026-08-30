using System.Reflection;
using System.Text;

namespace RunaString.Test;

public partial class RunaUtf8InterpolationHandlerTest
{
    public class InterfacePath
    {
        [Fact]
        public void AppendLiteral()
        {
            var handler = new RunaUtf8InterpolationHandler(256, 0);
            handler.AppendLiteral("Hello, world!");
            var st = handler.MoveToUtf8String().ToString();
            Assert.Equal("Hello, world!", st);
        }

        [Fact]
        public void AppendFormatted_Common()
        {
            var handler = new RunaUtf8InterpolationHandler(0, 1);
            TestCommon value = new("Hello, world!");
            handler.AppendFormatted(value);
            var st = handler.MoveToUtf8String().ToString();
            Assert.Equal("Hello, world!", st);
            Assert.Equal(TestCommon._ToString, value.LastCalledMember);
        }

        [Fact]
        public void AppendFormatted_Formattable()
        {
            var handler = new RunaUtf8InterpolationHandler(0, 1);
            TestFormattable value = new("Hello, world!");
            handler.AppendFormatted(value);
            var st = handler.MoveToUtf8String().ToString();
            Assert.Equal("Hello, world!", st);
            Assert.Equal(TestFormattable._ToString, value.LastCalledMember);
        }

        [Fact]
        public void AppendFormatted_SpanFormattable_StackBuffer()
        {
            var handler = new RunaUtf8InterpolationHandler(0, 1);
            TestSpanFormattable value = new("Hello, world!", RunaUtf8InterpolationHandler.StackBufferSize / sizeof(char) - 1);
            handler.AppendFormatted(value);
            var st = handler.MoveToUtf8String().ToString();
            Assert.Equal("Hello, world!", st);
            Assert.Equal(TestSpanFormattable._TryFormat, value.LastCalledMember);
        }

        [Fact]
        public void AppendFormatted_SpanFormattable_HeapBuffer()
        {
            var handler = new RunaUtf8InterpolationHandler(0, 1);
            TestSpanFormattable value = new("Hello, world!", RunaUtf8InterpolationHandler.StackBufferSize / sizeof(char) + 1);
            handler.AppendFormatted(value);
            var st = handler.MoveToUtf8String().ToString();
            Assert.Equal("Hello, world!", st);
            Assert.Equal(TestSpanFormattable._TryFormat, value.LastCalledMember);
        }

        [Fact]
        public void AppendFormatted_SpanFormattable_Fallback()
        {
            var handler = new RunaUtf8InterpolationHandler(0, 1);
            TestSpanFormattable value = new("Hello, world!", RunaUtf8InterpolationHandler.HeapBufferSize / sizeof(char) + 1);
            handler.AppendFormatted(value);
            var st = handler.MoveToUtf8String().ToString();
            Assert.Equal("Hello, world!", st);
            Assert.Equal(TestSpanFormattable._ToString, value.LastCalledMember);
        }

        [Fact]
        public void AppendFormatted_Utf8SpanFormattable_StackBuffer()
        {
            var handler = new RunaUtf8InterpolationHandler(0, 1);
            TestUtf8SpanFormattable value = new("Hello, world!", RunaUtf8InterpolationHandler.StackBufferSize - 1);
            handler.AppendFormatted(value);
            var st = handler.MoveToUtf8String().ToString();
            Assert.Equal("Hello, world!", st);
            Assert.Equal(TestUtf8SpanFormattable._TryFormat, value.LastCalledMember);
        }

        [Fact]
        public void AppendFormatted_Utf8SpanFormattable_HeapBuffer()
        {
            var handler = new RunaUtf8InterpolationHandler(0, 1);
            TestUtf8SpanFormattable value = new("Hello, world!", RunaUtf8InterpolationHandler.StackBufferSize + 1);
            handler.AppendFormatted(value);
            var st = handler.MoveToUtf8String().ToString();
            Assert.Equal("Hello, world!", st);
            Assert.Equal(TestUtf8SpanFormattable._TryFormat, value.LastCalledMember);
        }
    }
}


file class TestCommon(string formatResult)
{
    public MethodInfo? LastCalledMember { get; private set; }

    public static readonly MethodInfo _ToString =
        typeof(TestCommon).GetMethod(nameof(ToString), Type.EmptyTypes)!;
    public override string ToString()
    {
        LastCalledMember = _ToString;
        return formatResult;
    }
}

file class TestFormattable(string formatResult)
    : IFormattable
{
    public MethodInfo? LastCalledMember { get; private set; }

    public static readonly MethodInfo _ToString =
        typeof(TestFormattable).GetMethod(nameof(ToString), [typeof(string), typeof(IFormatProvider)])!;
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        LastCalledMember = _ToString;
        return formatResult;
    }
}

file class TestSpanFormattable(string formatResult, int spanFormatRequirement)
    : ISpanFormattable
{
    public MethodInfo? LastCalledMember { get; private set; }

    public static readonly MethodInfo _TryFormat =
        typeof(TestSpanFormattable).GetMethod(nameof(TryFormat), [typeof(Span<char>), typeof(int).MakeByRefType(), typeof(ReadOnlySpan<char>), typeof(IFormatProvider)])!;
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
    {
        LastCalledMember = _TryFormat;
        if (spanFormatRequirement > destination.Length)
        {
            charsWritten = 0;
            return false;
        }
        formatResult.AsSpan().CopyTo(destination);
        charsWritten = formatResult.Length;
        return true;
    }

    public static readonly MethodInfo _ToString =
        typeof(TestSpanFormattable).GetMethod(nameof(ToString), [typeof(string), typeof(IFormatProvider)])!;
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        LastCalledMember = _ToString;
        return formatResult;
    }
}

file class TestUtf8SpanFormattable(string formatResult, int spanFormatRequirement)
    : IFormattable, IUtf8SpanFormattable
{
    public MethodInfo? LastCalledMember { get; private set; }

    public static readonly MethodInfo _TryFormat =
        typeof(TestUtf8SpanFormattable).GetMethod(nameof(TryFormat), [typeof(Span<byte>), typeof(int).MakeByRefType(), typeof(ReadOnlySpan<char>), typeof(IFormatProvider)])!;
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        LastCalledMember = _TryFormat;
        if (spanFormatRequirement > utf8Destination.Length)
        {
            bytesWritten = 0;
            return false;
        }
        bytesWritten = Encoding.UTF8.GetBytes(formatResult.AsSpan(), utf8Destination);
        return true;
    }

    public static readonly MethodInfo _ToString =
        typeof(TestUtf8SpanFormattable).GetMethod(nameof(ToString), [typeof(string), typeof(IFormatProvider)])!;
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        LastCalledMember = _ToString;
        return formatResult;
    }
}


file class TestUtf8SpanFormattableWithoutIFormattable(string formatResult, int spanFormatRequirement)
    : IUtf8SpanFormattable
{
    public MethodInfo? LastCalledMember { get; private set; }

    public static readonly MethodInfo _TryFormat =
        typeof(TestUtf8SpanFormattableWithoutIFormattable).GetMethod(nameof(TryFormat), [typeof(Span<byte>), typeof(int).MakeByRefType(), typeof(ReadOnlySpan<char>), typeof(IFormatProvider)])!;
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        LastCalledMember = _TryFormat;
        if (spanFormatRequirement > utf8Destination.Length)
        {
            bytesWritten = 0;
            return false;
        }
        bytesWritten = Encoding.UTF8.GetBytes(formatResult.AsSpan(), utf8Destination);
        return true;
    }

    public static readonly MethodInfo _ToString =
        typeof(TestUtf8SpanFormattableWithoutIFormattable).GetMethod(nameof(ToString), [])!;
    public override string ToString()
    {
        LastCalledMember = _ToString;
        return formatResult;
    }
}
