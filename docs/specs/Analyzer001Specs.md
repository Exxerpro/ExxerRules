# Epic: EXXER001 - UseResultPattern Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER001`  
**Source**: `src/code/IndFusion.Analyzer/ErrorHandling/UseResultPatternAnalyzer.cs`  
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

### 1.1. Story: Allow Domain Guard Methods Returning `bool`

**As a** developer  
**I want** the analyzer to allow domain guard methods that return `bool` and throw exceptions  
**So that** I can perform desirable domain validation without false positives.

#### 1.1.1. Acceptance Criteria

**Given** a domain guard method (e.g., `AppliesTo`, `Validate`) returns `bool` and throws a validation exception (e.g., `ArgumentOutOfRangeException`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Allow Startup/Configuration Guards

**As a** developer  
**I want** the analyzer to allow guards in startup and configuration code  
**So that** I can validate essential configuration at application startup.

#### 1.2.1. Acceptance Criteria

**Given** a method inside a `Program` or `Startup` class throws an `InvalidOperationException` for missing configuration  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Allow Identity UI Scaffolding Exceptions

**As a** developer  
**I want** the analyzer to allow exceptions required by the Identity UI framework  
**So that** I can use the default Identity scaffolding without modification.

#### 1.3.1. Acceptance Criteria

**Given** a method in an Identity UI component (e.g., in a `.Components.Account` namespace) throws a `NotSupportedException` as required by the framework  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Allow Guard Helper/ThrowHelper Methods

**As a** developer  
**I want** the analyzer to allow centralized guard helper methods that intentionally throw exceptions  
**So that** I can reuse validation logic across the application.

#### 1.4.1. Acceptance Criteria

**Given** a method or type is named with a guard-like pattern (e.g., `Guard`, `ThrowHelper`, `Ensure`) and intentionally throws exceptions  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Allow Value Object Factories to Throw Exceptions

**As a** developer  
**I want** value object factory methods to be able to throw exceptions when invariants fail  
**So that** I can enforce the correctness of value objects at creation.

#### 1.5.1. Acceptance Criteria

**Given** a static factory method in a value object-like type (e.g., `Percentage`, `Amount`) throws a guard exception (`ArgumentNullException`, `ArgumentOutOfRangeException`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Allow `?? throw` Guards in Properties

**As a** developer  
**I want** to use null-coalescing throw expressions in expression-bodied properties for lazy initialization  
**So that** I can write concise and clear property guards.

#### 1.6.1. Acceptance Criteria

**Given** an expression-bodied property uses a null-coalescing throw expression (`get => _service ?? throw ...`) for a guard  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Allow Test Helper Methods Inside Test Classes

**As a** developer  
**I want** the analyzer to allow helper methods within test classes to throw exceptions  
**So that** I can create intentional failure scenarios for my tests.

#### 1.7.1. Acceptance Criteria

**Given** a private helper method inside a test class (e.g., a class ending with `Tests`) throws an exception  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Allow Async Handlers Returning `Task` to Throw

**As a** developer  
**I want** asynchronous event handlers or background jobs that return a non-generic `Task` to be able to throw exceptions  
**So that** I can surface fatal errors in background processes.

#### 1.8.1. Acceptance Criteria

**Given** an async method that returns `Task` or `ValueTask` (and is identified as a background worker or event handler) throws an exception  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Allow Local Functions Performing Validation

**As a** developer  
**I want** to use local functions to encapsulate validation logic that throws exceptions  
**So that** I can structure my validation code cleanly within a method.

#### 1.9.1. Acceptance Criteria

**Given** a local function is used for immediate validation and throws an exception  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Provide an Opt-Out Attribute

**As a** developer  
**I want** a way to explicitly opt-out of the `UseResultPattern` rule for specific modules or methods  
**So that** I can handle legacy code or specific scenarios where exceptions are required.

#### 1.10.1. Acceptance Criteria

**Given** a class or method is decorated with an `[AllowExceptions]` attribute  
**When** the analyzer runs  
**Then** no diagnostic should be reported for any `throw` statements within that scope.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).