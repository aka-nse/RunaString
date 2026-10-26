using System.Text.RegularExpressions;

namespace RunaString.RegularExpressions;

[RegexCodeHashAnalyzer(
    "System/Text/RegularExpressions/RegexInterpreterCode.cs",
    "b82513811267fdbfe43c2f91c89199f3d603098268f22fb4867f6d8703bc10f9")]
internal sealed class RegexInterpreterCode(
#pragma warning disable CS9113
    object? placeholder,
#pragma warning restore CS9113
    RegexOptions options, int[] codes, string[] strings, int trackcount)
{
    public readonly RegexOptions Options = options;
    public readonly int[] Codes = codes;
    public readonly string[] Strings = strings;
    public readonly uint[]?[] StringsAsciiLookup = new uint[strings.Length][];
    public readonly int TrackCount = trackcount;

    public static bool OpcodeBacktracks(RegexOpcode opcode)
    {
        opcode &= RegexOpcode.OperatorMask;
        return opcode is
            RegexOpcode.Oneloop or
            RegexOpcode.Onelazy or
            RegexOpcode.Notoneloop or
            RegexOpcode.Notonelazy or
            RegexOpcode.Setloop or
            RegexOpcode.Setlazy or
            RegexOpcode.Lazybranch or
            RegexOpcode.Branchmark or
            RegexOpcode.Lazybranchmark or
            RegexOpcode.Nullcount or
            RegexOpcode.Setcount or
            RegexOpcode.Branchcount or
            RegexOpcode.Lazybranchcount or
            RegexOpcode.Setmark or
            RegexOpcode.Capturemark or
            RegexOpcode.Getmark or
            RegexOpcode.Setjump or
            RegexOpcode.Backjump or
            RegexOpcode.Forejump or
            RegexOpcode.Goto;
    }
}
