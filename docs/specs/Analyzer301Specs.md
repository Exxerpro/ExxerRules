# Epic: EXXER301 - UseConfigureAwaitFalse Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER301`  
**Source**: `src/code/IndFusion.Analyzer/Async/UseConfigureAwaitFalseAnalyzer.cs`  
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

### 1.1. Story: Exempt Test Methods

**As a** developer writing tests  
**I want** the analyzer to ignore `await` expressions within test methods  
**So that** I don't get unnecessary warnings in my test code.

#### 1.1.1. Acceptance Criteria

**Given** an `await` expression is inside a method decorated with a test attribute (e.g., `[Fact]`, `[Theory]`, `[TestMethod]`, `[Test]`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Exempt Test Helper Methods

**As a** developer writing tests  
**I want** the analyzer to ignore `await` expressions within helper methods in test classes  
**So that** I can structure my test code with async helpers without getting warnings.

#### 1.2.1. Acceptance Criteria

**Given** an `await` expression is inside a method within a class whose name ends with `Tests`, `Test`, `Specs`, or `Spec`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Exempt Test-Related Namespaces

**As a** developer writing tests  
**I want** the analyzer to ignore `await` expressions in test-related namespaces  
**So that** my test utility projects are not flagged.

#### 1.3.1. Acceptance Criteria

**Given** an `await` expression is in a file within a namespace containing `.Tests` or `.TestUtilities`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Exempt `IAsyncLifetime` Implementations

**As a** developer using xUnit fixtures  
**I want** the analyzer to ignore `await` expressions in `IAsyncLifetime` methods  
**So that** I can implement async fixture setup and teardown.

#### 1.4.1. Acceptance Criteria

**Given** an `await` expression is inside an implementation of `IAsyncLifetime.InitializeAsync` or `IAsyncLifetime.DisposeAsync`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Exempt Collection and Assembly Fixtures

**As a** developer using xUnit fixtures  
**I want** the analyzer to ignore `await` expressions in collection and assembly fixtures  
**So that** I can write complex test setup logic without warnings.

#### 1.5.1. Acceptance Criteria

**Given** an `await` expression is inside a class whose name ends with `Fixture` and is part of a `[CollectionDefinition]` or `[Collection]`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Exempt Blazor Component Lifecycle Methods

**As a** developer of Blazor components  
**I want** the analyzer to ignore `await` expressions in component lifecycle methods  
**So that** I can perform async operations during component initialization and rendering.

#### 1.6.1. Acceptance Criteria

**Given** an `await` expression is inside a Blazor component lifecycle method (e.g., `OnInitializedAsync`) in a class inheriting from `ComponentBase`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Exempt Blazor EventCallback Handlers

**As a** developer of Blazor components  
**I want** the analyzer to ignore `await` expressions in event handlers  
**So that** I can write async event handling logic without warnings.

#### 1.7.1. Acceptance Criteria

**Given** an `await` expression is inside a private async method that handles a UI event in a Blazor component  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Exempt Awaits on Expressions Without `ConfigureAwait` Overloads

**As a** developer  
**I want** the analyzer to ignore `await` expressions on tasks that do not have a `ConfigureAwait` overload  
**So that** I am not warned about code that cannot be changed.

#### 1.8.1. Acceptance Criteria

**Given** an `await` expression is used on a task-like object that does not have a `ConfigureAwait` method (e.g., `Task.WhenAll`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).