# UseConfigureAwaitFalse Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER301`  
**Source**: `src/code/IndFusion.Analyzer/Async/UseConfigureAwaitFalseAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- `docs/specs` only contains `Analyzer300Specs.md`; EXXER301 has no spec coverage.  
- The IndTrace testing tree (`Test Project\Src\Tests`) contains 3,199 `await` statements (`rg --no-heading --trim 'await ' ...`), and none of those test helpers append `.ConfigureAwait(false)`.  
- Real files showing diagnostics today:
  - `Test Project\Src\Tests\Presentation\IndTrace.Oee.Tests\IndTrace.Oee.Tests\RepoPlcServiceTests.cs:32` (`[Fact]` method awaiting repository calls).  
  - `Test Project\Src\Tests\Integration\HubConnection.IntegrationTests\Fixtures\SignalRTestFixture.cs:77` (overrides SignalR hub lifecycle methods).  
  - `Test Project\Src\Code\Presentation\IndTrace.OEE\Components\Account\Pages\Login.razor:77` (Blazor event handler).  
- During manual runs, EXXER301 warnings dominate the analyzer output inside the testing solution, outnumbering other IDs.

## 1. Intent, Scope, and Validation Strategy

- **Intent**  
  Encourage library code to avoid context-capturing awaits by ensuring `ConfigureAwait(false)` is appended when appropriate.

- **Scope**  
  Currently fires on every `await` expression unless `HasConfigureAwait` succeeds or simple boundary guards (names containing `Program`, `App`, `.Presentation`, etc.) match. This causes widespread noise in tests, fixtures, and UI boundary code.

- **Validation Plan**  
  1. Extend `IndFusion.Analyzer.Tests` with targeted scenarios listed below; each should fail before the analyzer changes and pass afterward.  
  2. Re-run `dotnet test src/test/IndFusion.Analyzer.Tests` and `dotnet test "Test Project\Src\Tests\...\*.csproj"` to ensure the new mitigations suppress false positives without silencing true positives.  
  3. Capture a diagnostic snapshot on key solutions pre/post patch to verify the warning count drop is intentional.  
  4. Add regression cases to `MoreFalsePositiveTests` and introduce a dedicated `UseConfigureAwaitFalseAnalyzerFalsePositiveTests` fixture to hold the new samples.

## 2. Enhancement Opportunities (>=10 Items)

Each subsection documents (a) the observed false positive, (b) the proposed analyzer tweak, and (c) a TDD-friendly unit test sketch.

### 1. Test Methods Decorated with `[Fact]`/`[Theory]`

- **Scenario**: xUnit methods in files such as `RepoPlcServiceTests.cs` are flagged despite test pipelines not requiring `ConfigureAwait(false)`.  
- **Proposal**: When a method carries `[Fact]`, `[Theory]`, or `[FactAttribute]`, short-circuit reporting.  
- **Sample Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Fact_Method()
{
    const string testCode = @"
using System.Threading.Tasks;
using Xunit;

public class DemoTests
{
    [Fact]
    public async Task Exercise()
    {
        await Task.Delay(10);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

- **Implementation Notes**: Reuse `PatternDetector.DetectTestAttributes` to recognise xUnit attributes.

### 2. MSTest `[TestMethod]` Support

- **Scenario**: Internal harness projects still use MSTest. Awaited calls in `[TestMethod]` bodies generate EXXER301 noise.  
- **Proposal**: Extend attribute detection to include `[TestMethod]` and `[TestInitialize]`.  
- **Sample Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_MSTest_Method()
{
    const string testCode = @"
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class LegacyTests
{
    [TestMethod]
    public async Task Exercise()
    {
        await Task.Delay(10);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

### 3. NUnit `[Test]`/`[TestCase]` Support

- **Scenario**: Shared libraries host NUnit-based regression suites; namespaces such as `IndTrace.Shared.Tests` hit the analyzer.  
- **Proposal**: Treat `[Test]`, `[TestCase]`, and `[TestCaseSource]` similarly to xUnit attributes.  
- **Sample Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_NUnit_Test()
{
    const string testCode = @"
using System.Threading.Tasks;
using NUnit.Framework;

public class NUnitSpec
{
    [Test]
    public async Task Exercise()
    {
        await Task.Delay(10);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

### 4. Helper Methods Inside `*Tests` Classes

- **Scenario**: Private async helpers inside `RepoPlcServiceTests` and similar classes (e.g., `LoadTagsAsync_Helper`) still raise EXXER301.  
- **Proposal**: When the containing type name ends with `Tests`, `Test`, `Specs`, or `Spec`, suppress diagnostics for nested members.  
- **Sample Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Test_Helper_Method()
{
    const string testCode = @"
using System.Threading.Tasks;

public class WidgetTests
{
    public async Task HelperAsync()
    {
        await Task.Delay(10);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

### 5. Namespaces Containing `.Tests` or `.TestUtilities`

- **Scenario**: Even with neutral class names, any await in `namespace IndTrace.Oee.Tests` is noisy.  
- **Proposal**: If the namespace chain contains `.Tests`, `.TestUtilities`, or `.Testing`, skip diagnostics.  
- **Sample Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Tests_Namespace()
{
    const string testCode = @"
using System.Threading.Tasks;

namespace Sample.Component.Tests;

public class AsyncProbe
{
    public async Task RunAsync()
    {
        await Task.Delay(5);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

### 6. `IAsyncLifetime` Implementations

- **Scenario**: Fixture classes such as `TestHostFixture` implement `IAsyncLifetime` and must match the interface signature.  
- **Proposal**: When the method implements `Xunit.IAsyncLifetime.InitializeAsync` or `.DisposeAsync`, skip reporting.  
- **Sample Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_IAsyncLifetime_Method()
{
    const string testCode = @"
using System.Threading.Tasks;
using Xunit;

public class Fixture : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await Task.Delay(10);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

### 7. Collection/Assembly Fixtures (`*Fixture` Classes)

- **Scenario**: Classes ending with `Fixture` that provide async setup (e.g., `SignalRTestFixture`) still raise warnings on helper methods beyond `InitializeAsync`.  
- **Proposal**: When a class name ends with `Fixture` and the file contains `[CollectionDefinition]`/`[Collection]`, treat it as fixture infrastructure and suppress diagnostics for its members.  
- **Sample Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Collection_Fixture_Helper()
{
    const string testCode = @"
using System.Threading.Tasks;
using Xunit;

[CollectionDefinition(""demo"")]
public class DemoCollection : ICollectionFixture<FixtureHelper> { }

public class FixtureHelper
{
    public async Task WarmupAsync()
    {
        await Task.Delay(5);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

### 8. Blazor `ComponentBase` Lifecycle Methods

- **Scenario**: Blazor pages such as `Login.razor` override `OnInitializedAsync` without `ConfigureAwait`, producing false positives despite being UI boundary code.  
- **Proposal**: Detect when the containing type inherits from `Microsoft.AspNetCore.Components.ComponentBase` (including partials) and treat lifecycle methods (`OnInitializedAsync`, `OnParametersSetAsync`, `OnAfterRenderAsync`) as exempt.  
- **Sample Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Component_Lifecycle()
{
    const string testCode = @"
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public partial class SampleComponent : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(1);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

### 9. EventCallback Handlers (`OnXAsync` with UI EventArgs)

- **Scenario**: Private handlers such as `private async Task OnValidSubmitAsync(EditContext context)` are flagged even though they are invoked by the Blazor dispatcher.  
- **Proposal**: When a method is `private`, starts with `On`, and either (a) resides in a `ComponentBase` derivative or (b) accepts known event-args types (`ChangeEventArgs`, `MouseEventArgs`, `EditContext`), skip diagnostics.  
- **Sample Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_EventCallback_Handler()
{
    const string testCode = @"
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

public partial class SampleForm : ComponentBase
{
    private async Task OnValidSubmitAsync(EditContext context)
    {
        await Task.Delay(1);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

### 10. Awaited Expressions Without Cancellation-Friendly Overloads

- **Scenario**: Awaiting `Task.WhenAll`, `Task.WhenAny`, or framework helpers that lack a `ConfigureAwait` overload generates impossible-to-fix warnings.  
- **Proposal**: Inspect the awaiting expression; if the target method group has no accessible overload with a `ConfigureAwait` extension (e.g., the symbol returns `Task` but the member is static), skip the diagnostic.  
- **Sample Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Task_WhenAll()
{
    const string testCode = @"
using System.Threading.Tasks;

public class BatchRunner
{
    public async Task ExecuteAsync()
    {
        await Task.WhenAll(Task.Delay(1), Task.Delay(2));
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

## 3. Test-Driven Implementation Plan

1. Create `UseConfigureAwaitFalseAnalyzerFalsePositiveTests` under `src/test/IndFusion.Analyzer.Tests/TestCases`.  
2. Add the ten snippets above as discrete `[Fact]` cases verifying `ShouldBeEmpty()`.  
3. Introduce companion positive tests to confirm diagnostics still appear in genuine library code (e.g., domain service with missing `ConfigureAwait(false)`).  
4. Update analyzer logic iteratively:
   - Extend attribute detection via semantic model queries.  
   - Add helper methods for namespace/type heuristics.  
   - Enhance `HasConfigureAwait` guard with overload detection.  
   - Cache semantic checks to avoid performance regressions.  
5. Re-run the expanded test suite; confirm new tests fail prior to implementation and pass after.  
6. Capture a before/after diagnostic diff on `Test Project\Src` to demonstrate the false-positive drop.  
7. Update `AnalyzerReleases.Unshipped.md` documenting the behavioural change and new guardrails.

## 4. Acceptance Checklist

- [ ] Analyzer code updated with the heuristics listed above.  
- [ ] Ten new regression tests added to the analyzer test suite and passing.  
- [ ] Diagnostic noise in the testing project confirmed to shrink materially (manual run or pipeline artifact).  
- [ ] Documentation (`AnalyzerReleases.Unshipped.md`) reflects the change.  
- [ ] `dotnet format` & `dotnet test` succeed for solution and analyzer projects.
