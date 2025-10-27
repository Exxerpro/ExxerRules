using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using Microsoft.Extensions.Logging;

namespace IndFusion.SemanticRag.Infrastructure.Services;

/// <summary>
/// Implementation of document ingestion service with vector storage and knowledge graph integration.
/// </summary>
public class DocumentIngestionService : IDocumentIngestionService
{
    private readonly IDocumentProcessingPipeline _documentProcessor;
    private readonly IVectorSearchService _vectorSearchService;
    private readonly IKnowledgeGraphService _knowledgeGraphService;
    private readonly ILogger<DocumentIngestionService> _logger;
    private readonly Dictionary<string, DocumentIngestionStatus> _ingestionStatuses = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentIngestionService"/> class.
    /// </summary>
    /// <param name="documentProcessor">Document processing pipeline.</param>
    /// <param name="vectorSearchService">Vector search service.</param>
    /// <param name="knowledgeGraphService">Knowledge graph service.</param>
    /// <param name="logger">Logger instance.</param>
    public DocumentIngestionService(
        IDocumentProcessingPipeline documentProcessor,
        IVectorSearchService vectorSearchService,
        IKnowledgeGraphService knowledgeGraphService,
        ILogger<DocumentIngestionService> logger)
    {
        _documentProcessor = documentProcessor;
        _vectorSearchService = vectorSearchService;
        _knowledgeGraphService = knowledgeGraphService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<DocumentIngestionResult> IngestDocumentAsync(
        DocumentInput input, 
        DocumentIngestionOptions options, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting document ingestion: {DocumentId}", input.Id);

            var startTime = DateTime.UtcNow;
            UpdateIngestionStatus(input.Id, IngestionStatus.Processing, 10, "Processing document");

            // Step 1: Process the document
            var processingResult = await _documentProcessor.ProcessDocumentAsync(
                input, 
                options.ProcessingOptions, 
                cancellationToken);

            if (processingResult.Status != ProcessingStatus.Success)
            {
                return new DocumentIngestionResult
                {
                    DocumentId = input.Id,
                    Success = false,
                    ChunksCreated = 0,
                    EmbeddingsGenerated = 0,
                    GraphNodesCreated = 0,
                    ErrorMessage = processingResult.ErrorMessage,
                    ElapsedMilliseconds = (long)(DateTime.UtcNow - startTime).TotalMilliseconds,
                    ProcessingResult = processingResult
                };
            }

            UpdateIngestionStatus(input.Id, IngestionStatus.GeneratingEmbeddings, 30, "Generating embeddings");

            var chunksCreated = 0;
            var embeddingsGenerated = 0;
            var graphNodesCreated = 0;

            // Step 2: Generate embeddings and store in vector database
            if (options.GenerateEmbeddings && options.StoreInVectorDatabase)
            {
                foreach (var chunk in processingResult.Chunks)
                {
                    var chunkMetadata = new Dictionary<string, object>
                    {
                        ["document_id"] = input.Id,
                        ["chunk_id"] = chunk.Id,
                        ["chunk_index"] = chunk.Index,
                        ["document_type"] = processingResult.DocumentType.ToString(),
                        ["source"] = input.FilePath ?? "unknown"
                    };

                    // Add tags if provided
                    if (options.Tags != null)
                    {
                        chunkMetadata["tags"] = options.Tags;
                    }

                    // Add source ID if provided
                    if (!string.IsNullOrEmpty(options.SourceId))
                    {
                        chunkMetadata["source_id"] = options.SourceId;
                    }

                    // Merge with chunk metadata
                    if (chunk.Metadata != null)
                    {
                        foreach (var kvp in chunk.Metadata)
                        {
                            chunkMetadata[kvp.Key] = kvp.Value;
                        }
                    }

                    await _vectorSearchService.StoreDocumentAsync(
                        $"{input.Id}_{chunk.Id}",
                        chunk.Content,
                        chunkMetadata,
                        cancellationToken);

                    chunksCreated++;
                    embeddingsGenerated++;
                }
            }

            UpdateIngestionStatus(input.Id, IngestionStatus.StoringInKnowledgeGraph, 70, "Storing in knowledge graph");

            // Step 3: Store in knowledge graph
            if (options.StoreInKnowledgeGraph)
            {
                var codeNode = new CodeNode
                {
                    Id = input.Id,
                    Type = "Document",
                    Name = input.Name,
                    FilePath = input.FilePath ?? string.Empty,
                    StartLine = 0,
                    EndLine = processingResult.Content.Split('\n').Length,
                    CodeContent = processingResult.Content,
                    Language = processingResult.DocumentType.ToString(),
                    Properties = processingResult.Metadata,
                    Namespace = options.SourceId,
                    Patterns = new List<string>()
                };

                await _knowledgeGraphService.AddCodeNodeAsync(codeNode, cancellationToken);
                graphNodesCreated++;

                // Create relationships between chunks
                for (int i = 0; i < processingResult.Chunks.Count - 1; i++)
                {
                    var currentChunk = processingResult.Chunks[i];
                    var nextChunk = processingResult.Chunks[i + 1];

                    var relationship = new GraphRelationship
                    {
                        Id = $"{input.Id}_chunk_{i}_to_{i + 1}",
                        Type = "FOLLOWS",
                        FromNodeId = $"{input.Id}_{currentChunk.Id}",
                        ToNodeId = $"{input.Id}_{nextChunk.Id}",
                        Properties = new Dictionary<string, object>
                        {
                            ["document_id"] = input.Id,
                            ["sequence"] = i
                        },
                        Weight = 1.0f
                    };

                    await _knowledgeGraphService.CreateRelationshipAsync(relationship, cancellationToken);
                }
            }

            UpdateIngestionStatus(input.Id, IngestionStatus.Completed, 100, "Ingestion completed");

            var elapsedMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

            _logger.LogInformation(
                "Document ingestion completed: {DocumentId} in {ElapsedMs}ms with {ChunksCreated} chunks", 
                input.Id, 
                elapsedMs, 
                chunksCreated);

            return new DocumentIngestionResult
            {
                DocumentId = input.Id,
                Success = true,
                ChunksCreated = chunksCreated,
                EmbeddingsGenerated = embeddingsGenerated,
                GraphNodesCreated = graphNodesCreated,
                ElapsedMilliseconds = elapsedMs,
                ProcessingResult = processingResult
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting document: {DocumentId}", input.Id);
            
            UpdateIngestionStatus(input.Id, IngestionStatus.Failed, 0, "Ingestion failed", ex.Message);

            return new DocumentIngestionResult
            {
                DocumentId = input.Id,
                Success = false,
                ChunksCreated = 0,
                EmbeddingsGenerated = 0,
                GraphNodesCreated = 0,
                ErrorMessage = ex.Message,
                ElapsedMilliseconds = 0
            };
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DocumentIngestionResult>> IngestDocumentsAsync(
        IReadOnlyList<DocumentInput> inputs, 
        DocumentIngestionOptions options, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting batch ingestion of {Count} documents", inputs.Count);

        var results = new List<DocumentIngestionResult>();

        foreach (var input in inputs)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var result = await IngestDocumentAsync(input, options, cancellationToken);
            results.Add(result);
        }

        var successCount = results.Count(r => r.Success);
        _logger.LogInformation(
            "Batch ingestion completed: {SuccessCount}/{TotalCount} successful", 
            successCount, 
            results.Count);

        return results;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DocumentIngestionResult>> IngestDirectoryAsync(
        string directoryPath, 
        DocumentIngestionOptions options, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting directory ingestion: {DirectoryPath}", directoryPath);

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
        }

        var inputs = new List<DocumentInput>();
        var supportedExtensions = new[] { ".cs", ".ts", ".js", ".py", ".md", ".txt", ".pdf", ".png", ".jpg", ".jpeg" };

        foreach (var filePath in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (supportedExtensions.Contains(extension))
            {
                var content = await File.ReadAllBytesAsync(filePath, cancellationToken);
                var input = new DocumentInput
                {
                    Id = Path.GetFileNameWithoutExtension(filePath),
                    Name = Path.GetFileName(filePath),
                    Content = content,
                    FilePath = filePath,
                    Metadata = new Dictionary<string, object>
                    {
                        ["directory"] = directoryPath,
                        ["relative_path"] = Path.GetRelativePath(directoryPath, filePath)
                    }
                };

                inputs.Add(input);
            }
        }

        _logger.LogInformation("Found {Count} supported files in directory", inputs.Count);

        return await IngestDocumentsAsync(inputs, options, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DocumentIngestionStatus> GetIngestionStatusAsync(
        string documentId, 
        CancellationToken cancellationToken = default)
    {
        if (_ingestionStatuses.TryGetValue(documentId, out var status))
        {
            return status;
        }

        return new DocumentIngestionStatus
        {
            DocumentId = documentId,
            Status = IngestionStatus.Pending,
            ProgressPercentage = 0,
            CurrentStep = "Not started",
            StartedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates the ingestion status for a document.
    /// </summary>
    private void UpdateIngestionStatus(
        string documentId, 
        IngestionStatus status, 
        int progressPercentage, 
        string currentStep, 
        string? errorMessage = null)
    {
        var statusUpdate = new DocumentIngestionStatus
        {
            DocumentId = documentId,
            Status = status,
            ProgressPercentage = progressPercentage,
            CurrentStep = currentStep,
            ErrorMessage = errorMessage,
            StartedAt = _ingestionStatuses.TryGetValue(documentId, out var existing) 
                ? existing.StartedAt 
                : DateTime.UtcNow,
            CompletedAt = status == IngestionStatus.Completed || status == IngestionStatus.Failed 
                ? DateTime.UtcNow 
                : null
        };

        _ingestionStatuses[documentId] = statusUpdate;
    }
}


