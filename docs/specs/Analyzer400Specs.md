# PublicMembersShouldHaveXmlDocumentation Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER400`  
**Source**: `src/code/IndFusion.Analyzer/Documentation/PublicMembersShouldHaveXmlDocumentationAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- No spec currently exists for EXXER400.  
- The analyzer reports Info-level diagnostics on every public member lacking `///` XML comments. In IndTrace, thousands of generated/partial/domain DTO types are public without XML docs, so the rule generates pervasive noise.  
- Representative hot spots:
  - Blazor partial classes such as `Test Project\Src\Code\Presentation\IndTrace.Components\Area\OEE\TrendSparkline.razor.cs` (UI components where documentation is redundant and markup lives in `.razor`).  
  - Record types exposed as domain value objects (`Test Project\Src\Code\Core\Domain\ValueObjects\OeeMetrics.cs`), where constructors or deconstruct members trigger repeated warnings even when the type itself is documented.  
  - DTOs and ViewModels in `Test Project\Src\Code\Core\Application\WorkFlows\Queries\GetList\VariableListVm.cs` and similar files created solely for serialization; XML comments add little value and often fall out of sync.

Given the volume of false positives and the impact on developer experience, EXXER400 is the next analyzer requiring a mitigation blueprint.

## 1. Specification

- **Intent**  
  Encourage meaningful XML documentation on public-facing APIs that need IntelliSense support and consumer guidance.

- **Scope**  
  Registers syntax node actions on class/struct/record/interface/enum declarations and on public methods, properties, fields (non-const), and events. A diagnostic is raised whenever the member is public (or part of an interface) and lacks raw XML doc trivia.

## 2. Validation Plan

1. Build `PublicMembersShouldHaveXmlDocumentationAnalyzerFalsePositiveTests` to encode the scenarios below.  
2. Add representative integration files (Blazor components, record DTOs, generated migrations).  
3. Run `dotnet test` on analyzer suites and IndTrace projects to confirm the warning count shrinks dramatically.  
4. Preserve positive cases (public API surfaces in core libraries without documentation) so genuine coverage gaps remain visible.

## 3. Enhancement Opportunities (>=10 Items)

Each entry highlights an observed false-positive, proposes a fix, and includes an xUnit/Shouldly test sketch.

### 1. Blazor Partial Components (`*.razor.cs`)

