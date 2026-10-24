using System.Diagnostics;
using System.Text;

namespace RunaString.RegularExpressions;

[RegexSourceMigration(
    "System/Text/RegularExpressions/RegexFindOptimizations.cs",
    ["System", "System.Text.RegularExpressions"],
    ["SYSTEM_TEXT_REGULAREXPRESSIONS"])]
internal partial class RegexFindOptimizations
{
    public bool TryFindNextStartingPositionRightToLeft<TString, TIndex>(TString text, ref TIndex pos, TIndex start)
        where TString : IRunaString<TString, TIndex>, allows ref struct
        where TIndex : struct, IRunaIndex<TIndex>
    {
        if(pos.RuneIndex < MinRequiredLength)
        {
            pos = default;
            return false;
        }

        switch (FindMode)
        {
        // There's an anchor.  For some, we can simply compare against the current position.
        // For others, we can jump to the relevant location.

        case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_Beginning:
            if (pos != default)
            {
                pos = default;
            }
            return true;

        case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_Start:
            if (pos != start)
            {
                pos = default;
                return false;
            }
            return true;

        case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_EndZ:
            if (pos < text.Length - 1 || ((uint)pos < (uint)text.Length && text[pos] != (Rune)'\n'))
            {
                pos = default;
                return false;
            }
            return true;

        case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_End:
            if (pos < text.Length)
            {
                // If we're not currently at the end, we'll never be (we're iterating from end to beginning),
                // so fail immediately.
                pos = 0;
                return false;
            }
            return true;

        // There's a case-sensitive prefix.  Search for it with ordinal IndexOf.

        case FindNextStartingPositionMode.LeadingString_RightToLeft:
            {
                int i = text.Slice(0, pos).LastIndexOf(LeadingPrefix.AsSpan());
                if (i >= 0)
                {
                    pos = i + LeadingPrefix.Length;
                    return true;
                }

                pos = 0;
                return false;
            }

        // There's a literal at the beginning of the pattern.  Search for it.

        case FindNextStartingPositionMode.LeadingChar_RightToLeft:
            {
                int i = text.Slice(0, pos).LastIndexOf(FixedDistanceLiteral.Char);
                if (i >= 0)
                {
                    pos = i + 1;
                    return true;
                }

                pos = 0;
                return false;
            }

        // There's a set at the beginning of the pattern.  Search for it.

        case FindNextStartingPositionMode.LeadingSet_RightToLeft:
            {
                ref uint[]? startingAsciiLookup = ref _asciiLookups![0];
                string set = FixedDistanceSets![0].Set;

                ReadOnlySpan<char> span = text.Slice(0, pos);
                for (int i = span.Length - 1; i >= 0; i--)
                {
                    if (RegexCharClass.CharInClass(span[i], set, ref startingAsciiLookup))
                    {
                        pos = i + 1;
                        return true;
                    }
                }

                pos = 0;
                return false;
            }

        // Nothing special to look for.  Just return true indicating this is a valid position to try to match.

        default:
            Debug.Assert(FindMode == FindNextStartingPositionMode.NoSearch);
            return true;
        }
    }
}