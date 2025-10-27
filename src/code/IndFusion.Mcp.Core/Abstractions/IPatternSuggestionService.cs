namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Defines the contract for pattern suggestion services that provide remediation strategies
/// with confidence scores and citations from the knowledge base.
/// </summary>
public interface IPatternSuggestionService
{
    /// <summary>
    /// Suggests remediation strategies for a specific violation with confidence scores and citations.
    /// </summary>
    /// <param name="request">The pattern suggestion request containing violation details and context.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing suggested patterns, confidence scores, and source citations.</returns>
    Task<PatternSuggestionResult> SuggestPatternsAsync(PatternSuggestionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes code or project for alignment with pattern families and suggests improvements.
    /// </summary>
    /// <param name="request">The pattern analysis request containing project path and pattern type.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing pattern analysis, alignment scores, and improvement suggestions.</returns>
    Task<PatternAnalysisResult> AnalyzePatternsAsync(PatternAnalysisRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries the pattern knowledge graph for relationships, dependencies, and violation clusters.
    /// </summary>
    /// <param name="request">The graph query request containing query filters and repository scope.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing graph traversal results, relationships, and pattern insights.</returns>
    Task<PatternGraphResult> QueryPatternGraphAsync(PatternGraphRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts patterns from existing code and adds them to the knowledge base.
    /// </summary>
    /// <param name="request">The pattern extraction request containing source code and metadata.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result indicating successful pattern extraction and knowledge base updates.</returns>
    Task<PatternExtractionResult> ExtractPatternsAsync(PatternExtractionRequest request, CancellationToken cancellationToken = default);
}