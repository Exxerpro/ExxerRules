namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for moving override methods and generating appropriate stubs.
/// </summary>
public class MoveOverrideMethodToolTests
{
    /// <summary>
    /// Ensures 'override' is removed on the moved method and a stub remains in the derived class.
    /// </summary>
    [Fact]
    public void MoveOverrideMethod_RemovesOverrideInNewClass()
    {
        var source = @"public class Base { public virtual void Foo() {} } public class Derived : Base { public override void Foo() {} }";
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetRoot();

        var moveResult = MoveMethodAst.MoveInstanceMethodAst(root, "Derived", "Foo", "Target", "", "");
        var updatedRoot = MoveMethodAst.AddMethodToTargetClass(moveResult.NewSourceRoot, "Target", moveResult.MovedMethod, moveResult.Namespace);
        var formattedRoot = Formatter.Format(updatedRoot, new AdhocWorkspace());

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
