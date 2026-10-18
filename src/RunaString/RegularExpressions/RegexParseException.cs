using System.Text.RegularExpressions;

namespace RunaString.RegularExpressions;

/// <summary></summary>
[RegexCodeHashAnalyzer(
    "Text/RegularExpressions/RegexParseException.cs",
    "e65d012583cef731895c1653d46672f1e4559a4dcda774495508ac624aa23e45")]
public sealed class RegexParseException : ArgumentException
{
    /// <summary></summary>
    public RegexParseError Error { get; }

    /// <summary></summary>
    public int Offset { get; }

    internal RegexParseException(RegexParseError error, int offset, string message)
        : base(message)
    {
        Error = error;
        Offset = offset;
    }
}
