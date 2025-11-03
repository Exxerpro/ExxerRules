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

            // Set handler for root command using new System.CommandLine v2 API
            // System.CommandLine automatically handles --version and --help, so we just show default message
            rootCommand.SetAction(parseResult =>
            {
                // If no specific command, show help
                Console.WriteLine(rootCommand.Description);
                Console.WriteLine("Use 'indfusion --help' for more information.");
                return 0;
            });

            // Execute the command using new System.CommandLine v2 API
            return rootCommand.Parse(args).Invoke();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error: {ex.Message}");
            return 1;
        }
    }
}