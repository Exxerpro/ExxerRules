namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for MoveOverrideMethodToolTests.
/// </summary>
public class MoveOverrideMethodToolTests
{
	/// <summary>
	/// MoveOverrideMethod WithBaseCall AddsWrapper.
	/// </summary>
	[Fact]
	public void MoveOverrideMethod_WithBaseCall_AddsWrapper()
	{
		var source = @"public class Base { public virtual void Foo() {} } public class Derived : Base { public override void Foo() { base.Foo(); } }";
		var tree = CSharpSyntaxTree.ParseText(source, cancellationToken: TestContext.Current.CancellationToken);
		var root = tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken);

		var moveResult = MoveMethodAst.MoveInstanceMethodAst(root, "Derived", "Foo", "Target", string.Empty, string.Empty);
		var updatedRoot = MoveMethodAst.AddMethodToTargetClass(moveResult.NewSourceRoot, "Target", moveResult.MovedMethod, moveResult.Namespace);
		var formattedRoot = Formatter.Format(updatedRoot, new AdhocWorkspace(), options: null, cancellationToken: TestContext.Current.CancellationToken);

		Assert.Contains("BaseFoo", formattedRoot.ToFullString());
	}
}
