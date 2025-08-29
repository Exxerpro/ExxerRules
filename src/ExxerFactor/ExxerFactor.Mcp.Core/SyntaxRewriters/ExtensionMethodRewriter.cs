using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ExxerFactor.Mcp.Core.Tools;

namespace ExxerFactor.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewrites a method into an extension method by introducing a 'this' parameter and qualifying member accesses.
/// </summary>
public class ExtensionMethodRewriter : CSharpSyntaxRewriter
{
    private readonly string _parameterName;
    private readonly string _parameterType;
    private readonly SemanticModel? _semanticModel;
    private readonly INamedTypeSymbol? _typeSymbol;
    private readonly HashSet<string>? _knownMembers;

    /// <summary>
    /// Initializes a new instance using semantic model metadata for qualification.
    /// </summary>
    public ExtensionMethodRewriter(string parameterName, string parameterType, SemanticModel semanticModel, INamedTypeSymbol typeSymbol)
    {
        _parameterName = parameterName;
        _parameterType = parameterType;
        _semanticModel = semanticModel;
        _typeSymbol = typeSymbol;
    }

    /// <summary>
    /// Initializes a new instance using a predefined set of known instance member names for qualification.
    /// </summary>
    public ExtensionMethodRewriter(string parameterName, string parameterType, HashSet<string> knownMembers)
    {
        _parameterName = parameterName;
        _parameterType = parameterType;
        _knownMembers = knownMembers;
    }

    /// <summary>
    /// Rewrites the specified method declaration as an extension method.
    /// </summary>
    public MethodDeclarationSyntax Rewrite(MethodDeclarationSyntax method)
    {
        return (MethodDeclarationSyntax)Visit(method)!;
    }

    /// <inheritdoc />
    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var thisParam = SyntaxFactory.Parameter(SyntaxFactory.Identifier(_parameterName))
            .WithType(SyntaxFactory.ParseTypeName(_parameterType))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.ThisKeyword));

        var parameters = node.ParameterList.Parameters.Insert(0, thisParam);
        var updated = node.WithParameterList(node.ParameterList.WithParameters(parameters));
        updated = AstTransformations.EnsureStaticModifier(updated);

        // Remove explicit interface specifier when converting to an extension method
        if (updated.ExplicitInterfaceSpecifier != null)
        {
            updated = updated.WithExplicitInterfaceSpecifier(null)
                .WithIdentifier(updated.Identifier.WithoutTrivia())
                .WithTriviaFrom(updated);
        }

        return base.VisitMethodDeclaration(updated)!;
    }

    /// <inheritdoc />
    public override SyntaxNode VisitThisExpression(ThisExpressionSyntax node)
    {
        return SyntaxFactory.IdentifierName(_parameterName).WithTriviaFrom(node);
    }

    /// <inheritdoc />
    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        bool qualify = false;

        if (_semanticModel != null)
        {
            var sym = _semanticModel.GetSymbolInfo(node).Symbol;
            if (sym is IFieldSymbol or IPropertySymbol or IMethodSymbol &&
                SymbolEqualityComparer.Default.Equals(sym.ContainingType, _typeSymbol) &&
                !sym.IsStatic && node.Parent is not MemberAccessExpressionSyntax)
            {
                qualify = true;
            }
        }
        else if (_knownMembers != null &&
                 _knownMembers.Contains(node.Identifier.ValueText) &&
                 node.Parent is not MemberAccessExpressionSyntax)
        {
            qualify = true;
        }

        if (qualify)
        {
            return SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(_parameterName),
                    node.WithoutTrivia())
                .WithTriviaFrom(node);
        }

        return base.VisitIdentifierName(node);
    }
}