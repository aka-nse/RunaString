using System.Collections.Immutable;
using System.Data;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RunaString.InternalGenerators.Regex;

using static Constant;

[Generator(LanguageNames.CSharp)]
public class RegexCodeHashAnalyzerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource("RegexCodeHashAnalyzer.g.cs", """
            using System;

            namespace RunaString.RegularExpressions;

            [AttributeUsage(
                AttributeTargets.Class
                | AttributeTargets.Struct
                | AttributeTargets.Interface
                | AttributeTargets.Enum,
                Inherited = false, AllowMultiple = false)]
            internal sealed class RegexCodeHashAnalyzerAttribute(string referenceSourceFile, string sha256) : Attribute
            {
                public string ReferenceSourceFile { get; } = referenceSourceFile;
                public string Sha256 { get; } = sha256;
            }
            """);
        });
    }
}


[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RegexCodeHashAnalyzer : DiagnosticAnalyzer
{
    private const string _targetAttribute = "RunaString.RegularExpressions.RegexCodeHashAnalyzerAttribute";
    private const string _diagnosticId1 = "RSRG001";
    private static readonly DiagnosticDescriptor _rule1 = new(
        _diagnosticId1,
        "Invalid attribute usage",
        "Invalid attribute usage",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    private const string _diagnosticId2 = "RSRG002";
    private static readonly DiagnosticDescriptor _rule2 = new(
        _diagnosticId2,
        "Uppdation is detected",
        "dotnet runtime repository has several updates",
        "Versions",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [_rule1, _rule2];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }


    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;

        bool hasAttribute = symbol
            .GetAttributes()
            .Any(static attr => attr.AttributeClass?.ToDisplayString() == _targetAttribute);

        if (!hasAttribute)
        {
            return;
        }

        var referenceSourceFile = symbol
            .GetAttributes()
            .First(static attr => attr.AttributeClass?.ToDisplayString() == _targetAttribute)
            .ConstructorArguments[0]
            .Value as string;
        var expectedSha256 = (symbol
            .GetAttributes()
            .First(static attr => attr.AttributeClass?.ToDisplayString() == _targetAttribute)
            .ConstructorArguments[1]
            .Value as string)
            ?.AsHexString();

        using var referenceSourceStream = typeof(RegexCodeHashAnalyzer).Assembly
            .GetManifestResourceStream($"{RegexResourcePrefix}{referenceSourceFile?.Replace(['/', '\\'], '.')}");
        if(referenceSourceStream is null)
        {
            ReportDiagnostic(context, symbol, _rule1);
            return;
        }
        var actualSha256 = SHA256.Create().ComputeHash(referenceSourceStream);

        if(!actualSha256.AsSpan().SequenceEqual(expectedSha256))
        {
            ReportDiagnostic(context, symbol, _rule2);
            return;
        }
    }


    private static void ReportDiagnostic(SymbolAnalysisContext context, INamedTypeSymbol symbol, DiagnosticDescriptor rule)
    {
        var diagnostic = Diagnostic.Create(rule, symbol.Locations[0], symbol.Name);
        context.ReportDiagnostic(diagnostic);
    }
}
