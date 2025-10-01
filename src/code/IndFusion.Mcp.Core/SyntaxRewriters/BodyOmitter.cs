using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewriter that replaces method bodies and arrow expressions with an omitted placeholder.
/// Intended for previewing or collapsing bodies during transformations.
/// </summary>
public class BodyOmitter : CSharpSyntaxRewriter
{
    /// <inheritdoc />
    public override SyntaxNode? VisitBlock(BlockSyntax node)
    {
        // Replace any block with an empty body
        return SyntaxFactory.Block();
    }

    /// <inheritdoc />
    public override SyntaxNode? VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
    {
        // Convert expression-bodied members into empty blocks by replacing the parent appropriately
        return SyntaxFactory.Block();
    }

    /// <inheritdoc />
    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        // Ensure we preserve the signature but omit the implementation consistently as "{}"
        var updated = node.WithBody(SyntaxFactory.Block()).WithExpressionBody(null).WithSemicolonToken(default);
        return base.VisitMethodDeclaration(updated);
    }
}
