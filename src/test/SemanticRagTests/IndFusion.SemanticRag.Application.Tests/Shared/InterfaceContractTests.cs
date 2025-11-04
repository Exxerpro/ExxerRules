using Microsoft.Extensions.Logging;
using NSubstitute;

namespace IndFusion.SemanticRag.Application.Tests.Shared;

/// <summary>
/// <para>
/// <strong>SEALED Generic Interface Contract Test Base Class</strong>
/// </para>
/// <para>
/// This sealed, generic class enforces IITDD principles and ensures Liskov Substitution Principle compliance.
/// The same test class can validate <strong>ALL implementations</strong> of an interface.
/// </para>
/// <para>
/// <strong>CRITICAL LISKOV SUBSTITUTION PRINCIPLE PATTERN:</strong>
/// </para>
/// <para>
/// This pattern allows you to write <strong>ONE test class</strong> that validates <strong>ANY implementation</strong>
/// of an interface. All implementations must satisfy the same contract.
/// </para>
/// <para>
/// <strong>Example Usage Pattern:</strong>
/// </para>
/// <code>
/// // Step 1: Define sealed interface contract tests
/// public sealed class IVectorDatabasePortContractTests 
///     : InterfaceContractTests&lt;IVectorDatabasePort, SearchRequest, SearchResult&gt;
/// {
///     // Contract tests using mocks
/// }
/// 
/// // Step 2: Use SAME contract tests with REAL implementations
/// public sealed class QdrantVectorDatabaseAdapterTests 
///     : InterfaceContractTests&lt;IVectorDatabasePort, SearchRequest, SearchResult&gt;
/// {
///     // Reuse contract tests but provide real implementation factory
///     protected override IVectorDatabasePort CreateImplementation() 
///         => new QdrantVectorDatabaseAdapter(client, logger);
/// }
/// 
/// public sealed class InMemoryVectorDatabaseAdapterTests 
///     : InterfaceContractTests&lt;IVectorDatabasePort, SearchRequest, SearchResult&gt;
/// {
///     // Same contract tests validate InMemoryAdapter
///     protected override IVectorDatabasePort CreateImplementation() 
///         => new InMemoryVectorDatabaseAdapter(logger);
/// }
/// 
/// // RESULT: If QdrantAdapter fails but InMemoryAdapter passes - 
/// // QdrantAdapter violates Liskov Substitution Principle!
/// </code>
/// <para>
/// <strong>WHY SEALED:</strong> Prevents inheritance that could break IITDD patterns.
/// Forces developers to use the contract-based approach correctly.
/// </para>
/// <para>
/// <strong>WHY THIS PREVENTS IITDD VIOLATIONS:</strong>
/// </para>
/// <list type="number">
/// <item>Sealed class cannot be inherited to bypass contract tests</item>
/// <item>Generic pattern enforces interface-based testing</item>
/// <item>Contract tests must pass for ALL implementations</item>
/// <item>Cannot add implementation-specific assertions</item>
/// </list>
/// </summary>
/// <typeparam name="TInterface">
/// The port interface type to test. MUST be an interface.
/// All implementations of this interface will be validated by the same contract tests.
/// </typeparam>
/// <typeparam name="TRequest">The request type used by the interface method.</typeparam>
/// <typeparam name="TResult">The result type returned by the interface method.</typeparam>
/// <remarks>
/// <para>
/// <strong>IITDD Contract Testing Rules:</strong>
/// </para>
/// <list type="number">
/// <item>✅ Test interface contract (IsSuccess, IsFailure, ErrorCode)</item>
/// <item>✅ Tests must pass for ANY valid implementation</item>
/// <item>✅ Use mocks for IITDD tests (this validates the contract)</item>
/// <item>✅ Use real implementations for TDD tests (this validates behavior)</item>
/// <item>❌ DO NOT test implementation details (RecordCount, ExecutionTimeMs, etc.)</item>
/// <item>❌ DO NOT test specific values unless contract requires them</item>
/// <item>❌ DO NOT mock concrete packages (QdrantClient, IDriver, etc.)</item>
/// </list>
/// </remarks>
public abstract class InterfaceContractTests<TInterface, TRequest, TResult>
    where TInterface : class
    where TResult : class
{
    /// <summary>
    /// Gets the mock port interface instance for IITDD contract testing.
    /// <para>
    /// <strong>IITDD Principle:</strong> Use mocks to test interface contracts.
    /// </para>
    /// </summary>
    protected TInterface MockPort { get; private set; } = null!;

    /// <summary>
    /// Gets a real implementation instance for Liskov Substitution Principle validation.
    /// <para>
    /// <strong>LSP Pattern:</strong> If provided, same contract tests validate real implementations.
    /// This ensures all implementations satisfy the interface contract.
    /// </para>
    /// </summary>
    protected TInterface? RealImplementation { get; private set; }

    /// <summary>
    /// Gets the logger instance.
    /// </summary>
    protected ILogger<TInterface> Logger { get; private set; } = null!;

    /// <summary>
    /// Creates a real implementation of the interface for LSP validation.
    /// <para>
    /// <strong>Liskov Substitution Pattern:</strong> Override this to provide a real implementation.
    /// The same contract tests will validate this real implementation against the interface contract.
    /// </para>
    /// <para>
    /// <strong>If null:</strong> Only mock-based contract testing is performed (pure IITDD).
    /// </para>
    /// <para>
    /// <strong>If provided:</strong> Same contract tests validate the real implementation (LSP compliance).
    /// </para>
    /// </summary>
    /// <returns>A real implementation instance, or null for mock-only testing.</returns>
    /// <example>
    /// <code>
    /// // Pure IITDD (mock only):
    /// protected override IVectorDatabasePort? CreateImplementation() => null;
    /// 
    /// // LSP validation (mock + real):
    /// protected override IVectorDatabasePort? CreateImplementation() 
    ///     => new QdrantVectorDatabaseAdapter(client, logger);
    /// </code>
    /// </example>
    protected virtual TInterface? CreateImplementation() => null;

    /// <summary>
    /// Initializes test setup. Override to customize setup.
    /// </summary>
    protected virtual void SetUp()
    {
        // Always create mock for contract testing
        MockPort = Substitute.For<TInterface>();
        Logger = Substitute.For<ILogger<TInterface>>();

        // Optionally create real implementation for LSP validation
        RealImplementation = CreateImplementation();
    }

    /// <summary>
    /// Asserts that a Result&lt;T&gt; is successful (contract validation).
    /// <para>
    /// <strong>Liskov Principle:</strong> This assertion must pass for ANY valid implementation.
    /// If an implementation fails this assertion, it violates the interface contract.
    /// </para>
    /// </summary>
    /// <param name="result">The result to assert.</param>
    protected void AssertContractSuccess(Result<TResult> result)
    {
        result.IsSuccess.ShouldBeTrue(
            $"Contract violation: Expected success, but got error: {result.Error}. " +
            $"All valid implementations must satisfy this contract (Liskov Substitution Principle).");
        result.Value.ShouldNotBeNull(
            "Contract violation: Success result must have non-null value. " +
            "This is a contract requirement that all implementations must satisfy.");
    }

    /// <summary>
    /// Asserts that a Result&lt;T&gt; is a failure with optional error code (contract validation).
    /// <para>
    /// <strong>Liskov Principle:</strong> This assertion validates the error contract.
    /// All implementations must return failures with valid error messages when inputs are invalid.
    /// </para>
    /// </summary>
    /// <param name="result">The result to assert.</param>
    /// <param name="errorCode">Optional error code to verify.</param>
    protected void AssertContractFailure(Result<TResult> result, string? errorCode = null)
    {
        result.IsFailure.ShouldBeTrue(
            "Contract violation: Expected failure for invalid input. " +
            "All implementations must return failure for invalid inputs (contract requirement).");
        result.Error.ShouldNotBeNullOrEmpty(
            "Contract violation: Failure result must have non-empty error message. " +
            "This is a contract requirement that all implementations must satisfy.");

        if (!string.IsNullOrEmpty(errorCode))
        {
            result.Error.ShouldBe(errorCode,
                $"Contract violation: Expected error code '{errorCode}', but got '{result.Error}'. " +
                $"All implementations must use consistent error codes (contract requirement).");
        }
    }

    /// <summary>
    /// Validates that a real implementation satisfies the interface contract (Liskov Substitution Principle).
    /// <para>
    /// <strong>LSP Validation Pattern:</strong> Call this after testing with mocks to ensure
    /// real implementations also satisfy the contract.
    /// </para>
    /// </summary>
    /// <param name="contractTestMethod">A method that tests the interface contract using the provided implementation.</param>
    /// <example>
    /// <code>
    /// [Fact]
    /// public async Task SearchAsync_RealImplementation_SatisfiesContract()
    /// {
    ///     // First test with mock (IITDD contract test)
    ///     await SearchAsync_WithValidRequest_ShouldReturnSuccess();
    ///     
    ///     // Then validate real implementation satisfies same contract (LSP)
    ///     ValidateRealImplementationSatisfiesContract(async impl =>
    ///     {
    ///         var result = await impl.SearchAsync(validRequest);
    ///         AssertContractSuccess(result);
    ///     });
    /// }
    /// </code>
    /// </example>
    protected void ValidateRealImplementationSatisfiesContract(Func<TInterface, Task> contractTestMethod)
    {
        if (RealImplementation == null)
        {
            throw new InvalidOperationException(
                "Cannot validate real implementation: CreateImplementation() returned null. " +
                "Override CreateImplementation() to provide a real instance for LSP validation.");
        }

        // Execute same contract test with real implementation
        // If this fails, the real implementation violates Liskov Substitution Principle
        contractTestMethod(RealImplementation).Wait();
    }

    /// <summary>
    /// Creates a valid request for testing. Override to provide test-specific request creation.
    /// </summary>
    /// <returns>A valid request instance.</returns>
    protected abstract TRequest CreateValidRequest();

    /// <summary>
    /// Creates an invalid request for testing. Override to provide test-specific invalid request creation.
    /// </summary>
    /// <returns>An invalid request instance.</returns>
    protected abstract TRequest CreateInvalidRequest();
}
