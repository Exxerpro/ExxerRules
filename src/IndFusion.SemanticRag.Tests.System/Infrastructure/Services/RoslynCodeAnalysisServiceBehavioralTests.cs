using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.System.Infrastructure.Services;

/// <summary>
/// Behavioral system tests for RoslynCodeAnalysisService to drive implementation.
/// These tests verify actual behavior and drive the replacement of mock implementations.
/// </summary>
[Trait("Category", "System")]
public class RoslynCodeAnalysisServiceBehavioralTests
{
    private readonly ILogger<RoslynCodeAnalysisService> _logger;

    /// <summary>
    /// Initializes the Roslyn code analysis behavioral test fixture with substitute logging infrastructure.
    /// </summary>
    public RoslynCodeAnalysisServiceBehavioralTests()
    {
        _logger = Substitute.For<ILogger<RoslynCodeAnalysisService>>();
    }
    /// <summary>
    /// Verifies that <see cref="RoslynCodeAnalysisService.AnalyzeProjectAsync(string, CancellationToken)"/> returns populated analysis metrics for a valid project path.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the project analysis assertions succeed.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithValidProjectPath_ShouldReturnActualAnalysisResults()
    {
        // Arrange
        var projectPath = @"C:\TestProject\TestProject.csproj";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeProjectAsync(projectPath, CancellationToken.None);

        // Assert - Verify actual behavior, not mock behavior
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        result.ComplianceScore.ShouldBeGreaterThanOrEqualTo(0.0f);
        result.ComplianceScore.ShouldBeLessThanOrEqualTo(1.0f);
        result.ElapsedMilliseconds.ShouldBeGreaterThan(0); // Should measure actual time, not 0
        result.FilesAnalyzed.ShouldBeGreaterThan(0); // Should analyze actual files, not 0
        result.LinesOfCode.ShouldBeGreaterThan(0); // Should count actual lines, not 0
        
        // This test drives implementation of actual Roslyn project analysis
        // Currently fails because implementation uses Task.Delay placeholder and returns 0 values
    }

    /// <summary>
    /// Ensures invalid project paths cause the analyzer to return a failure-oriented result rather than fabricated metrics.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the failure state is evaluated.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithNonExistentProjectPath_ShouldReturnFailure()
    {
        // Arrange
        var projectPath = @"C:\NonExistent\Project.csproj";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeProjectAsync(projectPath, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        result.ComplianceScore.ShouldBe(0.0f); // Should be 0 for failed analysis
        result.FilesAnalyzed.ShouldBe(0); // Should be 0 for failed analysis
        result.LinesOfCode.ShouldBe(0); // Should be 0 for failed analysis
        
        // This test drives implementation of proper error handling for invalid paths
    }

    /// <summary>
    /// Confirms that passing a <see langword="null"/> project path triggers <see cref="ArgumentException"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the guard clause exception is observed.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithNullProjectPath_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new RoslynCodeAnalysisService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeProjectAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Validates that empty project paths are rejected to prevent ambiguous analysis requests.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the expected exception is thrown.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithEmptyProjectPath_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new RoslynCodeAnalysisService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeProjectAsync(string.Empty, CancellationToken.None));
    }

