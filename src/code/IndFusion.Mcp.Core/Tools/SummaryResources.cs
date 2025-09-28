using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Core.SyntaxRewriters;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Provides a lightweight summary view of C# files by omitting method bodies.
/// </summary>
[McpServerResourceType]
public static class SummaryResources
{
    /// <summary>
    /// Returns a formatted summary of a C# file with bodies omitted.
    /// </summary>
    /// <param name="file">Path-like identifier after the summary:// scheme.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Plain text summary of the file.</returns>
    [McpServerResource(UriTemplate = "summary://{*file}", MimeType = "text/plain")]
    public static async Task<string> GetSummary(string file, CancellationToken cancellationToken = default)
    {
        var normalized = file.Replace('/', Path.DirectorySeparatorChar);
        if (!File.Exists(normalized))
        {
            return $"// File not found: {file}";
        }

        var text = await File.ReadAllTextAsync(normalized, cancellationToken);
        var tree = CSharpSyntaxTree.ParseText(text);
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
