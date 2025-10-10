# Epic: EXXER503 - DoNotUseRegions Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER503`  
**Source**: `src/code/IndFusion.Analyzer/CodeQuality/DoNotUseRegionsAnalyzer.cs`  
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

### 1.1. Story: Allow Regions for Constant Observability Buckets

**As a** developer  
**I want** to use regions to group constant `EventId` fields for logging and observability  
**So that** I can organize my logging constants without getting warnings.

#### 1.1.1. Acceptance Criteria

**Given** a `#region` in a static class contains only `const` or `static readonly` field declarations  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Allow Regions for Activity Source Constants

**As a** developer  
**I want** to use regions to group `ActivitySource` constants for OpenTelemetry  
**So that** I can keep my tracing-related constants organized.

#### 1.2.1. Acceptance Criteria

**Given** a `#region` in a static class contains only `const string` fields and private helper methods for OpenTelemetry activities  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Allow Regions for Pipeline Steps in Command Handlers

**As a** developer implementing the Railway-oriented programming pattern  
**I want** to use regions to group pipeline step methods in my command handlers  
**So that** I can clearly visualize the sequence of operations.

#### 1.3.1. Acceptance Criteria

**Given** a `#region` name contains "Pipeline" and all enclosed members are private methods ending with "Step"  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Allow Regions for Success/Failure Handlers

**As a** developer  
**I want** to use regions to group success and failure handling logic in my command handlers  
**So that** I can separate the happy path from error handling.

#### 1.4.1. Acceptance Criteria

**Given** a `#region` name contains "Success" or "Failure" and the enclosed members are private logging or handling methods  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Allow Regions for Helper Methods in Gateway Pipelines

**As a** developer  
**I want** to use regions to group helper methods in my gateway pipelines  
**So that** my pipeline code remains clean and organized.

#### 1.5.1. Acceptance Criteria

**Given** a `#region` name contains "Helper" and all enclosed members are private methods returning `Task` or `Result`-like types  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Allow Regions for Nested Context Classes

**As a** developer  
**I want** to use regions to encapsulate nested context classes within a handler  
**So that** I can keep related data structures co-located with the logic that uses them.

#### 1.6.1. Acceptance Criteria

**Given** a `#region` contains exactly one nested type declaration (class, record, struct, or enum)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Allow Regions for Private Helpers in Static Utilities

**As a** developer  
**I want** to use regions to group private helper methods in static utility classes  
**So that** I can organize complex utility classes.

#### 1.7.1. Acceptance Criteria

**Given** a `#region` in a static class contains only `private static` members  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Allow Regions for Scenario Grouping in Tests

**As a** developer writing tests  
**I want** to use regions to group related test scenarios within a test class  
**So that** my test files are easier to navigate.

#### 1.8.1. Acceptance Criteria

**Given** a `#region` is in a test file and all enclosed members have xUnit attributes (`[Fact]`, `[Theory]`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Allow Regions for Fixture Definitions in Test Suites

**As a** developer writing tests  
**I want** to use regions to group nested fixture classes within a test suite  
**So that** I can keep my test setup code organized.

#### 1.9.1. Acceptance Criteria

**Given** a `#region` in a test file encloses only nested type declarations for test fixtures  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).