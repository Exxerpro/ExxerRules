# DomainShouldNotReferenceInfrastructure Analyzer - False-Positive Mitigation Spec

**Analyzer ID**: `EXXER600`  
**Source**: `src/code/IndFusion.Analyzer/Architecture/DomainShouldNotReferenceInfrastructureAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-08)

## 0. Selection Rationale

- `docs/specs` lacks coverage for EXXER600 even though it is emitted as an **error** (see `AnalyzerReleases.Unshipped.md:16`).  
- Reviewing the IndTrace sample showed multiple legitimate scenarios where domain-focused code and tests already need EF Core and provider namespaces. When the analyzer is enabled these files get blocked despite living in test assemblies or value-object helpers: e.g.,  
  - `Test Project/Src/Code/Core/Domain/ValueObjects/ProductionData.cs` is a value object that the team marks with EF Core `[Owned]` attributes in feature branches, triggering EXXER600.  
  - Domain unit-tests in `Test Project/Src/Tests/Core/Domain.UnitTests/Services/Products/ProductValidatorTests.cs` spin up an InMemory `DbContext` to exercise validation logic; they currently rely on `<NoWarn>` to avoid analyzer noise.  
- Lower-numbered missing analyzer EXXER103 (UseNSubstitute) still produces no diagnostics within the approved search roots (`Test Project/Src`, `src/test/IndFusion.Mcp.*`), so per task instructions the next eligible analyzer is EXXER600.

## 1. Specification

- **Intent**  
  Guard the Clean Architecture boundary by preventing Domain-layer source files from referencing Infrastructure-layer namespaces or ADO.NET providers.

- **Scope**  
  Registers `SyntaxKind.UsingDirective`. A diagnostic is reported when:
  - The containing namespace textually matches any pattern with `.Domain.`/`Domain.`/`Domain` and  
  - The `using` directive namespace contains *Infrastructure* or matches hard-coded provider/EF Core namespaces (`Microsoft.EntityFrameworkCore`, `System.Data.SqlClient`, `Npgsql`, `MySql.Data`, etc.).
  No semantic checks are performed; the rule is entirely string based.

## 2. Validation Plan

1. Add `DomainShouldNotReferenceInfrastructureAnalyzerFalsePositiveTests` in `src/test/IndFusion.Analyzer.Tests/TestCases/Architecture`.  
2. Port the ten scenarios below into Roslyn analyzer unit tests plus regression coverage inside `Test Project/Src/Tests/Core/Domain.UnitTests`.  
3. Exercise the analyzer against `Test Project/Src/Code/Core/Domain` and domain unit-test assemblies to confirm flagged diagnostics disappear once mitigations land.  
4. Run `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release` and a spot `dotnet build Test Project/Src/Code/Core/Domain/Core.Domain.csproj -c Release` to verify boundary checks still catch genuine violations (e.g., domain types calling `SqlConnection` directly).

## 3. Enhancement Opportunities (≥10 Items)

### 1. EF Owned Value Objects in Domain
- **Problem**: Value objects like `ProductionData` (`Test Project/Src/Code/Core/Domain/ValueObjects/ProductionData.cs`) must carry `[Owned]` or `[ComplexType]` attributes. Importing `Microsoft.EntityFrameworkCore` to access these attributes triggers EXXER600 even though the attribute is metadata, not an infrastructure dependency.
- **Mitigation**: Perform semantic analysis. If the `using` resolves to an attribute applied to a record/class inside the same file, skip reporting. Alternatively, allow EF Core attribute namespaces when the file contains no executable member referencing infrastructure services.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Owned_ValueObject_Metadata()
  {
      const string code = @"
using Microsoft.EntityFrameworkCore;

namespace IndTrace.Domain.ValueObjects;

[Owned]
public sealed record ProductionData(double Actual, double Plan);";

      AnalyzerTestHelper
          .RunAnalyzer(code, new DomainShouldNotReferenceInfrastructureAnalyzer())
          .ShouldBeEmpty(); // currently fails with EXXER600
  }
  ```

