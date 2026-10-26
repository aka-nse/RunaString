using System.Collections;
using System.Globalization;
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
    public readonly string[]? Capturenames = captureNames;
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
}
