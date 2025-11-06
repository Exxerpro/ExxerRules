using Microsoft.CodeAnalysis;

namespace IndFusion.Mcp.Core.SyntaxWalkers;

/// <summary>
/// Aggregates several analysis walkers to identify refactoring opportunities and produce suggestions.
/// </summary>
public class ExxerFactoringOpportunityWalker
{
    private readonly MethodMetricsWalker _methodMetrics;
    private readonly ClassMetricsWalker _classMetrics;
    private readonly UnusedMembersWalker _unusedMembers;
    private readonly UseInterfaceWalker _useInterface;

    /// <summary>
    /// Gets the generated list of refactoring suggestions.
    /// </summary>
    public List<string> Suggestions { get; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ExxerFactoringOpportunityWalker"/> class.
    /// </summary>
    /// <param name="model">Optional semantic model for analysis.</param>
    /// <param name="solution">Optional solution context used by some analyses.</param>
    public ExxerFactoringOpportunityWalker(SemanticModel? model = null, Solution? solution = null)
    {
        _methodMetrics = new MethodMetricsWalker(model);
        _classMetrics = new ClassMetricsWalker();
        _unusedMembers = new UnusedMembersWalker(model, solution);
        _useInterface = new UseInterfaceWalker(model);
    }

    /// <summary>
    /// Visits the provided syntax root with all underlying analyzers.
    /// </summary>
    /// <param name="root">The syntax root to analyze.</param>
    public void Visit(SyntaxNode root)
    {
        _methodMetrics.Visit(root);
        _classMetrics.Visit(root);
        _unusedMembers.Visit(root);
        _useInterface.Visit(root);
    }

    /// <summary>
    /// Performs any asynchronous post-processing and aggregates suggestions.
    /// </summary>
    public async Task PostProcessAsync()
    {
        await _unusedMembers.PostProcessAsync();
        Suggestions.AddRange(_methodMetrics.Suggestions);
        Suggestions.AddRange(_classMetrics.Suggestions);
        Suggestions.AddRange(_unusedMembers.Suggestions);
        Suggestions.AddRange(_useInterface.Suggestions);
    }
}
