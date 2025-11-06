using IndFusion.SemanticRag.Application.Services;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Tests.Application.Services;

/// <summary>
/// Unit tests for comprehensive search options validation.
/// </summary>
public class ComprehensiveSearchOptionsTests
{
    [Fact(Timeout = 5000)]
    public void Should_CreateDefaultOptions_When_DefaultCalled()
    {
        // Act
        var options = ComprehensiveSearchOptions.Default();

        // Assert
        // Note: SemanticSearchOptions is a struct, so new SemanticSearchOptions() equals default(SemanticSearchOptions)
        // Instead, we verify that the options are properly configured with expected values
        options.RagConfig.ShouldNotBe(default(SemanticRagConfig));
        options.RagConfig.Id.ShouldBe("default");
        options.RagConfig.Name.ShouldBe("Default Configuration");
        options.EnableKnowledgeExtraction.ShouldBeTrue();
        options.MaxResultsForExtraction.ShouldBe(5);
        options.EnableContextRetrieval.ShouldBeTrue();
    }
}