# Hexagonal Architecture & IITDD Integration Strategy

## Overview

Following clean architecture principles, we will implement a hexagonal architecture with IITDD (Integration Interface Test-Driven Development) to ensure proper separation of concerns, testability, and maintainability for the IndFusion Semantic RAG platform.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                        │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │   Use Cases     │  │   Domain        │  │   Ports     │ │
│  │   (Services)    │  │   (Entities)    │  │ (Interfaces)│ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                        │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │   Adapters      │  │   External      │  │   IITDD     │ │
│  │   (Implement.)  │  │   Services      │  │   Tests     │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## Core Ports (Interfaces) - Application Layer

### Document Processing Ports

```csharp
// Application/Ports/DocumentProcessing/
public interface IDocumentIngestionService
{
    Task<DocumentIngestionResult> IngestDocumentAsync(DocumentSource source, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentMetadata>> GetIngestedDocumentsAsync(CancellationToken cancellationToken = default);
    Task<DocumentIngestionResult> UpdateDocumentAsync(string documentId, DocumentSource source, CancellationToken cancellationToken = default);
}

public interface IDocumentMetadataRepository
{
    Task<DocumentMetadata> GetMetadataAsync(string documentId, CancellationToken cancellationToken = default);
    Task SaveMetadataAsync(DocumentMetadata metadata, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentMetadata>> SearchMetadataAsync(SearchCriteria criteria, CancellationToken cancellationToken = default);
    Task DeleteMetadataAsync(string documentId, CancellationToken cancellationToken = default);
}

public interface IDocumentTruthSystem
{
    Task<TruthValidationResult> ValidateDocumentAsync(string documentId, CancellationToken cancellationToken = default);
    Task<TruthScore> CalculateTruthScoreAsync(DocumentContent content, CancellationToken cancellationToken = default);
    Task<IEnumerable<TruthViolation>> GetTruthViolationsAsync(string documentId, CancellationToken cancellationToken = default);
}
```

### Processing Pipeline Ports

```csharp
// Application/Ports/Processing/
public interface IDocumentProcessingPipeline
{
    Task<ProcessingResult> ProcessDocumentAsync(DocumentInput input, ProcessingOptions options, CancellationToken cancellationToken = default);
    Task<ProcessingStatus> GetProcessingStatusAsync(string processingId, CancellationToken cancellationToken = default);
    Task CancelProcessingAsync(string processingId, CancellationToken cancellationToken = default);
}

public interface IOCRService
{
    Task<OCRResult> ExtractTextAsync(ImageInput image, CancellationToken cancellationToken = default);
    Task<OCRResult> ExtractTextFromPdfAsync(PdfInput pdf, CancellationToken cancellationToken = default);
    Task<OCRResult> ExtractTextFromDocumentAsync(DocumentInput document, CancellationToken cancellationToken = default);
}

public interface ILLMExtractionService
{
    Task<ExtractionResult> ExtractEntitiesAsync(string text, ExtractionOptions options, CancellationToken cancellationToken = default);
    Task<ExtractionResult> ExtractRelationshipsAsync(string text, CancellationToken cancellationToken = default);
    Task<ExtractionResult> ExtractKeyPhrasesAsync(string text, CancellationToken cancellationToken = default);
}
```

### Semantic Search Ports

```csharp
// Application/Ports/SemanticSearch/
public interface IVectorSearchService
{
    Task<SearchResult> SearchSimilarAsync(string query, SearchOptions options, CancellationToken cancellationToken = default);
    Task<SearchResult> SearchByEmbeddingAsync(float[] embedding, SearchOptions options, CancellationToken cancellationToken = default);
    Task<string> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
}

public interface IKnowledgeGraphService
{
    Task<GraphQueryResult> QueryGraphAsync(GraphQuery query, CancellationToken cancellationToken = default);
    Task<GraphNode> AddNodeAsync(GraphNode node, CancellationToken cancellationToken = default);
    Task<GraphEdge> AddEdgeAsync(GraphEdge edge, CancellationToken cancellationToken = default);
    Task<IEnumerable<GraphNode>> GetRelatedNodesAsync(string nodeId, CancellationToken cancellationToken = default);
}
```

### Infrastructure Ports

