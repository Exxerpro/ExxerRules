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
        return SyntaxFactory.Block(SyntaxFactory.ParseStatement("// ...\n"));
    }

    /// <inheritdoc />
    public override SyntaxNode? VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
    {
        return SyntaxFactory.Block(SyntaxFactory.ParseStatement("// ...\n"));
    }
}
