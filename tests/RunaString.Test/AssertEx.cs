namespace RunaString.Test;

internal static class AssertEx
{
    public static void Equal<T>(T expected, T actual)
        where T : IEquatable<T>, allows ref struct
    {
        Assert.True(actual.Equals(expected));
    }
}
