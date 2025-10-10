# Epic: EXXER100 - TestNamingConvention Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER100`  
**Source**: `src/code/IndFusion.Analyzer/Testing/TestNamingConventionAnalyzer.cs`  
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

### 1.1. Story: Allow Ordered Test Prefixes

**As a** developer  
**I want** the analyzer to allow alphabetical or numeric prefixes in test names  
**So that** I can control the execution order of my tests without getting naming convention warnings.

#### 1.1.1. Acceptance Criteria

**Given** a test method name is prefixed with a single letter or number followed by an underscore (e.g., `A_Should_Do_Something`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Allow Subject-First Naming

**As a** developer  
**I want** the analyzer to allow test names that start with the subject under test (e.g., `Constructor_Should_Do_Something`)  
**So that** I can clearly indicate what is being tested.

#### 1.2.1. Acceptance Criteria

**Given** a test method name follows a `Subject_Should_Action_When_Condition` pattern  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Respect DisplayName Overrides

**As a** developer  
**I want** the analyzer to suppress warnings when a test has a `DisplayName` attribute  
**So that** I can use custom test names without being forced to follow a specific convention.

#### 1.3.1. Acceptance Criteria

**Given** a test method is decorated with a `[Fact(DisplayName = "...")]` or `[Theory(DisplayName = "...")]` attribute  
**When** the analyzer runs  
**Then** no diagnostic should be reported for that method's name.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Allow Nested Context Classes

**As a** developer  
**I want** the analyzer to allow different naming conventions within nested context classes (e.g., `Given_...`, `When_...`)  
**So that** I can structure my tests using BDD-style nested classes.

#### 1.4.1. Acceptance Criteria

**Given** a test method is inside a nested class whose name starts with `Given_` or `When_`  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the test method's name.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Allow Domain-Descriptive Names Without "Should"

**As a** developer  
**I want** the analyzer to be more flexible with test names that are descriptive but don't start with "Should"  
**So that** I can write test names that align with my domain language.

#### 1.5.1. Acceptance Criteria

**Given** a test name is descriptive and contains verbs other than "Should" at the beginning (e.g., `Process_Returns_Response_When_Valid`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Handle Async Suffix Placement

**As a** developer  
**I want** the analyzer to correctly handle `Async` suffixes in test names  
**So that** my asynchronous tests are not flagged incorrectly.

#### 1.6.1. Acceptance Criteria

**Given** a test method name includes an `Async` suffix (e.g., `ProcessAsync_Should_Return_Result`)  
**When** the analyzer runs  
**Then** the `Async` suffix should be ignored during validation and no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Allow Subject Prefixes Using PascalCase

**As a** developer  
**I want** the analyzer to allow PascalCase subjects in test names  
**So that** I can follow my team's naming conventions.

#### 1.7.1. Acceptance Criteria

**Given** a test method name starts with a PascalCase subject (e.g., `Machine_Should_Start_InIdle`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Allow Tests Describing Negative Scenarios Without a Condition

**As a** developer  
**I want** the analyzer to allow test names for negative scenarios that don't have a `_When_` clause  
**So that** I can write clear and concise names for architectural or behavior-only tests.

#### 1.8.1. Acceptance Criteria

**Given** a test method name describes a negative scenario or behavior without a `_When_` clause (e.g., `Services_Should_Not_Depend_On_UI`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Allow Lowercase Condition Tokens

**As a** developer  
**I want** the analyzer to allow lowercase tokens in the condition part of a test name  
**So that** I have more flexibility in naming my tests.

#### 1.9.1. Acceptance Criteria

**Given** a test method name contains lowercase tokens in the condition part (e.g., `_When_cancelled`)  
**When** the analyzer runs  
**Then** the casing should be ignored during validation and no diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Recognize Opt-Out Attribute in Nested Types

**As a** developer  
**I want** the analyzer to respect the `AllowTestNamingVariations` attribute on ancestor classes  
**So that** I can opt-out an entire group of nested tests from the naming convention rule.

#### 1.10.1. Acceptance Criteria

**Given** an ancestor class of a test method is decorated with the `[AllowTestNamingVariations]` attribute  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the test method's name.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).