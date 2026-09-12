#nullable enable
namespace RunaString.Test;

using System.Globalization;
using System.Text;

public partial class RunaUtf8InterpolationHandlerTest
{
    [Fact]
    public void AppendLiteral()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        handler.AppendLiteral("Hello, world!");
        var st = handler.MoveToUtf8String().ToString();
        Assert.Equal("Hello, world!", st);
    }

    [Fact]
    public void AppendFormatted_Ascii_CommonType_NoAlignment()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            marker: default(OverloadResolutionMarker));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_CommonType_WithEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            20,
            marker: default(OverloadResolutionMarker));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("       Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_CommonType_WithEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            -20,
            marker: default(OverloadResolutionMarker));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!       ", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_CommonType_WithNotEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            5,
            marker: default(OverloadResolutionMarker));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_CommonType_WithNotEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            -5,
            marker: default(OverloadResolutionMarker));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_FormattableType_NoAlignment()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            marker: default(OverloadResolutionMarker.AssignableFrom<IFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_FormattableType_WithEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            20,
            marker: default(OverloadResolutionMarker.AssignableFrom<IFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("       Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_FormattableType_WithEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            -20,
            marker: default(OverloadResolutionMarker.AssignableFrom<IFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!       ", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_FormattableType_WithNotEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            5,
            marker: default(OverloadResolutionMarker.AssignableFrom<IFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_FormattableType_WithNotEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            -5,
            marker: default(OverloadResolutionMarker.AssignableFrom<IFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_SpanFormattableType_NoAlignment()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            marker: default(OverloadResolutionMarker.AssignableFrom<ISpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_SpanFormattableType_WithEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            20,
            marker: default(OverloadResolutionMarker.AssignableFrom<ISpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("       Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_SpanFormattableType_WithEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            -20,
            marker: default(OverloadResolutionMarker.AssignableFrom<ISpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!       ", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_SpanFormattableType_WithNotEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            5,
            marker: default(OverloadResolutionMarker.AssignableFrom<ISpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_SpanFormattableType_WithNotEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            -5,
            marker: default(OverloadResolutionMarker.AssignableFrom<ISpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_Utf8SpanFormattableType_NoAlignment()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            marker: default(OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_Utf8SpanFormattableType_WithEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            20,
            marker: default(OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("       Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_Utf8SpanFormattableType_WithEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            -20,
            marker: default(OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!       ", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_Utf8SpanFormattableType_WithNotEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            5,
            marker: default(OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_Ascii_Utf8SpanFormattableType_WithNotEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("Hello, world!");
        handler.AppendFormatted(
            value,
            -5,
            marker: default(OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("Hello, world!", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_CommonType_NoAlignment()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            marker: default(OverloadResolutionMarker));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_CommonType_WithEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            20,
            marker: default(OverloadResolutionMarker));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("           こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_CommonType_WithEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            -20,
            marker: default(OverloadResolutionMarker));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！           ", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_CommonType_WithNotEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            5,
            marker: default(OverloadResolutionMarker));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_CommonType_WithNotEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            -5,
            marker: default(OverloadResolutionMarker));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_FormattableType_NoAlignment()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            marker: default(OverloadResolutionMarker.AssignableFrom<IFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_FormattableType_WithEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            20,
            marker: default(OverloadResolutionMarker.AssignableFrom<IFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("           こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_FormattableType_WithEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            -20,
            marker: default(OverloadResolutionMarker.AssignableFrom<IFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！           ", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_FormattableType_WithNotEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            5,
            marker: default(OverloadResolutionMarker.AssignableFrom<IFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_FormattableType_WithNotEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            -5,
            marker: default(OverloadResolutionMarker.AssignableFrom<IFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_SpanFormattableType_NoAlignment()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            marker: default(OverloadResolutionMarker.AssignableFrom<ISpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_SpanFormattableType_WithEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            20,
            marker: default(OverloadResolutionMarker.AssignableFrom<ISpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("           こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_SpanFormattableType_WithEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            -20,
            marker: default(OverloadResolutionMarker.AssignableFrom<ISpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！           ", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_SpanFormattableType_WithNotEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            5,
            marker: default(OverloadResolutionMarker.AssignableFrom<ISpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_SpanFormattableType_WithNotEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            -5,
            marker: default(OverloadResolutionMarker.AssignableFrom<ISpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_Utf8SpanFormattableType_NoAlignment()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            marker: default(OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_Utf8SpanFormattableType_WithEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            20,
            marker: default(OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("           こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_Utf8SpanFormattableType_WithEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            -20,
            marker: default(OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！           ", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_Utf8SpanFormattableType_WithNotEnoughtAlignmentLeft()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            5,
            marker: default(OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


    [Fact]
    public void AppendFormatted_NonAscii_Utf8SpanFormattableType_WithNotEnoughtAlignmentRight()
    {
        var handler = new RunaUtf8InterpolationHandler(0, 1);
        var value = new FormatTestClass("こんにちは、世界！");
        handler.AppendFormatted(
            value,
            -5,
            marker: default(OverloadResolutionMarker.AssignableFrom<IUtf8SpanFormattable>));
        var st = handler.MoveToUtf8SpanString().ToString();
        Assert.Equal("こんにちは、世界！", st);
    }


}


file class FormatTestClass(string value)
    : IFormattable, ISpanFormattable, IUtf8SpanFormattable
{
    public override string ToString() => value;

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        value;

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if(destination.Length < value.Length)
        {
            charsWritten = 0;
            return false;
        }
        value.CopyTo(destination);
        charsWritten = value.Length;
        return true;
    }

    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) =>
        Encoding.UTF8.TryGetBytes(value, utf8Destination, out bytesWritten);
}

