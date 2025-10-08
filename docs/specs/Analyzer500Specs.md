# AvoidMagicNumbersAndStrings Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER500`  
**Source**: `src/code/IndFusion.Analyzer/CodeQuality/AvoidMagicNumbersAndStringsAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- Specs now exist for analyzers 003, 200, 300, 301, and 302, but EXXER500 remains undocumented.  
- The IndTrace solution contains numerous legitimate numeric thresholds and domain messages (e.g., `ProductValidator`, `ShiftDetectionRule`, `OeePerformanceLevel`) that surface EXXER500 warnings because the analyzer relies on string heuristics and treats nearly every literal as “magic.”  
- Examples from `Test Project\Src`:
  - `OeePerformanceLevel` enum (`Poor = 0`, `WorldClass = 3`) – values are not “magic” but part of the domain model.  
  - `ProductValidator` uses business rules like `partNumber.Length < 3`, `> 80`, and descriptive error strings (lines 27-64).  
  - `ShiftDetectionRule` enforces hours between 0 and 23 (`ShiftDetectionRule.cs:29-47`).  
  - Regex validation `Regex.IsMatch(partNumber, @"^[A-Za-z0-9\-]+$")` (line 53) and other pattern strings are flagged.  
- Because EXXER500 drives widespread noise without a spec, it is the next target for mitigation.

## 1. Specification

- **Intent**  
  Encourage developers to avoid unexplained literals by nudging them toward named constants.

- **Scope**  
  Registers syntax node actions for numeric and string literal expressions. It skips const/static readonly declarations, attribute arguments, switch labels, and a fixed list of “common” numbers/strings. Every other literal produces a diagnostic regardless of context.

- **Validation Plan**  
  1. Add `AvoidMagicNumbersAndStringsAnalyzerFalsePositiveTests` covering the ten scenarios listed below.  
  2. Introduce regression cases (enums, validators, regex patterns, time spans) in analyzer tests.  
  3. Run `dotnet test` for analyzer/test projects and sample IndTrace projects to ensure warning count drops.  
  4. Maintain positive cases ensuring actual magic literals (e.g., `if (x == 17)` in domain code) still trigger warnings.

## 2. Enhancement Opportunities (>=10 Items)

Each item identifies an existing false-positive, proposes analyzer improvements, and includes an xUnit/Shouldly test snippet.

### 1. Enum Member Values

- **Issue**: Enum declarations such as `WorldClass = 3` are flagged even though explicit values are part of the contract.  
- **Proposal**: When the literal belongs to an `EnumMemberDeclarationSyntax`, skip diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Enum_Members()
{
    const string testCode = @"
public enum PerformanceLevel
{
    Poor = 0,
    Fair = 1,
    Good = 2,
    WorldClass = 3
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer())
        .ShouldBeEmpty();
}
```

### 2. Bit-Flag Enums (`1 << n`)

