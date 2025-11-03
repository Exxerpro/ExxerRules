using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Interfaces;

/// <summary>
/// ITDD tests for IPatternSuggestService interface contract validation.
/// </summary>
public class IPatternSuggestServiceTests
{
    private readonly IPatternSuggestService _mockPatternSuggestService;

    /// <summary>
    /// Initializes a new instance of the IPatternSuggestServiceTests class.
    /// </summary>
    public IPatternSuggestServiceTests()
    {
        _mockPatternSuggestService = Substitute.For<IPatternSuggestService>();
    }

    /// <summary>
    /// Verifies that SuggestPatternsAsync returns success for valid code context.
    /// </summary>
    [Fact]
    public async Task SuggestPatternsAsync_Should_Return_Success_For_Valid_CodeContext()
    {
        // Arrange
        var codeContext = "public class TestClass { public void TestMethod() { } }";
        var options = new PatternSuggestionOptions(
            MaxSuggestions: 5,
            MinConfidence: 0.7f,
            Categories: new List<string> { "Design Patterns" },
            IncludeCodeExamples: true,
            IncludeEffortEstimate: true);

        var expectedSuggestions = new List<PatternSuggestion>
        {
            new(
                Id: "suggestion1",
                ViolationId: "violation1",
                Title: "Consider using Repository Pattern",
                Description: "This class could benefit from the Repository pattern for data access",
                CodeExample: "public interface IRepository<T> { ... }",
                Confidence: 0.8f,
                Effort: SuggestionEffort.Medium,
                Impact: SuggestionImpact.High)
        };

        _mockPatternSuggestService.SuggestPatternsAsync(codeContext, options, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternSuggestion>>.Success(expectedSuggestions));

        // Act
        var result = await _mockPatternSuggestService.SuggestPatternsAsync(codeContext, options, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Id.ShouldBe("suggestion1");
        result.Value[0].Title.ShouldBe("Consider using Repository Pattern");
        result.Value[0].Confidence.ShouldBe(0.8f);
    }

    /// <summary>
    /// Verifies that SuggestPatternsAsync returns empty list for no suggestions.
    /// </summary>
    [Fact]
    public async Task SuggestPatternsAsync_Should_Return_Empty_List_For_No_Suggestions()
    {
        // Arrange
        var codeContext = "// Empty comment";
        var options = new PatternSuggestionOptions();

        _mockPatternSuggestService.SuggestPatternsAsync(codeContext, options, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternSuggestion>>.Success(new List<PatternSuggestion>()));

        // Act
        var result = await _mockPatternSuggestService.SuggestPatternsAsync(codeContext, options, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that SuggestPatternsAsync returns failure for invalid code context.
    /// </summary>
    [Fact]
    public async Task SuggestPatternsAsync_Should_Return_Failure_For_Invalid_CodeContext()
    {
        // Arrange
        var codeContext = "";
        var options = new PatternSuggestionOptions();
        var expectedError = "Code context cannot be empty";

        _mockPatternSuggestService.SuggestPatternsAsync(codeContext, options, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternSuggestion>>.WithFailure(expectedError));

        // Act
        var result = await _mockPatternSuggestService.SuggestPatternsAsync(codeContext, options, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(expectedError);
    }

    /// <summary>
    /// Verifies that AnalyzePatternAsync returns success for valid pattern analysis.
    /// </summary>
    [Fact]
    public async Task AnalyzePatternAsync_Should_Return_Success_For_Valid_Pattern_Analysis()
    {
        // Arrange
        var code = "public class Singleton { private static Singleton instance; }";
        var patternType = "Singleton";
        var expectedAnalysis = new PatternAnalysis(
            PatternType: "Singleton",
            Matches: new List<Domain.Models.PatternMatch>
            {
                new(
                    Pattern: new SemanticPattern("singleton1", "Singleton", "Singleton pattern", "class.*Singleton", "Design Patterns"),
                    Match: "public class Singleton",
                    StartIndex: 0,
                    EndIndex: 20,
                    Confidence: 0.9f)
            },
            Violations: new List<PatternViolation>(),
            Suggestions: new List<PatternSuggestion>
            {
                new(
                    Id: "suggestion1",
                    ViolationId: "violation1",
                    Title: "Implement thread-safe Singleton",
                    Description: "Consider using thread-safe implementation",
                    Confidence: 0.8f)
            },
            Confidence: 0.85f,
            AnalysisTimeMs: 150);

        _mockPatternSuggestService.AnalyzePatternAsync(code, patternType, Arg.Any<CancellationToken>())
            .Returns(Result<PatternAnalysis>.Success(expectedAnalysis));

        // Act
        var result = await _mockPatternSuggestService.AnalyzePatternAsync(code, patternType, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<PatternAnalysis>(default);
        result.Value.PatternType.ShouldBe("Singleton");
        result.Value.MatchCount.ShouldBe(1);
        result.Value.ViolationCount.ShouldBe(0);
        result.Value.SuggestionCount.ShouldBe(1);
        result.Value.Confidence.ShouldBe(0.85f);
        result.Value.HasViolations.ShouldBeFalse();
    }

    /// <summary>
    /// Verifies that AnalyzePatternAsync returns failure for invalid pattern type.
    /// </summary>
    [Fact]
    public async Task AnalyzePatternAsync_Should_Return_Failure_For_Invalid_Pattern_Type()
    {
        // Arrange
        var code = "public class Test { }";
        var patternType = "NonExistentPattern";
        var expectedError = "Unknown pattern type: NonExistentPattern";

        _mockPatternSuggestService.AnalyzePatternAsync(code, patternType, Arg.Any<CancellationToken>())
            .Returns(Result<PatternAnalysis>.WithFailure(expectedError));

        // Act
        var result = await _mockPatternSuggestService.AnalyzePatternAsync(code, patternType, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(expectedError);
    }

    /// <summary>
    /// Verifies that FindViolationsAsync returns success with violations.
    /// </summary>
    [Fact]
    public async Task FindViolationsAsync_Should_Return_Success_With_Violations()
    {
        // Arrange
        var code = "public class Test { public void Method() { var x = null; } }";
        var filePath = "Test.cs";
        var expectedViolations = new List<PatternViolation>
        {
            new(
                Id: "violation1",
                PatternId: "null-assignment",
                PatternName: "Null Assignment",
                Severity: PatternSeverity.Warning,
                Message: "Avoid assigning null to variables",
                FilePath: filePath,
                LineNumber: 1,
                CodeSnippet: "var x = null;")
        };

        _mockPatternSuggestService.FindViolationsAsync(code, filePath, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternViolation>>.Success(expectedViolations));

        // Act
        var result = await _mockPatternSuggestService.FindViolationsAsync(code, filePath, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Id.ShouldBe("violation1");
        result.Value[0].PatternName.ShouldBe("Null Assignment");
        result.Value[0].Severity.ShouldBe(PatternSeverity.Warning);
        result.Value[0].HasLocation.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that FindViolationsAsync returns empty list for clean code.
    /// </summary>
    [Fact]
    public async Task FindViolationsAsync_Should_Return_Empty_List_For_Clean_Code()
    {
        // Arrange
        var code = "public class Test { public void Method() { var x = \"test\"; } }";
        var filePath = "Test.cs";

        _mockPatternSuggestService.FindViolationsAsync(code, filePath, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternViolation>>.Success(new List<PatternViolation>()));

        // Act
        var result = await _mockPatternSuggestService.FindViolationsAsync(code, filePath, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that FindViolationsAsync handles null file path.
    /// </summary>
    [Fact]
    public async Task FindViolationsAsync_Should_Handle_Null_FilePath()
    {
        // Arrange
        var code = "public class Test { }";
        var expectedViolations = new List<PatternViolation>
        {
            new(
                Id: "violation1",
                PatternId: "empty-class",
                PatternName: "Empty Class",
                Severity: PatternSeverity.Info,
                Message: "Consider adding functionality to this class")
        };

        _mockPatternSuggestService.FindViolationsAsync(code, null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternViolation>>.Success(expectedViolations));

        // Act
        var result = await _mockPatternSuggestService.FindViolationsAsync(code, null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].HasLocation.ShouldBeFalse();
    }

    /// <summary>
    /// Verifies that GetPatternDefinitionsAsync returns success for valid category.
    /// </summary>
    [Fact]
    public async Task GetPatternDefinitionsAsync_Should_Return_Success_For_Valid_Category()
    {
        // Arrange
        var category = "Design Patterns";
        var expectedDefinitions = new List<PatternDefinition>
        {
            new(
                Id: "singleton",
                Name: "Singleton Pattern",
                Description: "Ensures a class has only one instance",
                Category: category,
                Severity: PatternSeverity.Info,
                Pattern: "class.*Singleton",
                Tags: new List<string> { "creational", "singleton" })
        };

        _mockPatternSuggestService.GetPatternDefinitionsAsync(category, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternDefinition>>.Success(expectedDefinitions));

        // Act
        var result = await _mockPatternSuggestService.GetPatternDefinitionsAsync(category, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Id.ShouldBe("singleton");
        result.Value[0].Name.ShouldBe("Singleton Pattern");
        result.Value[0].Category.ShouldBe(category);
        result.Value[0].HasTag("creational").ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that GetPatternDefinitionsAsync returns empty list for unknown category.
    /// </summary>
    [Fact]
    public async Task GetPatternDefinitionsAsync_Should_Return_Empty_List_For_Unknown_Category()
    {
        // Arrange
        var category = "Unknown Category";

        _mockPatternSuggestService.GetPatternDefinitionsAsync(category, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternDefinition>>.Success(new List<PatternDefinition>()));

        // Act
        var result = await _mockPatternSuggestService.GetPatternDefinitionsAsync(category, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that GetPatternCategoriesAsync returns success with categories.
    /// </summary>
    [Fact]
    public async Task GetPatternCategoriesAsync_Should_Return_Success_With_Categories()
    {
        // Arrange
        var expectedCategories = new List<string>
        {
            "Design Patterns",
            "Anti-Patterns",
            "Performance",
            "Security",
            "Maintainability"
        };

        _mockPatternSuggestService.GetPatternCategoriesAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<string>>.Success(expectedCategories));

        // Act
        var result = await _mockPatternSuggestService.GetPatternCategoriesAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(5);
        result.Value.ShouldContain("Design Patterns");
        result.Value.ShouldContain("Anti-Patterns");
        result.Value.ShouldContain("Performance");
    }

    /// <summary>
    /// Verifies that ValidatePatternDefinitionAsync returns success for valid definition.
    /// </summary>
    [Fact]
    public async Task ValidatePatternDefinitionAsync_Should_Return_Success_For_Valid_Definition()
    {
        // Arrange
        var patternDefinition = new PatternDefinition(
            Id: "test-pattern",
            Name: "Test Pattern",
            Description: "A test pattern for validation",
            Category: "Test",
            Severity: PatternSeverity.Info,
            Pattern: "test.*pattern",
            Tags: new List<string> { "test" });

        _mockPatternSuggestService.ValidatePatternDefinitionAsync(patternDefinition, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _mockPatternSuggestService.ValidatePatternDefinitionAsync(patternDefinition, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that ValidatePatternDefinitionAsync returns failure for invalid definition.
    /// </summary>
    [Fact]
    public async Task ValidatePatternDefinitionAsync_Should_Return_Failure_For_Invalid_Definition()
    {
        // Arrange
        var patternDefinition = new PatternDefinition(
            Id: "",
            Name: "Test Pattern",
            Description: "A test pattern for validation",
            Category: "Test",
            Severity: PatternSeverity.Info,
            Pattern: "test.*pattern",
            Tags: new List<string> { "test" });

        var expectedError = "Pattern ID cannot be empty";

        _mockPatternSuggestService.ValidatePatternDefinitionAsync(patternDefinition, Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure(expectedError));

        // Act
        var result = await _mockPatternSuggestService.ValidatePatternDefinitionAsync(patternDefinition, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(expectedError);
    }

    /// <summary>
    /// Verifies that all methods handle cancellation token.
    /// </summary>
    [Fact]
    public async Task All_Methods_Should_Handle_Cancellation_Token()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockPatternSuggestService.SuggestPatternsAsync(Arg.Any<string>(), Arg.Any<PatternSuggestionOptions>(), cts.Token)
            .Returns(Result<IReadOnlyList<PatternSuggestion>>.WithFailure("Cancelled"));
        _mockPatternSuggestService.AnalyzePatternAsync(Arg.Any<string>(), Arg.Any<string>(), cts.Token)
            .Returns(Result<PatternAnalysis>.WithFailure("Cancelled"));
        _mockPatternSuggestService.FindViolationsAsync(Arg.Any<string>(), Arg.Any<string>(), cts.Token)
            .Returns(Result<IReadOnlyList<PatternViolation>>.WithFailure("Cancelled"));
        _mockPatternSuggestService.GetPatternDefinitionsAsync(Arg.Any<string>(), cts.Token)
            .Returns(Result<IReadOnlyList<PatternDefinition>>.WithFailure("Cancelled"));
        _mockPatternSuggestService.GetPatternCategoriesAsync(cts.Token)
            .Returns(Result<IReadOnlyList<string>>.WithFailure("Cancelled"));
        _mockPatternSuggestService.ValidatePatternDefinitionAsync(Arg.Any<PatternDefinition>(), cts.Token)
            .Returns(Result.WithFailure("Cancelled"));

        // Act & Assert
        var suggestResult = await _mockPatternSuggestService.SuggestPatternsAsync("test", new PatternSuggestionOptions(), cts.Token);
        suggestResult.IsFailure.ShouldBeTrue();

        var analyzeResult = await _mockPatternSuggestService.AnalyzePatternAsync("test", "test", cts.Token);
        analyzeResult.IsFailure.ShouldBeTrue();

        var violationsResult = await _mockPatternSuggestService.FindViolationsAsync("test", "test.cs", cts.Token);
        violationsResult.IsFailure.ShouldBeTrue();

        var definitionsResult = await _mockPatternSuggestService.GetPatternDefinitionsAsync("test", cts.Token);
        definitionsResult.IsFailure.ShouldBeTrue();

        var categoriesResult = await _mockPatternSuggestService.GetPatternCategoriesAsync(cts.Token);
        categoriesResult.IsFailure.ShouldBeTrue();

        var validateResult = await _mockPatternSuggestService.ValidatePatternDefinitionAsync(
            new PatternDefinition("test", "test", "test", "test", PatternSeverity.Info, "test", new List<string>()), cts.Token);
        validateResult.IsFailure.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that SuggestPatternsAsync handles null categories.
    /// </summary>
    [Fact]
    public async Task SuggestPatternsAsync_Should_Handle_Null_Categories()
    {
        // Arrange
        var codeContext = "public class Test { }";
        var options = new PatternSuggestionOptions(Categories: null);
        var expectedSuggestions = new List<PatternSuggestion>();

        _mockPatternSuggestService.SuggestPatternsAsync(codeContext, options, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternSuggestion>>.Success(expectedSuggestions));

        // Act
        var result = await _mockPatternSuggestService.SuggestPatternsAsync(codeContext, options, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }
}
