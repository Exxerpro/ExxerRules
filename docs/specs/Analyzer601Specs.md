# UseRepositoryPattern Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER601`  
**Source**: `src/code/IndFusion.Analyzer/Architecture/UseRepositoryPatternAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- No spec exists for EXXER601.  
- The analyzer flags every constructor parameter, field, or property whose type name contains `DbContext`, `SqlConnection`, `IDbConnection`, etc., unless the class name itself looks like a repository.  
- In real projects, application-layer handlers, background workers, and infrastructure services legitimately depend on EF Core `DbContext` or Dapper connections. Forcing the repository pattern everywhere creates excessive false positives.  
- Notable examples:
  - `Test Project\Src\Code\Core\Application\Machines\Commands\Update\MachineUpdateCommandHandler.cs:23` comments about `DbContext` usage cause diagnostics even when the handler only touches repositories.  
  - `Test Project\Src\Code\Infrastructure\Sharp7.Rx` types manipulate hardware connections (not EF contexts) yet the analyzer flags their fields because type names contain `Connection`.  
  - Integration tests with `DbContext` fixtures (e.g., `HybridTestDataManager`) receive warnings despite being part of test infrastructure.

Given the breadth of legitimate direct data access in the solution, EXXER601 is the next highest-impact analyzer requiring mitigation.

## 1. Specification

- **Intent**  
  Encourage clean architecture by steering domain/application layers toward repository abstractions instead of binding directly to data access implementations.

- **Scope**  
  Scans every class declaration. If the class name does not resemble a repository (`*Repository`, `*Dal`, `*DataAccess`), it reports a diagnostic whenever:
  - A field or property type name contains one of the hardcoded tokens (e.g., `DbContext`, `SqlConnection`).  
  - A constructor parameter names one of those types.

## 2. Validation Plan

1. Add `UseRepositoryPatternAnalyzerFalsePositiveTests` covering the ten scenarios below plus positive controls.  
2. Include integration cases from Infrastructure/test projects to prove the heuristics no longer fire.  
3. Run `dotnet test` for analyzer suites and selected solutions to confirm EXXER601 warning counts drop meaningfully.  
4. Maintain positive coverage to ensure genuine misuse (e.g., domain service instantiating `SqlConnection`) still surfaces.

## 3. Enhancement Opportunities (>=10 Items)

Each entry presents a false-positive scenario, mitigation idea, and xUnit/Shouldly snippet.

### 1. Application Layer Handlers Using `DbContext`

- **Problem**: Command/query handlers occasionally require direct EF Core queries for performance; analyzer treats all of them as violations.  
- **Mitigation**: Allow classes located in `*.Application.*` namespaces to depend on `DbContext` if they’re input-bound (e.g., decorated with `IRequestHandler`, `IMonitorRequestHandler`) or if they call `.AsNoTracking()`/`Set<T>()` as part of read-only operations.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Handler_With_DbContext()
{
    const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace Sample.Application.Orders;

public sealed class GetOrderHandler
{
    private readonly DbContext _context;

    public GetOrderHandler(DbContext context) => _context = context;

    public Task<int> HandleAsync() => _context.Set<int>().CountAsync();
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 2. Infrastructure Layer (Namespace contains `.Infrastructure`)

- **Problem**: Infrastructure projects purposely wrap low-level data access (e.g., `ShardDbContext`, `Sharp7Connector`). Analyzer still emits warnings.  
- **Mitigation**: Automatically skip diagnostics when the containing namespace or project path includes `.Infrastructure`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Infrastructure_Namespace()
{
    const string testCode = @"
namespace Sample.Infrastructure.Persistence;

public sealed class ReportingDbContext : Microsoft.EntityFrameworkCore.DbContext";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Test/Fixture Classes

- **Problem**: Integration tests (`HybridTestDataManager`) rely on `DbContext` for seeding data, yet the analyzer flags them.  
- **Mitigation**: Skip classes whose namespace or name ends with `Tests`, `Fixture`, `Integration`, or located under `Tests\`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Test_Fixtures()
{
    const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace Sample.Tests.Infrastructure;

public sealed class DbFixture
{
    public DbFixture(DbContext context) { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. Connection Wrapper Classes (e.g., Dapper)

- **Problem**: Types like `SqlConnectionFactory`, `ConnectionManager` or hardware connectors (Sharp7) are expected to expose raw connections.  
- **Mitigation**: Allow classes whose name ends with `Connection`, `Connector`, `Factory`, `Scheduler`, etc., to hold connection objects.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Connection_Factory()
{
    const string testCode = @"
using System.Data;

public sealed class SqlConnectionFactory
{
    public IDbConnection Create() => new System.Data.SqlClient.SqlConnection();
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. `DbContextOptions` or EF Services

- **Problem**: Constructors injecting `DbContextOptions<T>` or `IDbContextFactory<T>` are flagged even though they’re DI infrastructure.  
- **Mitigation**: Skip types whose names contain `DbContextOptions`, `IDbContextFactory`, `IServiceScopeFactory`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_DbContextOptions()
{
    const string testCode = @"
using Microsoft.EntityFrameworkCore;

public sealed class TenantFactory
{
    public TenantFactory(DbContextOptions options) { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. Minimal APIs / Program.cs

- **Problem**: `Program` classes or top-level statements legitimately create `DbContext`/`SqlConnection` instances.  
- **Mitigation**: Skip diagnostics when the containing type is `Program` or file contains top-level statements.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Program_Class()
{
    const string testCode = @"
using Microsoft.EntityFrameworkCore;

public static class Program
{
    public static void Main(DbContext context) { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. Multi-Tenant or Generic Infrastructure Services

- **Problem**: Services such as `DbContextTransactionBehavior` or `UnitOfWork` legitimately depend on `DbContext` to provide shared transaction scopes.  
- **Mitigation**: Allow classes whose names include `Transaction`, `UnitOfWork`, `Migration`, `Seeder`, `Seeder`, or are decorated with `[InfrastructureService]`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_UnitOfWork()
{
    const string testCode = @"
using Microsoft.EntityFrameworkCore;

public sealed class UnitOfWork
{
    private readonly DbContext _context;
    public UnitOfWork(DbContext context) => _context = context;
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. Repositories Already Implemented via Generics

- **Problem**: Generic repository base classes (`RepositoryBase<TContext>`) contain `DbContext` fields by design but should be considered part of the pattern.  
- **Mitigation**: If a class inherits from or contains `RepositoryBase`, `ReadOnlyRepository`, or similar, suppress diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Generic_Repository_Base()
{
    const string testCode = @"
using Microsoft.EntityFrameworkCore;

public abstract class RepositoryBase<TContext>
{
    protected RepositoryBase(TContext context) { }
}

public sealed class CustomerRepository : RepositoryBase<DbContext>
{
    public CustomerRepository(DbContext context) : base(context) { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. Domain-Specific EF Extensions

- **Problem**: Methods that accept `DbContext` as parameters (e.g., extension methods `AwaitDbContext`) are flagged even though they are infrastructure utilities.  
- **Mitigation**: Skip static classes in namespaces ending with `.Persistence`, `.EntityFramework`, or decorated with `[DbContextFactory]`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Persistence_Extensions()
{
    const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace Sample.Infrastructure.Persistence;

public static class DbContextExtensions
{
    public static TEntity FindCached<TEntity>(this DbContext context, object key) where TEntity : class =>
        context.Set<TEntity>().Find(key);
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Opt-Out Attribute (`AllowDirectDataAccess`)

- **Problem**: There are legitimate cases (batch jobs, data migrations) where direct `DbContext` access is intended.  
- **Mitigation**: Introduce `[AllowDirectDataAccess]` attribute respected at class or method level.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_OptOut_Attribute()
{
    const string testCode = @"
using System;
using Microsoft.EntityFrameworkCore;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AllowDirectDataAccessAttribute : Attribute { }

[AllowDirectDataAccess]
public sealed class DataMigration
{
    public DataMigration(DbContext context) { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer())
        .ShouldBeEmpty();
}
```

## 4. Test-Driven Fix Strategy

1. Encode the ten regression tests above (plus existing true positives) in the analyzer test suite.  
2. Update `UseRepositoryPatternAnalyzer` to:
   - Use semantic information (`INamedTypeSymbol`) instead of plain `string.Contains`.  
   - Respect namespace/project context (Infrastructure, Tests) and pattern-based exclusions (ConnectionFactory, UnitOfWork).  
   - Honour opt-out attributes and known EF constructs (`DbContextOptions`, `IDbContextFactory`).  
3. Run analyzer tests to ensure new cases fail before changes and pass after.  
4. Execute `dotnet test` on target solutions to observe EXXER601 warning reductions, particularly in Infrastructure and Test projects.  
5. Update `AnalyzerReleases.Unshipped.md` summarising the refined heuristics and attribute opt-outs.

## 5. Acceptance Checklist

- [ ] Analyzer enhanced with contextual detection and opt-outs.  
- [ ] Ten regression tests added/passing.  
- [ ] Solution builds/tests succeed.  
- [ ] Documented drop in EXXER601 warnings across application/infrastructure/test layers.  
- [ ] Release notes updated.
