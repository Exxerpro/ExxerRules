using System.CommandLine;
using IndFusion.Tools.Cli.Commands;

namespace IndFusion.Tools.Cli;

/// <summary>
/// Main entry point for IndFusion Tools CLI Application
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Exit code</returns>
    public static async Task<int> Main(string[] args)
    {
        try
        {
            // Create root command
            var rootCommand = new RootCommand("IndFusion Tools CLI - Professional refactoring and code analysis tools")
            {
                new RefactorCommand(),
                new AnalyzeCommand(),
                new InteractiveCommand()
            };

            // Add global options
            rootCommand.AddGlobalOption(new Option<bool>(
                aliases: ["--version", "-V"],
                description: "Show version information"));

            rootCommand.AddGlobalOption(new Option<bool>(
                aliases: ["--help", "-h", "-?"],
                description: "Show help information"));

            // Set handler for root command
            rootCommand.SetHandler((bool version, bool help) =>
            {
                if (version)
                {
                    Console.WriteLine("IndFusion Tools CLI v1.0.7");
                    Console.WriteLine("Professional refactoring and code analysis tools");
                    return;
                }

                if (help)
                {
                    Console.WriteLine(rootCommand.Description);
                    Console.WriteLine();
                    Console.WriteLine("Available commands:");
                    Console.WriteLine("  refactor     Execute refactoring operations on code");
                    Console.WriteLine("  analyze      Analyze code metrics, complexity, and refactoring opportunities");
                    Console.WriteLine("  interactive  Start interactive guided refactoring workflow");
                    Console.WriteLine();
                    Console.WriteLine("Use 'indfusion <command> --help' for more information about a command.");
                    return;
                }

                // If no specific command, show help
                Console.WriteLine(rootCommand.Description);
                Console.WriteLine("Use 'indfusion --help' for more information.");
            }, 
            new Option<bool>("--version", "Show version information").FromAmong(),
            new Option<bool>("--help", "Show help information").FromAmong());

            // Execute the command
            return await rootCommand.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error: {ex.Message}");
            return 1;
        }
    }
}
