using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using IndFusion.Mcp.Mcp.Core.Tools;

namespace IndFusion.Mcp.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewriter that removes a parameter from a method by index and updates call sites accordingly.
/// </summary>
public class ParameterRemovalRewriter : CSharpSyntaxRewriter
{
    private readonly string _methodName;
    private readonly int _parameterIndex;
    private readonly SyntaxGenerator _generator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterRemovalRewriter"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method to modify.</param>
    /// <param name="parameterIndex">The zero-based index of the parameter to remove.</param>
    /// <param name="generator">Syntax generator used for argument updates.</param>
    public ParameterRemovalRewriter(string methodName, int parameterIndex, SyntaxGenerator generator)
    {
        _methodName = methodName;
        _parameterIndex = parameterIndex;
        _generator = generator;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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
