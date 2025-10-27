using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Service for analyzing code using Roslyn analyzers.
/// </summary>
public interface ICodeAnalysisService
{
    /// <summary>
    /// Analyzes code in a project for patterns and violations.
    /// </summary>
    /// <param name="projectPath">Path to the project to analyze.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Analysis results.</returns>
    Task<CodeAnalysisResult> AnalyzeProjectAsync(
        string projectPath, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes a specific code file for patterns and violations.
    /// </summary>
    /// <param name="filePath">Path to the file to analyze.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Analysis results.</returns>
    Task<CodeAnalysisResult> AnalyzeFileAsync(
        string filePath, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes a code snippet for patterns and violations.
    /// </summary>
    /// <param name="code">Code to analyze.</param>
    /// <param name="language">Programming language of the code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Analysis results.</returns>
    Task<CodeAnalysisResult> AnalyzeCodeAsync(
        string code, 
        string language, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available analyzers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of available analyzers.</returns>
    Task<IReadOnlyList<AnalyzerInfo>> GetAvailableAnalyzersAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the result of code analysis.
/// </summary>
public record CodeAnalysisResult
{
    /// <summary>
    /// List of pattern violations found.
    /// </summary>
    public required IReadOnlyList<PatternViolation> Violations { get; init; }

    /// <summary>
    /// List of pattern suggestions.
    /// </summary>
    public required IReadOnlyList<PatternSuggestion> Suggestions { get; init; }

    /// <summary>
    /// Overall compliance score (0-1).
    /// </summary>
    public required float ComplianceScore { get; init; }

    /// <summary>
    /// Time taken for analysis in milliseconds.
    /// </summary>
    public required long ElapsedMilliseconds { get; init; }

    /// <summary>
    /// Number of files analyzed.
    /// </summary>
    public required int FilesAnalyzed { get; init; }

    /// <summary>
    /// Total lines of code analyzed.
    /// </summary>
    public required int LinesOfCode { get; init; }
}

/// <summary>
/// Represents information about an analyzer.
/// </summary>
public record AnalyzerInfo
{
    /// <summary>
    /// Unique identifier for the analyzer.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Name of the analyzer.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Description of what the analyzer does.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Category of the analyzer.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Whether the analyzer is enabled.
    /// </summary>
    public required bool IsEnabled { get; init; }
}
