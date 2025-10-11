using IndFusion.Tools.Cli.Core;
using IndFusion.Tools.Cli.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Tools.Cli.Tests;

/// <summary>
/// Unit tests for CliApplication
/// </summary>
public class CliApplicationTests
{
    /// <summary>
    /// Tests that the default service provider can be created successfully
    /// </summary>
    [Fact]
    public void Should_Create_Default_Service_Provider()
    {
        // Act
        var serviceProvider = CliApplication.CreateDefaultServiceProvider();

        // Assert
        serviceProvider.ShouldNotBeNull();
        
        // Verify key services are registered
        serviceProvider.GetService<RefactoringOrchestrator>().ShouldNotBeNull();
        serviceProvider.GetService<CodeAnalyzer>().ShouldNotBeNull();
        serviceProvider.GetService<RefactoringService>().ShouldNotBeNull();
        serviceProvider.GetService<ILogger<CliApplication>>().ShouldNotBeNull();
    }

    /// <summary>
    /// Tests that the CLI application executes successfully with help argument
    /// </summary>
    [Fact]
    public async Task Should_Execute_With_Help_Argument()
    {
        // Arrange
        var serviceProvider = CliApplication.CreateDefaultServiceProvider();
        var cliApp = new CliApplication(serviceProvider);
        var args = new[] { "--help" };

        // Act
        var exitCode = await cliApp.ExecuteAsync(args, TestContext.Current.CancellationToken);

        // Assert
        exitCode.ShouldBe(0);
    }

    /// <summary>
    /// Tests that the CLI application executes successfully with version argument
    /// </summary>
    [Fact]
    public async Task Should_Execute_With_Version_Argument()
    {
        // Arrange
        var serviceProvider = CliApplication.CreateDefaultServiceProvider();
        var cliApp = new CliApplication(serviceProvider);
        var args = new[] { "--version" };

        // Act
        var exitCode = await cliApp.ExecuteAsync(args, TestContext.Current.CancellationToken);

        // Assert
        exitCode.ShouldBe(0);
    }

    /// <summary>
    /// Tests that the CLI application executes successfully with no arguments
    /// </summary>
    [Fact]
    public async Task Should_Execute_With_No_Arguments()
    {
        // Arrange
        var serviceProvider = CliApplication.CreateDefaultServiceProvider();
        var cliApp = new CliApplication(serviceProvider);
        var args = Array.Empty<string>();

        // Act
        var exitCode = await cliApp.ExecuteAsync(args, TestContext.Current.CancellationToken);

        // Assert
        exitCode.ShouldBe(0);
    }

    /// <summary>
    /// Tests that the CLI application handles invalid arguments gracefully
    /// </summary>
    [Fact]
    public async Task Should_Handle_Invalid_Arguments()
    {
        // Arrange
        var serviceProvider = CliApplication.CreateDefaultServiceProvider();
        var cliApp = new CliApplication(serviceProvider);
        var args = new[] { "invalid-command" };

        // Act
        var exitCode = await cliApp.ExecuteAsync(args, TestContext.Current.CancellationToken);

        // Assert
        exitCode.ShouldBe(1); // Should return error code for invalid command
    }

    /// <summary>
    /// Tests that the CLI application executes successfully with cancellation token
    /// </summary>
    [Fact]
    public async Task Should_Execute_With_Cancellation_Token()
    {
        // Arrange
        var serviceProvider = CliApplication.CreateDefaultServiceProvider();
        var cliApp = new CliApplication(serviceProvider);
        var args = new[] { "--help" };

        // Act
        var exitCode = await cliApp.ExecuteAsync(args, TestContext.Current.CancellationToken);

        // Assert
        exitCode.ShouldBe(0);
    }
}
