using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using IndFusion.Tools.Cli.Core.Commands;
using IndFusion.Tools.Cli.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IndFusion.Tools.Cli.Core;

/// <summary>
/// Main CLI application class that can be called from console wrapper or unit tests
/// </summary>
/// <remarks>
/// Initializes a new instance of the CliApplication class
/// </remarks>
/// <param name="serviceProvider">The service provider for dependency injection</param>
public class CliApplication(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    /// <summary>
    /// Executes the CLI application with the given arguments
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Exit code</returns>
    public async Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        try
        {
            var rootCommand = BuildCommandLine();
            // Fix: Use rootCommand.Parse(args).Invoke() for System.CommandLine 2.x or earlier
            var parseResult = rootCommand.Parse(args);
            return parseResult.Invoke();
        }
        catch (Exception ex)
        {
            var logger = _serviceProvider.GetService<ILogger<CliApplication>>();
            logger?.LogError(ex, "Fatal error in CLI application");
            Console.Error.WriteLine($"Fatal error: {ex.Message}");
            return 1;
        }
    }

    /// <summary>
    /// Builds the command line parser with all commands and options
    /// </summary>
    /// <returns>Configured root command</returns>
    private RootCommand BuildCommandLine()
    {
        var rootCommand = new RootCommand("IndFusion Tools CLI - Professional refactoring and code analysis tools")
        {
            new RefactorCommand(),
            new AnalyzeCommand(),
            new InteractiveCommand()
        };

        // Set handler for root command - System.CommandLine automatically handles --version and --help
        rootCommand.SetAction(parseResult =>
        {
            // If no specific command, show help
            Console.WriteLine(rootCommand.Description);
            Console.WriteLine("Use 'indfusion --help' for more information.");
            return 0;
        });

        return rootCommand;
    }

    /// <summary>
    /// Creates a default service provider with all required services
    /// </summary>
    /// <returns>Configured service provider</returns>
    public static IServiceProvider CreateDefaultServiceProvider()
    {
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(configure => configure.AddConsole());

        // Add CLI services
        services.AddSingleton<RefactoringOrchestrator>(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<RefactoringOrchestrator>();
            return new RefactoringOrchestrator(logger);
        });
        services.AddSingleton<CodeAnalyzer>(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<CodeAnalyzer>();
            return new CodeAnalyzer(logger);
        });
        services.AddSingleton<RefactoringService>(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<RefactoringService>();
            return new RefactoringService(logger);
        });

        return services.BuildServiceProvider();
    }
}