using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;

namespace IndFusion.SemanticRag.Application.Services;

/// <summary>
/// Options for question answering operations.
/// </summary>
/// <param name="SearchOptions">Search options for finding relevant context.</param>
/// <param name="RagConfig">RAG configuration for context retrieval.</param>
/// <param name="MaxContextDocuments">Maximum number of documents to use as context.</param>
/// <param name="IncludeEntityContext">Whether to include entity context.</param>
/// <param name="IncludeRelationshipContext">Whether to include relationship context.</param>
public readonly record struct QuestionAnswerOptions(
    SemanticSearchOptions SearchOptions,
    SemanticRagConfig RagConfig,
    int MaxContextDocuments = 10,
    bool IncludeEntityContext = true,
    bool IncludeRelationshipContext = true);