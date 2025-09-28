namespace IndFusion.Tools.Mcp.App.Tools;

/// <summary>
/// MCP resource that returns a summarized view of a C# source file by
/// omitting method bodies (useful to reduce context size in tooling).
/// </summary>
[McpServerResourceType]
public static class SummaryResources
{
    /// <summary>
    /// Returns a formatted summary of the specified file with bodies removed.
    /// </summary>
    /// <param name="file">Path to the C# file (relative or absolute).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Summary text.</returns>
    [McpServerResource(UriTemplate = "summary://{*file}", MimeType = "text/plain")]
    public static async Task<string> GetSummary(string file, CancellationToken cancellationToken = default)
    {
        var normalized = file.Replace('/', Path.DirectorySeparatorChar);
        if (!File.Exists(normalized))
        {
            return $"// File not found: {file}";
        }

        var text = await File.ReadAllTextAsync(normalized, cancellationToken);
        var tree = CSharpSyntaxTree.ParseText(text, cancellationToken: cancellationToken);
        var root = await tree.GetRootAsync(cancellationToken);
        var summarized = new BodyOmitter().Visit(root);
        var workspace = new AdhocWorkspace();
        var formatted = Formatter.Format(summarized, workspace);

        var sb = new StringBuilder();
        sb.AppendLine($"// summary://{file}");
        sb.AppendLine("// This file omits method bodies for brevity.");
        sb.Append(formatted.ToFullString());
        return sb.ToString();
    }
}
