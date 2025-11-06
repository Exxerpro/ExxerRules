using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for analyzer integration in the Semantic RAG system.
/// This defines the contract for running analyzers and processing diagnostics.
/// </summary>
public interface IAnalyzerIntegrationPort
{
    /// <summary>
    /// Runs analyzers on a solution and returns diagnostics.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file.</param>
    /// <param name="options">Analyzer execution options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of analyzer diagnostics.</returns>
    Task<IEnumerable<AnalyzerDiagnostic>> RunAnalyzersAsync(string solutionPath, AnalyzerExecutionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies code fixes for specific diagnostics.
    /// </summary>
    /// <param name="diagnostics">Diagnostics to fix.</param>
    /// <param name="options">Fix application options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing applied fixes and validation results.</returns>
    Task<CodeFixResult> ApplyCodeFixesAsync(IEnumerable<AnalyzerDiagnostic> diagnostics, CodeFixOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that applied fixes don't introduce new issues.
    /// </summary>
    /// <param name="originalCode">Original code before fixes.</param>
    /// <param name="fixedCode">Code after applying fixes.</param>
    /// <param name="options">Validation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Validation result with any new issues found.</returns>
    Task<FixValidationResult> ValidateFixesAsync(string originalCode, string fixedCode, FixValidationOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available analyzers and their capabilities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of available analyzers with metadata.</returns>
    Task<IEnumerable<AnalyzerMetadata>> GetAvailableAnalyzersAsync(CancellationToken cancellationToken = default);
}