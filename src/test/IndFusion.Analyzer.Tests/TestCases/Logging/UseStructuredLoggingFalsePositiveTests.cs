using System;
using Microsoft.Extensions.Logging;
using IndFusion.Analyzers;
using IndFusion.Analyzers.Logging;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Logging;

/// <summary>
/// Tests for UseStructuredLoggingAnalyzer false-positive mitigation scenarios.
/// </summary>
public class UseStructuredLoggingFalsePositiveTests
{
    #region Story 1.1: Verify Receiver is ILogger

    /// <summary>
    /// Tests that non-ILogger receivers are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Non_ILogger_Receivers()
    {
        const string testCode = @"
using System;

namespace MyProject.Services
{
    public class NonLoggerService
    {
        public void ProcessData()
        {
            // These should not be flagged - not ILogger receivers
            var calculator = new Calculator();
            calculator.LogCalculation($""Result: {42}"");
            
            var fileWriter = new FileWriter();
            fileWriter.LogToFile($""Writing data: {DateTime.Now}"");
            
            var customLogger = new CustomLogger();
            customLogger.LogMessage($""Custom log: {Guid.NewGuid()}"");
        }
    }

    public class Calculator
    {
        public void LogCalculation(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class FileWriter
    {
        public void LogToFile(string message)
        {
            System.IO.File.WriteAllText(""log.txt"", message);
        }
    }

    public class CustomLogger
    {
        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.2: Recognize Existing Structured Templates

    /// <summary>
    /// Tests that existing structured templates are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Existing_Structured_Templates()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace MyProject.Services
{
    public class StructuredLoggingService
    {
        private readonly ILogger<StructuredLoggingService> _logger;

        public StructuredLoggingService(ILogger<StructuredLoggingService> logger)
        {
            _logger = logger;
        }

        public void ProcessUser(int userId, string userName)
        {
            // These should not be flagged - already using structured templates
            _logger.LogInformation(""User {UserId} with name {UserName} processed"", userId, userName);
            _logger.LogWarning(""User {UserId} has invalid data"", userId);
            _logger.LogError(""Failed to process user {UserId} at {Timestamp}"", userId, DateTime.Now);
            _logger.LogDebug(""Processing user {UserId} with role {UserRole}"", userId, ""Admin"");
        }

        public void HandleRequest(string requestId, int statusCode)
        {
            // These should not be flagged - already using structured templates
            _logger.LogInformation(""Request {RequestId} completed with status {StatusCode}"", requestId, statusCode);
            _logger.LogWarning(""Request {RequestId} took {Duration}ms"", requestId, 1500);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.3: Support Interpolated String Handlers

    /// <summary>
    /// Tests that interpolated string handlers are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Interpolated_String_Handlers()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace MyProject.Services
{
    public class InterpolatedStringHandlerService
    {
        private readonly ILogger<InterpolatedStringHandlerService> _logger;

        public InterpolatedStringHandlerService(ILogger<InterpolatedStringHandlerService> logger)
        {
            _logger = logger;
        }

        public void ProcessData(int id, string name)
        {
            // These should not be flagged - using interpolated string handlers
            _logger.LogInformation($""Processing item {id} with name {name}"");
            _logger.LogWarning($""Item {id} has issues"");
            _logger.LogError($""Failed to process item {id} at {DateTime.Now}"");
        }

        public void HandleEvent(string eventType, object data)
        {
            // These should not be flagged - using interpolated string handlers
            _logger.LogInformation($""Handling event {eventType} with data {data}"");
            _logger.LogDebug($""Event {eventType} processed successfully"");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.4: Support Logging Wrapper Helpers

    /// <summary>
    /// Tests that logging wrapper helpers are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Logging_Wrapper_Helpers()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace MyProject.Services
{
    public class LoggingWrapperService
    {
        private readonly ILogger<LoggingWrapperService> _logger;

        public LoggingWrapperService(ILogger<LoggingWrapperService> logger)
        {
            _logger = logger;
        }

        public void ProcessUser(int userId, string userName)
        {
            // These should not be flagged - using helper methods for templates
            _logger.LogInformation(GetUserProcessedTemplate(), userId, userName);
            _logger.LogWarning(GetUserWarningTemplate(), userId);
            _logger.LogError(GetUserErrorTemplate(), userId, DateTime.Now);
        }

        private string GetUserProcessedTemplate() => ""User {UserId} with name {UserName} processed"";
        private string GetUserWarningTemplate() => ""User {UserId} has invalid data"";
        private string GetUserErrorTemplate() => ""Failed to process user {UserId} at {Timestamp}"";
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.5: Support Interpolation with Positional Arguments

    /// <summary>
    /// Tests that interpolation with positional arguments is not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Interpolation_With_Positional_Arguments()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace MyProject.Services
{
    public class InterpolationWithPositionalService
    {
        private readonly ILogger<InterpolationWithPositionalService> _logger;

        public InterpolationWithPositionalService(ILogger<InterpolationWithPositionalService> logger)
        {
            _logger = logger;
        }

        public void ProcessData(int id, string name)
        {
            // These should not be flagged - interpolation with structured placeholders
            _logger.LogInformation($""Processing item {{ItemId}} with name {{ItemName}}"", id, name);
            _logger.LogWarning($""Item {{ItemId}} has issues"", id);
            _logger.LogError($""Failed to process item {{ItemId}} at {{Timestamp}}"", id, DateTime.Now);
        }

        public void HandleRequest(string requestId, int statusCode)
        {
            // These should not be flagged - interpolation with structured placeholders
            _logger.LogInformation($""Request {{RequestId}} completed with status {{StatusCode}}"", requestId, statusCode);
            _logger.LogWarning($""Request {{RequestId}} took {{Duration}}ms"", requestId, 1500);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.6: Support Localization Resources

    /// <summary>
    /// Tests that localization resources are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Localization_Resources()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

namespace MyProject.Services
{
    public class LocalizationService
    {
        private readonly ILogger<LocalizationService> _logger;
        private readonly IStringLocalizer<LocalizationService> _localizer;

        public LocalizationService(ILogger<LocalizationService> logger, IStringLocalizer<LocalizationService> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        public void ProcessUser(int userId, string userName)
        {
            // These should not be flagged - using localization resources
            _logger.LogInformation(_localizer[""UserProcessed""], userId, userName);
            _logger.LogWarning(_localizer[""UserWarning""], userId);
            _logger.LogError(_localizer[""UserError""], userId, DateTime.Now);
        }

        public void HandleRequest(string requestId, int statusCode)
        {
            // These should not be flagged - using localization resources
            _logger.LogInformation(_localizer[""RequestCompleted""], requestId, statusCode);
            _logger.LogWarning(_localizer[""RequestSlow""], requestId, 1500);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.7: Support Other Structured Logging Libraries

    /// <summary>
    /// Tests that other structured logging libraries are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Other_Structured_Logging_Libraries()
    {
        const string testCode = @"
using Serilog;
using NLog;

namespace MyProject.Services
{
    public class OtherLoggingLibrariesService
    {
        private readonly ILogger _serilogLogger;
        private readonly NLog.ILogger _nlogLogger;

        public OtherLoggingLibrariesService()
        {
            _serilogLogger = Log.Logger;
            _nlogLogger = NLog.LogManager.GetCurrentClassLogger();
        }

        public void ProcessData(int id, string name)
        {
            // These should not be flagged - using Serilog
            _serilogLogger.Information($""Processing item {id} with name {name}"");
            _serilogLogger.Warning($""Item {id} has issues"");
            _serilogLogger.Error($""Failed to process item {id} at {DateTime.Now}"");

            // These should not be flagged - using NLog
            _nlogLogger.Info($""Processing item {id} with name {name}"");
            _nlogLogger.Warn($""Item {id} has issues"");
            _nlogLogger.Error($""Failed to process item {id} at {DateTime.Now}"");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.8: Exempt Non-Structured Sinks

    /// <summary>
    /// Tests that non-structured sinks are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Non_Structured_Sinks()
    {
        const string testCode = @"
using System;

namespace MyProject.Services
{
    public class NonStructuredSinkService
    {
        public void ProcessData(int id, string name)
        {
            // These should not be flagged - non-structured sinks
            Console.WriteLine($""Processing item {id} with name {name}"");
            Console.WriteLine($""Item {id} has issues"");
            Console.WriteLine($""Failed to process item {id} at {DateTime.Now}"");

            // These should not be flagged - non-structured sinks
            System.Diagnostics.Debug.WriteLine($""Processing item {id} with name {name}"");
            System.Diagnostics.Trace.WriteLine($""Item {id} has issues"");

            // These should not be flagged - non-structured sinks
            System.IO.File.WriteAllText(""log.txt"", $""Processing item {id} with name {name}"");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.9: Exempt Testing Context Output

    /// <summary>
    /// Tests that testing context output is not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Testing_Context_Output()
    {
        const string testCode = @"
using System;
using Xunit;
using Xunit.Abstractions;

namespace MyProject.Tests
{
    public class TestingContextOutputTests
    {
        private readonly ITestOutputHelper _output;

        public TestingContextOutputTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestMethod()
        {
            // These should not be flagged - testing context output
            _output.WriteLine($""Test started at {DateTime.Now}"");
            _output.WriteLine($""Processing test data {Guid.NewGuid()}"");
            _output.WriteLine($""Test completed with result {42}"");
        }

        [Fact]
        public void AnotherTestMethod()
        {
            // These should not be flagged - testing context output
            _output.WriteLine($""Another test with data {DateTime.Now:yyyy-MM-dd}"");
            _output.WriteLine($""Test result: {true}"");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.10: Provide an Opt-Out Attribute

    /// <summary>
    /// Tests that opt-out attribute allows interpolated logging.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Opt_Out_Attribute()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace MyProject.Services
{
    [AllowInterpolatedLogging]
    public class OptOutService
    {
        private readonly ILogger<OptOutService> _logger;

        public OptOutService(ILogger<OptOutService> logger)
        {
            _logger = logger;
        }

        public void ProcessData(int id, string name)
        {
            // These should not be flagged due to the attribute
            _logger.LogInformation($""Processing item {id} with name {name}"");
            _logger.LogWarning($""Item {id} has issues"");
            _logger.LogError($""Failed to process item {id} at {DateTime.Now}"");
        }

        [AllowInterpolatedLogging]
        public void AnotherMethod(int id)
        {
            // This should not be flagged due to the method attribute
            _logger.LogInformation($""Processing item {id}"");
        }
    }

    public class AllowInterpolatedLoggingAttribute : Attribute
    {
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Positive Control Tests

    /// <summary>
    /// Tests that actual inefficient logging is still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Actual_Inefficient_Logging()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace MyProject.Services
{
    public class InefficientLoggingService
    {
        private readonly ILogger<InefficientLoggingService> _logger;

        public InefficientLoggingService(ILogger<InefficientLoggingService> logger)
        {
            _logger = logger;
        }

        public void ProcessData(int id, string name)
        {
            // These should be flagged - inefficient string interpolation
            _logger.LogInformation($""Processing item {id} with name {name}"");
            _logger.LogWarning($""Item {id} has issues"");
            _logger.LogError($""Failed to process item {id} at {DateTime.Now}"");

            // These should be flagged - inefficient string concatenation
            _logger.LogInformation(""Processing item "" + id + "" with name "" + name);
            _logger.LogWarning(""Item "" + id + "" has issues"");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.UseStructuredLogging);
    }

    #endregion
}
