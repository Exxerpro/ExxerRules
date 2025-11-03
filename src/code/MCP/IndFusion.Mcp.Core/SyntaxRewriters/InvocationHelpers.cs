using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Core.SyntaxRewriters;

/// <summary>
/// Helper methods for inspecting and classifying invocation expressions.
/// </summary>
public static class InvocationHelpers
{
    /// <summary>
    /// Gets the simple method name invoked by the specified invocation expression, when available.
    /// </summary>
    public static string? GetInvokedMethodName(InvocationExpressionSyntax node)
        => node.Expression switch
        {
            IdentifierNameSyntax id => id.Identifier.ValueText,
            MemberAccessExpressionSyntax ma when ma.Name is IdentifierNameSyntax id => id.Identifier.ValueText,
            _ => null
        };

    /// <summary>
    /// Determines whether the invocation targets a method with the given name.
    /// </summary>
    public static bool IsInvocationOf(InvocationExpressionSyntax node, string methodName)
        => GetInvokedMethodName(node) == methodName;

    /// <summary>
    /// Determines whether the invocation is a base call to a method with the given name.
    /// </summary>
    public static bool IsBaseInvocationOf(InvocationExpressionSyntax node, string methodName)
        => node.Expression is MemberAccessExpressionSyntax { Expression: BaseExpressionSyntax, Name: IdentifierNameSyntax id } && id.Identifier.ValueText == methodName;
}
