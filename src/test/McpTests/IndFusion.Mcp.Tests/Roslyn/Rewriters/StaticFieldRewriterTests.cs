namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// StaticFieldRewriter QualifiesStaticField.
    /// </summary>
    [Fact]
    public void StaticFieldRewriter_QualifiesStaticField()
    {
        var method = SyntaxFactory.ParseMemberDeclaration("void Test(){ x = 1; }") as MethodDeclarationSyntax;
        var rewriter = new StaticFieldRewriter(["x"], "C");
        var result = rewriter.Visit(method!)!.NormalizeWhitespace().ToFullString();
        Assert.Contains("C.x", result);
    }
}