```csharp
// Application/Ports/Infrastructure/
public interface IStorageProvider
{
    Task<StorageResult> StoreAsync(string key, byte[] data, StorageOptions options, CancellationToken cancellationToken = default);
    Task<byte[]> RetrieveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}

public interface ISecretsVault
{
    Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken = default);
    Task SetSecretAsync(string secretName, string secretValue, CancellationToken cancellationToken = default);
    Task<bool> SecretExistsAsync(string secretName, CancellationToken cancellationToken = default);
}

public interface IQueueDispatcher
{
    Task<string> EnqueueAsync<T>(T message, QueueOptions options, CancellationToken cancellationToken = default);
    Task<T> DequeueAsync<T>(string queueName, CancellationToken cancellationToken = default);
    Task<QueueStatus> GetQueueStatusAsync(string queueName, CancellationToken cancellationToken = default);
    Task<int> GetQueueLengthAsync(string queueName, CancellationToken cancellationToken = default);
}
```

## Repository Structure with Clean Architecture

```
src/
├── IndFusion.SemanticRag.Application/          # Application Layer
│   ├── Ports/                                  # Interface definitions
│   │   ├── DocumentProcessing/
│   │   │   ├── IDocumentIngestionService.cs
│   │   │   ├── IDocumentMetadataRepository.cs
│   │   │   └── IDocumentTruthSystem.cs
│   │   ├── Processing/
│   │   │   ├── IDocumentProcessingPipeline.cs
│   │   │   ├── IOCRService.cs
│   │   │   └── ILLMExtractionService.cs
│   │   ├── SemanticSearch/
│   │   │   ├── IVectorSearchService.cs
│   │   │   └── IKnowledgeGraphService.cs
│   │   └── Infrastructure/
│   │       ├── IStorageProvider.cs
│   │       ├── ISecretsVault.cs
│   │       └── IQueueDispatcher.cs
│   ├── Services/                               # Use cases/Application services
│   │   ├── DocumentIngestionService.cs
│   │   ├── SemanticSearchService.cs
│   │   ├── KnowledgeGraphService.cs
│   │   └── MCPToolIntegrationService.cs
│   ├── Models/                                 # Domain models
│   │   ├── DocumentMetadata.cs
│   │   ├── ProcessingResult.cs
│   │   ├── SearchCriteria.cs
│   │   ├── GraphNode.cs
│   │   └── GraphEdge.cs
│   └── Exceptions/                             # Domain exceptions
│       ├── DocumentProcessingException.cs
│       ├── SemanticSearchException.cs
│       └── KnowledgeGraphException.cs
├── IndFusion.SemanticRag.Infrastructure/       # Infrastructure Layer
│   ├── Adapters/                               # Interface implementations
│   │   ├── DocumentProcessing/
│   │   │   ├── ExxerAIDocumentIngestionAdapter.cs
│   │   │   ├── QdrantDocumentMetadataAdapter.cs
│   │   │   └── Neo4jTruthSystemAdapter.cs
│   │   ├── Processing/
│   │   │   ├── TesseractDocumentProcessingAdapter.cs
│   │   │   ├── TesseractOCRAdapter.cs
│   │   │   └── OllamaLLMExtractionAdapter.cs
│   │   ├── SemanticSearch/
│   │   │   ├── QdrantVectorSearchAdapter.cs
│   │   │   └── Neo4jKnowledgeGraphAdapter.cs
│   │   └── Infrastructure/
│   │       ├── LocalFileStorageAdapter.cs
│   │       ├── LocalSecretsAdapter.cs
│   │       └── InMemoryQueueAdapter.cs
│   ├── Configuration/                          # Infrastructure configuration
│   │   ├── ServiceCollectionExtensions.cs
│   │   ├── Options/
│   │   │   ├── QdrantOptions.cs
│   │   │   ├── Neo4jOptions.cs
│   │   │   ├── OllamaOptions.cs
│   │   │   └── TesseractOptions.cs
│   │   └── HealthChecks/
│   │       ├── QdrantHealthCheck.cs
│   │       ├── Neo4jHealthCheck.cs
│   │       └── OllamaHealthCheck.cs
│   └── Extensions/                             # Infrastructure extensions
│       ├── DependencyInjectionExtensions.cs
│       └── ConfigurationExtensions.cs
└── IndFusion.SemanticRag.Tests/                # Test Layer
    ├── Integration/
    │   └── IITDD/                              # Integration Interface Tests
    │       ├── Fixtures/
    │       │   ├── DocumentIngestionFixture.cs
    │       │   ├── ProcessingPipelineFixture.cs
    │       │   ├── SemanticSearchFixture.cs
    │       │   └── KnowledgeGraphFixture.cs
    │       ├── Builders/
    │       │   ├── DocumentSourceBuilder.cs
    │       │   ├── ProcessingResultBuilder.cs
    │       │   ├── SearchResultBuilder.cs
    │       │   └── GraphNodeBuilder.cs
    │       ├── DocumentProcessing/
    │       │   ├── DocumentIngestionServiceIITDDTests.cs
    │       │   └── DocumentMetadataRepositoryIITDDTests.cs
    │       ├── Processing/
    │       │   ├── DocumentProcessingPipelineIITDDTests.cs
    │       │   └── OCRServiceIITDDTests.cs
    │       ├── SemanticSearch/
    │       │   ├── VectorSearchServiceIITDDTests.cs
    │       │   └── KnowledgeGraphServiceIITDDTests.cs
    │       └── Infrastructure/
    │           ├── StorageProviderIITDDTests.cs
    │           └── QueueDispatcherIITDDTests.cs
    └── Unit/
        ├── Application/
        │   ├── DocumentIngestionServiceTests.cs
        │   ├── SemanticSearchServiceTests.cs
        │   └── KnowledgeGraphServiceTests.cs
        └── Infrastructure/
            ├── QdrantVectorSearchAdapterTests.cs
            └── Neo4jKnowledgeGraphAdapterTests.cs
```

