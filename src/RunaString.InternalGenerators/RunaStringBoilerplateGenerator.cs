using Microsoft.CodeAnalysis;
using SourceGeneratorToolkit;

namespace RunaString.InternalGenerators;

[Generator(LanguageNames.CSharp)]
public class RunaStringBoilerplateGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static cxt =>
        {
            cxt.AddSource(
                $"RunaString.Attribute.g.cs",
                """
                using System;
                namespace RunaString;

                [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
                internal sealed class RunaStringAttribute : Attribute
                {
                }
                """);
        });
        var source = context.SyntaxProvider.ForAttributeWithMetadataName(
            "RunaString.RunaStringAttribute",
            static (node, token) => true,
            Transform);
        context.RegisterSourceOutput(source, Emit);
    }

    private static RunaStringInfo Transform(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        var stringSymbol = (INamedTypeSymbol)context.TargetSymbol;
        var interfaceSymbol = stringSymbol.Interfaces.Single(static i => i.Name == "IRunaString");
        var enumeratorSymbol = (INamedTypeSymbol)interfaceSymbol.TypeArguments[1];
        var indexSymbol = (INamedTypeSymbol)interfaceSymbol.TypeArguments[2];
        return new(
            stringSymbol.IsRefLikeType,
            stringSymbol.Name,
            interfaceSymbol.Name,
            indexSymbol.Name);
    }

    private static void Emit(SourceProductionContext context, RunaStringInfo source)
    {
        var sb = new SourceBuilderSlim();
        sb.AppendLine("""
            using System.Text;
            namespace RunaString;
            
            """);
            
        sb.AppendLine();
        if (source.IsRefStruct)
        {
            sb.AppendLine($"ref partial struct {source.StringTypeName}");
        }
        else
        {
            sb.AppendLine($"partial struct {source.StringTypeName}");
        }
        sb.AppendLine($$"""
            {
                /// <inheritdoc />
                public partial Rune this[{{source.IndexTypeName}} index] =>
                    TryGetRune(index, out var rune)
                    ? rune
                    : throw new IndexOutOfRangeException();

                /// <inheritdoc />
                public partial bool TryGetRune({{source.IndexTypeName}} index, out Rune rune) =>
                    TryGetRune(index, out rune, out _);
            }
            """);

        var code = sb.Build();
        context.AddSource(
            $"{source.StringTypeName}.g.cs",
            code);
    }


    private record RunaStringInfo(
        bool IsRefStruct,
        string StringTypeName,
        string EnumeratorTypeName,
        string IndexTypeName);
}

