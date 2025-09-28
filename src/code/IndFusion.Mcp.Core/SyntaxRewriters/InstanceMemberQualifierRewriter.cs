using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Rewriter that qualifies instance member accesses with a provided parameter name.
/// Useful when converting instance methods into static/extension methods.
/// </summary>
public class InstanceMemberQualifierRewriter : CSharpSyntaxRewriter
{
    private readonly string _parameterName;
    private readonly SemanticModel? _semanticModel;
    private readonly INamedTypeSymbol? _typeSymbol;
    private readonly HashSet<string>? _knownMembers;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstanceMemberQualifierRewriter"/> class.
    /// </summary>
    /// <param name="parameterName">The parameter name to use for qualification.</param>
    /// <param name="semanticModel">Optional semantic model for precise symbol checks.</param>
    /// <param name="typeSymbol">Optional containing type used for hierarchy checks.</param>
    /// <param name="knownMembers">Optional set of known instance member names.</param>
    public InstanceMemberQualifierRewriter(
        string parameterName,
        SemanticModel? semanticModel = null,
        INamedTypeSymbol? typeSymbol = null,
        HashSet<string>? knownMembers = null)
    {
        _parameterName = parameterName;
        _semanticModel = semanticModel;
        _typeSymbol = typeSymbol;
        _knownMembers = knownMembers;
    }

    /// <inheritdoc />
    public override SyntaxNode VisitThisExpression(ThisExpressionSyntax node)
        => SyntaxFactory.IdentifierName(_parameterName).WithTriviaFrom(node);

    /// <inheritdoc />
    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        var parent = node.Parent;
        if (parent is ParameterSyntax or TypeSyntax)
            return base.VisitIdentifierName(node);

        bool qualify = false;
        if (_semanticModel != null && _typeSymbol != null)
        {
            var sym = _semanticModel.GetSymbolInfo(node).Symbol;
            if (sym is IFieldSymbol or IPropertySymbol or IMethodSymbol &&
                !sym.IsStatic && parent is not MemberAccessExpressionSyntax &&
                sym.ContainingType is INamedTypeSymbol ct &&
                IsInTypeHierarchy(ct))
            {
                qualify = true;
            }
        }
        else if (_knownMembers != null &&
                 _knownMembers.Contains(node.Identifier.ValueText) &&
                 parent is not MemberAccessExpressionSyntax)
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

    private bool IsInTypeHierarchy(INamedTypeSymbol containing)
    {
        if (_typeSymbol == null)
            return false;

        var current = _typeSymbol;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, containing))
                return true;
            current = current.BaseType;
        }

        foreach (var iface in _typeSymbol.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(iface, containing))
                return true;
        }

        return false;
    }
}
