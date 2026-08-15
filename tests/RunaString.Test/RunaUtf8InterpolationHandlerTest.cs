namespace RunaString.Test;

public partial class RunaUtf8InterpolationHandlerTest
{
    public static TheoryData<string, Utf8String> FormatTestCases()
    {
        var data = new TheoryData<string, Utf8String>();

        static Wrapper<T> wrap<T>(T value) where T : IFormattable => new(value);

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
            $"Hello, {wrap(1234567),16:X08}!",
            $"Hello, {wrap(1234567),16:X08}!");
        addTestCase(
            $"Hello, {wrap(1234567),-16:X08}!",
            $"Hello, {wrap(1234567),-16:X08}!");
        addTestCase(
            $"Hello, {wrap(1234567),2:X08}!",
            $"Hello, {wrap(1234567),2:X08}!");
        addTestCase(
            $"Hello, {wrap(1234567),-2:X08}!",
            $"Hello, {wrap(1234567),-2:X08}!");

        return data;
    }


    [Theory]
    [MemberData(nameof(FormatTestCases))]
    public void Format(string expected, Utf8String actual)
    {
        Assert.Equal(expected, actual.ToString());
    }
}


file record struct Wrapper<T>(T Value) : IFormattable
    where T : IFormattable
{
    public readonly override string? ToString() => Value.ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) =>
        Value.ToString(format, formatProvider);
}