## IITDD Test Structure

### Test Fixtures and Builders

```csharp
// Tests/Integration/IITDD/Fixtures/
public class DocumentIngestionFixture
{
    public DocumentSource CreateValidDocumentSource() => new DocumentSourceBuilder()
        .WithContent("Sample document content")
        .WithMetadata(new DocumentMetadataBuilder().Build())
        .WithType(DocumentType.Text)
        .Build();

    public DocumentIngestionResult CreateSuccessfulResult() => new DocumentIngestionResultBuilder()
        .WithDocumentId("test-doc-123")
        .WithStatus(IngestionStatus.Success)
        .WithTimestamp(DateTime.UtcNow)
        .Build();

    public DocumentIngestionResult CreateFailedResult(string errorMessage) => new DocumentIngestionResultBuilder()
        .WithDocumentId("test-doc-456")
        .WithStatus(IngestionStatus.Failed)
        .WithErrorMessage(errorMessage)
        .Build();
}

public class SemanticSearchFixture
{
    public SearchQuery CreateValidSearchQuery() => new SearchQueryBuilder()
        .WithText("Find similar documents")
        .WithLimit(10)
        .WithThreshold(0.8f)
        .Build();

    public SearchResult CreateSearchResult() => new SearchResultBuilder()
        .WithQuery("test query")
        .WithResults(new List<SearchResultItem>
        {
            new SearchResultItemBuilder()
                .WithDocumentId("doc-1")
                .WithScore(0.95f)
                .WithSnippet("Relevant content snippet")
                .Build()
        })
        .Build();
}

public class KnowledgeGraphFixture
{
    public GraphNode CreateValidNode() => new GraphNodeBuilder()
        .WithId("node-1")
        .WithType("Document")
        .WithProperties(new Dictionary<string, object>
        {
            ["title"] = "Test Document",
            ["author"] = "Test Author"
        })
        .Build();

    public GraphEdge CreateValidEdge() => new GraphEdgeBuilder()
        .WithId("edge-1")
        .WithSourceNodeId("node-1")
        .WithTargetNodeId("node-2")
        .WithType("RELATES_TO")
        .WithProperties(new Dictionary<string, object>
        {
            ["strength"] = 0.8f
        })
        .Build();
}
```

### Integration Interface Tests

