using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace IndFusion.SemanticRag.Infrastructure.Services;

/// <summary>
/// Implementation of document processing pipeline with OCR and multi-format support.
/// </summary>
public class DocumentProcessingPipeline : IDocumentProcessingPipeline
{
    private readonly ILogger<DocumentProcessingPipeline> _logger;
    private readonly IOcrService _ocrService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentProcessingPipeline"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="ocrService">OCR service for image text extraction.</param>
    public DocumentProcessingPipeline(
        ILogger<DocumentProcessingPipeline> logger,
        IOcrService ocrService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ocrService = ocrService ?? throw new ArgumentNullException(nameof(ocrService));
    }

    /// <inheritdoc />
    public async Task<DocumentProcessingResult> ProcessDocumentAsync(
        DocumentInput input, 
        DocumentProcessingOptions options, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing document: {DocumentId}", input.Id);

            var startTime = DateTime.UtcNow;

            // Validate file size - MaxFileSize is not part of DocumentProcessingOptions, removed validation
            // If file size validation is needed, it should be done at the ingestion level using RepositoryIngestionConfig.MaxFileSize

            // Detect document type
            cancellationToken.ThrowIfCancellationRequested();
            var documentType = await DetectDocumentTypeAsync(input, cancellationToken);

            // Process based on document type
            cancellationToken.ThrowIfCancellationRequested();
            var content = documentType switch
            {
                DocumentType.CSharpCode => await ProcessCodeDocumentAsync(input, cancellationToken),
                DocumentType.TypeScriptCode => await ProcessCodeDocumentAsync(input, cancellationToken),
                DocumentType.PythonCode => await ProcessCodeDocumentAsync(input, cancellationToken),
                DocumentType.Markdown => await ProcessTextDocumentAsync(input, cancellationToken),
                DocumentType.Pdf => await ProcessPdfDocumentAsync(input, cancellationToken),
                DocumentType.Image => await ProcessImageDocumentAsync(input, options, cancellationToken),
                DocumentType.Text => await ProcessTextDocumentAsync(input, cancellationToken),
                _ => string.Empty
            };

            // Create chunks
            cancellationToken.ThrowIfCancellationRequested();
            var chunks = CreateChunks(content, options, input);

            // Extract metadata
            cancellationToken.ThrowIfCancellationRequested();
            var metadata = options.ExtractMetadata 
                ? await ExtractMetadataAsync(input, documentType, cancellationToken)
                : null;

            var elapsedMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

            _logger.LogInformation(
                "Document processed successfully: {DocumentId} in {ElapsedMs}ms", 
                input.Id, 
                elapsedMs);

            return new DocumentProcessingResult
            {
                Id = Guid.NewGuid().ToString(),
                DocumentId = input.Id,
                Content = content,
                Chunks = chunks,
                Metadata = metadata,
                DocumentType = documentType,
                Status = ProcessingStatus.Success,
                ElapsedMilliseconds = elapsedMs
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Document processing was cancelled: {DocumentId}", input.Id);
            
            return new DocumentProcessingResult
            {
                Id = Guid.NewGuid().ToString(),
                DocumentId = input.Id,
                Content = string.Empty,
                Chunks = [],
                Metadata = [],
                DocumentType = DocumentType.Unknown,
                Status = ProcessingStatus.Cancelled,
                ErrorMessage = ErrorCodes.OperationCancelled,
                ElapsedMilliseconds = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing document: {DocumentId}", input.Id);
            
            return new DocumentProcessingResult
            {
                Id = Guid.NewGuid().ToString(),
                DocumentId = input.Id,
                Content = string.Empty,
                Chunks = [],
                Metadata = [],
                DocumentType = DocumentType.Unknown,
                Status = ProcessingStatus.Failed,
                ErrorMessage = ex.Message,
                ElapsedMilliseconds = 0
            };
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DocumentProcessingResult>> ProcessDocumentsAsync(
        IReadOnlyList<DocumentInput> inputs, 
        DocumentProcessingOptions options, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing {Count} documents in batch", inputs.Count);

        var results = new List<DocumentProcessingResult>();

        foreach (var input in inputs)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var result = await ProcessDocumentAsync(input, options, cancellationToken);
            results.Add(result);
        }

        _logger.LogInformation("Batch processing completed: {SuccessCount}/{TotalCount} successful", 
            results.Count(r => r.Status == ProcessingStatus.Success), 
            results.Count);

        return results;
    }

    /// <inheritdoc />
    public async Task<DocumentType> DetectDocumentTypeAsync(
        DocumentInput input, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var content = Encoding.UTF8.GetString(input.Content);
            
            // Analyze content first - content analysis can override MIME type/extension
            if (IsCodeContent(content))
            {
                return DetectCodeLanguage(content);
            }

            // Check file extension second (before MIME type) - extension is more specific
            if (!string.IsNullOrEmpty(input.FilePath))
            {
                var extension = Path.GetExtension(input.FilePath).ToLowerInvariant();
                var typeFromExtension = extension switch
                {
                    ".cs" => DocumentType.CSharpCode,
                    ".ts" or ".tsx" or ".js" or ".jsx" => DocumentType.TypeScriptCode,
                    ".py" => DocumentType.PythonCode,
                    ".md" => DocumentType.Markdown,
                    ".pdf" => DocumentType.Pdf,
                    ".png" or ".jpg" or ".jpeg" or ".gif" => DocumentType.Image,
                    ".txt" => DocumentType.Text,
                    _ => DocumentType.Unknown
                };
                
                // If extension gives us a specific type (not Unknown or Text), use it
                if (typeFromExtension != DocumentType.Unknown && typeFromExtension != DocumentType.Text)
                {
                    return typeFromExtension;
                }
            }

            // Check MIME type third (but not for text/plain if extension suggests something else)
            if (!string.IsNullOrEmpty(input.MimeType))
            {
                var mimeType = input.MimeType.ToLowerInvariant();
                return mimeType switch
                {
                    "text/plain" => DocumentType.Text, // Only if not detected as code above
                    "text/markdown" => DocumentType.Markdown,
                    "application/pdf" => DocumentType.Pdf,
                    "image/png" or "image/jpeg" or "image/jpg" or "image/gif" => DocumentType.Image,
                    "text/x-csharp" or "application/x-csharp" => DocumentType.CSharpCode,
                    "text/typescript" or "application/typescript" => DocumentType.TypeScriptCode,
                    "text/x-python" or "application/x-python" => DocumentType.PythonCode,
                    _ => DocumentType.Unknown
                };
            }

            return DocumentType.Unknown;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting document type for: {DocumentId}", input.Id);
            return DocumentType.Unknown;
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<DocumentType> GetSupportedDocumentTypes()
    {
        return new[]
        {
            DocumentType.CSharpCode,
            DocumentType.TypeScriptCode,
            DocumentType.PythonCode,
            DocumentType.Markdown,
            DocumentType.Pdf,
            DocumentType.Image,
            DocumentType.Text
        };
    }

    /// <summary>
    /// Processes a code document.
    /// </summary>
    private async Task<string> ProcessCodeDocumentAsync(
        DocumentInput input, 
        CancellationToken cancellationToken = default)
    {
        var content = Encoding.UTF8.GetString(input.Content);
        
        // For code files, we might want to add syntax highlighting metadata
        // or extract specific patterns, but for now just return the content
        return content;
    }

    /// <summary>
    /// Processes a text document.
    /// </summary>
    private async Task<string> ProcessTextDocumentAsync(
        DocumentInput input, 
        CancellationToken cancellationToken = default)
    {
        return Encoding.UTF8.GetString(input.Content);
    }

    /// <summary>
    /// Processes a PDF document using PdfPig (FOSS alternative to iTextSharp).
    /// </summary>
    private async Task<string> ProcessPdfDocumentAsync(
        DocumentInput input, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var document = PdfDocument.Open(input.Content);
            
            var text = new StringBuilder();
            
            foreach (var page in document.GetPages())
            {
                var pageText = page.Text;
                text.AppendLine(pageText);
            }
            
            return text.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PDF document: {DocumentId}", input.Id);
            return string.Empty;
        }
    }

    /// <summary>
    /// Processes an image document using OCR.
    /// </summary>
    private async Task<string> ProcessImageDocumentAsync(
        DocumentInput input, 
        DocumentProcessingOptions options, 
        CancellationToken cancellationToken = default)
    {
        if (!options.EnableOcr)
        {
            return string.Empty;
        }

        try
        {
            var text = await _ocrService.ExtractTextAsync(input.Content, cancellationToken).ConfigureAwait(false);
            return text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image with OCR: {DocumentId}", input.Id);
            return string.Empty;
        }
    }

    /// <summary>
    /// Creates chunks from document content.
    /// </summary>
    private IReadOnlyList<DocumentChunk> CreateChunks(
        string content, 
        DocumentProcessingOptions options, 
        DocumentInput input)
    {
        var chunks = new List<DocumentChunk>();

        switch (options.ChunkingStrategy)
        {
            case ChunkingStrategy.FixedSize:
                chunks = CreateFixedSizeChunks(content, options.MaxChunkSize, input);
                break;
            case ChunkingStrategy.Semantic:
                chunks = CreateSemanticChunks(content, options.MaxChunkSize, input);
                break;
            case ChunkingStrategy.LineBased:
                chunks = CreateLineBasedChunks(content, options.MaxChunkSize, input);
                break;
            case ChunkingStrategy.ParagraphBased:
                chunks = CreateParagraphBasedChunks(content, options.MaxChunkSize, input);
                break;
        }

        return chunks;
    }

    /// <summary>
    /// Creates fixed-size chunks.
    /// </summary>
    private List<DocumentChunk> CreateFixedSizeChunks(string content, int maxChunkSize, DocumentInput input)
    {
        var chunks = new List<DocumentChunk>();
        
        if (string.IsNullOrEmpty(content))
        {
            return chunks;
        }
        
        var chunkIndex = 0;
        var startPosition = 0;
        var contentLength = content.Length;

        // Split content into chunks of maxChunkSize characters
        while (startPosition < contentLength)
        {
            var remainingLength = contentLength - startPosition;
            var chunkLength = Math.Min(maxChunkSize, remainingLength);
            var chunkContent = content.Substring(startPosition, chunkLength);
            
            // Try to break at word boundary if not at end of content
            if (startPosition + chunkLength < contentLength && chunkLength == maxChunkSize)
            {
                var lastSpaceIndex = chunkContent.LastIndexOf(' ');
                var lastNewlineIndex = chunkContent.LastIndexOf('\n');
                var breakIndex = Math.Max(lastSpaceIndex, lastNewlineIndex);
                
                if (breakIndex > maxChunkSize / 2) // Only break if we're at least halfway through
                {
                    chunkLength = breakIndex + 1;
                    chunkContent = content.Substring(startPosition, chunkLength);
                }
            }

            chunks.Add(new DocumentChunk(
                Id: $"{chunkIndex}",
                DocumentId: input.Id,
                Content: chunkContent,
                ChunkIndex: chunkIndex,
                StartPosition: startPosition,
                EndPosition: startPosition + chunkLength,
                Metadata: new Dictionary<string, object>
                {
                    ["chunk_type"] = "fixed_size",
                    ["chunk_size"] = chunkLength
                }
            ));

            startPosition += chunkLength;
            chunkIndex++;
        }

        return chunks;
    }

    /// <summary>
    /// Creates semantic chunks based on content structure.
    /// </summary>
    private List<DocumentChunk> CreateSemanticChunks(string content, int maxChunkSize, DocumentInput input)
    {
        // For now, use paragraph-based chunking as a semantic approach
        return CreateParagraphBasedChunks(content, maxChunkSize, input);
    }

    /// <summary>
    /// Creates line-based chunks for code files.
    /// </summary>
    private List<DocumentChunk> CreateLineBasedChunks(string content, int maxChunkSize, DocumentInput input)
    {
        var chunks = new List<DocumentChunk>();
        var lines = content.Split('\n');
        var currentChunk = new StringBuilder();
        var chunkIndex = 0;
        var startPosition = 0;

        foreach (var line in lines)
        {
            if (currentChunk.Length + line.Length > maxChunkSize && currentChunk.Length > 0)
            {
                chunks.Add(new DocumentChunk(
                    Id: $"{chunkIndex}",
                    DocumentId: input.Id,
                    Content: currentChunk.ToString(),
                    ChunkIndex: chunkIndex,
                    StartPosition: startPosition,
                    EndPosition: startPosition + currentChunk.Length,
                    Metadata: new Dictionary<string, object>
                    {
                        ["chunk_type"] = "line_based",
                        ["line_count"] = currentChunk.ToString().Split('\n').Length
                    }
                ));

                startPosition += currentChunk.Length;
                currentChunk.Clear();
                chunkIndex++;
            }

            currentChunk.AppendLine(line);
        }

        if (currentChunk.Length > 0)
        {
            chunks.Add(new DocumentChunk(
                Id: $"{chunkIndex}",
                DocumentId: input.Id,
                Content: currentChunk.ToString(),
                ChunkIndex: chunkIndex,
                StartPosition: startPosition,
                EndPosition: startPosition + currentChunk.Length,
                Metadata: new Dictionary<string, object>
                {
                    ["chunk_type"] = "line_based",
                    ["line_count"] = currentChunk.ToString().Split('\n').Length
                }
            ));
        }

        return chunks;
    }

    /// <summary>
    /// Creates paragraph-based chunks for text documents.
    /// </summary>
    private List<DocumentChunk> CreateParagraphBasedChunks(string content, int maxChunkSize, DocumentInput input)
    {
        var chunks = new List<DocumentChunk>();
        var paragraphs = content.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        var chunkIndex = 0;
        var startPosition = 0;

        foreach (var paragraph in paragraphs)
        {
            if (paragraph.Length > maxChunkSize)
            {
                // Split large paragraphs into smaller chunks
                var subChunks = CreateFixedSizeChunks(paragraph, maxChunkSize, input);
                foreach (var subChunk in subChunks)
                {
                    chunks.Add(new DocumentChunk(
                        Id: $"{chunkIndex}",
                        DocumentId: input.Id,
                        Content: subChunk.Content,
                        ChunkIndex: chunkIndex,
                        StartPosition: startPosition + subChunk.StartPosition,
                        EndPosition: startPosition + subChunk.EndPosition,
                        Metadata: subChunk.Metadata
                    ));
                    chunkIndex++;
                }
            }
            else
            {
                chunks.Add(new DocumentChunk(
                    Id: $"{chunkIndex}",
                    DocumentId: input.Id,
                    Content: paragraph,
                    ChunkIndex: chunkIndex,
                    StartPosition: startPosition,
                    EndPosition: startPosition + paragraph.Length,
                    Metadata: new Dictionary<string, object>
                    {
                        ["chunk_type"] = "paragraph",
                        ["paragraph_length"] = paragraph.Length
                    }
                ));
                chunkIndex++;
            }

            startPosition += paragraph.Length + 2; // +2 for paragraph separator
        }

        return chunks;
    }

    /// <summary>
    /// Extracts metadata from the document.
    /// </summary>
    private async Task<Dictionary<string, object>> ExtractMetadataAsync(
        DocumentInput input, 
        DocumentType documentType, 
        CancellationToken cancellationToken = default)
    {
        var metadata = new Dictionary<string, object>
        {
            ["document_id"] = input.Id,
            ["document_name"] = input.Name,
            ["document_type"] = documentType.ToString(),
            ["file_size"] = input.Content.Length,
            ["processed_at"] = DateTime.UtcNow
        };

        if (!string.IsNullOrEmpty(input.FilePath))
        {
            metadata["file_path"] = input.FilePath;
            metadata["file_extension"] = Path.GetExtension(input.FilePath);
        }

        if (!string.IsNullOrEmpty(input.MimeType))
        {
            metadata["mime_type"] = input.MimeType;
        }

        // Add custom metadata from input
        if (input.Metadata != null)
        {
            foreach (var kvp in input.Metadata)
            {
                metadata[kvp.Key] = kvp.Value;
            }
        }

        return metadata;
    }

    /// <summary>
    /// Determines if content appears to be code.
    /// </summary>
    private static bool IsCodeContent(string content)
    {
        var codeIndicators = new[]
        {
            "public class", "private class", "public interface", "public enum",
            "function ", "const ", "let ", "var ", "import ", "export ",
            "def ", "class ", "import ", "from ", "if __name__",
            "using ", "namespace ", "public static", "private static"
        };

        return codeIndicators.Any(indicator => 
            content.Contains(indicator, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Detects the programming language from code content.
    /// </summary>
    private static DocumentType DetectCodeLanguage(string content)
    {
        // Check C# patterns first (most specific)
        if (content.Contains("using ", StringComparison.OrdinalIgnoreCase) || 
            content.Contains("namespace ", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("public class", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("private class", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("public interface", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("public enum", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("public static", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("private static", StringComparison.OrdinalIgnoreCase))
        {
            return DocumentType.CSharpCode;
        }
        
        // Check TypeScript/JavaScript patterns second
        if (content.Contains("function ", StringComparison.OrdinalIgnoreCase) || 
            content.Contains("const ", StringComparison.OrdinalIgnoreCase) || 
            content.Contains("let ", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("var ", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("export ", StringComparison.OrdinalIgnoreCase))
        {
            return DocumentType.TypeScriptCode;
        }
        
        // Check Python patterns last (but also check for Python-specific patterns)
        if (content.Contains("def ", StringComparison.OrdinalIgnoreCase) || 
            content.Contains("import ", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("from ", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("if __name__", StringComparison.OrdinalIgnoreCase) ||
            (content.Contains("class ", StringComparison.OrdinalIgnoreCase) && 
             !content.Contains("public class", StringComparison.OrdinalIgnoreCase) &&
             !content.Contains("private class", StringComparison.OrdinalIgnoreCase)))
        {
            return DocumentType.PythonCode;
        }
        
        return DocumentType.Text;
    }

    /// <summary>
    /// Disposes of resources.
    /// </summary>
    public void Dispose()
    {
        if (_ocrService is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}


