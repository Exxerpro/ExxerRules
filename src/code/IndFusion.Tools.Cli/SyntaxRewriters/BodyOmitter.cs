namespace IndFusion.Tools.Mcp.App.SyntaxRewriters;

internal class BodyOmitter : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitBlock(BlockSyntax node)
    {
        return SyntaxFactory.Block(SyntaxFactory.ParseStatement("// ...\n"));
    }

    public override SyntaxNode? VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
    {
        return SyntaxFactory.Block(SyntaxFactory.ParseStatement("// ...\n"));
    }
}
