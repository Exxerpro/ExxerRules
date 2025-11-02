using System;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace IndFusion.SemanticRag.Tests.Unit.Shared;

/// <summary>
/// Base class for TDD (Test-Driven Development) tests.
/// Provides common setup and assertion helpers for testing concrete implementations.
/// </summary>
/// <typeparam name="TImplementation">The implementation type to test.</typeparam>
public abstract class BaseTDDTest<TImplementation>
    where TImplementation : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTDDTest{TImplementation}"/> class.
    /// Calls SetUp() to initialize the implementation instance.
    /// </summary>
    protected BaseTDDTest()
    {
        SetUp();
    }

    /// <summary>
    /// Gets the implementation instance under test.
    /// </summary>
    protected TImplementation Implementation { get; private set; } = null!;

    /// <summary>
    /// Gets the logger instance (can be mocked).
    /// </summary>
    protected ILogger<TImplementation> Logger { get; private set; } = null!;

    /// <summary>
    /// Initializes test setup. Override this method to customize setup.
    /// </summary>
    protected virtual void SetUp()
    {
        Implementation = CreateImplementation();
        Logger = CreateLogger();
    }

    /// <summary>
    /// Creates the implementation instance. Must be overridden by derived classes.
    /// </summary>
    /// <returns>The implementation instance.</returns>
    protected abstract TImplementation CreateImplementation();

    /// <summary>
    /// Creates a logger instance. Override to provide custom logger creation.
    /// </summary>
    /// <returns>The logger instance.</returns>
    protected virtual ILogger<TImplementation> CreateLogger()
    {
        // Default implementation - can be overridden
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
