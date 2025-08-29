using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExxerFactor.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Collects information about private field declarations and their types.
/// </summary>
public class PrivateFieldInfoWalker : CSharpSyntaxWalker
{
    /// <summary>
    /// Gets a map from field name to its declared <see cref="TypeSyntax"/>.
    /// </summary>
    public Dictionary<string, TypeSyntax> Infos { get; } = new();

    /// <inheritdoc />
    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
        {
            foreach (var variable in node.Declaration.Variables)
                Infos[variable.Identifier.ValueText] = node.Declaration.Type;
        }
        base.VisitFieldDeclaration(node);
    }
}