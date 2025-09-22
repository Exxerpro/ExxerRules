namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// SetterToInitRewriter ReplacesSetterWithInit.
    /// </summary>
    [Fact]
    public void SetterToInitRewriter_ReplacesSetterWithInit()
    {
        var prop = SyntaxFactory.ParseMemberDeclaration("public int P { get; set; }") as PropertyDeclarationSyntax;
        var rewriter = new SetterToInitRewriter("P");
        var result = rewriter.Visit(prop!)!.NormalizeWhitespace().ToFullString();
        Assert.Contains("init", result);
    }
}
