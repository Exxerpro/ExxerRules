using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;
using Microsoft.Extensions.Logging;
using IndQuestResults;

namespace IndFusion.SemanticRag.Application.Services;

/// <summary>
/// Orchestrates semantic RAG operations by coordinating domain services.
/// </summary>
public class SemanticRagOrchestrationService
{
    private readonly ISemanticRagService _semanticRagService;
    private readonly ISemanticSearchService _searchService;
    private readonly IDocumentIngestionService _ingestionService;
    private readonly IKnowledgeExtractionService _extractionService;
    private readonly ILogger<SemanticRagOrchestrationService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticRagOrchestrationService"/> class.
    /// </summary>
    /// <param name="semanticRagService">The semantic RAG service.</param>
    /// <param name="searchService">The semantic search service.</param>
    /// <param name="ingestionService">The document ingestion service.</param>
    /// <param name="extractionService">The knowledge extraction service.</param>
    /// <param name="logger">The logger.</param>
    public SemanticRagOrchestrationService(
        ISemanticRagService semanticRagService,
        ISemanticSearchService searchService,
        IDocumentIngestionService ingestionService,
        IKnowledgeExtractionService extractionService,
        ILogger<SemanticRagOrchestrationService> logger)
    {
        _semanticRagService = semanticRagService ?? throw new ArgumentNullException(nameof(semanticRagService));
        _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        _ingestionService = ingestionService ?? throw new ArgumentNullException(nameof(ingestionService));
        _extractionService = extractionService ?? throw new ArgumentNullException(nameof(extractionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Performs comprehensive semantic search with knowledge extraction.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="options">Search options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the comprehensive search results.</returns>
    public async Task<Result<ComprehensiveSearchResult>> ComprehensiveSearchAsync(
        string query,
        ComprehensiveSearchOptions options,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting comprehensive search for query: {Query}", query);

        try
        {
            // Perform semantic search
            var searchResult = await _searchService.SearchAsync(
                query,
                options.SearchOptions,
                cancellationToken);

            if (searchResult.IsFailure)
            {
                _logger.LogWarning("Semantic search failed: {Error}", searchResult.Error);
                return Result<ComprehensiveSearchResult>.WithFailure(searchResult.Error!);
            }

            var searchResponse = searchResult.Value;

            // Extract knowledge from top results if enabled
            var extractedKnowledge = new List<IndFusion.SemanticRag.Domain.Models.KnowledgeExtractionResult>();
            if (options.EnableKnowledgeExtraction && searchResponse.Results != null && searchResponse.Results.Any())
            {
                var topResults = searchResponse.Results.Take(options.MaxResultsForExtraction);
                foreach (var result in topResults)
                {
                    if (result.Document == null) continue;
                    
                    var extractionResult = await _extractionService.ExtractKnowledgeAsync(
                        result.Document,
                        options.ExtractionOptions,
                        cancellationToken);

                    if (extractionResult.IsSuccess)
                    {
                        // Convert from Services version to Models version
                        var servicesResult = extractionResult.Value;
                        var modelsResult = new IndFusion.SemanticRag.Domain.Models.KnowledgeExtractionResult(
                            DocumentId: result.Document.Id,
                            Entities: servicesResult.Entities ?? [],
                            Relationships: servicesResult.Relationships ?? [],
                            Summary: $"Extracted {servicesResult.Entities?.Count ?? 0} entities and {servicesResult.Relationships?.Count ?? 0} relationships",
                            Confidence: servicesResult.Confidence,
                            Metadata: new Dictionary<string, object>
                            {
                                ["ProcessingTimeMs"] = servicesResult.ProcessingTimeMs,
                                ["CodeEntitiesCount"] = servicesResult.CodeEntities?.Count ?? 0,
                                ["ConceptsCount"] = servicesResult.Concepts?.Count ?? 0
                            }
                        );
                        extractedKnowledge.Add(modelsResult);
                    }
                }
            }

            // Get additional context if enabled
            SemanticContext? additionalContext = null;
            if (options.EnableContextRetrieval)
            {
                var contextResult = await _semanticRagService.GetContextAsync(
                    query,
                    options.RagConfig,
                    cancellationToken);

                if (contextResult.IsSuccess)
                {
                    additionalContext = contextResult.Value;
                }
            }

            var comprehensiveResult = new ComprehensiveSearchResult(
                SearchResults: searchResponse.Results ?? [],
                TotalCount: searchResponse.TotalCount,
                Query: query,
                ProcessingTimeMs: searchResponse.ProcessingTimeMs,
                ExtractedKnowledge: extractedKnowledge,
                AdditionalContext: additionalContext,
                SearchSuggestions: searchResponse.Suggestions);

            _logger.LogInformation("Comprehensive search completed successfully. Found {Count} results in {Time}ms",
                comprehensiveResult.TotalCount, comprehensiveResult.ProcessingTimeMs);

            return Result<ComprehensiveSearchResult>.Success(comprehensiveResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during comprehensive search for query: {Query}", query);
            return Result<ComprehensiveSearchResult>.WithFailure($"Search failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Ingests and processes a repository for semantic RAG.
    /// </summary>
    /// <param name="repositoryPath">Path to the repository.</param>
    /// <param name="config">Repository ingestion configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the ingestion results.</returns>
    public async Task<Result<RepositoryIngestionResult>> IngestRepositoryAsync(
        string repositoryPath,
        IndFusion.SemanticRag.Domain.Models.RepositoryIngestionConfig config,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting repository ingestion for path: {Path}", repositoryPath);

        try
        {
            // Validate configuration
            var validationResult = config.Validate();
            if (validationResult.IsFailure)
            {
                return Result<RepositoryIngestionResult>.WithFailure(validationResult.Error!);
            }

            // Ingest documents
            // Map from Models.RepositoryIngestionConfig to Services.RepositoryIngestionConfig
            // Note: DocumentIngestionOptions doesn't have ExtractCodeEntities/ExtractComments/ProcessDependencies,
            // so we use defaults (true) or check if EnableEntityExtraction is enabled
            var servicesConfig = new IndFusion.SemanticRag.Domain.Services.RepositoryIngestionConfig(
                IncludePatterns: config.IncludePatterns ?? [],
                ExcludePatterns: config.ExcludePatterns ?? [],
                MaxFileSize: config.MaxFileSize,
                ExtractCodeEntities: config.IngestionOptions?.EnableEntityExtraction ?? true,
                ExtractComments: config.IngestionOptions?.EnableEntityExtraction ?? true,
                ProcessDependencies: config.IngestionOptions?.EnableKnowledgeGraph ?? true,
                MaxDepth: config.MaxDepth
            );
            var documentsResult = await _ingestionService.IngestRepositoryAsync(
                repositoryPath,
                servicesConfig,
                cancellationToken);

            if (documentsResult.IsFailure)
            {
                _logger.LogWarning("Document ingestion failed: {Error}", documentsResult.Error);
                return Result<RepositoryIngestionResult>.WithFailure(documentsResult.Error!);
            }

            var documents = documentsResult.Value ?? [];
            var processedDocuments = new List<SemanticDocument>();
            var extractedKnowledge = new List<IndFusion.SemanticRag.Domain.Models.KnowledgeExtractionResult>();

            // Process each document
            foreach (var document in documents)
            {
                // Index the document
                var indexResult = await _semanticRagService.IndexDocumentAsync(document, cancellationToken);
                if (indexResult.IsSuccess)
                {
                    processedDocuments.Add(document);
                }

                // Extract knowledge if enabled
                // TODO: Fix RepositoryIngestionConfig properties
                if (true) // config.ExtractCodeEntities || config.ExtractComments
                {
                    var extractionResult = await _extractionService.ExtractKnowledgeAsync(
                        document,
                        ComprehensiveExtractionOptions.Default(),
                        cancellationToken);

                    if (extractionResult.IsSuccess)
                    {
                        // Convert from Services version to Models version
                        var servicesResult = extractionResult.Value;
                        var modelsResult = new IndFusion.SemanticRag.Domain.Models.KnowledgeExtractionResult(
                            DocumentId: document.Id,
                            Entities: servicesResult.Entities,
                            Relationships: servicesResult.Relationships,
                            Summary: $"Extracted {servicesResult.Entities.Count} entities and {servicesResult.Relationships.Count} relationships",
                            Confidence: servicesResult.Confidence,
                            Metadata: new Dictionary<string, object>
                            {
                                ["ProcessingTimeMs"] = servicesResult.ProcessingTimeMs,
                                ["CodeEntitiesCount"] = servicesResult.CodeEntities.Count,
                                ["ConceptsCount"] = servicesResult.Concepts.Count
                            }
                        );
                        extractedKnowledge.Add(modelsResult);

                        // Add extracted entities to the knowledge graph
                        foreach (var entity in servicesResult.Entities)
                        {
                            await _semanticRagService.AddEntityAsync(entity, cancellationToken);
                        }

                        // Add extracted relationships
                        foreach (var relationship in servicesResult.Relationships)
                        {
                            await _semanticRagService.CreateRelationshipAsync(relationship, cancellationToken);
                        }
                    }
                }
            }

            var result = new RepositoryIngestionResult(
                ProcessedDocuments: processedDocuments,
                TotalDocuments: documents.Count,
                ExtractedKnowledge: extractedKnowledge,
                ProcessingTimeMs: 0, // TODO: Calculate actual processing time
                Success: true);

            _logger.LogInformation("Repository ingestion completed successfully. Processed {Count} documents",
                result.TotalDocuments);

            return Result<RepositoryIngestionResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during repository ingestion for path: {Path}", repositoryPath);
            return Result<RepositoryIngestionResult>.WithFailure($"Repository ingestion failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Performs intelligent question answering using semantic RAG.
    /// </summary>
    /// <param name="question">The question to answer.</param>
    /// <param name="options">QA options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the answer and supporting context.</returns>
    public async Task<Result<QuestionAnswerResult>> AnswerQuestionAsync(
        string question,
        QuestionAnswerOptions options,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting question answering for: {Question}", question);

        try
        {
            // Get relevant context
            var contextResult = await _semanticRagService.GetContextAsync(
                question,
                options.RagConfig,
                cancellationToken);

            if (contextResult.IsFailure)
            {
                return Result<QuestionAnswerResult>.WithFailure(contextResult.Error!);
            }

            var context = contextResult.Value;

            // Perform semantic search for additional context
            var searchResult = await _searchService.SearchAsync(
                question,
                options.SearchOptions,
                cancellationToken);

            if (searchResult.IsFailure)
            {
                return Result<QuestionAnswerResult>.WithFailure(searchResult.Error!);
            }

            var searchResponse = searchResult.Value;

            // Combine context and search results
#pragma warning disable CS8602 // Dereference of a possibly null reference
            var contextDocuments = context.Documents ?? Enumerable.Empty<SemanticDocument>();
#pragma warning restore CS8602
            var searchResults = searchResponse.Results ?? Enumerable.Empty<SemanticSearchResult>();
            var searchDocuments = searchResults.Where(r => r.Document != null).Select(r => r.Document!);
            var allDocuments = contextDocuments.Concat(searchDocuments).ToList();
            var allEntities = (context.Entities ?? Enumerable.Empty<KnowledgeEntity>()).ToList();
            var allRelationships = (context.Relationships ?? Enumerable.Empty<EntityRelationship>()).Select(r => new KnowledgeRelationship(
                Id: r.Id,
                FromNodeId: r.SourceEntityId,
                ToNodeId: r.TargetEntityId,
                RelationshipType: r.RelationshipType,
                Properties: r.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                CreatedAt: DateTimeOffset.UtcNow
            )).ToList();

            // Generate answer (this would typically involve an LLM)
            var answer = GenerateAnswer(question, allDocuments, allEntities, options);

            var result = new QuestionAnswerResult(
                Question: question,
                Answer: answer,
                SupportingDocuments: allDocuments,
                SupportingEntities: allEntities,
                SupportingRelationships: allRelationships,
                Confidence: CalculateAnswerConfidence(answer, allDocuments),
                ProcessingTimeMs: searchResponse.ProcessingTimeMs);

            _logger.LogInformation("Question answering completed successfully");

            return Result<QuestionAnswerResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during question answering for: {Question}", question);
            return Result<QuestionAnswerResult>.WithFailure($"Question answering failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets system statistics and health information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing system statistics.</returns>
    public async Task<Result<SystemHealthResult>> GetSystemHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var statsResult = await _semanticRagService.GetStatsAsync(cancellationToken);
            if (statsResult.IsFailure)
            {
                return Result<SystemHealthResult>.WithFailure(statsResult.Error!);
            }

            var stats = statsResult.Value;
            var health = new SystemHealthResult(
                IsHealthy: stats.TotalDocuments > 0,
                TotalDocuments: stats.TotalDocuments,
                TotalEntities: stats.TotalEntities,
                TotalRelationships: stats.TotalRelationships,
                LastIndexedAt: stats.LastIndexedAt,
                AverageDocumentSize: stats.AverageDocumentSize,
                EmbeddingDimension: stats.EmbeddingDimension,
                HealthScore: CalculateHealthScore(stats));

            return Result<SystemHealthResult>.Success(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system health");
            return Result<SystemHealthResult>.WithFailure($"Health check failed: {ex.Message}");
        }
    }

    private string GenerateAnswer(string question, IReadOnlyList<SemanticDocument> documents, 
        IReadOnlyList<KnowledgeEntity> entities, QuestionAnswerOptions options)
    {
        // TODO: Implement actual answer generation using LLM
        // This is a placeholder implementation
        var documentCount = documents.Count;
        var entityCount = entities.Count;
        
        return $"Based on {documentCount} documents and {entityCount} entities, " +
               $"I found relevant information about '{question}'. " +
               $"The answer would be generated using an LLM with the provided context.";
    }

    private float CalculateAnswerConfidence(string answer, IReadOnlyList<SemanticDocument> documents)
    {
        // TODO: Implement actual confidence calculation
        // This is a placeholder implementation
        return Math.Min(0.9f, 0.5f + (documents.Count * 0.1f));
    }

    private float CalculateHealthScore(SemanticRagStats stats)
    {
        // TODO: Implement actual health score calculation
        // This is a placeholder implementation
        var score = 0.0f;
        
        if (stats.TotalDocuments > 0) score += 0.4f;
        if (stats.TotalEntities > 0) score += 0.3f;
        if (stats.TotalRelationships > 0) score += 0.2f;
        if (stats.LastIndexedAt.HasValue) score += 0.1f;
        
        return Math.Min(1.0f, score);
    }
}

/// <summary>
/// Options for comprehensive search operations.
/// </summary>
/// <param name="SearchOptions">Basic search options.</param>
/// <param name="RagConfig">RAG configuration.</param>
/// <param name="EnableKnowledgeExtraction">Whether to extract knowledge from results.</param>
/// <param name="MaxResultsForExtraction">Maximum number of results to extract knowledge from.</param>
/// <param name="ExtractionOptions">Knowledge extraction options.</param>
/// <param name="EnableContextRetrieval">Whether to retrieve additional context.</param>
public readonly record struct ComprehensiveSearchOptions(
    SemanticSearchOptions SearchOptions,
    SemanticRagConfig RagConfig,
    bool EnableKnowledgeExtraction = true,
    int MaxResultsForExtraction = 5,
    ComprehensiveExtractionOptions ExtractionOptions = default,
    bool EnableContextRetrieval = true)
{
    /// <summary>
    /// Default comprehensive search options.
    /// </summary>
    public static ComprehensiveSearchOptions Default() => new(
        SearchOptions: new SemanticSearchOptions(),
        RagConfig: new SemanticRagConfig(
            Id: "default",
            Name: "Default Configuration",
            EmbeddingModel: "text-embedding-ada-002",
            VectorDimensions: 1536,
            SimilarityThreshold: 0.7,
            MaxResults: 10,
            Properties: []
        ),
        EnableKnowledgeExtraction: true,
        MaxResultsForExtraction: 5,
        ExtractionOptions: ComprehensiveExtractionOptions.Default(),
        EnableContextRetrieval: true);
}

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

/// <summary>
/// Result of repository ingestion operations.
/// </summary>
/// <param name="ProcessedDocuments">Documents that were successfully processed.</param>
/// <param name="TotalDocuments">Total number of documents found.</param>
/// <param name="ExtractedKnowledge">Knowledge extracted from documents.</param>
/// <param name="ProcessingTimeMs">Time taken for processing in milliseconds.</param>
/// <param name="Success">Whether the ingestion was successful.</param>
public readonly record struct RepositoryIngestionResult(
    IReadOnlyList<SemanticDocument> ProcessedDocuments,
    int TotalDocuments,
    IReadOnlyList<IndFusion.SemanticRag.Domain.Models.KnowledgeExtractionResult> ExtractedKnowledge,
    long ProcessingTimeMs,
    bool Success);

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

/// <summary>
/// Result of question answering operations.
/// </summary>
/// <param name="Question">The original question.</param>
/// <param name="Answer">The generated answer.</param>
/// <param name="SupportingDocuments">Documents used to generate the answer.</param>
/// <param name="SupportingEntities">Entities used to generate the answer.</param>
/// <param name="SupportingRelationships">Relationships used to generate the answer.</param>
/// <param name="Confidence">Confidence score for the answer (0.0 to 1.0).</param>
/// <param name="ProcessingTimeMs">Time taken for processing in milliseconds.</param>
public readonly record struct QuestionAnswerResult(
    string Question,
    string Answer,
    IReadOnlyList<SemanticDocument> SupportingDocuments,
    IReadOnlyList<KnowledgeEntity> SupportingEntities,
    IReadOnlyList<KnowledgeRelationship> SupportingRelationships,
    float Confidence,
    long ProcessingTimeMs);

/// <summary>
/// Result of system health checks.
/// </summary>
/// <param name="IsHealthy">Whether the system is healthy.</param>
/// <param name="TotalDocuments">Total number of indexed documents.</param>
/// <param name="TotalEntities">Total number of knowledge entities.</param>
/// <param name="TotalRelationships">Total number of relationships.</param>
/// <param name="LastIndexedAt">When the last document was indexed.</param>
/// <param name="AverageDocumentSize">Average size of documents in characters.</param>
/// <param name="EmbeddingDimension">Dimension of the embedding vectors.</param>
/// <param name="HealthScore">Overall health score (0.0 to 1.0).</param>
public readonly record struct SystemHealthResult(
    bool IsHealthy,
    int TotalDocuments,
    int TotalEntities,
    int TotalRelationships,
    DateTimeOffset? LastIndexedAt,
    double AverageDocumentSize,
    int EmbeddingDimension,
    float HealthScore);