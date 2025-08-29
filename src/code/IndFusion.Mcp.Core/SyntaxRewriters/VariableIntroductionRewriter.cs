using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Introduces a local variable declaration into a block and replaces occurrences of a target expression with the variable reference.
/// </summary>
public class VariableIntroductionRewriter : ExpressionIntroductionRewriter<BlockSyntax>
{
    private readonly StatementSyntax? _containingStatement;
    private readonly int _insertIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableIntroductionRewriter"/> class.
    /// </summary>
    /// <param name="targetExpression">The expression to replace with the variable reference.</param>
    /// <param name="variableReference">The identifier that references the introduced variable.</param>
    /// <param name="variableDeclaration">The local variable declaration to insert.</param>
    /// <param name="containingStatement">Optional statement before which the declaration should be inserted.</param>
    /// <param name="containingBlock">Optional containing block where the declaration will be inserted.</param>
    public VariableIntroductionRewriter(
        ExpressionSyntax targetExpression,
        IdentifierNameSyntax variableReference,
        LocalDeclarationStatementSyntax variableDeclaration,
        StatementSyntax? containingStatement,
        BlockSyntax? containingBlock)
        : base(targetExpression, variableReference, variableDeclaration, containingBlock)
    {
        _containingStatement = containingStatement;
        _insertIndex = containingBlock != null && containingStatement != null
            ? containingBlock.Statements.IndexOf(containingStatement)
            : -1;
    }

    /// <inheritdoc />
    protected override BlockSyntax InsertDeclaration(BlockSyntax node, SyntaxNode declaration)
    {
        if (_insertIndex >= 0)
            return node.WithStatements(node.Statements.Insert(_insertIndex, (StatementSyntax)declaration));
        return node;
    }

    /// <inheritdoc />
    public override SyntaxNode Visit(SyntaxNode? node)
    {
        if (node is ExpressionSyntax expr && SyntaxFactory.AreEquivalent(expr, TargetExpression))
            return Replacement;

        return base.Visit(node)!;
    }

    /// <inheritdoc />
    public override SyntaxNode VisitBlock(BlockSyntax node)
    {
        var rewritten = (BlockSyntax)base.VisitBlock(node)!;
        return MaybeInsertDeclaration(node, rewritten);
    }
}
