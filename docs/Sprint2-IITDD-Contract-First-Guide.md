# Sprint 2 IITDD Contract-First Implementation Guide

## 🎯 Overview

This document provides a comprehensive guide for implementing Sprint 2 using Integration Interface Test-Driven Development (IITDD) with a contract-first approach. Based on the Unified Semantic RAG Standards Initiative, we're establishing the foundation for MCP tooling surface development.

## 📋 Current Status

**✅ Completed:**
- MCP tool contracts defined (`ILintingService`, `IPatternSuggestionService`, `ICodeTransformationService`, `IKnowledgeRagService`)
- Hexagonal architecture ports defined (`IVectorStorePort`, `IKnowledgeGraphPort`, `IDocumentProcessingPort`, `IAnalyzerIntegrationPort`)
- Domain models created for all contracts
- IITDD test infrastructure established
- Contract tests created for `ILintingService` and `IVectorStorePort`

**🔄 In Progress:**
- Contract tests for remaining services
- Service composition contracts
- Integration test scenarios

## 🏗️ Architecture Overview

### Contract-First Approach

```
┌─────────────────────────────────────────────────────────────┐
│                    CONTRACT LAYER                           │
├─────────────────────────────────────────────────────────────┤
│  MCP Tool Contracts                                        │
│  • ILintingService                                         │
│  • IPatternSuggestionService                               │
│  • ICodeTransformationService                              │
│  • IKnowledgeRagService                                    │
├─────────────────────────────────────────────────────────────┤
│  Hexagonal Architecture Ports                             │
│  • IVectorStorePort                                        │
│  • IKnowledgeGraphPort                                     │
│  • IDocumentProcessingPort                                 │
│  • IAnalyzerIntegrationPort                                │
├─────────────────────────────────────────────────────────────┤
│  Domain Models                                             │
│  • Request/Response Models                                │
│  • Entity Models                                          │
│  • Configuration Models                                   │
└─────────────────────────────────────────────────────────────┘
```

### IITDD Test Structure

```
┌─────────────────────────────────────────────────────────────┐
│                    IITDD TEST LAYER                         │
├─────────────────────────────────────────────────────────────┤
│  Infrastructure                                            │
│  • IITDDTestBase                                          │
│  • ServiceContractTestBase<TInterface, TImplementation>  │
├─────────────────────────────────────────────────────────────┤
│  Contract Tests                                            │
│  • LintingServiceContractTests                            │
│  • VectorStorePortContractTests                           │
│  • PatternSuggestionServiceContractTests                  │
│  • CodeTransformationServiceContractTests                 │
│  • KnowledgeRagServiceContractTests                       │
├─────────────────────────────────────────────────────────────┤
│  Integration Tests                                         │
│  • ServiceCompositionTests                                │
│  • EndToEndWorkflowTests                                  │
│  • CrossServiceIntegrationTests                           │
└─────────────────────────────────────────────────────────────┘
```

## 🔧 Implementation Steps

### Step 1: Define Contracts (✅ Complete)

All MCP tool contracts and hexagonal architecture ports have been defined with comprehensive request/response models.

### Step 2: Create Contract Tests (🔄 In Progress)

**Completed:**
- `LintingServiceContractTests` - Tests `ILintingService` contract
- `VectorStorePortContractTests` - Tests `IVectorStorePort` contract

**Remaining:**
- `PatternSuggestionServiceContractTests`
- `CodeTransformationServiceContractTests`
- `KnowledgeRagServiceContractTests`
- `KnowledgeGraphPortContractTests`
- `DocumentProcessingPortContractTests`
- `AnalyzerIntegrationPortContractTests`

### Step 3: Create Stub Implementations

Each contract test includes a stub implementation that:
- Satisfies the interface contract
- Provides realistic behavior for testing
- Includes proper logging and error handling
- Handles cancellation tokens appropriately

### Step 4: Service Composition Contracts

Define how services will be composed and tested together:

```csharp
public interface IServiceCompositionContract
{
    Task<CompositionResult> ComposeServicesAsync(CompositionRequest request, CancellationToken cancellationToken = default);
    Task<ValidationResult> ValidateCompositionAsync(CompositionRequest request, CancellationToken cancellationToken = default);
}
```

### Step 5: Integration Test Scenarios

Create end-to-end integration tests that:
- Test complete workflows
- Validate service interactions
- Ensure proper error handling
- Verify performance characteristics

## 📝 Contract Test Patterns

### Base Contract Test Structure

