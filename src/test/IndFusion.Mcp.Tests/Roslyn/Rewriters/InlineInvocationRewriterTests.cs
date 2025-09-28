namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary> ///  InlineInvocationRewriter InlinesVoidMethodCall. /// </summary>
    [Fact]
    public void InlineInvocationRewriter_InlinesVoidMethodCall()
    {
        var code = @"class C{ void Helper(){ Console.WriteLine(""Hi""); } void Call(){ Helper(); } }";
        var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: TestContext.Current.CancellationToken);
        var root = tree.GetRoot(TestContext.Current.CancellationToken);
        var helper = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First(m => m.Identifier.ValueText == "Helper");
        var rewriter = new InlineInvocationRewriter(helper);
        var newRoot = Formatter.Format(rewriter.Visit(root)!, new AdhocWorkspace(), options: null, cancellationToken: TestContext.Current.CancellationToken);
        var callMethod = newRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().First(m => m.Identifier.ValueText == "Call");
        var text = callMethod.Body!.Statements.ToFullString();
        Assert.Contains("Console.WriteLine(\"Hi\");", text);
    }
}
