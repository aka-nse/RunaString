using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RunaString.InternalGenerators.Regex;

[Generator(LanguageNames.CSharp)]
public partial class RunaRegexGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        InitializeParserCodeOutput(context);
    }


    private static CompilationUnitSyntax GetReferenceSource(string resourceName)
    {
        using var stream = typeof(RunaRegexGenerator)
            .Assembly
            .GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Resource '{resourceName}' not found.");
        using var sr = new StreamReader(stream);
        return CSharpSyntaxTree.ParseText(sr.ReadToEnd()).GetCompilationUnitRoot();
    }
}