```csharp
public class ServiceContractTests : ServiceContractTestBase<IService, ServiceStub>
{
    protected override void ConfigureServiceDependencies(IServiceCollection services)
    {
        // Register dependencies
    }

    [Fact]
    public async Task Method_WithValidInput_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = CreateValidRequest();
        
        // Act
        var result = await Service.MethodAsync(request, CreateTestCancellationToken());
        
        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        // Additional assertions
    }

    public override async Task Service_ShouldHandleCancellationTokens()
    {
        // Test cancellation handling for all async methods
    }

    public override async Task Service_ShouldHandleNullParameters()
    {
        // Test null parameter handling
    }
}
```

### Stub Implementation Pattern

```csharp
public class ServiceStub : IService
{
    private readonly ILogger<ServiceStub> _logger;

    public ServiceStub(ILogger<ServiceStub> logger)
    {
        _logger = logger;
    }

    public async Task<ServiceResult> MethodAsync(ServiceRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Executing method with request: {Request}", request);
        
        await Task.Delay(100, cancellationToken); // Simulate processing
        
        return new ServiceResult(
            Success: true,
            Message: "Operation completed successfully",
            // Additional result properties
        );
    }
}
```

## 🧪 Testing Guidelines

### Contract Test Requirements

1. **Interface Compliance**: Verify all interface methods are implemented
2. **Method Signatures**: Validate method signatures match exactly
3. **Return Types**: Ensure return types are correct
4. **Cancellation Handling**: Test cancellation token support
5. **Null Parameter Handling**: Test null parameter scenarios
6. **Error Handling**: Verify proper error handling patterns
7. **Logging**: Ensure proper logging is implemented

### Integration Test Requirements

1. **Service Composition**: Test services working together
2. **End-to-End Workflows**: Test complete user scenarios
3. **Error Propagation**: Test error handling across services
4. **Performance**: Validate performance characteristics
5. **Concurrency**: Test concurrent operations
6. **Resource Management**: Test proper resource disposal

## 🚀 Next Steps

### Immediate Actions

1. **Complete Contract Tests**: Finish remaining contract tests for all services
2. **Create Service Composition Tests**: Define how services work together
3. **Implement Real Services**: Replace stubs with real implementations
4. **Add Integration Scenarios**: Create end-to-end test scenarios

### Implementation Order

1. **Vector Store Implementation**: Implement `IVectorStorePort` with Qdrant
2. **Knowledge Graph Implementation**: Implement `IKnowledgeGraphPort` with Neo4j
3. **Document Processing Implementation**: Implement `IDocumentProcessingPort` with OCR/LLM
4. **Analyzer Integration Implementation**: Implement `IAnalyzerIntegrationPort` with Roslyn
5. **MCP Tool Implementation**: Implement MCP tools using the ports
6. **Service Composition**: Wire services together with dependency injection

## 📊 Success Criteria

### Contract Test Coverage

- ✅ 100% of interface methods tested
- ✅ All cancellation token scenarios covered
- ✅ All null parameter scenarios covered
- ✅ All error handling paths tested
- ✅ All logging scenarios validated

### Integration Test Coverage

- ✅ Service composition scenarios tested
- ✅ End-to-end workflows validated
- ✅ Error propagation tested
- ✅ Performance characteristics validated
- ✅ Resource management tested

### Implementation Quality

- ✅ All services implement their contracts correctly
- ✅ Proper error handling throughout
- ✅ Comprehensive logging implemented
- ✅ Cancellation token support everywhere
- ✅ Resource disposal properly handled

## 🔍 Validation Commands

### Run Contract Tests

```bash
dotnet test src/test/IndFusion.Mcp.Tests.Integration/Contracts/ --logger "console;verbosity=detailed"
```

### Run Integration Tests

```bash
dotnet test src/test/IndFusion.Mcp.Tests.Integration/ --logger "console;verbosity=detailed"
```

### Validate Service Registrations

```bash
dotnet test src/test/IndFusion.Mcp.Tests.Integration/Infrastructure/ --logger "console;verbosity=detailed"
```

## 📚 References

- [Unified Semantic RAG Standards Initiative](docs/Unified-Semantic-RAG-Standards-Initiative.md)
- [Hexagonal Architecture Strategy](docs/architecture/Hexagonal-Architecture-IITDD-Strategy.md)
- [Agentic Execution Framework](docs/architecture/Agentic-Execution-Framework.md)
- [TDD Lessons Learned](docs/Sprint1-TDD-Lessons-Learned.md)

---

**Note**: This guide follows the IITDD methodology established in Sprint 1, ensuring that all implementations are driven by integration tests and interface contracts. The contract-first approach guarantees that services fulfill their contracts before implementation begins.