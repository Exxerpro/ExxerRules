using IndQuestResults;

namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Defines the contract for build validation services that verify code transformations.
/// </summary>
public interface IBuildValidationService
{
    /// <summary>
    /// Validates code transformations by running analyzers and build verification on temporary workspace.
    /// </summary>
    /// <param name="request">The validation request containing transformed code and validation criteria.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing validation results, analyzer output, and build status.</returns>
    Task<Result<BuildValidationResult>> ValidateTransformationAsync(BuildValidationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a single file transformation.
    /// </summary>
    /// <param name="filePath">Path to the file to validate.</param>
    /// <param name="originalContent">Original file content.</param>
    /// <param name="transformedContent">Transformed file content.</param>
    /// <param name="validationOptions">Options for validation.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing validation results for the single file.</returns>
    Task<Result<FileValidationResult>> ValidateFileTransformationAsync(
        string filePath, 
        string originalContent, 
        string transformedContent, 
        TransformationValidationOptions validationOptions, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a temporary workspace for validation purposes.
    /// </summary>
    /// <param name="solutionPath">Path to the original solution.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing the temporary workspace path and cleanup information.</returns>
    Task<Result<TemporaryWorkspace>> CreateTemporaryWorkspaceAsync(string solutionPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up a temporary workspace.
    /// </summary>
    /// <param name="workspace">The temporary workspace to clean up.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result indicating success or failure of cleanup.</returns>
    Task<Result> CleanupTemporaryWorkspaceAsync(TemporaryWorkspace workspace, CancellationToken cancellationToken = default);
}
