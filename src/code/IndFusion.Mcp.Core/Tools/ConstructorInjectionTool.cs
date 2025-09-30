using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Core.SyntaxRewriters;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Converts method parameters to constructor-injected dependencies.
/// Supports solution-aware and single-file modes.
/// </summary>
[McpServerToolType]
public static class ConstructorInjectionTool
{
    /// <summary>
    /// Represents a pair of method and parameter names for constructor injection.
    /// </summary>
    /// <param name="MethodName">The target method name.</param>
    /// <param name="ParameterName">The parameter name to inject.</param>
    public readonly record struct MethodParameterPair(string MethodName, string ParameterName);

    /// <summary>
    /// Converts specified method parameters into constructor-injected dependencies.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file.</param>
    /// <param name="methodParameters">Pairs of method and parameter names to inject.</param>
    /// <param name="useProperty">Whether to use public properties instead of private fields.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Status message for the operation.</returns>
    [McpServerTool, Description("Convert method parameters to constructor injection (preferred for large C# file ExxerFactoring)")]
    public static async Task<string> ConvertToConstructorInjection(
        [Description("Absolute path to the solution file (.sln)")] string? solutionPath,
        [Description("Path to the C# file")] string filePath,
        [Description("Method and parameter pairs in the format Method:Parameter;...")] MethodParameterPair[] methodParameters,
        [Description("Use a public property instead of a private field")] bool useProperty = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(solutionPath))
            {
                return await ExxerFactoringHelpers.RunWithSolutionOrFile(
                    solutionPath,
                    filePath,
                    doc => ConvertWithSolution(doc, methodParameters, useProperty, cancellationToken),
                    path => ConvertSingleFile(path, methodParameters, useProperty, cancellationToken));
            }

            return await ConvertSingleFile(filePath, methodParameters, useProperty, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new McpException($"Error performing constructor injection: {ex.Message}", ex);
        }
    }


    private static async Task<string> ConvertWithSolution(Document document, MethodParameterPair[] methodParameters, bool useProperty, CancellationToken cancellationToken)
    {
        var sourceText = (await document.GetTextAsync(cancellationToken)).ToString();
        var newText = ConvertInSource(sourceText, methodParameters, useProperty);
        if (newText.StartsWith("Error:"))
            return newText;

        var encoding = await ExxerFactoringHelpers.GetFileEncodingAsync(document.FilePath!);
        await File.WriteAllTextAsync(document.FilePath!, newText, encoding, cancellationToken);
        var newDoc = document.WithText(SourceText.From(newText, encoding));
        ExxerFactoringHelpers.UpdateSolutionCache(newDoc);
        return $"Successfully injected parameters via constructor in {document.FilePath} (solution mode)";
    }

    private static async Task<string> ConvertSingleFile(string filePath, MethodParameterPair[] methodParameters, bool useProperty, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
            throw new McpException($"Error: File {filePath} not found");
        var (sourceText, encoding) = await ExxerFactoringHelpers.ReadFileWithEncodingAsync(filePath);
        var newText = ConvertInSource(sourceText, methodParameters, useProperty);
        if (newText.StartsWith("Error:"))
            return newText;
        await File.WriteAllTextAsync(filePath, newText, encoding, cancellationToken);
        ExxerFactoringHelpers.UpdateFileCaches(filePath, newText);
        return $"Successfully injected parameters via constructor in {filePath} (single file mode)";
    }

    /// <summary>
    /// Converts multiple method parameters to constructor-injected dependencies in the source text.
    /// </summary>
    /// <param name="sourceText">The C# source text.</param>
    /// <param name="methodParameters">Pairs of method and parameter names to inject.</param>
    /// <param name="useProperty">Whether to use public properties instead of private fields.</param>
    /// <returns>The updated source text.</returns>
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
    /// Converts a single method parameter to a constructor-injected dependency in the source text.
    /// </summary>
    /// <param name="sourceText">The C# source text.</param>
    /// <param name="methodName">Target method name.</param>
    /// <param name="parameterName">Parameter name to inject.</param>
    /// <param name="useProperty">Whether to use a public property instead of a private field.</param>
    /// <param name="model">Optional semantic model.</param>
    /// <returns>The updated source text.</returns>
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
