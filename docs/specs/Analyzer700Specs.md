# Epic: EXXER700 - UseEfficientLinq Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER700`  
**Source**: `src/code/IndFusion.Analyzer/Performance/UseEfficientLinqAnalyzer.cs`  
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

### 1.1. Story: Exempt Guard Patterns on `ICollection` and Arrays

**As a** developer  
**I want** the analyzer to ignore LINQ guard patterns on `ICollection<T>` and arrays  
**So that** I can perform efficient checks like `!collection.Any()` without getting false positives.

#### 1.1.1. Acceptance Criteria

**Given** a LINQ method like `Any()` or `Count()` is called on an `ICollection<T>`, `IReadOnlyCollection<T>`, or an array  
**When** the analyzer runs  
**Then** it should not be flagged as a multiple enumeration.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Recognize Materialized Queries

**As a** developer  
**I want** the analyzer to recognize when a query has been materialized with `ToList()` or `ToArray()`  
**So that** I can perform multiple operations on the materialized collection without warnings.

#### 1.2.1. Acceptance Criteria

**Given** a LINQ query is materialized using `ToList()`, `ToArray()`, or `ToHashSet()`  
**When** multiple LINQ methods are called on the resulting collection  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Exempt `IQueryable`

**As a** developer using EF Core  
**I want** the analyzer to ignore chained LINQ calls on `IQueryable<T>`  
**So that** I can build up database queries without the analyzer misinterpreting them as multiple enumerations.

#### 1.3.1. Acceptance Criteria

**Given** multiple LINQ operators are chained on an `IQueryable<T>` object  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Exempt Set Operations

**As a** developer  
**I want** the analyzer to recognize that set operations like `Union` and `Distinct` produce new sequences  
**So that** I am not warned about multiple enumerations when combining or filtering sequences.

#### 1.4.1. Acceptance Criteria

**Given** LINQ set operators like `Union`, `Except`, `Concat`, or `Distinct` are used  
**When** the analyzer runs  
**Then** these should not be counted as a re-enumeration of the original source.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Exempt `Any()` Guard Followed by `First()` on Lists

**As a** developer  
**I want** the analyzer to allow the common pattern of checking `Any()` before calling `First()` on a list or array  
**So that** I can safely access the first element of a collection.

#### 1.5.1. Acceptance Criteria

**Given** `Any()` is called on a list or array, followed by a call to `First()` on the same collection  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Exempt Async LINQ

**As a** developer using EF Core  
**I want** the analyzer to ignore async LINQ methods like `AnyAsync` and `FirstAsync`  
**So that** I can perform asynchronous database queries without false positives.

#### 1.6.1. Acceptance Criteria

**Given** async LINQ methods from EF Core (e.g., `AnyAsync`, `FirstAsync`) are used on an `IQueryable<T>`  
**When** the analyzer runs  
**Then** no diagnostic should be reported as multiple enumerations.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Exempt Null-Coalesced Enumerables

**As a** developer  
**I want** the analyzer to recognize when a null enumerable is coalesced to an empty one  
**So that** I can safely handle potentially null sequences.

#### 1.7.1. Acceptance Criteria

**Given** a LINQ query is performed on a sequence that has been null-coalesced with `Enumerable.Empty<T>()`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Exempt Expression-Bodied Properties

**As a** developer  
**I want** the analyzer to correctly handle expression-bodied properties that use LINQ methods  
**So that** I can define simple, calculated properties without warnings.

#### 1.8.1. Acceptance Criteria

**Given** an expression-bodied property returns the result of a LINQ method (e.g., `public bool HasItems => this.items.Any();`)  
**When** the analyzer runs  
**Then** this should be treated as a single evaluation and not contribute to a multiple enumeration warning.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Differentiate Query Variables Semantically

**As a** developer  
**I want** the analyzer to differentiate between different query variables, even if they are textually similar  
**So that** I don't get false positives from unrelated queries.

#### 1.9.1. Acceptance Criteria

**Given** two different variables are used in separate LINQ queries  
**When** the analyzer runs  
**Then** it should use semantic analysis to distinguish them and not report a multiple enumeration warning.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Provide an Opt-Out Attribute

**As a** developer  
**I want** a way to opt-out of the multiple enumeration check for specific methods  
**So that** I can handle known safe cases or performance-critical code where multiple enumerations are intentional.

#### 1.10.1. Acceptance Criteria

**Given** a method is decorated with an `[AllowMultipleEnumeration]` attribute  
**When** the analyzer runs  
**Then** no diagnostic should be reported for that method.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).