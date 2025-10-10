# Epic: EXXER104 - DoNotMockDbContext Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER104`  
**Source**: `src/code/IndFusion.Analyzer/Testing/DoNotMockDbContextAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-08)

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

### 1.1. Story: Allow Mocking of Domain Context Records

**As a** developer  
**I want** the analyzer to distinguish between EF Core DbContext and simple domain context records  
**So that** I can mock domain-specific context objects without getting false positives.

#### 1.1.1. Acceptance Criteria

**Given** a type name ends with `Context` but does not inherit from `Microsoft.EntityFrameworkCore.DbContext`  
**When** I mock this type  
**Then** no EXXER104 diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Allow Mocking of Private Context Records in Pipelines

**As a** developer  
**I want** to be able to mock private context records used in command handler pipelines  
**So that** I can test individual steps of a pipeline in isolation.

#### 1.2.1. Acceptance Criteria

**Given** a private record or class named with a `Context` suffix is defined within a command handler  
**When** I mock this type in a test  
**Then** no EXXER104 diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Allow Mocking of Nested Workflow Context Classes

**As a** developer  
**I want** to be able to mock nested context classes used in workflows  
**So that** I can unit test complex workflows.

#### 1.3.1. Acceptance Criteria

**Given** a nested class named with a `Context` suffix is used for a workflow  
**When** I mock this nested class  
**Then** no EXXER104 diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Allow Mocking of Aggregation Context Records

**As a** developer  
**I want** to be able to mock context records used in aggregation queries  
**So that** I can validate assembler logic without a real data source.

#### 1.4.1. Acceptance Criteria

**Given** a record used for data aggregation is named with a `Context` suffix  
**When** I mock this record  
**Then** no EXXER104 diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Allow Mocking of Infrastructure Abstraction Interfaces

**As a** developer  
**I want** to be able to mock interfaces that represent a unit of work or data context abstraction  
**So that** I can test repositories and services that depend on these abstractions.

#### 1.5.1. Acceptance Criteria

**Given** an interface is named with a `Context` suffix (e.g., `IDataContext`) but does not inherit from `DbContext`  
**When** I mock this interface  
**Then** no EXXER104 diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Allow Mocking of MSTest TestContext

**As a** developer  
**I want** the analyzer to allow mocking of the MSTest `TestContext` utility  
**So that** I can write tests for legacy test harnesses.

#### 1.6.1. Acceptance Criteria

**Given** I am mocking the MSTest `TestContext` class  
**When** the analyzer runs  
**Then** no EXXER104 diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Allow Mocking of HttpContext for Middleware Testing

**As a** developer  
**I want** to be able to mock `HttpContext` for testing ASP.NET Core middleware  
**So that** I can unit test my middleware components.

#### 1.7.1. Acceptance Criteria

**Given** I am mocking `Microsoft.AspNetCore.Http.HttpContext`  
**When** the analyzer runs  
**Then** no EXXER104 diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Allow Mocking of ValidationContext

**As a** developer  
**I want** to be able to mock `ValidationContext` for testing request validation logic  
**So that** I can unit test my validation behaviors.

#### 1.8.1. Acceptance Criteria

**Given** I am mocking `System.ComponentModel.DataAnnotations.ValidationContext`  
**When** the analyzer runs  
**Then** no EXXER104 diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Allow Mocking of IDbContextFactory<T>

**As a** developer  
**I want** to be able to create test doubles for `IDbContextFactory<T>`  
**So that** I can supply pre-built `DbContext` instances in my tests.

#### 1.9.1. Acceptance Criteria

**Given** I am creating a substitute or mock for `IDbContextFactory<T>`  
**When** the analyzer runs  
**Then** no EXXER104 diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Allow Mocking of Custom TestContextLogger Helpers

**As a** developer  
**I want** to be able to mock custom test helper classes named with a `Context` suffix  
**So that** I can verify telemetry and other diagnostic utilities in my tests.

#### 1.10.1. Acceptance Criteria

**Given** a class is a test helper (e.g., in a `Integration.Tests.Infrastructure` namespace) and is named with a `Context` suffix  
**When** I mock this class  
**Then** no EXXER104 diagnostic should be reported.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).