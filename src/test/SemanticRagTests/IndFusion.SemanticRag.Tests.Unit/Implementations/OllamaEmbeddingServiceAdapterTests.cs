using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Adapters;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.Tests.Unit.Shared;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Meziantou.Extensions.Logging.Xunit.v3;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Implementations;

/// <summary>
/// TDD tests for OllamaEmbeddingServiceAdapter implementation behavior.
/// These tests validate the concrete implementation behavior, not the interface contract.
/// Tests use real adapter instances with mocked underlying dependencies.
/// </summary>
public class OllamaEmbeddingServiceAdapterTests : BaseTDDTest<OllamaEmbeddingServiceAdapter>
{
    private HttpClient _httpClient = null!;
    private IOptions<OllamaOptions> _options = null!;
    private ILogger<OllamaEmbeddingServiceAdapter> _logger = null!;
    private OllamaOptions _ollamaOptions = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaEmbeddingServiceAdapterTests"/> class.
    /// </summary>
    /// <param name="output">The test output helper for logging.</param>
    public OllamaEmbeddingServiceAdapterTests(ITestOutputHelper output) : base(output)
    {
    }

    protected override OllamaEmbeddingServiceAdapter CreateImplementation()
    {
        // ✅ Phase 1.3: Use mocked HttpClient consistently - prevents hanging on real HTTP calls
        // For validation tests, validation happens before HTTP calls, so mock is safe
        _httpClient = CreateMockHttpClient(new float[] { 0.1f, 0.2f, 0.3f });
        _logger = Logger; // Use Meziantou logger from base class

        _ollamaOptions = new OllamaOptions
        {
            BaseUrl = "http://localhost:11434",
            EmbeddingModel = "nomic-embed-text",
            EmbeddingDimension = 768,
            MaxTextLength = 8192,
            MaxConcurrency = 5,
            TimeoutSeconds = 30
        };
        _options = Options.Create(_ollamaOptions);

        return new OllamaEmbeddingServiceAdapter(_httpClient, _logger, _options);
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingAsync_WithNullText_ShouldReturnFailure()
    {
        // ✅ TDD: Test implementation behavior - adapter validates before calling HTTP
        var result = await Implementation.GenerateEmbeddingAsync(null!, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ TDD: Assert implementation handles invalid input correctly
        AssertResultFailure(result);
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingAsync_WithEmptyText_ShouldReturnFailure()
    {
        // ✅ TDD: Test implementation behavior
        var result = await Implementation.GenerateEmbeddingAsync(string.Empty, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ TDD: Assert implementation handles invalid input correctly
        AssertResultFailure(result);
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingAsync_WithWhitespaceText_ShouldReturnFailure()
    {
        // ✅ TDD: Test implementation behavior
        var result = await Implementation.GenerateEmbeddingAsync("   ", cancellationToken: TestContext.Current.CancellationToken);

        // ✅ TDD: Assert implementation handles invalid input correctly
        AssertResultFailure(result);
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingsAsync_WithNullTexts_ShouldReturnFailure()
    {
        // ✅ TDD: Test implementation behavior
        var result = await Implementation.GenerateEmbeddingsAsync(null!, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ TDD: Assert implementation handles invalid input correctly
        AssertResultFailure(result);
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public void GetEmbeddingDimension_ShouldReturnConfiguredDimension()
    {
        // ✅ TDD: Test implementation behavior - adapter returns configured dimension
        var dimension = Implementation.GetEmbeddingDimension();

        // ✅ TDD: Assert implementation-specific behavior
        dimension.ShouldBe(_ollamaOptions.EmbeddingDimension);
        dimension.ShouldBeGreaterThan(0);
    }

    [Fact(Timeout = 5000)]
    public void GetMaxTextLength_ShouldReturnConfiguredMaxLength()
    {
        // ✅ TDD: Test implementation behavior - adapter returns configured max length
        var maxLength = Implementation.GetMaxTextLength();

        // ✅ TDD: Assert implementation-specific behavior
        maxLength.ShouldBe(_ollamaOptions.MaxTextLength);
        maxLength.ShouldBeGreaterThan(0);
    }

    [Fact(Timeout = 5000)]
    public void ValidateTextLength_WithValidText_ShouldReturnSuccess()
    {
        // ✅ TDD: Test implementation behavior
        var validText = "This is a valid text";

        var result = Implementation.ValidateTextLength(validText);

        // ✅ TDD: Assert implementation behavior
        AssertResultSuccess(result);
    }

    [Fact(Timeout = 5000)]
    public void ValidateTextLength_WithTooLongText_ShouldReturnFailure()
    {
        // ✅ TDD: Test implementation behavior - adapter validates text length
        var tooLongText = new string('a', _ollamaOptions.MaxTextLength + 1);

        var result = Implementation.ValidateTextLength(tooLongText);

        // ✅ TDD: Assert implementation handles invalid input correctly
        AssertResultFailure(result);
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public void ValidateTextLength_WithNullText_ShouldReturnFailure()
    {
        // ✅ TDD: Test implementation behavior
        var result = Implementation.ValidateTextLength(null!);

        // ✅ TDD: Assert implementation handles invalid input correctly
        AssertResultFailure(result);
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingWithMetadataAsync_WithNullText_ShouldReturnFailure()
    {
        // ✅ TDD: Test implementation behavior
        var metadata = new Dictionary<string, object> { ["source"] = "test" };

        var result = await Implementation.GenerateEmbeddingWithMetadataAsync(null!, metadata, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ TDD: Assert implementation handles invalid input correctly
        AssertResultFailure(result);
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public async Task GenerateEmbeddingWithMetadataAsync_WithEmptyText_ShouldReturnFailure()
    {
        // ✅ TDD: Test implementation behavior
        var metadata = new Dictionary<string, object> { ["source"] = "test" };

        var result = await Implementation.GenerateEmbeddingWithMetadataAsync(string.Empty, metadata, cancellationToken: TestContext.Current.CancellationToken);

        // ✅ TDD: Assert implementation handles invalid input correctly
        AssertResultFailure(result);
        result.Error.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// Creates a mocked HttpClient with the specified embedding response.
    /// Used to prevent hanging on real HTTP calls in unit tests.
    /// </summary>
    /// <param name="expectedEmbedding">The embedding array to return in the mock response.</param>
    /// <returns>A mocked HttpClient instance.</returns>
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

    /// <summary>
    /// Mock HttpMessageHandler for testing HTTP responses without real network calls.
    /// </summary>
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;
        private readonly HttpStatusCode _statusCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpMessageHandler"/> class.
        /// </summary>
        /// <param name="response">The response body to return.</param>
        /// <param name="statusCode">The HTTP status code to return.</param>
        public MockHttpMessageHandler(string response, HttpStatusCode statusCode)
        {
            _response = response;
            _statusCode = statusCode;
        }

        /// <inheritdoc />
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