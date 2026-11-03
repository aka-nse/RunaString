using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RunaString.RegularExpressions;

[RegexCodeHashAnalyzer(
    "System/Text/RegularExpressions/RegexTree.cs",
    "bd5a9983744a4a34af62d7e3a7c5aefb2deba78e2e7badc2d32074eedf50764e")]
internal class RegexTree(
    RegexNode root,
    int captureCount,
    string[]? captureNames,
    Dictionary<string, int>? captureNameToNumberMapping,
    Dictionary<int, int>? captureNumberSparseMapping,
    RegexOptions options,
    CultureInfo? culture)
{
    public readonly RegexOptions Options = options;
    public readonly RegexNode Root = root;
    public readonly object? FindOptimizations = null;  // Placeholder for automatic ingestion via source generators
    public readonly int CaptureCount = captureCount;
    public readonly CultureInfo? Culture = culture;
    public readonly string[]? CaptureNames = captureNames;
    public readonly Dictionary<string, int>? CaptureNameToNumberMapping = captureNameToNumberMapping;
    public readonly Dictionary<int, int>? CaptureNumberSparseMapping = captureNumberSparseMapping;

    internal RegexTree(
        RegexNode root,
        int captureCount,
        string[]? captureNames,
        Hashtable? captureNameToNumberMapping,
        Hashtable? captureNumberSparseMapping,
        RegexOptions options,
        CultureInfo? culture)
        : this(
            root,
            captureCount,
            captureNames,
            captureNameToNumberMapping?.Cast<DictionaryEntry>().ToDictionary(entry => (string)entry.Key, entry => (int)entry.Value!),
            captureNumberSparseMapping?.Cast<DictionaryEntry>().ToDictionary(entry => (int)entry.Key, entry => (int)entry.Value!),
            options,
            culture
            )
    {
    }


    public override string ToString()
    {
        var sb = new StringBuilder();
        ToString(Root, sb, "");
        return sb.ToString();
    }


    private static void ToString(RegexNode node, StringBuilder sb, string indent)
    {
        sb.Append(indent);
        sb.Append(node.Kind);
        switch (node.Kind)
        {
        case RegexNodeKind.Set:
            sb.Append(' ');
            sb.Append(node.Str!);
            break;
        case RegexNodeKind.Multi:
            sb.Append(' ');
            sb.Append(node.Str!);
            break;
        case RegexNodeKind.One:
            sb.Append(' ');
            sb.Append(node.Ch);
            break;
        case RegexNodeKind.Notone:
            sb.Append(' ');
            sb.Append(node.Ch);
            break;
        case RegexNodeKind.Capture:
        case RegexNodeKind.Backreference:
            sb.Append(' ');
            sb.Append(node.M);
            break;
        default:
            break;
        }
        indent = indent + "  ";
        for(var i = 0; i < node.ChildCount(); i++)
        {
            sb.AppendLine();
            ToString(node.Child(i), sb, indent);
        }
    }
}
