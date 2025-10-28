using ExxerAI.Application.Extensions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Diagnostic test to debug SeqLogAnalyzer SQL query validation issues.
/// </summary>
[Collection("Sequential")]
public sealed class SqlAnalyzerDiagnosticTest : IAsyncDisposable
{
    private readonly SeqLoggingTestFixture _fixture;

    public SqlAnalyzerDiagnosticTest()
    {
        _fixture = new SeqLoggingTestFixture();
    }

    [Fact(Timeout = 30_000)]
    public async Task DiagnosticTest_InspectLogCaptureAndAnalysis()
    {
        // Generate some test logs to analyze
        var correlationId = LoggingExtensions.CreateCorrelationId();

        var logger = _fixture.CreateLogger("DiagnosticTestService");

        _fixture.ClearLogs();

        // Create diverse logging patterns
        logger.LogInformation("System diagnostic test initiated");
        logger.LogOperationStart("TestOperation", correlationId, new { TestData = "diagnostic" });
        logger.LogExternalApiCallStart("TestAPI", "/test/endpoint", "GET", correlationId);
        logger.LogExternalApiCallSuccess("TestAPI", "/test/endpoint", "GET", TimeSpan.FromMilliseconds(50), correlationId, 200.ToString(), 1024.ToString());
        logger.LogOperationSuccess("TestOperation", correlationId, TimeSpan.FromMilliseconds(75), new { Result = "completed" });

        await Task.Delay(100); // Allow logs to flush

        // Inspect captured logs
        var logs = _fixture.GetCapturedLogs().ToList();

        // Output diagnostic information
        Console.WriteLine($"[DIAGNOSTIC] Total logs captured: {logs.Count}");

        foreach (var log in logs.Take(10)) // Show first 10 logs
        {
            Console.WriteLine($"[DIAGNOSTIC] Level: {log.Level}, Message: '{log.Message}', Properties: {log.Properties.Count}");
            foreach (var prop in log.Properties)
            {
                Console.WriteLine($"[DIAGNOSTIC]   - {prop.Key}: {prop.Value}");
            }
        }

        // Test basic SeqLogAnalyzer functionality
        using var analyzer = new SeqLogAnalyzer(logs);

        // Get analytics
        var analytics = analyzer.GetAnalytics();
        Console.WriteLine($"[DIAGNOSTIC] Analytics - Total: {analytics.TotalLogCount}, Operations: {analytics.OperationTypes.Count}, CorrelationIDs: {analytics.UniqueCorrelationIds}");

        // Test a simple validation
        try
        {
            var hasInfoLogs = analyzer.ValidateLoggingPattern(
                "SELECT COUNT(*) FROM logs WHERE Level = 'Information' >= 1");
            Console.WriteLine($"[DIAGNOSTIC] Information logs validation: {hasInfoLogs}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DIAGNOSTIC] Error validating Information logs: {ex.Message}");
        }

        // Test external API validation
        try
        {
            var externalApiValid = analyzer.ValidateLoggingPattern(
                "VALIDATE EXTERNAL_API_PATTERN");
            Console.WriteLine($"[DIAGNOSTIC] External API pattern validation: {externalApiValid}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DIAGNOSTIC] Error validating external API pattern: {ex.Message}");
        }

        // Basic assertion to ensure we captured some logs
        logs.ShouldNotBeEmpty("Should capture logs for diagnostic analysis");
        analytics.TotalLogCount.ShouldBeGreaterThan(0, "Analytics should show captured logs");
    }

    public async ValueTask DisposeAsync()
    {
        if (_fixture != null)
            await _fixture.DisposeAsync();
    }
}