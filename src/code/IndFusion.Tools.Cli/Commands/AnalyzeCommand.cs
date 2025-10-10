using System.CommandLine;
using Microsoft.Extensions.Logging;
using IndFusion.Tools.Cli.Services;

namespace IndFusion.Tools.Cli.Commands;

/// <summary>
/// Command for analyzing code metrics, complexity, and refactoring opportunities
/// </summary>
public class AnalyzeCommand : BaseCommand
{
    private static readonly Option<string> AnalysisTypeOption = new(
        aliases: ["--type", "-t"],
        description: "Type of analysis to perform: metrics, complexity, opportunities")
    {
        IsRequired = true
    };

    private static readonly Option<string> PathOption = new(
        aliases: ["--path", "-p"],
        description: "Path to analyze (file or directory)");

    private static readonly Option<string> OutputFormatOption = new(
        aliases: ["--format", "-f"],
        description: "Output format: json, csv, markdown, console",
        getDefaultValue: () => "console");

    private static readonly Option<string> OutputFileOption = new(
        aliases: ["--output-file"],
        description: "Output file path (if not using --output directory)");

    private static readonly Option<bool> IncludeMetricsOption = new(
        aliases: ["--include-metrics"],
        description: "Include detailed metrics in analysis");

    private static readonly Option<bool> IncludeComplexityOption = new(
        aliases: ["--include-complexity"],
        description: "Include complexity analysis");

    private static readonly Option<bool> IncludeOpportunitiesOption = new(
        aliases: ["--include-opportunities"],
        description: "Include refactoring opportunities");

    private static readonly Option<int> MaxDepthOption = new(
        aliases: ["--max-depth"],
        description: "Maximum depth for recursive analysis",
        getDefaultValue: () => 10);

    /// <summary>
    /// Initializes a new instance of the AnalyzeCommand class
    /// </summary>
    public AnalyzeCommand() : base("analyze", "Analyze code metrics, complexity, and refactoring opportunities")
    {
        AddOption(AnalysisTypeOption);
        AddOption(PathOption);
        AddOption(OutputFormatOption);
        AddOption(OutputFileOption);
        AddOption(IncludeMetricsOption);
        AddOption(IncludeComplexityOption);
        AddOption(IncludeOpportunitiesOption);
        AddOption(MaxDepthOption);

        this.SetHandler(ExecuteAsync,
            AnalysisTypeOption,
            SolutionOption,
            PathOption,
            OutputFormatOption,
            OutputFileOption,
            IncludeMetricsOption,
            IncludeComplexityOption,
            IncludeOpportunitiesOption,
            MaxDepthOption,
            VerboseOption,
            ConfigOption,
            LogLevelOption,
            OutputOption);
    }

    /// <summary>
    /// Executes the analyze command
    /// </summary>
    private async Task<int> ExecuteAsync(
        string analysisType,
        FileInfo? solution,
        string? path,
        string outputFormat,
        string? outputFile,
        bool includeMetrics,
        bool includeComplexity,
        bool includeOpportunities,
        int maxDepth,
        bool verbose,
        FileInfo? config,
        LogLevel logLevel,
        DirectoryInfo? output)
    {
        try
        {
            // Validate required parameters
            if (!ValidateSolutionPath(solution, "analyze"))
            {
                return 1;
            }

            var logger = GetLogger(logLevel, verbose);
            logger.LogInformation("Starting analysis: {AnalysisType}", analysisType);

            // Create analysis request
            var request = new AnalysisRequest
            {
                AnalysisType = analysisType,
                SolutionPath = solution!.FullName,
                TargetPath = path,
                OutputFormat = outputFormat,
                OutputFile = outputFile,
                IncludeMetrics = includeMetrics,
                IncludeComplexity = includeComplexity,
                IncludeOpportunities = includeOpportunities,
                MaxDepth = maxDepth,
                OutputDirectory = output?.FullName
            };

            // Execute analysis
            var analyzer = new CodeAnalyzer(logger);
            var result = await analyzer.AnalyzeAsync(request, CancellationToken.None);

            if (result.Success)
            {
                logger.LogInformation("Analysis completed successfully");
                
                // Output results based on format
                if (result.Output != null)
                {
                    switch (outputFormat.ToLowerInvariant())
                    {
                        case "json":
                            Console.WriteLine(result.Output.JsonOutput);
                            break;
                        case "csv":
                            Console.WriteLine(result.Output.CsvOutput);
                            break;
                        case "markdown":
                            Console.WriteLine(result.Output.MarkdownOutput);
                            break;
                        case "console":
                        default:
                            Console.WriteLine(result.Output.ConsoleOutput);
                            break;
                    }
                }

                return 0;
            }
            else
            {
                logger.LogError("Analysis failed: {Error}", result.ErrorMessage);
                Console.Error.WriteLine($"Analysis failed: {result.ErrorMessage}");
                return 1;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unexpected error: {ex.Message}");
            return 1;
        }
    }
}

/// <summary>
/// Represents an analysis request
/// </summary>
public class AnalysisRequest
{
    /// <summary>
    /// Gets or sets the type of analysis to perform
    /// </summary>
    public string AnalysisType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the solution file
    /// </summary>
    public string SolutionPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the target path to analyze
    /// </summary>
    public string? TargetPath { get; set; }

    /// <summary>
    /// Gets or sets the output format
    /// </summary>
    public string OutputFormat { get; set; } = "console";

    /// <summary>
    /// Gets or sets the output file path
    /// </summary>
    public string? OutputFile { get; set; }

    /// <summary>
    /// Gets or sets whether to include detailed metrics
    /// </summary>
    public bool IncludeMetrics { get; set; }

    /// <summary>
    /// Gets or sets whether to include complexity analysis
    /// </summary>
    public bool IncludeComplexity { get; set; }

    /// <summary>
    /// Gets or sets whether to include refactoring opportunities
    /// </summary>
    public bool IncludeOpportunities { get; set; }

    /// <summary>
    /// Gets or sets the maximum depth for recursive analysis
    /// </summary>
    public int MaxDepth { get; set; } = 10;

    /// <summary>
    /// Gets or sets the output directory
    /// </summary>
    public string? OutputDirectory { get; set; }
}