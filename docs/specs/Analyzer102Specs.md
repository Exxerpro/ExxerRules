# Epic: EXXER102 - UseShouldly Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER102`  
**Source**: `src/code/IndFusion.Analyzer/Testing/DoNotUseFluentAssertionsAnalyzer.cs`  
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

### 1.1. Story: Enable Member Access Analysis

**As a** developer  
**I want** the analyzer to inspect member access expressions  
**So that** FluentAssertions usage is detected even without an explicit `using` directive.

#### 1.1.1. Acceptance Criteria

**Given** a test method uses FluentAssertions via a member access expression (e.g., `result.Should().Be(42)`)  
**When** the analyzer runs without a `using FluentAssertions;` directive in the file  
**Then** a diagnostic with ID `EXXER102` should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Distinguish Shouldly from FluentAssertions

**As a** developer  
**I want** the analyzer to differentiate between Shouldly and FluentAssertions  
**So that** I do not get false positives when correctly using Shouldly.

#### 1.2.1. Acceptance Criteria

**Given** a test method is using Shouldly assertions (e.g., `result.ShouldBe(42)`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

**Given** a test method is using FluentAssertions (e.g., `result.Should().Be(42)`)  
**When** the analyzer runs  
**Then** a diagnostic with ID `EXXER102` should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Handle Global Using Statements

**As a** developer  
**I want** the analyzer to detect `global using FluentAssertions`  
**So that** all forms of FluentAssertions usage are identified.

#### 1.3.1. Acceptance Criteria

**Given** a project contains a `global using FluentAssertions;` statement  
**And** a test method uses FluentAssertions  
**When** the analyzer runs  
**Then** a diagnostic with ID `EXXER102` should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Ignore FluentAssertions in Documentation Comments

**As a** developer  
**I want** the analyzer to ignore code examples in documentation  
**So that** I don't get false positives from comments.

#### 1.4.1. Acceptance Criteria

**Given** a file contains FluentAssertions usage examples within XML documentation comments  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the commented code.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Handle FluentAssertions in String Literals

**As a** developer  
**I want** the analyzer to ignore code examples in string literals  
**So that** I don't get false positives from string content.

#### 1.5.1. Acceptance Criteria

**Given** a file contains string literals with FluentAssertions code examples  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the string literals.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Support Configuration for Legacy Projects

**As a** developer working on a migrating project  
**I want** to be able to temporarily allow FluentAssertions  
**So that** I can gradually transition to Shouldly without being blocked.

#### 1.6.1. Acceptance Criteria

**Given** the analyzer configuration `dotnet_diagnostic.EXXER102.allow_fluent_assertions` is set to `true`  
**And** a test method uses FluentAssertions  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Detect FluentAssertions Extension Methods

**As a** developer  
**I want** the analyzer to detect various FluentAssertions extension methods  
**So that** coverage of the rule is comprehensive.

#### 1.7.1. Acceptance Criteria

**Given** a test method uses FluentAssertions extension methods like `BeEquivalentTo`, `Contain`, etc.  
**When** the analyzer runs  
**Then** a diagnostic with ID `EXXER102` should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Handle FluentAssertions in Conditional Compilation

**As a** developer  
**I want** to allow FluentAssertions in debug builds for diagnostic purposes  
**So that** I can use it for debugging without getting warnings.

#### 1.8.1. Acceptance Criteria

**Given** the analyzer configuration `dotnet_diagnostic.EXXER102.allow_in_debug` is set to `true`  
**And** FluentAssertions is used within a `#if DEBUG` block  
**When** the analyzer runs  
**Then** no diagnostic should be reported for that usage.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [- ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Improve Chain Detection Logic

**As a** developer  
**I want** the analyzer to detect complex FluentAssertions chains  
**So that** more advanced usage patterns are covered.

#### 1.9.1. Acceptance Criteria

**Given** a test method uses a complex FluentAssertions chain (e.g., `person.Name.Should().NotBeNullOrEmpty().And.Be("John")`)  
**When** the analyzer runs  
**Then** a diagnostic with ID `EXXER102` should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Provide Better Error Messages

**As a** developer  
**I want** the analyzer to provide helpful migration guidance  
**So that** I can easily refactor my code to use Shouldly.

#### 1.10.1. Acceptance Criteria

**Given** the analyzer reports a diagnostic for FluentAssertions usage  
**When** I view the diagnostic message  
**Then** the message should contain a clear suggestion to use Shouldly and provide a simple example.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).
