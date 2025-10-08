# TestNamingConvention Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER100`  
**Source**: `src/code/IndFusion.Analyzer/Testing/TestNamingConventionAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- No spec currently exists for EXXER100.  
- The analyzer enforces the rigid `Should_Action_When_Condition` pattern. IndTrace tests predominantly use `MethodUnderTest_Should_Result_When_Condition`, `Feature_Should_Behavior`, and other variants.  
- Representative violations:
  - `LoadPlcsAsync_Should_Return_Success_When_PLCs_Found` in `Test Project\Src\Tests\Presentation\IndTrace.Oee.Tests\IndTrace.Oee.Tests\RepoPlcServiceTests.cs` (method prefixes context with the async method name).  
  - `Services_Should_Not_Depend_On_UI` in `Test Project\Src\Tests\Architecture\Layers\LayerTests.cs` (feature-oriented prefix).  
  - `Metrics_Should_Track_Messages_Sent_With_Lock_Free_Counters` in `Test Project\Src\Tests\Core\HubConnection.Tests\Unit\Metrics\IHubConnectionMetricsTests.cs`.  
- Because the current rule reports hundreds of legitimate tests, EXXER100 is the highest-volume false-positive analyzer still lacking a mitigation plan.

## 1. Specification

- **Intent**  
  Encourage descriptive test names that communicate expected outcomes using a consistent template.

- **Scope**  
  The analyzer visits every `MethodDeclarationSyntax`, relies on `PatternDetector.DetectTestAttributes` to confirm the method is a test, and then validates the identifier against a single regex: `^Should_[A-Z][a-zA-Z0-9]*(_When_[A-Z][a-zA-Z0-9]*)?$`.

## 2. Validation Plan

1. Create `TestNamingConventionAnalyzerFalsePositiveTests` exercising all scenarios below.  
2. Add integration-level samples from IndTrace test projects to ensure real-world patterns pass.  
3. Run `dotnet test` on analyzer tests and on `Test Project\Src\Tests\...` to confirm diagnostic counts drop.  
4. Preserve positive coverage for truly ambiguous names (e.g., `Test1`, `Should_DoStuff` without context).

## 3. Enhancement Opportunities (>=10 Items)

Each item pairs a documented false-positive with a mitigation and an xUnit/Shouldly test sketch.

### 1. Method-Under-Test Prefix (`LoadPlcsAsync_Should_Return…`)

- **Problem**: Many tests include the method under test before `Should_` (RepoPlcServiceTests mentioned above).  
- **Mitigation**: Allow optional `PascalCase` context before the first `Should_`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_MethodPrefix()
{
    const string testCode = @"
using Xunit;

public class SampleTests
{
    [Fact]
    public void LoadPlcsAsync_Should_Return_Success_When_PLCs_Found() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer())
        .ShouldBeEmpty();
}
```

### 2. Feature-Oriented Prefix (`Services_Should_Not_Depend_On_UI`)

