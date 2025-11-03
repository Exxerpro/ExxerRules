using System;
using IndFusion.Analyzers;
using IndFusion.Analyzers.CodeQuality;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.CodeQuality;

/// <summary>
/// Tests for DoNotUseRegionsAnalyzer false-positive mitigation scenarios.
/// </summary>
public class DoNotUseRegionsFalsePositiveTests
{
    //  Story 1.1: Allow Regions for Constant Observability Buckets

    /// <summary>
    /// Tests that regions containing only const or static readonly fields are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Constant_Observability_Buckets()
    {
        const string testCode = @"
namespace TestProject
{
    public static class LoggingEvents
    {
        //  User Events
        public const int UserCreated = 1001;
        public const int UserUpdated = 1002;
        public const int UserDeleted = 1003;
        public static readonly string UserEventCategory = ""User"";
         // 

        //  Order Events
        public const int OrderPlaced = 2001;
        public const int OrderCancelled = 2002;
        public const int OrderCompleted = 2003;
        public static readonly string OrderEventCategory = ""Order"";
         // 
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseRegionsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.2: Allow Regions for Activity Source Constants

    /// <summary>
    /// Tests that regions containing ActivitySource constants for OpenTelemetry are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Activity_Source_Constants()
    {
        const string testCode = @"
using System.Diagnostics;

namespace TestProject
{
    public static class ActivitySources
    {
        //  Database Activities
        public const string DatabaseQuery = ""Database.Query"";
        public const string DatabaseConnection = ""Database.Connection"";
        
        private static string GetDatabaseActivityName(string operation)
        {
            return $""Database.{operation}"";
        }
         // 

        //  API Activities
        public const string ApiRequest = ""API.Request"";
        public const string ApiResponse = ""API.Response"";
        
        private static string GetApiActivityName(string endpoint)
        {
            return $""API.{endpoint}"";
        }
         // 
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseRegionsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.3: Allow Regions for Pipeline Steps in Command Handlers

    /// <summary>
    /// Tests that regions named ""Pipeline"" containing private methods ending with ""Step"" are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Pipeline_Steps_In_Command_Handlers()
    {
        const string testCode = @"
namespace TestProject
{
    public class CreateUserCommandHandler
    {
        public async Task<Result> Handle(CreateUserCommand command)
        {
            return await ValidateStep(command)
                .BindAsync(ProcessStep)
                .BindAsync(SaveStep);
        }

        //  Pipeline Steps
        private async Task<Result> ValidateStep(CreateUserCommand command)
        {
            // Validation logic
            return Result.Success();
        }

        private async Task<Result> ProcessStep(CreateUserCommand command)
        {
            // Processing logic
            return Result.Success();
        }

        private async Task<Result> SaveStep(CreateUserCommand command)
        {
            // Save logic
            return Result.Success();
        }
         // 
    }

    public class CreateUserCommand { }
    public class Result
    {
        public static Result Success() => new Result();
        public Result BindAsync(Func<CreateUserCommand, Task<Result>> func) => this;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseRegionsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.4: Allow Regions for Success/Failure Handlers

    /// <summary>
    /// Tests that regions named ""Success"" or ""Failure"" containing private handling methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Success_Failure_Handlers()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        //  Success Handlers
        private void LogUserCreated(string userId)
        {
            _logger.LogInformation(""User {UserId} created successfully"", userId);
        }

        private void LogUserUpdated(string userId)
        {
            _logger.LogInformation(""User {UserId} updated successfully"", userId);
        }
         // 

        //  Failure Handlers
        private void LogUserCreationFailed(string reason)
        {
            _logger.LogError(""User creation failed: {Reason}"", reason);
        }

        private void LogUserUpdateFailed(string userId, string reason)
        {
            _logger.LogError(""User {UserId} update failed: {Reason}"", userId, reason);
        }
         // 
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseRegionsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.5: Allow Regions for Helper Methods in Gateway Pipelines

    /// <summary>
    /// Tests that regions named ""Helper"" containing private methods returning Task or Result-like types are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Helper_Methods_In_Gateway_Pipelines()
    {
        const string testCode = @"
namespace TestProject
{
    public class ApiGateway
    {
        public async Task<Result> ProcessRequest(string request)
        {
            return await ValidateRequest(request)
                .BindAsync(TransformRequest)
                .BindAsync(SendToBackend);
        }

        //  Helper Methods
        private async Task<Result> ValidateRequest(string request)
        {
            // Validation logic
            return Result.Success();
        }

        private async Task<Result> TransformRequest(string request)
        {
            // Transformation logic
            return Result.Success();
        }

        private async Task<Result> SendToBackend(string request)
        {
            // Backend communication logic
            return Result.Success();
        }
         // 
    }

    public class Result
    {
        public static Result Success() => new Result();
        public Result BindAsync(Func<string, Task<Result>> func) => this;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseRegionsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.6: Allow Regions for Nested Context Classes

    /// <summary>
    /// Tests that regions containing exactly one nested type declaration are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Nested_Context_Classes()
    {
        const string testCode = @"
namespace TestProject
{
    public class UserHandler
    {
        public void ProcessUser(UserData data)
        {
            var context = new UserContext(data);
            // Process user logic
        }

        //  User Context
        private class UserContext
        {
            public UserData Data { get; }
            public DateTime CreatedAt { get; }

            public UserContext(UserData data)
            {
                Data = data;
                CreatedAt = DateTime.UtcNow;
            }
        }
         // 

        //  User Result
        private record UserResult(bool Success, string Message);
         // 
    }

    public class UserData { }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseRegionsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.7: Allow Regions for Private Helpers in Static Utilities

    /// <summary>
    /// Tests that regions in static classes containing only private static members are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Private_Helpers_In_Static_Utilities()
    {
        const string testCode = @"
namespace TestProject
{
    public static class StringUtils
    {
        public static string FormatName(string firstName, string lastName)
        {
            return CombineNames(firstName, lastName);
        }

        //  Private Helpers
        private static string CombineNames(string firstName, string lastName)
        {
            return $""{firstName} {lastName}"";
        }

        private static bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name);
        }

        private static string SanitizeName(string name)
        {
            return name.Trim();
        }
         // 
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseRegionsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.8: Allow Regions for Scenario Grouping in Tests

    /// <summary>
    /// Tests that regions in test files containing only xUnit test methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Scenario_Grouping_In_Tests()
    {
        const string testCode = @"
using Xunit;

namespace TestProject.Tests
{
    public class UserServiceTests
    {
        //  Create User Tests
        [Fact]
        public void Should_Create_User_When_Valid_Data()
        {
            // Test implementation
        }

        [Theory]
        [InlineData("""")]
        [InlineData(null)]
        public void Should_Not_Create_User_When_Invalid_Data(string invalidData)
        {
            // Test implementation
        }
         // 

        //  Update User Tests
        [Fact]
        public void Should_Update_User_When_Valid_Data()
        {
            // Test implementation
        }

        [Fact]
        public void Should_Not_Update_User_When_User_Not_Found()
        {
            // Test implementation
        }
         // 
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseRegionsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.9: Allow Regions for Fixture Definitions in Test Suites

    /// <summary>
    /// Tests that regions in test files containing only nested type declarations for test fixtures are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Fixture_Definitions_In_Test_Suites()
    {
        const string testCode = @"
using Xunit;

namespace TestProject.Tests
{
    public class IntegrationTests : IClassFixture<DatabaseFixture>
    {
        public IntegrationTests(DatabaseFixture fixture)
        {
            // Test setup
        }

        [Fact]
        public void Should_Test_Something()
        {
            // Test implementation
        }

        //  Test Fixtures
        public class DatabaseFixture : IDisposable
        {
            public void Dispose()
            {
                // Cleanup
            }
        }

        public class ApiFixture : IDisposable
        {
            public void Dispose()
            {
                // Cleanup
            }
        }
         // 
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseRegionsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Positive Control Tests

    /// <summary>
    /// Tests that regular regions are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Regular_Regions()
    {
        const string testCode = @"
namespace TestProject
{
    public class RegularClass
    {
        //  Properties
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
         // 

        //  Methods
        public void DoSomething()
        {
            // Implementation
        }

        public void DoSomethingElse()
        {
            // Implementation
        }
         // 
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseRegionsAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.DoNotUseRegions);
    }

     // 
}
