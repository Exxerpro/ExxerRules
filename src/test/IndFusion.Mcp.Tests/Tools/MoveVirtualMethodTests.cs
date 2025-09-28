namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Tests for MoveVirtualMethodTests.
/// </summary>
public class MoveVirtualMethodTests
{
    /// <summary>
    /// MoveOverrideMethod WithBaseCall AddsWrapper.
    /// </summary>
    [Fact]
    public void MoveOverrideMethod_WithBaseCall_AddsWrapper()
    {
        var source = "public class A { public virtual void Method1() {} } " +
                     "public class B : A { public override void Method1() { base.Method1(); System.Console.WriteLine(\"B\"); } }";
        var tree = CSharpSyntaxTree.ParseText(source, cancellationToken: TestContext.Current.CancellationToken);
        var root = tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken);

        var moveResult = MoveMethodAst.MoveInstanceMethodAst(root, "B", "Method1", "ExtractedFromB", "", "");
        var updatedRoot = MoveMethodAst.AddMethodToTargetClass(moveResult.NewSourceRoot, "ExtractedFromB", moveResult.MovedMethod, moveResult.Namespace);
        var formattedRoot = Formatter.Format(updatedRoot, new AdhocWorkspace(), options: null, cancellationToken: TestContext.Current.CancellationToken);

        var bClass = formattedRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First(c => c.Identifier.ValueText == "B");
        Assert.Contains("BaseMethod1", bClass.ToFullString());

        var targetClass = formattedRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First(c => c.Identifier.ValueText == "ExtractedFromB");
        var moved = targetClass.Members.OfType<MethodDeclarationSyntax>().First();
        Assert.Contains("BaseMethod1", moved.ToFullString());
    }
}
