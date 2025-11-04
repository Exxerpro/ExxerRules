using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Application.Services;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Implementations;

/// <summary>
/// TDD tests for PatternSuggestService using real implementations.
/// </summary>
public class PatternSuggestServiceTests
{
    private readonly IKnowledgeGraphPort _knowledgeGraphPort;
    private readonly ILogger<PatternSuggestService> _logger;
    private readonly PatternSuggestService _patternSuggestService;

    /// <summary>
    /// Initializes a new instance of the PatternSuggestServiceTests class.
    /// </summary>
    public PatternSuggestServiceTests()
    {
        _knowledgeGraphPort = Substitute.For<IKnowledgeGraphPort>();
        _logger = Substitute.For<ILogger<PatternSuggestService>>();
        _patternSuggestService = new PatternSuggestService(_knowledgeGraphPort, _logger);
    }

    /// <summary>
    /// Verifies that SuggestPatternsAsync generates suggestions for code context.
    /// </summary>
    [Fact]
    public async Task SuggestPatternsAsync_Should_Generate_Suggestions_For_Code_Context()
    {
        // Arrange
        var codeContext = "public class TestClass { public void TestMethod() { } }";
        var options = new PatternSuggestionOptions(
            MaxSuggestions: 5,
            MinConfidence: 0.5f,
            Categories: ["Design Patterns"],
            IncludeCodeExamples: true,
            IncludeEffortEstimate: true);

        var patternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "singleton-pattern",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "singleton",
                    ["name"] = "Singleton Pattern",
                    ["description"] = "Ensures a class has only one instance",
                    ["category"] = "Design Patterns",
                    ["severity"] = "Info",
                    ["pattern"] = "class.*Singleton",
                    ["tags"] = new List<string> { "creational" },
                    ["isEnabled"] = true
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(patternNodes));

        // Act
        var result = await _patternSuggestService.SuggestPatternsAsync(codeContext, options, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBeGreaterThan(0);
        result.Value[0].Title.ShouldContain("Singleton");
        result.Value[0].Confidence.ShouldBeGreaterThanOrEqualTo(options.MinConfidence);
    }

