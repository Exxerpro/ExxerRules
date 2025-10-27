using Microsoft.Extensions.Logging;
using IndFusion.Mcp.Core.Abstractions;
using System.Reflection;
using System.Text.Json;
using IndFusion.Mcp.Core.Tools;
using IndFusion.Mcp.Core.Move;

namespace IndFusion.Mcp.Core.Services;

/// <summary>
/// Default implementation of <see cref="IExxerFactoringService"/> that coordinates refactoring
/// operations, logs execution details and returns structured results.
/// </summary>
public class ExxerFactoringService : IExxerFactoringService
{
    private readonly ILogger<ExxerFactoringService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExxerFactoringService"/> class.
    /// </summary>
    /// <param name="logger">The logger used to record operational information and errors.</param>
    public ExxerFactoringService(ILogger<ExxerFactoringService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    /// <inheritdoc />
    public async Task<ExxerFactoringResult> ExecuteExxerFactoringAsync(ExxerFactoringRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing ExxerFactoring: {ToolName} on solution: {SolutionPath}",
            request.ToolName, request.SolutionPath);

        try
        {
            // Route to appropriate tool based on tool name
            return request.ToolName switch
            {
                "extract-method" => await HandleExtractMethodRequest(request, cancellationToken),
                "move-method" => await HandleMoveMethodRequest(request, cancellationToken),
                "introduce-variable" => await HandleIntroduceVariableRequest(request, cancellationToken),
                _ => new ExxerFactoringResult(
                    Success: false,
                    Message: $"Tool {request.ToolName} not yet implemented in core service"
                )
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing ExxerFactoring {ToolName}", request.ToolName);
            return new ExxerFactoringResult(
                Success: false,
                Message: "ExxerFactoring failed",
                ErrorDetails: ex.ToString()
            );
        }
    }

    private async Task<ExxerFactoringResult> HandleExtractMethodRequest(ExxerFactoringRequest request, CancellationToken cancellationToken)
    {
        if (!request.Parameters.TryGetValue("filePath", out var filePathObj) ||
            !request.Parameters.TryGetValue("startLine", out var startLineObj) ||
            !request.Parameters.TryGetValue("endLine", out var endLineObj) ||
            !request.Parameters.TryGetValue("methodName", out var methodNameObj))
        {
            return new ExxerFactoringResult(
                Success: false,
                Message: "Missing required parameters: filePath, startLine, endLine, methodName"
            );
        }

        var filePath = filePathObj.ToString()!;
        var startLine = Convert.ToInt32(startLineObj);
        var endLine = Convert.ToInt32(endLineObj);
        var methodName = methodNameObj.ToString()!;

        return await ExtractMethodAsync(request.SolutionPath, filePath, startLine, endLine, methodName, cancellationToken);
    }

    private async Task<ExxerFactoringResult> HandleMoveMethodRequest(ExxerFactoringRequest request, CancellationToken cancellationToken)
    {
        if (!request.Parameters.TryGetValue("sourceFilePath", out var sourceFilePathObj) ||
            !request.Parameters.TryGetValue("methodName", out var methodNameObj) ||
            !request.Parameters.TryGetValue("targetClassName", out var targetClassNameObj))
        {
            return new ExxerFactoringResult(
                Success: false,
                Message: "Missing required parameters: sourceFilePath, methodName, targetClassName"
            );
        }

        var sourceFilePath = sourceFilePathObj.ToString()!;
        var methodName = methodNameObj.ToString()!;
        var targetClassName = targetClassNameObj.ToString()!;

        return await MoveMethodAsync(request.SolutionPath, sourceFilePath, methodName, targetClassName, cancellationToken);
    }

    private async Task<ExxerFactoringResult> HandleIntroduceVariableRequest(ExxerFactoringRequest request, CancellationToken cancellationToken)
    {
        if (!request.Parameters.TryGetValue("filePath", out var filePathObj) ||
            !request.Parameters.TryGetValue("line", out var lineObj) ||
            !request.Parameters.TryGetValue("column", out var columnObj) ||
            !request.Parameters.TryGetValue("variableName", out var variableNameObj))
        {
            return new ExxerFactoringResult(
                Success: false,
                Message: "Missing required parameters: filePath, line, column, variableName"
            );
        }

        var filePath = filePathObj.ToString()!;
        var line = Convert.ToInt32(lineObj);
        var column = Convert.ToInt32(columnObj);
        var variableName = variableNameObj.ToString()!;

        return await IntroduceVariableAsync(request.SolutionPath, filePath, line, column, variableName, cancellationToken);
    }

    /// <inheritdoc />
    /// <inheritdoc />
    public async Task<ExxerFactoringResult> ExtractMethodAsync(string solutionPath, string filePath, int startLine, int endLine, string newMethodName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Extracting method {MethodName} from {FilePath} lines {StartLine}-{EndLine}",
            newMethodName, filePath, startLine, endLine);

        try
        {
            // Convert line numbers to selection range format expected by ExtractMethodTool
            var selectionRange = $"{startLine}:1-{endLine}:1";
            
            // Use the existing ExtractMethodTool
            var result = await ExtractMethodTool.ExtractMethod(
                solutionPath,
                filePath,
                selectionRange,
                newMethodName);

            return new ExxerFactoringResult(
                Success: true,
                Message: result,
                ModifiedFiles: new[] { filePath }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting method {MethodName}", newMethodName);
            return new ExxerFactoringResult(
                Success: false,
                Message: "Extract method failed",
                ErrorDetails: ex.ToString()
            );
        }
    }

    /// <inheritdoc />
    /// <inheritdoc />
    public async Task<ExxerFactoringResult> MoveMethodAsync(string solutionPath, string sourceFilePath, string methodName, string targetClassName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Moving method {MethodName} from {SourceFile} to {TargetClass}",
            methodName, sourceFilePath, targetClassName);

        try
        {
            // Use the existing MoveMethodFileService to move a static method
            var result = await MoveMethodFileService.MoveStaticMethodInFile(
                sourceFilePath,
                methodName,
                targetClassName,
                cancellationToken: cancellationToken);

            return new ExxerFactoringResult(
                Success: true,
                Message: result,
                ModifiedFiles: new[] { sourceFilePath }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving method {MethodName}", methodName);
            return new ExxerFactoringResult(
                Success: false,
                Message: "Move method failed",
                ErrorDetails: ex.ToString()
            );
        }
    }

    /// <inheritdoc />
    /// <inheritdoc />
    public async Task<ExxerFactoringResult> IntroduceVariableAsync(string solutionPath, string filePath, int line, int column, string variableName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Introducing variable {VariableName} at {FilePath}:{Line}:{Column}",
            variableName, filePath, line, column);

        try
        {
            // Convert line and column to selection range format expected by IntroduceVariableTool
            var selectionRange = $"{line}:{column}-{line}:{column}";
            
            // Use the existing IntroduceVariableTool
            var result = await IntroduceVariableTool.IntroduceVariable(
                solutionPath,
                filePath,
                selectionRange,
                variableName,
                cancellationToken);

            return new ExxerFactoringResult(
                Success: true,
                Message: result,
                ModifiedFiles: new[] { filePath }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error introducing variable {VariableName}", variableName);
            return new ExxerFactoringResult(
                Success: false,
                Message: "Introduce variable failed",
                ErrorDetails: ex.ToString()
            );
        }
    }

    /// <inheritdoc />
    /// <inheritdoc />
    public async Task<string> GetMetricsAsync(string solutionPath, string path, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting metrics for path: {Path} in solution: {SolutionPath}", path, solutionPath);

        try
        {
            // Use the existing MetricsProvider to get metrics
            var metrics = await MetricsProvider.GetFileMetrics(solutionPath, path, cancellationToken);
            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metrics for {Path}", path);
            return JsonSerializer.Serialize(new { error = ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> ListAvailableToolsAsync()
    {
        await Task.Delay(10);

        // TODO: Implement tool discovery from internal tools
        return new[]
        {
            "extract-method",
            "move-method",
            "introduce-variable",
            "introduce-field",
            "introduce-parameter",
            "convert-to-static",
            "make-field-readonly",
            "inline-method",
            "safe-delete",
            "cleanup-usings",
            "rename-symbol"
        };
    }
}
