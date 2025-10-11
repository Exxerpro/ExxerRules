# Analyzer Contract Alignment Plan

## AsyncMethodsShouldAcceptCancellationTokenAnalyzer

- **File/Test**: `TestCases/Async/AsyncMethodsShouldAcceptCancellationTokenFalsePositiveTests.cs:50` (`Should_Not_Report_For_Explicit_Interface_Implementation`)

  **Current snippet**
  ```csharp
  public interface IDataService
  {
      Task<string> GetDataAsync();
  }

  public class DataService : IDataService
  {
      async Task<string> IDataService.GetDataAsync()
      {
          await Task.Delay(100);
          return "data";
      }
  }
  ```

  **Suggested code**
  ```csharp
  public interface IDataService
  {
      Task<string> GetDataAsync(CancellationToken cancellationToken);
  }

  public class DataService : IDataService
  {
      async Task<string> IDataService.GetDataAsync(CancellationToken cancellationToken)
      {
          await Task.Delay(100, cancellationToken);
          return "data";
      }
  }
  ```

  **Rationale**: Explicit interface implementations in production code still need to expose and honor `CancellationToken`. Updating the false-positive story keeps the analyzer lenient only when tokens are correctly threaded, while `TestCases/EdgeCaseTests.cs:105` continues to verify that missing tokens are flagged.

## UseConfigureAwaitFalseAnalyzer

- **File/Test**: `TestCases/EdgeCaseTests.cs:221` (`Should_HandleNestedAwaitExpressions_ConfigureAwaitAnalyzer`)

  **Current snippet**
  ```csharp
  var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
  diagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
  ```

  **Suggested code**
  ```csharp
  var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
  diagnostics.ShouldBeEmpty();
  ```

- **File/Test**: `TestCases/AdvancedEdgeCaseTests.cs:238` (`Should_HandleDeeplyNestedExpressions_AllAnalyzers`)

  **Current snippet**
  ```csharp
  var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
  configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
  ```

  **Suggested code**
  ```csharp
  var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
  configureAwaitDiagnostics.ShouldBeEmpty();
  ```

  **Rationale**: `Task.FromResult` and `Task.Yield` provide no `ConfigureAwait` overload, and the guideline confines `ConfigureAwait(false)` to production library awaits. Aligning these edge-case assertions with `TestCases/Async/UseConfigureAwaitFalseFalsePositiveTests.cs:321` removes the contradictory expectation while leaving the positive-control coverage (e.g., `Task.Delay` scenarios) intact.

## AvoidThrowingExceptionsAnalyzer

- **File/Test**: `TestCases/ErrorHandlingTests.cs:139` (`Should_ReportDiagnostic_When_DifferentExceptionThrowingPatterns`)

  **Current snippet**
  ```csharp
  public void ProcessUser(string userName)
  {
      if (string.IsNullOrEmpty(userName))
      {
          throw new ArgumentException("User name cannot be null or empty");
      }

      if (userName.Length < 3)
      {
          throw new InvalidOperationException("User name too short");
      }

      if (userName.Length > 50)
      {
          throw new ArgumentOutOfRangeException("User name too long");
      }
  }
  ```

  **Suggested code**
  ```csharp
  public Result ProcessUser(string userName)
  {
      if (string.IsNullOrEmpty(userName))
      {
          return Result.WithFailure("User name cannot be null or empty");
      }

      if (userName.Length < 3)
      {
          return Result.WithFailure("User name too short");
      }

      if (userName.Length > 50)
      {
          return Result.WithFailure("User name too long");
      }

      return Result.Success();
  }
  ```

  **Rationale**: Guards are acceptable in traditional methods, but railway-oriented code should surface failures through `Result` rather than exceptions. Refactoring the test keeps the analyzer strict in Result-returning scenarios and avoids clashing with the guard exemption validated in `TestCases/AvoidThrowingExceptionsAnalyzerFalsePositiveTests.cs:38`.

## DoNotThrowExceptionsAnalyzer

- **File/Test**: `TestCases/FunctionalPatternsTests.cs:70` (`Should_ReportDiagnostic_When_ThrowingInsteadOfReturningResult`)

  **Current snippet**
  ```csharp
  public string ProcessData(string input)
  {
      if (string.IsNullOrEmpty(input))
          throw new ArgumentException("Input cannot be null");

      return input.ToUpper();
  }
  ```

  **Suggested code**
  ```csharp
  public Result<string> ProcessData(string input)
  {
      if (string.IsNullOrEmpty(input))
          throw new ArgumentException("Input cannot be null");

      return Result.Ok(input.ToUpper());
  }
  ```

  **Rationale**: To respect the “no throws in railway” rule while keeping guard throws legal in traditional methods, move the violation into a `Result<T>` pipeline. The analyzer should continue to flag the throw, and the guard exemption in `TestCases/DoNotThrowExceptionsAnalyzerFalsePositiveTests.cs:68` remains valid for non-Result members.

## Proposed Additional Coverage

- **New Test Suggestion**: add to `TestCases/Async/AsyncTests.cs`

  ```csharp
  [Fact]
  public void Should_Not_Report_When_ResultBased_Async_Propagates_Cancellation()
  {
      const string testCode = @"
using System.Threading;
using System.Threading.Tasks;
using IndFusion.Analyzers.Operations;
using Xunit.Sdk;

namespace TestProject
{
    public sealed class Service
    {
        public async Task<Result<string>> LoadAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Result.WithFailure(""Canceled"");
            }

            await Task.Delay(50, cancellationToken).ConfigureAwait(false);
            return Result.Ok(""data"");
        }
    }

    public static class Consumer
    {
        public static Task<Result<string>> ExecuteAsync()
            => new Service().LoadAsync(TestContext.Current.CancellationToken);
    }
}";

      var tokenDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
      tokenDiagnostics.ShouldBeEmpty();

      var configureDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
      configureDiagnostics.ShouldBeEmpty();
  }
  ```

  **Rationale**: Demonstrates the expected “railway” behaviour for async methods that return `Result<T>`—the token is honoured, cancellation is surfaced as `Result.WithFailure`, `ConfigureAwait(false)` is used because the code models a library scenario, and no analyzer should emit diagnostics when those requirements are met.

- **New Test Suggestion**: add to `TestCases/Async/AsyncTests.cs`

  ```csharp
  [Fact]
  public void Should_ReportDiagnostic_When_ResultBasedAsyncOmitsCancellationToken()
  {
      const string testCode = @"
using System.Threading.Tasks;
using IndFusion.Analyzers.Operations;

namespace TestProject
{
    public sealed class Service
    {
        public async Task<Result<string>> LoadAsync()
        {
            await Task.Delay(25).ConfigureAwait(false);
            return Result.Ok(""data"");
        }
    }
}";

      var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
      diagnostics.Select(d => d.Id).ShouldContain(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken);
  }
  ```

  **Rationale**: Guards against regressions where Task-returning railway code forgets to surface `CancellationToken`. The analyzer should treat Result-based methods the same as other asynchronous operations—if the token is missing, a diagnostic must fire.
