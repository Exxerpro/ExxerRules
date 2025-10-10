# UseShouldly Analyzer - False-Positive Mitigation Spec

**Analyzer ID**: `EXXER102`  
**Source**: `src/code/IndFusion.Analyzer/Testing/DoNotUseFluentAssertionsAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- The analyzer currently only detects `using FluentAssertions;` directives but misses many common FluentAssertions usage patterns in actual test code.
- Member access analysis is implemented but not registered, leading to false negatives where FluentAssertions is used without explicit using statements.
- The analyzer needs to distinguish between legitimate Shouldly usage and FluentAssertions patterns to avoid false positives.

## 1. Specification

- **Intent**  
  Enforce using Shouldly instead of FluentAssertions for test assertions to maintain consistent testing patterns across the project.

- **Scope**  
  Registers `SyntaxKind.UsingDirective` and should register `SyntaxKind.MemberAccessExpression`. It raises warnings when it detects FluentAssertions usage patterns.

## 2. Validation Plan

1. Create a new `UseShouldlyAnalyzerFalsePositiveTests` class with the mitigation scenarios below plus existing positive cases.
2. Update the analyzer to properly register member access analysis and implement robust pattern detection.
3. Run `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release` to confirm all analyzer tests pass.
4. Test with real-world FluentAssertions usage patterns to ensure comprehensive coverage.

## 3. Enhancement Opportunities (≥10 Items)

Each item records an observed or reported false positive, the proposed mitigation, and a regression test snippet (xUnit + Shouldly) that currently fails and will pass after the fix.

### 1. Enable Member Access Analysis

- **Problem**: The analyzer has `AnalyzeMemberAccess` method but doesn't register it, missing FluentAssertions usage without explicit using statements.
- **Mitigation**: Register `SyntaxKind.MemberAccessExpression` in the `Initialize` method.
- **Test**:

```csharp
[Fact]
public void Should_Report_FluentAssertions_MemberAccess_Without_Using()
{
    const string testCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var result = GetResult();
        result.Should().Be(42);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer());
    diagnostics.ShouldNotBeEmpty();
    diagnostics.Single().Id.ShouldBe(DiagnosticIds.UseShouldly);
}
```

### 2. Distinguish Shouldly from FluentAssertions

- **Problem**: Both frameworks use `.Should()` method, causing false positives when Shouldly is used correctly.
- **Mitigation**: Check the namespace context to determine if `.Should()` belongs to Shouldly or FluentAssertions.
- **Test**:

```csharp
[Fact]
public void Should_Not_Report_For_Shouldly_Usage()
{
    const string testCode = @"
using Shouldly;

public class TestClass
{
    public void TestMethod()
    {
        var result = GetResult();
        result.ShouldBe(42);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Handle Global Using Statements

- **Problem**: `global using FluentAssertions;` in GlobalUsings.cs is not detected by the current analyzer.
- **Mitigation**: Check for global using statements and report them appropriately.
- **Test**:

```csharp
[Fact]
public void Should_Report_Global_Using_FluentAssertions()
{
    const string testCode = @"
global using FluentAssertions;

public class TestClass
{
    public void TestMethod()
    {
        var result = GetResult();
        result.Should().Be(42);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer());
    diagnostics.ShouldNotBeEmpty();
}
```

### 4. Ignore FluentAssertions in Documentation Comments

- **Problem**: Documentation samples mentioning FluentAssertions trigger false positives.
- **Mitigation**: Skip analysis of using directives and member access within documentation comments.
- **Test**:

```csharp
[Fact]
public void Should_Not_Report_For_FluentAssertions_In_Documentation()
{
    const string testCode = @"
/// <summary>
/// Example using FluentAssertions: result.Should().Be(42)
/// </summary>
public class TestClass
{
    public void TestMethod()
    {
        var result = GetResult();
        result.ShouldBe(42); // Using Shouldly
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. Handle FluentAssertions in String Literals

- **Problem**: String literals containing FluentAssertions code samples trigger false positives.
- **Mitigation**: Skip analysis of member access within string literals.
- **Test**:

```csharp
[Fact]
public void Should_Not_Report_For_FluentAssertions_In_Strings()
{
    const string testCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var sample = ""result.Should().Be(42)"";
        var result = GetResult();
        result.ShouldBe(42); // Using Shouldly
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. Support Configuration for Legacy Projects

- **Problem**: Projects in migration phase need to gradually transition from FluentAssertions to Shouldly.
- **Mitigation**: Support analyzer configuration to allow specific files or projects to use FluentAssertions temporarily.
- **Test**:

```csharp
[Fact]
public void Should_Respect_Configuration_To_Allow_FluentAssertions()
{
    const string testCode = @"
using FluentAssertions;

public class TestClass
{
    public void TestMethod()
    {
        var result = GetResult();
        result.Should().Be(42);
    }
}";

    var options = AnalyzerOptionsFactory.Create(
        ("dotnet_diagnostic.EXXER102.allow_fluent_assertions", "true"));

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer(),
        analyzerOptions: options)
        .ShouldBeEmpty();
}
```

### 7. Detect FluentAssertions Extension Methods

- **Problem**: FluentAssertions extension methods like `BeEquivalentTo`, `Contain`, etc. are not detected.
- **Mitigation**: Expand the list of FluentAssertions methods and improve chain detection.
- **Test**:

```csharp
[Fact]
public void Should_Report_FluentAssertions_Extension_Methods()
{
    const string testCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var list = new[] { 1, 2, 3 };
        list.Should().Contain(2);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer());
    diagnostics.ShouldNotBeEmpty();
}
```

### 8. Handle FluentAssertions in Conditional Compilation

- **Problem**: FluentAssertions usage in `#if DEBUG` blocks may be legitimate for debugging purposes.
- **Mitigation**: Allow FluentAssertions in conditional compilation blocks with appropriate configuration.
- **Test**:

```csharp
[Fact]
public void Should_Allow_FluentAssertions_In_Debug_Blocks_When_Configured()
{
    const string testCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var result = GetResult();
#if DEBUG
        result.Should().Be(42);
#endif
        result.ShouldBe(42); // Using Shouldly
    }
}";

    var options = AnalyzerOptionsFactory.Create(
        ("dotnet_diagnostic.EXXER102.allow_in_debug", "true"));

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer(),
        analyzerOptions: options)
        .ShouldBeEmpty();
}
```

### 9. Improve Chain Detection Logic

- **Problem**: Current `IsPartOfFluentAssertionsChain` method is too simplistic and may miss complex FluentAssertions chains.
- **Mitigation**: Implement more robust chain detection that follows the full FluentAssertions API patterns.
- **Test**:

```csharp
[Fact]
public void Should_Detect_Complex_FluentAssertions_Chains()
{
    const string testCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var person = GetPerson();
        person.Name.Should().NotBeNullOrEmpty().And.Be(""John"");
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer());
    diagnostics.ShouldNotBeEmpty();
}
```

### 10. Provide Better Error Messages

- **Problem**: Current error messages don't provide clear guidance on how to migrate from FluentAssertions to Shouldly.
- **Mitigation**: Include specific migration suggestions in diagnostic messages.
- **Test**:

```csharp
[Fact]
public void Should_Provide_Helpful_Migration_Message()
{
    const string testCode = @"
using FluentAssertions;

public class TestClass
{
    public void TestMethod()
    {
        var result = GetResult();
        result.Should().Be(42);
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer());
    diagnostics.ShouldNotBeEmpty();
    diagnostics.Single().GetMessage().ShouldContain("use Shouldly");
}
```

## 4. Test-Driven Fix Strategy

- Add `UseShouldlyAnalyzerFalsePositiveTests` with the ten scenarios above plus existing positive-control tests.
- Extend `DoNotUseFluentAssertionsAnalyzer` to:
  - Register member access analysis properly.
  - Distinguish between Shouldly and FluentAssertions usage.
  - Handle global using statements.
  - Skip analysis in documentation and string literals.
  - Support configuration options for migration scenarios.
  - Improve chain detection logic.
  - Provide better error messages with migration guidance.
- Wire configuration helpers into `AnalyzerTestHelper` to support the new configuration options.
- Run analyzer tests to confirm all scenarios pass and false positives are eliminated.