namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Tests for MoveProtectedOverrideDependencyTests.
/// </summary>
public class MoveProtectedOverrideDependencyTests
{
	/// <summary>
	/// MoveProtectedOverrideDependency AddsBaseWrapper.
	/// </summary>
	[Fact]
	public void MoveProtectedOverrideDependency_AddsBaseWrapper()
	{
		var source = @"public class Base { protected virtual void DoIt() {} } public class Derived : Base { protected override void DoIt() {} public void Another() { DoIt(); } } public class Target { }";
		var tree = CSharpSyntaxTree.ParseText(source, cancellationToken: TestContext.Current.CancellationToken);
		var root = tree.GetRoot(cancellationToken: TestContext.Current.CancellationToken);

		var moveResult = MoveMethodAst.MoveInstanceMethodAst(root, "Derived", "DoIt", "Target", string.Empty, string.Empty);
		var updatedRoot = MoveMethodAst.AddMethodToTargetClass(moveResult.NewSourceRoot, "Target", moveResult.MovedMethod, moveResult.Namespace);
		var formattedRoot = Formatter.Format(updatedRoot, new AdhocWorkspace(), options: null, cancellationToken: TestContext.Current.CancellationToken);

		var targetClass = formattedRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First(c => c.Identifier.ValueText == "Target");
		Assert.Contains("BaseDoIt", targetClass.ToFullString());
	}
}
