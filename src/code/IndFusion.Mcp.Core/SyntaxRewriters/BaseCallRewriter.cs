using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewrites base method invocations to call a provided wrapper on a specified parameter instead.
/// Useful when moving instance methods and needing to redirect base calls.
/// </summary>
public class BaseCallRewriter : CSharpSyntaxRewriter
{
    private readonly string _methodName;
    private readonly string _parameterName;
    private readonly string _wrapperName;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseCallRewriter"/> class.
    /// </summary>
    /// <param name="methodName">The original method name that may be invoked via base.</param>
    /// <param name="parameterName">The name of the parameter instance to use for redirection.</param>
    /// <param name="wrapperName">The name of the wrapper method to call instead of the base call.</param>
    public BaseCallRewriter(string methodName, string parameterName, string wrapperName)
    {
        _methodName = methodName;
        _parameterName = parameterName;
        _wrapperName = wrapperName;
    }

    /// <summary>
    /// Rewrites invocation expressions, redirecting base calls to the configured wrapper.
    /// </summary>
    /// <param name="node">The invocation expression to visit.</param>
    /// <returns>The possibly rewritten node.</returns>
    public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (InvocationHelpers.IsBaseInvocationOf(node, _methodName))
        {
            var memberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(_parameterName),
                SyntaxFactory.IdentifierName(_wrapperName));
            return node.WithExpression(memberAccess);
        }

        return base.VisitInvocationExpression(node)!;
    }
}
