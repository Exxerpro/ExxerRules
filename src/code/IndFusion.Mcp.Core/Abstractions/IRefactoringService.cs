using Microsoft.CodeAnalysis;

namespace IndFusion.Mcp.Mcp.Core.Abstractions;

/// <summary>
/// Defines the contract for high-level refactoring operations provided by the core service.
/// Implementations should orchestrate analysis and code transformations while handling logging and errors.
/// </summary>
public interface IExxerFactoringService
{
    /// <summary>
    /// Executes a refactoring tool by name with the provided request payload.
    /// </summary>
    /// <param name="request">The refactoring request describing the tool, target solution and parameters.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result object indicating success, messages, and any produced artifacts.</returns>
    Task<ExxerFactoringResult> ExecuteExxerFactoringAsync(ExxerFactoringRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts a block of code into a new method within the same type.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the source file containing the code to extract.</param>
    /// <param name="startLine">The starting line (1-based) of the code block.</param>
    /// <param name="endLine">The ending line (1-based) of the code block.</param>
    /// <param name="newMethodName">The name of the newly created method.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The refactoring result including any messages or updated code.</returns>
    Task<ExxerFactoringResult> ExtractMethodAsync(string solutionPath, string filePath, int startLine, int endLine, string newMethodName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a method from a source file/type to a target type.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file (.sln).</param>
    /// <param name="sourceFilePath">Path to the file that contains the method to move.</param>
    /// <param name="methodName">The method name to move.</param>
    /// <param name="targetClassName">The fully qualified name of the target type.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The refactoring result including any messages or updated code.</returns>
    Task<ExxerFactoringResult> MoveMethodAsync(string solutionPath, string sourceFilePath, string methodName, string targetClassName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Introduces a new variable for the selected expression at the given location.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the source file containing the expression.</param>
    /// <param name="line">The target line (1-based).</param>
    /// <param name="column">The target column (1-based).</param>
    /// <param name="variableName">The variable name to introduce.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The refactoring result including any messages or updated code.</returns>
    Task<ExxerFactoringResult> IntroduceVariableAsync(string solutionPath, string filePath, int line, int column, string variableName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Produces code or project metrics for the specified path within the solution.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file (.sln).</param>
    /// <param name="path">A file or directory path relative to the solution.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A JSON string containing metrics or an error message.</returns>
    Task<string> GetMetricsAsync(string solutionPath, string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the refactoring tools currently available in the system.
    /// </summary>
    /// <returns>A list of tool identifiers that can be executed.</returns>
    Task<IEnumerable<string>> ListAvailableToolsAsync();
}
