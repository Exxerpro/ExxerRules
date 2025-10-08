# UseExpressionBodiedMembers Analyzer - False-Positive Mitigation Spec

**Analyzer ID**: `EXXER501`  
**Source**: `src/code/IndFusion.Analyzer/ModernCSharp/UseExpressionBodiedMembersAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- `docs/specs` has no entry for EXXER501 even though modern C# analyzers (702) already have a mitigation plan.  
- EXXER501 reports on any method/property whose body is a single `return` statement. In IndTrace, dozens of command factories and reset hooks follow a block-bodied pattern for readability, documentation, and future extensibility.  
- Evidence:
  - `Test Project/Src/Code/Core/Application/BarCodes/Commands/Create/CreateBarCodeCommand.cs:47` (`Create` factory)  
  - `Test Project/Src/Code/Core/Application/Cycles/Commands/Create/CreateCyclesCommand.cs:37` (`Create` factory)  
  - `Test Project/Src/Code/Core/Application/BarCodes/Commands/Create/CreateBarCodeCommand.cs:73` (`TryReset`)  
  - `Test Project/Src/Code/Core/Application/BarCodes/Commands/Create/CreateBarCodeCommandHandler.cs:227` (`TryReset`)  
  - `src/code/IndFusion.Mcp.Server/Services/McpServerBuilder.cs:37` (`WithWebSocketTransport`)  
- These diagnostics are noisy enough that developers leave the block bodies untouched, so EXXER501 needs a mitigation spec before broader adoption.

## 1. Specification

- **Intent**  
  Encourage concise expression-bodied members where simple `return` statements suffice, aligning with modern C# style guidance.

- **Scope**  
  Registers `SyntaxKind.MethodDeclaration` and `SyntaxKind.PropertyDeclaration`. For each node, if the body exists and `CanBeExpressionBodied` (single `return` statement) returns true, it reports an info-level diagnostic suggesting expression-bodied form.

## 2. Validation Plan

1. Create `UseExpressionBodiedMembersAnalyzerFalsePositiveTests` covering every mitigation scenario below plus a control case that should still trigger.  
2. Ensure existing tests in `CodeQualityTests` keep the positive coverage (e.g., trivial helper method that should convert).  
3. Run `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release` before and after implementing mitigations.  
4. Once mitigations land, build the IndTrace solution (Release) to confirm EXXER501 noise disappears from `Test Project/Src`. Capture diagnostic counts before/after in release notes.

## 3. Enhancement Opportunities (≥10 Items)

Each item documents the observed false positive, proposes the mitigation, and sketches an xUnit + Shouldly regression test.

### 1. ICommandData Factory Methods Returning New Command Instances

- **Problem**: Command types implement `ICommandData.Create` as a block-bodied factory returning `new <Command>(taskGatewayRequest);`. EXXER501 flags methods such as `CreateBarCodeCommand.Create` (`Test Project/Src/Code/Core/Application/BarCodes/Commands/Create/CreateBarCodeCommand.cs:47`) even though the block is intentional for symmetry with other fluent members and to host XML docs.  
- **Mitigation**: When the method implements `ICommandData.Create`, returns a `new` expression, and the containing type ends with `Command`/`Query`, skip diagnostics. Use semantic checks (`methodSymbol.ContainingType.AllInterfaces`) and ensure the return expression constructs the containing type.  
- **Test**:

```csharp
[Fact]
public void Should_NotReport_For_ICommandData_Create_Factory()
{
    const string testCode = @"
using IndTrace.Domain.Entities;

public interface ICommandData { TaskGatewayRequest Command { get; set; } ICommandData Create(TaskGatewayRequest req); }

public sealed class CreateBarCodeCommand : ICommandData
{
    public TaskGatewayRequest Command { get; set; } = new();

    public ICommandData Create(TaskGatewayRequest request)
    {
        return new CreateBarCodeCommand(request);
    }

    private CreateBarCodeCommand(TaskGatewayRequest req) => Command = req;
    public CreateBarCodeCommand() { }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

### 2. Update Command Factories Mirroring the Pattern

- **Problem**: Other gateway commands (`UpdateBarcodeCommand.Create`, `UpdateCyclesOkCommand.Create`, `UpdateCyclesNotOkCommand.Create`) hit the same heuristic and are equally noisy.  
- **Mitigation**: Reuse the interface-based suppression from item 1 so every `ICommandData.Create` implementation returning a new command instance is ignored, regardless of concrete type.  
- **Test**:

```csharp
[Fact]
public void Should_NotReport_For_UpdateCommand_Create_Factory()
{
    const string testCode = @"
public interface ICommandData { ICommandData Create(string payload); }

public sealed class UpdateCyclesOkCommand : ICommandData
{
    public ICommandData Create(string payload)
    {
        return new UpdateCyclesOkCommand(payload);
    }

    private UpdateCyclesOkCommand(string _) { }
    public UpdateCyclesOkCommand() { }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

### 3. Query Factories (ReadBarCodeQuery.Create)

- **Problem**: Query objects also implement `ICommandData.Create` but return a specialized query. EXXER501 still fires (see `ReadBarCodeQuery.cs:34`).  
- **Mitigation**: Extend the rule so it tolerates `return new ReadBarCodeQuery(...)` when the constructed type equals the containing type or implements `ICommandData`.  
- **Test**:

```csharp
[Fact]
public void Should_NotReport_For_Query_Create_Factory()
{
    const string testCode = @"
public interface ICommandData { ICommandData Create(int id); }

public sealed class ReadBarCodeQuery : ICommandData
{
    public ICommandData Create(int id)
    {
        return new ReadBarCodeQuery(id);
    }

    private ReadBarCodeQuery(int _) { }
    public ReadBarCodeQuery() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. Performance Command Factories

- **Problem**: `PerformanceDataCommandFactory.cs:33` uses the same pattern but maps to performance-specific commands.  
- **Mitigation**: Covered by the generalized factory suppression; add a regression test ensuring no diagnostic when the constructed type aliases the containing type.  
- **Test**:

```csharp
[Fact]
public void Should_NotReport_For_PerformanceCommand_Create()
{
    const string testCode = @"
public interface ICommandData { ICommandData Create(object ctx); }

public sealed class PerformanceDataCommand : ICommandData
{
    public ICommandData Create(object ctx)
    {
        return new PerformanceDataCommand(ctx);
    }

    private PerformanceDataCommand(object _) { }
    public PerformanceDataCommand() { }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. Fluent TODO Stubs (McpServerBuilder.WithWebSocketTransport)

- **Problem**: `McpServerBuilder.WithWebSocketTransport` (`src/code/IndFusion.Mcp.Server/Services/McpServerBuilder.cs:37`) intentionally keeps a block with a `// TODO` comment and `return this;` placeholder. EXXER501 suggests an expression body, erasing the design note.  
- **Mitigation**: If the body contains leading trivia with `TODO/FIXME` (or a comment-only block), skip the diagnostic.  
- **Test**:

```csharp
[Fact]
public void Should_NotReport_For_Method_With_Todo_Placeholder()
{
    const string testCode = @"
public sealed class McpServerBuilder
{
    public McpServerBuilder WithWebSocketTransport()
    {
        // TODO: add WebSocket pipeline once reverse proxy lands
        return this;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. IResettable.TryReset Returning Constant Success

- **Problem**: Gateway commands implement `IResettable.TryReset` and currently return a constant `true` (e.g., `CreateBarCodeCommand.cs:73`). The block form matches the project’s doc-comment style and leaves room for future logic.  
- **Mitigation**: If the method name is `TryReset`, returns `bool`, and the containing type implements `IResettable`, suppress diagnostics when the body is a single `return true/false;`.  
- **Test**:

```csharp
[Fact]
public void Should_NotReport_For_TryReset_Interface_Implementation()
{
    const string testCode = @"
public interface IResettable { bool TryReset(); }

public sealed class CreateBarCodeCommand : IResettable
{
    public bool TryReset()
    {
        return true;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. Handler-Level TryReset Hooks

- **Problem**: Handlers such as `CreateBarCodeCommandHandler.cs:227` also implement `IResettable.TryReset` with a stubbed return value. The analyzer reports the same false positive.  
- **Mitigation**: The interface-based suppression from item 6 should extend to all `IResettable` implementations, including handlers. Add a regression test to cover non-command types.  
- **Test**:

```csharp
[Fact]
public void Should_NotReport_For_Handler_TryReset()
{
    const string testCode = @"
public interface IResettable { bool TryReset(); }

public sealed class CreateBarCodeCommandHandler : IResettable
{
    public bool TryReset()
    {
        return true;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. Command WithData Fluent Blocks Returning `this`

- **Problem**: `CreateBarCodeCommand.WithData` (and other commands) set state then `return this;`. Although multi-statement methods are already ignored, the analyzer still fires on variants such as `WithData` implementations copied into smaller helpers that currently only set one property. When the body contains both an assignment and a return, the analyzer should skip reporting even if future refactors temporarily leave only `return this;`.  
- **Mitigation**: Extend `CanBeExpressionBodied` guard to require *exactly one* statement **and** no leading comments/attributes. If the body contains comments or will likely gain statements (e.g., method name starts with `With`), skip diagnostics.  
- **Test**:

```csharp
[Fact]
public void Should_NotReport_For_Fluent_WithData_Method()
{
    const string testCode = @"
public sealed class CreateBarCodeCommand
{
    public CreateBarCodeCommand WithData(string value)
    {
        // Parameter normalization expected to grow here
        return this;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. Fluent Builder Methods Returning `this` After Guarded Assignment

- **Problem**: Some command methods currently only return `this` because the guard logic lives elsewhere (e.g., generated stubs). Forcing expression-bodied syntax makes it harder to add guards later and breaks formatting parity with neighbouring methods.  
- **Mitigation**: When a method name starts with `With`/`Set` and the return expression is `this`, require at least one statement before returning to trigger diagnostics; otherwise, suppress.  
- **Test**:

```csharp
[Fact]
public void Should_NotReport_For_Fluent_WithMethod_ReturningThis()
{
    const string testCode = @"
public sealed class CommandBuilder
{
    public CommandBuilder WithPlaceholder()
    {
        return this;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Domain Entity Resetters Resetting Mutable State

- **Problem**: Some entities (`BarCodeResult.TryReset`, `PerformanceDataCommand.TryReset`) reinitialize fields and return `true`. Although multi-statement methods are not flagged today, future edits that temporarily reduce them to `return true;` (e.g., while scaffolding) immediately trigger EXXER501.  
- **Mitigation**: Treat methods with XML documentation mentioning “reset” or containing the word `Reset` in their name as opt-out candidates when they return a boolean literal, even if currently minimal.  
- **Test**:

```csharp
[Fact]
public void Should_NotReport_For_Reset_Method_ReturningTrue()
{
    const string testCode = @"
public sealed class ResettableComponent
{
    /// <summary>Resets the component state.</summary>
    public bool Reset()
    {
        return true;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer())
        .ShouldBeEmpty();
}
```

## 4. Test-Driven Fix Strategy

1. Add a new `UseExpressionBodiedMembersAnalyzerFalsePositiveTests` fixture with the 10 tests above plus a control test asserting the analyzer still reports on a trivial helper (`int GetValue() { return 42; }`).  
2. Update `UseExpressionBodiedMembersAnalyzer`:
   - Introduce semantic helper methods (`ImplementsInterface`, `IsCommandFactory`, `IsResetMethod`, `HasTodoComment`, etc.).  
   - Extend `CanBeExpressionBodied` to accept additional context (symbol, body trivia, naming heuristics).  
   - Apply early exits when any mitigation rule matches.  
3. Run analyzer tests locally; confirm new tests fail before changes (diagnostic reported) and pass afterwards.  
4. Build IndTrace solution and spot-check previously noisy files (`CreateBarCodeCommand.cs`, `CreateBarCodeCommandHandler.cs`, `McpServerBuilder.cs`) to ensure diagnostics disappear without suppressions.  
5. Document the behaviour change in release notes, emphasizing that EXXER501 now honours fluent factories, reset stubs, and TODO placeholders while still encouraging expression-bodied members for genuine one-line helpers.
