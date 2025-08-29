using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Mcp.Core.SyntaxRewriters;

namespace IndFusion.Mcp.Mcp.Core.Tools;

/// <summary>
/// Converts property setters to init-only setters where possible.
/// </summary>
[McpServerToolType]
public static class TransformSetterToInitTool
{
    /// <summary>
    /// Converts a property's setter to an init accessor.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file.</param>
    /// <param name="propertyName">Name of the property to transform.</param>
    /// <returns>Status message for the operation.</returns>
    [McpServerTool, Description("Convert property setter to init-only setter (preferred for large C# file ExxerFactoring)")]
    public static async Task<string> TransformSetterToInit(
        [Description("Absolute path to the solution file (.sln)")] string solutionPath,
        [Description("Path to the C# file")] string filePath,
        [Description("Name of the property to transform")] string propertyName)
    {
        try
        {
            return await ExxerFactoringHelpers.RunWithSolutionOrFile(
                solutionPath,
                filePath,
                doc => TransformSetterToInitWithSolution(doc, propertyName),
                path => TransformSetterToInitSingleFile(path, propertyName));
        }
        catch (Exception ex)
        {
            throw new McpException($"Error transforming setter: {ex.Message}", ex);
        }
    }

    private static async Task<string> TransformSetterToInitWithSolution(Document document, string propertyName)
    {
        var syntaxRoot = await document.GetSyntaxRootAsync();

        var property = syntaxRoot!.DescendantNodes()
            .OfType<PropertyDeclarationSyntax>()
            .FirstOrDefault(p => p.Identifier.ValueText == propertyName);
        if (property == null)
            throw new McpException($"Error: No property named '{propertyName}' found");

        var setter = property.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.SetAccessorDeclaration));
        if (setter == null)
            throw new McpException($"Error: Property '{propertyName}' has no setter");

        var rewriter = new SetterToInitRewriter(propertyName);
        var newRoot = rewriter.Visit(syntaxRoot);
        var formatted = Formatter.Format(newRoot!, document.Project.Solution.Workspace);
        var newDocument = document.WithSyntaxRoot(formatted);
        var newText = await newDocument.GetTextAsync();
        var encoding = await ExxerFactoringHelpers.GetFileEncodingAsync(document.FilePath!);
        await File.WriteAllTextAsync(document.FilePath!, newText.ToString(), encoding);
        ExxerFactoringHelpers.UpdateSolutionCache(newDocument);

        return $"Successfully converted setter to init for '{propertyName}' in {document.FilePath} (solution mode)";
    }

    private static Task<string> TransformSetterToInitSingleFile(string filePath, string propertyName)
    {
        return ExxerFactoringHelpers.ApplySingleFileEdit(
            filePath,
            text => TransformSetterToInitInSource(text, propertyName),
            $"Successfully converted setter to init for '{propertyName}' in {filePath} (single file mode)");
    }

    /// <summary>
    /// Converts a property's setter to an init accessor within the provided source text.
    /// </summary>
    /// <param name="sourceText">The C# source text.</param>
    /// <param name="propertyName">Name of the property to transform.</param>
    /// <returns>Updated source text.</returns>
    public static string TransformSetterToInitInSource(string sourceText, string propertyName)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
        var syntaxRoot = syntaxTree.GetRoot();

        var property = syntaxRoot.DescendantNodes()
            .OfType<PropertyDeclarationSyntax>()
            .FirstOrDefault(p => p.Identifier.ValueText == propertyName);
        if (property == null)
            throw new McpException($"Error: No property named '{propertyName}' found");

        var setter = property.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.SetAccessorDeclaration));
        if (setter == null)
            throw new McpException($"Error: Property '{propertyName}' has no setter");

        var rewriter = new SetterToInitRewriter(propertyName);
        var newRoot = rewriter.Visit(syntaxRoot);
        var formatted = Formatter.Format(newRoot!, ExxerFactoringHelpers.SharedWorkspace);
        return formatted.ToFullString();
    }
}
