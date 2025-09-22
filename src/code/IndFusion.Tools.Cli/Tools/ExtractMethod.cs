namespace IndFusion.Tools.Mcp.App.Tools;

[McpServerToolType]
/// <summary>
/// Provides operations to extract a selected block of code into a new method
/// either in a Roslyn solution context or in a single file.
/// </summary>
public static class ExtractMethodTool
{
    [McpServerTool, Description("Extract a code block into a new method (preferred for large C# file ExxerFactoring)")]
    /// <summary>
    /// Extracts a selected code range into a new method and writes the updated file(s).
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file to modify.</param>
    /// <param name="selectionRange">Range in format 'startLine:startColumn-endLine:endColumn'.</param>
    /// <param name="methodName">Name for the new method.</param>
    /// <returns>A status message describing the performed extraction.</returns>
    public static async Task<string> ExtractMethod(
        [Description("Absolute path to the solution file (.sln)")] string solutionPath,
        [Description("Path to the C# file")] string filePath,
        [Description("Range in format 'startLine:startColumn-endLine:endColumn'")] string selectionRange,
        [Description("Name for the new method")] string methodName)
    {
        try
        {
            return await ExxerFactoringHelpers.RunWithSolutionOrFile(
                solutionPath,
                filePath,
                doc => ExtractMethodWithSolution(doc, selectionRange, methodName),
                path => ExtractMethodSingleFile(path, selectionRange, methodName));
        }
        catch (Exception ex)
        {
            throw new McpException($"Error extracting method: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Performs the extract method operation within a solution context.
    /// </summary>
    /// <param name="document">The Roslyn document to update.</param>
    /// <param name="selectionRange">The selected range to extract.</param>
    /// <param name="methodName">The name for the new method.</param>
    /// <returns>A status message describing the changes.</returns>
    private static async Task<string> ExtractMethodWithSolution(Document document, string selectionRange, string methodName)
    {
        var sourceText = await document.GetTextAsync();
        var syntaxRoot = await document.GetSyntaxRootAsync();

        if (!ExxerFactoringHelpers.TryParseRange(selectionRange, out var startLine, out var startColumn, out var endLine, out var endColumn))
            throw new McpException("Error: Invalid selection range format. Use 'startLine:startColumn-endLine:endColumn'");

        if (!ExxerFactoringHelpers.ValidateRange(sourceText, startLine, startColumn, endLine, endColumn, out var error))
            throw new McpException(error);

        var startPosition = sourceText.Lines[startLine - 1].Start + startColumn - 1;
        var endPosition = sourceText.Lines[endLine - 1].Start + endColumn - 1;
        var span = TextSpan.FromBounds(startPosition, endPosition);

        var selectedNodes = syntaxRoot!.DescendantNodes()
            .Where(n => span.Contains(n.Span))
            .ToList();

        if (!selectedNodes.Any())
            throw new McpException("Error: No valid code selected");

        var containingMethod = selectedNodes.First().Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (containingMethod == null)
            throw new McpException("Error: Selected code is not within a method");

        var statementsToExtract = containingMethod.Body!.Statements
            .Where(s => span.IntersectsWith(s.FullSpan))
            .ToList();

        if (!statementsToExtract.Any())
            throw new McpException("Error: Selected code does not contain extractable statements");

        var containingClass = containingMethod.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        var rewriter = new ExtractMethodRewriter(containingMethod, containingClass, statementsToExtract, methodName);
        var newRoot = rewriter.Visit(syntaxRoot);

        var formattedRoot = Formatter.Format(newRoot!, document.Project.Solution.Workspace);
        var newDocument = document.WithSyntaxRoot(formattedRoot);

        // Write the changes back to the file
        var newText = await newDocument.GetTextAsync();
        var encoding = await ExxerFactoringHelpers.GetFileEncodingAsync(document.FilePath!);
        await File.WriteAllTextAsync(document.FilePath!, newText.ToString(), encoding);
        ExxerFactoringHelpers.UpdateSolutionCache(newDocument);

        return $"Successfully extracted method '{methodName}' from {selectionRange} in {document.FilePath} (solution mode)";
    }

    /// <summary>
    /// Performs the extract method operation against a single file path.
    /// </summary>
    /// <param name="filePath">File to update.</param>
    /// <param name="selectionRange">The selected range to extract.</param>
    /// <param name="methodName">The name for the new method.</param>
    /// <returns>A task that resolves to a status message.</returns>
    private static Task<string> ExtractMethodSingleFile(string filePath, string selectionRange, string methodName)
    {
        return ExxerFactoringHelpers.ApplySingleFileEdit(
            filePath,
            text => ExtractMethodInSource(text, selectionRange, methodName),
            $"Successfully extracted method '{methodName}' from {selectionRange} in {filePath} (single file mode)");
    }

    /// <summary>
    /// Applies extract method transformation to the provided source text and returns the new source.
    /// </summary>
    /// <param name="sourceText">Original C# source code.</param>
    /// <param name="selectionRange">Selected range to extract.</param>
    /// <param name="methodName">Name for the new method.</param>
    /// <returns>The updated source code with the new method extracted.</returns>
    public static string ExtractMethodInSource(string sourceText, string selectionRange, string methodName)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
        var syntaxRoot = syntaxTree.GetRoot();
        var text = SourceText.From(sourceText);
        var textLines = text.Lines;

        if (!ExxerFactoringHelpers.TryParseRange(selectionRange, out var startLine, out var startColumn, out var endLine, out var endColumn))
            throw new McpException("Error: Invalid selection range format. Use 'startLine:startColumn-endLine:endColumn'");

        if (!ExxerFactoringHelpers.ValidateRange(text, startLine, startColumn, endLine, endColumn, out var error))
            throw new McpException(error);

        var startPosition = textLines[startLine - 1].Start + startColumn - 1;
        var endPosition = textLines[endLine - 1].Start + endColumn - 1;
        var span = TextSpan.FromBounds(startPosition, endPosition);

        var selectedNodes = syntaxRoot.DescendantNodes()
            .Where(n => span.Contains(n.Span))
            .ToList();

        if (!selectedNodes.Any())
            throw new McpException("Error: No valid code selected");

        var containingMethod = selectedNodes.First().Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (containingMethod == null)
            throw new McpException("Error: Selected code is not within a method");

        var statementsToExtract = containingMethod.Body!.Statements
            .Where(s => span.IntersectsWith(s.FullSpan))
            .ToList();

        if (!statementsToExtract.Any())
            throw new McpException("Error: Selected code does not contain extractable statements");

        var containingClass = containingMethod.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        var rewriter = new ExtractMethodRewriter(containingMethod, containingClass, statementsToExtract, methodName);
        var newRoot = rewriter.Visit(syntaxRoot);

        var formattedRoot = Formatter.Format(newRoot, ExxerFactoringHelpers.SharedWorkspace);
        return formattedRoot.ToFullString();
    }
}
