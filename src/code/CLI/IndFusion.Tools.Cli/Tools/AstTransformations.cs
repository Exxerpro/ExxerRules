namespace IndFusion.Tools.Cli.Tools;

/// <summary>
/// Small Roslyn AST transformation helpers used by CLI tools.
/// </summary>
public static class AstTransformations
{
    /// <summary>
    /// Adds a parameter to a method declaration.
    /// </summary>
    /// <param name="method">The method to modify.</param>
    /// <param name="name">Parameter name.</param>
    /// <param name="type">Parameter type (as C# type name).</param>
    /// <returns>The updated method declaration.</returns>
    public static MethodDeclarationSyntax AddParameter(MethodDeclarationSyntax method, string name, string type)
    {
        var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(name))
            .WithType(SyntaxFactory.ParseTypeName(type));
        return method.WithParameterList(method.ParameterList.AddParameters(parameter));
    }

    /// <summary>
    /// Replaces 'this' references in a method body with an identifier.
    /// </summary>
    /// <param name="method">The method to modify.</param>
    /// <param name="parameterName">Identifier to use instead of 'this'.</param>
    /// <returns>The updated method declaration.</returns>
    public static MethodDeclarationSyntax ReplaceThisReferences(MethodDeclarationSyntax method, string parameterName)
    {
        return method.ReplaceNodes(
            method.DescendantNodes().OfType<ThisExpressionSyntax>(),
            (_, _) => SyntaxFactory.IdentifierName(parameterName));
    }

    /// <summary>
    /// Qualifies unqualified instance member identifiers with a given parameter (semantic model aware).
    /// </summary>
    /// <param name="method">The method to modify.</param>
    /// <param name="parameterName">The qualifier identifier.</param>
    /// <param name="semanticModel">Semantic model for symbol resolution.</param>
    /// <param name="typeSymbol">The declaring type whose members should be qualified.</param>
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
    /// Qualifies specified instance member identifiers with a given parameter (string name based).
    /// </summary>
    /// <param name="method">The method to modify.</param>
    /// <param name="parameterName">The qualifier identifier.</param>
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
    /// Ensures a method declaration has the static modifier.
    /// </summary>
    /// <param name="method">The method to update.</param>
    /// <returns>The updated method declaration.</returns>
    public static MethodDeclarationSyntax EnsureStaticModifier(MethodDeclarationSyntax method)
    {
        var modifiers = method.Modifiers;
        if (!modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
        return method.WithModifiers(modifiers);
    }

    /// <summary>
    /// Adds an argument expression to an invocation.
    /// </summary>
    /// <param name="invocation">The invocation to modify.</param>
    /// <param name="argumentExpression">The argument expression to add.</param>
    /// <param name="generator">Syntax generator to construct the argument.</param>
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
    /// Removes an argument by index from an invocation.
    /// </summary>
    /// <param name="invocation">The invocation to modify.</param>
    /// <param name="argumentIndex">Zero-based index of the argument to remove.</param>
    /// <returns>The updated invocation.</returns>
    public static InvocationExpressionSyntax RemoveArgument(
        InvocationExpressionSyntax invocation,
        int argumentIndex)
    {
        var newArgs = invocation.ArgumentList.Arguments.RemoveAt(argumentIndex);
        return invocation.WithArgumentList(invocation.ArgumentList.WithArguments(newArgs));
    }
}
