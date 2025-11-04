using System.Net;
using System.Text.Json;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Infrastructure.Adapters;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace IndFusion.SemanticRag.Tests.Infratructure.Tests.Infrastructure.Adapters;

/// <summary>
/// Unit tests for the Ollama embedding service adapter.
/// </summary>
public class OllamaEmbeddingServiceAdapterTests
{
    private readonly ILogger<OllamaEmbeddingServiceAdapter> _logger;
    private readonly IOptions<OllamaOptions> _options;
    private readonly OllamaOptions _ollamaOptions;

    public OllamaEmbeddingServiceAdapterTests()
    {
        _logger = Substitute.For<ILogger<OllamaEmbeddingServiceAdapter>>();
        _ollamaOptions = new OllamaOptions
        {
            BaseUrl = "http://localhost:11434",
            Model = "nomic-embed-text",
            EmbeddingDimension = 768,
            MaxTextLength = 8192,
            MaxConcurrency = 5,
            TimeoutSeconds = 30
        };
        _options = Options.Create(_ollamaOptions);
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingAsync_WithValidText_ShouldReturnEmbedding()
    {
        // Arrange
        var text = "This is a test text";
        var expectedEmbedding = new float[] { 0.1f, 0.2f, 0.3f };
        
        var httpClient = CreateMockHttpClient(expectedEmbedding);
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingAsync(text, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Length.ShouldBe(3);
        result.Value[0].ShouldBe(0.1f);
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingAsync_WithNullText_ShouldReturnFailure()
    {
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        // Arrange
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation fails first)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingAsync(null!, CancellationToken.None);

        // Assert: Use error code assertion
        result.ShouldFailWith(ErrorCodes.ParameterNullOrWhitespace);
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingAsync_WithEmptyText_ShouldReturnFailure()
    {
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        // Arrange
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation fails first)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingAsync(string.Empty, CancellationToken.None);

        // Assert: Use error code assertion
        result.ShouldFailWith(ErrorCodes.ParameterNullOrWhitespace);
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingAsync_WithTextTooLong_ShouldReturnFailure()
    {
        // Arrange
        // Use mocked HttpClient to avoid infrastructure dependency
        // Validation should happen before HTTP call, so this shouldn't be called
        var mockEmbedding = new float[] { 0.1f };
        var httpClient = CreateMockHttpClient(mockEmbedding);
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);
        var longText = new string('a', _ollamaOptions.MaxTextLength + 1);

        // Act
        var result = await adapter.GenerateEmbeddingAsync(longText, CancellationToken.None);

        // Assert: Use error code assertion - validation should fail before HTTP call
        result.ShouldFailWith(ErrorCodes.ValueOutOfRange);
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingsAsync_WithValidTexts_ShouldReturnEmbeddings()
    {
        // Arrange
        var texts = new List<string> { "Text 1", "Text 2", "Text 3" }.AsReadOnly();
        var expectedEmbedding = new float[] { 0.1f, 0.2f, 0.3f };
        
        var httpClient = CreateMockHttpClient(expectedEmbedding);
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingsAsync(texts, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(3);
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingWithMetadataAsync_WithValidText_ShouldReturnVectorEmbedding()
    {
        // Arrange
        var text = "This is a test text";
        var metadata = new Dictionary<string, object> { { "source", "test" } };
        var expectedEmbedding = new float[] { 0.1f, 0.2f, 0.3f };
        
        var httpClient = CreateMockHttpClient(expectedEmbedding);
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingWithMetadataAsync(text, metadata, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(text);
        result.Value.Embedding.Length.ShouldBe(3);
        result.Value.Metadata.ShouldContainKey("source");
    }

    [Fact(Timeout = 5000)]
    public void GetEmbeddingDimension_ShouldReturnConfiguredDimension()
    {
        // Arrange
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation/test logic doesn't call HTTP)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var dimension = adapter.GetEmbeddingDimension();

        // Assert
        dimension.ShouldBe(_ollamaOptions.EmbeddingDimension);
    }

    [Fact(Timeout = 5000)]
    public void GetMaxTextLength_ShouldReturnConfiguredMaxLength()
    {
        // Arrange
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation/test logic doesn't call HTTP)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var maxLength = adapter.GetMaxTextLength();

        // Assert
        maxLength.ShouldBe(_ollamaOptions.MaxTextLength);
    }

    [Fact(Timeout = 5000)]
    public void ValidateTextLength_WithValidText_ShouldReturnSuccess()
    {
        // Arrange
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation/test logic doesn't call HTTP)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);
        var text = "Valid text";

        // Act
        var result = adapter.ValidateTextLength(text);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public void ValidateTextLength_WithTextTooLong_ShouldReturnFailure()
    {
        // Arrange
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation/test logic doesn't call HTTP)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);
        var longText = new string('a', _ollamaOptions.MaxTextLength + 1);

        // Act
        var result = adapter.ValidateTextLength(longText);

        // Assert: Use error code assertion
        result.ShouldFailWith(ErrorCodes.ValueOutOfRange);
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingsAsync_WithNullTexts_ShouldReturnFailure()
    {
        // Arrange
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation/test logic doesn't call HTTP)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingsAsync(null!, CancellationToken.None);

        // Assert: Use error code assertion
        result.ShouldFailWith(ErrorCodes.CollectionEmpty);
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingsAsync_WithEmptyTexts_ShouldReturnFailure()
    {
        // Arrange
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation/test logic doesn't call HTTP)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingsAsync(new List<string>().AsReadOnly(), CancellationToken.None);

        // Assert: Use error code assertion
        result.ShouldFailWith(ErrorCodes.CollectionEmpty);
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation/test logic doesn't call HTTP)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingAsync("test text", cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingsAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation/test logic doesn't call HTTP)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);
        var texts = new List<string> { "Text 1", "Text 2" }.AsReadOnly();

        // Act
        var result = await adapter.GenerateEmbeddingsAsync(texts, cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingWithMetadataAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation/test logic doesn't call HTTP)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingWithMetadataAsync(
            "test text",
            new Dictionary<string, object>(),
            cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
    }

    [Fact(Timeout = 5000)]
    public async Task GetModelInfoAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        // ✅ Phase 1.3: Use mocked HttpClient consistently - validation tests don't need real HTTP
        var httpClient = CreateMockHttpClient(new float[] { 0.1f }); // Mock not used (validation/test logic doesn't call HTTP)
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GetModelInfoAsync(cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
    }

    private HttpClient CreateMockHttpClient(float[] expectedEmbedding)
    {
        var response = new
        {
            Embedding = expectedEmbedding  // Match OllamaEmbeddingResponse record property name (uppercase E)
        };

        var json = JsonSerializer.Serialize(response);
        var handler = new MockHttpMessageHandler(json, HttpStatusCode.OK);
        return new HttpClient(handler);
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;
        private readonly HttpStatusCode _statusCode;

        public MockHttpMessageHandler(string response, HttpStatusCode statusCode)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_response)
            });
        }
    }
}

