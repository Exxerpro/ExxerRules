using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewriter that qualifies method calls with a provided parameter when the call targets a known instance method.
/// </summary>
public class MethodCallRewriter : CSharpSyntaxRewriter
{
    private readonly HashSet<string> _classMethodNames;
    private readonly string _parameterName;

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodCallRewriter"/> class.
    /// </summary>
    /// <param name="classMethodNames">Set of method names that should be qualified.</param>
    /// <param name="parameterName">The parameter name to qualify calls with.</param>
    public MethodCallRewriter(HashSet<string> classMethodNames, string parameterName)
    {
        _classMethodNames = classMethodNames;
        _parameterName = parameterName;
    }

    /// <inheritdoc />
    public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (node.Expression is IdentifierNameSyntax identifier && _classMethodNames.Contains(identifier.Identifier.ValueText))
        {
            var memberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(_parameterName),
                identifier);
            return node.WithExpression(memberAccess);
        }
        else if (node.Expression is MemberAccessExpressionSyntax member &&
                 member.Expression is ThisExpressionSyntax &&
                 InvocationHelpers.GetInvokedMethodName(node) is string name &&
                 _classMethodNames.Contains(name))
        {
            var updatedMember = member.WithExpression(SyntaxFactory.IdentifierName(_parameterName));
            return node.WithExpression(updatedMember);
        }

        return base.VisitInvocationExpression(node);
    }
}
