namespace IndFusion.Tools.Mcp.App.Tools;

[McpServerToolType]
/// <summary>
/// Provides a refactoring that introduces a simple observer event on a class and
/// raises it from a specified method.
/// </summary>
public static class AddObserverTool
{
    [McpServerTool, Description("Introduce a simple observer event and raise it in a method")]
    /// <summary>
    /// Adds an event to a class and invokes it from the specified method, updating files accordingly.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file to modify.</param>
    /// <param name="className">Name of the class containing the method.</param>
    /// <param name="methodName">Name of the method that should raise the event.</param>
    /// <param name="eventName">Name of the event to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A status message describing the observer addition.</returns>
    public static async Task<string> AddObserver(
        [Description("Absolute path to the solution file (.sln)")] string solutionPath,
        [Description("Path to the C# file")] string filePath,
        [Description("Name of the class containing the method")] string className,
        [Description("Name of the method to raise the event from")] string methodName,
        [Description("Name of the event to create")] string eventName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await ExxerFactoringHelpers.RunWithSolutionOrFile(
                solutionPath,
                filePath,
                doc => AddObserverWithSolution(doc, className, methodName, eventName),
                path => AddObserverSingleFile(path, className, methodName, eventName));
        }
        catch (Exception ex)
        {
            throw new McpException($"Error adding observer: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Applies the observer transformation within a solution-aware context.
    /// </summary>
    /// <param name="document">Document to update.</param>
    /// <param name="className">Target class name.</param>
    /// <param name="methodName">Method to raise the event.</param>
    /// <param name="eventName">Event name to introduce.</param>
    /// <returns>Status message.</returns>
    private static async Task<string> AddObserverWithSolution(Document document, string className, string methodName, string eventName)
    {
        var text = await document.GetTextAsync();
        var newText = AddObserverInSource(text.ToString(), className, methodName, eventName);
        var enc = await ExxerFactoringHelpers.GetFileEncodingAsync(document.FilePath!);
        await File.WriteAllTextAsync(document.FilePath!, newText, enc);
        ExxerFactoringHelpers.UpdateSolutionCache(document.WithText(SourceText.From(newText, enc)));
        return $"Added observer {eventName} to {document.FilePath} (solution mode)";
    }

    /// <summary>
    /// Applies the observer transformation to a single file path.
    /// </summary>
    /// <param name="filePath">File to update.</param>
    /// <param name="className">Target class name.</param>
    /// <param name="methodName">Method to raise the event.</param>
    /// <param name="eventName">Event name to introduce.</param>
    /// <returns>A task that resolves to a status message.</returns>
    private static Task<string> AddObserverSingleFile(string filePath, string className, string methodName, string eventName)
    {
        return ExxerFactoringHelpers.ApplySingleFileEdit(
            filePath,
            text => AddObserverInSource(text, className, methodName, eventName),
            $"Added observer {eventName} to {filePath} (single file mode)");
    }

    /// <summary>
    /// Performs the observer introduction directly against source text and returns the updated source.
    /// </summary>
    /// <param name="sourceText">Original source code.</param>
    /// <param name="className">Target class name.</param>
    /// <param name="methodName">Method to raise the event.</param>
    /// <param name="eventName">Event name to introduce.</param>
    /// <returns>Updated source code.</returns>
    public static string AddObserverInSource(string sourceText, string className, string methodName, string eventName)
    {
        var tree = CSharpSyntaxTree.ParseText(sourceText);
        var root = (CompilationUnitSyntax)tree.GetRoot();
        var classNode = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
            .FirstOrDefault(c => c.Identifier.ValueText == className);
        if (classNode == null)
            throw new McpException($"Error: Class '{className}' not found");
        var method = classNode.Members.OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.ValueText == methodName);
        if (method == null)
            throw new McpException($"Error: Method '{methodName}' not found");

        var param = method.ParameterList.Parameters.FirstOrDefault();
        var eventType = param != null ? $"Action<{param.Type}>" : "Action";
        var eventField = SyntaxFactory.EventFieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName(eventType))
                    .AddVariables(SyntaxFactory.VariableDeclarator(eventName)))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

        var invocationArgs = param != null ? param.Identifier.ValueText : string.Empty;
        var invokeStmt = SyntaxFactory.ParseStatement($"{eventName}?.Invoke({invocationArgs});");

        var newMethod = method.WithBody(method.Body!.AddStatements(invokeStmt));
        var newClass = classNode.ReplaceNode(method, newMethod).AddMembers(eventField);

        var newRoot = root.ReplaceNode(classNode, newClass);
        var formatted = Formatter.Format(newRoot, ExxerFactoringHelpers.SharedWorkspace);
        return formatted.ToFullString();
    }
}
