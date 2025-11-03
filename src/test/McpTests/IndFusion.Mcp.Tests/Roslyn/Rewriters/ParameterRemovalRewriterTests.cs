namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary>
    /// ParameterRemovalRewriter RemovesParameter.
    /// </summary>
    [Fact]
    public void ParameterRemovalRewriter_RemovesParameter()
    {
        var tree = CSharpSyntaxTree.ParseText("class A{void M(int a,int b){}} class B{void Call(){new A().M(1,2);}} ", cancellationToken: TestContext.Current.CancellationToken);
        var root = tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken);
        var generator = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);
        var rewriter = new ParameterRemovalRewriter("M", 1, generator);
        var newRoot = Formatter.Format(rewriter.Visit(root)!, new AdhocWorkspace(), cancellationToken: TestContext.Current.CancellationToken);
        var text = newRoot.ToFullString();
        Assert.Contains("void M(int a)", text);
        Assert.Contains("M(1)", text);
        Assert.DoesNotContain("2)", text);
    }
}
