using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace IndFusion.Tools.Cli.Core.Commands;

/// <summary>
/// Base class for all CLI commands providing common functionality and options
/// </summary>
public abstract class BaseCommand : Command
{
    /// <summary>
    /// Global verbose option for detailed output
    /// </summary>
    protected static readonly Option<bool> VerboseOption = new(
        aliases: ["--verbose", "-v"],
        description: "Enable verbose output");

    /// <summary>
    /// Global configuration file option
    /// </summary>
    protected static readonly Option<FileInfo?> ConfigOption = new(
        aliases: ["--config", "-c"],
        description: "Path to configuration file");

    /// <summary>
    /// Global log level option
    /// </summary>
    protected static readonly Option<LogLevel> LogLevelOption = new(
        aliases: ["--log-level"],
        description: "Set the logging level",
        getDefaultValue: () => LogLevel.Information);

    /// <summary>
    /// Global solution path option
    /// </summary>
    protected static readonly Option<FileInfo?> SolutionOption = new(
        aliases: ["--solution", "-s"],
        description: "Path to solution file");

    /// <summary>
    /// Global output directory option
    /// </summary>
    protected static readonly Option<DirectoryInfo?> OutputOption = new(
        aliases: ["--output", "-o"],
        description: "Output directory for results");

    /// <summary>
    /// Initializes a new instance of the BaseCommand class
    /// </summary>
    /// <param name="name">The name of the command</param>
    /// <param name="description">The description of the command</param>
    protected BaseCommand(string name, string description) : base(name, description)
    {
        // Add global options to all commands
        AddGlobalOption(VerboseOption);
        AddGlobalOption(ConfigOption);
        AddGlobalOption(LogLevelOption);
        AddGlobalOption(SolutionOption);
        AddGlobalOption(OutputOption);
    }

    /// <summary>
    /// Gets the logger for the command
    /// </summary>
    /// <param name="logLevel">The log level from command options</param>
    /// <param name="verbose">Whether verbose output is enabled</param>
    /// <returns>Configured logger instance</returns>
    protected static ILogger GetLogger(LogLevel logLevel, bool verbose)
    {
        var effectiveLogLevel = verbose ? LogLevel.Debug : logLevel;
        
        using var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddConsole().SetMinimumLevel(effectiveLogLevel));
        
        return loggerFactory.CreateLogger("IndFusion.Cli");
    }

    /// <summary>
    /// Validates that required options are provided
    /// </summary>
    /// <param name="solutionPath">The solution path option value</param>
    /// <param name="commandName">The name of the command for error messages</param>
    /// <returns>True if validation passes, false otherwise</returns>
    protected static bool ValidateSolutionPath(FileInfo? solutionPath, string commandName)
    {
        if (solutionPath == null)
        {
            Console.Error.WriteLine($"Error: {commandName} requires a solution file. Use --solution or -s option.");
            return false;
        }

        if (!solutionPath.Exists)
        {
            Console.Error.WriteLine($"Error: Solution file '{solutionPath.FullName}' does not exist.");
            return false;
        }

        if (!solutionPath.Extension.Equals(".sln", StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine($"Error: '{solutionPath.FullName}' is not a valid solution file (.sln).");
            return false;
        }

        return true;
    }
}