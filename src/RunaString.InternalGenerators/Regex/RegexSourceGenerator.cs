using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RunaString.InternalGenerators.Regex;

[Generator(LanguageNames.CSharp)]
public partial class RegexSourceGenerator : IIncrementalGenerator
{
    public const string NamespaceName = "RunaString.RegularExpressions";
    public const string AttributeName = "RegexSourceMigrationAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource("RegexSourceMigrationAttribute.g.cs", $$"""
                using System;
                
                namespace {{NamespaceName}};
                
                [AttributeUsage(
                    AttributeTargets.Class
                    | AttributeTargets.Struct
                    | AttributeTargets.Interface,
                    Inherited = false, AllowMultiple = true)]
                internal sealed class {{AttributeName}}(
                    string referenceSourceFile,
                    string[] additionalUsings)
                    : Attribute
                {
                    public string ReferenceSourceFile { get; } = referenceSourceFile;
                    public string[] AdditionalUsings => [.. additionalUsings];
                }

                """);
        });

        var source = context.SyntaxProvider.ForAttributeWithMetadataName(
            $"{NamespaceName}.{AttributeName}",
            static (node, _) => node
                is ClassDeclarationSyntax
                or StructDeclarationSyntax
                or InterfaceDeclarationSyntax,
            GetMigrationInfo)
            .WithComparer(MigrationInfoComparer.Instance);

        context.RegisterSourceOutput(source, EmitMigrationSource);
    }


    private static MigrationInfo GetMigrationInfo(
        GeneratorAttributeSyntaxContext context,
        CancellationToken canceller)
    {
        var attributeData = context.Attributes
            .Where(static ad => ad.AttributeClass?.ToDisplayString() == $"{NamespaceName}.{AttributeName}")
            .Select(static ad =>
            {
                var referenceSourceFile = ad.ConstructorArguments[0].Value?.ToString();
                var additionalNamespaceUsings = ad.ConstructorArguments[1].Values
                    .Select(v => v.Value?.ToString())
                    .OfType<string>()
                    .ToImmutableArray();
                return new MigrationEntry(referenceSourceFile!, additionalNamespaceUsings);
            });
        return new MigrationInfo([.. attributeData]);
    }


    private static void EmitMigrationSource(
        SourceProductionContext context,
        MigrationInfo migrationInfo)
    {
        foreach(var entry in migrationInfo.Entries)
        {
            var usings = entry.AdditionalNamespaceUsings
                .Select(ns => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns)));
            var source = GetCompilationUnit(entry.ReferenceSourceFile.AsResourceName())
                .ReplaceNamespace()
                .AppendPartialKeyword()
                .WithLeadingTrivia(FileHelpers.NullableDirectiveTrivia)
                .AddUsings([.. usings])
                .NormalizeWhitespace()
                .ToFullString();
            context.AddSource($"{Path.GetFileNameWithoutExtension(entry.ReferenceSourceFile)}.g.cs", source);
        }
    }


    private static CompilationUnitSyntax GetCompilationUnit(string resourceName)
    {
        var assembly = typeof(RegexSourceGenerator).Assembly;
        using var stream = assembly.GetManifestResourceStream($"{Constant.RegexResourcePrefix}{resourceName}");
        using var sw = new StreamReader(stream!);
        return SyntaxFactory.ParseCompilationUnit(
            sw.ReadToEnd(),
            options: FileHelpers.Options);
    }


    internal record MigrationEntry(string ReferenceSourceFile, ImmutableArray<string> AdditionalNamespaceUsings);


    internal record MigrationInfo(ImmutableArray<MigrationEntry> Entries)
    {
        public static readonly MigrationInfo Empty = new([]);
    }


    internal class MigrationInfoComparer : IEqualityComparer<MigrationInfo>
    {
        public static MigrationInfoComparer Instance { get; } = new();

        public bool Equals(MigrationInfo x, MigrationInfo y)
        {
            if (x.Entries.Length != y.Entries.Length)
            {
                return false;
            }

            for (int i = 0; i < x.Entries.Length; i++)
            {
                if (x.Entries[i].ReferenceSourceFile != y.Entries[i].ReferenceSourceFile)
                {
                    return false;
                }
                if (!x.Entries[i].AdditionalNamespaceUsings.SequenceEqual(y.Entries[i].AdditionalNamespaceUsings))
                {
                    return false;
                }
            }
            return true;
        }


        public int GetHashCode(MigrationInfo obj)
        {
            static uint combineHashCodes(uint h1, uint h2)
            {
                unchecked
                {
                    return ((h1 << 13) | (h1 >> 19)) ^ h2;
                }
            }

            uint hash = 0u;
            foreach (var entry in obj.Entries)
            {
                hash = combineHashCodes(hash, (uint)entry.ReferenceSourceFile.GetHashCode());
                foreach (var ns in entry.AdditionalNamespaceUsings)
                {
                    hash = combineHashCodes(hash, (uint)ns.GetHashCode());
                }
            }
            return (int)hash;
        }
    }
}



file static class FileHelpers
{
    public static readonly CSharpParseOptions Options = new(
        LanguageVersion.CSharp14,
        preprocessorSymbols: ["DEBUG"]);


    public static readonly SyntaxTriviaList NullableDirectiveTrivia = SyntaxFactory.TriviaList(
        SyntaxFactory.Trivia(
            SyntaxFactory.NullableDirectiveTrivia(
                SyntaxFactory.Token(SyntaxKind.EnableKeyword),
                isActive: true)
            ));


    public static string AsResourceName(this string filePath)
    {
        return filePath
            .Replace(Path.DirectorySeparatorChar, '.')
            .Replace(Path.AltDirectorySeparatorChar, '.');
    }


    public static CompilationUnitSyntax ReplaceNamespace(this CompilationUnitSyntax cu)
    {
        var ns = cu
            .DescendantNodes()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault();
        return ns switch
        {
            NamespaceDeclarationSyntax xns =>
                cu.ReplaceNode(xns, xns.WithName(SyntaxFactory.ParseName("RunaString.RegularExpressions"))),
            FileScopedNamespaceDeclarationSyntax fns =>
                cu.ReplaceNode(fns, fns.WithName(SyntaxFactory.ParseName("RunaString.RegularExpressions"))),
            _ => cu,
        };
    }


    public static CompilationUnitSyntax AppendPartialKeyword(this CompilationUnitSyntax cu)
    {
        var targetTypes = cu
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>();
        return cu.ReplaceNodes(
            targetTypes,
            static (_, targetType) =>
            {
                if(targetType.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    return targetType;
                }
                return targetType.AddModifiers(_partialToken);
            });
    }
    private static readonly SyntaxToken _partialToken = SyntaxFactory.Token(SyntaxKind.PartialKeyword);

}