using Microsoft.Extensions.Logging;
using NSubstitute;

namespace IndFusion.SemanticRag.Tests.Infratructure.Tests.Shared;

/// <summary>
/// <para>
/// <strong>IITDD (Interface-based Integration Test-Driven Development) Base Class</strong>
/// </para>
/// <para>
/// This sealed, generic base class enforces IITDD principles and Liskov Substitution Principle (LSP).
/// </para>
/// <para>
/// <strong>CRITICAL IITDD PRINCIPLE:</strong> Tests written using this base class validate the <strong>interface contract</strong>,
/// not implementation details. The same test class can validate <strong>ANY valid implementation</strong> of the interface,
/// ensuring all implementations satisfy the contract (Liskov Substitution Principle).
/// </para>
/// <para>
/// <strong>HOW IT ENFORCES LISKOV SUBSTITUTION PRINCIPLE:</strong>
/// </para>
/// <list type="number">
/// <item>Tests use <strong>mocks</strong> to validate interface contracts, not concrete implementations</item>
/// <item>The same test class can validate multiple implementations: QdrantAdapter, InMemoryAdapter, MockAdapter, etc.</item>
/// <item>All implementations must pass the same contract tests - if one fails, it violates LSP</item>
/// <item>Tests assert <strong>contract states</strong> (IsSuccess, IsFailure), not implementation details (RecordCount, ExecutionTime)</item>
/// </list>
/// <para>
/// <strong>EXAMPLE: Reusing Tests Across Implementations</strong>
/// </para>
/// <code>
/// // One test class validates ALL implementations:
/// // 1. IVectorDatabasePortTests validates the interface contract
/// // 2. QdrantVectorDatabaseAdapterTests uses SAME tests with real QdrantAdapter
/// // 3. InMemoryVectorDatabaseAdapterTests uses SAME tests with real InMemoryAdapter
/// // 
/// // If QdrantAdapter fails, but InMemoryAdapter passes - QdrantAdapter violates LSP!
/// </code>
/// <para>
/// <strong>WHY SEALED:</strong> Prevents developers from breaking IITDD patterns by:
/// - Adding implementation-specific assertions
/// - Testing concrete packages instead of port interfaces
/// - Bypassing contract validation
/// </para>
/// </summary>
/// <typeparam name="TInterface">The port interface type to test. Must be an interface.</typeparam>
/// <typeparam name="TRequest">The request type used by the interface method.</typeparam>
/// <typeparam name="TResult">The result type returned by the interface method.</typeparam>
/// <remarks>
/// <para>
/// <strong>IITDD vs TDD - CRITICAL DISTINCTION:</strong>
/// </para>
/// <list type="bullet">
/// <item><strong>IITDD (this class):</strong> Tests interface contracts using mocks. Tests must pass for ANY implementation.</item>
/// <item><strong>TDD (BaseTDDTest):</strong> Tests concrete implementations using real objects. Tests validate specific behavior.</item>
/// </list>
/// <para>
/// <strong>DO NOT confuse IITDD with TDD!</strong> IITDD validates contracts, TDD validates implementations.
/// </para>
/// </remarks>
public abstract class BaseIITDDTest<TInterface, TRequest, TResult>
    where TInterface : class
    where TResult : class
{
    /// <summary>
    /// Gets the mock port interface instance.
    /// <para>
    /// <strong>IITDD Principle:</strong> We mock the interface, NOT the concrete package (e.g., QdrantClient).
    /// This ensures tests validate the contract, not implementation details.
    /// </para>
    /// </summary>
    protected TInterface MockPort { get; private set; } = null!;

    /// <summary>
    /// Gets the logger instance (can be mocked).
    /// </summary>
    protected ILogger<TInterface> Logger { get; private set; } = null!;

    /// <summary>
    /// Initializes test setup. Override this method to customize setup.
    /// <para>
    /// <strong>IITDD Principle:</strong> Always mock the port interface, never the concrete package.
    /// </para>
    /// </summary>
    protected virtual void SetUp()
    {
        MockPort = Substitute.For<TInterface>();
        Logger = Substitute.For<ILogger<TInterface>>();
    }

    /// <summary>
    /// Asserts that a Result&lt;T&gt; is successful and has a non-null value.
    /// <para>
    /// <strong>IITDD Principle:</strong> This validates the contract (success state), not implementation details.
    /// This assertion must pass for ANY valid implementation of the interface (Liskov Substitution Principle).
    /// </para>
    /// <para>
    /// <strong>DO NOT assert:</strong> RecordCount, ExecutionTimeMs, specific values (unless contract requires them).
    /// These are implementation details that can vary between valid implementations.
    /// </para>
    /// </summary>
    /// <param name="result">The result to assert.</param>
    protected void AssertSuccess(Result<TResult> result)
    {
        result.IsSuccess.ShouldBeTrue($"Expected result to be successful, but it failed with error: {result.Error}");
        result.Value.ShouldNotBeNull("Result value should not be null when successful");
    }

    /// <summary>
    /// Asserts that a Result&lt;T&gt; is a failure with an optional error code check.
    /// </summary>
    /// <param name="result">The result to assert.</param>
    /// <param name="errorCode">Optional error code to verify. If null, only checks for failure state.</param>
    protected void AssertFailure(Result<TResult> result, string? errorCode = null)
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
    protected void AssertSuccess(Result result)
    {
        result.IsSuccess.ShouldBeTrue($"Expected result to be successful, but it failed with error: {result.Error}");
    }

    /// <summary>
    /// Asserts that a non-generic Result is a failure with an optional error code check.
    /// </summary>
    /// <param name="result">The result to assert.</param>
    /// <param name="errorCode">Optional error code to verify. If null, only checks for failure state.</param>
    protected void AssertFailure(Result result, string? errorCode = null)
    {
        result.IsFailure.ShouldBeTrue("Expected result to be a failure, but it was successful");
        result.Error.ShouldNotBeNullOrEmpty("Result error should not be null or empty when failed");

        if (!string.IsNullOrEmpty(errorCode))
        {
            result.Error.ShouldBe(errorCode, $"Expected error code '{errorCode}', but got '{result.Error}'");
        }
    }

    /// <summary>
    /// Creates a valid request for testing. Override this method to provide test-specific request creation.
    /// </summary>
    /// <returns>A valid request instance.</returns>
    protected virtual TRequest CreateValidRequest()
    {
        throw new NotImplementedException("Override CreateValidRequest() to provide test-specific request creation.");
    }

    /// <summary>
    /// Creates an invalid request for testing. Override this method to provide test-specific invalid request creation.
    /// </summary>
    /// <returns>An invalid request instance.</returns>
    protected virtual TRequest CreateInvalidRequest()
    {
        throw new NotImplementedException("Override CreateInvalidRequest() to provide test-specific invalid request creation.");
    }

    /// <summary>
    /// Sets up the mock port to return a successful result.
    /// </summary>
    /// <param name="returnValue">The value to return on success.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    protected void SetupSuccess(TResult returnValue, CancellationToken cancellationToken = default)
    {
        // This is a placeholder - derived classes should override or use specific mock setup
        // based on their interface methods
    }

    /// <summary>
    /// Sets up the mock port to return a failure result.
    /// </summary>
    /// <param name="errorCode">The error code to return on failure.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    protected void SetupFailure(string errorCode, CancellationToken cancellationToken = default)
    {
        // This is a placeholder - derived classes should override or use specific mock setup
        // based on their interface methods
    }
}
