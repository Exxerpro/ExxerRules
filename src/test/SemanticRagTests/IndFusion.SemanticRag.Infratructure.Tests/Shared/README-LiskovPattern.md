# Liskov Substitution Principle Test Pattern - Implementation Guide

## 🎯 The Problem This Pattern Solves

**Issue**: Developers don't understand IITDD vs TDD, think IITDD is an antipattern, and destroy IITDD patterns during implementation by:
- Testing implementation details instead of contracts
- Mocking concrete packages instead of port interfaces
- Creating separate test classes for each implementation
- Breaking contract tests with implementation-specific assertions

**Solution**: **Sealed, Generic Test Pattern** that enforces Liskov Substitution Principle.

---

## ✅ The Pattern: One Test Class Validates ALL Implementations

### How It Works

1. **Define Interface Contract Tests ONCE** (using mocks - IITDD)
2. **Reuse SAME Contract Tests** with ALL implementations (LSP validation)
3. **If any implementation fails** → It violates Liskov Substitution Principle!

### Visual Pattern

```
┌─────────────────────────────────────────────────────────┐
│  Interface Contract Tests (IITDD)                       │
│  └─ IVectorDatabasePortContractTests                    │
│     └─ Contract tests using MOCKS                       │
│        ✅ SearchAsync_WithValidRequest_ShouldReturnSuccess │
│        ✅ SearchAsync_WithInvalidRequest_ShouldReturnFailure│
└─────────────────────────────────────────────────────────┘
                    │
                    │ (Inherits and reuses contract tests)
                    │
        ┌───────────┼───────────┐
        │           │           │
        ▼           ▼           ▼
┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│ QdrantAdapter│ │ InMemoryAdapter│ │ MockAdapter  │
│ LSP Tests    │ │ LSP Tests     │ │ LSP Tests    │
└──────────────┘ └──────────────┘ └──────────────┘
     │                  │                  │
     └──────────────────┼──────────────────┘
                        │
            ┌───────────▼───────────┐
            │  SAME Contract Tests │
            │  Validate ALL        │
            │  Implementations!    │
            └──────────────────────┘
```

---

## 📋 Implementation Steps

### Step 1: Create Interface Contract Tests (IITDD)

**File**: `Interfaces/IVectorDatabasePortContractTests.cs`

```csharp
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Tests.Unit.Shared;

namespace IndFusion.SemanticRag.Tests.Unit.Interfaces;

/// <summary>
/// SEALED interface contract tests for IVectorDatabasePort.
/// 
/// CRITICAL: These tests validate the INTERFACE CONTRACT, not implementation details.
/// The SAME contract tests validate ALL implementations (Liskov Substitution Principle).
/// </summary>
public sealed class IVectorDatabasePortContractTests 
    : InterfaceContractTests<IVectorDatabasePort, string, CollectionInfo?>
{
    protected override IVectorDatabasePort? CreateImplementation() => null; // Pure IITDD
    
    protected override string CreateValidRequest() => "test-collection";
    protected override string CreateInvalidRequest() => null!;

    [Fact]
    public async Task GetCollectionInfoAsync_WithValidCollectionName_ShouldReturnSuccess()
    {
        // Arrange: Mock returns success (contract test)
        MockPort.GetCollectionInfoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<CollectionInfo?>.Success(new CollectionInfo(...)));

        // Act: Test contract
        var result = await MockPort.GetCollectionInfoAsync(CreateValidRequest());

        // Assert: Validate CONTRACT (not implementation details)
        AssertContractSuccess(result);
        
        // ❌ DO NOT assert: RecordCount, ExecutionTimeMs (implementation details)
    }
}
```

### Step 2: Create LSP Validation Tests (Reuse Contract Tests)

**File**: `Implementations/QdrantVectorDatabaseAdapterLSPTests.cs`

```csharp
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Adapters;
using IndFusion.SemanticRag.Tests.Unit.Interfaces;
using Qdrant.Client;

namespace IndFusion.SemanticRag.Tests.Unit.Implementations;

/// <summary>
/// LSP validation: QdrantVectorDatabaseAdapter must satisfy IVectorDatabasePort contract.
/// 
/// CRITICAL: This class REUSES the same contract tests from IVectorDatabasePortContractTests.
/// If QdrantAdapter fails these tests, it violates Liskov Substitution Principle!
/// </summary>
public sealed class QdrantVectorDatabaseAdapterLSPTests 
    : IVectorDatabasePortContractTests
{
    protected override IVectorDatabasePort? CreateImplementation() 
    {
        var client = new QdrantClient("localhost", 6333);
        var logger = Substitute.For<ILogger<QdrantVectorDatabaseAdapter>>();
        return new QdrantVectorDatabaseAdapter(client, logger);
    }
    
    // SAME contract tests from base class automatically validate QdrantAdapter!
    // No need to rewrite tests - they're inherited!
}
```

**File**: `Implementations/InMemoryVectorDatabaseAdapterLSPTests.cs`

