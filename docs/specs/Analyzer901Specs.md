# Epic: EXXER901 - CodeFormatting Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER901`  
**Source**: `src/code/IndFusion.Analyzer/CodeFormatting/CodeFormattingAnalyzer.cs`  
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

### 1.1. Story: Correctly Handle LINQ Projections

**As a** developer  
**I want** the code formatting analyzer to correctly handle LINQ projection assignments  
**So that** I don't get false "inconsistent variable formatting" warnings.

#### 1.1.1. Acceptance Criteria

**Given** a variable is assigned the result of a LINQ projection (e.g., `.Select(...)`) with correct spacing  
**When** the `CodeFormattingAnalyzer` runs  
**Then** no EXXER901 diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Correctly Handle Guard Clause Mock Data Assignments

**As a** developer  
**I want** the formatting analyzer to correctly handle assignments within debug guard clauses  
**So that** my diagnostic code is not flagged.

#### 1.2.1. Acceptance Criteria

**Given** a variable is assigned within a debug guard clause (e.g., `#if DEBUG`) with correct spacing  
**When** the `CodeFormattingAnalyzer` runs  
**Then** no EXXER901 diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Correctly Handle Dictionary Initializations

**As a** developer  
**I want** the formatting analyzer to correctly handle dictionary initializations, especially in result pipelines  
**So that** I don't get false positives on complex data structures.

#### 1.3.1. Acceptance Criteria

**Given** a dictionary is initialized with a complex type with correct spacing  
**When** the `CodeFormattingAnalyzer` runs  
**Then** no EXXER901 diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Correctly Handle Awaited Repository Calls

**As a** developer  
**I want** the formatting analyzer to correctly handle assignments from awaited repository calls  
**So that** my asynchronous data access code is not flagged.

#### 1.4.1. Acceptance Criteria

**Given** a variable is assigned the result of an awaited repository call with correct spacing  
**When** the `CodeFormattingAnalyzer` runs  
**Then** no EXXER901 diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Correctly Handle Projections to DTOs

**As a** developer  
**I want** the formatting analyzer to correctly handle LINQ projections to DTOs  
**So that** my mapping code is not flagged.

#### 1.5.1. Acceptance Criteria

**Given** a variable is assigned the result of a LINQ `.Select()` call that projects to a DTO, with correct spacing  
**When** the `CodeFormattingAnalyzer` runs  
**Then** no EXXER901 diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Correctly Handle GroupBy/ToDictionary Pipelines

**As a** developer  
**I want** the formatting analyzer to correctly handle complex LINQ pipelines involving `GroupBy` and `ToDictionary`  
**So that** my data aggregation code is not flagged.

#### 1.6.1. Acceptance Criteria

**Given** a variable is assigned the result of a LINQ pipeline using `GroupBy` and `ToDictionary` with correct spacing  
**When** the `CodeFormattingAnalyzer` runs  
**Then** no EXXER901 diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Correctly Handle Fluent Result Pipelines

**As a** developer using a fluent `Result` pattern  
**I want** the formatting analyzer to correctly handle assignments from awaited `Result` pipelines  
**So that** my functional-style code is not flagged.

#### 1.7.1. Acceptance Criteria

**Given** a variable is assigned the result of an awaited fluent `Result` pipeline with correct spacing  
**When** the `CodeFormattingAnalyzer` runs  
**Then** no EXXER901 diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Correctly Handle Specification Builder Assignments

**As a** developer using the specification pattern  
**I want** the formatting analyzer to correctly handle the instantiation of specification objects  
**So that** my specification builder code is not flagged.

#### 1.8.1. Acceptance Criteria

**Given** a variable is assigned a `new Specification<T>(...)` with correct spacing  
**When** the `CodeFormattingAnalyzer` runs  
**Then** no EXXER901 diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Correctly Handle Dictionary Materialization from Collections

**As a** developer  
**I want** the formatting analyzer to correctly handle the materialization of a dictionary from a collection using `ToDictionary`  
**So that** my data transformation code is not flagged.

#### 1.9.1. Acceptance Criteria

**Given** a variable is assigned the result of a `ToDictionary` call on a collection with correct spacing  
**When** the `CodeFormattingAnalyzer` runs  
**Then** no EXXER901 diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).