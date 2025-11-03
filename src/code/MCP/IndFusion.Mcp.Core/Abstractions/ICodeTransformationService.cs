namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Defines the contract for code transformation services that provide deterministic code repair
/// with validation using Fixer001 and safe regex operations.
/// </summary>
public interface ICodeTransformationService
{
    /// <summary>
    /// Applies Fixer001 transformations to resolve specific diagnostics with build validation.
    /// </summary>
    /// <param name="request">The fixer request containing diagnostic ID, target files, and validation options.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing transformation details, validation results, and diff previews.</returns>
    Task<CodeTransformationResult> ApplyFixer001Async(Fixer001Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs safe regex-based code transformations with dry-run validation and build verification.
    /// </summary>
    /// <param name="request">The regex transformation request containing pattern, replacement, and validation settings.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing transformation preview, validation results, and application status.</returns>
    Task<CodeTransformationResult> ApplySafeRegexAsync(SafeRegexRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates code transformations by running analyzers and build verification on temporary workspace.
    /// </summary>
    /// <param name="request">The validation request containing transformed code and validation criteria.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing validation results, analyzer output, and build status.</returns>
    Task<TransformationValidationResult> ValidateTransformationAsync(TransformationValidationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compares code revisions to highlight semantic drift and suggest fixes.
    /// </summary>
    /// <param name="request">The change review request containing original and modified code.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing semantic diff analysis, drift detection, and fix suggestions.</returns>
    Task<SemanticChangeReviewResult> ReviewSemanticChangesAsync(SemanticChangeReviewRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current Fixer001 configuration and available transformations.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file (.sln).</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The current Fixer001 configuration including available transformations and settings.</returns>
    Task<Fixer001Configuration> GetFixer001ConfigurationAsync(string solutionPath, CancellationToken cancellationToken = default);
}