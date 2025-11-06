namespace IndFusion.Tools.Cli.Core.Commands;

/// <summary>
/// Represents the result of an interactive session
/// </summary>
public class SessionResult
{
    /// <summary>
    /// Gets whether the session was successful
    /// </summary>
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage) && !string.IsNullOrEmpty(SuccessMessage);

    /// <summary>
    /// Gets the error message if the session failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Gets the success message if the session succeeded
    /// </summary>
    public string? SuccessMessage { get; private set; }

    /// <summary>
    /// Creates a successful session result
    /// </summary>
    /// <param name="message">Success message</param>
    /// <returns>Successful session result</returns>
    public static SessionResult Success(string message)
    {
        return new SessionResult { SuccessMessage = message };
    }

    /// <summary>
    /// Creates a failed session result
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Failed session result</returns>
    public static SessionResult Failure(string errorMessage)
    {
        return new SessionResult { ErrorMessage = errorMessage };
    }
}