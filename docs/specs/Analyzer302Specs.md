# Epic: EXXER302 - AvoidAsyncVoid Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER302`  
**Source**: `src/code/IndFusion.Analyzer/Async/AvoidAsyncVoidAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## Definition of Ready

- [ ] Sufficient context about the implementation has been collected.
- [ ] The document has been updated with a detailed plan.
- [ ] All dependencies and potential blockers have been identified.
- [ ] The team has reviewed and agreed upon the plan.

## Definition of Done

- [ ] All stories are complete and meet their acceptance criteria.
- [ ] All new regression tests are added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build warnings treated as errors, and 0 failing tests on all the test suite.
- [ ] Documentation updated (this spec + release notes).
- [ ] The project builds successfully without any new warnings or errors.

---

## Stories

### 1.1. Story: Allow Nullable Event Handler Parameters

**As a** developer  
**I want** the analyzer to allow nullable parameters in event handlers  
**So that** I can use nullable reference types in my UI code without getting false positives.

#### 1.1.1. Acceptance Criteria

**Given** an `async void` method has parameters that match the event handler pattern but with nullable annotations (e.g., `object? sender`, `EventArgs? e`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Allow Derived EventArgs Types

**As a** developer  
**I want** the analyzer to allow event handlers with event argument types that derive from `EventArgs`  
**So that** I can use custom event arguments in my application.

#### 1.2.1. Acceptance Criteria

**Given** an `async void` method has a second parameter that derives from `System.EventArgs`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Allow Routed Event Patterns

**As a** developer using WPF or WinUI  
**I want** the analyzer to allow `RoutedEventArgs` in event handlers  
**So that** I can write event handlers for routed events.

#### 1.3.1. Acceptance Criteria

**Given** an `async void` event handler uses `RoutedEventArgs` or other event argument types from `System.Windows` or `Microsoft.UI` namespaces  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Allow `ICommand.Execute` Methods

**As a** developer using the MVVM pattern  
**I want** the analyzer to allow `async void` for `ICommand.Execute` implementations  
**So that** I can have asynchronous command logic.

#### 1.4.1. Acceptance Criteria

**Given** an `async void` method is an implementation of `System.Windows.Input.ICommand.Execute`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Allow Overridden `async void` Methods

**As a** developer  
**I want** the analyzer to ignore `async void` methods that are overrides  
**So that** I am not warned about method signatures that are constrained by a base class.

#### 1.5.1. Acceptance Criteria

**Given** an `async void` method is marked with the `override` keyword  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Allow Interface Implementations Requiring `void`

**As a** developer  
**I want** the analyzer to allow `async void` when implementing an interface that requires a `void` return type  
**So that** I can implement legacy or COM-interop interfaces.

#### 1.6.1. Acceptance Criteria

**Given** an `async void` method is an implementation of an interface member that returns `void`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Allow Custom EventHandler Delegate Aliases

**As a** developer  
**I want** the analyzer to recognize event handlers that use custom delegate types  
**So that** I can use my own event patterns.

#### 1.7.1. Acceptance Criteria

**Given** an `async void` method's signature matches a custom event handler delegate  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Allow Partial Methods in Blazor Components

**As a** developer of Blazor components  
**I want** the analyzer to allow `async void` for event handlers in partial classes  
**So that** I can write UI logic in my component's code-behind file.

#### 1.8.1. Acceptance Criteria

**Given** an `async void` method in a Blazor component's partial class appears to be an event handler  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Allow Fire-and-Forget Methods with an Attribute

**As a** developer  
**I want** to be able to mark intentional fire-and-forget `async void` methods with an attribute  
**So that** the analyzer will ignore them.

#### 1.9.1. Acceptance Criteria

**Given** an `async void` method is decorated with a specific suppression attribute (e.g., `[FireAndForget]`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).