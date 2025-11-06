namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Semantic diff analysis.
/// </summary>
/// <param name="StructuralChanges">Structural changes detected.</param>
/// <param name="BehavioralChanges">Behavioral changes detected.</param>
/// <param name="ImpactAnalysis">Analysis of the impact of changes.</param>
/// <param name="ConfidenceScore">Confidence in the analysis (0.0-1.0).</param>
public record SemanticDiffAnalysis(
    IEnumerable<StructuralChange> StructuralChanges,
    IEnumerable<BehavioralChange> BehavioralChanges,
    ImpactAnalysis ImpactAnalysis,
    double ConfidenceScore
);