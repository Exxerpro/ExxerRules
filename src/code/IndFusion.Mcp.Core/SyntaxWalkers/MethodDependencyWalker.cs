using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Walks an invocation tree to collect dependencies on a set of candidate method names.
/// </summary>
public class MethodDependencyWalker : CSharpSyntaxWalker
{
    private readonly HashSet<string> _candidateMethods;

    /// <summary>
    /// Gets the set of method names that were invoked and matched the candidates.
    /// </summary>
    public HashSet<string> Dependencies { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodDependencyWalker"/> class.
    /// </summary>
    /// <param name="candidateMethods">The set of method names to track.</param>
    public MethodDependencyWalker(HashSet<string> candidateMethods)
    {
        _candidateMethods = candidateMethods;
    }

    /// <inheritdoc />
    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        var identifier = node.Expression switch
        {
            IdentifierNameSyntax id => id.Identifier.ValueText,
            MemberAccessExpressionSyntax ma => ma.Name.Identifier.ValueText,
            _ => null
        };

        if (identifier != null && _candidateMethods.Contains(identifier))
        {
            Dependencies.Add(identifier);
        }

        base.VisitInvocationExpression(node);
    }
}
