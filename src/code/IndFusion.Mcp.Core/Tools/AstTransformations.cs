using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace IndFusion.Mcp.Mcp.Core.Tools;

/// <summary>
/// AST helpers for small, composable syntax transformations used by refactoring tools.
/// </summary>
public static class AstTransformations
{
    /// <summary>
    /// Adds a parameter to a method declaration.
    /// </summary>
    /// <param name="method">The method to modify.</param>
    /// <param name="name">Parameter name.</param>
    /// <param name="type">Parameter type.</param>
    /// <returns>The updated method declaration.</returns>
    public static MethodDeclarationSyntax AddParameter(MethodDeclarationSyntax method, string name, string type)
    {
        var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(name))
            .WithType(SyntaxFactory.ParseTypeName(type));
        return method.WithParameterList(method.ParameterList.AddParameters(parameter));
    }

    /// <summary>
    /// Replaces 'this' expressions with an identifier for an injected instance parameter.
    /// </summary>
    /// <param name="method">The method to rewrite.</param>
    /// <param name="parameterName">The instance parameter name.</param>
    /// <returns>The updated method declaration.</returns>
    public static MethodDeclarationSyntax ReplaceThisReferences(MethodDeclarationSyntax method, string parameterName)
    {
        return method.ReplaceNodes(
            method.DescendantNodes().OfType<ThisExpressionSyntax>(),
            (_, _) => SyntaxFactory.IdentifierName(parameterName));
    }

    /// <summary>
    /// Qualifies instance member usages with the provided instance parameter using semantic model and type symbol.
    /// </summary>
    /// <param name="method">The method to rewrite.</param>
    /// <param name="parameterName">The instance parameter name.</param>
    /// <param name="semanticModel">Semantic model for symbol resolution.</param>
    /// <param name="typeSymbol">The enclosing type symbol.</param>
    /// <returns>The updated method declaration.</returns>
    public static MethodDeclarationSyntax QualifyInstanceMembers(MethodDeclarationSyntax method, string parameterName, SemanticModel semanticModel, INamedTypeSymbol typeSymbol)
    {
        return method.ReplaceNodes(
            method.DescendantNodes().OfType<IdentifierNameSyntax>().Where(id =>
            {
                var sym = semanticModel.GetSymbolInfo(id).Symbol;
                return sym is IFieldSymbol or IPropertySymbol or IMethodSymbol &&
                       SymbolEqualityComparer.Default.Equals(sym.ContainingType, typeSymbol) &&
                       !sym.IsStatic && id.Parent is not MemberAccessExpressionSyntax;
            }),
            (old, _) => SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(parameterName),
                SyntaxFactory.IdentifierName(old.Identifier)));
    }

    /// <summary>
    /// Qualifies instance member usages with the provided instance parameter using a known member name set.
    /// </summary>
    /// <param name="method">The method to rewrite.</param>
    /// <param name="parameterName">The instance parameter name.</param>
    /// <param name="members">Set of member names to qualify.</param>
    /// <returns>The updated method declaration.</returns>
    public static MethodDeclarationSyntax QualifyInstanceMembers(MethodDeclarationSyntax method, string parameterName, HashSet<string> members)
    {
        return method.ReplaceNodes(
            method.DescendantNodes().OfType<IdentifierNameSyntax>().Where(id =>
                members.Contains(id.Identifier.ValueText) && id.Parent is not MemberAccessExpressionSyntax),
            (old, _) => SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(parameterName),
                SyntaxFactory.IdentifierName(old.Identifier)));
    }

    /// <summary>
    /// Ensures that a method has the static modifier.
    /// </summary>
    /// <param name="method">The method declaration.</param>
    /// <returns>The updated method declaration.</returns>
    public static MethodDeclarationSyntax EnsureStaticModifier(MethodDeclarationSyntax method)
    {
        var modifiers = method.Modifiers;
        if (!modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
        return method.WithModifiers(modifiers);
    }

    /// <summary>
    /// Adds a new argument to an invocation expression.
    /// </summary>
    /// <param name="invocation">The invocation to modify.</param>
    /// <param name="argumentExpression">Expression to use as the argument.</param>
    /// <param name="generator">Syntax generator utility.</param>
    /// <returns>The updated invocation.</returns>
    public static InvocationExpressionSyntax AddArgument(
        InvocationExpressionSyntax invocation,
        ExpressionSyntax argumentExpression,
        SyntaxGenerator generator)
    {
        var argument = (ArgumentSyntax)generator.Argument(argumentExpression);
        return invocation.WithArgumentList(invocation.ArgumentList.AddArguments(argument));
    }

    /// <summary>
    /// Removes an argument at the specified index from an invocation expression.
    /// </summary>
    /// <param name="invocation">The invocation to modify.</param>
    /// <param name="argumentIndex">Zero-based argument index to remove.</param>
    /// <returns>The updated invocation.</returns>
    public static InvocationExpressionSyntax RemoveArgument(
        InvocationExpressionSyntax invocation,
        int argumentIndex)
    {
        var newArgs = invocation.ArgumentList.Arguments.RemoveAt(argumentIndex);
        return invocation.WithArgumentList(invocation.ArgumentList.WithArguments(newArgs));
    }
}
