namespace IndFusion.Tools.Cli.Services;

/// <summary>
/// Represents the result of code analysis
/// </summary>
public class AnalysisResult
{
    /// <summary>
    /// Gets whether the Analysis was successful
    /// </summary>
    private bool isSuccess => string.IsNullOrEmpty(ErrorMessage) && Output is not null;

    /// <summary>
    /// Gets the error message if the analysis failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Gets the analysis output
    /// </summary>
    public AnalysisOutput? Output { get; private set; }

    /// <summary>
    /// Creates a successful analysis result
    /// </summary>
    /// <param name="output">The analysis output</param>
    /// <returns>Successful analysis result</returns>
    public static AnalysisResult Success(AnalysisOutput output)
    {
        return new AnalysisResult { Output = output };
    }

    /// <summary>
    /// Creates a failed analysis result
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Failed analysis result</returns>
    public static AnalysisResult Failure(string errorMessage)
    {
        return new AnalysisResult { ErrorMessage = errorMessage };
    }
}