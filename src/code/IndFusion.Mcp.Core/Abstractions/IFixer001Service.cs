using IndQuestResults;

namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Defines the contract for Fixer001 code transformation service.
/// </summary>
public interface IFixer001Service
{
    /// <summary>
    /// Applies Fixer001 transformations to resolve specific diagnostics with build validation.
    /// </summary>
    /// <param name="request">The fixer request containing diagnostic ID, target files, and validation options.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing transformation details, validation results, and diff previews.</returns>
    Task<Result<Fixer001Result>> ApplyFixer001Async(Fixer001Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current Fixer001 configuration and available transformations.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file (.sln).</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The current Fixer001 configuration including available transformations and settings.</returns>
    Task<Result<Fixer001Configuration>> GetFixer001ConfigurationAsync(string solutionPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a dry-run of Fixer001 transformation without applying changes.
    /// </summary>
    /// <param name="request">The fixer request.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing preview of changes that would be made.</returns>
    Task<Result<Fixer001PreviewResult>> PreviewFixer001TransformationAsync(Fixer001Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that Fixer001 can be applied to the specified files.
    /// </summary>
    /// <param name="request">The fixer request to validate.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing validation results and readiness assessment.</returns>
    Task<Result<Fixer001ValidationResult>> ValidateFixer001ReadinessAsync(Fixer001Request request, CancellationToken cancellationToken = default);
}