- **Problem**: Architecture tests express the subject first (e.g., `Services_Should...`).  
- **Mitigation**: Permit one or more leading nouns separated by underscores before `Should`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_FeaturePrefix()
{
    const string testCode = @"
using Xunit;

public class ArchitectureTests
{
    [Fact]
    public void Services_Should_Not_Depend_On_UI() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Async Suffix (`*_Async` / `*_Async_When`)

- **Problem**: Async tests naturally include `Async` either before or after the `Should` clause (`LoadPlcsAsync_Should...`).  
- **Mitigation**: Update regex to accept `Async` suffixes adjacent to any segment.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Async_Suffix()
{
    const string testCode = @"
using Xunit;

public class AsyncTests
{
    [Fact]
    public async Task ProcessAsync_Should_Return_Result_When_Success() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. Theory with Scenario Name (`Should_Return_BarCodeDetail_For_Known_Label`)

- **Problem**: Integration tests use `For_` instead of `When_`.  
- **Mitigation**: Allow alternative causal connectors (`When`, `For`, `With`, `If`, `On`).  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Alternate_Causal_Connectors()
{
    const string testCode = @"
using Xunit;

public class BarcodeTests
{
    [Theory]
    public void Should_Return_BarCodeDetail_For_Known_Label() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. Multiple Conditions (`Extensions_Should_Handle_Multiple_Concurrent_Real_Connections`)

- **Problem**: Some scenarios describe multiple conditions with `_And_` or `_With_`.  
- **Mitigation**: Permit additional trailing segments after the condition (e.g., `_When_*_And_*`).  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Compound_Conditions()
{
    const string testCode = @"
using Xunit;

public class HubConnectionExtensionsTests
{
    [Fact]
    public async Task Extensions_Should_Handle_Multiple_Concurrent_Real_Connections() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. Behavior-Only Names (`Metrics_Should_Track_Reconnection_Count`)

- **Problem**: Metrics tests omit the `_When_Condition` suffix.  
- **Mitigation**: Allow `Should_*` without an explicit `_When_` clause.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Behavior_Only()
{
    const string testCode = @"
using Xunit;

public class MetricsTests
{
    [Fact]
    public void Metrics_Should_Track_Reconnection_Count() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. Parameterized Names with Values (`Process_Should_Return_Failure_When_Cancelled`)

- **Problem**: Some tests describe plural scenarios like `_When_Cancelled` or `_When_Request_Null`. Recognise simple words after `_When_` without enforcing PascalCase.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Lowercase_Conditions()
{
    const string testCode = @"
using Xunit;

public class CommandHandlerTests
{
    [Fact]
    public async Task Process_Should_Return_Failure_When_cancelled() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. DisplayName Overrides

- **Problem**: Tests using `[Fact(DisplayName = ...)]` already provide descriptive names, so enforcing method-level naming is redundant.  
- **Mitigation**: Skip diagnostics when `DisplayName` (or `Description`) is present on the attribute.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_DisplayName_Override()
{
    const string testCode = @"
using Xunit;

public class DisplayNameTests
{
    [Fact(DisplayName = ""Scenario: user requests data"")]
    public void Test1() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. Nested Test Classes (e.g., fixtures)

- **Problem**: Inner classes describing contexts (`public class When_No_Data`) contain tests like `ReturnsEmptyResult`. Analyzer still enforces global pattern.  
- **Mitigation**: When method resides in a class whose name already conveys context (`When_*`), relax naming requirements.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Nested_Context_Classes()
{
    const string testCode = @"
using Xunit;

public class Given_Data
{
    public class When_No_Data
    {
        [Fact]
        public void ReturnsEmptyResult() { }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Attribute-Based Opt-Out

- **Problem**: Some teams follow alternative BDD conventions (`Given_When_Then`). Provide an opt-out attribute to silence the analyzer for specific methods or classes.  
- **Mitigation**: Honor `[AllowTestNamingVariations]` (new attribute) on methods/types.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_OptOut_Attribute()
{
    const string testCode = @"
using System;
using Xunit;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AllowTestNamingVariationsAttribute : Attribute { }

public class SampleSpecs
{
    [Fact, AllowTestNamingVariations]
    public void GivenUser_WhenAuthenticated_ThenShowsDashboard() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer())
        .ShouldBeEmpty();
}
```

## 4. Test-Driven Fix Strategy

1. Implement the ten unit tests above (plus positive cases).  
2. Extend the analyzer to:
   - Update the regex to support optional context prefixes, alternative connectors, async suffixes, and multi-segment conditions.  
   - Detect `[Fact(DisplayName=...)]`, nested context classes, and custom opt-out attributes.  
   - Relax case sensitivity for condition fragments without sacrificing readability.  
3. Run analyzer tests to ensure new cases fail before code changes and succeed afterward.  
4. Execute `dotnet test` on IndTrace test projects to verify the warning count drops dramatically.  
5. Record the behavior change in `AnalyzerReleases.Unshipped.md`.

## 5. Acceptance Checklist

- [ ] Analyzer updated with flexible pattern support and opt-outs.  
- [ ] Ten regression tests added/passing.  
- [ ] Solution builds/tests succeed.  
- [ ] EXXER100 warnings significantly reduced across test projects.  
- [ ] Release notes updated.
