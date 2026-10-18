using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RunaString.InternalGenerators.Regex;

internal static class SyntaxHelpers
{
    extension(CompilationUnitSyntax cu)
    {
        public CompilationUnitSyntax ReplaceNamespaceName(string newName)
        {
            var ns = cu
                .DescendantNodes()
                .Single(static node => node is BaseNamespaceDeclarationSyntax);
            cu = ns switch
            {
                NamespaceDeclarationSyntax xns =>
                    cu.ReplaceNode(xns, xns.WithName(SyntaxFactory.ParseName(newName))),

                FileScopedNamespaceDeclarationSyntax fns =>
                    cu.ReplaceNode(fns, fns.WithName(SyntaxFactory.ParseName(newName))),

                _ => throw new InvalidOperationException(""),
            };
            return cu;
        }


        public CompilationUnitSyntax ReplaceTypeName(string oldName, string newName)
        {
            while (true)
            {
                var node = cu
                    .DescendantNodes()
                    .FirstOrDefault(node => node is TypeDeclarationSyntax typeDecl && typeDecl.Identifier.Text == oldName);
                switch(node)
                {
                case ClassDeclarationSyntax cls:
                    cu = cu.ReplaceNode(cls, cls.WithIdentifier(SyntaxFactory.Identifier(newName)));
                    continue;
                case StructDeclarationSyntax strct:
                    cu = cu.ReplaceNode(strct, strct.WithIdentifier(SyntaxFactory.Identifier(newName)));
                    continue;
                default:
                    break;
                }
                break;
            }
            while(true)
            {
                var node = cu
                    .DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .FirstOrDefault(node => node.Identifier.Text == oldName);
                if(node is not null)
                {
                    cu = cu.ReplaceNode(node, node.WithIdentifier(SyntaxFactory.Identifier(newName)));
                    continue;
                }
                    break;
            }
            return cu;
        }
    }
}
