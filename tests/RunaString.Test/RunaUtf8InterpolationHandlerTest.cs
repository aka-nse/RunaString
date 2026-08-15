using System.Text;

namespace RunaString.Test;

public partial class RunaUtf8InterpolationHandlerTest
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
        handler.AppendFormatted(new TestCommon("Hello, world!"));
        var st = handler.MoveToUtf8String().ToString();
        Assert.Equal("ToString(): Hello, world!", st);
    }

    [Fact]
    public void AppendFormatted_Formattable()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        handler.AppendFormatted(new TestFormattable("Hello, world!"));
        var st = handler.MoveToUtf8String().ToString();
        Assert.Equal("ToString(string, IFormatProvider): Hello, world!", st);
    }

    [Fact]
    public void AppendFormatted_SpanFormattable_StackBuffer()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        handler.AppendFormatted(new TestSpanFormattable("Hello, world!", RunaUtf8InterpolationHandler.StackBufferSize / sizeof(char) - 1));
        var st = handler.MoveToUtf8String().ToString();
        Assert.Equal("TryFormat(Span<char>, out int, ReadOnlySpan<char>, IFormatProvider): Hello, world!", st);
    }

    [Fact]
    public void AppendFormatted_SpanFormattable_HeapBuffer()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        handler.AppendFormatted(new TestSpanFormattable("Hello, world!", RunaUtf8InterpolationHandler.StackBufferSize / sizeof(char) + 1));
        var st = handler.MoveToUtf8String().ToString();
        Assert.Equal("TryFormat(Span<char>, out int, ReadOnlySpan<char>, IFormatProvider): Hello, world!", st);
    }

    [Fact]
    public void AppendFormatted_SpanFormattable_Fallback()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        handler.AppendFormatted(new TestSpanFormattable("Hello, world!", RunaUtf8InterpolationHandler.HeapBufferSize / sizeof(char) + 1));
        var st = handler.MoveToUtf8String().ToString();
        Assert.Equal("ToString(string, IFormatProvider): Hello, world!", st);
    }

    [Fact]
    public void AppendFormatted_Utf8SpanFormattable_StackBuffer()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        handler.AppendFormatted(new TestUtf8SpanFormattable("Hello, world!", RunaUtf8InterpolationHandler.StackBufferSize - 1));
        var st = handler.MoveToUtf8String().ToString();
        Assert.Equal("TryFormat(Span<byte>, out int, ReadOnlySpan<char>, IFormatProvider): Hello, world!", st);
    }

    [Fact]
    public void AppendFormatted_Utf8SpanFormattable_HeapBuffer()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        handler.AppendFormatted(new TestUtf8SpanFormattable("Hello, world!", RunaUtf8InterpolationHandler.StackBufferSize + 1));
        var st = handler.MoveToUtf8String().ToString();
        Assert.Equal("TryFormat(Span<byte>, out int, ReadOnlySpan<char>, IFormatProvider): Hello, world!", st);
    }

    [Fact]
    public void AppendFormatted_Utf8SpanFormattable_FallbackToFormattable()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        handler.AppendFormatted(new TestUtf8SpanFormattable("Hello, world!", RunaUtf8InterpolationHandler.HeapBufferSize + 1));
        var st = handler.MoveToUtf8String().ToString();
        Assert.Equal("ToString(string, IFormatProvider): Hello, world!", st);
    }

    [Fact]
    public void AppendFormatted_Utf8SpanFormattable_FallbackToCommon()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        handler.AppendFormatted(new TestUtf8SpanFormattableWithoutIFormattable("Hello, world!", RunaUtf8InterpolationHandler.HeapBufferSize + 1));
        var st = handler.MoveToUtf8String().ToString();
        Assert.Equal("ToString(): Hello, world!", st);
    }
}


file class TestCommon(string formatResult)
{
    public override string ToString() =>
        $"ToString(): {formatResult}";
}

file class TestFormattable(string formatResult)
    : IFormattable
{
    public string ToString(string? format, IFormatProvider? formatProvider) =>
        $"ToString(string, IFormatProvider): {formatResult}";
}

file class TestSpanFormattable(string formatResult, int spanFormatRequirement)
    : ISpanFormattable
{
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? formatProvider)
    {
        if (spanFormatRequirement > destination.Length)
        {
            charsWritten = 0;
            return false;
        }
        var s = $"TryFormat(Span<char>, out int, ReadOnlySpan<char>, IFormatProvider): {formatResult}";
        s.AsSpan().CopyTo(destination);
        charsWritten = s.Length;
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        $"ToString(string, IFormatProvider): {formatResult}";
}

file class TestUtf8SpanFormattable(string formatResult, int spanFormatRequirement)
    : IFormattable, IUtf8SpanFormattable
{
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (spanFormatRequirement > utf8Destination.Length)
        {
            bytesWritten = 0;
            return false;
        }
        var s = $"TryFormat(Span<byte>, out int, ReadOnlySpan<char>, IFormatProvider): {formatResult}";
        bytesWritten = Encoding.UTF8.GetBytes(s.AsSpan(), utf8Destination);
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        $"ToString(string, IFormatProvider): {formatResult}";
}


file class TestUtf8SpanFormattableWithoutIFormattable(string formatResult, int spanFormatRequirement)
    : IUtf8SpanFormattable
{
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (spanFormatRequirement > utf8Destination.Length)
        {
            bytesWritten = 0;
            return false;
        }
        var s = $"TryFormat(Span<byte>, out int, ReadOnlySpan<char>, IFormatProvider): {formatResult}";
        bytesWritten = Encoding.UTF8.GetBytes(s.AsSpan(), utf8Destination);
        return true;
    }

    public override string ToString() =>
        $"ToString(): {formatResult}";
}
