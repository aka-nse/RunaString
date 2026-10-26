using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RunaString.InternalGenerators.Regex;

namespace RunaString.InternalGenerators;

internal static class InternalHelpers
{
    extension(string str)
    {
        public string Replace(ReadOnlySpan<char> oldValues, char newValue)
        {
            var sb = new StringBuilder(str.Length);
            int startIndex = 0;
            while (true)
            {
                int index = str.AsSpan(startIndex).IndexOfAny(oldValues);
                if (index == -1)
                {
                    sb.Append(str.AsSpan(startIndex));
                    return sb.ToString();
                }
                sb.Append(str.AsSpan(startIndex, index));
                sb.Append(newValue);
                startIndex += index + 1;
            }
        }


        public byte[] AsHexString()
        {
            var bytes = new byte[str.Length / 2];
            for (int i = 0, j = 0; i < str.Length; i += 2, j++)
            {
                if (char.IsWhiteSpace(str[i]))
                {
                    continue;
                }
                bytes[j] = Convert.ToByte(str.AsSpan(i, 2).ToString(), 16);
            }
            return bytes;
        }
    }


    extension(StringBuilder sb)
    {
        public void Append(ReadOnlySpan<char> value)
        {
            unsafe
            {
                fixed (char* pValue = value)
                {
                    sb.Append(pValue, value.Length);
                }
            }
        }
    }


    public static CompilationUnitSyntax GetCompilationUnit(string resourceName)
    {
        var assembly = typeof(RegexSourceGenerator).Assembly;
        using var stream = assembly.GetManifestResourceStream($"{Constant.RegexResourcePrefix}{resourceName}");
        using var sw = new StreamReader(stream!);
        return SyntaxFactory.ParseCompilationUnit(
            sw.ReadToEnd(),
            options: _options);
    }
    private static readonly CSharpParseOptions _options = new(
        LanguageVersion.CSharp14,
        preprocessorSymbols: ["DEBUG"]);


    public static CompilationUnitSyntax ReplaceNamespace(this CompilationUnitSyntax cu, string newNamespace)
    {
        var ns = cu
            .DescendantNodes()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault();
        return ns switch
        {
            NamespaceDeclarationSyntax xns =>
                cu.ReplaceNode(xns, xns.WithName(SyntaxFactory.ParseName(newNamespace))),
            FileScopedNamespaceDeclarationSyntax fns =>
                cu.ReplaceNode(fns, fns.WithName(SyntaxFactory.ParseName(newNamespace))),
            _ => cu,
        };
    }
    public static readonly SyntaxTriviaList NullableDirectiveTrivia = SyntaxFactory.TriviaList(
        SyntaxFactory.Trivia(
            SyntaxFactory.NullableDirectiveTrivia(
                SyntaxFactory.Token(SyntaxKind.EnableKeyword),
                isActive: true)
            ));


    public static ImmutableArray<T> NullToEmpty<T>(this ImmutableArray<T> source) =>
        source.IsDefault ? [] : source;


    public static string AsResourceName(this string filePath)
    {
        return filePath
            .Replace(Path.DirectorySeparatorChar, '.')
            .Replace(Path.AltDirectorySeparatorChar, '.');
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
                if (targetType.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    return targetType;
                }
                return targetType.AddModifiers(_partialToken);
            });
    }
    private static readonly SyntaxToken _partialToken = SyntaxFactory.Token(SyntaxKind.PartialKeyword);


    public static ImmutableArray<string> GetStringArrayArgument(this ImmutableArray<TypedConstant> args) =>
        [.. args
            .NullToEmpty()
            .Select(static arg => arg.Value?.ToString())
            .OfType<string>(),
        ];
}
