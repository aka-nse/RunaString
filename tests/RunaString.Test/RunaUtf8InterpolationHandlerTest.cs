namespace RunaString.Test;

using System.Globalization;
using static FileHelpers;

public partial class RunaUtf8InterpolationHandlerTest
{
    public static TheoryData<string, Utf8String> MemoryFormatTestCases()
    {
        var data = new TheoryData<string, Utf8String>();

        void addTestCase(string expected, RunaUtf8InterpolationHandler actual)
        {
            data.Add(expected, actual.MoveToUtf8String());
        }

        addTestCase(
            $"Hello, world!",
            $"Hello, world!");
        addTestCase(
            $"{123}",
            $"{123}");
        addTestCase(
            $"{1.234}",
            $"{1.234}");
        addTestCase(
            $"{1234567:X08}",
            $"{1234567:X08}");
        addTestCase(
            $"{1.234,-10}",
            $"{1.234,-10}");
        addTestCase(
            $"{1.234,10}",
            $"{1.234,10}");
        addTestCase(
            $"{1.234567,3}",
            $"{1.234567,3}");
        addTestCase(
            $"{1.234567,-3}",
            $"{1.234567,-3}");
        addTestCase(
            $"{1234567,16:X08}",
            $"{1234567,16:X08}");
        addTestCase(
            $"{1234567,-16:X08}",
            $"{1234567,-16:X08}");
        addTestCase(
            $"{1234567,2:X08}",
            $"{1234567,2:X08}");
        addTestCase(
            $"{1234567,-2:X08}",
            $"{1234567,-2:X08}");
        addTestCase(
            $"Hello, {1234567,16:X08}!",
            $"Hello, {1234567,16:X08}!");
        addTestCase(
            $"Hello, {1234567,-16:X08}!",
            $"Hello, {1234567,-16:X08}!");
        addTestCase(
            $"Hello, {1234567,2:X08}!",
            $"Hello, {1234567,2:X08}!");
        addTestCase(
            $"Hello, {1234567,-2:X08}!",
            $"Hello, {1234567,-2:X08}!");
        addTestCase(
            $"Hello, {Wrap(1234567),16:X08}!",
            $"Hello, {Wrap(1234567),16:X08}!");
        addTestCase(
            $"Hello, {Wrap(1234567),-16:X08}!",
            $"Hello, {Wrap(1234567),-16:X08}!");
        addTestCase(
            $"Hello, {Wrap(1234567),2:X08}!",
            $"Hello, {Wrap(1234567),2:X08}!");
        addTestCase(
            $"Hello, {Wrap(1234567),-2:X08}!",
            $"Hello, {Wrap(1234567),-2:X08}!");

        return data;
    }

    public static TheoryData<string, Utf8String> MemoryFormatWithProviderTestCases()
    {
        var data = new TheoryData<string, Utf8String>();
        var provider = CultureInfo.GetCultureInfo("fr-FR");

        void addTestCase(string expected, Utf8String actual)
        {
            data.Add(expected, actual);
        }

        addTestCase(
            string.Create(provider, $"Hello, world!"),
            Utf8String.FromFormat(provider, $"Hello, world!"));
        addTestCase(
            string.Create(provider, $"{123}"),
            Utf8String.FromFormat($"{123}"));
        addTestCase(
            string.Create(provider, $"{1.234}"),
            Utf8String.FromFormat(provider, $"{1.234}"));
        addTestCase(
            string.Create(provider, $"{1234567:X08}"),
            Utf8String.FromFormat(provider, $"{1234567:X08}"));
        addTestCase(
            string.Create(provider, $"{1.234,-10}"),
            Utf8String.FromFormat(provider, $"{1.234,-10}"));
        addTestCase(
            string.Create(provider, $"{1.234,10}"),
            Utf8String.FromFormat(provider, $"{1.234,10}"));
        addTestCase(
            string.Create(provider, $"{1.234567,3}"),
            Utf8String.FromFormat(provider, $"{1.234567,3}"));
        addTestCase(
            string.Create(provider, $"{1.234567,-3}"),
            Utf8String.FromFormat(provider, $"{1.234567,-3}"));
        addTestCase(
            string.Create(provider, $"{1234567,16:X08}"),
            Utf8String.FromFormat(provider, $"{1234567,16:X08}"));
        addTestCase(
            string.Create(provider, $"{1234567,-16:X08}"),
            Utf8String.FromFormat(provider, $"{1234567,-16:X08}"));
        addTestCase(
            string.Create(provider, $"{1234567,2:X08}"),
            Utf8String.FromFormat(provider, $"{1234567,2:X08}"));
        addTestCase(
            string.Create(provider, $"{1234567,-2:X08}"),
            Utf8String.FromFormat(provider, $"{1234567,-2:X08}"));
        addTestCase(
            string.Create(provider, $"Hello, {1234567,16:X08}!"),
            Utf8String.FromFormat(provider, $"Hello, {1234567,16:X08}!"));
        addTestCase(
            string.Create(provider, $"Hello, {1234567,-16:X08}!"),
            Utf8String.FromFormat(provider, $"Hello, {1234567,-16:X08}!"));
        addTestCase(
            string.Create(provider, $"Hello, {1234567,2:X08}!"),
            Utf8String.FromFormat(provider, $"Hello, {1234567,2:X08}!"));
        addTestCase(
            string.Create(provider, $"Hello, {1234567,-2:X08}!"),
            Utf8String.FromFormat(provider, $"Hello, {1234567,-2:X08}!"));
        addTestCase(
            string.Create(provider, $"Hello, {Wrap(1234567),16:X08}!"),
            Utf8String.FromFormat(provider, $"Hello, {Wrap(1234567),16:X08}!"));
        addTestCase(
            string.Create(provider, $"Hello, {Wrap(1234567),-16:X08}!"),
            Utf8String.FromFormat(provider, $"Hello, {Wrap(1234567),-16:X08}!"));
        addTestCase(
            string.Create(provider, $"Hello, {Wrap(1234567),2:X08}!"),
            Utf8String.FromFormat(provider, $"Hello, {Wrap(1234567),2:X08}!"));
        addTestCase(
            string.Create(provider, $"Hello, {Wrap(1234567),-2:X08}!"),
            Utf8String.FromFormat(provider, $"Hello, {Wrap(1234567),-2:X08}!"));

        return data;
    }




    [Theory]
    [MemberData(nameof(MemoryFormatTestCases))]
    [MemberData(nameof(MemoryFormatWithProviderTestCases))]
    public void Format(string expected, Utf8String actual)
    {
        Assert.Equal(expected, actual.ToString());
    }
}


file static class FileHelpers
{
    public static Wrapper<T> Wrap<T>(T value) where T : IFormattable => new(value);
}


file record struct Wrapper<T>(T Value) : IFormattable
    where T : IFormattable
{
    public readonly override string? ToString() => Value.ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) =>
        Value.ToString(format, formatProvider);
}
