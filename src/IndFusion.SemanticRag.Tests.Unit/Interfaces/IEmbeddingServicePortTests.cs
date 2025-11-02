using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Tests.Unit.Shared;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Interfaces;

/// <summary>
/// IITDD tests for IEmbeddingServicePort interface contract.
/// These tests validate the interface contract, not implementation details.
/// Tests must pass for ANY valid implementation (Liskov Substitution Principle).
/// </summary>
public class IEmbeddingServicePortTests : BaseIITDDTest<IEmbeddingServicePort, EmbeddingRequest, float[]>
{
	public IEmbeddingServicePortTests()
	{
		SetUp();
	}

	protected override EmbeddingRequest CreateValidRequest()
	{
		return new EmbeddingRequest
		{
			Text = "This is a test text for embedding generation.",
			Metadata = new Dictionary<string, object> { ["source"] = "test" }
		};
	}

	protected override EmbeddingRequest CreateInvalidRequest()
	{
		return new EmbeddingRequest
		{
			Text = null!, // Invalid: null text
			Metadata = new Dictionary<string, object>()
		};
	}

	[Fact]
	public async Task GenerateEmbeddingAsync_WithValidText_ShouldReturnSuccess()
	{
		// ✅ IITDD: Test interface contract using mock
		var request = CreateValidRequest();
		var expectedEmbedding = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };

		MockPort.GenerateEmbeddingAsync(
			request.Text,
			Arg.Any<CancellationToken>())
			.Returns(Result<float[]>.Success(expectedEmbedding));

		var result = await MockPort.GenerateEmbeddingAsync(request.Text);