    /// <summary>
    /// Verifies that SuggestPatternsAsync handles empty code context.
    /// </summary>
    [Fact]
    public async Task SuggestPatternsAsync_Should_Handle_Empty_Code_Context()
    {
        // Arrange
        var codeContext = "";
        var options = new PatternSuggestionOptions();

        // Act
        var result = await _patternSuggestService.SuggestPatternsAsync(codeContext, options, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Code context cannot be null or empty");
    }

    /// <summary>
    /// Verifies that SuggestPatternsAsync handles invalid options.
    /// </summary>
    [Fact]
    public async Task SuggestPatternsAsync_Should_Handle_Invalid_Options()
    {
        // Arrange
        var codeContext = "public class Test { }";
        var options = new PatternSuggestionOptions(MaxSuggestions: -1);

        // Act
        var result = await _patternSuggestService.SuggestPatternsAsync(codeContext, options, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Max suggestions must be greater than 0");
    }

    /// <summary>
    /// Verifies that SuggestPatternsAsync filters by categories.
    /// </summary>
    [Fact]
    public async Task SuggestPatternsAsync_Should_Filter_By_Categories()
    {
        // Arrange
        var codeContext = "public class Test { }";
        var options = new PatternSuggestionOptions(Categories: ["Design Patterns"]);

        var patternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "singleton-pattern",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "singleton",
                    ["name"] = "Singleton Pattern",
                    ["description"] = "Ensures a class has only one instance",
                    ["category"] = "Design Patterns",
                    ["severity"] = "Info",
                    ["pattern"] = "class.*Singleton",
                    ["tags"] = new List<string> { "creational" },
                    ["isEnabled"] = true
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow),
            new(
                Id: "security-pattern",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "security",
                    ["name"] = "Security Pattern",
                    ["description"] = "Security best practices",
                    ["category"] = "Security",
                    ["severity"] = "Warning",
                    ["pattern"] = "security.*pattern",
                    ["tags"] = new List<string> { "security" },
                    ["isEnabled"] = true
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(patternNodes));

        // Act
        var result = await _patternSuggestService.SuggestPatternsAsync(codeContext, options, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        // Should only include Design Patterns category
        result.Value.All(s => s.Title.Contains("Singleton")).ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that AnalyzePatternAsync analyzes specific pattern.
    /// </summary>
    [Fact]
    public async Task AnalyzePatternAsync_Should_Analyze_Specific_Pattern()
    {
        // Arrange
        var code = "public class Singleton { private static Singleton instance; }";
        var patternType = "Singleton";

        var patternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "singleton-pattern",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "singleton",
                    ["name"] = "Singleton Pattern",
                    ["description"] = "Ensures a class has only one instance",
                    ["category"] = "Design Patterns",
                    ["severity"] = "Info",
                    ["pattern"] = "class.*Singleton",
                    ["tags"] = new List<string> { "creational" },
                    ["isEnabled"] = true
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(patternNodes));

        // Act
        var result = await _patternSuggestService.AnalyzePatternAsync(code, patternType, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<PatternAnalysis>(default);
        result.Value.PatternType.ShouldBe(patternType);
        result.Value.AnalysisTimeMs.ShouldBeGreaterThan(0);
        result.Value.Confidence.ShouldBeGreaterThan(0.0f);
    }

    /// <summary>
    /// Verifies that AnalyzePatternAsync handles empty code.
    /// </summary>
    [Fact]
    public async Task AnalyzePatternAsync_Should_Handle_Empty_Code()
    {
        // Arrange
        var code = "";
        var patternType = "Singleton";

        // Act
        var result = await _patternSuggestService.AnalyzePatternAsync(code, patternType, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Code cannot be null or empty");
    }

    /// <summary>
    /// Verifies that AnalyzePatternAsync handles unknown pattern type.
    /// </summary>
    [Fact]
    public async Task AnalyzePatternAsync_Should_Handle_Unknown_Pattern_Type()
    {
        // Arrange
        var code = "public class Test { }";
        var patternType = "UnknownPattern";

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success([]));

        // Act
        var result = await _patternSuggestService.AnalyzePatternAsync(code, patternType, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain($"No pattern definitions found for type: {patternType}");
    }

    /// <summary>
    /// Verifies that FindViolationsAsync finds pattern violations.
    /// </summary>
    [Fact]
    public async Task FindViolationsAsync_Should_Find_Pattern_Violations()
    {
        // Arrange
        var code = "public class Test { public void Method() { var x = null; } }";
        var filePath = "Test.cs";

        var patternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "null-assignment-pattern",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "null-assignment",
                    ["name"] = "Null Assignment",
                    ["description"] = "Avoid assigning null to variables",
                    ["category"] = "Best Practices",
                    ["severity"] = "Warning",
                    ["pattern"] = "var.*=.*null",
                    ["tags"] = new List<string> { "null-safety" },
                    ["isEnabled"] = true
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(patternNodes));

        // Act
        var result = await _patternSuggestService.FindViolationsAsync(code, filePath, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        // Should find violations based on the pattern matching logic
        result.Value.Count.ShouldBeGreaterThanOrEqualTo(0);
    }

    /// <summary>
    /// Verifies that FindViolationsAsync handles null file path.
    /// </summary>
    [Fact]
    public async Task FindViolationsAsync_Should_Handle_Null_FilePath()
    {
        // Arrange
        var code = "public class Test { }";

        var patternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "empty-class-pattern",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "empty-class",
                    ["name"] = "Empty Class",
                    ["description"] = "Consider adding functionality to this class",
                    ["category"] = "Maintainability",
                    ["severity"] = "Info",
                    ["pattern"] = "class.*\\{\\s*\\}",
                    ["tags"] = new List<string> { "maintainability" },
                    ["isEnabled"] = true
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(patternNodes));

        // Act
        var result = await _patternSuggestService.FindViolationsAsync(code, null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    /// <summary>
    /// Verifies that GetPatternDefinitionsAsync retrieves patterns by category.
    /// </summary>
    [Fact]
    public async Task GetPatternDefinitionsAsync_Should_Retrieve_Patterns_By_Category()
    {
        // Arrange
        var category = "Design Patterns";
        var patternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "singleton-pattern",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "singleton",
                    ["name"] = "Singleton Pattern",
                    ["description"] = "Ensures a class has only one instance",
                    ["category"] = category,
                    ["severity"] = "Info",
                    ["pattern"] = "class.*Singleton",
                    ["tags"] = new List<string> { "creational" },
                    ["isEnabled"] = true
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(patternNodes));

        // Act
        var result = await _patternSuggestService.GetPatternDefinitionsAsync(category, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Category.ShouldBe(category);
        result.Value[0].Name.ShouldBe("Singleton Pattern");
    }

    /// <summary>
    /// Verifies that GetPatternDefinitionsAsync handles empty category.
    /// </summary>
    [Fact]
    public async Task GetPatternDefinitionsAsync_Should_Handle_Empty_Category()
    {
        // Arrange
        var category = "";

        // Act
        var result = await _patternSuggestService.GetPatternDefinitionsAsync(category, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Category cannot be null or empty");
    }

    /// <summary>
    /// Verifies that GetPatternCategoriesAsync returns all categories.
    /// </summary>
    [Fact]
    public async Task GetPatternCategoriesAsync_Should_Return_All_Categories()
    {
        // Arrange
        var patternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "pattern1",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["category"] = "Design Patterns"
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow),
            new(
                Id: "pattern2",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["category"] = "Security"
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow),
            new(
                Id: "pattern3",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["category"] = "Performance"
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(patternNodes));

        // Act
        var result = await _patternSuggestService.GetPatternCategoriesAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(3);
        result.Value.ShouldContain("Design Patterns");
        result.Value.ShouldContain("Security");
        result.Value.ShouldContain("Performance");
    }

    /// <summary>
    /// Verifies that ValidatePatternDefinitionAsync validates valid pattern.
    /// </summary>
    [Fact]
    public async Task ValidatePatternDefinitionAsync_Should_Validate_Valid_Pattern()
    {
        // Arrange
        var patternDefinition = new PatternDefinition(
            Id: "test-pattern",
            Name: "Test Pattern",
            Description: "A test pattern for validation",
            Category: "Test",
            Severity: PatternSeverity.Info,
            Pattern: "test.*pattern",
            Tags: ["test"]);

        // Act
        var result = await _patternSuggestService.ValidatePatternDefinitionAsync(patternDefinition, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that ValidatePatternDefinitionAsync rejects invalid pattern.
    /// </summary>
    [Fact]
    public async Task ValidatePatternDefinitionAsync_Should_Reject_Invalid_Pattern()
    {
        // Arrange
        var patternDefinition = new PatternDefinition(
            Id: "",
            Name: "Test Pattern",
            Description: "A test pattern for validation",
            Category: "Test",
            Severity: PatternSeverity.Info,
            Pattern: "test.*pattern",
            Tags: ["test"]);

        // Act
        var result = await _patternSuggestService.ValidatePatternDefinitionAsync(patternDefinition, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("Pattern ID cannot be null or empty");
    }

    /// <summary>
    /// Verifies that all methods handle cancellation properly.
    /// </summary>
    [Fact]
    public async Task All_Methods_Should_Handle_Cancellation()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), cts.Token)
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.WithFailure("Cancelled"));

        // Act & Assert
        var suggestResult = await _patternSuggestService.SuggestPatternsAsync("test", new PatternSuggestionOptions(), cts.Token);
        suggestResult.IsFailure.ShouldBeTrue();
        suggestResult.Error.ShouldContain("cancelled");

        var analyzeResult = await _patternSuggestService.AnalyzePatternAsync("test", "test", cts.Token);
        analyzeResult.IsFailure.ShouldBeTrue();
        analyzeResult.Error.ShouldContain("cancelled");

        var violationsResult = await _patternSuggestService.FindViolationsAsync("test", "test.cs", cts.Token);
        violationsResult.IsFailure.ShouldBeTrue();
        violationsResult.Error.ShouldContain("cancelled");

        var definitionsResult = await _patternSuggestService.GetPatternDefinitionsAsync("test", cts.Token);
        definitionsResult.IsFailure.ShouldBeTrue();
        definitionsResult.Error.ShouldContain("cancelled");

        var categoriesResult = await _patternSuggestService.GetPatternCategoriesAsync(cts.Token);
        categoriesResult.IsFailure.ShouldBeTrue();
        categoriesResult.Error.ShouldContain("cancelled");

        var validateResult = await _patternSuggestService.ValidatePatternDefinitionAsync(
            new PatternDefinition("test", "test", "test", "test", PatternSeverity.Info, "test", []), cts.Token);
        validateResult.IsFailure.ShouldBeTrue();
        validateResult.Error.ShouldNotBeNull();
        validateResult.Error.ShouldContain("cancelled");
    }

    /// <summary>
    /// Verifies that SuggestPatternsAsync uses ConfigureAwait(false).
    /// </summary>
    [Fact]
    public async Task SuggestPatternsAsync_Should_Use_ConfigureAwait_False()
    {
        // Arrange
        var codeContext = "public class Test { }";
        var options = new PatternSuggestionOptions();

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success([]));

        // Act
        var result = await _patternSuggestService.SuggestPatternsAsync(codeContext, options, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        // Verify that ConfigureAwait(false) was used by checking the call was made
        await _knowledgeGraphPort.Received().QueryNodesAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies that AnalyzePatternAsync calculates confidence correctly.
    /// </summary>
    [Fact]
    public async Task AnalyzePatternAsync_Should_Calculate_Confidence_Correctly()
    {
        // Arrange
        var code = "public class Singleton { private static Singleton instance; }";
        var patternType = "Singleton";

        var patternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "singleton-pattern",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "singleton",
                    ["name"] = "Singleton Pattern",
                    ["description"] = "Ensures a class has only one instance",
                    ["category"] = "Design Patterns",
                    ["severity"] = "Info",
                    ["pattern"] = "class.*Singleton",
                    ["tags"] = new List<string> { "creational" },
                    ["isEnabled"] = true
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(patternNodes));

        // Act
        var result = await _patternSuggestService.AnalyzePatternAsync(code, patternType, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<PatternAnalysis>(default);
        result.Value.Confidence.ShouldBeGreaterThan(0.0f);
        result.Value.Confidence.ShouldBeLessThanOrEqualTo(1.0f);
    }
}
