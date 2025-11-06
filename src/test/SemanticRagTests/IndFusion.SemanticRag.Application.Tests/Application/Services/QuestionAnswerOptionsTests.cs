using IndFusion.SemanticRag.Application.Services;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;

namespace IndFusion.SemanticRag.Application.Tests.Application.Services;

/// <summary>
/// Unit tests for question answer options validation.
/// </summary>
public class QuestionAnswerOptionsTests
{
    [Fact(Timeout = 5000)]
    public void Should_CreateQuestionAnswerOptions_When_ValidParametersProvided()
    {
        // Arrange
        var searchOptions = new SemanticSearchOptions();
        var ragConfig = new SemanticRagConfig(
            Id: "test-config",
            Name: "Test Config",
            EmbeddingModel: "test-model",
            VectorDimensions: 1536,
            SimilarityThreshold: 0.7,
            MaxResults: 10,
            Properties: []);
        var maxContextDocuments = 10;
        var includeEntityContext = true;
        var includeRelationshipContext = true;

        // Act
        var options = new QuestionAnswerOptions(
            searchOptions,
            ragConfig,
            maxContextDocuments,
            includeEntityContext,
            includeRelationshipContext);

        // Assert
        options.SearchOptions.ShouldBe(searchOptions);
        options.RagConfig.ShouldBe(ragConfig);
        options.MaxContextDocuments.ShouldBe(maxContextDocuments);
        options.IncludeEntityContext.ShouldBe(includeEntityContext);
        options.IncludeRelationshipContext.ShouldBe(includeRelationshipContext);
    }
}