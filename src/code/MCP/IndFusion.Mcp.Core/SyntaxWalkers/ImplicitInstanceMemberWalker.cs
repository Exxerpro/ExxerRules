using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Identifies member names that are implicitly referenced as instance members
/// (i.e., without an explicit <c>this.</c> qualifier) within method bodies.
/// </summary>
public class ImplicitInstanceMemberWalker : CSharpSyntaxWalker
{
    private readonly HashSet<string> _parameters = [];
    private readonly HashSet<string> _locals = [];
    /// <summary>Collected member names inferred to be instance members.</summary>
    public HashSet<string> Members { get; } = [];

    /// <summary>
    /// Records parameter names so they are not treated as instance members.
    /// </summary>
    /// <param name="node">The method declaration node.</param>
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        foreach (var p in node.ParameterList.Parameters)
            _parameters.Add(p.Identifier.ValueText);
        base.VisitMethodDeclaration(node);
    }

    /// <summary>
    /// Records local variable names so they are not treated as instance members.
    /// </summary>
    /// <param name="node">The variable declarator node.</param>
    public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
    {
        _locals.Add(node.Identifier.ValueText);
        base.VisitVariableDeclarator(node);
    }

    /// <summary>
    /// Adds identifiers that are likely instance members to <see cref="Members"/>.
    /// </summary>
    /// <param name="node">The identifier node.</param>
    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        var name = node.Identifier.ValueText;
        var parent = node.Parent;

        if (_parameters.Contains(name) || _locals.Contains(name) || parent is TypeSyntax || SyntaxFacts.IsInNamespaceOrTypeContext(node))
        {
            base.VisitIdentifierName(node);
            return;
        }

        if (parent is AssignmentExpressionSyntax assign &&
            assign.Left == node &&
            assign.Parent is InitializerExpressionSyntax init &&
            (init.IsKind(SyntaxKind.ObjectInitializerExpression) || init.IsKind(SyntaxKind.WithInitializerExpression)))
        {
            base.VisitIdentifierName(node);
            return;
        }

        if (parent is NameColonSyntax { Parent: SubpatternSyntax { Parent: PropertyPatternClauseSyntax } })
        {
            base.VisitIdentifierName(node);
            return;
        }

        if (parent is MemberAccessExpressionSyntax ma && ma.Expression == node)
        {
            base.VisitIdentifierName(node);
            return;
        }

        if (parent is QualifiedNameSyntax qn && qn.Left == node)
        {
            base.VisitIdentifierName(node);
            return;
        }

        if (parent is InvocationExpressionSyntax)
        {
            base.VisitIdentifierName(node);
            return;
        }

        Members.Add(name);
        base.VisitIdentifierName(node);
    }
}
