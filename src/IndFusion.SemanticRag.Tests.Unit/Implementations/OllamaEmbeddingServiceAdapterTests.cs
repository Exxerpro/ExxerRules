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

	protected override OllamaEmbeddingServiceAdapter CreateImplementation()
	{
		// ✅ TDD: Use real HttpClient with mocked responses (or test double)
		_httpClient = new HttpClient();
		_logger = NullLogger<OllamaEmbeddingServiceAdapter>.Instance;
		
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

	[Fact]
	public async Task GenerateEmbeddingAsync_WithNullText_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior - adapter validates before calling HTTP
		var result = await Implementation.GenerateEmbeddingAsync(null!);
		
		// ✅ TDD: Assert implementation handles invalid input correctly
		AssertResultFailure(result);
		result.Error.ShouldNotBeNullOrEmpty();
	}

	[Fact]
	public async Task GenerateEmbeddingAsync_WithEmptyText_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var result = await Implementation.GenerateEmbeddingAsync(string.Empty);
		
		// ✅ TDD: Assert implementation handles invalid input correctly
		AssertResultFailure(result);
		result.Error.ShouldNotBeNullOrEmpty();
	}

	[Fact]
	public async Task GenerateEmbeddingAsync_WithWhitespaceText_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var result = await Implementation.GenerateEmbeddingAsync("   ");
		
		// ✅ TDD: Assert implementation handles invalid input correctly
		AssertResultFailure(result);
		result.Error.ShouldNotBeNullOrEmpty();
	}

	[Fact]
	public async Task GenerateEmbeddingsAsync_WithNullTexts_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var result = await Implementation.GenerateEmbeddingsAsync(null!);
		
		// ✅ TDD: Assert implementation handles invalid input correctly
		AssertResultFailure(result);
		result.Error.ShouldNotBeNullOrEmpty();
	}

	[Fact]
	public void GetEmbeddingDimension_ShouldReturnConfiguredDimension()
	{
		// ✅ TDD: Test implementation behavior - adapter returns configured dimension
		var dimension = Implementation.GetEmbeddingDimension();
		
		// ✅ TDD: Assert implementation-specific behavior
		dimension.ShouldBe(_ollamaOptions.EmbeddingDimension);
		dimension.ShouldBeGreaterThan(0);
	}

	[Fact]
	public void GetMaxTextLength_ShouldReturnConfiguredMaxLength()
	{
		// ✅ TDD: Test implementation behavior - adapter returns configured max length
		var maxLength = Implementation.GetMaxTextLength();
		
		// ✅ TDD: Assert implementation-specific behavior
		maxLength.ShouldBe(_ollamaOptions.MaxTextLength);
		maxLength.ShouldBeGreaterThan(0);
	}

	[Fact]
	public void ValidateTextLength_WithValidText_ShouldReturnSuccess()
	{
		// ✅ TDD: Test implementation behavior
		var validText = "This is a valid text";
		
		var result = Implementation.ValidateTextLength(validText);
		
		// ✅ TDD: Assert implementation behavior
		AssertResultSuccess(result);
	}

	[Fact]
	public void ValidateTextLength_WithTooLongText_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior - adapter validates text length
		var tooLongText = new string('a', _ollamaOptions.MaxTextLength + 1);
		
		var result = Implementation.ValidateTextLength(tooLongText);
		
		// ✅ TDD: Assert implementation handles invalid input correctly
		AssertResultFailure(result);
		result.Error.ShouldNotBeNullOrEmpty();
	}

	[Fact]
	public void ValidateTextLength_WithNullText_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var result = Implementation.ValidateTextLength(null!);
		
		// ✅ TDD: Assert implementation handles invalid input correctly
		AssertResultFailure(result);
		result.Error.ShouldNotBeNullOrEmpty();
	}

	[Fact]
	public async Task GenerateEmbeddingWithMetadataAsync_WithNullText_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var metadata = new Dictionary<string, object> { ["source"] = "test" };
		
		var result = await Implementation.GenerateEmbeddingWithMetadataAsync(null!, metadata);
		
		// ✅ TDD: Assert implementation handles invalid input correctly
		AssertResultFailure(result);
		result.Error.ShouldNotBeNullOrEmpty();
	}

	[Fact]
	public async Task GenerateEmbeddingWithMetadataAsync_WithEmptyText_ShouldReturnFailure()
	{
		// ✅ TDD: Test implementation behavior
		var metadata = new Dictionary<string, object> { ["source"] = "test" };
		
		var result = await Implementation.GenerateEmbeddingWithMetadataAsync(string.Empty, metadata);
		
		// ✅ TDD: Assert implementation handles invalid input correctly
		AssertResultFailure(result);
		result.Error.ShouldNotBeNullOrEmpty();
	}
}

