namespace IndFusion.Mcp.Tests.Tools;

/// <summary> /// Represents the  MoveOverrideMethodTests  class. /// </summary>
public class MoveOverrideMethodTests
{
    /// <summary> ///  MoveOverrideMethod RemovesOverrideInNewClass. /// </summary>
    [Fact]
    public void MoveOverrideMethod_RemovesOverrideInNewClass()
    {
        var source = @"public class Base { public virtual void Foo() {} } public class Derived : Base { public override void Foo() {} }";
        var tree = CSharpSyntaxTree.ParseText(source, cancellationToken: TestContext.Current.CancellationToken);
        var root = tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken);

        var moveResult = MoveMethodAst.MoveInstanceMethodAst(root, "Derived", "Foo", "Target", "", "");
        var updatedRoot = MoveMethodAst.AddMethodToTargetClass(moveResult.NewSourceRoot, "Target", moveResult.MovedMethod, moveResult.Namespace);
        var formattedRoot = Formatter.Format(updatedRoot, new AdhocWorkspace(), options: null, cancellationToken: TestContext.Current.CancellationToken);

        var targetClass = formattedRoot.DescendantNodes().OfType<ClassDeclarationSyntax>()
            .First(c => c.Identifier.ValueText == "Target");
        var movedMethod = targetClass.Members.OfType<MethodDeclarationSyntax>().First();
        Assert.DoesNotContain("override", movedMethod.Modifiers.ToFullString());

        var derivedClass = formattedRoot.DescendantNodes().OfType<ClassDeclarationSyntax>()
            .First(c => c.Identifier.ValueText == "Derived");
        var stub = derivedClass.Members.OfType<MethodDeclarationSyntax>().First(m => m.Identifier.ValueText == "Foo");
        Assert.Contains("override", stub.Modifiers.ToFullString());
    }
}
