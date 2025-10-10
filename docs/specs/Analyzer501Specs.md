# Epic: EXXER501 - UseExpressionBodiedMembers Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER501`  
**Source**: `src/code/IndFusion.Analyzer/ModernCSharp/UseExpressionBodiedMembersAnalyzer.cs`  
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

### 1.1. Story: Exempt ICommandData Factory Methods

**As a** developer  
**I want** the analyzer to ignore `Create` factory methods in command types  
**So that** I can maintain a consistent block-bodied style for command factories for readability and future extension.

#### 1.1.1. Acceptance Criteria

**Given** a method implements `ICommandData.Create` and returns a new instance of a command or query  
**When** the analyzer runs  
**Then** no diagnostic should be reported, even if the method body is a single `return` statement.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Exempt Fluent TODO Stubs

**As a** developer  
**I want** the analyzer to ignore methods that contain `// TODO` or `// FIXME` comments  
**So that** I can leave placeholders for future implementation without getting formatting warnings.

#### 1.2.1. Acceptance Criteria

**Given** a method with a single `return` statement also contains a `TODO` or `FIXME` comment in its body  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Exempt `IResettable.TryReset` Methods

**As a** developer  
**I want** the analyzer to ignore `TryReset` methods that return a constant boolean value  
**So that** I can implement the `IResettable` interface with a simple block body for future extension.

#### 1.3.1. Acceptance Criteria

**Given** a method named `TryReset` implements `IResettable.TryReset` and returns `true` or `false`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Exempt Fluent `WithData` Methods

**As a** developer  
**I want** the analyzer to ignore fluent `With...` methods that return `this`  
**So that** I can maintain a readable block-body style for fluent builder methods.

#### 1.4.1. Acceptance Criteria

**Given** a method name starts with `With` or `Set` and its body consists of a single `return this;` statement  
**When** the analyzer runs  
**Then** no diagnostic should be reported if the body also contains comments indicating future extension.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Exempt Domain Entity Resetters

**As a** developer  
**I want** the analyzer to ignore reset methods in domain entities that return a boolean literal  
**So that** I can write simple reset methods that are expected to grow in complexity.

#### 1.5.1. Acceptance Criteria

**Given** a method has XML documentation containing the word "reset" or has `Reset` in its name, and it returns a boolean literal  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).