# Epic: EXXER003 - DoNotThrowExceptions Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER003`  
**Source**: `src/code/IndFusion.Analyzer/FunctionalPatterns/DoNotThrowExceptionsAnalyzer.cs`  
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

### 1.1. Story: Allow Guard Clauses Throwing ArgumentNullException

**As a** developer  
**I want** the analyzer to allow `ArgumentNullException` in guard clauses  
**So that** I can perform standard null-checking without getting false positives.

#### 1.1.1. Acceptance Criteria

**Given** a method contains a guard clause that throws an `ArgumentNullException`  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the guard clause.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Allow Null-Coalescing Throws in Guard Clauses

**As a** developer  
**I want** the analyzer to allow null-coalescing throw expressions in guard clauses  
**So that** I can use modern C# features for null-checking.

#### 1.2.1. Acceptance Criteria

**Given** a method uses a null-coalescing throw expression (e.g., `_ = value ?? throw new ArgumentNullException(...)`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Allow Range Checks Throwing ArgumentOutOfRangeException

**As a** developer  
**I want** the analyzer to allow `ArgumentOutOfRangeException` in range-check guard clauses  
**So that** I can validate input ranges effectively.

#### 1.3.1. Acceptance Criteria

**Given** a method contains a guard clause that throws an `ArgumentOutOfRangeException` for range validation  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Allow Throws in Switch Expression Default Arms

**As a** developer  
**I want** the analyzer to allow throw expressions in the default arm of a switch expression  
**So that** I can enforce exhaustiveness in switch expressions.

#### 1.4.1. Acceptance Criteria

**Given** a switch expression's default arm (`_ => ...`) contains a throw expression  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Allow Domain-Specific Parsing Exceptions

**As a** developer  
**I want** the analyzer to allow custom domain-specific exceptions for validation  
**So that** I can provide meaningful, domain-specific error information.

#### 1.5.1. Acceptance Criteria

**Given** a method in a parser or validator class throws a custom domain-specific exception (e.g., `InvalidS7AddressException`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Allow Constructors to Enforce Invariants

**As a** developer  
**I want** constructors to be able to throw exceptions to enforce invariants  
**So that** I can prevent the creation of invalid objects.

#### 1.6.1. Acceptance Criteria

**Given** a constructor throws an exception to validate its parameters or enforce an invariant  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Allow Factory Methods to Perform Validation

**As a** developer  
**I want** factory methods to be able to throw exceptions for validation  
**So that** I can ensure factories only create valid objects, even during migration to a functional style.

#### 1.7.1. Acceptance Criteria

**Given** a factory method (e.g., `Create`, `Build`) throws a guard exception (`ArgumentException`, etc.)  
**When** the analyzer runs  
**Then** the diagnostic should be downgraded or suppressed.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Allow Throw Expressions in Expression-Bodied Members

**As a** developer  
**I want** to use throw expressions in expression-bodied members for guards  
**So that** I can write concise validation logic.

#### 1.8.1. Acceptance Criteria

**Given** an expression-bodied member uses a throw expression for validation  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Allow Public API Defensive Checks

**As a** developer  
**I want** to use `NotSupportedException` or `NotImplementedException` for defensive checks in public APIs  
**So that** I can clearly communicate unsupported features.

#### 1.9.1. Acceptance Criteria

**Given** a public API method throws a `NotSupportedException` or `NotImplementedException`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Allow Re-throwing Wrapped Exceptions in Catch Blocks

**As a** developer  
**I want** to wrap and re-throw exceptions in catch blocks to add context  
**So that** I can provide more meaningful error information.

#### 1.10.1. Acceptance Criteria

**Given** a catch block re-throws an exception that includes the original exception as an inner exception  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).