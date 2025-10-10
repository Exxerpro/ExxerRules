# Epic: EXXER600 - DomainShouldNotReferenceInfrastructure Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER600`  
**Source**: `src/code/IndFusion.Analyzer/Architecture/DomainShouldNotReferenceInfrastructureAnalyzer.cs`  
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

### 1.1. Story: Exempt EF Core Attributes on Domain Value Objects

**As a** developer  
**I want** to be able to use EF Core attributes like `[Owned]` on my domain value objects  
**So that** I can configure how they are mapped to the database without violating architectural rules.

#### 1.1.1. Acceptance Criteria

**Given** a domain value object uses an attribute from `Microsoft.EntityFrameworkCore` for metadata purposes  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the `using` directive.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Exempt Domain Enum Seeding Extensions

**As a** developer  
**I want** to create seeding extension methods for my domain enums that use `ModelBuilder`  
**So that** I can keep my seeding logic co-located with my domain model.

#### 1.2.1. Acceptance Criteria

**Given** a static extension method in the domain layer uses `Microsoft.EntityFrameworkCore.ModelBuilder` to provide seeding logic  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Exempt Nested `IEntityTypeConfiguration`

**As a** developer  
**I want** to define `IEntityTypeConfiguration` implementations as nested classes within my domain entities  
**So that** my entity configuration is kept close to the entity itself.

#### 1.3.1. Acceptance Criteria

**Given** a nested class within a domain entity implements `IEntityTypeConfiguration<T>`  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the `using Microsoft.EntityFrameworkCore` directive.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Exempt Domain Tests Using EF InMemory Providers

**As a** developer writing domain unit tests  
**I want** to be able to use the EF Core InMemory provider  
**So that** I can test my domain logic with a lightweight, in-memory database.

#### 1.4.1. Acceptance Criteria

**Given** a file is in a `.UnitTests` or `.Tests` project or namespace  
**And** it uses `Microsoft.EntityFrameworkCore.InMemory` or other EF Core namespaces  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Exempt Domain Tests Validating ModelBuilder Projections

**As a** developer writing domain unit tests  
**I want** to be able to validate `ModelBuilder` projections  
**So that** I can ensure my EF Core mappings are correct.

#### 1.5.1. Acceptance Criteria

**Given** a domain unit test uses `Microsoft.EntityFrameworkCore.Metadata` to inspect model metadata  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Exempt `SqlConnectionStringBuilder` for Guard Logic

**As a** developer  
**I want** to use `SqlConnectionStringBuilder` in my domain for parsing connection strings  
**So that** I can validate connection string components without a direct dependency on data access.

#### 1.6.1. Acceptance Criteria

**Given** a domain service uses `Microsoft.Data.SqlClient.SqlConnectionStringBuilder` for parsing and validation only  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Exempt Provider-Specific Validation in Domain Rules

**As a** developer  
**I want** to use provider-specific types for validation in my domain rules tests  
**So that** I can create realistic test data.

#### 1.7.1. Acceptance Criteria

**Given** a domain unit test uses a provider-specific type like `NpgsqlTypes.NpgsqlInterval` for test data  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Exempt Domain Enum Synchronization Scripts

**As a** developer  
**I want** to create domain-level scripts for synchronizing enums with a database that may use ADO.NET builders  
**So that** I can validate data formats without creating a hard dependency on infrastructure.

#### 1.8.1. Acceptance Criteria

**Given** a domain unit test or utility uses an ADO.NET connection string builder for validation purposes  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Exempt `ValueComparer` Usage in Domain Tests

**As a** developer writing domain unit tests  
**I want** to use EF Core's `ValueComparer` to compare collection snapshots  
**So that** I can test my auditable entities.

#### 1.9.1. Acceptance Criteria

**Given** a domain unit test uses `Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Exempt Migration Snapshot Verification in Domain Tests

**As a** developer writing domain unit tests  
**I want** to verify that my domain model is compatible with EF Core migration snapshots  
**So that** I can catch breaking changes early.

#### 1.10.1. Acceptance Criteria

**Given** a domain unit test uses `Microsoft.EntityFrameworkCore.Migrations` to parse snapshot metadata  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).