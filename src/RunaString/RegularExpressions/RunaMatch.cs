namespace RunaString.RegularExpressions;

/// <summary>Represents the results from a single regular expression match.</summary>
[RegexCodeHashAnalyzer(
    "System/Text/RegularExpressions/Match.cs",
    "")]
public partial class RunaMatch
{
    internal readonly int[] _matchcount;

    public string Text => throw new NotImplementedException();
    public int Index => throw new NotImplementedException();
    public int Length => throw new NotImplementedException();

    internal ReadOnlyMemory<char> LastGroupToStringImpl() =>
            GroupToStringImpl(_matchcount.Length - 1);

    internal ReadOnlyMemory<char> GroupToStringImpl(int groupnum)
    {
        throw new NotImplementedException();
    }

    internal ReadOnlyMemory<char> GetLeftSubstring()
    {
        throw new NotImplementedException();
    }

    internal ReadOnlyMemory<char> GetRightSubstring()
    {
        throw new NotImplementedException();
    }
}
