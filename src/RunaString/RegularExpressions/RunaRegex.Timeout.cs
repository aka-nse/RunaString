namespace RunaString.RegularExpressions;

partial class RunaRegex
{
    private static readonly TimeSpan _defaultMatchTimeout = Helpers.InitDefaultMatchTimeout();

    internal static void ValidateMatchTimeout(TimeSpan matchTimeout)
    {
        long matchTimeoutTicks = matchTimeout.Ticks;
        if (matchTimeoutTicks != Timeout.InfiniteTimeSpan.Ticks && ((ulong)(matchTimeoutTicks - 1) >= Helpers.MaximumMatchTimeoutTicks))
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.matchTimeout);
        }
    }
}


[RegexCodeHashAnalyzer(
    "System/Text/RegularExpressions/Regex.Timeout.cs",
    "addafe45e3335a5920ddfc8a9f65923d717ad9896c52cd1cdef19f3565f92d61")]
file static class Helpers
{
    private const string _defaultMatchTimeout_ConfigKeyName = "REGEX_DEFAULT_MATCH_TIMEOUT";

    public static ulong MaximumMatchTimeoutTicks => (ulong)TimeSpan.FromMilliseconds(int.MaxValue - 1).Ticks;

    internal static TimeSpan InitDefaultMatchTimeout()
    {
        object? defaultTimeout = AppContext.GetData(_defaultMatchTimeout_ConfigKeyName);

        if (defaultTimeout is not TimeSpan defaultMatchTimeOut)
        {
            if (defaultTimeout is null)
            {
                return Timeout.InfiniteTimeSpan;
            }
            throw new InvalidCastException();
        }

        try
        {
            RunaRegex.ValidateMatchTimeout(defaultMatchTimeOut);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentOutOfRangeException();
        }

        return defaultMatchTimeOut;
    }
}