namespace RunaString.RegularExpressions;


[RegexSourceMigration(
    "System/Text/RegularExpressions/RegexParser.cs",
    ["System", "System.Text", "System.Text.RegularExpressions"])]
internal ref partial struct RegexParser
{
    internal static int MapCaptureNumber(int capnum, Dictionary<int, int>? caps)
    {
        if(capnum == -1)
        {
            return -1;
        }
        if(caps is null)
        {
            return capnum;
        }
        return caps[capnum];
    }
}
