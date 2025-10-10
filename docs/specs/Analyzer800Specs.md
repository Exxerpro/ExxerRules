# Epic: EXXER800 - UseStructuredLogging Analyzer False-Positive Mitigation

**Analyzer ID**: `EXXER800`  
**Source**: `src/code/IndFusion.Analyzer/Logging/UseStructuredLoggingAnalyzer.cs`  
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

### 1.1. Story: Verify Receiver is `ILogger`

**As a** developer  
**I want** the analyzer to only flag logging calls made to an `ILogger` instance  
**So that** I don't get false positives from other methods that happen to be named `Log...`.

#### 1.1.1. Acceptance Criteria

**Given** a method call's name starts with "Log" but the receiver is not an `ILogger`  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.1.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.2. Story: Recognize Existing Structured Templates

**As a** developer  
**I want** the analyzer to recognize when I am already using a structured logging template  
**So that** I don't get warnings for correctly formatted log messages.

#### 1.2.1. Acceptance Criteria

**Given** a logging call uses a string literal that already contains structured logging placeholders (e.g., `"User {UserId} requested"`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.2.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.3. Story: Support Interpolated String Handlers

**As a** developer using modern C# and logging features  
**I want** the analyzer to support interpolated string handlers  
**So that** I can use features like `LoggerMessageAttribute` without getting warnings.

#### 1.3.1. Acceptance Criteria

**Given** a logging call uses an interpolated string handler  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.3.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.4. Story: Support Logging Wrapper Helpers

**As a** developer  
**I want** the analyzer to allow logging calls that use a helper method to generate the message template  
**So that** I can create reusable, structured logging messages.

#### 1.4.1. Acceptance Criteria

**Given** the first argument to a logging call is an invocation of a helper method that returns a structured string template  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.4.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.5. Story: Support Interpolation with Positional Arguments

**As a** developer  
**I want** the analyzer to correctly handle interpolated strings that also contain structured logging placeholders  
**So that** I can combine string interpolation with structured logging where appropriate.

#### 1.5.1. Acceptance Criteria

**Given** an interpolated string used in a logging call contains structured logging placeholders (e.g., `$"User {{UserId}} requested"`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.5.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.6. Story: Support Localization Resources

**As a** developer  
**I want** the analyzer to allow the use of localization resources for log messages  
**So that** I can provide localized logging.

#### 1.6.1. Acceptance Criteria

**Given** a logging call uses an `IStringLocalizer` to retrieve the message template  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.6.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.7. Story: Support Other Structured Logging Libraries

**As a** developer using libraries like Serilog or NLog  
**I want** the analyzer to recognize their logging APIs  
**So that** I can use my preferred logging library without false positives.

#### 1.7.1. Acceptance Criteria

**Given** a logging call is made using a known structured logging library like Serilog or NLog  
**When** the analyzer runs  
**Then** no diagnostic should be reported for valid structured logging calls.

#### 1.7.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.8. Story: Exempt Non-Structured Sinks

**As a** developer  
**I want** the analyzer to allow interpolated strings when writing to non-structured sinks like the console  
**So that** I can write simple diagnostic messages without warnings.

#### 1.8.1. Acceptance Criteria

**Given** an interpolated string is passed to a method that is not a structured logger (e.g., `Console.WriteLine`)  
**When** the analyzer runs  
**Then** no diagnostic should be reported.

#### 1.8.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).

---

### 1.9. Story: Exempt Testing Context Output

**As a** developer writing tests  
**I want** the analyzer to allow writing to the test output helper  
**So that** I can log information during test execution.

#### 1.9.1. Acceptance Criteria

**Given** an interpolated string is passed to an `ITestOutputHelper` instance  
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
**I want** a way to opt-out of the structured logging rule for specific cases  
**So that** I can handle legacy code or diagnostic loggers that are intentionally not structured.

#### 1.10.1. Acceptance Criteria

**Given** a method or class is decorated with an `[AllowInterpolatedLogging]` attribute  
**When** the analyzer runs  
**Then** no diagnostic should be reported for logging calls within that scope.

#### 1.10.2. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all scenarios.
- [ ] All new regression tests added and passing.
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`). Zero build test with warning as error treated, 0 failing test on all the test suite.
- [ ] Documentation updated (this spec + release notes).