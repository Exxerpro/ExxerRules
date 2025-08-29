using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExxerFactor.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewrites identifier usages for known static field names to include an explicit
/// type qualifier when the context omits it, improving clarity and correctness.
/// </summary>
public class StaticFieldRewriter : CSharpSyntaxRewriter
{
    private readonly HashSet<string> _staticFieldNames;
    private readonly string _sourceClassName;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticFieldRewriter"/> class.
    /// </summary>
    /// <param name="staticFieldNames">Set of field identifiers considered static.</param>
    /// <param name="sourceClassName">The declaring class name to qualify member access with.</param>
    public StaticFieldRewriter(HashSet<string> staticFieldNames, string sourceClassName)
    {
        _staticFieldNames = staticFieldNames;
        _sourceClassName = sourceClassName;
    }

    /// <summary>
    /// Rewrites identifier names to explicit member access when matching a known static field.
    /// </summary>
    /// <param name="node">The identifier node.</param>
    /// <returns>The potentially rewritten syntax node.</returns>
    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        var parent = node.Parent;
        if (parent is ParameterSyntax or TypeSyntax)
            return base.VisitIdentifierName(node);

        if (_staticFieldNames.Contains(node.Identifier.ValueText))
        {
            if (parent is not MemberAccessExpressionSyntax ||
                (parent is MemberAccessExpressionSyntax memberAccess && memberAccess.Name == node))
            {
                return SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(_sourceClassName),
                    node);
            }
        }

        return base.VisitIdentifierName(node);
    }
}