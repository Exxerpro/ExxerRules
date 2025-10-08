# AvoidAsyncVoid Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER302`  
**Source**: `src/code/IndFusion.Analyzer/Async/AvoidAsyncVoidAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- Specs exist for analyzers 003, 200, 300, and 301; EXXER302 currently lacks documentation.  
- The analyzer’s simplistic event-handler heuristic (`object sender, EventArgs e`) fails for nullable annotations, derived event args, and override scenarios.  
- In the IndTrace test project (`Test Project\Src`) we observed repeated EXXER302 warnings on legitimate UI handlers (e.g., `private async void Button_Click(object? sender, EventArgs e)` documented in `IndFusionAnalyzerFalsePositives.md`) and on command implementations whose interfaces require `void`.  
- Because EXXER302 remains unspecced and continues to generate false positives in UI, command, and override contexts, it is the next priority for mitigation planning.

## 1. Specification

- **Intent**  
  Discourage the use of `async void` methods because they swallow exceptions and are hard to test, allowing only genuine event handlers or interface-mandated signatures.

- **Scope**  
  Visits `MethodDeclarationSyntax` nodes, checks for the `async` modifier, ensures return type is `void`, skips boundary classes (`*Controller`, namespaces containing `.Web`/`.Presentation`), and determines whether a method “looks like” an event handler via a narrow parameter name/type check.

- **Validation Plan**  
  1. Extend `IndFusion.Analyzer.Tests` with a `AvoidAsyncVoidAnalyzerFalsePositiveTests` suite covering each mitigation item.  
  2. Add regression files representing common UI patterns (Blazor, WPF, WinForms) in `TestCases`.  
  3. Execute `dotnet test` for analyzer tests and targeted IndTrace projects to prove warning reduction.  
  4. Maintain true-positive coverage by keeping tests that ensure non-handler `async void` methods (e.g., service methods) still produce diagnostics.

## 2. Enhancement Opportunities (>=10 Items)

Each enhancement lists the observed false-positive, proposed fix, and a concise xUnit/Shouldly test sample.

### 1. Nullable Sender Parameter

- **Problem**: Event handlers declared as `async void Button_Click(object? sender, EventArgs e)` are flagged because `LooksLikeEventHandler` only matches `object` without nullable suffices.  
- **Mitigation**: Normalize parameter type names by trimming nullable annotations (`?`, `!`) before comparison and use semantic checks (`SpecialType.System_Object`).  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Nullable_Sender()
{
    const string testCode = @"
using System;

public sealed class SampleView
{
    private async void Button_Click(object? sender, EventArgs e)
    {
        await Task.Delay(1);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer())
        .ShouldBeEmpty();
}
```

### 2. Nullable EventArgs Parameter

