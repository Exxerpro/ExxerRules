# CodeFormatting Analyzer - False-Positive Mitigation Spec

**Analyzer ID**: `EXXER901`  
**Source**: `src/code/IndFusion.Analyzer/CodeFormatting/CodeFormattingAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-08)

## 0. Selection Rationale

- No spec existed for EXXER901 even though `AnalyzerReleases.Unshipped.md:19` ships it as an Info diagnostic alongside EXXER900.  
- While researching EXXER900 we observed the formatting heuristics shared by EXXER901 generate numerous false positives through simple string matching. Real examples appear in production code such as `Test Project/Src/Code/Core/Application/Registers/Services/RegisterInformationService.cs` (lines 24, 61, 82, 103, 118, 221) and `Test Project/Src/Code/Core/Application/BarCodes/Commands/Create/CreateBarCodeCommandHandler.cs` (lines 63, 95, 103, 183). Developers cannot act on these diagnostics; they are either already well-formatted or express intentional pipelines.  
- Because EXXER901 fires as Info, it clutters solution-error lists and IDE light bulbs, prompting teams to ignore EXXER900/901 instead of using the intended formatting flow.

## 1. Specification

- **Intent**  
  Highlight common formatting issues (spacing, brace placement, accessor style) so developers can invoke quick fixes or run `dotnet format` locally.

- **Scope**  
  Registers syntax node actions for `ClassDeclaration`, `MethodDeclaration`, `PropertyDeclaration`, and `VariableDeclaration`. Each handler inspects the node’s text via `ToString()`/`Contains` heuristics (e.g., looking for `" ="`, `"= "`, multi-line parameter lists, or missing blank lines). No semantic model checks or trivia inspection occurs.

## 2. Validation Plan

1. Introduce `CodeFormattingAnalyzerFalsePositiveTests` next to `ModernCSharpTests.cs`, carrying the ten scenarios below plus the existing positive coverage.  
2. Update `AnalyzerTestHelper.RunAnalyzer` to surface EXXER901 diagnostics explicitly for formatter assertions.  
3. Execute `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release` after each mitigation.  
4. Run `dotnet build "Test Project/Src/Code/Core/Application/Core.Application.csproj" -c Release` to verify EXXER901 no longer flags the cited files.

## 3. Enhancement Opportunities (≥10 Items)

Each entry lists the current false-positive, mitigation idea, and a regression snippet that fails today (EXXER901 produced) but should pass after the fix.

### 1. LINQ Projections Flagged as “Inconsistent Variable Formatting”

- **Problem**: `Test Project/Src/Code/Core/Application/Registers/Services/RegisterInformationService.cs:24` assigns `availableRegisters = availableRecords.Select(...)`. The analyzer flags the line because `Declaration.ToString()` contains `" ="`.  
- **Mitigation**: Use `VariableDeclarationSyntax` tokens (`EqualsToken.HasLeadingTrivia/HasTrailingTrivia`) to ensure actual spacing issues exist before reporting.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Linq_Projection_Assignments()
  {
      const string code = @"
using System.Linq;
namespace Samples;
public static class Target
{
    public static void Build()
    {
        var availableRegisters = Enumerable.Range(1, 3)
            .Select(i => new { Id = i, Name = $""PLC{i:D3}"" });
    }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new CodeFormattingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 2. Guard Clause Mock Data Assignments

- **Problem**: `RegisterInformationService.cs:61` assigns `var mockData = this.GenerateMockData(maxItems);` inside a debug guard. EXXER901 flags it despite perfect spacing.  
- **Mitigation**: Recognize single-expression assignments without embedded operators as compliant when `EqualsToken` already has surrounding trivia.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Debug_Guard_Assignments()
  {
      const string code = @"
namespace Samples;
public class Target
{
    public object Create(bool debug)
    {
        if (debug)
        {
            var mockData = GenerateMockData();
            return mockData;
        }

        return new object();
    }

    private static int[] GenerateMockData() => new[] { 1, 2, 3 };
}";
      AnalyzerTestHelper.RunAnalyzer(code, new CodeFormattingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 3. Dictionary Initializations in Result Pipelines

- **Problem**: `RegisterInformationService.cs:82` declares `var totalItems = new Dictionary<(int MachineId, string Name), List<TimeSeriesDataPoint>>();` which EXXER901 marks as inconsistent.  
- **Mitigation**: When the initializer is a single `ObjectCreationExpressionSyntax` and the equals token is surrounded by whitespace, skip diagnostics.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Dictionary_ObjectCreation()
  {
      const string code = @"
using System.Collections.Generic;
namespace Samples;
public class Target
{
    public Dictionary<(int, string), List<int>> Build()
    {
        var totalItems = new Dictionary<(int, string), List<int>>();
        return totalItems;
    }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new CodeFormattingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 4. Awaited Repository Calls

- **Problem**: `RegisterInformationService.cs:103` (`var registerResult = await registerRepository.ListAsync(...)`) is flagged because the analyzer’s string search sees `" = `".  
- **Mitigation**: Detect `await` expressions and ensure the equals token plus subsequent trivia already conforms before reporting.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Awaited_Assignments()
  {
      const string code = @"
using System.Threading.Tasks;
namespace Samples;
public class Target
{
    public async Task Execute(IRepository repo)
    {
        var registerResult = await repo.ListAsync();
    }
}
public interface IRepository { Task<int> ListAsync(); }";
      AnalyzerTestHelper.RunAnalyzer(code, new CodeFormattingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 5. Projection to DTOs

- **Problem**: `RegisterInformationService.cs:118` maps repository results to DTOs with `var registerDtos = registerResult.Value.Select(...)`. EXXER901 labels this as inconsistent spacing.  
- **Mitigation**: Ignore assignments whose initializer is an `InvocationExpressionSyntax` starting on the same line with correct trivia.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Select_Projection_Assignments()
  {
      const string code = @"
using System.Linq;
namespace Samples;
public class Target
{
    public void Map(int[] values)
    {
        var registerDtos = values.Select(v => new { Value = v });
    }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new CodeFormattingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 6. GroupBy/ToDictionary Pipelines

- **Problem**: `RegisterInformationService.cs:221-225` sets `var result = registerDtos.GroupBy(...).ToDictionary(...)`; EXXER901 flags each assignment within `MapToTimeSeries`.  
- **Mitigation**: Treat fluent chains as safe when each invocation already uses proper whitespace, especially when the initializer begins on a new indented line.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_GroupBy_ToDictionary_Chains()
  {
      const string code = @"
using System.Collections.Generic;
using System.Linq;
namespace Samples;
public static class Target
{
    public static Dictionary<(int, string), IEnumerable<int>> Map(IEnumerable<Entry> source)
    {
        var result = source
            .GroupBy(e => (e.Id, e.Name))
            .ToDictionary(group => group.Key, group => group.AsEnumerable());
        return result;
    }
}
public sealed record Entry(int Id, string Name);";
      AnalyzerTestHelper.RunAnalyzer(code, new CodeFormattingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 7. Fluent Result Pipelines

- **Problem**: `CreateBarCodeCommandHandler.cs:63` composes `var result = await Result.Success(cmd.Command)...`, but EXXER901 flags the initial assignment.  
- **Mitigation**: Detect chained invocations following an assignment (`InvocationExpressionSyntax` with `MemberBindingExpression`) and skip diagnostics when tokens already include whitespace.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Result_Pipelines()
  {
      const string code = @"
using System.Threading.Tasks;
namespace Samples;
public class Target
{
    public async Task<int> Execute()
    {
        var result = await Result.Success(5)
            .ThenAsync(Task.FromResult);
        return result;
    }
}
public static class Result
{
    public static Task<int> Success(int value) => Task.FromResult(value);
}";
      AnalyzerTestHelper.RunAnalyzer(code, new CodeFormattingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 8. Specification Builders (Machine)

- **Problem**: `CreateBarCodeCommandHandler.cs:95` defines `var spec = new Specification<Machine>(...)`; EXXER901 reports it despite idiomatic formatting.  
- **Mitigation**: Only flag assignments lacking spaces around the equals token. For generic `new` expressions with initializer spanning multiple lines, suppress.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Generic_Specification_Assignments()
  {
      const string code = @"
namespace Samples;
public static class Target
{
    public static void Build()
    {
        var spec = new Specification<int>(n => n > 0);
    }
}
public sealed class Specification<T>
{
    public Specification(System.Func<T, bool> predicate) { }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new CodeFormattingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 9. Specification Builders (Product)

- **Problem**: Identical noise occurs at `CreateBarCodeCommandHandler.cs:103` for product specifications.  
- **Mitigation**: Same as above; ensure analyzer pairs with trivia rather than string search.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Product_Specification_Assignments()
  {
      const string code = @"
namespace Samples;
public static class Target
{
    public static void Build()
    {
        var spec = new Specification<string>(part => part.StartsWith(""PN-""));
    }
}
public sealed class Specification<T>
{
    public Specification(System.Func<T, bool> predicate) { }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new CodeFormattingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 10. Dictionary Materialization from Variables Collection

- **Problem**: `CreateBarCodeCommandHandler.cs:183` calls `var references = variables.ToDictionary(...)`. The analyzer marks it despite consistent formatting.  
- **Mitigation**: Recognize `InvocationExpressionSyntax` returning dictionaries and skip when tokens already have surrounding whitespace.  
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_ToDictionary_Assignments()
  {
      const string code = @"
using System.Collections.Generic;
using System.Linq;
namespace Samples;
public static class Target
{
    public static Dictionary<string, int> Build(IEnumerable<Item> variables)
    {
        var references = variables.ToDictionary(v => v.Name, v => v.Value);
        return references;
    }
}
public sealed record Item(string Name, int Value);";
      AnalyzerTestHelper.RunAnalyzer(code, new CodeFormattingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

## 4. Test-Driven Fix Strategy

- Extend `CodeFormattingAnalyzer` to rely on syntax tokens and trivia (e.g., `EqualsToken.TrailingTrivia.Any(SyntaxKind.WhitespaceTrivia)`) rather than raw `string.Contains`.  
- Specifically guard against cases where the initializer is an `InvocationExpressionSyntax`, `ObjectCreationExpressionSyntax`, or `AwaitExpressionSyntax` with proper trivia.  
- Add helper utilities in the analyzer to check for real spacing differences (e.g., missing spaces around binary operators) using `SyntaxToken.GetPreviousToken()` comparisons.  
- Update analyzer tests with the ten regression scenarios plus existing positive cases.  
- After the refactor, run `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release` and rebuild representative projects to ensure EXXER901 only flags genuine formatting defects.