```csharp
namespace IndFusion.SemanticRag.Tests.Unit.Implementations;

public sealed class InMemoryVectorDatabaseAdapterLSPTests 
    : IVectorDatabasePortContractTests
{
    protected override IVectorDatabasePort? CreateImplementation() 
        => new InMemoryVectorDatabaseAdapter(logger);
    
    // SAME contract tests validate InMemoryAdapter!
}
```

---

## 🎯 What This Achieves

### ✅ Prevents IITDD Violations

1. **Sealed classes** prevent inheritance that breaks contract tests
2. **Generic pattern** forces interface-based testing
3. **Same tests** validate all implementations
4. **LSP validation** ensures all implementations satisfy contract
5. **Clear separation** between IITDD (contract) and TDD (behavior)

### ✅ Enforces Liskov Substitution Principle

- **If QdrantAdapter fails** but InMemoryAdapter passes → **QdrantAdapter violates LSP!**
- **If InMemoryAdapter fails** but QdrantAdapter passes → **InMemoryAdapter violates LSP!**
- **All implementations MUST pass** the same contract tests

### ✅ Makes IITDD Clear to Developers

- **One contract test class** validates interface contract
- **Multiple LSP test classes** validate real implementations
- **Same contract tests** used everywhere
- **Clear documentation** explains Liskov Principle
- **Impossible to bypass** (sealed, generic pattern)

---

## 📝 Key Principles

### IITDD Principle (Interface Contract Testing)

```csharp
// ✅ GOOD: Test contract with mock
MockPort.SearchAsync(...).Returns(Result<SearchResult>.Success(...));
var result = await MockPort.SearchAsync(request);
AssertContractSuccess(result); // Validates contract

// ❌ BAD: Test implementation details
result.RecordCount.ShouldBeGreaterThan(0); // Implementation detail!
```

### Liskov Substitution Principle

```csharp
// Same contract tests validate ALL implementations
public sealed class QdrantAdapterLSPTests : IVectorDatabasePortContractTests { }
public sealed class InMemoryAdapterLSPTests : IVectorDatabasePortContractTests { }
public sealed class MockAdapterLSPTests : IVectorDatabasePortContractTests { }

// If QdrantAdapter fails but InMemoryAdapter passes - QdrantAdapter violates LSP!
```

---

## 🚫 What NOT to Do

### ❌ DON'T: Create Separate Test Classes

```csharp
// ❌ BAD: Separate test classes for each implementation
public class QdrantAdapterTests { /* tests */ }
public class InMemoryAdapterTests { /* different tests */ }
// Problem: Tests may not validate same contract!
```

### ❌ DON'T: Test Implementation Details

```csharp
// ❌ BAD: Testing implementation details
result.RecordCount.ShouldBeGreaterThan(0); // Implementation detail!
result.ExecutionTimeMs.ShouldBeGreaterThan(0); // Implementation detail!
```

### ❌ DON'T: Mock Concrete Packages

```csharp
// ❌ BAD: Mocking concrete package
var client = Substitute.For<QdrantClient>(); // Cannot mock!

// ✅ GOOD: Mock port interface
var port = Substitute.For<IVectorDatabasePort>(); // Mock interface!
```

---

## ✅ What TO Do

### ✅ DO: Define Contract Tests Once

```csharp
// ✅ GOOD: One contract test class
public sealed class IVectorDatabasePortContractTests 
    : InterfaceContractTests<IVectorDatabasePort, Request, Result>
{
    // Contract tests using mocks
}
```

### ✅ DO: Reuse Contract Tests

```csharp
// ✅ GOOD: Reuse same contract tests for all implementations
public sealed class QdrantAdapterLSPTests : IVectorDatabasePortContractTests { }
public sealed class InMemoryAdapterLSPTests : IVectorDatabasePortContractTests { }
```

### ✅ DO: Test Contract States

```csharp
// ✅ GOOD: Test contract states
AssertContractSuccess(result); // Validates contract
AssertContractFailure(result, ErrorCode); // Validates error contract
```

---

## 📚 Files Created

1. **`InterfaceContractTests.cs`** - Sealed, generic base class for contract tests
2. **`LiskovSubstitutionTestPattern.cs`** - Alternative base class with LSP validation helpers
3. **`Example_IVectorDatabasePortContractTests.cs.example`** - Complete example implementation
4. **`LiskovTestPatternExample.md`** - Detailed pattern documentation
5. **`README-LiskovPattern.md`** - This guide

---

## 🎓 Developer Education

### Key Messages for Developers

1. **IITDD validates CONTRACTS**, not implementations
2. **Same contract tests validate ALL implementations** (Liskov Principle)
3. **If an implementation fails contract tests** → It violates LSP
4. **Sealed, generic pattern** prevents breaking IITDD
5. **DO NOT test implementation details** (RecordCount, ExecutionTimeMs, etc.)

### When Developers Are Confused

1. **Point to this README** for pattern explanation
2. **Show the example files** for concrete implementation
3. **Explain Liskov Principle** - all implementations must satisfy same contract
4. **Remind them** - IITDD validates contracts, TDD validates implementations

---

**This pattern ensures developers cannot destroy IITDD patterns - the sealed, generic structure enforces contract-based testing!**