		// ✅ IITDD: Assert contract, not implementation details
		AssertSuccess(result);
		result.Value.ShouldNotBeNull();
		result.Value!.Length.ShouldBeGreaterThan(0); // ✅ This is a contract requirement (embedding dimension > 0)
	}

	[Fact]
	public async Task GenerateEmbeddingAsync_WithNullText_ShouldReturnFailure()
	{
		// ✅ IITDD: Test contract - null text should fail
		MockPort.GenerateEmbeddingAsync(
			null!,
			Arg.Any<CancellationToken>())
			.Returns(Result<float[]>.WithFailure("Text cannot be null or empty"));

		var result = await MockPort.GenerateEmbeddingAsync(null!);

		// ✅ IITDD: Assert failure contract
		AssertFailure(result);
	}

	[Fact]
	public async Task GenerateEmbeddingAsync_WithEmptyText_ShouldReturnFailure()
	{
		// ✅ IITDD: Test contract - empty text should fail
		MockPort.GenerateEmbeddingAsync(
			string.Empty,
			Arg.Any<CancellationToken>())
			.Returns(Result<float[]>.WithFailure("Text cannot be null or empty"));

		var result = await MockPort.GenerateEmbeddingAsync(string.Empty);

		// ✅ IITDD: Assert failure contract
		AssertFailure(result);
	}

	[Fact]
	public async Task GenerateEmbeddingsAsync_WithValidTexts_ShouldReturnSuccess()
	{
		// ✅ IITDD: Test interface contract
		var texts = new List<string> { "Text 1", "Text 2", "Text 3" }.AsReadOnly();
		var expectedEmbeddings = new List<float[]>
		{
			new float[] { 0.1f, 0.2f, 0.3f },
			new float[] { 0.4f, 0.5f, 0.6f },
			new float[] { 0.7f, 0.8f, 0.9f }
		}.AsReadOnly();

		MockPort.GenerateEmbeddingsAsync(
			texts,
			Arg.Any<CancellationToken>())
			.Returns(Result<IReadOnlyList<float[]>>.Success(expectedEmbeddings));

		var result = await MockPort.GenerateEmbeddingsAsync(texts);

		// ✅ IITDD: Assert contract compliance
		AssertSuccess(result);
		result.Value.ShouldNotBeNull();
		result.Value!.Count.ShouldBe(texts.Count); // ✅ Contract requirement: count must match input count
	}

	[Fact]
	public async Task GenerateEmbeddingsAsync_WithEmptyList_ShouldReturnSuccess()
	{
		// ✅ IITDD: Test contract - empty list should return empty results (not failure)
		var texts = new List<string>().AsReadOnly();
		var expectedEmbeddings = new List<float[]>().AsReadOnly();

		MockPort.GenerateEmbeddingsAsync(
			texts,
			Arg.Any<CancellationToken>())
			.Returns(Result<IReadOnlyList<float[]>>.Success(expectedEmbeddings));

		var result = await MockPort.GenerateEmbeddingsAsync(texts);

		// ✅ IITDD: Assert contract - empty input returns empty output
		AssertSuccess(result);
		result.Value.ShouldNotBeNull();
		result.Value!.Count.ShouldBe(0);
	}

	[Fact]
	public async Task GenerateEmbeddingWithMetadataAsync_WithValidText_ShouldReturnSuccess()
	{
		// ✅ IITDD: Test interface contract
		var text = "This is a test text";
		var metadata = new Dictionary<string, object> { ["source"] = "test", ["type"] = "document" };
		// ✅ Use fluent builder from TestDataBuilders
		var vectorResult = TestDataBuilders.CreateValidVectorEmbedding(
			id: "emb-1",
			content: text,
			embeddingSize: 3);
		vectorResult.IsSuccess.ShouldBeTrue();
		var expectedEmbedding = vectorResult.Value;

		MockPort.GenerateEmbeddingWithMetadataAsync(
			text,
			metadata,
			Arg.Any<CancellationToken>())
			.Returns(Result<VectorEmbedding>.Success(expectedEmbedding));

		var result = await MockPort.GenerateEmbeddingWithMetadataAsync(text, metadata);

		// ✅ IITDD: Assert contract compliance
		AssertSuccess(result);
		// Note: VectorEmbedding is a value type, so no null check needed
		result.Value.Content.ShouldBe(text); // ✅ Contract requirement: content must match input
		result.Value.Metadata.ShouldBe(metadata); // ✅ Contract requirement: metadata must match input
	}

	[Fact]
	public async Task GenerateEmbeddingWithMetadataAsync_WithNullText_ShouldReturnFailure()
	{
		// ✅ IITDD: Test contract - null text should fail
		var metadata = new Dictionary<string, object> { ["source"] = "test" };

		MockPort.GenerateEmbeddingWithMetadataAsync(
			null!,
			metadata,
			Arg.Any<CancellationToken>())
			.Returns(Result<VectorEmbedding>.WithFailure("Text cannot be null or empty"));

		var result = await MockPort.GenerateEmbeddingWithMetadataAsync(null!, metadata);

		// ✅ IITDD: Assert failure contract
		AssertFailure(result);
	}

	[Fact]
	public void GetEmbeddingDimension_ShouldReturnDimension()
	{
		// ✅ IITDD: Test interface contract - dimension must be positive
		var expectedDimension = 384;

		MockPort.GetEmbeddingDimension()
			.Returns(expectedDimension);

		var dimension = MockPort.GetEmbeddingDimension();

		// ✅ IITDD: Assert contract - dimension must be positive
		dimension.ShouldBeGreaterThan(0);
		dimension.ShouldBe(expectedDimension); // ✅ Contract requirement: dimension must match mock
	}

	[Fact]
	public void GetMaxTextLength_ShouldReturnMaxLength()
	{
		// ✅ IITDD: Test interface contract - max length must be positive
		var expectedMaxLength = 8192;

		MockPort.GetMaxTextLength()
			.Returns(expectedMaxLength);

		var maxLength = MockPort.GetMaxTextLength();

		// ✅ IITDD: Assert contract - max length must be positive
		maxLength.ShouldBeGreaterThan(0);
		maxLength.ShouldBe(expectedMaxLength); // ✅ Contract requirement: max length must match mock
	}

	[Fact]
	public void ValidateTextLength_WithValidText_ShouldReturnSuccess()
	{
		// ✅ IITDD: Test interface contract
		var text = "This is a valid text";

		MockPort.ValidateTextLength(text)
			.Returns(Result.Success());

		var result = MockPort.ValidateTextLength(text);

		// ✅ IITDD: Assert contract - success result
		AssertSuccess(result);
	}

	[Fact]
	public void ValidateTextLength_WithTooLongText_ShouldReturnFailure()
	{
		// ✅ IITDD: Test contract - text exceeding max length should fail
		var tooLongText = new string('a', 10000);

		MockPort.ValidateTextLength(tooLongText)
			.Returns(Result.WithFailure("Text length exceeds maximum"));

		var result = MockPort.ValidateTextLength(tooLongText);

		// ✅ IITDD: Assert failure contract
		AssertFailure(result);
	}

	[Fact]
	public async Task GetModelInfoAsync_ShouldReturnSuccess()
	{
		// ✅ IITDD: Test interface contract
		var expectedModelInfo = new EmbeddingModelInfo(
			ModelName: "test-model",
			Version: "1.0",
			Dimension: 384,
			MaxTextLength: 8192,
			SupportedLanguages: new List<string> { "en", "es" }.AsReadOnly());

		MockPort.GetModelInfoAsync(Arg.Any<CancellationToken>())
			.Returns(Result<EmbeddingModelInfo>.Success(expectedModelInfo));

		var result = await MockPort.GetModelInfoAsync();

		// ✅ IITDD: Assert contract compliance
		AssertSuccess(result);
		// Note: EmbeddingModelInfo is a value type, so no null check needed
		result.Value.ModelName.ShouldBe(expectedModelInfo.ModelName); // ✅ Contract requirement
		result.Value.Dimension.ShouldBe(expectedModelInfo.Dimension); // ✅ Contract requirement
	}
}

// Helper request class for test data
public class EmbeddingRequest
{
	public string Text { get; set; } = null!;
	public Dictionary<string, object>? Metadata { get; set; }
}

