using IndFusion.Tools.Cli.SyntaxRewriters;

namespace IndFusion.Tools.Cli.Tools;

/// <summary>
/// Converts selected method parameters into injected constructor dependencies.
/// </summary>
[McpServerToolType]
public static class ConstructorInjectionTool
{
    /// <summary>
    /// Represents a method/parameter pair to inject.
    /// </summary>
    /// <param name="MethodName">The method name containing the parameter.</param>
    /// <param name="ParameterName">The parameter to inject via constructor.</param>
    public readonly record struct MethodParameterPair(string MethodName, string ParameterName);

    /// <summary>
    /// Converts specified method parameters to constructor-injected members.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file.</param>
    /// <param name="methodParameters">Pairs of method and parameter to inject.</param>
    /// <param name="useProperty">Use public property instead of private field.</param>
    /// <returns>Status message describing the operation.</returns>
    [McpServerTool, Description("Convert method parameters to constructor injection (preferred for large C# file ExxerFactoring)")]
    public static async Task<string> ConvertToConstructorInjection(
        [Description("Absolute path to the solution file (.sln)")] string solutionPath,
        [Description("Path to the C# file")] string filePath,
        [Description("Method and parameter pairs in the format Method:Parameter;...")] MethodParameterPair[] methodParameters,
        [Description("Use a public property instead of a private field")] bool useProperty = false)
    {
        try
        {
            return await ExxerFactoringHelpers.RunWithSolutionOrFile(
                solutionPath,
                filePath,
                doc => ConvertWithSolution(doc, methodParameters, useProperty),
                path => ConvertSingleFile(path, methodParameters, useProperty));
        }
        catch (Exception ex)
        {
            throw new McpException($"Error performing constructor injection: {ex.Message}", ex);
        }
    }

    private static async Task<string> ConvertWithSolution(Document document, MethodParameterPair[] methodParameters, bool useProperty)
    {
        var sourceText = (await document.GetTextAsync()).ToString();
        var newText = ConvertInSource(sourceText, methodParameters, useProperty);
        if (newText.StartsWith("Error:"))
            return newText;

        var encoding = await ExxerFactoringHelpers.GetFileEncodingAsync(document.FilePath!);
        await File.WriteAllTextAsync(document.FilePath!, newText, encoding);
        var newDoc = document.WithText(SourceText.From(newText, encoding));
        ExxerFactoringHelpers.UpdateSolutionCache(newDoc);
        return $"Successfully injected parameters via constructor in {document.FilePath} (solution mode)";
    }

    private static async Task<string> ConvertSingleFile(string filePath, MethodParameterPair[] methodParameters, bool useProperty)
    {
        if (!File.Exists(filePath))
            throw new McpException($"Error: File {filePath} not found");
        var (sourceText, encoding) = await ExxerFactoringHelpers.ReadFileWithEncodingAsync(filePath);
        var newText = ConvertInSource(sourceText, methodParameters, useProperty);
        if (newText.StartsWith("Error:"))
            return newText;
        await File.WriteAllTextAsync(filePath, newText, encoding);
        ExxerFactoringHelpers.UpdateFileCaches(filePath, newText);
        return $"Successfully injected parameters via constructor in {filePath} (single file mode)";
    }

    /// <summary>
    /// Applies constructor injection to multiple parameters in source text.
    /// </summary>
    /// <param name="sourceText">Original source text.</param>
    /// <param name="methodParameters">Pairs of method and parameter to inject.</param>
    /// <param name="useProperty">Use public property instead of private field.</param>
    /// <returns>Updated source text or an error message starting with "Error:".</returns>
    public static string ConvertInSource(string sourceText, MethodParameterPair[] methodParameters, bool useProperty)
    {
        var text = sourceText;
        foreach (var pair in methodParameters)
        {
            text = ConvertInSource(text, pair.MethodName, pair.ParameterName, useProperty);
            if (text.StartsWith("Error:"))
                return text;
        }
        return text;
    }

    /// <summary>
    /// Applies constructor injection to a single method/parameter in source text.
    /// </summary>
    /// <param name="sourceText">Original source text.</param>
    /// <param name="methodName">Name of the method containing the parameter.</param>
    /// <param name="parameterName">Name of the parameter to inject.</param>
    /// <param name="useProperty">Use public property instead of private field.</param>
    /// <param name="model">Optional semantic model.</param>
    /// <returns>Updated source text or an error message starting with "Error:".</returns>
    public static string ConvertInSource(string sourceText, string methodName, string parameterName, bool useProperty, SemanticModel? model = null)
    {
        var tree = model?.SyntaxTree ?? CSharpSyntaxTree.ParseText(sourceText);
        var root = tree.GetRoot();
        var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.ValueText == methodName);
        if (method == null)
            return $"Error: Method '{methodName}' not found";
        var parameter = method.ParameterList.Parameters.FirstOrDefault(p => p.Identifier.ValueText == parameterName);
        if (parameter == null)
            return $"Error: Parameter '{parameterName}' not found";
        var index = method.ParameterList.Parameters.IndexOf(parameter);
        var type = parameter.Type ?? SyntaxFactory.ParseTypeName("object");
        var fieldName = useProperty ? char.ToUpper(parameterName[0]) + parameterName.Substring(1) : "_" + parameterName;
        var rewriter = new ConstructorInjectionRewriter(methodName, parameterName, index, type, fieldName, useProperty);
        var newRoot = rewriter.Visit(root)!;
        var formatted = Formatter.Format(newRoot, ExxerFactoringHelpers.SharedWorkspace);
        return formatted.ToFullString();
    }
}
