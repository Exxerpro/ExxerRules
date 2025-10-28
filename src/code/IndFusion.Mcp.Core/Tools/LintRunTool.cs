using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Services;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// MCP tool for running EXXER analyzers and returning violations with policy recommendations.
/// This tool orchestrates analyzer execution, violation detection, and policy enforcement.
/// </summary>
[McpServerToolType]
public static class LintRunTool
{
    /// <summary>
    /// Runs EXXER analyzers on the specified solution and returns violations with policy recommendations.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="scope">Scope for linting: "all" for entire solution, or specific file path.</param>
    /// <param name="severityConfig">Comma-separated severity levels to include: "error", "warning", "suggestion".</param>
    /// <param name="progress">Optional progress reporter for operation status updates.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A structured result containing violations, policy decisions, and remediation suggestions.</returns>
    [McpServerTool, Description("Run EXXER analyzers on solution and return violations with policy recommendations")]
    public static async Task<string> LintRun(
        [Description("Absolute path to the solution file (.sln)")] string solutionPath,
        [Description("Scope for linting: 'all' for entire solution, or specific file path")] string scope = "all",
        [Description("Comma-separated severity levels: 'error', 'warning', 'suggestion'")] string severityConfig = "error,warning",
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            progress?.Report("Starting linting analysis...");

            // Validate inputs
            if (!File.Exists(solutionPath))
            {
                throw new McpException($"Solution file not found at {solutionPath}");
            }

            if (string.IsNullOrWhiteSpace(scope))
            {
                throw new McpException("Scope cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(severityConfig))
            {
                throw new McpException("Severity configuration cannot be null or empty");
            }

            progress?.Report("Loading solution and preparing analyzers...");

            // TODO: Implement proper solution loading with caching
            // For now, use a simple workspace approach
            using var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionPath, progress: null, cancellationToken);

            progress?.Report("Running EXXER analyzers...");

            // Parse severity configuration
            var severities = ParseSeverityConfig(severityConfig);
            
            // Create linting request
            var request = new LintingRequest(
                SolutionPath: solutionPath,
                Scope: scope,
                SeverityFilter: "Warning", // TODO: Use parsed severities
                IncludePolicyRecommendations: true
            );

            // Create linting service (this would be injected in real implementation)
            var logger = new LoggerFactory().CreateLogger<LintingService>();
            var lintingService = new LintingService(logger);

            // Execute linting
            var result = await lintingService.RunLintingAsync(request, cancellationToken);

            progress?.Report("Analysis complete, formatting results...");

            // Format results for MCP response
            return FormatLintingResult(result);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new McpException($"Error running linting analysis: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Parses the severity configuration string into a list of DiagnosticSeverity values.
    /// </summary>
    /// <param name="severityConfig">Comma-separated severity configuration string.</param>
    /// <returns>List of DiagnosticSeverity values.</returns>
    private static List<DiagnosticSeverity> ParseSeverityConfig(string severityConfig)
    {
        var severities = new List<DiagnosticSeverity>();
        var parts = severityConfig.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            var trimmed = part.Trim().ToLowerInvariant();
            switch (trimmed)
            {
                case "error":
                    severities.Add(DiagnosticSeverity.Error);
                    break;
                case "warning":
                    severities.Add(DiagnosticSeverity.Warning);
                    break;
                case "suggestion":
                case "info":
                    severities.Add(DiagnosticSeverity.Info);
                    break;
                case "hidden":
                    severities.Add(DiagnosticSeverity.Hidden);
                    break;
                default:
                    throw new McpException($"Invalid severity level: {part}. Valid values are: error, warning, suggestion, hidden");
            }
        }

        return severities;
    }

    /// <summary>
    /// Formats the linting result into a human-readable string for MCP response.
    /// </summary>
    /// <param name="result">The linting result to format.</param>
    /// <returns>Formatted string representation of the linting result.</returns>
    private static string FormatLintingResult(LintingResult result)
    {
        var output = new System.Text.StringBuilder();
        
        output.AppendLine("=== EXXER Linting Analysis Results ===");
        output.AppendLine($"Analysis completed at: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        output.AppendLine();

        if (result.Violations?.Any() == true)
        {
            output.AppendLine($"Found {result.Violations.Count()} violations:");
            output.AppendLine();

            foreach (var violation in result.Violations)
            {
                output.AppendLine($"🔍 {violation.RuleId}: {violation.Message}");
                output.AppendLine($"   📍 Location: {violation.FilePath}:{violation.Line}:{violation.Column}");
                output.AppendLine($"   ⚠️  Severity: {violation.Severity}");
                
                if (violation.PolicyRecommendation != null)
                {
                    output.AppendLine($"   💡 Policy Recommendation: {violation.PolicyRecommendation.Action} - {violation.PolicyRecommendation.Reason}");
                }

                if (violation.RemediationSuggestions?.Any() == true)
                {
                    output.AppendLine($"   🔧 Remediation Suggestions:");
                    foreach (var suggestion in violation.RemediationSuggestions)
                    {
                        output.AppendLine($"      • {suggestion}");
                    }
                }

                output.AppendLine();
            }
        }
        else
        {
            output.AppendLine("✅ No violations found! Code meets EXXER standards.");
        }

        if (result.PolicyDecisions?.Any() == true)
        {
            output.AppendLine("📋 Policy Decisions:");
            foreach (var decision in result.PolicyDecisions)
            {
                output.AppendLine($"   • {decision.RuleId}: {decision.Decision} - {decision.Reason}");
            }
            output.AppendLine();
        }

        if (result.Summary != null)
        {
            output.AppendLine("📊 Summary:");
            output.AppendLine($"   • Total violations: {result.Summary.TotalViolations}");
            output.AppendLine($"   • Errors: {result.Summary.ErrorCount}");
            output.AppendLine($"   • Warnings: {result.Summary.WarningCount}");
            output.AppendLine($"   • Info: {result.Summary.InfoCount}");
            output.AppendLine($"   • Hints: {result.Summary.HintCount}");
            output.AppendLine($"   • Files analyzed: {result.Summary.FilesAnalyzed}");
            output.AppendLine($"   • Rules checked: {result.Summary.RulesChecked}");
        }

        return output.ToString();
    }
}