### 2. Domain Enum Seed Extensions Using ModelBuilder
- **Problem**: Lookup enums under `Test Project/Src/Code/Core/Domain/Enum/LookUpTable` expose static `EnumLookUpTable.Seed(ModelBuilder)` helpers. Those helpers live in the domain assembly so they can be reused during Infrastructure composition. The method requires `Microsoft.EntityFrameworkCore.ModelBuilder`, which the analyzer bans.
- **Mitigation**: Allow EF Core namespaces when the symbol references reside in `partial` classes or static extension methods returning `ModelBuilder` that live alongside enum helpers. Alternatively, exempt files ending in `.SeedExtensions.cs`.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Domain_Lookup_Seeding_Extension()
  {
      const string code = @"
using Microsoft.EntityFrameworkCore;

namespace IndTrace.Domain.Enum.LookUpTable;

public static class LookupModelBuilderExtensions
{
    public static ModelBuilder RegisterLookups(this ModelBuilder builder)
    {
        return builder; // domain-owned seed helper
    }
}";

      AnalyzerTestHelper
          .RunAnalyzer(code, new DomainShouldNotReferenceInfrastructureAnalyzer())
          .ShouldBeEmpty();
  }
  ```

### 3. Domain Configurations Sharing IEntityTypeConfiguration
- **Problem**: Several domain entities (see `IndTrace.Domain.Entities.BarCodes.BarCode`) ship their own `Configuration` types to keep fluent rules near the model. Those configurations implement `IEntityTypeConfiguration<T>` and therefore require `Microsoft.EntityFrameworkCore`. Domain files containing both the entity and nested configuration emit EXXER600.
- **Mitigation**: Detect nested classes implementing `IEntityTypeConfiguration<T>` whose symbol lives in the same file as the domain entity. If the configuration only configures the containing domain type, omit the diagnostic.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Nested_EntityTypeConfiguration()
  {
      const string code = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Domain.Entities;

public class Machine : IEntityRoot
{
    public int MachineId { get; set; }

    internal sealed class Configuration : IEntityTypeConfiguration<Machine>
    {
        public void Configure(EntityTypeBuilder<Machine> builder) =>
            builder.HasKey(x => x.MachineId);
    }
}";

      AnalyzerTestHelper
          .RunAnalyzer(code, new DomainShouldNotReferenceInfrastructureAnalyzer())
          .ShouldBeEmpty();
  }
  ```

### 4. Domain Tests Using EF InMemory Providers
- **Problem**: `Test Project/Src/Tests/Core/Domain.UnitTests/Services/Products/ProductValidatorTests.cs` spins up an InMemory `DbContext` to evaluate validation logic with seeded entities. Importing `Microsoft.EntityFrameworkCore.InMemory` triggers EXXER600 because the namespace contains `.Domain.` in the test namespace.
- **Mitigation**: Skip diagnostics when the file path or namespace ends with `.UnitTests`/`.Tests`. Alternatively inspect the containing project assembly name (ends with `.Tests`) before flagging.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Domain_UnitTests_InMemoryContext()
  {
      const string code = @"
using Microsoft.EntityFrameworkCore;

namespace IndTrace.Domain.UnitTests.Services.Products;

public class ProductValidatorTests
{
    [Fact]
    public void Should_Validate_With_InMemory_Context()
    {
        var options = new DbContextOptionsBuilder().UseInMemoryDatabase(""DomainTests"").Options;
    }
}";

      AnalyzerTestHelper
          .RunAnalyzer(code, new DomainShouldNotReferenceInfrastructureAnalyzer(),
              filePath: @""C:\src\Test Project\Src\Tests\Core\Domain.UnitTests\Services\Products\ProductValidatorTests.cs"")
          .ShouldBeEmpty();
  }
  ```

### 5. Domain Tests Validating ModelBuilder Projections
- **Problem**: `WorkflowBinderTests` (domain unit tests) validate EF projections by composing a `ModelBuilder`. Pulling in `Microsoft.EntityFrameworkCore.Metadata` for `IMutableEntityType` analysis is flagged even though this logic lives strictly in tests.
- **Mitigation**: Same as above—detect test project context (namespace ends with `.UnitTests`) and bypass. Alternatively, allow EF Core namespaces when the file resides under `Test Project\Src\Tests`.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Domain_ModelBuilder_Test_Verifications()
  {
      const string code = @"
using Microsoft.EntityFrameworkCore.Metadata;

namespace IndTrace.Domain.UnitTests.Services.Products;

public class WorkflowBinderTests
{
    [Fact]
    public void Should_Inspect_Model_Metadata()
    {
        IMutableEntityType metadata = null!;
    }
}";

      AnalyzerTestHelper
          .RunAnalyzer(code, new DomainShouldNotReferenceInfrastructureAnalyzer(),
              filePath: @""C:\src\Test Project\Src\Tests\Core\Domain.UnitTests\Services\Products\WorkflowBinderTests.cs"")
          .ShouldBeEmpty();
  }
  ```

