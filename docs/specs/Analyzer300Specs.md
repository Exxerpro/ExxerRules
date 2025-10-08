# AsyncMethodsShouldAcceptCancellationToken Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER300`  
**Source**: `src/code/IndFusion.Analyzer/Async/AsyncMethodsShouldAcceptCancellationTokenAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 1. Intent, Scope, and Validation Strategy

- **Intent**  
  Guard library-style code against async operations that cannot be cancelled by requiring an explicit `CancellationToken` parameter on eligible methods.

- **Scope**  
  Applies to `async` methods (`MethodDeclarationSyntax`) that return `Task`/`Task<T>` (non-`void`), excluding generated code and obvious application-entry points as implemented by `IsSkippableMethod` and `IsInBoundaryLayer`.

- **Validation Plan**  
  1. Extend analyzer unit tests in `IndFusion.Analyzer.Tests` with scenario-focused TDD cases for each mitigation below.  
  2. Validate across `Test Project\Src` using `dotnet test` to ensure real-world regression coverage.  
  3. Capture diagnostic baselines for key solutions before/after changes to ensure no regression in true-positive coverage (e.g., maintain warnings on domain services).  
  4. Backstop with integration smoke test: build and run `src/test/IndFusion.Analyzer.Tests` with the analyzer referenced to confirm no unhandled edge cases or performance regressions.

## 2. False-Positive Hotspots Observed in IndTrace

| Hotspot | Evidence |
|---------|----------|
| Blazor component lifecycle overrides (e.g., `protected override async Task OnInitializedAsync()`) | Numerous matches across `.razor(.cs)` files, e.g., `Test Project\Src\Code\Presentation\IndTrace.OEE\Components\Account\Shared\ManageNavMenu.razor:21`. |
| SignalR hub overrides (`Hub.OnConnectedAsync`/`OnDisconnectedAsync`) | `Test Project\Src\Tests\Integration\HubConnection.IntegrationTests\Fixtures\SignalRTestFixture.cs:77-84`. |
| xUnit test methods (`[Fact]`, `[Theory]`) | Hundreds of `async Task` test methods, e.g., `Test Project\Src\Tests\Presentation\IndTrace.Oee.Tests\IndTrace.Oee.Tests\RepoPlcServiceTests.cs:37`. |
| Test helpers inside `*Tests` classes (private `async Task` helpers) | `Test Project\Src\Tests\Core\HubConnection.Tests\Unit\Implementations\SignalRHubConnectionAdapterTests.cs:114`. |
| `IAsyncLifetime` contracts (`InitializeAsync`, `DisposeAsync`) | `Test Project\Src\Tests\Integration\HubConnection.IntegrationTests\Resilience\ConnectionResilienceTests.cs:21`. |
| Test fixtures (`*Fixture`) providing async setup/teardown | `Test Project\Src\Tests\Integration\Indtrace.Integration.Tests\Infrastructure\TestHostFixture.cs:39`. |
| UI event handlers bound via `EventCallback` (`private async Task OnValidSubmitAsync()`) | `Test Project\Src\Code\Presentation\IndTrace.Components\Area\Products\ProductsForm.razor:184`. |
| UI change handlers with framework args (`ChangeEventArgs`, `MouseEventArgs`) | `Test Project\Src\Code\Presentation\IndTrace.Components\Area\Products\ProductsList.razor.cs:148`. |
| Methods awaiting APIs without cancellation overloads (e.g., `AuthenticationService.LoginAsync()`) | `Test Project\Src\Code\Presentation\IndTrace.Monitor\Components\Account\Pages\Login.razor:77`. |
| Explicit interface implementations enforced by third-party frameworks | `Test Project\Src\Tests\Core\Agregation.Dependices\Agregation.Dependices\Dependencies\DependenciesFactory.cs:23` (`IAsyncLifetime.InitializeAsync`). |

These hotspots demonstrate why EXXER300 currently dominates false positives in the IndTrace test project.

## 3. Enhancement Backlog (>=10 Items)

| # | Mitigation Theme | Goal |
|---|------------------|------|
| 1 | Override & explicit-interface exemption | Skip signatures that are mandated by a base type or interface. |
| 2 | Blazor lifecycle override guardrail | Detect `ComponentBase` overrides explicitly, even if `override` keyword is absent (metadata-only partial). |
| 3 | SignalR hub override guardrail | Exempt `Hub` lifecycle overrides (`OnConnectedAsync`, `OnDisconnectedAsync`). |
| 4 | Attribute-based test detection | Suppress diagnostics on methods annotated with `[Fact]`, `[Theory]`, `[Test]`, `[TestMethod]`, etc. |
| 5 | Test-class heuristic | Suppress diagnostics in classes whose names end with `Tests`, `Specs`, or `TestFixture`, even for helper methods. |
| 6 | `IAsyncLifetime` contract awareness | Skip `InitializeAsync`/`DisposeAsync` implementations. |
| 7 | Fixture pattern recognition | Skip async methods in classes ending with `Fixture` that implement `IAsyncLifetime` or contain `[CollectionDefinition]`. |
| 8 | Blazor `EventCallback` handler detection | Skip private async handlers wire-bound to UI events. |
| 9 | Cancellation availability analysis | Only flag awaited calls when an overload accepting `CancellationToken` exists. |
|10 | Captured token awareness | Skip diagnostics when a method accesses an existing `CancellationToken` (field/property/ambient context). |

## 4. Detailed Proposals (Test-Driven)

### 1. Override & Explicit Interface Exemption
- **Problem**: Overrides like `protected override async Task OnInitializedAsync()` cannot change their signature yet trigger EXXER300.
- **Proposed Behaviour**: If `IMethodSymbol.IsOverride` is true or `ExplicitInterfaceImplementations` is non-empty, do not report.
- **TDD Plan**:
  - Add `Does_Not_Report_For_Overrides` test case creating a derived class overriding `Task OnInitializedAsync()` without a token.
  - Add `Does_Not_Report_For_Explicit_Interface_Impl` case using an interface with an async method.
- **Implementation Notes**: Extend `AnalyzeMethod` to pull the method symbol and short-circuit prior to cancellation checks.

### 2. Blazor Lifecycle Override Guardrail
- **Problem**: Some `.razor` partial classes compile methods marked `protected async Task OnInitializedAsync()` without the explicit `override` token (metadata applied in generated partial).
- **Proposed Behaviour**: When the containing type inherits from `Microsoft.AspNetCore.Components.ComponentBase` (or derived), skip lifecycle methods (`OnInitializedAsync`, `OnParametersSetAsync`, `OnAfterRenderAsync`).
- **TDD Plan**:
  - Add `Does_Not_Report_For_ComponentBaseLifecycle` test that synthesizes a partial `class SampleComponent : ComponentBase`.
  - Validate both with and without the `override` keyword.
- **Implementation Notes**: Inspect `methodSymbol.ContainingType.BaseType` chain for `ComponentBase`.

### 3. SignalR Hub Override Guardrail
- **Problem**: SignalR hubs often override `Task OnConnectedAsync()` without cancellation support.
- **Proposed Behaviour**: Skip diagnostics when containing type derives from `Microsoft.AspNetCore.SignalR.Hub` and method name is `OnConnectedAsync` or `OnDisconnectedAsync`.
- **TDD Plan**:
  - Add `Does_Not_Report_For_HubLifecycle` covering both overrides.
- **Implementation Notes**: Mirror approach from Proposal 2 by checking base-type metadata.

### 4. Attribute-Based Test Detection
- **Problem**: xUnit and NUnit test methods (`[Fact]`, `[Theory]`, `[Test]`, `[TestMethod]`) are reported even though test frameworks drive the signature.
- **Proposed Behaviour**: Detect the presence of known test attributes on method declarations and skip diagnostics.
- **TDD Plan**:
  - Add cases for `[Fact]` (xUnit), `[Theory]` (xUnit v3), `[Test]` (NUnit), `[TestMethod]` (MSTest).
  - Ensure unaffected methods still report.
- **Implementation Notes**: Reuse `PatternDetector.DetectTestAttributes` for attribute evaluation.

### 5. Test-Class Heuristic
- **Problem**: Helper methods within `*Tests` classes (without attributes) still raise EXXER300.
- **Proposed Behaviour**: If the containing class name ends with `Tests`, `Test`, `Specs`, or `Spec`, skip diagnostics.
- **TDD Plan**:
  - Add `Does_Not_Report_For_Helper_In_TestClass` verifying private async helpers inside an attribute-decorated test class.
- **Implementation Notes**: Expand `IsApplicationCode` or introduce a dedicated test-scoping helper.

### 6. `IAsyncLifetime` Contract Awareness
- **Problem**: xUnit fixtures implementing `IAsyncLifetime.InitializeAsync()` & `DisposeAsync()` must match exact signatures.
- **Proposed Behaviour**: If the method implements `Xunit.IAsyncLifetime.InitializeAsync` or `.DisposeAsync`, skip diagnostics.
- **TDD Plan**:
  - Add `Does_Not_Report_For_IAsyncLifetime_InitializeAsync` and `_DisposeAsync` cases.
- **Implementation Notes**: Inspect `methodSymbol.ContainingType.AllInterfaces` for `IAsyncLifetime`.

### 7. Fixture Pattern Recognition
- **Problem**: Async setup routines in classes ending with `Fixture` (e.g., `TestHostFixture`) are invoked by xUnit collection fixtures.
- **Proposed Behaviour**: Skip diagnostics for public `async Task` members inside classes whose names end with `Fixture` and decorated with `[CollectionDefinition]` or implementing fixture marker interfaces.
- **TDD Plan**:
  - Add `Does_Not_Report_For_CollectionFixtureAsyncSetup` test.
- **Implementation Notes**: Combine naming and attribute checks to minimise overreach.

### 8. Blazor `EventCallback` Handler Detection
- **Problem**: Private `async Task` handlers (e.g., `private async Task OnValidSubmitAsync()`) wired to `EventCallback` cannot accept extra parameters.
- **Proposed Behaviour**: Skip diagnostics when method is `private`, resides in a `ComponentBase` descendant, and parameters match known event args (`ChangeEventArgs`, `MouseEventArgs`, etc.) or none.
- **TDD Plan**:
  - Add `Does_Not_Report_For_EventCallbackHandler` using a component partial class referencing a handler.
- **Implementation Notes**: Optionally confirm the method is referenced by an `EventCallback` field/property (future work), but initial heuristic can rely on naming (`On` prefix) + component context.

### 9. Cancellation Availability Analysis
- **Problem**: Methods that await APIs without any cancellation overload (e.g., frameworks that omit a `CancellationToken`) are currently flagged unfairly.
- **Proposed Behaviour**: Inspect each awaited invocation – if no overload or delegate parameter accepts `CancellationToken`, downgrade/skip the diagnostic.
- **TDD Plan**:
  - Add two tests: `Reports_When_Cancellation_Overload_Exists` and `Does_Not_Report_When_No_Cancellation_Overload`.
- **Implementation Notes**: Use Roslyn symbol analysis to examine candidate method groups; requires caching for performance.

### 10. Captured Token Awareness
- **Problem**: Some classes capture a `CancellationToken` via constructor or ambient context (`TestContext.Current.CancellationToken`) and use it internally, making an additional parameter redundant.
- **Proposed Behaviour**: Skip diagnostics if the method references a field/property of type `CancellationToken` or `CancellationTokenSource.Token`.
- **TDD Plan**:
  - Add `Does_Not_Report_When_Token_Field_Used` demonstrating a class storing a token in a private readonly field and using it in the method body.
- **Implementation Notes**: Add a semantic walk over the method body to detect member access expressions returning `CancellationToken`.

## 5. Delivery Checklist

- [ ] Implement mitigation logic for items 1–10 in `AsyncMethodsShouldAcceptCancellationTokenAnalyzer`.  
- [ ] Add targeted unit tests under `src/test/IndFusion.Analyzer.Tests/Async/AsyncMethodsShouldAcceptCancellationTokenAnalyzerTests.cs`.  
- [ ] Backfill integration sample(s) in `Test Project\Src\Tests` to confirm diagnostics disappear.  
- [ ] Update analyzer release notes and bump version in `AnalyzerReleases.Unshipped.md`.  
- [ ] Run `dotnet format` and `dotnet test` for both analyzer and IndTrace solutions prior to publishing.
