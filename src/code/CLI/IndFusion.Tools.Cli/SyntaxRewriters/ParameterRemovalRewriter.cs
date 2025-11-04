using IndFusion.Tools.Cli.Tools;

namespace IndFusion.Tools.Cli.SyntaxRewriters;

/// <summary>
/// Rewrites a syntax tree to remove a specific parameter from a method declaration
/// and corresponding argument from invocations of that method.
/// </summary>
public class ParameterRemovalRewriter : CSharpSyntaxRewriter
{
    private readonly string _methodName;
    private readonly int _parameterIndex;
    private readonly SyntaxGenerator _generator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterRemovalRewriter"/>.
    /// </summary>
    /// <param name="methodName">The name of the method whose parameter should be removed.</param>
    /// <param name="parameterIndex">Zero-based index of the parameter to remove.</param>
    /// <param name="generator">The syntax generator used for safe node creation.</param>
    public ParameterRemovalRewriter(string methodName, int parameterIndex, SyntaxGenerator generator)
    {
        _methodName = methodName;
        _parameterIndex = parameterIndex;
        _generator = generator;
    }

    /// <summary>
    /// Removes the parameter at the configured index from the visited method declaration.
    /// </summary>
    /// <param name="node">The method declaration to transform.</param>
    /// <returns>The transformed method declaration.</returns>
    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var visited = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node)!;
        if (node.Identifier.ValueText == _methodName && _parameterIndex < node.ParameterList.Parameters.Count)
        {
            var newParams = visited.ParameterList.Parameters.RemoveAt(_parameterIndex);
            visited = visited.WithParameterList(visited.ParameterList.WithParameters(newParams));
        }
        return visited;
    }

    /// <summary>
    /// Removes the corresponding argument from invocations of the configured method.
    /// </summary>
    /// <param name="node">The invocation node to transform.</param>
    /// <returns>The transformed invocation node.</returns>
    public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        var visited = (InvocationExpressionSyntax)base.VisitInvocationExpression(node)!;
        if (InvocationHelpers.IsInvocationOf(visited, _methodName) && _parameterIndex < visited.ArgumentList.Arguments.Count)
        {
            visited = AstTransformations.RemoveArgument(visited, _parameterIndex);
        }

        return visited;
    }
}
