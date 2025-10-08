# DoNotMockDbContext Analyzer - False-Positive Mitigation Spec

**Analyzer ID**: `EXXER104`  
**Source**: `src/code/IndFusion.Analyzer/Testing/DoNotMockDbContextAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-08)

## 0. Selection Rationale
- `docs/specs/Analyzer103Specs.md` is absent, but the EXXER103 analyzer only inspects `using Moq;`. A repository-wide search across the mandated scopes (`src/test`, `Test Project/Src`) shows no `using Moq;` statements, so no reproducible false positives exist. Per **TaskAnalyzer.md** we advance to the next analyzer without a spec.
- `DoNotMockDbContextAnalyzer` (EXXER104) is the immediate successor and already produces noise in the IndTrace sample solution because its heuristics treat any `Mock<T>` where `T` ends with `Context` as an EF Core context.
- High-impact examples observed while reviewing IndTrace source and tests:
  - `MachineConfigContext` domain record (`Test Project/Src/Code/Core/Application/Machines/Queries/GetMachinesConfig/DataLoaders/IMachineConfigDataLoader.cs:26`) is routinely constructed and occasionally substituted in tests. Attempting to mock it triggers EXXER104 even though it has no relationship to `Microsoft.EntityFrameworkCore.DbContext`.
  - Aggregation flows rely on `CreateBarcodeContext` and `BarCodeCreationContext` helper types (`Test Project/Src/Code/Core/Application/BarCodes/Commands/Create/CreateBarCodeCommandHandler.cs:54`, `CreateBarCodeCommandHandlerRefactored.cs:322`). Mocking these orchestration records to drive command handlers is flagged incorrectly.
  - Presentation middleware and identity helpers depend on ASP.NET Core’s `HttpContext` (`Test Project/Src/Code/Infrastructure/IndTrace.Identity/Components/Account/IdentityRedirectManager.cs:70`). Mocking `HttpContext` is a legitimate test technique but currently triggers EXXER104.
- Because EXXER104 is shipped as an **error**, these false positives block builds; mitigating them is critical before the analyzer can be adopted.

## 1. Specification
- **Intent**  
  Discourage mocking of EF Core `DbContext` instances so that tests exercise the InMemory provider or real database fixtures instead of substituting the context.

- **Scope**  
  Registers syntax node actions for `ObjectCreationExpression`, `InvocationExpression`, and `GenericName`. It reports EXXER104 when:
  - `new Mock<DbContextLike>()`
  - `Mock.Of<DbContextLike>()`
  - `Substitute.For<DbContextLike>()`
  - Any `Mock<...>` type argument that ends with `DbContext` or `Context`, based purely on string comparison.
  - `GenericName` tokens named `Mock` with `Context`-suffixed type arguments.
  The implementation relies on `ExtractGenericTypeArgument` + `EndsWith("Context")` heuristics before attempting any semantic analysis, so type names that merely contain “Context” are treated as EF contexts.

## 2. Validation Plan
1. Create `DoNotMockDbContextAnalyzerFalsePositiveTests` inside `src/test/IndFusion.Analyzer.Tests/TestCases/Testing/`.
2. Add regression scenarios covering the ten cases below plus positive controls (`Mock<IndTraceDbContext>` should continue to fail).
3. Extend `AnalyzerTestHelper` to allow custom file paths (so we can simulate `Test Project/Src/...` directories) and to assert diagnostic IDs.
4. Run `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release` after each mitigation.
5. Execute targeted solution builds (`dotnet build IndFusion.sln -c Release`) to confirm noise disappears in IndTrace sample projects.

## 3. Enhancement Opportunities (≥10)
Each scenario lists a current false positive, the mitigation, and an xUnit/Shouldly snippet that fails today (EXXER104 reported) but succeeds after the fix.

### 1. MachineConfigContext Records
- **Problem**: `MachineConfigContext` (`Test Project/Src/Code/Core/Application/Machines/Queries/GetMachinesConfig/DataLoaders/IMachineConfigDataLoader.cs:26`) is a simple domain record. Developers mock it to isolate `MachineConfigAssembler`, but `Mock<MachineConfigContext>` is flagged as a DbContext.
- **Mitigation**: Require a semantic check that the type argument derives from `Microsoft.EntityFrameworkCore.DbContext`. If the type symbol is a record/class without that base, stop early.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Domain_Context_Record()
  {
      const string code = @"
using Moq;
namespace Samples;
public sealed record MachineConfigContext(string PartNumber);
public static class Sut
{
    public static void Arrange() => _ = new Mock<MachineConfigContext>();
}";
      var diagnostics = AnalyzerTestHelper.RunAnalyzer(code, new DoNotMockDbContextAnalyzer());
      diagnostics.ShouldBeEmpty(); // currently fails with EXXER104
  }
  ```

