using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Core.SyntaxRewriters;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Converts an instance method to static by adding an explicit instance parameter.
/// Supports solution-aware and single-file modes.
/// </summary>
[McpServerToolType]
public static class ConvertToStaticWithInstanceTool
{
    private static SyntaxNode ConvertToStaticWithInstanceAst(
        SyntaxNode root,
        MethodDeclarationSyntax method,
        string instanceParameterName,
        SemanticModel? semanticModel = null)
    {
        var classDecl = method.Ancestors().OfType<ClassDeclarationSyntax>().First();

        var typeName = semanticModel != null
            ? ((INamedTypeSymbol)semanticModel.GetDeclaredSymbol(classDecl)!).ToDisplayString()
            : classDecl.Identifier.ValueText;

        HashSet<string>? members = null;
        INamedTypeSymbol? typeSymbol = null;
        if (semanticModel != null)
        {
            typeSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(classDecl)!;
        }
        else
        {
            members = classDecl.Members
                .Where(m => m is FieldDeclarationSyntax or PropertyDeclarationSyntax or MethodDeclarationSyntax)
                .Select(m => m switch
                {
                    FieldDeclarationSyntax f => f.Declaration.Variables.First().Identifier.ValueText,
                    PropertyDeclarationSyntax p => p.Identifier.ValueText,
                    MethodDeclarationSyntax md => md.Identifier.ValueText,
                    _ => string.Empty
                })
                .Where(n => !string.IsNullOrEmpty(n))
                .ToHashSet();
        }

        var rewriter = new StaticConversionRewriter(
            new[] { (instanceParameterName, typeName) },
            instanceParameterName,
            members,
            semanticModel,
            typeSymbol);

        var updatedMethod = rewriter.Rewrite(method);
        return root.ReplaceNode(method, updatedMethod);
    }
    /// <summary>
    /// Converts an instance method to static by adding an instance parameter.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file.</param>
    /// <param name="methodName">Name of the method to convert.</param>
    /// <param name="instanceParameterName">Name for the instance parameter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status message for the operation.</returns>
    [McpServerTool, Description("Transform instance method to static by adding instance parameter (preferred for large C# file ExxerFactoring)")]
    public static async Task<string> ConvertToStaticWithInstance(
        [Description("Absolute path to the solution file (.sln)")] string solutionPath,
        [Description("Path to the C# file")] string filePath,
        [Description("Name of the method to convert")] string methodName,
        [Description("Name for the instance parameter")] string instanceParameterName = "instance",
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await ExxerFactoringHelpers.RunWithSolutionOrFile(
                solutionPath,
                filePath,
                doc => ConvertToStaticWithInstanceWithSolution(doc, methodName, instanceParameterName),
                path => ConvertToStaticWithInstanceSingleFile(path, methodName, instanceParameterName));
        }
        catch (Exception ex)
        {
            throw new McpException($"Error converting method to static: {ex.Message}", ex);
        }
    }


    private static async Task<string> ConvertToStaticWithInstanceWithSolution(Document document, string methodName, string instanceParameterName)
    {
        var sourceText = await document.GetTextAsync();
        var syntaxRoot = await document.GetSyntaxRootAsync();

        var method = syntaxRoot!.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.ValueText == methodName);
        if (method == null)
            return $"Error: No method named '{methodName}' found";

        var semanticModel = await document.GetSemanticModelAsync();
        var newRoot = ConvertToStaticWithInstanceAst(syntaxRoot!, method, instanceParameterName, semanticModel);
        var formatted = Formatter.Format(newRoot, document.Project.Solution.Workspace);
        var newDocument = document.WithSyntaxRoot(formatted);
        var newText = await newDocument.GetTextAsync();
        var encoding = await ExxerFactoringHelpers.GetFileEncodingAsync(document.FilePath!);
        await File.WriteAllTextAsync(document.FilePath!, newText.ToString(), encoding);
        ExxerFactoringHelpers.UpdateSolutionCache(newDocument);

        return $"Successfully converted method '{methodName}' to static with instance parameter in {document.FilePath} (solution mode)";
    }

    private static Task<string> ConvertToStaticWithInstanceSingleFile(string filePath, string methodName, string instanceParameterName)
    {
        return ExxerFactoringHelpers.ApplySingleFileEdit(
            filePath,
            text => ConvertToStaticWithInstanceInSource(text, methodName, instanceParameterName),
            $"Successfully converted method '{methodName}' to static with instance parameter in {filePath} (single file mode)");
    }

    /// <summary>
    /// Converts an instance method to static by adding an instance parameter within the provided source text.
    /// </summary>
    /// <param name="sourceText">The C# source text.</param>
    /// <param name="methodName">Name of the method to convert.</param>
    /// <param name="instanceParameterName">Name to use for the injected instance parameter.</param>
    /// <returns>Updated source text with the static method.</returns>
    public static string ConvertToStaticWithInstanceInSource(string sourceText, string methodName, string instanceParameterName)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
        var syntaxRoot = syntaxTree.GetRoot();

        var method = syntaxRoot.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m => m.Identifier.ValueText == methodName);
        if (method == null)
            return $"Error: No method named '{methodName}' found";

        var classDecl = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (classDecl == null)
            throw new McpException($"Error: Method '{methodName}' is not inside a class");

        var newRoot = ConvertToStaticWithInstanceAst(syntaxRoot, method, instanceParameterName);
        var formatted = Formatter.Format(newRoot, ExxerFactoringHelpers.SharedWorkspace);
        return formatted.ToFullString().Replace("\r\n", "\n");
    }
}
