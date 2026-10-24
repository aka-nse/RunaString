namespace RunaString.RegularExpressions;

[RegexSourceMigration(
    "System/Text/RegularExpressions/RegexReplacement.cs",
    [
        "System", "System.Text", "System.Text.RegularExpressions",
        "Regex = RunaString.RegularExpressions.RunaRegex",
        "Match = RunaString.RegularExpressions.RunaMatch",
    ])]
internal sealed partial class RegexReplacement
{
}
