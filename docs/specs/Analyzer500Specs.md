# Epic: EXXER500 - AvoidMagicNumbersAndStrings Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER500`  
**Source**: `src/code/IndFusion.Analyzer/CodeQuality/AvoidMagicNumbersAndStringsAnalyzer.cs`  
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

### 1.1. Story: Exempt Enum Member Values

**As a** developer  
**I want** the analyzer to ignore literal values assigned to enum members  
**So that** I can define enums with explicit underlying values without getting warnings.

#### 1.1.1. Acceptance Criteria

**Given** a numeric literal is used to define the value of an enum member  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Exempt Bit-Flag Enum Values

**As a** developer  
**I want** the analyzer to ignore bit-shift operations used to define flag enum members  
**So that** I can create `[Flags]` enums using standard bitwise patterns.

#### 1.2.1. Acceptance Criteria

**Given** a `[Flags]` enum uses bit-shift expressions (e.g., `1 << 0`) to define its members  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Exempt Domain Range Guards

**As a** developer  
**I want** the analyzer to allow numeric literals in domain range guard clauses  
**So that** I can implement clear and explicit validation for domain rules.

#### 1.3.1. Acceptance Criteria

**Given** a numeric literal is used in a comparison within a guard clause that throws an `ArgumentOutOfRangeException`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Exempt Business Rule Thresholds

**As a** developer  
**I want** the analyzer to allow numeric literals for business rule thresholds, like string length validation  
**So that** I can define validation logic without unnecessary constants.

#### 1.4.1. Acceptance Criteria

**Given** a numeric literal is used in a comparison against a property like `Length` or `Count` inside a validation method  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Exempt Exception Messages

**As a** developer  
**I want** the analyzer to allow string literals used as exception messages  
**So that** I can provide clear and descriptive error information.

#### 1.5.1. Acceptance Criteria

**Given** a string literal is passed as the message argument to an `Exception` constructor  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Exempt Result/Validation Messages

**As a** developer  
**I want** the analyzer to allow string literals for validation messages in collections or `Result` objects  
**So that** I can return descriptive error messages from validation logic.

#### 1.6.1. Acceptance Criteria

**Given** a string literal is added to a collection named `errors` or `warnings`, or passed to a `Result.WithFailure` method  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Exempt Regex and Pattern Literals

**As a** developer  
**I want** the analyzer to ignore string literals that are regular expressions or other pattern-based strings  
**So that** I can use patterns without getting warnings.

#### 1.7.1. Acceptance Criteria

**Given** a string literal contains regex metacharacters or is passed to a `Regex` constructor or method  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Exempt Culture and Locale Codes

**As a** developer  
**I want** the analyzer to allow string literals for culture and locale codes  
**So that** I can specify cultures without creating unnecessary constants.

#### 1.8.1. Acceptance Criteria

**Given** a string literal matches a culture code pattern (e.g., "en-US") and is used with `CultureInfo` or `RegionInfo`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Exempt TimeSpan and DateTime Construction

**As a** developer  
**I want** the analyzer to allow numeric literals when constructing `TimeSpan` or `DateTime` objects  
**So that** I can clearly express durations and specific dates.

#### 1.9.1. Acceptance Criteria

**Given** a numeric literal is used as an argument to a `TimeSpan` or `DateTime` factory method or constructor  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Exempt Logging Message Templates

**As a** developer  
**I want** the analyzer to allow string literals for structured logging message templates  
**So that** I can use `ILogger` effectively.

#### 1.10.1. Acceptance Criteria

**Given** a string literal is the first argument to an `ILogger.Log` method  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).