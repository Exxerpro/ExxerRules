using IndQuestResults;

namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Defines the contract for safe regex-based code transformations with validation.
/// </summary>
public interface ISafeRegexService
{
    /// <summary>
    /// Performs safe regex-based code transformations with dry-run validation and build verification.
    /// </summary>
    /// <param name="request">The regex transformation request containing pattern, replacement, and validation settings.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing transformation preview, validation results, and application status.</returns>
    Task<Result<SafeRegexResult>> ApplySafeRegexAsync(SafeRegexRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a regex pattern for safety and potential issues.
    /// </summary>
    /// <param name="pattern">The regex pattern to validate.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing validation results and safety assessment.</returns>
    Task<Result<RegexValidationResult>> ValidateRegexPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a dry-run of regex transformation without applying changes.
    /// </summary>
    /// <param name="request">The regex transformation request.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing preview of changes that would be made.</returns>
    Task<Result<SafeRegexPreviewResult>> PreviewRegexTransformationAsync(SafeRegexRequest request, CancellationToken cancellationToken = default);
}
