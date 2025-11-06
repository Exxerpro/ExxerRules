namespace IndFusion.Tools.Cli.Core.Services;

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