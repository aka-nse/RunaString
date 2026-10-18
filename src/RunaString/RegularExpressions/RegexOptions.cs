#if !NET11_0_OR_GREATER
using System.Text.RegularExpressions;

namespace RunaString.RegularExpressions;

internal static class RegexOptionsExtensions
{
    extension(RegexOptions)
    {
        public static RegexOptions AnyNewLine =>
            (RegexOptions)0x0800;
    }
}
#endif
