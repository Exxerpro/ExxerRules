using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Computes simple method-level metrics and emits actionable refactoring suggestions.
/// </summary>
public class MethodMetricsWalker : CSharpSyntaxWalker
{
    private readonly SemanticModel? _model;
    /// <summary>Collected suggestions derived from method metrics.</summary>
    public List<string> Suggestions { get; } = new();

    /// <summary>
    /// Initializes a new instance using an optional semantic model for symbol analysis.
    /// </summary>
    /// <param name="model">The semantic model used to resolve symbol information.</param>
    public MethodMetricsWalker(SemanticModel? model)
    {
        _model = model;
    }

    /// <summary>
    /// Visits method declarations and computes basic metrics such as length and parameter count.
    /// </summary>
    /// <param name="node">The method declaration node.</param>
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        base.VisitMethodDeclaration(node);

        var span = node.GetLocation().GetLineSpan();
        var lines = span.EndLinePosition.Line - span.StartLinePosition.Line + 1;
        if (lines > 30)
            Suggestions.Add($"Method '{node.Identifier}' is {lines} lines long -> consider extract-method");

        var parameters = node.ParameterList.Parameters.Count;
        if (parameters >= 5)
            Suggestions.Add($"Method '{node.Identifier}' has {parameters} parameters -> consider introducing parameter object");

        if (_model != null && !node.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            var accessesInstance = node.DescendantNodes().Any(n => n is ThisExpressionSyntax ||
                                                                   n is MemberAccessExpressionSyntax ma && _model.GetSymbolInfo(ma).Symbol is { IsStatic: false });
            if (!accessesInstance)
                Suggestions.Add($"Method '{node.Identifier}' does not access instance state -> make-static");
        }
    }
}
