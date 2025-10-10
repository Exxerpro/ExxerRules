# Epic: EXXER300 - AsyncMethodsShouldAcceptCancellationToken Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER300`  
**Source**: `src/code/IndFusion.Analyzer/Async/AsyncMethodsShouldAcceptCancellationTokenAnalyzer.cs`  
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

### 1.1. Story: Exempt Overridden and Explicitly Implemented Methods

**As a** developer  
**I want** the analyzer to ignore async methods that are overrides or explicit interface implementations  
**So that** I am not forced to change method signatures that are constrained by a base type or interface.

#### 1.1.1. Acceptance Criteria

**Given** an async method is marked with the `override` keyword  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

**Given** an async method is an explicit interface implementation  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Exempt Blazor Lifecycle Methods

**As a** developer of Blazor components  
**I want** the analyzer to ignore Blazor lifecycle methods like `OnInitializedAsync`  
**So that** I can implement component logic without getting false positives.

#### 1.2.1. Acceptance Criteria

**Given** an async method is a Blazor lifecycle override (e.g., `OnInitializedAsync`, `OnParametersSetAsync`) in a class inheriting from `ComponentBase`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Exempt SignalR Hub Lifecycle Methods

**As a** developer of SignalR hubs  
**I want** the analyzer to ignore SignalR hub lifecycle methods like `OnConnectedAsync`  
**So that** I can implement hub logic without getting false positives.

#### 1.3.1. Acceptance Criteria

**Given** an async method is a SignalR hub lifecycle override (e.g., `OnConnectedAsync`, `OnDisconnectedAsync`) in a class inheriting from `Hub`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Exempt Test Methods

**As a** developer writing tests  
**I want** the analyzer to ignore async test methods  
**So that** I can write tests without being required to add a `CancellationToken` parameter.

#### 1.4.1. Acceptance Criteria

**Given** an async method is decorated with a test attribute (e.g., `[Fact]`, `[Theory]`, `[Test]`, `[TestMethod]`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Exempt Test Class Helper Methods

**As a** developer writing tests  
**I want** the analyzer to ignore async helper methods within test classes  
**So that** I can structure my test code cleanly.

#### 1.5.1. Acceptance Criteria

**Given** an async method is a helper method within a test class (a class with a name ending in `Tests`, `Specs`, or `TestFixture`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Exempt `IAsyncLifetime` Contract Methods

**As a** developer using xUnit fixtures  
**I want** the analyzer to ignore `InitializeAsync` and `DisposeAsync` methods from `IAsyncLifetime`  
**So that** I can implement test fixture setup and teardown logic.

#### 1.6.1. Acceptance Criteria

**Given** an async method is an implementation of `IAsyncLifetime.InitializeAsync` or `IAsyncLifetime.DisposeAsync`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Exempt Test Fixture Methods

**As a** developer using xUnit fixtures  
**I want** the analyzer to ignore async methods in test fixture classes  
**So that** I can write async setup and teardown logic for my fixtures.

#### 1.7.1. Acceptance Criteria

**Given** an async method is in a class whose name ends with `Fixture` and that implements `IAsyncLifetime` or is part of a `CollectionDefinition`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Exempt Blazor `EventCallback` Handlers

**As a** developer of Blazor components  
**I want** the analyzer to ignore private async event handlers  
**So that** I can handle UI events without getting false positives.

#### 1.8.1. Acceptance Criteria

**Given** a private async method in a Blazor component appears to be an event handler (e.g., `OnValidSubmitAsync`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Analyze Cancellation Availability

**As a** developer  
**I want** the analyzer to only flag awaited calls when a `CancellationToken` overload is available  
**So that** I am not warned about methods that don't support cancellation.

#### 1.9.1. Acceptance Criteria

**Given** an async method awaits a call to another method  
**And** the awaited method does not have an overload that accepts a `CancellationToken`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Be Aware of Captured Tokens

**As a** developer  
**I want** the analyzer to detect when a `CancellationToken` is already available in the scope  
**So that** I am not required to add a redundant `CancellationToken` parameter.

#### 1.10.1. Acceptance Criteria

**Given** an async method has access to a `CancellationToken` through a field, property, or ambient context  
**And** the method uses this token in its async operations  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the missing parameter.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).