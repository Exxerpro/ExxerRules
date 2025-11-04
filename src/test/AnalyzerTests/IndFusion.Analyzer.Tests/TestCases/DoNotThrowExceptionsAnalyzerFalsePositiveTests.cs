using IndFusion.Analyzer.FunctionalPatterns;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases;

/// <summary>
/// Regression tests ensuring EXXER003 (DoNotThrowExceptionsAnalyzer) avoids false positives per spec.
/// </summary>
public sealed class DoNotThrowExceptionsAnalyzerFalsePositiveTests
{
    private const int AnalyzerTimeoutMs = 30000;

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for methods that guard against null arguments by throwing
    /// an ArgumentNullException.
    /// </summary>
    /// <remarks>This test ensures that the analyzer correctly recognizes standard argument null checks as
    /// valid and does not produce false positives when an ArgumentNullException is thrown for null
    /// parameters.</remarks>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_ArgumentNull_Guards()
    {
        const string testCode = @"
using System;

public class Guarded
{
    public static void Validate(string input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for null-coalescing throw expressions
    /// </summary>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_NullCoalescing_Throw()
    {
        const string testCode = @"
using System;

public sealed class Widget
{
    public string Label { get; }

    public Widget(string label)
    {
        Label = label ?? throw new ArgumentNullException(nameof(label));
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for range guard checks that throw ArgumentOutOfRangeException.
    /// </summary>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_Range_Guard()
    {
        const string testCode = @"
using System;

public static class Parser
{
    public static void Validate(int port)
    {
        if (port < 0 || port > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port));
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for switch expressions that throw ArgumentOutOfRangeException in the default case.
    /// </summary>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_Switch_Default_Throw()
    {
        const string testCode = @"
using System;

public enum DbType { Byte, Word }

public static class Resolver
{
    public static int Resolve(DbType type) => type switch
    {
        DbType.Byte => 1,
        DbType.Word => 2,
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for domain-specific validation exceptions.
    /// </summary>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_Domain_Validation_Exception()
    {
        const string testCode = @"
using System;

public sealed class InvalidWidgetException : Exception
{
    public InvalidWidgetException(string message) : base(message) { }
}

public static class WidgetParser
{
    public static void Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new InvalidWidgetException(""Input required"");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for exceptions thrown in a constructor to enforce
    /// invariants.
    /// </summary>
    /// <remarks>This test ensures that the analyzer correctly allows exceptions used for input validation
    /// within constructors, which is a common and recommended practice for enforcing class invariants.</remarks>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_Constructor_Invariant()
    {
        const string testCode = @"
using System;

public sealed class Capacity
{
    public Capacity(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for exception throwing in a factory method used for input
    /// validation.
    /// </summary>
    /// <remarks>This test ensures that the DoNotThrowExceptionsAnalyzer does not flag exception usage in
    /// factory methods where argument validation is performed, as such patterns are considered acceptable in .NET
    /// design guidelines.</remarks>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_Validation_In_Factory()
    {
        const string testCode = @"
using System;

public static class WidgetFactory
{
    public static Widget Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(""Name required"", nameof(name));
        }

        return new Widget(name);
    }
}

public sealed record Widget(string Name);
";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for expression-bodied properties that use guard clauses
    /// with exception throwing.
    /// </summary>
    /// <remarks>This test ensures that properties using expression-bodied syntax with guard clauses, such as
    /// throwing an exception when a required value is not configured, are not incorrectly flagged by the analyzer. The
    /// scenario reflects a common pattern for enforcing invariants in property getters.</remarks>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_Expression_Bodied_Guard()
    {
        const string testCode = @"
using System;

public sealed class Settings
{
    private readonly string _connectionString = """";

    public string ConnectionString => string.IsNullOrEmpty(_connectionString)
        ? throw new InvalidOperationException(""Connection string not configured"")
        : _connectionString;
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for defensive throws of NotSupportedException in legacy
    /// adapter scenarios.
    /// </summary>
    /// <remarks>This test ensures that the DoNotThrowExceptionsAnalyzer does not flag code where
    /// NotSupportedException is used to indicate unsupported operations in legacy or read-only adapters, which is
    /// considered an acceptable defensive pattern.</remarks>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_NotSupported_Defensive_Throw()
    {
        const string testCode = @"
using System;

public sealed class LegacyAdapter
{
    public void Execute()
    {
        throw new NotSupportedException(""Legacy adapter is read-only"");
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for exception wrapping scenarios where an exception is caught
    /// </summary>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_Exception_Wrapping()
    {
        const string testCode = @"
using System;

public static class Wrapper
{
    public static void Execute(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(""Failed to execute action"", ex);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer reports a diagnostic when a generic exception is thrown in user code.
    /// </summary>
    /// <remarks>This test provides a code sample that throws a generic Exception and asserts that the
    /// analyzer correctly identifies and reports the violation. Use this test to ensure the analyzer enforces best
    /// practices regarding exception types.</remarks>

    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Positive_Control_Should_Report_When_Generic_Exception()
    {
        const string testCode = @"
using System;

public sealed class Service
{
    public void DoWork()
    {
        throw new Exception();
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(1);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for framework boundary classes like Controllers.
    /// </summary>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_Framework_Boundary()
    {
        const string testCode = @"
using System;
using Microsoft.AspNetCore.Mvc;

public class UserController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return Ok();
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for domain validation classes.
    /// </summary>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_Domain_Validation()
    {
        const string testCode = @"
using System;

public class OrderValidator
{
    public void Validate(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        if (order.Amount <= 0) throw new InvalidOrderException(""Amount must be positive"");
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for configuration classes.
    /// </summary>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_Configuration_Context()
    {
        const string testCode = @"
using System;

public class AppSettings
{
    public string ConnectionString { get; set; } = ""DefaultConnection"";
    
    public void Validate()
    {
        if (string.IsNullOrEmpty(ConnectionString)) 
            throw new InvalidOperationException(""Connection string not configured"");
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Comprehensive regression test covering all new framework boundary patterns.
    /// </summary>
    [Fact(Timeout = AnalyzerTimeoutMs)]
    public void Should_Not_Report_For_All_Framework_Boundary_Patterns()
    {
        const string testCode = @"
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

// ASP.NET Core Controller
public class UserController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return Ok();
    }
}

// SignalR Hub
public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context == null) throw new InvalidOperationException(""Context is null"");
        await base.OnConnectedAsync();
    }
}

// Entity Framework Repository
public class UserRepository : IUserRepository
{
    private readonly DbContext _context;
    
    public UserRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<User> GetByIdAsync(int id)
    {
        if (id <= 0) throw new ArgumentException(""Invalid ID"");
        return await _context.Set<User>().FindAsync(id);
    }
}

// Middleware
public class CustomMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        // Middleware logic
    }
}

// Filter
public class CustomFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
    }
    
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
    }
}

// Attribute
public class CustomAttribute : Attribute
{
    public CustomAttribute(string value)
    {
        if (string.IsNullOrEmpty(value)) throw new ArgumentException(""Value cannot be null or empty"");
    }
}

// Infrastructure namespace
namespace MyApp.Infrastructure
{
    public class DatabaseService
    {
        public void Connect()
        {
            throw new InvalidOperationException(""Database connection failed"");
        }
    }
}

// Persistence namespace
namespace MyApp.Persistence
{
    public class UnitOfWork
    {
        public void SaveChanges()
        {
            throw new InvalidOperationException(""Save changes failed"");
        }
    }
}

// DataAccess namespace
namespace MyApp.DataAccess
{
    public class DataService
    {
        public void ProcessData()
        {
            throw new InvalidOperationException(""Data processing failed"");
        }
    }
}

// Domain namespace
namespace MyApp.Domain
{
    public class OrderValidator
    {
        public void Validate(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (order.Amount <= 0) throw new InvalidOrderException(""Amount must be positive"");
        }
    }
    
    public class OrderFactory
    {
        public Order Create(string customerId, decimal amount)
        {
            if (string.IsNullOrEmpty(customerId)) 
                throw new ArgumentException(""Customer ID required"");
            if (amount <= 0) 
                throw new ArgumentException(""Amount must be positive"");
                
            return new Order(customerId, amount);
        }
    }
}

// Business namespace
namespace MyApp.Business
{
    public class OrderRule
    {
        public void Apply(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
        }
    }
}

// Configuration namespace
namespace MyApp.Configuration
{
    public class AppConfig
    {
        public void Validate()
        {
            throw new InvalidOperationException(""Configuration validation failed"");
        }
    }
}

// Settings namespace
namespace MyApp.Settings
{
    public class DatabaseSettings
    {
        public void Validate()
        {
            throw new InvalidOperationException(""Database settings validation failed"");
        }
    }
}

// Supporting types
public class CreateRequest { }
public class User { }
public class Order { public decimal Amount { get; set; } }
public class InvalidOrderException : Exception { public InvalidOrderException(string message) : base(message) { } }
public interface IUserRepository { }
public interface IActionFilter { }
public class ActionExecutingContext { }
public class ActionExecutedContext { }
public class HttpContext { }
";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }
}
