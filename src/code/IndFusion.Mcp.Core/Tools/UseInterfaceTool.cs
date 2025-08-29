using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace IndFusion.Mcp.Mcp.Core.Tools;

/// <summary>
/// Refactoring tool to change a parameter's concrete type to an interface.
/// Supports both solution-aware and single-file modes.
/// </summary>
[McpServerToolType]
public static class UseInterfaceTool
{
    private static async Task<string> UseInterfaceWithSolution(Document document, string methodName, string parameterName, string interfaceName)
    {
        var root = await document.GetSyntaxRootAsync();
        var method = root!.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.ValueText == methodName);
        if (method == null)
            return $"Error: No method named '{methodName}' found";
        var param = method.ParameterList.Parameters
            .FirstOrDefault(p => p.Identifier.ValueText == parameterName);
        if (param == null)
            return $"Error: No parameter named '{parameterName}' found";

        var newParam = param.WithType(SyntaxFactory.ParseTypeName(interfaceName));
        var newMethod = method.ReplaceNode(param, newParam);
        var newRoot = root.ReplaceNode(method, newMethod);
        var formatted = Formatter.Format(newRoot, document.Project.Solution.Workspace);
        var newDoc = document.WithSyntaxRoot(formatted);
        var text = await newDoc.GetTextAsync();
        var encoding = await ExxerFactoringHelpers.GetFileEncodingAsync(document.FilePath!);
        await File.WriteAllTextAsync(document.FilePath!, text.ToString(), encoding);
        ExxerFactoringHelpers.UpdateSolutionCache(newDoc);
        return $"Successfully changed parameter '{parameterName}' to interface '{interfaceName}' in method '{methodName}' in {document.FilePath} (solution mode)";
    }

    private static Task<string> UseInterfaceSingleFile(string filePath, string methodName, string parameterName, string interfaceName)
    {
        return ExxerFactoringHelpers.ApplySingleFileEdit(
            filePath,
            text => UseInterfaceInSource(text, methodName, parameterName, interfaceName),
            $"Successfully changed parameter '{parameterName}' to interface '{interfaceName}' in method '{methodName}' in {filePath} (single file mode)");
    }

    /// <summary>
    /// Changes a method parameter type to the specified interface within the provided source text.
    /// </summary>
    /// <param name="sourceText">The C# source text.</param>
    /// <param name="methodName">Name of the method containing the parameter.</param>
    /// <param name="parameterName">Name of the parameter to change.</param>
    /// <param name="interfaceName">Fully qualified interface type name.</param>
    /// <returns>The updated source text.</returns>
    public static string UseInterfaceInSource(string sourceText, string methodName, string parameterName, string interfaceName)
    {
        var tree = CSharpSyntaxTree.ParseText(sourceText);
        var root = tree.GetRoot();
        var method = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.ValueText == methodName);
        if (method == null)
            return $"Error: No method named '{methodName}' found";
        var param = method.ParameterList.Parameters
            .FirstOrDefault(p => p.Identifier.ValueText == parameterName);
        if (param == null)
            return $"Error: No parameter named '{parameterName}' found";

        var newParam = param.WithType(SyntaxFactory.ParseTypeName(interfaceName));
        var newMethod = method.ReplaceNode(param, newParam);
        var newRoot = root.ReplaceNode(method, newMethod);
        var formatted = Formatter.Format(newRoot, ExxerFactoringHelpers.SharedWorkspace);
        return formatted.ToFullString();
    }

    /// <summary>
    /// Changes a method parameter type to the specified interface.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file.</param>
    /// <param name="methodName">Name of the method containing the parameter.</param>
    /// <param name="parameterName">Name of the parameter to change.</param>
    /// <param name="interfaceName">Fully qualified interface type name.</param>
    /// <returns>Status message for the operation.</returns>
    [McpServerTool, Description("Change a method parameter type to an interface (preferred for large C# file ExxerFactoring)")]
    public static async Task<string> UseInterface(
        [Description("Absolute path to the solution file (.sln)")] string solutionPath,
        [Description("Path to the C# file")] string filePath,
        [Description("Name of the method containing the parameter")] string methodName,
        [Description("Name of the parameter to change")] string parameterName,
        [Description("Interface type name to use")] string interfaceName)
    {
        try
        {
            return await ExxerFactoringHelpers.RunWithSolutionOrFile(
                solutionPath,
                filePath,
                doc => UseInterfaceWithSolution(doc, methodName, parameterName, interfaceName),
                path => UseInterfaceSingleFile(path, methodName, parameterName, interfaceName));
        }
        catch (Exception ex)
        {
            throw new McpException($"Error applying interface: {ex.Message}", ex);
        }
    }
}
