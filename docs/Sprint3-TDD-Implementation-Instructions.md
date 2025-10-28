# Sprint 3 TDD Implementation Phase - Agent Instructions

## 🎯 **Mission: Implement TDD Phase for Graph RAG Layer**

You are tasked with implementing the **TDD Implementation Phase** of Sprint 3. The Contract Phase has been completed successfully with 28/28 contract tests passing. Now you need to implement the real services using Test-Driven Development.

## 📋 **Current State - What's Already Done**

### ✅ **Completed (Contract Phase)**
- **4 Service Interfaces** created with comprehensive XML documentation:
  - `IPatternGraphService` - Query and manage pattern graphs
  - `IPatternSuggestionService` - Generate pattern suggestions  
  - `ISymbolGraphBuilder` - Build symbol graphs from codebases
  - `IGraphCacheManager` - Manage caching of symbol graphs

- **8 Domain Models** created as immutable records:
  - `PatternGraphQuery`, `PatternGraphQueryResult`, `GraphNode`, `GraphEdge`, `SymbolGraph`
  - `SourceLocation`, `QueryMetadata` (in PatternGraph namespace)
  - Uses existing `PatternSuggestion`, `PatternSuggestionRequest` from Abstractions

- **4 Placeholder Implementations** with `NotImplementedException`:
  - `PatternGraphService`, `PatternSuggestionService`, `SymbolGraphBuilder`, `GraphCacheManager`
  - All have proper constructor injection with `ILogger<T>`
  - All compile and build successfully

- **28 Contract Tests** - ALL PASSING ✅
  - `IPatternGraphServiceContractTests` (5 tests)
  - `IPatternSuggestionServiceContractTests` (6 tests) 
  - `ISymbolGraphBuilderContractTests` (7 tests)
  - `IGraphCacheManagerContractTests` (10 tests)

- **Architecture Tests** - Active and monitoring:
  - `NoDuplicateTypesTests` - Enforces DRY principles
  - `LayerDependencyTests` - Ensures clean architecture
  - `NamingConventionTests` - Enforces naming standards

## 🚀 **Your Mission: TDD Implementation Phase**

### **Phase 1: Create TDD Implementation Tests (Red Phase)**

Create implementation tests that call the **REAL** service classes (not mocks) and should **FAIL** with `NotImplementedException`:

**Files to Create:**
```
ExxerRules/src/test/IndFusion.Mcp.Tests/PatternGraph/
├── PatternGraphServiceTests.cs
├── PatternSuggestionServiceTests.cs  
├── SymbolGraphBuilderTests.cs
└── GraphCacheManagerTests.cs
```

**Test Structure Pattern:**
```csharp
public class PatternGraphServiceTests
{
    private readonly PatternGraphService _service;
    private readonly ILogger<PatternGraphService> _logger;

    public PatternGraphServiceTests()
    {
        _logger = Substitute.For<ILogger<PatternGraphService>>();
        _service = new PatternGraphService(_logger);
    }

    [Fact]
    public async Task QueryAsync_WithValidQuery_ShouldReturnSuccessResult()
    {
        // Arrange
        var query = new PatternGraphQuery(ProjectPath: "/test/project");
        
        // Act & Assert - Should throw NotImplementedException
        await Should.ThrowAsync<NotImplementedException>(
            () => _service.QueryAsync(query, CancellationToken.None));
    }
}
```

**Key Points:**
- Use **REAL** service instances, not mocks
- Tests should **FAIL** initially with `NotImplementedException`
- Use `Should.ThrowAsync<NotImplementedException>()` for assertions
- Test all public methods of each service
- Use realistic test data

### **Phase 2: Implement Services (Green Phase)**

Implement the services to make tests pass. **Use Sequential Thinking** for complex implementations:

#### **Implementation Order (Start Simple):**
1. **GraphCacheManager** - Simplest, just caching logic
2. **PatternSuggestionService** - Medium complexity
3. **SymbolGraphBuilder** - Complex, uses Roslyn
4. **PatternGraphService** - Most complex, orchestrates others

#### **SOLID Principles - Apply Strictly:**

**Single Responsibility Principle (SRP):**
- Each service has ONE clear purpose
- Extract helper classes for complex operations
- Keep methods focused and small

**Open/Closed Principle (OCP):**
- Use interfaces and dependency injection
- Make services extensible without modification
- Follow existing patterns from `LintingService`

**Liskov Substitution Principle (LSP):**
- Implementations must honor interface contracts exactly
- All methods must behave as specified in interfaces
- Handle all edge cases properly

**Interface Segregation Principle (ISP):**
- Keep interfaces focused and cohesive
- Don't force clients to depend on unused methods
- Use composition over inheritance

**Dependency Inversion Principle (DIP):**
- Depend on abstractions (`ILogger<T>`, interfaces)
- Inject dependencies through constructors
- Use existing abstractions from the codebase

#### **DRY Principle - Critical:**
- **NO duplicate code** - Architecture tests will catch this
- Extract common functionality into helper classes
- Use existing utilities from the codebase
- Follow established patterns from `LintingService`

#### **Implementation Strategy:**

