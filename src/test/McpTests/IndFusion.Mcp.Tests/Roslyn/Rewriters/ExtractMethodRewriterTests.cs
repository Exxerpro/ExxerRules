namespace IndFusion.Mcp.Tests.Roslyn.Rewriters;

/// <summary>
/// Tests for Rewriters.
/// </summary>
public partial class RoslynTransformationTests
{
    /// <summary> ///  ExtractMethodRewriter ExtractsStatementsToNewMethod. /// </summary>
    [Fact]
    public void ExtractMethodRewriter_ExtractsStatementsToNewMethod()
    {
        var code = @"class C{ void M(){ Console.WriteLine(1); Console.WriteLine(2); } }";
        var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: TestContext.Current.CancellationToken);
        var root = tree.GetRoot(TestContext.Current.CancellationToken);
        var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
        var firstStmt = method.Body!.Statements.First();
        var rewriter = new ExtractMethodRewriter(method, root.DescendantNodes().OfType<ClassDeclarationSyntax>().First(), [firstStmt], "NewMethod");
        var newRoot = Formatter.Format(rewriter.Visit(root)!, new AdhocWorkspace(), options: null, cancellationToken: TestContext.Current.CancellationToken);
        var text = newRoot.ToFullString();
        Assert.Contains("private void NewMethod()", text);
        Assert.Contains("NewMethod();", text);
    }
}
