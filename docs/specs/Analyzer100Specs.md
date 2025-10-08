# TestNamingConvention Analyzer - False-Positive Mitigation Spec

**Analyzer ID**: `EXXER100`  
**Source**: `src/code/IndFusion.Analyzer/Testing/TestNamingConventionAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-08)

## 0. Selection Rationale

- `docs/specs` contained no entry for EXXER100 even though the analyzer remains enabled (warning severity) in the IndFusion suite.  
- Repository tests show many legitimate naming patterns that deviate from the canonical `Should_Action_When_Condition` shape, causing noise in the IndTrace sample solution:
  - `Test Project/Src/Tests/Core/Domain.UnitTests/CommonTests/FixedSizedStackSequentialTests.cs` prefixes method names with alphabetical ordering (e.g., `AEnqueue_Should_Add_Item_To_Stack`). These exist to force xUnit execution ordering; the analyzer flags the leading `A/ B/ C` segments.
  - `Test Project/Src/Tests/Core/Application.UnitTests/Domain/Events/EventBusTests.cs:10` uses `Constructor_WithValidParameters_ShouldCreateInstance` style naming (common in domain unit tests to highlight the subject under test). EXXER100 raises warnings despite the method being descriptive.
- Teams rely on `AllowTestNamingVariationsAttribute`, nested `Given_/When_` classes, and xUnit `DisplayName` overrides to suppress noise. Documenting the heuristics and mitigation strategies enables tightening the analyzer while respecting valid naming variants.

## 1. Specification

- **Intent**  
  Encourage descriptive test method names that communicate scenario and expectation, roughly following `Should_Action_When_Condition`.

- **Scope**  
  - Analyzer inspects `MethodDeclarationSyntax` nodes flagged as tests by `PatternDetector.DetectTestAttributes`.  
  - Before matching, it evaluates opt-outs: `[AllowTestNamingVariations]` on method or containing type, explicit `[Fact(DisplayName=…)]`/`[Theory(Description=…)]`, or nested classes whose names start with `Given_`/`When_`.  
  - Remaining methods are validated against a flexible regular expression (`flexiblePattern`) supporting optional prefixes, alternate connectors (`When|For|With|If|On`), compound conditions, lowercase tokens, and optional `Async` suffixes. The regex is still strict enough to reject behavior-first names such as `Constructor_WithValidParameters_ShouldCreateInstance`.

## 2. Validation Plan

1. Expand `TestNamingConventionAnalyzerFalsePositiveTests` with the ten scenarios listed below plus existing coverage.  
2. Add integration-style tests that run the analyzer against real IndTrace files (e.g., `FixedSizedStackSequentialTests`) using `AnalyzerTestHelper.RunAnalyzer` and verify zero diagnostics once mitigations land.  
3. Execute `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release`.  
4. Run `dotnet build Test Project/Src/Tests/Core/Application.UnitTests/Core.Application.UnitTests.csproj -c Release` to ensure the IndTrace sample suite builds without EXXER100 noise.

## 3. Enhancement Opportunities (≥10 Items)

Each item documents a current false-positive, mitigation, and a regression snippet that fails today but passes once the analyzer is refined.

### 1. Ordered Test Prefixes (A/B/C…)
- **Problem**: Methods such as `AEnqueue_Should_Add_Item_To_Stack` (`FixedSizedStackSequentialTests.cs:34`) include leading alphabetical prefixes to control ordering. The regex rejects the prefix unless it exactly matches `Should_…`.  
- **Mitigation**: Permit single-letter or numeric prefixes before `Should_` when they’re separated by `_`.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Allow_Alpha_Prefixes()
  {
      const string code = @"
using Xunit;
public class OrderedTests
{
    [Fact]
    public void AEnqueue_Should_Add_Item_To_Stack() { }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new TestNamingConventionAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 2. SubjectFirst Naming (`Constructor_WithValidParameters_ShouldCreateInstance`)
- **Problem**: EventBus tests (`EventBusTests.cs:10`) express subject-first names (MethodUnderTest `_` Should `_`). The regex expects names to start with optional prefixes *followed by `Should_`*.  
- **Mitigation**: Accept patterns like `Target_Should_…` and `Member_With…_Should_…` by tolerating subject segments before `Should_`.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Allow_SubjectFirst_Names()
  {
      const string code = @"
using Xunit;
public class ConstructorTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance() { }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new TestNamingConventionAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 3. DisplayName Overrides Should Suppress Diagnostics
- **Problem**: Methods using `[Fact(DisplayName = "Scenario: user requests data")]` still hit the regex because the analyzer doesn’t short-circuit after detecting a `DisplayName`.  
- **Mitigation**: After detecting `DisplayName`/`Description` arguments, exit before regex validation.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Respect_DisplayName_Overrides()
  {
      const string code = @"
using Xunit;
public class DisplayNameTests
{
    [Fact(DisplayName = ""Scenario: user requests data"")]
    public void AnyNameIsFine() { }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new TestNamingConventionAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 4. Nested Context Classes (Given_/When_)
- **Problem**: Under nested `Given_/When_` context classes, methods such as `ReturnsEmptyResult()` intentionally drop the `Should_…` pattern (`TestNamingConventionAnalyzerFalsePositiveTests.cs:62`). The analyzer currently skips nested contexts, but we rely on fragile string comparisons.  
- **Mitigation**: Strengthen detection for nested context classes (case-insensitive, allow suffixes like `_Context`).  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Skip_When_Inside_Given_Class()
  {
      const string code = @"
using Xunit;
public class Given_UserContext
{
    public class When_NoData
    {
        [Fact]
        public void ReturnsEmptyResult() { }
    }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new TestNamingConventionAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 5. Domain-Descriptive Names Without “Should”
- **Problem**: Tests like `ProcessAsync_Completes_When_NoErrors` or `Service_ReturnsExpectedResponse` appear in IndTrace suites to match domain documentation.  
- **Mitigation**: Allow verbs like `Process_`, `Execute_` followed by `_Should_` or treat names with `_When_` connectors as acceptable even if the initial segment lacks `Should`.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Allow_Domain_Descriptive_Names()
  {
      const string code = @"
using Xunit;
public class DomainTests
{
    [Fact]
    public void ProcessAsync_Should_Return_Response_When_Valid() { }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new TestNamingConventionAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 6. Async Suffix Placement
- **Problem**: Methods like `ProcessAsync_Should_Return_Result_When_Success()` are flagged if the suffix appears before the first underscore or at the end. Current regex only handles trailing `Async`.  
- **Mitigation**: Normalize method names by stripping trailing `_Async` or `Async_` segments before regex evaluation.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Allow_Async_Suffixes()
  {
      const string code = @"
using Xunit;
public class AsyncTests
{
    [Fact]
    public void ProcessAsync_Should_Return_Result_When_Success() { }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new TestNamingConventionAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 7. Subject Prefixes Using PascalCase (Machine_Should_Start_InIdle)
- **Problem**: Many tests in `Test Project/Src/Tests/Core/Domain.UnitTests/…` use PascalCase object names before `Should`, e.g., `Machine_Should_Start_InIdle`. The existing regex handles lowercase `Should_` but is brittle when the subject segment contains uppercase words without underscores.  
- **Mitigation**: Expand prefix handling to cover PascalCase tokens before `Should_`.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Allow_PascalCase_Subjects()
  {
      const string code = @"
using Xunit;
public class MachineTests
{
    [Fact]
    public void Machine_Should_Start_InIdle() { }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new TestNamingConventionAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 8. Tests Describing Negative Scenarios Without Condition Clause
- **Problem**: Methods like `Service_Should_Not_Depend_On_UI` or `Metrics_Should_Track_Reconnection_Count` lack an explicit `_When_` clause but remain descriptive.  
- **Mitigation**: Treat names with `Should_Not_` or `Should_Track_` as well-formed even without a condition suffix.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Allow_Behavior_Only_Names()
  {
      const string code = @"
using Xunit;
public class ArchitectureTests
{
    [Fact]
    public void Services_Should_Not_Depend_On_UI() { }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new TestNamingConventionAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 9. Lowercase Condition Tokens
- **Problem**: Some tests use lowercase condition fragments (`_When_cancelled`). The regex currently expects uppercase or camel-case tokens.  
- **Mitigation**: Normalize tokens to ignore casing before applying the pattern.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Allow_Lowercase_Conditions()
  {
      const string code = @"
using Xunit;
public class CancellationTests
{
    [Fact]
    public void Process_Should_Return_Failure_When_cancelled() { }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new TestNamingConventionAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 10. Opt-Out Attribute Recognition in Nested Types
- **Problem**: `AllowTestNamingVariationsAttribute` defined at class level should suppress analyzer results for child methods, but the current check only looks at the immediate parent class.  
- **Mitigation**: Walk up the syntax tree to honor opt-out attributes applied at any ancestor level.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Respect_OptOut_Attribute_On_Ancestor()
  {
      const string code = @"
using System;
using Xunit;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AllowTestNamingVariationsAttribute : Attribute { }

[AllowTestNamingVariations]
public class SampleSpecs
{
    public class Nested
    {
        [Fact]
        public void GivenUser_WhenAuthenticated_ThenShowsDashboard() { }
    }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new TestNamingConventionAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

## 4. Test-Driven Fix Strategy

- Update `flexiblePattern` (and/or preprocess method names) to tolerate:
  - Optional alphabetical/numeric prefixes before `Should_`
  - Subject-first naming segments (`Constructor_WithValidParameters_…`)
  - Lowercase condition tokens and `_Async` segments  
- Enhance opt-out detection by checking ancestor classes for `[AllowTestNamingVariations]`.  
- Short-circuit when display names or descriptions exist; the current helper should return immediately after detection.  
- Normalize nested context class detection to be case-insensitive and cover suffixes.  
- After implementing, expand `TestNamingConventionAnalyzerFalsePositiveTests` with the snippets above, re-run analyzer tests, and ensure sample projects compile without EXXER100 warnings.
