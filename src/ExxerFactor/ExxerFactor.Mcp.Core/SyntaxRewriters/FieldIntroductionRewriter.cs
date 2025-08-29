using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ExxerFactor.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Introduces a field declaration into a class and replaces a target expression with a reference to that field.
/// </summary>
public class FieldIntroductionRewriter : ExpressionIntroductionRewriter<ClassDeclarationSyntax>
{
    private readonly FieldDeclarationSyntax _fieldDeclaration;

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldIntroductionRewriter"/> class.
    /// </summary>
    /// <param name="targetExpression">The expression to replace.</param>
    /// <param name="fieldReference">The identifier that will reference the introduced field.</param>
    /// <param name="fieldDeclaration">The field declaration to insert.</param>
    /// <param name="containingClass">Optional targeted class where the field should be inserted.</param>
    public FieldIntroductionRewriter(
        ExpressionSyntax targetExpression,
        IdentifierNameSyntax fieldReference,
        FieldDeclarationSyntax fieldDeclaration,
        ClassDeclarationSyntax? containingClass)
        : base(targetExpression, fieldReference, fieldDeclaration, containingClass)
    {
        _fieldDeclaration = fieldDeclaration;
    }

    /// <summary>
    /// Inserts the field declaration at the top of the class members.
    /// </summary>
    protected override ClassDeclarationSyntax InsertDeclaration(ClassDeclarationSyntax node, SyntaxNode declaration)
    {
        return node.WithMembers(node.Members.Insert(0, (MemberDeclarationSyntax)declaration));
    }

    /// <inheritdoc />
    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var rewritten = (ClassDeclarationSyntax)base.VisitClassDeclaration(node)!;
        return MaybeInsertDeclaration(node, rewritten);
    }
}