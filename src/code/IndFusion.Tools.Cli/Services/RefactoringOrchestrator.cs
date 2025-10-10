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

/// <summary>
/// Represents the result of request validation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets whether the validation passed
    /// </summary>
    public bool IsValid { get; private set; }

    /// <summary>
    /// Gets the error message if validation failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Creates a valid validation result
    /// </summary>
    /// <returns>Valid validation result</returns>
    public static ValidationResult Valid()
    {
        return new ValidationResult { IsValid = true };
    }

    /// <summary>
    /// Creates an invalid validation result
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Invalid validation result</returns>
    public static ValidationResult Invalid(string errorMessage)
    {
        return new ValidationResult { IsValid = false, ErrorMessage = errorMessage };
    }
}

/// <summary>
/// Registry for managing available refactoring tools
/// </summary>
public class ToolRegistry
{
    private readonly Dictionary<string, ToolInfo> _tools;

    /// <summary>
    /// Initializes a new instance of the ToolRegistry class
    /// </summary>
    public ToolRegistry()
    {
        _tools = new Dictionary<string, ToolInfo>(StringComparer.OrdinalIgnoreCase);
        InitializeTools();
    }

    /// <summary>
    /// Checks if a tool is available
    /// </summary>
    /// <param name="toolName">The name of the tool</param>
    /// <returns>True if the tool is available</returns>
    public bool IsToolAvailable(string toolName)
    {
        return _tools.ContainsKey(toolName);
    }

    /// <summary>
    /// Gets information about a tool
    /// </summary>
    /// <param name="toolName">The name of the tool</param>
    /// <returns>Tool information or null if not found</returns>
    public ToolInfo? GetToolInfo(string toolName)
    {
        return _tools.TryGetValue(toolName, out var toolInfo) ? toolInfo : null;
    }

    /// <summary>
    /// Gets all available tool names
    /// </summary>
    /// <returns>List of available tool names</returns>
    public IEnumerable<string> GetAvailableTools()
    {
        return _tools.Keys;
    }

    /// <summary>
    /// Initializes the available tools
    /// </summary>
    private void InitializeTools()
    {
        // Add all available refactoring tools
        _tools["extractmethod"] = new ToolInfo("ExtractMethod", "Extract a code block into a new method");
        _tools["renamemethod"] = new ToolInfo("RenameMethod", "Rename a method and update all references");
        _tools["movemethod"] = new ToolInfo("MoveMethod", "Move a method to a different class");
        _tools["inlinevariable"] = new ToolInfo("InlineVariable", "Inline a variable and replace its usage");
        _tools["extractinterface"] = new ToolInfo("ExtractInterface", "Extract an interface from a class");
        _tools["extractclass"] = new ToolInfo("ExtractClass", "Extract a new class from existing code");
        _tools["introduceparameter"] = new ToolInfo("IntroduceParameter", "Introduce a new parameter to a method");
        _tools["removeparameter"] = new ToolInfo("RemoveParameter", "Remove a parameter from a method");
        _tools["reorderparameters"] = new ToolInfo("ReorderParameters", "Reorder method parameters");
        _tools["changemethodsignature"] = new ToolInfo("ChangeMethodSignature", "Change method signature");
        _tools["pullup"] = new ToolInfo("PullUp", "Pull members up to base class");
        _tools["pushdown"] = new ToolInfo("PushDown", "Push members down to derived class");
        _tools["encapsulatefield"] = new ToolInfo("EncapsulateField", "Encapsulate a field with property");
        _tools["converttoauto"] = new ToolInfo("ConvertToAuto", "Convert property to auto-property");
        _tools["converttoexpression"] = new ToolInfo("ConvertToExpression", "Convert method to expression-bodied member");
        _tools["converttoasync"] = new ToolInfo("ConvertToAsync", "Convert method to async");
        _tools["convertfromasync"] = new ToolInfo("ConvertFromAsync", "Convert async method to synchronous");
        _tools["addnullcheck"] = new ToolInfo("AddNullCheck", "Add null check for parameter");
        _tools["addparametercheck"] = new ToolInfo("AddParameterCheck", "Add parameter validation");
        _tools["removeunusedusing"] = new ToolInfo("RemoveUnusedUsing", "Remove unused using statements");
        _tools["organizeusing"] = new ToolInfo("OrganizeUsing", "Organize and sort using statements");
        _tools["addbraces"] = new ToolInfo("AddBraces", "Add braces to single-line statements");
        _tools["removebraces"] = new ToolInfo("RemoveBraces", "Remove unnecessary braces");
        _tools["converttoforeach"] = new ToolInfo("ConvertToForeach", "Convert for loop to foreach");
        _tools["converttofor"] = new ToolInfo("ConvertToFor", "Convert foreach to for loop");
        _tools["converttoif"] = new ToolInfo("ConvertToIf", "Convert ternary operator to if statement");
        _tools["converttoternary"] = new ToolInfo("ConvertToTernary", "Convert if statement to ternary operator");
        _tools["converttoexpressionbody"] = new ToolInfo("ConvertToExpressionBody", "Convert method to expression body");
        _tools["convertfromexpressionbody"] = new ToolInfo("ConvertFromExpressionBody", "Convert expression body to block");
        _tools["converttoinitializer"] = new ToolInfo("ConvertToInitializer", "Convert to object initializer");
        _tools["converttointerpolated"] = new ToolInfo("ConvertToInterpolated", "Convert string concatenation to interpolation");
        _tools["converttoverbatim"] = new ToolInfo("ConvertToVerbatim", "Convert string to verbatim string");
        _tools["converttointerpolatedverbatim"] = new ToolInfo("ConvertToInterpolatedVerbatim", "Convert to interpolated verbatim string");
        _tools["converttointerpolatedraw"] = new ToolInfo("ConvertToInterpolatedRaw", "Convert to raw interpolated string");
        _tools["converttointerpolatedrawverbatim"] = new ToolInfo("ConvertToInterpolatedRawVerbatim", "Convert to raw interpolated verbatim string");
    }
}

/// <summary>
/// Represents information about a refactoring tool
/// </summary>
public class ToolInfo
{
    /// <summary>
    /// Gets or sets the type of the tool
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the tool
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the ToolInfo class
    /// </summary>
    /// <param name="type">The type of the tool</param>
    /// <param name="description">The description of the tool</param>
    public ToolInfo(string type, string description)
    {
        Type = type;
        Description = description;
    }
}