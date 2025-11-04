namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// StaticConversionRewriter ConvertsInstanceMethod.
    /// </summary>
    [Fact]
    public void StaticConversionRewriter_ConvertsInstanceMethod()
    {
        var method = SyntaxFactory.ParseMemberDeclaration("int GetX(){ return x; }") as MethodDeclarationSyntax;
        var rewriter = new StaticConversionRewriter(System.Array.Empty<(string Name, string Type)>(), "inst", ["x"]);
        var result = rewriter.Rewrite(method!).NormalizeWhitespace().ToFullString();
        Assert.Contains("static", result);
        Assert.Contains("inst.x", result);
    }
}
