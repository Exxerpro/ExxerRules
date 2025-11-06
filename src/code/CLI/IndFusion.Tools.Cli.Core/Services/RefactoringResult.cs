namespace IndFusion.Tools.Cli.Core.Services;

/// <summary>
/// Represents the result of a refactoring operation
/// </summary>
public class RefactoringResult
{
    /// <summary>
    /// Gets whether the Refactoring was successful
    /// </summary>
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage) && (!string.IsNullOrEmpty(Summary) || !string.IsNullOrEmpty(Preview));

    /// <summary>
    /// Gets the error message if the refactoring failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Gets the summary of changes made
    /// </summary>
    public string? Summary { get; private set; }

    /// <summary>
    /// Gets the preview of changes
    /// </summary>
    public string? Preview { get; private set; }

    /// <summary>
    /// Creates a successful refactoring result
    /// </summary>
    /// <param name="summary">Summary of changes</param>
    /// <param name="preview">Preview of changes</param>
    /// <returns>Successful refactoring result</returns>
    public static RefactoringResult Success(string? summary = null, string? preview = null)
    {
        return new RefactoringResult { Summary = summary, Preview = preview };
    }

    /// <summary>
    /// Creates a failed refactoring result
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Failed refactoring result</returns>
    public static RefactoringResult Failure(string errorMessage)
    {
        return new RefactoringResult { ErrorMessage = errorMessage };
    }
}