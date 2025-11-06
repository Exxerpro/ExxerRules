using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Logging;
using IndFusion.Tools.Cli.Core.Services;

namespace IndFusion.Tools.Cli.Core.Commands;

/// <summary>
/// Command for interactive guided refactoring workflow
/// </summary>
public class InteractiveCommand : BaseCommand
{
    private static readonly Option<bool> GuidedOption = new("--guided", "-g")
    {
        Description = "Enable guided mode with step-by-step instructions"
    };

    private static readonly Option<bool> AutoDetectOption = new("--auto-detect")
    {
        Description = "Automatically detect refactoring opportunities"
    };

    private static readonly Option<string> ProfileOption = new("--profile")
    {
        Description = "Use a specific configuration profile"
    };

    /// <summary>
    /// Initializes a new instance of the InteractiveCommand class
    /// </summary>
    public InteractiveCommand() : base("interactive", "Start interactive guided refactoring workflow")
    {
        Options.Add(GuidedOption);
        Options.Add(AutoDetectOption);
        Options.Add(ProfileOption);

        this.SetAction(async (ParseResult parseResult, CancellationToken cancellationToken) =>
        {
            var solution = parseResult.GetValue(SolutionOption);
            var guided = parseResult.GetValue(GuidedOption);
            var autoDetect = parseResult.GetValue(AutoDetectOption);
            var profile = parseResult.GetValue(ProfileOption);
            var verbose = parseResult.GetValue(VerboseOption);
            var config = parseResult.GetValue(ConfigOption);
            var logLevel = parseResult.GetValue(LogLevelOption);
            var output = parseResult.GetValue(OutputOption);

            return await ExecuteAsync(solution, guided, autoDetect, profile, verbose, config, logLevel, output);
        });
    }

    /// <summary>
    /// Executes the interactive command
    /// </summary>
    private async Task<int> ExecuteAsync(
        FileInfo? solution,
        bool guided,
        bool autoDetect,
        string? profile,
        bool verbose,
        FileInfo? config,
        LogLevel logLevel,
        DirectoryInfo? output)
    {
        try
        {
            // Validate required parameters
            if (!ValidateSolutionPath(solution, "interactive"))
            {
                return 1;
            }

            var logger = GetLogger(logLevel, verbose);
            logger.LogInformation("Starting interactive refactoring session");

            // Create interactive session
            var session = new InteractiveSession(logger)
            {
                SolutionPath = solution!.FullName,
                GuidedMode = guided,
                AutoDetectOpportunities = autoDetect,
                Profile = profile,
                OutputDirectory = output?.FullName
            };

            // Start interactive session
            var result = await session.StartAsync(CancellationToken.None);

            if (result.IsSuccess)
            {
                logger.LogInformation("Interactive session completed successfully");
                Console.WriteLine("Interactive refactoring session completed successfully!");
                return 0;
            }
            else
            {
                logger.LogError("Interactive session failed: {Error}", result.ErrorMessage);
                Console.Error.WriteLine($"Interactive session failed: {result.ErrorMessage}");
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