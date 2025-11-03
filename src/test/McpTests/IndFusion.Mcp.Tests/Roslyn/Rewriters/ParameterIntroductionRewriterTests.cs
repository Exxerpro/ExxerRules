namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary> ///  ParameterIntroductionRewriter AddsParameterAndArgument. /// </summary>
    [Fact]
    public void ParameterIntroductionRewriter_AddsParameterAndArgument()
    {
        var code = @"class C{ void M(){Console.WriteLine(1);} void Call(){ M(); } }";
        var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: TestContext.Current.CancellationToken);
        var root = tree.GetRoot(TestContext.Current.CancellationToken);
        var expr = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1));
        var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("p")).WithType(SyntaxFactory.ParseTypeName("int"));
        var paramRef = SyntaxFactory.IdentifierName("p");
        var generator = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);
        var rewriter = new ParameterIntroductionRewriter(expr, "M", parameter, paramRef, generator);
        var newRoot = Formatter.Format(rewriter.Visit(root)!, new AdhocWorkspace(), options: null, cancellationToken: TestContext.Current.CancellationToken);
        var text = newRoot.ToFullString();
        Assert.Contains("void M(int p)", text);
        Assert.Contains("Console.WriteLine(p)", text);
        Assert.Contains("M(1);", text);
    }
}