### 2. CreateBarcodeContext Pipelines
- **Problem**: `CreateBarCodeCommandHandler` uses a private record `CreateBarcodeContext` (`Test Project/Src/Code/Core/Application/BarCodes/Commands/Create/CreateBarCodeCommandHandler.cs:54`). Mocking that record to test individual pipeline steps produces EXXER104.
- **Mitigation**: When the type argument is an internal record defined in the same tree, inspect its base list. If there is no inheritance chain leading to `DbContext`, skip reporting.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Private_Context_Record()
  {
      const string code = @"
using Moq;
namespace Samples;
public class Handler
{
    private record CreateBarcodeContext(string Label);
    public void Arrange() => _ = new Mock<CreateBarcodeContext>();
}";
      AnalyzerTestHelper
          .RunAnalyzer(code, new DoNotMockDbContextAnalyzer())
          .ShouldBeEmpty();
  }
  ```

### 3. BarCodeCreationContext Workflow Class
- **Problem**: The refactored handler introduces an inner class `BarCodeCreationContext` (`Test Project/Src/Code/Core/Application/BarCodes/Commands/Create/CreateBarCodeCommandHandlerRefactored.cs:322`). Tests that substitute this class are flagged because the analyzer only inspects the name suffix.
- **Mitigation**: For nested types, obtain the semantic symbol from the `GenericName` node and evaluate its base types. Ignore when the symbol lives within the current assembly and lacks `DbContext` ancestry.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Nested_Context_Class()
  {
      const string code = @"
using Moq;
namespace Samples;
public sealed class Handler
{
    private class BarCodeCreationContext { }
    public void Arrange() => _ = new Mock<BarCodeCreationContext>();
}";
      AnalyzerTestHelper
          .RunAnalyzer(code, new DoNotMockDbContextAnalyzer())
          .ShouldBeEmpty();
  }
  ```

