using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Services;

/// <summary>
/// Result of comprehensive search operations.
/// </summary>
/// <param name="SearchResults">The search results.</param>
/// <param name="TotalCount">Total number of results found.</param>
/// <param name="Query">The original query.</param>
/// <param name="ProcessingTimeMs">Time taken for processing in milliseconds.</param>
/// <param name="ExtractedKnowledge">Knowledge extracted from results.</param>
/// <param name="AdditionalContext">Additional context retrieved.</param>
/// <param name="SearchSuggestions">Search suggestions for refinement.</param>
public readonly record struct ComprehensiveSearchResult(
    IReadOnlyList<SemanticSearchResult> SearchResults,
    int TotalCount,
    string Query,
    long ProcessingTimeMs,
    IReadOnlyList<IndFusion.SemanticRag.Domain.Models.KnowledgeExtractionResult> ExtractedKnowledge,
    SemanticContext? AdditionalContext,
    IReadOnlyList<string>? SearchSuggestions = null);