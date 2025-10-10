# Epic: EXXER702 - UseModernPatternMatching Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER702`  
**Source**: `src/code/IndFusion.Analyzer/ModernCSharp/UseModernPatternMatchingAnalyzer.cs`  
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

### 1.1. Story: Exempt Conditional Operator Guards

**As a** developer  
**I want** the analyzer to ignore `is` checks followed by a cast within a ternary/conditional expression  
**So that** I can use concise expression-bodied members for reflection and other utilities.

#### 1.1.1. Acceptance Criteria

**Given** a ternary expression uses an `is` or `is not` check in its condition and a cast in one of its branches  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Exempt Reflection Property Access

**As a** developer using reflection  
**I want** the analyzer to allow guard-and-cast patterns in reflection helpers  
**So that** my reflection utilities are not flagged with pattern matching warnings.

#### 1.2.1. Acceptance Criteria

**Given** a reflection helper method uses a ternary expression to check a value and cast it  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Exempt Type-Switched Casts

**As a** developer  
**I want** the analyzer to ignore casts within a `Type switch` expression  
**So that** I can perform type-safe conversions based on a `Type` object.

#### 1.3.1. Acceptance Criteria

**Given** a `switch` expression on a `Type` object contains a cast in one of its arms  
**When** the analyzer runs  
**Then** no diagnostic should be reported for that cast.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Exempt Local Function Closures

**As a** developer  
**I want** the analyzer to be smarter about casts within local functions and lambdas  
**So that** I don't get warnings for casts that are safely executed within a guarded scope.

#### 1.4.1. Acceptance Criteria

**Given** a cast occurs inside a local function or lambda that is defined and executed within the scope of an `if (obj is Type)` guard  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Support `is not null` Guard Clauses

**As a** developer  
**I want** the analyzer to recognize `is not null` as a valid guard before a cast  
**So that** I can use modern C# null-checking patterns.

#### 1.5.1. Acceptance Criteria

**Given** an `if` statement uses an `is not` pattern and the corresponding block contains a cast of the same variable  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Exempt Nullable Unwrap Patterns

**As a** developer  
**I want** the analyzer to allow unboxing casts that are guarded by a type check  
**So that** I can safely convert from `object?` to a value type.

#### 1.6.1. Acceptance Criteria

**Given** an `if (obj is ValueType)` check is followed by a cast to that `ValueType`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Exempt Type Equality Guards

**As a** developer  
**I want** the analyzer to recognize `if (type == typeof(T))` as a valid guard before a cast  
**So that** I can perform dynamic type checks and casts.

#### 1.7.1. Acceptance Criteria

**Given** a cast is guarded by an `if` statement that checks for type equality using `typeof`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Exempt Tuple Pattern Extraction

**As a** developer  
**I want** the analyzer to allow casts that are part of a tuple assignment or deconstruction within a guarded block  
**So that** I can work with tuples without false positives.

#### 1.8.1. Acceptance Criteria

**Given** a cast is performed as part of a tuple assignment or deconstruction inside a type-guarded block  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Exempt Pattern-Matched Exception Handling

**As a** developer  
**I want** the analyzer to ignore casts inside `catch` blocks that use pattern matching  
**So that** I can perform specific actions based on the exception type.

#### 1.9.1. Acceptance Criteria

**Given** a `catch` block uses a `when` clause with a type check, and the block itself contains a cast of the caught exception  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Exempt Temporary Variable Reassignment

**As a** developer  
**I want** the analyzer to be smarter about variable reassignment after a cast  
**So that** I don't get warnings for legitimate cloning or transformation patterns.

#### 1.10.1. Acceptance Criteria

**Given** a variable is cast and then reassigned to itself within a guarded block  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).