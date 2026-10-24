namespace RunaString.RegularExpressions;

[RegexSourceMigration(
    "System/Text/RegularExpressions/RegexCharClass.cs",
    ["System", "System.Text", "System.Text.RegularExpressions"])]
[RegexSourceMigration(
    "System/Text/RegularExpressions/RegexCharClass.Tables.cs",
    ["System", "System.Text", "System.Text.RegularExpressions"])]
internal partial class RegexCharClass
{
}
