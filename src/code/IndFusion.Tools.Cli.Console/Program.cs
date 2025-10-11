using IndFusion.Tools.Cli.Core;

namespace IndFusion.Tools.Cli.Console;

/// <summary>
/// Console wrapper for IndFusion CLI Tools
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point for the console application
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Exit code</returns>
    public static async Task<int> Main(string[] args)
    {
        // Create service provider
        var serviceProvider = CliApplication.CreateDefaultServiceProvider();
        
        // Create and execute CLI application
        var cliApp = new CliApplication(serviceProvider);
        return await cliApp.ExecuteAsync(args);
    }
}