using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExxerFactor.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Collects names of static fields declared in the visited syntax tree.
/// </summary>
public class StaticFieldNameWalker : NameCollectorWalker
{
    /// <inheritdoc />
    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        if (node.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            foreach (var variable in node.Declaration.Variables)
                Add(variable.Identifier.ValueText);
        }
        base.VisitFieldDeclaration(node);
    }
}