### 6. Domain Guard Logic Using SqlConnectionStringBuilder
- **Problem**: Domain service `TaskGatewayResponse` validates connection strings via `SqlConnectionStringBuilder`. Importing `Microsoft.Data.SqlClient` is blocked even though the logic merely parses strings and never talks to a database.
- **Mitigation**: Treat `SqlConnectionStringBuilder`, `SqlConnectionStringBuilder` usage as safe when only the builder type (and no connection creation/opening) is referenced.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_ConnectionString_Parsing()
  {
      const string code = @"
using Microsoft.Data.SqlClient;

namespace IndTrace.Domain.Services;

public static class ConnectionStringGuard
{
    public static bool IsValid(string value) =>
        new SqlConnectionStringBuilder(value).ConnectTimeout > 0;
}";

      AnalyzerTestHelper
          .RunAnalyzer(code, new DomainShouldNotReferenceInfrastructureAnalyzer())
          .ShouldBeEmpty();
  }
  ```

### 7. Provider-Specific Validation in Domain Rules
- **Problem**: `ShiftDetectionRule` (see `Test Project/Src/Code/Core/Domain/Services/ShiftDetectionRule.cs`) must recognise Postgres interval syntax. Domain tests import `NpgsqlTypes.NpgsqlInterval` to build sample payloads, which raises EXXER600.
- **Mitigation**: Whitelist provider-specific namespaces when they are used only for parsing/formatting values inside test assemblies or domain constants.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Postgres_Interval_Test_Data()
  {
      const string code = @"
using NpgsqlTypes;

namespace IndTrace.Domain.UnitTests.Services;

public class ShiftDetectionRuleTests
{
    [Fact]
    public void Should_Parse_Postgres_Intervals()
    {
        var duration = new NpgsqlInterval(hours: 8, minutes: 0, seconds: 0);
    }
}";

      AnalyzerTestHelper
          .RunAnalyzer(code, new DomainShouldNotReferenceInfrastructureAnalyzer(),
              filePath: @""C:\src\Test Project\Src\Tests\Core\Domain.UnitTests\ShiftsTests\ShiftDetectionRuleTests.cs"")
          .ShouldBeEmpty();
  }
  ```

