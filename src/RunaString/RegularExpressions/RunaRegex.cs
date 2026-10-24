using System.Globalization;
using System.Text.RegularExpressions;

namespace RunaString.RegularExpressions;

internal delegate bool MatchCallback<TState>(ref TState state, RunaMatch match);

/// <summary>
/// Represents an immutable regular expression. Also contains static methods that
/// allow use of regular expressions without instantiating a Regex explicitly.
/// </summary>
[RegexCodeHashAnalyzer(
    "System/Text/RegularExpressions/Regex.cs",
    "")]
public partial class RunaRegex
{
    #region place holders

    internal bool RightToLeft => throw new NotImplementedException();

    internal static string SegmentsToStringAndDispose(ref StructListBuilder<ReadOnlyMemory<char>> segments)
    {
        var span = segments.AsSpan();

        int length = 0;
        for (int i = 0; i < span.Length; i++)
        {
            length += span[i].Length;
        }

        string result = string.Create(length, span, static (dest, span) =>
        {
            for (int i = 0; i < span.Length; i++)
            {
                ReadOnlySpan<char> segment = span[i].Span;
                segment.CopyTo(dest);
                dest = dest.Slice(segment.Length);
            }
        });

        segments.Dispose();

        return result;
    }

    internal void RunAllMatchesWithCallback<TState>(string? input, int startat, ref TState state, MatchCallback<TState> callback, RegexRunnerMode mode, bool reuseMatchObject) =>
            RunAllMatchesWithCallback(input, (ReadOnlySpan<char>)input, startat, ref state, callback, mode, reuseMatchObject);
    
    internal void RunAllMatchesWithCallback<TState>(ReadOnlySpan<char> input, int startat, ref TState state, MatchCallback<TState> callback, RegexRunnerMode mode, bool reuseMatchObject) =>
        RunAllMatchesWithCallback(inputString: null, input, startat, ref state, callback, mode, reuseMatchObject);

    private void RunAllMatchesWithCallback<TState>(string? inputString, ReadOnlySpan<char> inputSpan, int startat, ref TState state, MatchCallback<TState> callback, RegexRunnerMode mode, bool reuseMatchObject)
    {
        throw new NotImplementedException();
    }

    #endregion

    private readonly string _pattern;
    private readonly RegexOptions _options;
    private readonly TimeSpan _matchTimeout;
    private readonly CultureInfo _culture;
    private readonly IRegexRunnerFactory _factory;

    public string Pattern => _pattern;

    /// <inheritdoc cref="RunaRegex(string, RegexOptions, TimeSpan, CultureInfo?)" />
    public RunaRegex(string pattern)
        : this(pattern, RegexOptions.None, _defaultMatchTimeout, null)
    {
    }



    /// <inheritdoc cref="RunaRegex(string, RegexOptions, TimeSpan, CultureInfo?)" />
    public RunaRegex(string pattern, RegexOptions options)
        : this(pattern, options, _defaultMatchTimeout, null)
    {
    }



    /// <inheritdoc cref="RunaRegex(string, RegexOptions, TimeSpan, CultureInfo?)" />
    public RunaRegex(string pattern, RegexOptions options, TimeSpan matchTimeout)
        : this(pattern, options, matchTimeout, null)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Regex" /> class for the specified regular expression.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="options"></param>
    /// <param name="matchTimeout"></param>
    /// <param name="culture"></param>
    public RunaRegex(string pattern, RegexOptions options, TimeSpan matchTimeout, CultureInfo? culture)
    {
        _pattern = pattern;
        _options = options;
        _matchTimeout = matchTimeout;
        _culture = culture ?? ((options & RegexOptions.CultureInvariant) != 0 ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture);

        var tree = RegexParser.Parse(_pattern, _options, _culture);
        _factory = new RegexInterpreterFactory(tree);
    }
}
