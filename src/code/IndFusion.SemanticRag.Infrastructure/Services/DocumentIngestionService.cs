using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using IngestionStatusEnum = IndFusion.SemanticRag.Domain.Models.IngestionStatus;

namespace IndFusion.SemanticRag.Infrastructure.Services;

/// <summary>
/// Implementation of document ingestion service with vector storage and knowledge graph integration.
/// </summary>
public class DocumentIngestionService : IDocumentIngestionService
{
    private readonly IDocumentProcessingPipeline _documentProcessor;
    private readonly IVectorSearchService _vectorSearchService;
    private readonly IKnowledgeGraphService _knowledgeGraphService;
    private readonly IEmbeddingServicePort _embeddingService;
    private readonly ILogger<DocumentIngestionService> _logger;
    private readonly Dictionary<string, DocumentIngestionStatus> _ingestionStatuses = new();
    private readonly Dictionary<string, CancellationTokenSource> _activeIngestions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentIngestionService"/> class.
    /// </summary>
    /// <param name="documentProcessor">Document processing pipeline.</param>
    /// <param name="vectorSearchService">Vector search service.</param>
    /// <param name="knowledgeGraphService">Knowledge graph service.</param>
    /// <param name="embeddingService">Embedding service port.</param>
    /// <param name="logger">Logger instance.</param>
    public DocumentIngestionService(
        IDocumentProcessingPipeline documentProcessor,
        IVectorSearchService vectorSearchService,
        IKnowledgeGraphService knowledgeGraphService,
        IEmbeddingServicePort embeddingService,
        ILogger<DocumentIngestionService> logger)
    {
        _documentProcessor = documentProcessor;
        _vectorSearchService = vectorSearchService;
        _knowledgeGraphService = knowledgeGraphService;
        _embeddingService = embeddingService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<DocumentIngestionResult>> IngestDocumentAsync(
        DocumentInput input, 
        DocumentIngestionOptions options, 
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTimeOffset.UtcNow;
        try
        {
            _logger.LogInformation("Starting document ingestion: {DocumentId}", input.Id);

            UpdateIngestionStatus(input.Id, IngestionStatusEnum.Processing, 10, "Processing document");

            // Step 1: Process the document
            var processingResult = await _documentProcessor.ProcessDocumentAsync(
                input, 
                options.ProcessingOptions ?? new DocumentProcessingOptions(), 
                cancellationToken);

            if (processingResult.Status != ProcessingStatus.Success)
            {
                var failureResult = new DocumentIngestionResult
                {
                    Id = Guid.NewGuid().ToString(),
                    DocumentId = input.Id,
                    Status = IngestionStatusEnum.Failed,
                    ProcessingResult = processingResult,
                    VectorEmbeddings = Array.Empty<VectorEmbedding>(),
                    ExtractedEntities = Array.Empty<KnowledgeEntity>(),
                    MappedRelationships = Array.Empty<KnowledgeRelationship>(),
                    StartedAt = startTime,
                    DurationMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds,
                    ErrorMessage = processingResult.ErrorMessage,
                    ProgressPercentage = 0
                };
                return Result<DocumentIngestionResult>.WithFailure(
                    processingResult.ErrorMessage ?? "Document processing failed",
                    failureResult);
            }

            UpdateIngestionStatus(input.Id, IngestionStatusEnum.Processing, 30, "Generating embeddings");

            var vectorEmbeddings = new List<VectorEmbedding>();
            var extractedEntities = new List<KnowledgeEntity>();
            var mappedRelationships = new List<KnowledgeRelationship>();

            // Step 2: Generate embeddings and store in vector database
            if (options.EnableVectorIndexing)
            {
                foreach (var chunk in processingResult.Chunks)
                {
                    var chunkMetadata = new Dictionary<string, object>
                    {
                        ["document_id"] = input.Id,
                        ["chunk_id"] = chunk.Id,
                        ["chunk_index"] = chunk.ChunkIndex,
                        ["document_type"] = processingResult.DocumentType.ToString(),
                        ["source"] = input.FilePath ?? "unknown"
                    };

                    // Add custom settings as metadata if provided
                    if (options.CustomSettings != null)
                    {
                        foreach (var kvp in options.CustomSettings)
                        {
                            chunkMetadata[kvp.Key] = kvp.Value;
                        }
                    }

                    // Merge with chunk metadata
                    if (chunk.Metadata != null)
                    {
                        foreach (var kvp in chunk.Metadata)
                        {
                            chunkMetadata[kvp.Key] = kvp.Value;
                        }
                    }

                    // Generate embedding and create VectorEmbedding
                    var embeddingResult = await _embeddingService.GenerateEmbeddingAsync(chunk.Content, cancellationToken);
                    if (embeddingResult.IsFailure)
                    {
                        _logger.LogWarning(
                            "Failed to generate embedding for chunk {ChunkId} in document {DocumentId}: {Error}",
                            chunk.Id,
                            input.Id,
                            embeddingResult.Error);
                        continue;
                    }

                    var embeddingVector = embeddingResult.Value;
                    var vectorEmbedding = new VectorEmbedding(
                        Id: $"{input.Id}_{chunk.Id}",
                        Content: chunk.Content,
                        Embedding: embeddingVector ?? Array.Empty<float>(),
                        Metadata: chunkMetadata,
                        CreatedAt: DateTimeOffset.UtcNow
                    );

                    // Index the vector
                    var indexResult = await _vectorSearchService.IndexAsync(vectorEmbedding, cancellationToken);
                    if (indexResult.IsSuccess)
                    {
                        vectorEmbeddings.Add(vectorEmbedding);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Failed to index embedding for chunk {ChunkId} in document {DocumentId}: {Error}",
                            chunk.Id,
                            input.Id,
                            indexResult.Error);
                    }
                }
            }

            UpdateIngestionStatus(input.Id, IngestionStatusEnum.Processing, 70, "Storing in knowledge graph");

            // Step 3: Store in knowledge graph
            if (options.EnableKnowledgeGraph)
            {
                // Create entity for the document
                var documentEntity = new KnowledgeEntity(
                    Id: input.Id,
                    Name: input.Name,
                    Type: "Document",
                    Description: $"Document: {input.Name}",
                    Properties: processingResult.Metadata ?? new Dictionary<string, object>(),
                    Confidence: 1.0,
                    CreatedAt: DateTime.UtcNow
                );

                var createEntityResult = await _knowledgeGraphService.CreateEntityAsync(documentEntity, cancellationToken);
                if (createEntityResult.IsSuccess)
                {
                    extractedEntities.Add(documentEntity);
                }

                // Create relationships between chunks
                for (int i = 0; i < processingResult.Chunks.Count - 1; i++)
                {
                    var currentChunk = processingResult.Chunks[i];
                    var nextChunk = processingResult.Chunks[i + 1];

                    var relationship = new KnowledgeRelationship(
                        Id: $"{input.Id}_chunk_{i}_to_{i + 1}",
                        FromNodeId: $"{input.Id}_{currentChunk.Id}",
                        ToNodeId: $"{input.Id}_{nextChunk.Id}",
                        RelationshipType: "FOLLOWS",
                        Properties: new Dictionary<string, object>
                        {
                            ["document_id"] = input.Id,
                            ["sequence"] = i
                        },
                        CreatedAt: DateTimeOffset.UtcNow
                    );

                    var createRelResult = await _knowledgeGraphService.CreateRelationshipAsync(relationship, cancellationToken);
                    if (createRelResult.IsSuccess)
                    {
                        mappedRelationships.Add(relationship);
                    }
                }
            }

            UpdateIngestionStatus(input.Id, IngestionStatusEnum.Completed, 100, "Ingestion completed");

            var elapsedMs = (long)(DateTimeOffset.UtcNow - startTime).TotalMilliseconds;

            _logger.LogInformation(
                "Document ingestion completed: {DocumentId} in {ElapsedMs}ms with {EmbeddingCount} embeddings and {EntityCount} entities", 
                input.Id, 
                elapsedMs, 
                vectorEmbeddings.Count,
                extractedEntities.Count);

            var successResult = new DocumentIngestionResult
            {
                Id = Guid.NewGuid().ToString(),
                DocumentId = input.Id,
                Status = IngestionStatusEnum.Completed,
                ProcessingResult = processingResult,
                VectorEmbeddings = vectorEmbeddings,
                ExtractedEntities = extractedEntities,
                MappedRelationships = mappedRelationships,
                StartedAt = startTime,
                CompletedAt = DateTimeOffset.UtcNow,
                DurationMs = elapsedMs,
                ProgressPercentage = 100
            };
            return Result<DocumentIngestionResult>.Success(successResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting document: {DocumentId}", input.Id);
            
            UpdateIngestionStatus(input.Id, IngestionStatusEnum.Failed, 0, "Ingestion failed", ex.Message);

            var failureResult = new DocumentIngestionResult
            {
                Id = Guid.NewGuid().ToString(),
                DocumentId = input.Id,
                Status = IngestionStatusEnum.Failed,
                VectorEmbeddings = Array.Empty<VectorEmbedding>(),
                ExtractedEntities = Array.Empty<KnowledgeEntity>(),
                MappedRelationships = Array.Empty<KnowledgeRelationship>(),
                StartedAt = startTime,
                DurationMs = 0,
                ErrorMessage = ex.Message,
                ProgressPercentage = 0
            };
            return Result<DocumentIngestionResult>.WithFailure(ex.Message, failureResult);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<DocumentIngestionResult>>> IngestDocumentsAsync(
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
            if (result.IsSuccess && result.Value != null)
            {
                results.Add(result.Value);
            }
            else
            {
                // Create failure result for tracking
                var startTime = DateTimeOffset.UtcNow;
                var failureResult = new DocumentIngestionResult
                {
                    Id = Guid.NewGuid().ToString(),
                    DocumentId = input.Id,
                    Status = IngestionStatusEnum.Failed,
                    ProcessingResult = null,
                    VectorEmbeddings = Array.Empty<VectorEmbedding>(),
                    ExtractedEntities = Array.Empty<KnowledgeEntity>(),
                    MappedRelationships = Array.Empty<KnowledgeRelationship>(),
                    StartedAt = startTime,
                    DurationMs = 0,
                    ErrorMessage = result.Error ?? "Ingestion failed",
                    ProgressPercentage = 0
                };
                results.Add(failureResult);
            }
        }

        var successCount = results.Count(r => r.Status == IngestionStatusEnum.Completed);
        _logger.LogInformation(
            "Batch ingestion completed: {SuccessCount}/{TotalCount} successful", 
            successCount, 
            results.Count);

        return Result<IReadOnlyList<DocumentIngestionResult>>.Success(results);
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<DocumentIngestionResult>>> IngestDirectoryAsync(
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
                var input = new DocumentInput(
                    Id: Path.GetFileNameWithoutExtension(filePath),
                    Name: Path.GetFileName(filePath),
                    Content: content,
                    MimeType: GetMimeType(extension),
                    FilePath: filePath,
                    Metadata: new Dictionary<string, object>
                    {
                        ["directory"] = directoryPath,
                        ["relative_path"] = Path.GetRelativePath(directoryPath, filePath)
                    }
                );

                inputs.Add(input);
            }
        }

        _logger.LogInformation("Found {Count} supported files in directory", inputs.Count);

        return await IngestDocumentsAsync(inputs, options, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<DocumentIngestionStatus>> GetIngestionStatusAsync(
        string documentId, 
        CancellationToken cancellationToken = default)
    {
        if (_ingestionStatuses.TryGetValue(documentId, out var status))
        {
            return Result<DocumentIngestionStatus>.Success(status);
        }

        var pendingStatus = new DocumentIngestionStatus
        {
            DocumentId = documentId,
            Status = IngestionStatusEnum.Pending,
            ProgressPercentage = 0,
            CurrentStage = "Not started",
            StartedAt = DateTimeOffset.UtcNow
        };
        return Result<DocumentIngestionStatus>.Success(pendingStatus);
    }

    /// <inheritdoc />
    public async Task<Result<RepositoryIngestionResult>> IngestRepositoryAsync(
        string repositoryPath,
        RepositoryIngestionConfig config,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTimeOffset.UtcNow;
        try
        {
            _logger.LogInformation("Ingesting repository: {RepositoryPath}", repositoryPath);
            
            if (!Directory.Exists(repositoryPath))
            {
                return Result<RepositoryIngestionResult>.WithFailure($"Repository path not found: {repositoryPath}");
            }

            // Create cancellation token source for this ingestion
            var ingestionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var ingestionId = Guid.NewGuid().ToString();
            _activeIngestions[ingestionId] = ingestionCts;

            try
            {
                // Enumerate files recursively
                var allFiles = Directory.GetFiles(repositoryPath, "*", SearchOption.AllDirectories);
                var validFiles = new List<string>();

                // Apply filters
                foreach (var filePath in allFiles)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    // Check file size
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length > config.MaxFileSize)
                    {
                        _logger.LogDebug("Skipping file {FilePath} - exceeds max size {MaxSize}", filePath, config.MaxFileSize);
                        continue;
                    }

                    // Check directory depth
                    var relativePath = Path.GetRelativePath(repositoryPath, filePath);
                    var depth = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length - 1;
                    if (depth > config.MaxDepth)
                    {
                        _logger.LogDebug("Skipping file {FilePath} - exceeds max depth {MaxDepth}", filePath, config.MaxDepth);
                        continue;
                    }

                    // Check include patterns
                    var fileName = Path.GetFileName(filePath);
                    var matched = false;
                    if (config.IncludePatterns != null && config.IncludePatterns.Count > 0)
                    {
                        foreach (var pattern in config.IncludePatterns)
                        {
                            if (MatchesPattern(fileName, pattern) || MatchesPattern(filePath, pattern))
                            {
                                matched = true;
                                break;
                            }
                        }
                        if (!matched)
                            continue;
                    }

                    // Check exclude patterns
                    var excluded = false;
                    if (config.ExcludePatterns != null && config.ExcludePatterns.Count > 0)
                    {
                        foreach (var pattern in config.ExcludePatterns)
                        {
                            if (MatchesPattern(filePath, pattern))
                            {
                                excluded = true;
                                break;
                            }
                        }
                        if (excluded)
                            continue;
                    }

                    validFiles.Add(filePath);
                }

                _logger.LogInformation("Found {Count} valid files to process", validFiles.Count);

                // Process files
                var processedDocuments = new List<SemanticDocument>();
                var extractedKnowledge = new List<KnowledgeExtractionResult>();
                var ingestionOptions = config.IngestionOptions ?? DocumentIngestionOptions.Default();

                foreach (var filePath in validFiles)
                {
                    if (ingestionCts.Token.IsCancellationRequested)
                        break;

                    try
                    {
                        var content = await File.ReadAllBytesAsync(filePath, ingestionCts.Token);
                        var fileName = Path.GetFileName(filePath);
                        
                        var input = new DocumentInput(
                            Id: Path.GetFileNameWithoutExtension(filePath),
                            Name: fileName,
                            Content: content,
                            MimeType: GetMimeType(Path.GetExtension(filePath)),
                            FilePath: filePath,
                            Metadata: new Dictionary<string, object>
                            {
                                ["repository_path"] = repositoryPath,
                                ["relative_path"] = Path.GetRelativePath(repositoryPath, filePath),
                                ["file_size"] = content.Length
                            }
                        );

                        var result = await IngestDocumentAsync(input, ingestionOptions, ingestionCts.Token);
                        if (result.IsSuccess && result.Value != null)
                        {
                            var ingestionResult = result.Value;
                            
                            // Create SemanticDocument from ingestion result
                            var semanticDocument = new SemanticDocument(
                                Id: ingestionResult.DocumentId,
                                Title: input.Name,
                                Content: ingestionResult.ProcessingResult?.Content ?? string.Empty,
                                Metadata: input.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>(),
                                CreatedAt: ingestionResult.StartedAt.DateTime,
                                UpdatedAt: ingestionResult.CompletedAt?.DateTime ?? ingestionResult.StartedAt.DateTime
                            );
                            processedDocuments.Add(semanticDocument);

                            // Create KnowledgeExtractionResult
                            var knowledgeResult = new KnowledgeExtractionResult(
                                DocumentId: ingestionResult.DocumentId,
                                   Entities: ingestionResult.ExtractedEntities ?? Array.Empty<KnowledgeEntity>(),
                                   Relationships: ingestionResult.MappedRelationships ?? Array.Empty<KnowledgeRelationship>(),
                                   Summary: $"Processed {ingestionResult.VectorEmbeddings?.Count ?? 0} chunks with {ingestionResult.ExtractedEntities?.Count ?? 0} entities",
                                   Confidence: ingestionResult.ExtractedEntities?.Any() == true 
                                       ? ingestionResult.ExtractedEntities.Average(e => e.Confidence) 
                                       : 0.0,
                                Metadata: new Dictionary<string, object>
                                {
                                    ["vector_embeddings_count"] = ingestionResult.VectorEmbeddings?.Count ?? 0,
                                    ["processing_duration_ms"] = ingestionResult.DurationMs
                                }
                            );
                            extractedKnowledge.Add(knowledgeResult);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to process file: {FilePath}", filePath);
                        // Continue with next file
                    }
                }

                var elapsedMs = (long)(DateTimeOffset.UtcNow - startTime).TotalMilliseconds;

                var repositoryResult = new RepositoryIngestionResult(
                    ProcessedDocuments: processedDocuments,
                    TotalDocuments: validFiles.Count,
                    ExtractedKnowledge: extractedKnowledge,
                    ProcessingTimeMs: elapsedMs,
                    Success: !ingestionCts.Token.IsCancellationRequested
                );

                _logger.LogInformation(
                    "Repository ingestion completed: {ProcessedCount}/{TotalCount} documents in {ElapsedMs}ms",
                    processedDocuments.Count,
                    validFiles.Count,
                    elapsedMs);

                return Result<RepositoryIngestionResult>.Success(repositoryResult);
            }
            finally
            {
                _activeIngestions.Remove(ingestionId);
                ingestionCts.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting repository: {RepositoryPath}", repositoryPath);
            return Result<RepositoryIngestionResult>.WithFailure(
                $"Failed to ingest repository: {ex.Message}",
                new RepositoryIngestionResult(
                    ProcessedDocuments: Array.Empty<SemanticDocument>(),
                    TotalDocuments: 0,
                    ExtractedKnowledge: Array.Empty<KnowledgeExtractionResult>(),
                    ProcessingTimeMs: (long)(DateTimeOffset.UtcNow - startTime).TotalMilliseconds,
                    Success: false
                ));
        }
    }

    /// <summary>
    /// Checks if a path matches a glob pattern.
    /// </summary>
    private static bool MatchesPattern(string path, string pattern)
    {
        // Simple glob pattern matching
        // Supports * (any characters) and ** (recursive directory)
        var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
            .Replace("\\*\\*", ".*")
            .Replace("\\*", "[^/]*")
            + "$";
        
        return System.Text.RegularExpressions.Regex.IsMatch(path, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    /// <inheritdoc />
    public async Task<Result> CancelIngestionAsync(string documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Cancelling ingestion for document: {DocumentId}", documentId);
            
            // Find active ingestion for this document
            CancellationTokenSource? cts = null;
            string? ingestionId = null;
            foreach (var kvp in _activeIngestions)
            {
                // Check if this ingestion is processing the document
                // We'll use a simple matching approach - in practice, you might want to track document-to-ingestion mapping
                if (_ingestionStatuses.ContainsKey(documentId))
                {
                    cts = kvp.Value;
                    ingestionId = kvp.Key;
                    break;
                }
            }

            if (cts != null && ingestionId != null)
            {
                cts.Cancel();
                _activeIngestions.Remove(ingestionId);
                
                UpdateIngestionStatus(documentId, IngestionStatusEnum.Cancelled, 0, "Cancelled", null);
                
                _logger.LogInformation("Successfully cancelled ingestion for document: {DocumentId}", documentId);
                return Result.Success();
            }
            else
            {
                // Check if ingestion already completed or doesn't exist
                if (_ingestionStatuses.TryGetValue(documentId, out var status))
                {
                    if (status.Status == IngestionStatusEnum.Completed || status.Status == IngestionStatusEnum.Failed)
                    {
                        _logger.LogWarning("Cannot cancel ingestion for document {DocumentId} - already {Status}", documentId, status.Status);
                        return Result.WithFailure($"Cannot cancel ingestion - already {status.Status}");
                    }
                }

                _logger.LogWarning("No active ingestion found for document: {DocumentId}", documentId);
                return Result.WithFailure($"No active ingestion found for document: {documentId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel ingestion for document: {DocumentId}", documentId);
            return Result.WithFailure($"Failed to cancel ingestion: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the ingestion status for a document.
    /// </summary>
    private void UpdateIngestionStatus(
        string documentId, 
        IngestionStatusEnum status, 
        int progressPercentage, 
        string currentStage, 
        string? errorMessage = null)
    {
        var statusUpdate = new DocumentIngestionStatus
        {
            DocumentId = documentId,
            Status = status,
            ProgressPercentage = progressPercentage,
            CurrentStage = currentStage,
            ErrorMessage = errorMessage,
            StartedAt = _ingestionStatuses.TryGetValue(documentId, out var existing) 
                ? existing.StartedAt 
                : DateTimeOffset.UtcNow,
            CompletedAt = status == IngestionStatusEnum.Completed || status == IngestionStatusEnum.Failed 
                ? DateTimeOffset.UtcNow 
                : null
        };

        _ingestionStatuses[documentId] = statusUpdate;
    }

    /// <summary>
    /// Gets the MIME type for a file extension.
    /// </summary>
    private static string GetMimeType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".cs" => "text/x-csharp",
            ".ts" => "text/x-typescript",
            ".js" => "text/javascript",
            ".py" => "text/x-python",
            ".md" => "text/markdown",
            ".txt" => "text/plain",
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
    }
}


