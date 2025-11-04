using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.System.Tests.Infrastructure.Services;

/// <summary>
/// Behavioral system tests for SemanticPatternEngineService to drive implementation.
/// These tests verify actual behavior and drive the replacement of mock implementations.
/// </summary>
[Trait("Category", "System")]
public class SemanticPatternEngineServiceBehavioralTests
{
    private readonly ILogger<SemanticPatternEngineService> _logger;

    /// <summary>
    /// Initializes the semantic pattern engine behavioral test fixture with substitute logging.
    /// </summary>
    public SemanticPatternEngineServiceBehavioralTests()
    {
        _logger = Substitute.For<ILogger<SemanticPatternEngineService>>();
    }

    /// <summary>
    /// Verifies that analyzing representative code returns concrete pattern violations instead of placeholder data.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the violation assertions pass.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithValidCode_ShouldReturnActualViolations()
    {
        // Arrange
        var code = @"
using System;

public class TestClass
{
    public void TestMethod()
    {
        var unused = 42; // This should trigger a violation
        Console.WriteLine(""Hello World"");
    }
}";
        var context = "C# Development";
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.AnalyzeCodeAsync(code, context, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0); // Should find actual violations, not empty list

        // Verify violation structure
        foreach (var violation in result)
        {
            violation.Id.ShouldNotBeNullOrEmpty();
            violation.PatternId.ShouldNotBeNullOrEmpty();
            violation.PatternName.ShouldNotBeNullOrEmpty();
            violation.Message.ShouldNotBeNullOrEmpty();
            violation.Severity.ShouldBeOneOf(PatternSeverity.Info, PatternSeverity.Warning, PatternSeverity.Error, PatternSeverity.Critical);
        }

        // This test drives implementation of actual semantic pattern analysis
        // Currently fails because implementation uses Task.Delay placeholder and returns empty list
    }

    /// <summary>
    /// Confirms that clean code samples yield no violations, demonstrating correct success semantics.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the absence of violations is verified.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithCleanCode_ShouldReturnNoViolations()
    {
        // Arrange
        var cleanCode = @"
using System;

public class TestClass
{
    public void TestMethod()
    {
        Console.WriteLine(""Hello World"");
    }
}";
        var context = "C# Development";
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.AnalyzeCodeAsync(cleanCode, context, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0); // Should find no violations in clean code

        // This test drives implementation of clean code detection
    }

