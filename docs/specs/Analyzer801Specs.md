# Epic: EXXER801 - DoNotUseConsoleWriteLine Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER801`  
**Source**: `src/code/IndFusion.Analyzer/Logging/DoNotUseConsoleWriteLineAnalyzer.cs`  
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

### 1.1. Story: Exempt Console Applications and Tooling

**As a** developer creating a console application or a command-line tool  
**I want** the analyzer to allow the use of `Console.WriteLine` in my application's entry point  
**So that** I can provide output to the user.

#### 1.1.1. Acceptance Criteria

**Given** a `Console.WriteLine` or `Console.Write` call is inside a `Main` method or a class named `Program`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Exempt Build and Deployment Scripts

**As a** developer writing build or deployment scripts  
**I want** to be able to write status information to the console  
**So that** I can monitor the progress of my scripts.

#### 1.2.1. Acceptance Criteria

**Given** a `Console.WriteLine` call is in a file or namespace related to build, deployment, or scripts  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Exempt Conditional Compilation Blocks

**As a** developer  
**I want** to use `Console.WriteLine` inside `#if DEBUG` blocks for diagnostic purposes  
**So that** I can debug my code without getting warnings.

#### 1.3.1. Acceptance Criteria

**Given** a `Console.WriteLine` call is within a `#if DEBUG` or `#if TRACE` conditional compilation block  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Exempt `ConditionalAttribute` Methods

**As a** developer  
**I want** the analyzer to ignore methods decorated with the `[Conditional]` attribute  
**So that** I can create debug-only helper methods that write to the console.

#### 1.4.1. Acceptance Criteria

**Given** a method containing a `Console.WriteLine` call is decorated with `[Conditional("DEBUG")]` or `[Conditional("TRACE")]`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Exempt Unit and Integration Tests

**As a** developer writing tests  
**I want** to be able to use `Console.WriteLine` for temporary diagnostics in my tests  
**So that** I can debug my tests without interference.

#### 1.5.1. Acceptance Criteria

**Given** a `Console.WriteLine` call is inside a test class (e.g., a class with a name ending in `Tests`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Exempt Redirected Console Output

**As a** developer  
**I want** to be able to redirect the console output and write to it  
**So that** I can capture console output for testing or logging purposes.

#### 1.6.1. Acceptance Criteria

**Given** `Console.SetOut` or `Console.SetError` is called in a method  
**When** `Console.WriteLine` is called subsequently in the same method  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Exempt CLI Prompting

**As a** developer creating a command-line tool  
**I want** to be able to prompt the user for input using `Console.Write`  
**So that** I can create interactive CLI experiences.

#### 1.7.1. Acceptance Criteria

**Given** a `Console.Write` call is followed by a `Console.ReadLine` or `Console.ReadKey` call in the same method  
**When** the analyzer runs  
**Then** no diagnostic should be reported for the `Console.Write` call.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Exempt Exception Reporting During Startup

**As a** developer  
**I want** to be able to log fatal startup exceptions to the console before a logger is available  
**So that** I can diagnose application startup failures.

#### 1.8.1. Acceptance Criteria

**Given** a `Console.Error.WriteLine` call is inside a `catch` block that is followed by `Environment.Exit`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Exempt Console Logger Adapters

**As a** developer  
**I want** to be able to create a simple console logger adapter  
**So that** I can have a fallback logging mechanism during development.

#### 1.9.1. Acceptance Criteria

**Given** a class is named `ConsoleLogger` or is decorated with `[AllowConsoleLogging]`  
**When** it uses `Console.WriteLine`  
**Then** no diagnostic should be reported.

#### 1.9.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.10. Story: Exempt Generated Code

**As a** developer  
**I want** the analyzer to ignore generated code  
**So that** I don't get warnings for code that I don't own.

#### 1.10.1. Acceptance Criteria

**Given** a `Console.WriteLine` call is in a file marked with `[GeneratedCode]` or in a `Generated` folder  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).