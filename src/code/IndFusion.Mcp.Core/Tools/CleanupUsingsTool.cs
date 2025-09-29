using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using CoreMcpException = IndFusion.Mcp.Core.Exceptions.McpException;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Removes unused using directives from C# files.
/// Supports solution-aware and single-file operation modes.
/// </summary>
[McpServerToolType]
public static class CleanupUsingsTool
{
    /// <summary>
    /// Removes unused using directives from a file.
    /// </summary>
    /// <param name="solutionPath">Optional absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status message describing the result.</returns>
    [McpServerTool, Description("Remove unused using directives from a C# file (preferred for large C# file ExxerFactoring)")]
    public static async Task<string> CleanupUsings(
        [Description("Absolute path to the solution file (.sln)")] string? solutionPath,
        [Description("Path to the C# file")] string filePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(solutionPath))
            {
                var solution = await ExxerFactoringHelpers.GetOrLoadSolution(solutionPath);
                var document = ExxerFactoringHelpers.GetDocumentByPath(solution, filePath);
                if (document != null)
                    return await CleanupUsingsWithSolution(document);
            }

            return await CleanupUsingsSingleFile(filePath);
        }        catch (CoreMcpException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new McpException($"Error cleaning up usings: {ex.Message}", ex);
        }
    }

    private static async Task<string> CleanupUsingsWithSolution(Document document)
    {
        var root = await document.GetSyntaxRootAsync();
        if (root == null)
            return $"No content in {document.FilePath}";

        var compilation = await document.Project.GetCompilationAsync();
        if (compilation == null)
            return $"Could not compile project for {document.FilePath}";

        var diagnostics = compilation.GetDiagnostics();
        var unused = diagnostics
            .Where(d => d.Id == "CS8019" && d.Location.IsInSource && d.Location.SourceTree == root.SyntaxTree)
            .Select(d => {
                try { return root.FindNode(d.Location.SourceSpan, getInnermostNodeForTie: true); }
                catch { return null; }
            })
            .OfType<UsingDirectiveSyntax>()
            .ToList();

        var newRoot = root!.RemoveNodes(unused, SyntaxRemoveOptions.KeepNoTrivia);
        if (newRoot == null)
            return $"Could not remove unused usings from {document.FilePath}";
        var formatted = Formatter.Format(newRoot, ExxerFactoringHelpers.SharedWorkspace);
        var encoding = await ExxerFactoringHelpers.GetFileEncodingAsync(document.FilePath!);
        var (originalText, _) = await ExxerFactoringHelpers.ReadFileWithEncodingAsync(document.FilePath!);
        var formattedText = formatted.ToFullString();
        // Preserve original newline style
        var normalized = formattedText.Replace("\r\n", "\n");
        if (originalText.Contains("\r\n")) normalized = normalized.Replace("\n", "\r\n");
        await File.WriteAllTextAsync(document.FilePath!, normalized, encoding);

        var newDocument = document.WithSyntaxRoot(formatted);
        ExxerFactoringHelpers.UpdateSolutionCache(newDocument);
        return $"Removed unused usings in {document.FilePath}";
    }

    private static Task<string> CleanupUsingsSingleFile(string filePath)
    {
        return ExxerFactoringHelpers.ApplySingleFileEdit(
            filePath,
            CleanupUsingsInSource,
            $"Removed unused usings in {filePath} (single file mode)");
    }

    /// <summary>
    /// Removes unused using directives from the provided source text.
    /// </summary>
    /// <param name="sourceText">The C# source text.</param>
    /// <returns>Formatted source text without unused usings.</returns>
    public static string CleanupUsingsInSource(string sourceText)
    {
        var tree = CSharpSyntaxTree.ParseText(sourceText);
        // Build compilation with full trusted platform assemblies to correctly resolve usages
        var tpa = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))?.Split(Path.PathSeparator) ?? Array.Empty<string>();
        var refs = tpa.Select(p => MetadataReference.CreateFromFile(p));
        var compilation = CSharpCompilation.Create("Cleanup", new[] { tree }, refs);
        var diagnostics = compilation.GetDiagnostics();
        var root = tree.GetRoot();
        var unused = diagnostics
            .Where(d => d.Id == "CS8019")
            .Select(d => root.FindNode(d.Location.SourceSpan))
            .OfType<UsingDirectiveSyntax>()
            .ToList();

        var newRoot = root.RemoveNodes(unused, SyntaxRemoveOptions.KeepNoTrivia);
        if (newRoot == null)
            return sourceText; // Return original if we can't remove nodes
        var formatted = Formatter.Format(newRoot, ExxerFactoringHelpers.SharedWorkspace);
        // Normalize to LF-only to make tests platform-independent
        return formatted.ToFullString().Replace("\r\n", "\n");
    }
}
