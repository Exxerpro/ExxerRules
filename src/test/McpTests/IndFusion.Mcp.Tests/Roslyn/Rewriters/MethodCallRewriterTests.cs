namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// MethodCallRewriter QualifiesMethodCalls.
    /// </summary>
    [Fact]
    public void MethodCallRewriter_QualifiesMethodCalls()
    {
        var method = SyntaxFactory.ParseMemberDeclaration("void Test(){ Do(); }") as MethodDeclarationSyntax;
        var rewriter = new MethodCallRewriter(["Do"], "inst");
        var result = rewriter.Visit(method!)!.NormalizeWhitespace().ToFullString();
        Assert.Contains("inst.Do()", result);
    }

    /// <summary>
    /// MethodCallRewriter QualifiesThisMethodCalls.
    /// </summary>
    [Fact]
    public void MethodCallRewriter_QualifiesThisMethodCalls()
    {
        var method = SyntaxFactory.ParseMemberDeclaration("void Test(){ this.Do(); }") as MethodDeclarationSyntax;
        var rewriter = new MethodCallRewriter(["Do"], "inst");
        var result = rewriter.Visit(method!)!.NormalizeWhitespace().ToFullString();
        Assert.Contains("inst.Do()", result);
        Assert.DoesNotContain("this.Do()", result);
    }
}
