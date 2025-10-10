# Epic: EXXER002 - AvoidThrowingExceptions Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER002`  
**Source**: `src/code/IndFusion.Analyzer/ErrorHandling/AvoidThrowingExceptionsAnalyzer.cs`  
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

### 1.1. Story: Allow Null-Guard Patterns

**As a** developer  
**I want** the analyzer to allow null-guard patterns like `?? throw new ArgumentNullException(...)`  
**So that** I can use standard C# idioms for null-checking without false positives.

#### 1.1.1. Acceptance Criteria

**Given** a constructor or method uses a null-coalescing throw expression for a guard  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Allow Range Validation Exceptions

**As a** developer  
**I want** the analyzer to allow `ArgumentOutOfRangeException` for range validation  
**So that** I can enforce domain invariants on numeric values.

#### 1.2.1. Acceptance Criteria

**Given** a method throws an `ArgumentOutOfRangeException` to validate a range  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Allow Configuration Failure Exceptions

**As a** developer  
**I want** the analyzer to allow `InvalidOperationException` for configuration failures at startup  
**So that** the application can fail fast with missing or invalid configuration.

#### 1.3.1. Acceptance Criteria

**Given** a method in a startup or bootstrap class throws an `InvalidOperationException` for a configuration error  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Allow Framework-Required Exceptions

**As a** developer  
**I want** the analyzer to allow exceptions that are required by a framework, such as the Identity UI  
**So that** I can use framework features without getting false positives.

#### 1.4.1. Acceptance Criteria

**Given** a method in a framework-specific context (e.g., Identity UI scaffolding) throws a required exception like `NotSupportedException`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Allow Fatal Error Logging with Exit

**As a** developer  
**I want** to be able to re-throw an exception in a catch block before exiting the application  
**So that** I can log a fatal error before termination.

#### 1.5.1. Acceptance Criteria

**Given** a catch block re-throws an exception and is immediately followed by application termination logic (e.g., `Environment.Exit`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the re-throw.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Allow Exception Wrapping for Context

**As a** developer  
**I want** to wrap exceptions to add more context  
**So that** I can provide more meaningful error information.

#### 1.6.1. Acceptance Criteria

**Given** a catch block throws a new exception that includes the original exception as an `InnerException`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Allow ThrowHelper Methods and Assertions

**As a** developer  
**I want** the analyzer to allow helper methods that centralize exception-throwing logic  
**So that** I can reuse validation and assertion logic.

#### 1.7.1. Acceptance Criteria

**Given** a method or type is named with a helper pattern (e.g., `ThrowHelper`, `Ensure`, `Guard`) and intentionally throws exceptions  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Allow Domain Value Invariants

**As a** developer  
**I want** value objects and records to be able to throw exceptions when their invariants are violated  
**So that** I can ensure the integrity of my domain model.

#### 1.8.1. Acceptance Criteria

**Given** a value object or record factory throws an exception to enforce an invariant  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Provide an Opt-Out Attribute for Legacy Code

**As a** developer  
**I want** a way to opt-out of the `AvoidThrowingExceptions` rule for legacy code or specific modules  
**So that** I can gradually adopt the rule without being blocked.

#### 1.9.1. Acceptance Criteria

**Given** a class or method is decorated with an `[AllowExceptions]` attribute  
**When** the analyzer runs  
**Then** no diagnostic should be reported for any `throw` statements within that scope.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Allow Exceptions in Test/Benchmark Harnesses

**As a** developer  
**I want** the analyzer to allow exceptions within test and benchmark classes  
**So that** I can write tests that assert exception-throwing behavior.

#### 1.10.1. Acceptance Criteria

**Given** a method inside a test or benchmark class (e.g., a class ending with `Tests`, `Benchmarks`, or `Specs`) throws an exception  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).