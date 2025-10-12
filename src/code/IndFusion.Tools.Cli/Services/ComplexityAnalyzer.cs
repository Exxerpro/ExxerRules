namespace IndFusion.Tools.Cli.Services;

/// <summary>
/// Analyzer for detecting complex methods
/// </summary>
public class ComplexityAnalyzer : CSharpSyntaxWalker
{
    /// <summary>
    /// Gets the list of complex methods
    /// </summary>
    public List<ComplexMethod> ComplexMethods { get; } = new();

    /// <summary>
    /// Visits a method declaration
    /// </summary>
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var complexity = CalculateComplexity(node);
        if (complexity > 10) // Threshold for complex methods
        {
            ComplexMethods.Add(new ComplexMethod
            {
                Name = node.Identifier.ValueText,
                Line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                Complexity = complexity
            });
        }

        base.VisitMethodDeclaration(node);
    }

    /// <summary>
    /// Calculates complexity for a method
    /// </summary>
    private static int CalculateComplexity(SyntaxNode node)
    {
        var complexity = 1; // Base complexity

        // Count decision points
        complexity += node.DescendantNodes().OfType<IfStatementSyntax>().Count();
        complexity += node.DescendantNodes().OfType<WhileStatementSyntax>().Count();
        complexity += node.DescendantNodes().OfType<ForStatementSyntax>().Count();
        complexity += node.DescendantNodes().OfType<ForEachStatementSyntax>().Count();
        complexity += node.DescendantNodes().OfType<SwitchStatementSyntax>().Count();
        complexity += node.DescendantNodes().OfType<ConditionalExpressionSyntax>().Count();
        complexity += node.DescendantNodes().OfType<CatchClauseSyntax>().Count();

        return complexity;
    }
}