- **Problem**: Handlers defined as `(object sender, EventArgs? e)` (common when enabling nullable reference types) are still valid but flagged.  
- **Mitigation**: Apply the same nullable normalization to the second parameter.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Nullable_EventArgs()
{
    const string testCode = @"
using System;

public sealed class SampleView
{
    private async void OnLoaded(object sender, EventArgs? e)
    {
        await Task.Delay(1);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Derived EventArgs Types

- **Problem**: Handlers receiving derived event args (e.g., `MouseEventArgs`, `SelectionChangedEventArgs`) are not recognized because the analyzer checks for literal `EventArgs`.  
- **Mitigation**: Use semantic analysis to confirm the second parameter derives from `System.EventArgs`.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Derived_EventArgs()
{
    const string testCode = @"
using System;

public sealed class MouseComponent
{
    private async void OnMouseMove(object sender, MouseEventArgs e)
    {
        await Task.Delay(1);
    }
}

public sealed class MouseEventArgs : EventArgs { }
";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. Routed Event Patterns with `object sender, RoutedEventArgs e`

- **Problem**: WPF/WinUI handlers typically use `RoutedEventArgs`, which the analyzer flags.  
- **Mitigation**: Recognize event-args types within the `System.Windows` and `Microsoft.UI` namespaces automatically.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_RoutedEventArgs()
{
    const string testCode = @"
using System.Windows;

public sealed class View
{
    private async void OnClick(object sender, RoutedEventArgs e)
    {
        await Task.Delay(1);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. Command Execute Methods (`ICommand.Execute`)

- **Problem**: MVVM command implementations must return `void` via `ICommand.Execute(object parameter)` even when asynchronous work is required. Wrapping with `async void` is standard when combined with fire-and-forget error handling.  
- **Mitigation**: If the method implements `System.Windows.Input.ICommand.Execute`, allow `async void` (optionally requiring suppression via attribute).  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_ICommand_Execute()
{
    const string testCode = @"
using System.Threading.Tasks;
using System.Windows.Input;

public sealed class AsyncCommand : ICommand
{
    public bool CanExecute(object parameter) => true;
    public event EventHandler? CanExecuteChanged;

    public async void Execute(object parameter)
    {
        await Task.Delay(1);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. Override Methods with Async Void Signatures

- **Problem**: Framework base classes sometimes expose `async void` overrides (e.g., `protected override async void OnStartup(...)`). The analyzer flags these despite the signature being fixed.  
- **Mitigation**: If `methodSymbol.IsOverride` is true, skip diagnostics.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Override_Async_Void()
{
    const string testCode = @"
using System.Threading.Tasks;

public abstract class Base
{
    protected abstract void OnActivated();
}

public sealed class Derived : Base
{
    protected override async void OnActivated()
    {
        await Task.Delay(1);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. Interface Implementations Requiring `void`

- **Problem**: Some legacy interfaces (e.g., `INotifyPropertyChanged.RaisePropertyChanged`) or COM callbacks require `void`. Implementations using `async void` are flagged.  
- **Mitigation**: Detect explicit/implicit interface implementations and suppress when the interface member returns `void`.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Interface_Implementation()
{
    const string testCode = @"
using System.ComponentModel;

public sealed class Notifier : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public async void Raise(string name)
    {
        await Task.Delay(1);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. EventHandler Delegate Aliases

- **Problem**: Methods assigned to custom delegate types (e.g., `public delegate void AsyncEventHandler(object sender, CustomEventArgs e);`) are valid event handlers but not matched.  
- **Mitigation**: Inspect delegate types attached to events; if the method signature matches the event’s delegate parameters, treat it as an event handler.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Custom_Delegate_Handler()
{
    const string testCode = @"
using System;

public delegate void AsyncEventHandler(object sender, CustomEventArgs e);

public sealed class CustomEventArgs : EventArgs { }

public sealed class Broadcaster
{
    public event AsyncEventHandler? Triggered;

    private async void OnTriggered(object sender, CustomEventArgs e)
    {
        await Task.Delay(1);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. Partial Methods in Blazor Components

- **Problem**: Razor-generated partial classes often declare `async void` lifecycle methods that forward to `Task` returning methods. Generated code may place the parameters in a different order or use `EventCallback`.  
- **Mitigation**: When the containing type inherits from `ComponentBase`, treat `async void` methods whose names start with `On` as event handlers.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Blazor_Partial_Method()
{
    const string testCode = @"
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public partial class SampleComponent : ComponentBase
{
    private async void OnButtonClick(object sender, EventArgs e)
    {
        await Task.Delay(1);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Fire-and-Forget Telemetry Hooks with Explicit Suppression Attribute

- **Problem**: Some fire-and-forget telemetry methods intentionally use `async void` and are decorated with custom attributes (`[FireAndForget]`). The analyzer should respect those opt-outs.  
- **Mitigation**: Honor an attribute-based suppression list (e.g., `[SuppressMessage]`, `[FireAndForget]`, `[DeterministicFireAndForget]`).  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_FireAndForget_Attribute()
{
    const string testCode = @"
using System;

[AttributeUsage(AttributeTargets.Method)]
public sealed class FireAndForgetAttribute : Attribute { }

public sealed class Telemetry
{
    [FireAndForget]
    private async void PublishAsync(string eventName)
    {
        await Task.Delay(1);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidAsyncVoidAnalyzer())
        .ShouldBeEmpty();
}
```

## 3. Test-Driven Fix Strategy

1. Add `AvoidAsyncVoidAnalyzerFalsePositiveTests` to `src/test/IndFusion.Analyzer.Tests/TestCases`.  
2. Populate the ten scenarios above as `[Fact]` tests asserting `ShouldBeEmpty()`.  
3. Retain/extend positive tests ensuring non-event `async void` members still raise EXXER302.  
4. Update analyzer logic iteratively:
   - Normalize nullable annotations and leverage `SemanticModel` for event arg inheritance.  
   - Short-circuit for overrides and interface implementations (`IsOverride`, `ExplicitInterfaceImplementations`).  
   - Examine associated event delegate signatures via `IMethodSymbol` because the analyzer already has semantic context.  
   - Recognize opt-out attributes (`[SuppressMessage]`, custom attributes).  
5. Run the new test suite; confirm each case fails before implementation and passes after adjustments.  
6. Execute `dotnet test` against IndTrace test projects to confirm EXXER302 warning counts decrease.  
7. Document behaviour changes in `AnalyzerReleases.Unshipped.md`.

## 4. Acceptance Checklist

- [ ] Analyzer updated with guardrails for nullable parameters, derived event args, overrides, interface implementations, command handlers, delegate aliases, Blazor components, and suppression attributes.  
- [ ] Ten regression tests added and passing.  
- [ ] Build and test pipelines succeed.  
- [ ] Diagnostic noise for EXXER302 verified to drop in the test project.  
- [ ] Documentation (this spec + release notes) up to date.
