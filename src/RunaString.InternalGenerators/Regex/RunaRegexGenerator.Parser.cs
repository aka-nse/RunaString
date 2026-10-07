using Microsoft.CodeAnalysis;

namespace RunaString.InternalGenerators.Regex;

using static Constant;

partial class RunaRegexGenerator
{
    static void InitializeParserCodeOutput(IncrementalGeneratorInitializationContext context)
    {
        var source_RegexCharClass = GetReferenceSource($"{ProjectName}.Regex.resources.Text.RegularExpressions.RegexCharClass.cs");
    }
}
