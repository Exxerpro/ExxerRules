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