### 4. PlcDetailContext Aggregations
- **Problem**: Aggregation queries use `PlcDetailContext` (`Test Project/Src/Code/Core/Application/Plcs/Queries/GetDetail/DataLoaders/IPlcDetailDataLoader.cs:27`). Mocking or substituting this record to validate assemblers fires EXXER104.
- **Mitigation**: Treat `record` declarations (syntactic check) as non-EF contexts unless semantic analysis proves they inherit `DbContext`.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Record_Context_In_Aggregations()
  {
      const string code = @"
using NSubstitute;
namespace Samples;
public sealed record PlcDetailContext(int PlcId);
public static class Sut =>
    Substitute.For<PlcDetailContext>();";
      AnalyzerTestHelper
          .RunAnalyzer(code, new DoNotMockDbContextAnalyzer())
          .ShouldBeEmpty();
  }
  ```

### 5. IDataContext Infrastructure Abstraction
- **Problem**: `SafeRepository` consumes `IDataContext` (`Test Project/Src/Code/Core/Application/Repository/SafeRepository.cs:31`). The interface represents a unit-of-work abstraction, not EF core. Mocking `IDataContext` trips the analyzer because the name ends with “Context”.
- **Mitigation**: When the type argument is an interface, check whether its members or base interfaces ultimately derive from `Microsoft.EntityFrameworkCore.DbContext`. Pure abstractions should be ignored.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Interface_Based_Context()
  {
      const string code = @"
using Moq;
namespace Samples;
public interface IDataContext { }
public sealed class Repo
{
    public Repo()
    {
        _ = new Mock<IDataContext>();
    }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new DoNotMockDbContextAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 6. MSTest TestContext Utility
- **Problem**: Legacy test harnesses still rely on MSTest’s `TestContext` (`Test Project/Src/Tests/Core/Aggregation.BoundedTests/.../CreateBarCodeCommandTests.cs:164`). Mocking `TestContext` to control cancellation tokens triggers EXXER104.
- **Mitigation**: Recognize framework-provided contexts (MSTest `TestContext`, NUnit `TestContext`, xUnit `TestContext`) and whitelist them explicitly.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_MSTest_TestContext()
  {
      const string code = @"
using Moq;
namespace Samples;
public class TestContext { }
public static class Harness
{
    public static TestContext Create() => new Mock<TestContext>().Object;
}";
      AnalyzerTestHelper.RunAnalyzer(code, new DoNotMockDbContextAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 7. HttpContext Middleware Testing
- **Problem**: Middleware such as `ErrorHandlingMiddleware` (`Test Project/Src/Code/Core/Application/Models/Middlewares/ErrorHandlingMiddleware.cs:36`) legitimately mocks `Microsoft.AspNetCore.Http.HttpContext`. The analyzer treats `Mock<HttpContext>` as a violation.
- **Mitigation**: Exempt ASP.NET Core HTTP abstractions (`Microsoft.AspNetCore.Http.HttpContext`, `HttpRequest`, `HttpResponse`) since they do not derive from EF `DbContext`.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_AspNet_HttpContext()
  {
      const string code = @"
using Moq;
using Microsoft.AspNetCore.Http;
namespace Samples;
public static class MiddlewareHarness
{
    public static HttpContext Arrange() => new Mock<HttpContext>().Object;
}";
      AnalyzerTestHelper.RunAnalyzer(code, new DoNotMockDbContextAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 8. ValidationContext in Request Validation
- **Problem**: `ValidationBehavior` constructs `ValidationContext<TRequest>` via `System.ComponentModel.DataAnnotations.ValidationContext` (`Test Project/Src/Code/Core/Application/Models/Behaviors/ValidationBehavior.cs:47`). Tests that mock `ValidationContext` to surface metadata suffer false positives.
- **Mitigation**: Treat BCL validation contexts as safe. Use fully qualified namespace matching (`System.ComponentModel.DataAnnotations.ValidationContext`).
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_DataAnnotations_ValidationContext()
  {
      const string code = @"
using Moq;
using System.ComponentModel.DataAnnotations;
namespace Samples;
public static class ValidatorHarness
{
    public static ValidationContext Create(object target) =>
        new Mock<ValidationContext>(target).Object;
}";
      AnalyzerTestHelper.RunAnalyzer(code, new DoNotMockDbContextAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 9. IDbContextFactory<T> Test Doubles
- **Problem**: Tests commonly substitute `IDbContextFactory<IndTraceDbContext>` to supply pre-built contexts (`Test Project/Src/Tests/Integration/Indtrace.Integration.Tests/Utilities/KeyedPersistenceRegistrationTest.cs:44`). The analyzer interprets `Substitute.For<IDbContextFactory<...>>()` as mocking a `DbContext`.
- **Mitigation**: When the outer generic type is `IDbContextFactory<T>`, inspect the symbol – if the constructed type is from `Microsoft.EntityFrameworkCore`, allow mocking because the factory is an abstraction, not the context itself.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_DbContextFactory_Substitute()
  {
      const string code = @"
using NSubstitute;
using Microsoft.EntityFrameworkCore;
namespace Samples;
public class IndTraceDbContext : DbContext { }
public static class Harness =>
    Substitute.For<IDbContextFactory<IndTraceDbContext>>();
";
      AnalyzerTestHelper.RunAnalyzer(code, new DoNotMockDbContextAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 10. Custom TestContextLogger Helpers
- **Problem**: Integration fixtures define helper classes such as `TestContextLogger` (`Test Project/Src/Tests/Integration/Indtrace.Integration.Tests/Infrastructure/TestHostFixture.cs:368`). These are plain loggers named “Context”. Mocking them (e.g., to verify telemetry) triggers EXXER104.
- **Mitigation**: For types declared inside the test project whose namespace starts with `Integration.Tests.Infrastructure`, skip EXXER104 because they are diagnostic utilities, not EF contexts.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_TestContextLogger_Helper()
  {
      const string code = @"
using Moq;
namespace Integration.Tests.Infrastructure;
public sealed class TestContextLogger { }
public static class Sut
{
    public static TestContextLogger Arrange() =>
        new Mock<TestContextLogger>().Object;
}";
      AnalyzerTestHelper.RunAnalyzer(code, new DoNotMockDbContextAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

## 4. Test-Driven Fix Strategy
- Add `DoNotMockDbContextAnalyzerFalsePositiveTests` containing the ten scenarios above plus a positive-control test that confirms `Mock<IndTraceDbContext>` still raises EXXER104.
- Update `DoNotMockDbContextAnalyzer`:
  - Resolve the type symbol from every candidate and require `IsDerivedFrom("Microsoft.EntityFrameworkCore.DbContext")`.
  - Introduce explicit allowlists for MSTest/NUnit/xUnit test contexts, ASP.NET HTTP contexts, DataAnnotations validation contexts, and in-repo helper namespaces (`Integration.Tests.Infrastructure`, `Test Project/Src/Code/Core/Application/**` records).
  - Recognize `IDbContextFactory<T>` and other factory abstractions; only flag when the generic argument itself inherits `DbContext`.
  - Remove the raw `EndsWith("Context")` guard once semantic checks are in place.
- Extend `AnalyzerTestHelper` to accept optional file paths so repro snippets can mimic the IndTrace folder structure (useful for namespace-based allowlists).
- Run analyzer tests (`dotnet test ...IndFusion.Analyzer.Tests.csproj -c Release`) and rebuild the solution to verify EXXER104 disappears from legitimate mocking scenarios.
