using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Collects names of methods encountered during syntax traversal.
/// </summary>
public class MethodNameWalker : NameCollectorWalker
{
    /// <inheritdoc />
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        Add(node.Identifier.ValueText);
        base.VisitMethodDeclaration(node);
    }
}