```csharp
// Tests/Integration/IITDD/DocumentProcessing/
[TestClass]
public class DocumentIngestionServiceIITDDTests
{
    private IDocumentIngestionService _service;
    private DocumentIngestionFixture _fixture;
    private IServiceProvider _serviceProvider;

    [TestInitialize]
    public void Setup()
    {
        _fixture = new DocumentIngestionFixture();
        _serviceProvider = TestServiceProvider.Create();
        _service = _serviceProvider.GetRequiredService<IDocumentIngestionService>();
    }

    [TestMethod]
    public async Task IngestDocument_WithValidSource_ShouldReturnSuccessResult()
    {
        // Arrange
        var source = _fixture.CreateValidDocumentSource();
        
        // Act
        var result = await _service.IngestDocumentAsync(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(IngestionStatus.Success);
        result.DocumentId.Should().NotBeNullOrEmpty();
        result.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [TestMethod]
    public async Task IngestDocument_WithInvalidSource_ShouldReturnFailedResult()
    {
        // Arrange
        var source = new DocumentSourceBuilder()
            .WithContent("")
            .WithType(DocumentType.Unknown)
            .Build();
        
        // Act
        var result = await _service.IngestDocumentAsync(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(IngestionStatus.Failed);
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task GetIngestedDocuments_ShouldReturnAllDocuments()
    {
        // Arrange
        var source1 = _fixture.CreateValidDocumentSource();
        var source2 = _fixture.CreateValidDocumentSource();
        
        await _service.IngestDocumentAsync(source1);
        await _service.IngestDocumentAsync(source2);
        
        // Act
        var documents = await _service.GetIngestedDocumentsAsync();
        
        // Assert
        documents.Should().NotBeNull();
        documents.Should().HaveCount(2);
        documents.Should().AllSatisfy(doc => doc.Id.Should().NotBeNullOrEmpty());
    }
}

// Tests/Integration/IITDD/SemanticSearch/
[TestClass]
public class VectorSearchServiceIITDDTests
{
    private IVectorSearchService _service;
    private SemanticSearchFixture _fixture;
    private IServiceProvider _serviceProvider;

    [TestInitialize]
    public void Setup()
    {
        _fixture = new SemanticSearchFixture();
        _serviceProvider = TestServiceProvider.Create();
        _service = _serviceProvider.GetRequiredService<IVectorSearchService>();
    }

    [TestMethod]
    public async Task SearchSimilar_WithValidQuery_ShouldReturnResults()
    {
        // Arrange
        var query = _fixture.CreateValidSearchQuery();
        
        // Act
        var result = await _service.SearchSimilarAsync(query.Text, new SearchOptions());
        
        // Assert
        result.Should().NotBeNull();
        result.Query.Should().Be(query.Text);
        result.Results.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GenerateEmbedding_WithValidText_ShouldReturnEmbedding()
    {
        // Arrange
        var text = "This is a test document for embedding generation";
        
        // Act
        var embedding = await _service.GenerateEmbeddingAsync(text);
        
        // Assert
        embedding.Should().NotBeNullOrEmpty();
        embedding.Should().HaveCount(1536); // OpenAI embedding dimension
    }
}
```

## Implementation Strategy

### Phase 1: Interface Definition (Sprint 1)
1. **Define Application Ports**: Create all interface definitions in Application layer
2. **Create Domain Models**: Define entities and value objects
3. **Document Contracts**: Create comprehensive interface documentation
4. **IITDD Test Structure**: Set up test fixtures and builders
5. **Dependency Injection Setup**: Configure service registration structure

### Phase 2: Stub Implementations (Sprint 2)
1. **Create Stub Adapters**: Implement interfaces with stub/mock implementations
2. **IITDD Test Implementation**: Write integration tests against stubs
3. **Service Registration**: Configure dependency injection with stubs
4. **Validation**: Ensure all IITDD tests pass with stubs
5. **Health Checks**: Implement health check endpoints

### Phase 3: Real Implementation (Sprint 3+)
1. **External Service Integration**: Implement real adapters for external services
2. **Configuration**: Set up real service configurations
3. **Testing**: Run IITDD tests against real implementations
4. **Performance**: Optimize and validate performance requirements
5. **Monitoring**: Add comprehensive logging and telemetry

## Benefits of This Approach

### 🧪 **Testability**
- IITDD ensures all interfaces are properly tested
- Clear separation between unit and integration tests
- Comprehensive test coverage with fixtures and builders

### 🔄 **Flexibility**
- Easy to swap implementations without changing application logic
- Support for multiple implementations of the same interface
- Feature flags can control which implementation is used

### 🛠️ **Maintainability**
- Clear separation of concerns and dependencies
- Single responsibility principle enforced
- Easy to understand and modify

### 📈 **Scalability**
- Infrastructure can be scaled independently
- Application logic is decoupled from infrastructure concerns
- Support for microservices architecture

### 🛡️ **Reliability**
- Integration tests catch interface contract violations early
- Health checks ensure service availability
- Comprehensive error handling and logging

## External Service Integration

### Vector Database (Qdrant)
```csharp
public class QdrantVectorSearchAdapter : IVectorSearchService
{
    private readonly QdrantClient _client;
    private readonly ILogger<QdrantVectorSearchAdapter> _logger;

    public async Task<SearchResult> SearchSimilarAsync(string query, SearchOptions options, CancellationToken cancellationToken = default)
    {
        var embedding = await GenerateEmbeddingAsync(query, cancellationToken);
        var searchRequest = new SearchRequest
        {
            Vector = embedding,
            Limit = options.Limit,
            ScoreThreshold = options.Threshold
        };

        var response = await _client.SearchAsync(searchRequest, cancellationToken);
        return MapToSearchResult(response);
    }
}
```