**For Each Service:**
1. **Use Sequential Thinking** to break down complex operations
2. **Start with the simplest method** first
3. **Use existing patterns** from `LintingService.cs` as reference
4. **Leverage Roslyn infrastructure** for code analysis
5. **Follow established logging patterns** with `ILogger<T>`
6. **Use Result<T> pattern** consistently
7. **Handle CancellationToken** properly in all async methods

**Example Implementation Pattern:**
```csharp
public async Task<Result<SymbolGraph?>> GetAsync(
    string projectHash, 
    CancellationToken cancellationToken = default)
{
    try
    {
        _logger.LogInformation("Retrieving cached symbol graph for hash: {ProjectHash}", projectHash);
        
        // Implementation logic here
        // Use existing caching patterns
        // Return Result<SymbolGraph?>.Success(graph) or Result<SymbolGraph?>.WithFailure(error)
        
        return Result<SymbolGraph?>.Success(cachedGraph);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to retrieve cached graph for hash: {ProjectHash}", projectHash);
        return Result<SymbolGraph?>.WithFailure($"Failed to retrieve cached graph: {ex.Message}");
    }
}
```

### **Phase 3: Quality Gates & Verification**

**After Each Implementation:**

1. **Run Architecture Tests:**
   ```bash
   dotnet run --no-build -- --filter-namespace "IndFusion.Mcp.Tests.Architecture"
   ```
   - Must pass 4/4 tests
   - Ensures DRY principles and quality standards

2. **Run Contract Tests:**
   ```bash
   dotnet run --no-build -- --filter-namespace "IndFusion.Mcp.Tests.PatternGraph"
   ```
   - Must pass 28/28 tests
   - Ensures interface contracts are honored

3. **Run Implementation Tests:**
   ```bash
   dotnet run --no-build -- --filter-method "PatternGraphServiceTests"
   ```
   - Should pass after implementation
   - Verifies real functionality works

4. **Build Verification:**
   ```bash
   dotnet build code/IndFusion.Mcp.Core/IndFusion.Mcp.Core.csproj -c Debug
   ```
   - Must compile without errors
   - Must have no warnings

## 🔧 **Technical Requirements**

### **Dependencies to Use:**
- **Roslyn**: `Microsoft.CodeAnalysis.CSharp`, `Microsoft.CodeAnalysis.CSharp.Workspaces`
- **Caching**: `Microsoft.Extensions.Caching.Memory`
- **Logging**: `ILogger<T>` with structured logging
- **Result Pattern**: `IndQuestResults` package
- **Existing Infrastructure**: Follow patterns from `LintingService.cs`

### **File Locations:**
- **Interfaces**: `ExxerRules/src/code/IndFusion.Mcp.Core/Abstractions/`
- **Models**: `ExxerRules/src/code/IndFusion.Mcp.Core/Models/PatternGraph/`
- **Services**: `ExxerRules/src/code/IndFusion.Mcp.Core/Services/`
- **Tests**: `ExxerRules/src/test/IndFusion.Mcp.Tests/PatternGraph/`

### **Existing Patterns to Follow:**
- Study `LintingService.cs` for implementation patterns
- Use existing caching infrastructure
- Follow established error handling patterns
- Use structured logging with proper log levels

## ✅ **Success Criteria**

### **Must Achieve:**
- ✅ **28/28 Contract Tests** continue to pass
- ✅ **4/4 Architecture Tests** continue to pass  
- ✅ **All Implementation Tests** pass (after implementation)
- ✅ **Zero compilation errors**
- ✅ **Zero warnings**
- ✅ **SOLID principles** applied throughout
- ✅ **DRY principle** enforced (no duplicate code)
- ✅ **Complete XML documentation** on all public members

### **Quality Standards:**
- **Clean Code**: Meaningful names, small methods, clear logic
- **Modern C#**: Records, nullable reference types, async/await
- **Error Handling**: Use Result<T> pattern, no exceptions in normal flow
- **Logging**: Structured logging with appropriate levels
- **Testing**: Comprehensive test coverage with realistic scenarios

## 🚨 **Critical Reminders**

1. **Use Sequential Thinking** for complex implementation decisions
2. **Run Architecture Tests** after each major change
3. **Follow DRY Principle** - Architecture tests will catch violations
4. **Start Simple** - Begin with GraphCacheManager, work up to PatternGraphService
5. **Test-Driven** - Write failing tests first, then implement to make them pass
6. **Follow Existing Patterns** - Study LintingService.cs for guidance
7. **Maintain Quality** - All tests must pass, no shortcuts

## 📞 **If You Get Stuck**

1. **Study Existing Code**: Look at `LintingService.cs` for patterns
2. **Use Sequential Thinking**: Break down complex problems step-by-step
3. **Check Architecture Tests**: They'll tell you if you're violating DRY
4. **Run Tests Frequently**: Catch issues early
5. **Follow the Contract**: Interfaces define the behavior you must implement

---

**Remember**: You're building the foundation for Sprint 3's Graph RAG Layer. Quality and adherence to SOLID principles are critical. The Architecture Tests are your "agent watchers" - they will catch any violations of DRY or quality standards.

**Good luck!** 🚀
