namespace HubConnection.IntegrationTests.Unit;

using System;
using IndTrace.Application.Models.Services;
using IndTrace.HubConnection.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

/// <summary>
/// Basic instantiation tests for hub connection classes.
/// Verifies System Under Test (SUT) can be created without throwing exceptions.
/// </summary>
public class BasicInstantiationTests
{
    private readonly ILogger _logger;

    public BasicInstantiationTests(ITestOutputHelper output)
    {
        _logger = Meziantou.Extensions.Logging.Xunit.v3.XUnitLogger.CreateLogger(output);
    }

    [Fact]
    public void HubConnectionFactory_Should_Instantiate_Successfully()
    {
        _logger.LogInformation("Starting HubConnectionFactory instantiation test");

        // Arrange
        _logger.LogDebug("Creating HubMonitorOptions with URL: http://localhost:5200/EventMonitor");
        var options = Options.Create(new HubMonitorOptions
        {
            Url = "http://localhost:5200/EventMonitor"
        });

        _logger.LogDebug("Creating typed logger for HubConnectionFactory");
        var factoryLogger = Meziantou.Extensions.Logging.Xunit.v3.XUnitLogger.CreateLogger<HubConnectionFactory>();

        // Act
        _logger.LogInformation("Attempting to instantiate HubConnectionFactory");
        var factory = new HubConnectionFactory(options, factoryLogger);

        // Assert
        _logger.LogInformation("Verifying HubConnectionFactory instance is not null");
        factory.ShouldNotBeNull();

        _logger.LogInformation("HubConnectionFactory instantiation test completed successfully");
    }

    [Fact]
    public void HubMonitorOptions_Should_Instantiate_With_Default_Values()
    {
        _logger.LogInformation("Starting HubMonitorOptions default instantiation test");

        // Act
        _logger.LogDebug("Creating HubMonitorOptions with default constructor");
        var options = new HubMonitorOptions();

        // Assert
        _logger.LogDebug("Verifying options instance is not null");
        options.ShouldNotBeNull();

        _logger.LogDebug("Verifying options type is correct");
        options.GetType().ShouldBe(typeof(HubMonitorOptions));

        _logger.LogInformation("Default values verification:");
        _logger.LogInformation("  - Url: {Url}", options.Url ?? "null");
        _logger.LogInformation("  - AcceptAnyServerCertificate: {AcceptAnyServerCertificate}", options.AcceptAnyServerCertificate);
        _logger.LogInformation("  - RetryTime: {RetryTime}", options.RetryTime);

        _logger.LogInformation("HubMonitorOptions default instantiation test completed successfully");
    }

    [Fact]
    public void HubMonitorOptions_Should_Accept_Valid_Url()
    {
        _logger.LogInformation("Starting HubMonitorOptions property assignment test");

        // Arrange
        var testUrl = "http://localhost:5200/EventMonitor";
        var testCertificateFlag = true;
        var testRetryTime = 30;

        _logger.LogDebug("Test values - URL: {Url}, AcceptAnyServerCertificate: {Certificate}, RetryTime: {RetryTime}",
            testUrl, testCertificateFlag, testRetryTime);

        // Act
        _logger.LogDebug("Creating HubMonitorOptions with specific values");
        var options = new HubMonitorOptions
        {
            Url = testUrl,
            AcceptAnyServerCertificate = testCertificateFlag,
            RetryTime = testRetryTime
        };

        // Assert
        _logger.LogDebug("Verifying options instance is not null");
        options.ShouldNotBeNull();

        _logger.LogInformation("Verifying assigned property values:");
        _logger.LogDebug("Checking URL assignment");
        options.Url.ShouldBe(testUrl);
        _logger.LogInformation("  ✓ URL correctly set to: {Url}", options.Url);

        _logger.LogDebug("Checking AcceptAnyServerCertificate assignment");
        options.AcceptAnyServerCertificate.ShouldBeTrue();
        _logger.LogInformation("  ✓ AcceptAnyServerCertificate correctly set to: {Certificate}", options.AcceptAnyServerCertificate);

        _logger.LogDebug("Checking RetryTime assignment");
        options.RetryTime.ShouldBe(testRetryTime);
        _logger.LogInformation("  ✓ RetryTime correctly set to: {RetryTime}", options.RetryTime);

        _logger.LogInformation("HubMonitorOptions property assignment test completed successfully");
    }
}
