namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// ParameterRewriter ReplacesIdentifiers.
    /// </summary>
    [Fact]
    public void ParameterRewriter_ReplacesIdentifiers()
    {
        var expr = SyntaxFactory.ParseExpression("a + b");
        var map = new Dictionary<string, ExpressionSyntax>
        {
            ["a"] = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
            ["b"] = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(2))
        };
        var rewriter = new ParameterRewriter(map);
        var result = rewriter.Visit(expr)!.NormalizeWhitespace().ToFullString();
        Assert.Equal("1 + 2", result);
    }

    /// <summary>
    /// ParameterRewriter ReplacesMemberAccess.
    /// </summary>
    [Fact]
    public void ParameterRewriter_ReplacesMemberAccess()
    {
        var expr = SyntaxFactory.ParseExpression("this.a + a");
        var map = new Dictionary<string, ExpressionSyntax>
        {
            ["a"] = SyntaxFactory.IdentifierName("p")
        };
        var rewriter = new ParameterRewriter(map);
        var result = rewriter.Visit(expr)!.NormalizeWhitespace().ToFullString();
        Assert.Equal("p + p", result);
    }
}