### Graph Database (Neo4j)
```csharp
public class Neo4jKnowledgeGraphAdapter : IKnowledgeGraphService
{
    private readonly IDriver _driver;
    private readonly ILogger<Neo4jKnowledgeGraphAdapter> _logger;

    public async Task<GraphQueryResult> QueryGraphAsync(GraphQuery query, CancellationToken cancellationToken = default)
    {
        using var session = _driver.AsyncSession();
        var result = await session.RunAsync(query.Cypher, query.Parameters, cancellationToken);
        return await MapToGraphQueryResult(result);
    }
}
```

### Document Processing (Tesseract OCR)
```csharp
public class TesseractDocumentProcessingAdapter : IDocumentProcessingPipeline
{
    private readonly TesseractEngine _tesseractEngine;
    private readonly ILogger<TesseractDocumentProcessingAdapter> _logger;

    public async Task<ProcessingResult> ProcessDocumentAsync(DocumentInput input, ProcessingOptions options, CancellationToken cancellationToken = default)
    {
        using var img = Pix.LoadFromMemory(input.Content);
        using var page = _tesseractEngine.Process(img);
        var text = page.GetText();
        var confidence = page.GetMeanConfidence();
        
        return new ProcessingResult
        {
            Text = text,
            Confidence = confidence,
            Status = ProcessingStatus.Completed
        };
    }
}
```

### LLM Integration (Ollama)
```csharp
public class OllamaLLMExtractionAdapter : ILLMExtractionService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;
    private readonly ILogger<OllamaLLMExtractionAdapter> _logger;

    public async Task<ExtractionResult> ExtractEntitiesAsync(string text, ExtractionOptions options, CancellationToken cancellationToken = default)
    {
        var prompt = $@"
Extract entities from the following text and return as JSON:
Text: {text}

Return format:
{{
  ""entities"": [
    {{""name"": ""entity_name"", ""type"": ""entity_type"", ""confidence"": 0.95}}
  ]
}}";

        var request = new OllamaRequest
        {
            Model = _options.Model,
            Prompt = prompt,
            Stream = false
        };

        var response = await _httpClient.PostAsJsonAsync($"{_options.BaseUrl}/api/generate", request, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken: cancellationToken);
        
        return ParseExtractionResult(result.Response);
    }

    public async Task<EmbeddingResult> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var request = new OllamaEmbeddingRequest
        {
            Model = _options.EmbeddingModel,
            Prompt = text
        };

        var response = await _httpClient.PostAsJsonAsync($"{_options.BaseUrl}/api/embeddings", request, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>(cancellationToken: cancellationToken);
        
        return new EmbeddingResult
        {
            Embedding = result.Embedding,
            Model = _options.EmbeddingModel
        };
    }
}
```

## Configuration and Dependency Injection

```csharp
// Infrastructure/Configuration/ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSemanticRagInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register options
        services.Configure<QdrantOptions>(configuration.GetSection("Qdrant"));
        services.Configure<Neo4jOptions>(configuration.GetSection("Neo4j"));
        services.Configure<OllamaOptions>(configuration.GetSection("Ollama"));
        services.Configure<TesseractOptions>(configuration.GetSection("Tesseract"));

        // Register adapters
        services.AddScoped<IVectorSearchService, QdrantVectorSearchAdapter>();
        services.AddScoped<IKnowledgeGraphService, Neo4jKnowledgeGraphAdapter>();
        services.AddScoped<IDocumentProcessingPipeline, TesseractDocumentProcessingAdapter>();
        services.AddScoped<ILLMExtractionService, OllamaLLMExtractionAdapter>();
        services.AddScoped<IStorageProvider, LocalFileStorageAdapter>();
        services.AddScoped<ISecretsVault, LocalSecretsAdapter>();
        services.AddScoped<IQueueDispatcher, InMemoryQueueAdapter>();

        // Register health checks
        services.AddHealthChecks()
            .AddCheck<QdrantHealthCheck>("qdrant")
            .AddCheck<Neo4jHealthCheck>("neo4j")
            .AddCheck<OllamaHealthCheck>("ollama");

        return services;
    }
}
```

This hexagonal architecture approach ensures that the IndFusion Semantic RAG platform is built with proper separation of concerns, comprehensive testing, and the flexibility to adapt to changing requirements while maintaining high quality and reliability.
