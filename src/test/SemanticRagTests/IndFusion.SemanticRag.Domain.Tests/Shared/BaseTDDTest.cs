using IndFusion.SemanticRag.Domain.Tests.Helpers;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.Logging;

namespace IndFusion.SemanticRag.Domain.Tests.Shared;

/// <summary>
/// Base class for TDD (Test-Driven Development) tests.
/// Provides common setup and assertion helpers for testing concrete implementations.
/// </summary>
/// <typeparam name="TImplementation">The implementation type to test.</typeparam>
public abstract class BaseTDDTest<TImplementation>
    where TImplementation : class
{
    private readonly ITestOutputHelper? _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTDDTest{TImplementation}"/> class.
    /// </summary>
    /// <param name="output">Optional test output helper for logging. If provided, enables Meziantou XUnit logger.</param>
    protected BaseTDDTest(ITestOutputHelper? output = null)
    {
        _output = output;
        SetUp();
    }

    /// <summary>
    /// Gets the implementation instance under test.
    /// </summary>
    protected TImplementation Implementation { get; private set; } = null!;

    /// <summary>
    /// Gets the logger instance using Meziantou XUnit logger if output is available, otherwise NullLogger.
    /// </summary>
    protected ILogger<TImplementation> Logger { get; private set; } = null!;

    /// <summary>
    /// Gets the SUT tracer instance for method entry/exit logging with correlation IDs.
    /// </summary>
    protected SUTTracer Tracer { get; private set; } = null!;

    /// <summary>
    /// Initializes test setup. Override this method to customize setup.
    /// </summary>
    protected virtual void SetUp()
    {
        // Create logger first so it's available in CreateImplementation()
        Logger = CreateLogger();
        Tracer = new SUTTracer(_output); // ✅ Phase 2.3: Integrate SUTTracer for method tracing
        Implementation = CreateImplementation();
    }

    /// <summary>
    /// Creates the implementation instance. Must be overridden by derived classes.
    /// </summary>
    /// <returns>The implementation instance.</returns>
    protected abstract TImplementation CreateImplementation();

    /// <summary>
    /// Creates a logger instance using Meziantou XUnit logger if output is available, otherwise NullLogger.
    /// Override to provide custom logger creation.
    /// </summary>
    /// <returns>The logger instance.</returns>
    protected virtual ILogger<TImplementation> CreateLogger()
    {
        // ✅ Use Meziantou XUnit logger if output is available (per project standards)
        // See: docs/Tasks/Task(Replace ILogger with Meziantou Xunit Logger).md
        if (_output != null)
        {
            return XUnitLogger.CreateLogger<TImplementation>(_output);
        }
        
        // Fallback to NullLogger if no output helper is provided
        return Microsoft.Extensions.Logging.Abstractions.NullLogger<TImplementation>.Instance;
    }

    /// <summary>
    /// Asserts that a Result&lt;T&gt; is successful and has a non-null value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to assert.</param>
    protected void AssertResultSuccess<T>(Result<T> result)
    {
        result.IsSuccess.ShouldBeTrue($"Expected result to be successful, but it failed with error: {result.Error}");
        
        // Only check for null if T is a reference type (value types can't be null)
        if (!typeof(T).IsValueType)
        {
            ((object?)result.Value).ShouldNotBeNull("Result value should not be null when successful");
        }
    }

    /// <summary>
    /// Asserts that a Result&lt;T&gt; is successful with a specific value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to assert.</param>
    /// <param name="expectedValue">The expected value.</param>
    protected void AssertResultSuccess<T>(Result<T> result, T expectedValue)
    {
        AssertResultSuccess(result);
        result.Value.ShouldBe(expectedValue);
    }

    /// <summary>
    /// Asserts that a Result&lt;T&gt; is a failure with an optional error code check.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to assert.</param>
    /// <param name="errorCode">Optional error code to verify. If null, only checks for failure state.</param>
    protected void AssertResultFailure<T>(Result<T> result, string? errorCode = null)
    {
        result.IsFailure.ShouldBeTrue("Expected result to be a failure, but it was successful");
        result.Error.ShouldNotBeNullOrEmpty("Result error should not be null or empty when failed");

        if (!string.IsNullOrEmpty(errorCode))
        {
            result.Error.ShouldBe(errorCode, $"Expected error code '{errorCode}', but got '{result.Error}'");
        }
    }

    /// <summary>
    /// Asserts that a non-generic Result is successful.
    /// </summary>
    /// <param name="result">The result to assert.</param>
    protected void AssertResultSuccess(Result result)
    {
        result.IsSuccess.ShouldBeTrue($"Expected result to be successful, but it failed with error: {result.Error}");
    }

    /// <summary>
    /// Asserts that a non-generic Result is a failure with an optional error code check.
    /// </summary>
    /// <param name="result">The result to assert.</param>
    /// <param name="errorCode">Optional error code to verify. If null, only checks for failure state.</param>
    protected void AssertResultFailure(Result result, string? errorCode = null)
    {
        result.IsFailure.ShouldBeTrue("Expected result to be a failure, but it was successful");
        result.Error.ShouldNotBeNullOrEmpty("Result error should not be null or empty when failed");

        if (!string.IsNullOrEmpty(errorCode))
        {
            result.Error.ShouldBe(errorCode, $"Expected error code '{errorCode}', but got '{result.Error}'");
        }
    }
}
