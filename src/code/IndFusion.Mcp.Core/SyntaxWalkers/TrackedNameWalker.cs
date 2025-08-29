using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Base walker that tracks occurrences of specified identifier names during traversal.
/// </summary>
public abstract class TrackedNameWalker : CSharpSyntaxWalker
{
    private readonly HashSet<string> _names;
    private readonly Action<string>? _onMatch;

    /// <summary>
    /// Gets the set of matched identifier names encountered during traversal.
    /// </summary>
    public HashSet<string> Matches { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackedNameWalker"/> class.
    /// </summary>
    /// <param name="names">Names to track.</param>
    /// <param name="onMatch">Optional callback when a name is recorded.</param>
    protected TrackedNameWalker(HashSet<string> names, Action<string>? onMatch = null)
    {
        _names = names;
        _onMatch = onMatch;
    }

    /// <summary>
    /// Determines whether the provided name is a target that should be tracked.
    /// </summary>
    /// <param name="name">The identifier name.</param>
    protected bool IsTarget(string name) => _names.Contains(name);

    /// <summary>
    /// Returns true if the node represents a parameter or a type identifier.
    /// </summary>
    protected static bool IsParameterOrType(SyntaxNode? node) =>
        node is ParameterSyntax or TypeSyntax;

    /// <summary>
    /// Returns true if the member access refers to this instance and the given identifier as its name.
    /// </summary>
    protected static bool IsThisMember(MemberAccessExpressionSyntax ma, IdentifierNameSyntax node) =>
        ma.Expression is ThisExpressionSyntax && ma.Name == node;

    /// <summary>
    /// Returns true if the identifier is the expression of a member access.
    /// </summary>
    protected static bool IsMemberExpression(IdentifierNameSyntax node, MemberAccessExpressionSyntax ma) =>
        ma.Expression == node;

    /// <summary>
    /// Extracts the invoked method name from an invocation expression when available.
    /// </summary>
    /// <param name="node">The invocation expression.</param>
    /// <returns>The method name or null.</returns>
    protected static string? GetInvocationName(InvocationExpressionSyntax node)
    {
        return node.Expression switch
        {
            IdentifierNameSyntax id => id.Identifier.ValueText,
            MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax, Name: IdentifierNameSyntax id } => id.Identifier.ValueText,
            _ => null
        };
    }

    /// <summary>
    /// Determines whether the given identifier name should be recorded as a match.
    /// </summary>
    /// <param name="node">The identifier node.</param>
    /// <returns>True to record, otherwise false.</returns>
    protected virtual bool ShouldRecordIdentifier(IdentifierNameSyntax node)
    {
        var parent = node.Parent;
        if (!IsTarget(node.Identifier.ValueText) || IsParameterOrType(parent))
            return false;

        if (parent is MemberAccessExpressionSyntax memberAccess)
        {
            if (IsThisMember(memberAccess, node) || IsMemberExpression(node, memberAccess))
                return true;
            return false;
        }

        if (parent is InvocationExpressionSyntax)
            return false;

        return true;
    }

    /// <inheritdoc />
    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (ShouldRecordIdentifier(node))
            RecordMatch(node.Identifier.ValueText);
        base.VisitIdentifierName(node);
    }

    /// <summary>
    /// Attempts to record an invocation occurrence for tracking.
    /// </summary>
    /// <param name="node">The invocation expression.</param>
    /// <returns>True if handled; otherwise false to let base visit.</returns>
    protected virtual bool TryRecordInvocation(InvocationExpressionSyntax node) => false;

    /// <inheritdoc />
    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (!TryRecordInvocation(node))
            base.VisitInvocationExpression(node);
    }

    /// <summary>
    /// Records a match and invokes the optional callback.
    /// </summary>
    /// <param name="name">The matched name.</param>
    protected void RecordMatch(string name)
    {
        Matches.Add(name);
        _onMatch?.Invoke(name);
    }
}