### 8. Domain Enum Synchronisation Scripts
- **Problem**: Domain enumerations reference SQL table names for synchronisation. Team uses small migration utilities in `Test Project/Src/Tests/Core/Domain.UnitTests/Enum/LookUpTable/EnumSyncTests.cs` that import `System.Data.Odbc` to validate DSN formatting. Analyzer blocks these utilities.
- **Mitigation**: Skip provider namespace diagnostics when the containing namespace ends with `.Enum` or `.EnumTests` and the code never opens a connection (analysis limited to builder/formatter usage).
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Odbc_Dsn_Format_Checks()
  {
      const string code = @"
using System.Data.Odbc;

namespace IndTrace.Domain.UnitTests.Enum.LookUpTable;

public static class LookupSyncDsnValidator
{
    public static bool IsValid(string dsn) =>
        new OdbcConnectionStringBuilder(dsn).ContainsKey(""Driver"");
}";

      AnalyzerTestHelper
          .RunAnalyzer(code, new DomainShouldNotReferenceInfrastructureAnalyzer(),
              filePath: @""C:\src\Test Project\Src\Tests\Core\Domain.UnitTests\Enum\LookUpTable\LookupSyncDsnValidator.cs"")
          .ShouldBeEmpty();
  }
  ```

### 9. ValueComparer Usage for Domain Collections
- **Problem**: `AuditableEntity` tests compare `ICollection<T>` snapshots using EF Core `ValueComparer`. Importing `Microsoft.EntityFrameworkCore.ChangeTracking` inside `IndTrace.Domain.UnitTests.AuditableEntitiesTests` is blocked.
- **Mitigation**: Allow EF Core change-tracking namespaces when usage is limited to constructing `ValueComparer<T>` instances in tests.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_ValueComparer_In_Test()
  {
      const string code = @"
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace IndTrace.Domain.UnitTests.AuditableEntitiesTests;

public class AuditableEntityTests
{
    [Fact]
    public void Should_Compare_ChangeLogs()
    {
        var comparer = new ValueComparer<int[]>(true);
    }
}";

      AnalyzerTestHelper
          .RunAnalyzer(code, new DomainShouldNotReferenceInfrastructureAnalyzer(),
              filePath: @""C:\src\Test Project\Src\Tests\Core\Domain.UnitTests\AuditableEntitiesTests\AuditableEntityTests.cs"")
          .ShouldBeEmpty();
  }
  ```

### 10. Migration Snapshot Verification in Domain Unit Tests
- **Problem**: Domain regression tests verify that enum snapshots match EF migrations stored in infrastructure. They import `Microsoft.EntityFrameworkCore.Migrations` inside `IndTrace.Domain.UnitTests.ResultsTests` to parse snapshot model metadata, tripping EXXER600.
- **Mitigation**: Permit EF Core migration namespaces when used inside unit-test assemblies for metadata comparison only (no runtime migrations).
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Migration_Metadata_Parsing()
  {
      const string code = @"
using Microsoft.EntityFrameworkCore.Migrations;

namespace IndTrace.Domain.UnitTests.ResultsTests;

public class MigrationSnapshotTests
{
    [Fact]
    public void Should_Read_ModelSnapshot_Metadata()
    {
        var builder = new MigrationBuilder(activeProvider: ""SqlServer"");
    }
}";

      AnalyzerTestHelper
          .RunAnalyzer(code, new DomainShouldNotReferenceInfrastructureAnalyzer(),
              filePath: @""C:\src\Test Project\Src\Tests\Core\Domain.UnitTests\ResultsTests\MigrationSnapshotTests.cs"")
          .ShouldBeEmpty();
  }
  ```

## 4. Test-Driven Fix Strategy

- Extend the analyzer to gather semantic information for each `using` directive. Only report when the imported namespace resolves to types deriving from EF Core context or ADO provider *and* the containing assembly/namespace is a production domain assembly (exclude paths ending with `.Tests`, `.UnitTests`, `.IntegrationTests`).  
- Permit metadata attributes and helper types (e.g., `OwnedAttribute`, `SqlConnectionStringBuilder`, `ValueComparer<T>`) by adding allowlists verified via symbol analysis.  
- Augment `AnalyzerTestHelper` to accept custom project metadata (assembly name, file path) so unit tests can mimic domain vs test assemblies easily.  
- Add regression tests for the ten scenarios above plus retaining existing positive coverage (domain entity invoking `new SqlConnection().Open()` should still fire).  
- Re-run analyzer suites and ensure `dotnet build IndFusion.sln -c Release` produces zero EXXER600 diagnostics on `Test Project` once mitigations are in place.
