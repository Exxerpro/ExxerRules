# Epic: EXXER601 - UseRepositoryPattern Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER601`  
**Source**: `src/code/IndFusion.Analyzer/Architecture/UseRepositoryPatternAnalyzer.cs`  
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

### 1.1. Story: Exempt Application Layer Handlers

**As a** developer  
**I want** the analyzer to allow application-layer handlers to use `DbContext` directly for performance-critical queries  
**So that** I can optimize data access without being forced to create a repository for every query.

#### 1.1.1. Acceptance Criteria

**Given** a class is in an `*.Application.*` namespace and implements a handler interface (e.g., `IRequestHandler`)  
**When** it takes a `DbContext` as a dependency  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Exempt Infrastructure Layer

**As a** developer  
**I want** the analyzer to ignore classes in the infrastructure layer  
**So that** I can write data access and other infrastructure-specific code without warnings.

#### 1.2.1. Acceptance Criteria

**Given** a class is in a namespace or project that contains `.Infrastructure`  
**When** it uses `DbContext` or other data access types  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Exempt Test and Fixture Classes

**As a** developer writing tests  
**I want** the analyzer to ignore test and fixture classes that use `DbContext`  
**So that** I can set up and seed data for my integration tests.

#### 1.3.1. Acceptance Criteria

**Given** a class is a test class or a test fixture (e.g., name ends with `Tests` or `Fixture`)  
**When** it uses `DbContext`  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Exempt Connection Wrapper Classes

**As a** developer  
**I want** the analyzer to allow connection wrapper classes (e.g., for Dapper or hardware connections) to hold connection objects  
**So that** I can create factories and managers for my connections.

#### 1.4.1. Acceptance Criteria

**Given** a class name suggests it is a connection wrapper (e.g., ends with `Connection`, `Connector`, `Factory`)  
**When** it holds a connection object (e.g., `IDbConnection`)  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Exempt `DbContextOptions` and EF Services

**As a** developer  
**I want** the analyzer to ignore `DbContextOptions` and other EF Core services injected into constructors  
**So that** I can configure and use EF Core services without warnings.

#### 1.5.1. Acceptance Criteria

**Given** a constructor injects `DbContextOptions<T>`, `IDbContextFactory<T>`, or `IServiceScopeFactory`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Exempt Minimal APIs and Program.cs

**As a** developer creating a minimal API or console application  
**I want** the analyzer to ignore `Program.cs` and files with top-level statements  
**So that** I can configure my application's entry point without warnings.

#### 1.6.1. Acceptance Criteria

**Given** a `DbContext` or `SqlConnection` is used in `Program.cs` or a file with top-level statements  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Exempt Generic Infrastructure Services

**As a** developer  
**I want** the analyzer to allow generic infrastructure services like `UnitOfWork` or `DbContextTransactionBehavior` to use `DbContext`  
**So that** I can create reusable infrastructure components.

#### 1.7.1. Acceptance Criteria

**Given** a class name suggests it is a generic infrastructure service (e.g., contains `Transaction`, `UnitOfWork`, `Migration`, `Seeder`)  
**When** it uses `DbContext`  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Exempt Generic Repository Base Classes

**As a** developer  
**I want** the analyzer to allow generic repository base classes to have `DbContext` fields  
**So that** I can create a reusable base for my repositories.

#### 1.8.1. Acceptance Criteria

**Given** a class inherits from a generic repository base class (e.g., `RepositoryBase<TContext>`)  
**When** it has a `DbContext` dependency  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Exempt Domain-Specific EF Extensions

**As a** developer  
**I want** to be able to create domain-specific EF Core extension methods  
**So that** I can create reusable query logic.

#### 1.9.1. Acceptance Criteria

**Given** a static class in a persistence-related namespace provides EF Core extension methods  
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
**I want** a way to opt-out of the repository pattern rule for specific classes  
**So that** I can handle legitimate cases of direct data access.

#### 1.10.1. Acceptance Criteria

**Given** a class is decorated with an `[AllowDirectDataAccess]` attribute  
**When** it uses `DbContext` or other data access types directly  
**Then** no diagnostic should be reported.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).