- **Issue**: UI components (partial classes) derive from markup; duplicating XML comments in the `.razor.cs` file adds no value.  
- **Mitigation**: Skip partial classes whose file path or namespace indicates Blazor (`*.razor.cs`, namespace contains `.Components` or `.Pages`, base type derives from `ComponentBase`).  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Blazor_Partial_Class()
{
    const string testCode = @"
using Microsoft.AspNetCore.Components;

namespace Sample.Components;

public partial class ExampleComponent : ComponentBase
{
    [Parameter] public string Title { get; set; } = string.Empty;
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new PublicMembersShouldHaveXmlDocumentationAnalyzer())
        .ShouldBeEmpty();
}
```

### 2. Auto-Generated or Tool-Generated Files

- **Issue**: EF migrations (`CreateIdentitySchema`), Swagger clients, and designer files are generated and include `[GeneratedCode]`. The analyzer should not require manual XML docs.  
- **Mitigation**: Detect `[GeneratedCode]`, `[DebuggerNonUserCode]`, or file names ending with `.g.cs`, `.generated.cs`, `Migrations\*.cs`, and skip.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Generated_Code()
{
    const string testCode = @"
using System;
using System.CodeDom.Compiler;

[GeneratedCode(""Tool"", ""1.0"")]
public class GeneratedRunner
{
    public void Execute() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new PublicMembersShouldHaveXmlDocumentationAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Record Types with Documentation on the Type

- **Issue**: When a record is documented, the auto-generated `Deconstruct`, properties, and `PrimaryConstructor` still trigger warnings.  
- **Mitigation**: Inherit documentation from the record declaration for synthesized members (auto props, primary constructor parameters).  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Documented_Record_Members()
{
    const string testCode = @"
/// <summary>Represents a simple DTO.</summary>
public record UserDto(string Name, string Email);
";

    AnalyzerTestHelper.RunAnalyzer(testCode, new PublicMembersShouldHaveXmlDocumentationAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. DTO/ViewModel Types in Application Layer

- **Issue**: Public classes like `VariableListVm` expose auto-properties meant for serialization. Enforcing XML docs on every property is unnecessary noise.  
- **Mitigation**: When a type resides in namespaces ending with `.Dto`, `.Vm`, `.Models`, `.Messages`, skip property-level diagnostics (optionally allowing type-level comment only).  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Dto_Properties()
{
    const string testCode = @"
namespace Sample.Application.Variables;

public class VariableVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new PublicMembersShouldHaveXmlDocumentationAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. Interface Members Inherit Documentation

- **Issue**: Class implementations of documented interfaces still trigger diagnostics (analyzer only skips explicit implementations).  
- **Mitigation**: When a method overrides/implements a member with existing XML documentation (via symbols), inherit comments automatically.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Interface_Implementations()
{
    const string testCode = @"
/// <summary>Contract.</summary>
public interface IService
{
    /// <summary>Does work.</summary>
    void DoWork();
}

public class Service : IService
{
    public void DoWork() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new PublicMembersShouldHaveXmlDocumentationAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. Unit Test Classes

- **Issue**: Test classes/methods (decorated with `[Fact]`, `[Theory]`) do not need XML docs; attributes already convey intent.  
- **Mitigation**: Skip diagnostics on methods/types identified as tests via `PatternDetector.DetectTestAttributes`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Test_Methods()
{
    const string testCode = @"
using Xunit;

public class SampleTests
{
    [Fact]
    public void Should_Do_Something() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new PublicMembersShouldHaveXmlDocumentationAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. Partial Types with Documentation in Companion File

- **Issue**: Partial classes where one part has XML docs should satisfy the requirement. The analyzer currently inspects each partial declaration in isolation.  
- **Mitigation**: Aggregate XML doc presence across partial declarations via symbol analysis.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_XmlDoc_On_Any_Partial()
{
    const string testCode = @"
public partial class Widget
{
    public void PartA() { }
}

/// <summary>Widget partial docs.</summary>
public partial class Widget
{
    public void PartB() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new PublicMembersShouldHaveXmlDocumentationAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. Minimal API/Top-Level Statements

- **Issue**: Top-level programs or static methods used in minimal APIs do not benefit from XML docs.  
- **Mitigation**: Skip diagnostics for files using top-level statements or `internal static partial class Program`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_TopLevel_Statements()
{
    const string testCode = @"
using System;

Console.WriteLine(""Hello"");
";

    AnalyzerTestHelper.RunAnalyzer(testCode, new PublicMembersShouldHaveXmlDocumentationAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. Public Fields with `[JsonPropertyName]`/Data Annotations

- **Issue**: Records or classes with annotated public fields/properties (serialize-only) are flagged even though annotations already document purpose.  
- **Mitigation**: Skip members decorated with serialization attributes (`JsonPropertyName`, `JsonProperty`, `DataMember`).  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Serialization_Fields()
{
    const string testCode = @"
using System.Text.Json.Serialization;

public class Payload
{
    [JsonPropertyName(""id"")]
    public int Id { get; set; }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new PublicMembersShouldHaveXmlDocumentationAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Opt-Out Attribute for Legacy Code

- **Issue**: Some assemblies intentionally skip XML docs (internal tools, throwaway prototypes). Provide a lightweight opt-out attribute.  
- **Mitigation**: Respect `[AllowUndocumentedMembers]` on classes/namespaces.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_OptOut_Attribute()
{
    const string testCode = @"
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly)]
public sealed class AllowUndocumentedMembersAttribute : Attribute { }

[AllowUndocumentedMembers]
public class LegacyTool
{
    public void Execute() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new PublicMembersShouldHaveXmlDocumentationAnalyzer())
        .ShouldBeEmpty();
}
```

## 4. Test-Driven Fix Strategy

1. Add the ten regression tests to a new fixture and ensure they fail with current analyzer behavior.  
2. Enhance `PublicMembersShouldHaveXmlDocumentationAnalyzer` to:
   - Aggregate documentation across partial declarations and inherited members.  
   - Use semantic checks for component base types, generated-code attributes, serialization annotations, and test attributes.  
   - Accept optional opt-out attributes and namespace patterns for DTOs/models.  
3. Re-run analyzer tests verifying new cases pass post-change while existing true positives remain.  
4. Execute `dotnet test` against IndTrace projects to quantify the reduction in EXXER400 diagnostics.  
5. Update `AnalyzerReleases.Unshipped.md` documenting the broadened heuristics and opt-out mechanisms.

## 5. Acceptance Checklist

- [ ] Analyzer updated with context-aware exemptions and attribute opt-outs.  
- [ ] Ten regression tests added/passing.  
- [ ] Solution builds/tests succeed.  
- [ ] Documented drop in EXXER400 warnings across Blazor components, DTOs, and generated files.  
- [ ] Release notes updated to reflect new behavior.
