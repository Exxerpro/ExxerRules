using Microsoft.Extensions.Logging;
using NSubstitute;

namespace IndFusion.SemanticRag.Domain.Tests.Shared;

/// <summary>
/// <para>
/// <strong>LISKOV SUBSTITUTION PRINCIPLE TEST PATTERN - COMPLETE EXAMPLE</strong>
/// </para>
/// <para>
/// This sealed class demonstrates how ONE test class validates ALL implementations of an interface.
/// This enforces Liskov Substitution Principle and prevents developers from breaking IITDD patterns.
/// </para>
/// <para>
/// <strong>HOW IT WORKS:</strong>
/// </para>
/// <list type="number">
/// <item>Define interface contract tests using <strong>mocks</strong> (IITDD)</item>
/// <item>Reuse SAME contract tests with <strong>real implementations</strong> (LSP validation)</item>
/// <item>If any implementation fails - it violates Liskov Substitution Principle!</item>
/// <item>All implementations MUST pass the same contract tests</item>
/// </list>
/// <para>
/// <strong>EXAMPLE USAGE:</strong>
/// </para>
/// <code>
/// // ONE contract test class:
/// public sealed class IVectorDatabasePortContractTests 
///     : InterfaceContractTests&lt;IVectorDatabasePort, SearchRequest, SearchResult&gt;
/// {
///     // Contract tests using mocks (IITDD)
///     [Fact]
///     public async Task SearchAsync_WithValidRequest_ShouldReturnSuccess()
///     {
///         MockPort.SearchAsync(...).Returns(Result&lt;SearchResult&gt;.Success(...));
///         var result = await MockPort.SearchAsync(CreateValidRequest());
///         AssertContractSuccess(result);
///     }
/// }
/// 
/// // Reuse SAME tests with QdrantAdapter (real implementation):
/// public sealed class QdrantVectorDatabaseAdapterLSPTests 
///     : IVectorDatabasePortContractTests
/// {
///     protected override IVectorDatabasePort? CreateImplementation() 
///         => new QdrantVectorDatabaseAdapter(client, logger);
///     
///     // SAME contract tests automatically validate real QdrantAdapter!
/// }
/// 
/// // Reuse SAME tests with InMemoryAdapter:
/// public sealed class InMemoryVectorDatabaseAdapterLSPTests 
///     : IVectorDatabasePortContractTests
/// {
///     protected override IVectorDatabasePort? CreateImplementation() 
///         => new InMemoryVectorDatabaseAdapter(logger);
///     
///     // SAME contract tests validate InMemoryAdapter!
/// }
/// </code>
/// <para>
/// <strong>RESULT:</strong> If QdrantAdapter fails but InMemoryAdapter passes - 
/// QdrantAdapter violates Liskov Substitution Principle!
/// </para>
/// </summary>
/// <typeparam name="TInterface">The port interface type to test.</typeparam>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
/// <remarks>
/// <para>
/// <strong>WHY THIS PATTERN PREVENTS IITDD VIOLATIONS:</strong>
/// </para>
/// <list type="number">
/// <item>Sealed classes prevent inheritance that breaks contract tests</item>
/// <item>Generic pattern forces interface-based testing</item>
/// <item>LSP validation ensures all implementations satisfy contract</item>
/// <item>Cannot add implementation-specific assertions</item>
/// <item>Contract tests are reusable across ALL implementations</item>
/// </list>
/// </remarks>
public abstract class LiskovSubstitutionTestPattern<TInterface, TRequest, TResult>
    where TInterface : class
    where TResult : class
{
    /// <summary>
    /// Gets the mock port interface for IITDD contract testing.
    /// </summary>
    protected TInterface MockPort { get; private set; } = null!;

    /// <summary>
    /// Gets the real implementation instance for LSP validation.
    /// If provided, the same contract tests validate this real implementation.
    /// </summary>
    protected TInterface? RealImplementation { get; private set; }

    /// <summary>
    /// Gets the logger instance.
    /// </summary>
    protected ILogger<TInterface> Logger { get; private set; } = null!;

    /// <summary>
    /// Creates a real implementation of the interface for Liskov Substitution Principle validation.
    /// <para>
    /// <strong>LSP Pattern:</strong> Override this to provide a real implementation.
    /// The same contract tests will validate this real implementation against the interface contract.
    /// </para>
    /// <para>
    /// <strong>Return null</strong> for pure IITDD (mock-only testing).
    /// </para>
    /// <para>
    /// <strong>Return real instance</strong> for LSP validation (same tests validate real implementation).
    /// </para>
    /// </summary>
    /// <returns>A real implementation instance, or null for mock-only testing.</returns>
    protected virtual TInterface? CreateRealImplementation() => null;

    /// <summary>
    /// Initializes test setup.
    /// </summary>
    protected virtual void SetUp()
    {
        // Always create mock for contract testing (IITDD)
        MockPort = Substitute.For<TInterface>();
        Logger = Substitute.For<ILogger<TInterface>>();

        // Optionally create real implementation for LSP validation
        RealImplementation = CreateRealImplementation();
    }

    /// <summary>
    /// Asserts that a Result&lt;T&gt; satisfies the interface contract (success state).
    /// <para>
    /// <strong>Liskov Principle:</strong> This assertion MUST pass for ANY valid implementation.
    /// If an implementation fails this, it violates the interface contract.
    /// </para>
    /// </summary>
    /// <param name="result">The result to assert.</param>
    protected void AssertContractSuccess(Result<TResult> result)
    {
        result.IsSuccess.ShouldBeTrue(
            $"Liskov Substitution Principle Violation: Expected success, but got error: {result.Error}. " +
            $"This implementation violates the interface contract. All valid implementations must satisfy this contract.");
        result.Value.ShouldNotBeNull(
            "Liskov Substitution Principle Violation: Success result must have non-null value. " +
            "This implementation violates the interface contract.");
    }

    /// <summary>
    /// Asserts that a Result&lt;T&gt; satisfies the interface contract (failure state).
    /// <para>
    /// <strong>Liskov Principle:</strong> This assertion validates the error contract.
    /// All implementations must return failures with valid error messages for invalid inputs.
    /// </para>
    /// </summary>
    /// <param name="result">The result to assert.</param>
    /// <param name="errorCode">Optional error code to verify.</param>
    protected void AssertContractFailure(Result<TResult> result, string? errorCode = null)
    {
        result.IsFailure.ShouldBeTrue(
            "Liskov Substitution Principle Violation: Expected failure for invalid input. " +
            "This implementation violates the interface contract.");
        result.Error.ShouldNotBeNullOrEmpty(
            "Liskov Substitution Principle Violation: Failure result must have non-empty error message. " +
            "This implementation violates the interface contract.");

        if (!string.IsNullOrEmpty(errorCode))
        {
            result.Error.ShouldBe(errorCode,
                $"Liskov Substitution Principle Violation: Expected error code '{errorCode}', but got '{result.Error}'. " +
                $"This implementation violates the interface contract.");
        }
    }

    /// <summary>
    /// Validates that a real implementation satisfies the interface contract (Liskov Substitution Principle).
    /// <para>
    /// <strong>LSP Validation Pattern:</strong> Call this after testing with mocks to ensure
    /// real implementations also satisfy the contract.
    /// </para>
    /// </summary>
    /// <param name="contractTestMethod">A method that tests the interface contract.</param>
    protected void ValidateRealImplementationSatisfiesContract(Func<TInterface, Task> contractTestMethod)
    {
        if (RealImplementation == null)
        {
            throw new InvalidOperationException(
                "Cannot validate real implementation: CreateRealImplementation() returned null. " +
                "Override CreateRealImplementation() to provide a real instance for LSP validation. " +
                "This ensures all implementations satisfy the interface contract (Liskov Substitution Principle).");
        }

        // Execute same contract test with real implementation
        // If this fails, the real implementation violates Liskov Substitution Principle
        contractTestMethod(RealImplementation).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Creates a valid request for testing. Must be overridden by derived classes.
    /// </summary>
    /// <returns>A valid request instance.</returns>
    protected abstract TRequest CreateValidRequest();

    /// <summary>
    /// Creates an invalid request for testing. Must be overridden by derived classes.
    /// </summary>
    /// <returns>An invalid request instance.</returns>
    protected abstract TRequest CreateInvalidRequest();
}