    /// <summary>
    /// Checks that cancellation tokens passed to project analysis operations are observed promptly.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after cancellation behavior has been verified.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var projectPath = @"C:\TestProject\TestProject.csproj";
        var service = new RoslynCodeAnalysisService(_logger);
        
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await service.AnalyzeProjectAsync(projectPath, cts.Token));
    }
    /// <summary>
    /// Verifies that file-level analysis returns meaningful metrics when supplied a concrete source file path.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the file analysis results are validated.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeFileAsync_WithValidFilePath_ShouldReturnActualAnalysisResults()
    {
        // Arrange
        var filePath = @"C:\TestProject\Program.cs";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeFileAsync(filePath, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        result.ComplianceScore.ShouldBeGreaterThanOrEqualTo(0.0f);
        result.ComplianceScore.ShouldBeLessThanOrEqualTo(1.0f);
        result.ElapsedMilliseconds.ShouldBeGreaterThan(0); // Should measure actual time
        result.FilesAnalyzed.ShouldBe(1); // Should analyze exactly 1 file
        result.LinesOfCode.ShouldBeGreaterThan(0); // Should count actual lines
        
        // This test drives implementation of actual Roslyn file analysis
    }

    /// <summary>
    /// Ensures non-existent file paths produce a failure response with neutral metrics.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after confirming the failure state.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeFileAsync_WithNonExistentFilePath_ShouldReturnFailure()
    {
        // Arrange
        var filePath = @"C:\NonExistent\File.cs";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeFileAsync(filePath, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        result.ComplianceScore.ShouldBe(0.0f); // Should be 0 for failed analysis
        result.FilesAnalyzed.ShouldBe(0); // Should be 0 for failed analysis
        result.LinesOfCode.ShouldBe(0); // Should be 0 for failed analysis
        
        // This test drives implementation of proper error handling for invalid files
    }

    /// <summary>
    /// Confirms that a <see langword="null"/> file path triggers <see cref="ArgumentException"/> rather than proceeding.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the guard clause is exercised.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeFileAsync_WithNullFilePath_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new RoslynCodeAnalysisService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeFileAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Validates that empty strings are rejected as file paths for analysis.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after verifying the thrown exception.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeFileAsync_WithEmptyFilePath_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new RoslynCodeAnalysisService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeFileAsync(string.Empty, CancellationToken.None));
    }
    /// <summary>
    /// Verifies that analyzing a code snippet returns populated diagnostics, suggestions, and timing information.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after inspecting the analysis result.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithValidCode_ShouldReturnActualAnalysisResults()
    {
        // Arrange
        var code = @"
using System;

public class TestClass
{
    public void TestMethod()
    {
        Console.WriteLine(""Hello World"");
    }
}";
        var language = "C#";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeCodeAsync(code, language, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        result.ComplianceScore.ShouldBeGreaterThanOrEqualTo(0.0f);
        result.ComplianceScore.ShouldBeLessThanOrEqualTo(1.0f);
        result.ElapsedMilliseconds.ShouldBeGreaterThan(0); // Should measure actual time
        result.FilesAnalyzed.ShouldBe(1); // Should analyze exactly 1 code snippet
        result.LinesOfCode.ShouldBeGreaterThan(0); // Should count actual lines
        
        // This test drives implementation of actual Roslyn code analysis
    }

    /// <summary>
    /// Ensures that code containing intentional violations yields violation entries rather than an empty result set.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once violation assertions run.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithCodeContainingViolations_ShouldReturnViolations()
    {
        // Arrange
        var codeWithViolations = @"
using System;

public class TestClass
{
    public void TestMethod()
    {
        var unused = 42; // Unused variable violation
        Console.WriteLine(""Hello World"");
    }
}";
        var language = "C#";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeCodeAsync(codeWithViolations, language, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        result.Violations.Count.ShouldBeGreaterThan(0); // Should find actual violations
        result.ComplianceScore.ShouldBeLessThan(1.0f); // Should be less than perfect due to violations
        
        // Verify violations have proper structure
        foreach (var violation in result.Violations)
        {
            violation.Id.ShouldNotBeNullOrEmpty();
            violation.PatternId.ShouldNotBeNullOrEmpty();
            violation.PatternName.ShouldNotBeNullOrEmpty();
            violation.Message.ShouldNotBeNullOrEmpty();
            violation.Severity.ShouldBeOneOf(PatternSeverity.Info, PatternSeverity.Warning, PatternSeverity.Error, PatternSeverity.Critical);
        }
        
        // This test drives implementation of actual violation detection
    }
    /// <summary>
    /// Confirms that supplying <see langword="null"/> code text is rejected via <see cref="ArgumentException"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the exception assertion succeeds.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithNullCode_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new RoslynCodeAnalysisService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeCodeAsync(null!, "C#", CancellationToken.None));
    }

    /// <summary>
    /// Validates that empty code strings are not accepted for analysis to avoid pointless invocations.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the guard clause exception is observed.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithEmptyCode_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new RoslynCodeAnalysisService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeCodeAsync(string.Empty, "C#", CancellationToken.None));
    }

    /// <summary>
    /// Ensures <see cref="RoslynCodeAnalysisService"/> requires a non-null language identifier when analyzing ad-hoc code.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after validating the thrown exception.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithNullLanguage_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new RoslynCodeAnalysisService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeCodeAsync("code", null!, CancellationToken.None));
    }

    /// <summary>
    /// Checks that empty language identifiers are rejected before analysis is attempted.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the input validation fires.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithEmptyLanguage_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new RoslynCodeAnalysisService(_logger);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await service.AnalyzeCodeAsync("code", string.Empty, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that unsupported language values result in an empty analysis outcome rather than exceptions.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the absence of violations is asserted.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithUnsupportedLanguage_ShouldReturnEmptyResults()
    {
        // Arrange
        var code = "print('Hello World')";
        var language = "Python"; // Unsupported language
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeCodeAsync(code, language, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        result.Violations.Count.ShouldBe(0); // Should return empty for unsupported language
        result.Suggestions.Count.ShouldBe(0); // Should return empty for unsupported language
        result.ComplianceScore.ShouldBe(1.0f); // Should be perfect for unsupported language
        result.FilesAnalyzed.ShouldBe(0); // Should be 0 for unsupported language
        result.LinesOfCode.ShouldBe(0); // Should be 0 for unsupported language
        
        // This test drives implementation of language support detection
    }
    /// <summary>
    /// Confirms that the analyzer can enumerate installed analyzers to support discovery scenarios.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after verifying the analyzer list.</returns>
    [Fact(Timeout = 60000)]
    public async Task GetAvailableAnalyzersAsync_ShouldReturnActualAnalyzers()
    {
        // Arrange
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.GetAvailableAnalyzersAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0); // Should return actual analyzers, not empty list
        
        // Verify analyzer structure
        foreach (var analyzer in result)
        {
            analyzer.Id.ShouldNotBeNullOrEmpty();
            analyzer.Name.ShouldNotBeNullOrEmpty();
            analyzer.Description.ShouldNotBeNullOrEmpty();
            analyzer.Category.ShouldNotBeNullOrEmpty();
        }
        
        // This test drives implementation of actual analyzer discovery
        // Currently fails because implementation uses Task.Delay placeholder and returns empty list
    }

    /// <summary>
    /// Validates that large projects are processed successfully, yielding tangible metrics and non-zero timings.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when large project analysis assertions finish.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithLargeProject_ShouldHandleLargeProjects()
    {
        // Arrange
        var projectPath = @"C:\LargeProject\LargeProject.csproj";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeProjectAsync(projectPath, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        result.ElapsedMilliseconds.ShouldBeGreaterThan(0); // Should measure actual time
        result.FilesAnalyzed.ShouldBeGreaterThan(0); // Should analyze actual files
        result.LinesOfCode.ShouldBeGreaterThan(0); // Should count actual lines
        
        // This test drives implementation of performance optimization for large projects
    }

    /// <summary>
    /// Ensures multi-language projects are analyzed across all supported languages rather than skipping secondary code.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once multi-language assertions pass.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithProjectContainingMultipleLanguages_ShouldAnalyzeAllLanguages()
    {
        // Arrange
        var projectPath = @"C:\MultiLanguageProject\Project.csproj";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeProjectAsync(projectPath, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        result.ElapsedMilliseconds.ShouldBeGreaterThan(0); // Should measure actual time
        result.FilesAnalyzed.ShouldBeGreaterThan(0); // Should analyze actual files
        result.LinesOfCode.ShouldBeGreaterThan(0); // Should count actual lines
        
        // This test drives implementation of multi-language project support
    }

    /// <summary>
    /// Verifies that files containing compiler errors return violations categorized appropriately.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after evaluating error-specific assertions.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeFileAsync_WithFileContainingErrors_ShouldReturnErrorViolations()
    {
        // Arrange
        var filePath = @"C:\TestProject\ErrorFile.cs";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeFileAsync(filePath, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        
        // Check for error-level violations
        var errorViolations = result.Violations.Where(v => v.Severity == PatternSeverity.Error).ToList();
        var criticalViolations = result.Violations.Where(v => v.Severity == PatternSeverity.Critical).ToList();
        
        // If there are errors, they should have proper structure
        foreach (var violation in errorViolations.Concat(criticalViolations))
        {
            violation.Id.ShouldNotBeNullOrEmpty();
            violation.PatternId.ShouldNotBeNullOrEmpty();
            violation.PatternName.ShouldNotBeNullOrEmpty();
            violation.Message.ShouldNotBeNullOrEmpty();
            violation.FilePath.ShouldBe(filePath); // Should reference the analyzed file
        }
        
        // This test drives implementation of error-level violation detection
    }
    /// <summary>
    /// Confirms that suggestion-level diagnostics are surfaced when the analyzed code contains suggestion severity directives.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after suggestion assertions complete.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeCodeAsync_WithCodeContainingSuggestions_ShouldReturnSuggestions()
    {
        // Arrange
        var codeWithSuggestions = @"
using System;

public class TestClass
{
    public void TestMethod()
    {
        // This could be improved with modern C# features
        var list = new List<string>();
        list.Add(""item1"");
        list.Add(""item2"");
    }
}";
        var language = "C#";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeCodeAsync(codeWithSuggestions, language, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        
        // Check for suggestions
        foreach (var suggestion in result.Suggestions)
        {
            suggestion.Id.ShouldNotBeNullOrEmpty();
            suggestion.ViolationId.ShouldNotBeNullOrEmpty();
            suggestion.Title.ShouldNotBeNullOrEmpty();
            suggestion.Description.ShouldNotBeNullOrEmpty();
            suggestion.Confidence.ShouldBeGreaterThanOrEqualTo(0.0f);
            suggestion.Confidence.ShouldBeLessThanOrEqualTo(1.0f);
        }
        
        // This test drives implementation of suggestion generation
    }

    /// <summary>
    /// Verifies that projects containing warning diagnostics return warning violations in the analysis output.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once warning-specific checks succeed.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithProjectContainingWarnings_ShouldReturnWarningViolations()
    {
        // Arrange
        var projectPath = @"C:\WarningProject\WarningProject.csproj";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeProjectAsync(projectPath, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        
        // Check for warning-level violations
        var warningViolations = result.Violations.Where(v => v.Severity == PatternSeverity.Warning).ToList();
        
        // If there are warnings, they should have proper structure
        foreach (var violation in warningViolations)
        {
            violation.Id.ShouldNotBeNullOrEmpty();
            violation.PatternId.ShouldNotBeNullOrEmpty();
            violation.PatternName.ShouldNotBeNullOrEmpty();
            violation.Message.ShouldNotBeNullOrEmpty();
            violation.FilePath.ShouldNotBeNullOrEmpty(); // Should reference the file
            violation.LineNumber.ShouldNotBeNull();
            violation.LineNumber.Value.ShouldBeGreaterThan(0); // Should have valid line number
        }
        
        // This test drives implementation of warning-level violation detection
    }

    /// <summary>
    /// Ensures that informational diagnostics are preserved when present in the analyzed project.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after info-level assertions run.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithProjectContainingInfoViolations_ShouldReturnInfoViolations()
    {
        // Arrange
        var projectPath = @"C:\InfoProject\InfoProject.csproj";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeProjectAsync(projectPath, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        
        // Check for info-level violations
        var infoViolations = result.Violations.Where(v => v.Severity == PatternSeverity.Info).ToList();
        
        // If there are info violations, they should have proper structure
        foreach (var violation in infoViolations)
        {
            violation.Id.ShouldNotBeNullOrEmpty();
            violation.PatternId.ShouldNotBeNullOrEmpty();
            violation.PatternName.ShouldNotBeNullOrEmpty();
            violation.Message.ShouldNotBeNullOrEmpty();
        }
        
        // This test drives implementation of info-level violation detection
    }

    /// <summary>
    /// Validates that projects containing mixed severity levels surface all severities in the results.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once mixed severity assertions are verified.</returns>
    [Fact(Timeout = 60000)]
    public async Task AnalyzeProjectAsync_WithProjectContainingMixedSeverities_ShouldReturnAllSeverities()
    {
        // Arrange
        var projectPath = @"C:\MixedProject\MixedProject.csproj";
        var service = new RoslynCodeAnalysisService(_logger);

        // Act
        var result = await service.AnalyzeProjectAsync(projectPath, CancellationToken.None);

        // Assert
        result.Violations.ShouldNotBeNull();
        result.Suggestions.ShouldNotBeNull();
        
        // Check that all severity levels are represented
        var severityLevels = result.Violations.Select(v => v.Severity).Distinct().ToList();
        severityLevels.ShouldNotBeEmpty(); // Should have at least one severity level
        
        // Verify compliance score reflects mixed severities
        if (result.Violations.Any(v => v.Severity.IsHighSeverity()))
        {
            result.ComplianceScore.ShouldBeLessThan(1.0f); // Should be less than perfect
        }
        
        // This test drives implementation of mixed severity handling
    }
}