- **Issue**: `[Flags]` enums using bit shifts (e.g., `Read = 1 << 0`) are reported.  
- **Proposal**: Recognize enum member assignments that use bit-shift expressions or power-of-two literals.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Flag_Enum_Shifts()
{
    const string testCode = @"
using System;

[Flags]
public enum Permissions
{
    None = 0,
    Read = 1 << 0,
    Write = 1 << 1,
    Execute = 1 << 2
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Domain Range Guards

- **Issue**: Guard clauses like `if (hour < 0 || hour > 23)` (ShiftDetectionRule) are flagged despite being explicit domain requirements.  
- **Proposal**: Detect numeric literals used exclusively within comparison operators tied to `ArgumentOutOfRangeException` or validation logic and skip them.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Domain_Range_Checks()
{
    const string testCode = @"
using System;

public static class ShiftRules
{
    public static void ValidateHour(int hour)
    {
        if (hour < 0 || hour > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(hour));
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. Business Rule Thresholds

- **Issue**: Validation logic like `partNumber.Length < 3` and `> 80` (ProductValidator) is flagged.  
- **Proposal**: Skip numeric literals when they appear in string-length or collection-count comparisons within validation methods.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Length_Thresholds()
{
    const string testCode = @"
using System.Collections.Generic;

public static class Validator
{
    public static List<string> Validate(string value)
    {
        var errors = new List<string>();
        if (value.Length < 3) errors.Add(""Too short"");
        if (value.Length > 80) errors.Add(""Too long"");
        return errors;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. Exception Messages

- **Issue**: Strings passed to exceptions (e.g., `"Hour must be between 0 and 23"`) are flagged though they provide crucial diagnostics.  
- **Proposal**: Whitelist strings passed as the message argument to `Exception` subclasses.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Exception_Message()
{
    const string testCode = @"
using System;

public static class Hours
{
    public static void Validate(int hour)
    {
        if (hour < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hour), ""Hour must be between 0 and 23"");
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. Result/Validation Messages

- **Issue**: Domain validation uses descriptive messages stored in `List<string>` or returned via `Result.WithFailure(...)`; EXXER500 flags them as magic.  
- **Proposal**: When a string literal is added to a collection named `errors`/`warnings` or passed to `Result.WithFailure`, consider it intentional.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Validation_Message_Collections()
{
    const string testCode = @"
using System.Collections.Generic;

public static class ProductValidator
{
    public static List<string> Validate()
    {
        var errors = new List<string>();
        errors.Add(""CustomerId must be greater than 0."");
        return errors;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. Regex and Pattern Literals

- **Issue**: Regex patterns (`@"^[A-Za-z0-9\-]+$"`) or SQL fragments are reported.  
- **Proposal**: Detect strings containing regex metacharacters or common SQL keywords and exempt them.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Regex_Patterns()
{
    const string testCode = @"
using System.Text.RegularExpressions;

public static class PartNumberRules
{
    private static readonly Regex Pattern = new(@""^[A-Za-z0-9\\-]+$"");
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. Culture and Locale Codes

- **Issue**: Strings like `"en-US"` passed to `CultureInfo.GetCultureInfo` or `new CultureInfo` are flagged.  
- **Proposal**: When the literal matches the `xx-XX` culture code pattern or is used with `CultureInfo`, `RegionInfo`, etc., skip diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_CultureCodes()
{
    const string testCode = @"
using System.Globalization;

public static class Localization
{
    public static CultureInfo DefaultCulture => CultureInfo.GetCultureInfo(""en-US"");
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. TimeSpan and DateTime Construction

- **Issue**: Literals used inside `TimeSpan.FromSeconds(30)` or `new TimeSpan(0, 0, 30)` are flagged even though they express durations.  
- **Proposal**: When numeric literals are arguments to `TimeSpan`/`DateTime` factory APIs, exempt them.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_TimeSpan_Factory_Values()
{
    const string testCode = @"
using System;

public static class Scheduler
{
    public static TimeSpan DefaultBackoff => TimeSpan.FromSeconds(30);
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Logging Message Templates

- **Issue**: Structured logging still requires literal templates (`""Processing {OrderId}""`); the analyzer flags them.  
- **Proposal**: When a string literal is the first argument to `ILogger.Log` methods, skip diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Logging_Templates()
{
    const string testCode = @"
using Microsoft.Extensions.Logging;

public sealed class Handler
{
    private readonly ILogger<Handler> _logger;

    public Handler(ILogger<Handler> logger) => _logger = logger;

    public void Handle(int orderId)
    {
        _logger.LogInformation(""Processing order {OrderId}"", orderId);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer())
        .ShouldBeEmpty();
}
```

## 3. Test-Driven Fix Strategy

1. Add the ten cases above as `[Fact]` tests under `AvoidMagicNumbersAndStringsAnalyzerFalsePositiveTests`.  
2. Keep existing positive coverage ensuring arbitrary literals still trigger EXXER500.  
3. Update the analyzer logic:
   - Detect enum contexts (`EnumMemberDeclarationSyntax`) and bit-shift patterns.  
   - Use `SemanticModel` to classify validation guards, logging calls, exception constructors, regex/culture APIs, and time factories.  
   - Extend string heuristics to recognise validation/error collection usage.  
   - Cache expensive semantic lookups for performance.  
4. Run analyzer tests before and after changes to confirm the new cases fail first and pass after mitigation.  
5. Execute `dotnet test` on representative IndTrace projects to observe the reduction in EXXER500 warnings.  
6. Update `AnalyzerReleases.Unshipped.md` summarising the improved literal classification.

## 4. Acceptance Checklist

- [ ] Analyzer updated with guardrails for enums, validation thresholds, exception/log messages, regex patterns, culture codes, and time factories.  
- [ ] Ten regression tests added/passing.  
- [ ] Solution builds/tests succeed.  
- [ ] Diagnostic noise from EXXER500 materially reduced across IndTrace projects.  
- [ ] Release notes updated accordingly.
