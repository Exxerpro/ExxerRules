using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExxerFactor.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Base rewriter that replaces a target expression with a provided
/// reference and inserts a declaration into the specified container
/// when that container is visited.
/// </summary>
public abstract class ExpressionIntroductionRewriter<TContainer> : CSharpSyntaxRewriter where TContainer : SyntaxNode
{
    private readonly ExpressionSyntax _targetExpression;
    private readonly ExpressionSyntax _replacement;
    private readonly SyntaxNode _declaration;
    private readonly TContainer? _targetContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionIntroductionRewriter{TContainer}"/> class.
    /// </summary>
    /// <param name="targetExpression">The expression to search for and replace.</param>
    /// <param name="replacement">The expression that will replace <paramref name="targetExpression"/>.</param>
    /// <param name="declaration">The declaration to insert into the container.</param>
    /// <param name="targetContainer">Optional container where the declaration should be inserted.</param>
    protected ExpressionIntroductionRewriter(
        ExpressionSyntax targetExpression,
        ExpressionSyntax replacement,
        SyntaxNode declaration,
        TContainer? targetContainer)
    {
        _targetExpression = targetExpression;
        _replacement = replacement;
        _declaration = declaration;
        _targetContainer = targetContainer;
    }

    /// <summary>
    /// Gets the declaration node that will be inserted when appropriate.
    /// </summary>
    protected SyntaxNode Declaration => _declaration;

    /// <summary>
    /// Gets the replacement expression used for occurrences of the target expression.
    /// </summary>
    protected ExpressionSyntax Replacement => _replacement;

    /// <summary>
    /// Gets the expression to search for and replace.
    /// </summary>
    protected ExpressionSyntax TargetExpression => _targetExpression;

    /// <inheritdoc />
    public override SyntaxNode Visit(SyntaxNode? node)
    {
        if (node is ExpressionSyntax expr && SyntaxFactory.AreEquivalent(expr, _targetExpression))
            return _replacement;

        return base.Visit(node)!;
    }

    /// <summary>
    /// Inserts the declaration into the provided container.
    /// </summary>
    protected abstract TContainer InsertDeclaration(TContainer container, SyntaxNode declaration);

    /// <summary>
    /// Conditionally inserts the declaration into the visited container when the condition holds.
    /// </summary>
    protected TContainer MaybeInsertDeclaration(TContainer original, TContainer visited, bool? condition = null)
    {
        bool shouldInsert = condition ?? (_targetContainer != null && ReferenceEquals(original, _targetContainer));
        if (shouldInsert)
            return InsertDeclaration(visited, _declaration);

        return visited;
    }
}