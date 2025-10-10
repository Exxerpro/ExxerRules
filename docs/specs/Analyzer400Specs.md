# Epic: EXXER400 - PublicMembersShouldHaveXmlDocumentation Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER400`  
**Source**: `src/code/IndFusion.Analyzer/Documentation/PublicMembersShouldHaveXmlDocumentationAnalyzer.cs`  
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

### 1.1. Story: Exempt Blazor Partial Components

**As a** developer of Blazor components  
**I want** the analyzer to ignore partial classes for Blazor components  
**So that** I don't get warnings for UI components where documentation is in the `.razor` file.

#### 1.1.1. Acceptance Criteria

**Given** a public member is in a partial class for a Blazor component (e.g., in a `.razor.cs` file or a class inheriting from `ComponentBase`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Exempt Auto-Generated Files

**As a** developer  
**I want** the analyzer to ignore auto-generated files  
**So that** I don't get warnings for code that I don't maintain.

#### 1.2.1. Acceptance Criteria

**Given** a file is marked with `[GeneratedCode]` or has a conventional generated file name (e.g., `.g.cs`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported for public members in that file.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Inherit Documentation for Record Members

**As a** developer using record types  
**I want** the analyzer to consider the documentation on the record type as sufficient for its auto-generated members  
**So that** I don't have to document every synthesized member of a record.

#### 1.3.1. Acceptance Criteria

**Given** a record type has XML documentation  
**When** the analyzer checks its auto-generated members (primary constructor, properties, `Deconstruct` method)  
**Then** no diagnostic should be reported for those members.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Exempt DTO/ViewModel Properties

**As a** developer  
**I want** the analyzer to exempt properties of DTOs and ViewModels from requiring documentation  
**So that** I don't have to write redundant comments for simple data transfer objects.

#### 1.4.1. Acceptance Criteria

**Given** a public property is in a class that appears to be a DTO or ViewModel (e.g., in a `.Dto` or `.Vm` namespace)  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the property.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Inherit Documentation from Interface Members

**As a** developer  
**I want** the analyzer to recognize that an implemented interface member is already documented  
**So that** I don't have to duplicate documentation on the implementation.

#### 1.5.1. Acceptance Criteria

**Given** a public method or property implements an interface member that has XML documentation  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the implementing member.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Exempt Unit Test Classes

**As a** developer writing tests  
**I want** the analyzer to ignore public members in test classes  
**So that** I don't have to document my test code.

#### 1.6.1. Acceptance Criteria

**Given** a public member is within a test class (e.g., a class with a `[TestFixture]` or `[Fact]` attribute)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Aggregate Documentation from Partial Types

**As a** developer using partial types  
**I want** the analyzer to consider documentation from any part of a partial type as sufficient  
**So that** I can place the documentation in the most logical location.

#### 1.7.1. Acceptance Criteria

**Given** a partial type has XML documentation on one of its declarations  
**When** the analyzer checks other declarations of the same partial type  
**Then** no diagnostic should be reported.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Exempt Minimal API and Top-Level Statements

**As a** developer creating a minimal API or console application  
**I want** the analyzer to ignore public members in files with top-level statements  
**So that** I don't get warnings for my application's entry point.

#### 1.8.1. Acceptance Criteria

**Given** a public member is in a file that uses top-level statements  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Exempt Serialized Fields and Properties

**As a** developer  
**I want** the analyzer to exempt public fields and properties that are marked with serialization attributes  
**So that** I don't have to document members whose purpose is defined by an attribute.

#### 1.9.1. Acceptance Criteria

**Given** a public field or property is decorated with a serialization attribute (e.g., `[JsonPropertyName]`, `[DataMember]`)  
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
**I want** a way to opt-out of the documentation requirement for specific classes or assemblies  
**So that** I can handle legacy code or internal tools where documentation is not necessary.

#### 1.10.1. Acceptance Criteria

**Given** a class, struct, or assembly is decorated with an `[AllowUndocumentedMembers]` attribute  
**When** the analyzer runs  
**Then** no diagnostic should be reported for public members within that scope.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).