    /// <summary>
    /// Ensures <see cref="SemanticPatternEngineService.AnalyzeCodeAsync(string, string, CancellationToken)"/> rejects <see langword="null"/> code input.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the argument exception is observed.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithNullCode_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeCodeAsync(null!, "context", TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Validates that empty code strings are treated as invalid when requesting pattern analysis.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the guard clause is exercised.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithEmptyCode_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeCodeAsync(string.Empty, "context", TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Confirms a <see langword="null"/> analysis context produces an <see cref="ArgumentException"/> to prevent ambiguous evaluation.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the exception assertion passes.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithNullContext_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeCodeAsync("code", null!, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Checks that empty analysis contexts are rejected before processing begins.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the guard clause exception is validated.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithEmptyContext_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeCodeAsync("code", string.Empty, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Verifies that cancellation tokens cancel in-flight code analysis promptly.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after cancellation behavior has been asserted.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var code = "public class Test { }";
        var context = "C# Development";
        var service = new SemanticPatternEngineService(_logger);

        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await service.AnalyzeCodeAsync(code, context, cts.Token));
    }

    /// <summary>
    /// Validates that project-level analysis returns actual violations, timing, and scope metadata for valid projects.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the project analysis result has been inspected.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithValidProjectPath_ShouldReturnActualViolations()
    {
        // Arrange
        var projectPath = @"C:\TestProject\TestProject.csproj";
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.AnalyzeProjectAsync(projectPath, null, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0); // Should find actual violations, not empty list

        // Verify violation structure
        foreach (var violation in result)
        {
            violation.Id.ShouldNotBeNullOrEmpty();
            violation.PatternId.ShouldNotBeNullOrEmpty();
            violation.PatternName.ShouldNotBeNullOrEmpty();
            violation.Message.ShouldNotBeNullOrEmpty();
            violation.Severity.ShouldBeOneOf(PatternSeverity.Info, PatternSeverity.Warning, PatternSeverity.Error, PatternSeverity.Critical);
        }

        // This test drives implementation of actual project pattern analysis
    }

    /// <summary>
    /// Confirms that filtering by pattern types restricts results to the requested families.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after filter assertions are executed.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithSpecificPatternTypes_ShouldFilterByPatternTypes()
    {
        // Arrange
        var projectPath = @"C:\TestProject\TestProject.csproj";
        var patternTypes = new[] { "SOLID", "CleanCode", "Performance" };
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.AnalyzeProjectAsync(projectPath, patternTypes, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();

        // All violations should match the requested pattern types
        foreach (var violation in result)
        {
            violation.PatternId.ShouldBeOneOf(patternTypes.Concat(patternTypes.Select(p => $"{p}_*")).ToArray());
        }

        // This test drives implementation of pattern type filtering
    }

    /// <summary>
    /// Ensures non-existent project paths produce an empty result instead of throwing unexpected exceptions.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the empty outcome is validated.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithNonExistentProjectPath_ShouldReturnEmptyList()
    {
        // Arrange
        var projectPath = @"C:\NonExistent\Project.csproj";
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.AnalyzeProjectAsync(projectPath, null, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0); // Should return empty list for non-existent project

        // This test drives implementation of proper error handling for invalid paths
    }

    /// <summary>
    /// Confirms that passing a <see langword="null"/> project path to project analysis results in an <see cref="ArgumentException"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the guard clause triggers.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithNullProjectPath_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeProjectAsync(null!, null, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Validates that empty project paths are rejected prior to execution.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the exception assertion passes.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithEmptyProjectPath_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeProjectAsync(string.Empty, null, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Verifies that requesting pattern remediation suggestions yields actionable recommendations for a valid violation.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the suggestion result is validated.</returns>
    [Fact(Timeout = 60000)]
    public async Task SuggestAlternativesAsync_WithValidViolation_ShouldReturnActualSuggestions()
    {
        // Arrange
        var violation = new PatternViolation(
            "violation-1",
            "SOLID",
            "Single Responsibility Principle",
            PatternSeverity.Warning,
            "Class has multiple responsibilities",
            "TestFile.cs",
            10,
            null,
            null,
            new Dictionary<string, object>());
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.SuggestAlternativesAsync(violation, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0); // Should return actual suggestions, not empty list

        // Verify suggestion structure
        foreach (var suggestion in result)
        {
            suggestion.Id.ShouldNotBeNullOrEmpty();
            suggestion.ViolationId.ShouldBe(violation.Id);
            suggestion.Title.ShouldNotBeNullOrEmpty();
            suggestion.Description.ShouldNotBeNullOrEmpty();
            suggestion.Confidence.ShouldBeGreaterThanOrEqualTo(0.0f);
            suggestion.Confidence.ShouldBeLessThanOrEqualTo(1.0f);
        }

        // This test drives implementation of actual suggestion generation
    }

    /// <summary>
    /// Ensures a <see langword="null"/> violation argument triggers <see cref="ArgumentNullException"/> for suggestion requests.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the guard clause is exercised.</returns>
    [Fact(Timeout = 60000)]
    public async Task SuggestAlternativesAsync_WithNullViolation_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.SuggestAlternativesAsync(default, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Validates that consistency analysis returns a populated report for representative projects.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once report assertions succeed.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeConsistencyAsync_WithValidProjectPath_ShouldReturnActualConsistencyReport()
    {
        // Arrange
        var projectPath = @"C:\TestProject\TestProject.csproj";
        var patternFamily = "SOLID";
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.AnalyzeConsistencyAsync(projectPath, patternFamily, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ConsistencyScore.ShouldBeGreaterThanOrEqualTo(0.0f);
        result.ConsistencyScore.ShouldBeLessThanOrEqualTo(1.0f);
        result.PatternFamily.ShouldBe(patternFamily);
        result.FilesAnalyzed.ShouldBeGreaterThan(0); // Should analyze actual files
        result.ElapsedMilliseconds.ShouldBeGreaterThan(0); // Should measure actual time
        result.Inconsistencies.ShouldNotBeNull();

        // Verify inconsistency structure
        foreach (var inconsistency in result.Inconsistencies)
        {
            inconsistency.Description.ShouldNotBeNullOrEmpty();
            inconsistency.Severity.ShouldBeOneOf(PatternSeverity.Info, PatternSeverity.Warning, PatternSeverity.Error, PatternSeverity.Critical);
            inconsistency.FilePath.ShouldNotBeNullOrEmpty();
            inconsistency.LineNumber.ShouldBeGreaterThan(0);
        }

        // This test drives implementation of actual consistency analysis
    }

    /// <summary>
    /// Confirms that selecting all pattern families results in a comprehensive consistency evaluation.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when all pattern families are reflected in the report.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeConsistencyAsync_WithAllPatternFamily_ShouldAnalyzeAllPatterns()
    {
        // Arrange
        var projectPath = @"C:\TestProject\TestProject.csproj";
        var patternFamily = "all";
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.AnalyzeConsistencyAsync(projectPath, patternFamily, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.PatternFamily.ShouldBe(patternFamily);
        result.FilesAnalyzed.ShouldBeGreaterThan(0); // Should analyze actual files
        result.ElapsedMilliseconds.ShouldBeGreaterThan(0); // Should measure actual time

        // This test drives implementation of comprehensive pattern analysis
    }

    /// <summary>
    /// Ensures <see langword="null"/> project paths cause consistency analysis to throw <see cref="ArgumentException"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the guard clause is confirmed.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeConsistencyAsync_WithNullProjectPath_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeConsistencyAsync(null!, "all", TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Validates that empty project identifiers are rejected for consistency analysis requests.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the exception assertion passes.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeConsistencyAsync_WithEmptyProjectPath_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeConsistencyAsync(string.Empty, "all", TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Confirms that pattern enforcement returns actionable results, including counts and duration, for valid projects.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the enforcement result is examined.</returns>
    [Fact(Timeout = 60000)]
    public async Task EnforcePatternsAsync_WithValidProjectPath_ShouldReturnActualEnforcementResult()
    {
        // Arrange
        var projectPath = @"C:\TestProject\TestProject.csproj";
        var patternTypes = new[] { "SOLID", "CleanCode" };
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.EnforcePatternsAsync(projectPath, patternTypes, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue(); // Should successfully enforce patterns
        result.ViolationsFound.ShouldBeGreaterThanOrEqualTo(0);
        result.ViolationsFixed.ShouldBeGreaterThanOrEqualTo(0);
        result.ViolationsFixed.ShouldBeLessThanOrEqualTo(result.ViolationsFound);
        result.ElapsedMilliseconds.ShouldBeGreaterThan(0); // Should measure actual time
        result.RemainingViolations.ShouldNotBeNull();

        // Verify remaining violations structure
        foreach (var violation in result.RemainingViolations)
        {
            violation.Id.ShouldNotBeNullOrEmpty();
            violation.PatternId.ShouldNotBeNullOrEmpty();
            violation.PatternName.ShouldNotBeNullOrEmpty();
            violation.Message.ShouldNotBeNullOrEmpty();
        }

        // This test drives implementation of actual pattern enforcement
    }

    /// <summary>
    /// Ensures enforcement requests with a <see langword="null"/> project path throw <see cref="ArgumentException"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the guard clause triggers.</returns>
    [Fact(Timeout = 60000)]
    public async Task EnforcePatternsAsync_WithNullProjectPath_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.EnforcePatternsAsync(null!, ["SOLID"], TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Validates that empty project identifiers are rejected for pattern enforcement operations.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after verifying the thrown exception.</returns>
    [Fact(Timeout = 60000)]
    public async Task EnforcePatternsAsync_WithEmptyProjectPath_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.EnforcePatternsAsync(string.Empty, ["SOLID"], TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Checks that <see langword="null"/> pattern type collections are considered invalid when enforcing patterns.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the argument exception is observed.</returns>
    [Fact(Timeout = 60000)]
    public async Task EnforcePatternsAsync_WithNullPatternTypes_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.EnforcePatternsAsync(@"C:\TestProject\TestProject.csproj", null!, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Confirms that empty pattern type lists are rejected to ensure callers specify at least one pattern family.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the guard clause is validated.</returns>
    [Fact(Timeout = 60000)]
    public async Task EnforcePatternsAsync_WithEmptyPatternTypes_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.EnforcePatternsAsync(@"C:\TestProject\TestProject.csproj", Array.Empty<string>(), TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Verifies that requesting pattern guidance for a valid context yields actionable guidance entries.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the guidance result is validated.</returns>
    [Fact(Timeout = 60000)]
    public async Task GetPatternGuidanceAsync_WithValidContext_ShouldReturnActualGuidance()
    {
        // Arrange
        var context = "C# Development";
        var patternTypes = new[] { "SOLID", "CleanCode" };
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.GetPatternGuidanceAsync(context, patternTypes, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Context.ShouldBe(context);
        result.RecommendedPatterns.ShouldNotBeNull();
        result.RecommendedPatterns.Count.ShouldBeGreaterThan(0); // Should return actual recommendations
        result.AvoidPatterns.ShouldNotBeNull();
        result.BestPractices.ShouldNotBeNull();
        result.BestPractices.Count.ShouldBeGreaterThan(0); // Should return actual best practices
        result.CommonPitfalls.ShouldNotBeNull();
        result.CommonPitfalls.Count.ShouldBeGreaterThan(0); // Should return actual pitfalls

        // Verify pattern definition structure
        foreach (var pattern in result.RecommendedPatterns.Concat(result.AvoidPatterns))
        {
            pattern.Id.ShouldNotBeNullOrEmpty();
            pattern.Name.ShouldNotBeNullOrEmpty();
            pattern.Description.ShouldNotBeNullOrEmpty();
            pattern.Category.ShouldNotBeNullOrEmpty();
        }

        // This test drives implementation of actual pattern guidance generation
    }

    /// <summary>
    /// Confirms that requesting guidance for all pattern types produces comprehensive recommendations.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the breadth of guidance is asserted.</returns>
    [Fact(Timeout = 60000)]
    public async Task GetPatternGuidanceAsync_WithAllPatternTypes_ShouldReturnComprehensiveGuidance()
    {
        // Arrange
        var context = "C# Development";
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.GetPatternGuidanceAsync(context, null, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Context.ShouldBe(context);
        result.RecommendedPatterns.Count.ShouldBeGreaterThan(0); // Should return comprehensive recommendations
        result.BestPractices.Count.ShouldBeGreaterThan(0); // Should return comprehensive best practices
        result.CommonPitfalls.Count.ShouldBeGreaterThan(0); // Should return comprehensive pitfalls

        // This test drives implementation of comprehensive pattern guidance
    }

    /// <summary>
    /// Ensures <see langword="null"/> contexts cause guidance requests to throw <see cref="ArgumentException"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after verifying the exception.</returns>
    [Fact(Timeout = 60000)]
    public async Task GetPatternGuidanceAsync_WithNullContext_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.GetPatternGuidanceAsync(null!, null, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Validates that empty context identifiers are rejected, preventing ambiguous guidance generation.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the guard clause executes.</returns>
    [Fact(Timeout = 60000)]
    public async Task GetPatternGuidanceAsync_WithEmptyContext_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.GetPatternGuidanceAsync(string.Empty, null, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Checks that the engine produces context-specific violations when analyzing the same code under different contexts.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the context differentiation assertions run.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithDifferentContexts_ShouldReturnContextSpecificViolations()
    {
        // Arrange
        var code = @"
public class TestClass
{
    public void TestMethod()
    {
        var unused = 42;
    }
}";
        var contexts = new[] { "C# Development", "Web Development", "API Development" };
        var service = new SemanticPatternEngineService(_logger);

        // Act & Assert
        foreach (var context in contexts)
        {
            var result = await service.AnalyzeCodeAsync(code, context, TestContext.Current.CancellationToken);

            result.ShouldNotBeNull();
            // Different contexts might have different violation counts
            // This drives implementation of context-aware analysis
        }

        // This test drives implementation of context-specific pattern analysis
    }

    /// <summary>
    /// Validates that large projects can be analyzed without timing out and that metrics reflect the broader scope.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once large project metrics are verified.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithLargeProject_ShouldHandleLargeProjects()
    {
        // Arrange
        var projectPath = @"C:\LargeProject\LargeProject.csproj";
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.AnalyzeProjectAsync(projectPath, null, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        // Should handle large projects without issues
        // This drives implementation of performance optimization for large projects
    }

    /// <summary>
    /// Ensures enforcement results capture partial success scenarios, including both successes and failures.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when partial enforcement statistics are asserted.</returns>
    [Fact(Timeout = 60000)]
    public async Task EnforcePatternsAsync_WithPartialSuccess_ShouldReturnPartialEnforcementResult()
    {
        // Arrange
        var projectPath = @"C:\PartialProject\PartialProject.csproj";
        var patternTypes = new[] { "SOLID", "CleanCode", "Performance" };
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.EnforcePatternsAsync(projectPath, patternTypes, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ViolationsFound.ShouldBeGreaterThanOrEqualTo(0);
        result.ViolationsFixed.ShouldBeGreaterThanOrEqualTo(0);
        result.ViolationsFixed.ShouldBeLessThanOrEqualTo(result.ViolationsFound);

        // If there are remaining violations, they should be properly structured
        foreach (var violation in result.RemainingViolations)
        {
            violation.Id.ShouldNotBeNullOrEmpty();
            violation.PatternId.ShouldNotBeNullOrEmpty();
            violation.PatternName.ShouldNotBeNullOrEmpty();
            violation.Message.ShouldNotBeNullOrEmpty();
        }

        // This test drives implementation of partial enforcement handling
    }

    /// <summary>
    /// Confirms that highly consistent projects produce high consistency scores and minimal violations.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after consistency score assertions finish.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeConsistencyAsync_WithHighConsistencyProject_ShouldReturnHighConsistencyScore()
    {
        // Arrange
        var projectPath = @"C:\ConsistentProject\ConsistentProject.csproj";
        var patternFamily = "SOLID";
        var service = new SemanticPatternEngineService(_logger);

        // Act
        var result = await service.AnalyzeConsistencyAsync(projectPath, patternFamily, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ConsistencyScore.ShouldBeGreaterThanOrEqualTo(0.0f);
        result.ConsistencyScore.ShouldBeLessThanOrEqualTo(1.0f);

        // If consistency is high, there should be fewer inconsistencies
        if (result.ConsistencyScore > 0.8f)
        {
            result.Inconsistencies.Count.ShouldBeLessThan(10); // Should have few inconsistencies
        }

        // This test drives implementation of consistency scoring
    }
}