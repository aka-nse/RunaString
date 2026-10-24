using System.Collections;

#pragma warning disable IDE0130
namespace RunaString.RegularExpressions;
#pragma warning restore IDE0130

[RegexCodeHashAnalyzer(
    "System/Collections/HashtableExtensions.cs",
    "85de5d381d32677ffa190a8a1c99210ac2caaac26c5fc70c57a0eb6d893c6e61")]
internal static class HashtableExtensions
{
    extension(Hashtable table)
    {
        public bool TryGetValue(object key, out int value)
        {
            if (table[key] is { } obj)
            {
                value = (int)obj;
                return true;
            }
            value = default;
            return false;
        }
    }
}
