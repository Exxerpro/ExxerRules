using Microsoft.Extensions.Logging;
using IndFusion.Tools.Cli.Models;

namespace IndFusion.Tools.Cli.Services;

/// <summary>
/// Orchestrates refactoring operations by coordinating between tools and services
/// </summary>
public class RefactoringOrchestrator
{
    private readonly ILogger _logger;
    private readonly RefactoringService _refactoringService;
    private readonly ToolRegistry _toolRegistry;

    /// <summary>
    /// Initializes a new instance of the RefactoringOrchestrator class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    public RefactoringOrchestrator(ILogger logger)
    {
        _logger = logger;
        _refactoringService = new RefactoringService(logger);
        _toolRegistry = new ToolRegistry();
    }

    /// <summary>
    /// Executes a refactoring request
    /// </summary>
    /// <param name="request">The refactoring request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refactoring result</returns>
    public async Task<RefactoringResult> ExecuteAsync(RefactoringRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Executing refactoring request: {ToolName}", request.ToolName);

            // Validate tool exists
            if (!_toolRegistry.IsToolAvailable(request.ToolName))
            {
                var availableTools = string.Join(", ", _toolRegistry.GetAvailableTools());
                return RefactoringResult.Failure($"Tool '{request.ToolName}' not found. Available tools: {availableTools}");
            }

            // Get tool information
            var toolInfo = _toolRegistry.GetToolInfo(request.ToolName);
            if (toolInfo == null)
            {
                return RefactoringResult.Failure($"Could not retrieve information for tool '{request.ToolName}'");
            }

            // Validate request parameters
            var validationResult = ValidateRequest(request, toolInfo);
            if (!validationResult.IsValid)
            {
                return RefactoringResult.Failure($"Invalid request parameters: {validationResult.ErrorMessage}");
            }

            // Execute the refactoring
            var result = await _refactoringService.ExecuteRefactoringAsync(request, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Refactoring completed successfully: {ToolName}", request.ToolName);
                return RefactoringResult.Success(result.Summary, result.Preview);
            }
            else
            {
                _logger.LogError("Refactoring failed: {ToolName} - {Error}", request.ToolName, result.ErrorMessage);
                return RefactoringResult.Failure(result.ErrorMessage ?? "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during refactoring execution");
            return RefactoringResult.Failure($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates a refactoring request against tool requirements
    /// </summary>
    /// <param name="request">The request to validate</param>
    /// <param name="toolInfo">The tool information</param>
    /// <returns>Validation result</returns>
    private static ValidationResult ValidateRequest(RefactoringRequest request, ToolInfo toolInfo)
    {
        // Check required parameters based on tool type
        switch (toolInfo.Type.ToLowerInvariant())
        {
            case "extractmethod":
                if (string.IsNullOrEmpty(request.Range) && (request.Line == null || request.Column == null))
                {
                    return ValidationResult.Invalid("ExtractMethod requires either --range or --line and --column parameters");
                }
                if (string.IsNullOrEmpty(request.NewName))
                {
                    return ValidationResult.Invalid("ExtractMethod requires --name parameter for the new method name");
                }
                break;

            case "renamemethod":
                if (string.IsNullOrEmpty(request.MethodName))
                {
                    return ValidationResult.Invalid("RenameMethod requires --method parameter");
                }
                if (string.IsNullOrEmpty(request.NewName))
                {
                    return ValidationResult.Invalid("RenameMethod requires --name parameter for the new method name");
                }
                break;

            case "movemethod":
                if (string.IsNullOrEmpty(request.MethodName))
                {
                    return ValidationResult.Invalid("MoveMethod requires --method parameter");
                }
                if (string.IsNullOrEmpty(request.TargetLocation))
                {
                    return ValidationResult.Invalid("MoveMethod requires --target parameter");
                }
                break;

            case "inlinevariable":
                if (string.IsNullOrEmpty(request.Range) && (request.Line == null || request.Column == null))
                {
                    return ValidationResult.Invalid("InlineVariable requires either --range or --line and --column parameters");
                }
                break;
        }

        return ValidationResult.Valid();
    }
}