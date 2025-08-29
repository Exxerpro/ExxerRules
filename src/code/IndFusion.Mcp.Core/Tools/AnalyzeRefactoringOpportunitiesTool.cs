using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Mcp.Core.SyntaxWalkers;

namespace IndFusion.Mcp.Mcp.Core.Tools;

/// <summary>
/// Analyzes a C# file for refactoring opportunities (e.g., long methods, static candidates).
/// </summary>
[McpServerToolType, McpServerPromptType]
public static class AnalyzeExxerFactoringOpportunitiesTool
{
    /// <summary>
    /// Analyzes a file for refactoring suggestions and returns a formatted list.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Human-readable suggestions string.</returns>
    [McpServerPrompt, Description("Analyze a C# file for ExxerFactoring opportunities like long methods or unused code")]
    public static async Task<string> AnalyzeExxerFactoringOpportunities(
        [Description("Absolute path to the solution file (.sln)")] string solutionPath,
        [Description("Path to the C# file")] string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (syntaxTree, model, solution) = await LoadSyntaxTreeAndModel(solutionPath, filePath, cancellationToken);
            var root = await syntaxTree.GetRootAsync(cancellationToken);

            var walker = new ExxerFactoringOpportunityWalker(model, solution);
            walker.Visit(root);
            await walker.PostProcessAsync();
            var suggestions = walker.Suggestions;

            if (suggestions.Count == 0)
                return "No obvious ExxerFactoring opportunities detected";

            return "Suggestions:\n" + string.Join("\n", suggestions);
        }
        catch (Exception ex)
        {
            throw new McpException($"Error analyzing file: {ex.Message}", ex);
        }
    }

    private static async Task<(SyntaxTree tree, SemanticModel? model, Solution? solution)> LoadSyntaxTreeAndModel(string solutionPath, string filePath, CancellationToken cancellationToken)
    {
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(solutionPath, cancellationToken);
        var document = ExxerFactoringHelpers.GetDocumentByPath(solution, filePath);
        if (document != null)
        {
            var tree = await document.GetSyntaxTreeAsync(cancellationToken);
            if (tree == null)
            {
                var (text, _) = await ExxerFactoringHelpers.ReadFileWithEncodingAsync(filePath, cancellationToken);
                tree = CSharpSyntaxTree.ParseText(text);
            }
            var model = await document.GetSemanticModelAsync(cancellationToken);
            return (tree, model, solution);
        }

        var (fileText, _) = await ExxerFactoringHelpers.ReadFileWithEncodingAsync(filePath, cancellationToken);
        var syntaxTree = CSharpSyntaxTree.ParseText(fileText);
        return (syntaxTree, null, null);
    }

}
