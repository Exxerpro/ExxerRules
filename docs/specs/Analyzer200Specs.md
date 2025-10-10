# Epic: EXXER200 - ValidateNullParameters Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER200`  
**Source**: `src/code/IndFusion.Analyzer/NullSafety/ValidateNullParametersAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)  
**Last Updated**: 2025-10-10

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

### 1.1. Story: Semantic Type Detection Foundation

**As a** developer using the analyzer  
**I want** value-type parameters to be correctly identified  
**So that** I don't receive false-positive warnings for non-nullable types

#### 1.1.1. Acceptance Criteria

**Given** a method with value-type parameters (`int`, `bool`, `DateTime`, `Guid`, `decimal`, etc.)  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** a method with nullable value-type parameters (`int?`, `bool?`, `DateTime?`)  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** a method with reference-type parameters without validation  
**When** the analyzer processes the method  
**Then** EXXER200 diagnostic should be reported (positive control)

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Infrastructure Type Exemptions

**As a** developer using dependency injection  
**I want** infrastructure parameters to be exempt from validation  
**So that** I don't receive false warnings for DI-managed dependencies

#### 1.2.1. Acceptance Criteria

**Given** a method with a `CancellationToken` parameter  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** a constructor with `IServiceProvider` and `ILogger<T>` parameters  
**When** the analyzer processes the constructor  
**Then** no EXXER200 diagnostic should be reported

**Given** a method with `IServiceProvider` parameter in a non-DI context  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported (conservative approach)

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Expression-Bodied Member Guard Detection

**As a** developer using modern C# features  
**I want** expression-bodied members with guards to be recognized  
**So that** I don't receive false warnings for concise guard implementations

#### 1.3.1. Acceptance Criteria

**Given** an expression-bodied method calling `Guard.Against.Null`  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** an expression-bodied method using null-coalescing operator  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** an expression-bodied method using null-conditional operator  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Extension Method Guard Pattern Support

**As a** developer using extension method guards  
**I want** `parameter.ThrowIfNull()` patterns to be recognized  
**So that** I don't receive warnings for idiomatic null checks

#### 1.4.1. Acceptance Criteria

**Given** a method calling `ArgumentNullException.ThrowIfNull(param)`  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** a method calling `param.ThrowIfNull()` extension method  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** a method calling custom guard extension methods  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Optional Parameter Support

**As a** developer using optional parameters  
**I want** parameters with default values to be exempt  
**So that** I don't receive warnings for intentionally nullable parameters

#### 1.5.1. Acceptance Criteria

**Given** a method with optional string parameter (`string? name = null`)  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** a method with multiple optional reference parameters  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** a method with required reference parameter (no default)  
**When** the analyzer processes the method without validation  
**Then** EXXER200 diagnostic should be reported

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Params Array Exemption

**As a** developer using params arrays  
**I want** params array parameters to be exempt  
**So that** I don't receive warnings for variadic methods

#### 1.6.1. Acceptance Criteria

**Given** a method with a params array parameter  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** a method with multiple params variations  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Record Primary Constructor Support

**As a** developer using modern record types  
**I want** record primary constructors with inline validation to be recognized  
**So that** I don't receive warnings for validated record parameters

#### 1.7.1. Acceptance Criteria

**Given** a record with primary constructor and inline validation  
**When** the analyzer processes the record  
**Then** no EXXER200 diagnostic should be reported

**Given** a record with secondary constructor containing guards  
**When** the analyzer processes the record  
**Then** no EXXER200 diagnostic should be reported

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Local Function Guard Detection

**As a** developer organizing validation logic  
**I want** local functions containing guards to be recognized  
**So that** I don't receive warnings when I delegate validation to helper functions

#### 1.8.1. Acceptance Criteria

**Given** a method with a local function performing validation  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** a method calling a local guard function  
**When** the analyzer processes the method  
**Then** the parameters validated by the local function should be recognized

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Result<T> Failure Pattern Support

**As a** developer using functional error handling  
**I want** methods returning `Result.Failure` for null parameters to be recognized  
**So that** I don't receive warnings for functional validation patterns

#### 1.9.1. Acceptance Criteria

**Given** a method returning `Result.Failure` for null parameter  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

**Given** a method using `Result.WithFailure` pattern  
**When** the analyzer processes the method  
**Then** no EXXER200 diagnostic should be reported

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Comprehensive Integration Testing

**As a** project maintainer  
**I want** comprehensive integration tests validating all scenarios  
**So that** I can ensure the analyzer works correctly in real-world codebases

#### 1.10.1. Acceptance Criteria

**Given** the complete analyzer implementation  
**When** all tests are run  
**Then** all 577 tests should pass with 0 failures

**Given** a representative production codebase  
**When** the analyzer is applied  
**Then** false-positive rate should be < 5%

**Given** the analyzer test suite  
**When** tests are run with code coverage  
**Then** coverage should be >= 90%

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).