# IndFusion Analyzer False Positives (Beta Review)

The sections below capture the four false-positive patterns encountered while beta testing `IndFusion.Analyzer` and `IndFusion.Fixer`. Each entry includes a minimal repro, observed behaviour, expected outcome, and suggested fix to carry forward to GitHub issues.

---

## 1. EXXER300 – Overrides Cannot Accept CancellationToken

**Repro**

```csharp
public abstract class BaseProcessor
{
    public abstract Task ExecuteAsync();
}

public sealed class DerivedProcessor : BaseProcessor
{
    public override async Task ExecuteAsync()
    {
        await Task.Delay(10);
    }
}
```

**Observed**  
`AsyncMethodsShouldAcceptCancellationTokenAnalyzer` reports EXXER300 on `DerivedProcessor.ExecuteAsync`.

**Expected**  
Overrides and explicit interface implementations cannot mutate their signatures, so methods inheriting a token-less signature should be exempt.

**Suggested Fix**  
Bail out when `methodSymbol.IsOverride` is true or when `methodSymbol.ExplicitInterfaceImplementations` is non-empty before enforcing the rule. See `src/code/IndFusion.Analyzer/Async/AsyncMethodsShouldAcceptCancellationTokenAnalyzer.cs`.

---

## 2. EXXER301 – ConfigureAwait(false) Warning Inside Tests

**Repro**

```csharp
public sealed class OrderServiceTests
{
    [Fact]
    public async Task Should_Save_Order()
    {
        await _sut.SaveAsync();
    }
}
```

**Observed**  
`UseConfigureAwaitFalseAnalyzer` flags the `await` expression in the test method, demanding `ConfigureAwait(false)`.

**Expected**  
Unit tests (classes ending with `Tests`, methods tagged with `[Fact]`, `[Theory]`, etc.) should be considered application boundary code and excluded from EXXER301.

**Suggested Fix**  
Extend the boundary detection logic (e.g., call into `PatternDetector.DetectTestClass` / `DetectTestAttributes`) before reporting. Relevant file: `src/code/IndFusion.Analyzer/Async/UseConfigureAwaitFalseAnalyzer.cs`.

---

## 3. EXXER003 – Guard Throws Conflict with “No Exceptions” Rule

**Repro**

```csharp
public static void GuardAgainstNull(string input)
{
    if (input is null)
    {
        throw new ArgumentNullException(nameof(input));
    }
}
```

**Observed**  
`DoNotThrowExceptionsAnalyzer` raises EXXER003 on the guard, even though EXXER200 requires null validation.

**Expected**  
Guard clauses throwing `ArgumentNullException`, `ArgumentOutOfRangeException`, or similar BCL guards should be allowed—otherwise EXXER003 blocks compliance with EXXER200.

**Suggested Fix**  
Detect guard patterns or whitelist common guard exception types before reporting. Refer to `src/code/IndFusion.Analyzer/FunctionalPatterns/DoNotThrowExceptionsAnalyzer.cs`.

---

## 4. EXXER302 – Nullable Event-Handler Parameters Misdetected

**Repro**

```csharp
private async void Button_Click(object? sender, EventArgs e)
{
    await _mediator.SendAsync();
}
```

**Observed**  
`AvoidAsyncVoidAnalyzer` reports EXXER302 on the event handler because the nullable annotation (`object?`) prevents the handler heuristic from recognizing the standard signature.

**Expected**  
Event handlers should remain exempt, regardless of nullable annotations.

**Suggested Fix**  
Normalize parameter type names (trim `?`) or use semantic information (`ITypeSymbol.SpecialType == SpecialType.System_Object`) when evaluating handler signatures. File: `src/code/IndFusion.Analyzer/Async/AvoidAsyncVoidAnalyzer.cs`.
