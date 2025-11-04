# Liskov Substitution Principle Test Pattern - Complete Example

## Overview

This document demonstrates how **ONE test class validates ALL implementations** of an interface, enforcing Liskov Substitution Principle and preventing IITDD violations.

## The Problem You're Solving

**Issue**: Developers don't understand IITDD vs TDD, think IITDD is an antipattern, and destroy IITDD patterns during implementation.

**Solution**: Use **sealed, generic test classes** that enforce Liskov Substitution Principle. Same contract tests validate ALL implementations.

---

## Pattern: One Test Class for All Implementations

### Step 1: Define Interface Contract Tests (IITDD)

```csharp
// File: Interfaces/IVectorDatabasePortContractTests.cs
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Tests.Unit.Shared;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Interfaces;

/// <summary>
/// SEALED interface contract tests for IVectorDatabasePort.
/// 
/// CRITICAL: These tests validate the INTERFACE CONTRACT, not implementation details.
/// The SAME contract tests validate ALL implementations (QdrantAdapter, InMemoryAdapter, etc.).
/// 
/// LISKOV SUBSTITUTION PRINCIPLE: If any implementation fails these tests,
/// it violates the interface contract!
/// </summary>
public sealed class IVectorDatabasePortContractTests 
    : InterfaceContractTests<IVectorDatabasePort, SearchRequest, SearchResult>
{
    // Pure IITDD - no real implementation (mock only)
    protected override IVectorDatabasePort? CreateImplementation() => null;

    protected override SearchRequest CreateValidRequest()
    {
        return new SearchRequest(
            CollectionName: "test-collection",
            QueryVector: new float[] { 0.1f, 0.2f, 0.3f },
            Limit: 10,
            ScoreThreshold: 0.7f);
    }

    protected override SearchRequest CreateInvalidRequest()
    {
        return new SearchRequest(
            CollectionName: null!, // Invalid
            QueryVector: Array.Empty<float>(),
            Limit: 0,
            ScoreThreshold: -1.0f);
    }

    [Fact]
    public async Task SearchAsync_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange: Setup mock to return success (contract test)
        MockPort.SearchAsync(Arg.Any<SearchRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<SearchResult>.Success(new SearchResult(/* ... */)));

        // Act: Test interface contract
        var result = await MockPort.SearchAsync(CreateValidRequest());

        // Assert: Validate contract (NOT implementation details)
        AssertContractSuccess(result);
        
        // ❌ DO NOT assert: result.RecordCount, result.ExecutionTimeMs (implementation details)
    }

    [Fact]
    public async Task SearchAsync_WithInvalidRequest_ShouldReturnFailure()
    {
        // Arrange: Setup mock to return failure for invalid input (contract test)
        MockPort.SearchAsync(Arg.Any<SearchRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<SearchResult>.WithFailure(ErrorCodes.InvalidRequest));

        // Act: Test error contract
        var result = await MockPort.SearchAsync(CreateInvalidRequest());

        // Assert: Validate error contract
        AssertContractFailure(result, ErrorCodes.InvalidRequest);
    }
}
```

### Step 2: Validate Real Implementations Using SAME Contract Tests

```csharp
// File: Implementations/QdrantVectorDatabaseAdapterLSPTests.cs
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Adapters;
using IndFusion.SemanticRag.Tests.Unit.Interfaces;
using Qdrant.Client;

namespace IndFusion.SemanticRag.Tests.Unit.Implementations;

/// <summary>
/// SEALED LSP validation tests for QdrantVectorDatabaseAdapter.
/// 
/// CRITICAL: This class REUSES the same contract tests from IVectorDatabasePortContractTests.
/// If QdrantAdapter fails these tests, it violates Liskov Substitution Principle!
/// </summary>
public sealed class QdrantVectorDatabaseAdapterLSPTests 
    : IVectorDatabasePortContractTests
{
    // Override to provide REAL QdrantAdapter for LSP validation
    protected override IVectorDatabasePort? CreateImplementation() 
    {
        var client = new QdrantClient("localhost", 6333);
        var logger = Substitute.For<ILogger<QdrantVectorDatabaseAdapter>>();
        return new QdrantVectorDatabaseAdapter(client, logger);
    }

    // SAME contract tests from base class automatically validate QdrantAdapter!
    // If QdrantAdapter fails - it violates LSP!
}

// File: Implementations/InMemoryVectorDatabaseAdapterLSPTests.cs
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Tests.Unit.Interfaces;

namespace IndFusion.SemanticRag.Tests.Unit.Implementations;

/// <summary>
/// SEALED LSP validation tests for InMemoryVectorDatabaseAdapter.
/// 
/// CRITICAL: This class REUSES the same contract tests from IVectorDatabasePortContractTests.
/// If InMemoryAdapter fails these tests, it violates Liskov Substitution Principle!
/// </summary>
public sealed class InMemoryVectorDatabaseAdapterLSPTests 
    : IVectorDatabasePortContractTests
{
    // Override to provide REAL InMemoryAdapter for LSP validation
    protected override IVectorDatabasePort? CreateImplementation() 
    {
        var logger = Substitute.For<ILogger<InMemoryVectorDatabaseAdapter>>();
        return new InMemoryVectorDatabaseAdapter(logger);
    }

    // SAME contract tests from base class automatically validate InMemoryAdapter!
    // If InMemoryAdapter fails - it violates LSP!
}
```

---

## What This Achieves

### ✅ Prevents IITDD Violations

1. **Sealed classes** prevent developers from breaking contract tests
2. **Generic pattern** forces interface-based testing
3. **LSP validation** ensures all implementations satisfy contract
4. **Same tests** validate ALL implementations

### ✅ Enforces Liskov Substitution Principle

- If QdrantAdapter passes, but InMemoryAdapter fails → **InMemoryAdapter violates LSP**
- If InMemoryAdapter passes, but QdrantAdapter fails → **QdrantAdapter violates LSP**
- All implementations MUST pass the same contract tests

### ✅ Makes IITDD Clear

- **One contract test class** validates interface contract
- **Multiple LSP test classes** validate real implementations
- **Same contract tests** used everywhere
- **Clear separation** between IITDD (contract) and TDD (behavior)

---

## Pattern Benefits

1. **Prevents Confusion**: Clear separation between IITDD (contract) and TDD (behavior)
2. **Enforces LSP**: Same tests validate all implementations
3. **Prevents Violations**: Sealed classes prevent breaking contract tests
4. **Reusable Tests**: One contract test class validates multiple implementations
5. **Clear Documentation**: Comments explain Liskov Principle and IITDD principles

---

## Usage Summary

```csharp
// 1. Define contract tests (IITDD) - ONCE
public sealed class IVectorDatabasePortContractTests 
    : InterfaceContractTests<IVectorDatabasePort, SearchRequest, SearchResult>
{
    // Contract tests using mocks
}

// 2. Validate ALL implementations with SAME tests
public sealed class QdrantAdapterLSPTests : IVectorDatabasePortContractTests { }
public sealed class InMemoryAdapterLSPTests : IVectorDatabasePortContractTests { }
public sealed class MockAdapterLSPTests : IVectorDatabasePortContractTests { }

// RESULT: All implementations validated by same contract tests!
// If one fails - it violates Liskov Substitution Principle!
```

---

**This pattern ensures developers cannot destroy IITDD patterns - the sealed, generic structure enforces contract-based testing!**
