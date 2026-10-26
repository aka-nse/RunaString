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
                #nullable enable
                using System;
                
                namespace {{NamespaceName}};
                
                [AttributeUsage(
                    AttributeTargets.Class
                    | AttributeTargets.Struct
                    | AttributeTargets.Interface,
                    Inherited = false, AllowMultiple = true)]
                internal sealed class {{AttributeName}}(
                    string referenceSourceFile,
                    string[]? additionalUsings = null,
                    string[]? additionalPreprocessorSymbols = null)
                    : Attribute
                {
                    public string ReferenceSourceFile { get; } = referenceSourceFile;
                    public string[] AdditionalUsings => [.. additionalUsings ?? []];
                    public string[] AdditionalPreprocessorSymbols => [.. additionalPreprocessorSymbols ?? []];
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
                var additionalNamespaceUsings = ad.ConstructorArguments[1].Values.GetStringArrayArgument();
                var additionalPreprocessorSymbols = ad.ConstructorArguments[2].Values.GetStringArrayArgument();
                return new MigrationEntry(
                    referenceSourceFile!,
                    additionalNamespaceUsings,
                    additionalPreprocessorSymbols);
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
            var preprocessorSymbols = string.Concat(entry.AdditionalPreprocessorSymbols.Select(static symbol => $"#define {symbol}\n"));
            var source =
                preprocessorSymbols +
                InternalHelpers.GetCompilationUnit(entry.ReferenceSourceFile.AsResourceName())
                .ReplaceNamespace("RunaString.RegularExpressions")
                .AppendPartialKeyword()
                .WithLeadingTrivia(InternalHelpers.NullableDirectiveTrivia)
                .AddUsings([.. usings])
                .NormalizeWhitespace()
                .ToFullString();
            context.AddSource($"{Path.GetFileNameWithoutExtension(entry.ReferenceSourceFile)}.g.cs", source);
        }
    }


    internal record MigrationEntry(
        string ReferenceSourceFile,
        ImmutableArray<string> AdditionalNamespaceUsings,
        ImmutableArray<string> AdditionalPreprocessorSymbols);


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
                if (!Enumerable.SequenceEqual(
                    x.Entries[i].AdditionalNamespaceUsings,
                    y.Entries[i].AdditionalNamespaceUsings))
                {
                    return false;
                }
                if (!Enumerable.SequenceEqual(
                    x.Entries[i].AdditionalPreprocessorSymbols,
                    y.Entries[i].AdditionalPreprocessorSymbols))
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
