# IndQuestResult Exception Support Port Plan

## Overview

This document outlines the changes needed to port exception support (`IsFaulted` and `Exception?` properties) from the minimal `IndFusion.Analyzer.Operations.Result` implementation to the main `IndQuestResult` library.

## Background

- **Current Implementation**: `IndFusion.Analyzer.Operations.Result` is a minimal implementation for .NET Core 2.0 support
- **Target Implementation**: `IndQuestResult` library (in separate repo) is much richer and feature-complete
- **Goal**: Port exception support to the main library and update the NuGet package

## Changes to Port

### 1. Add Exception Properties to Result Classes

#### For `Result` (non-generic):
```csharp
/// <summary>
/// Gets a value indicating whether the result was created from an exception (excluding cancellation).
/// </summary>
public bool IsFaulted { get; }

/// <summary>
/// Gets the exception that caused the failure, if any.
/// Contains the full stack trace for non-cancelled exceptions.
/// </summary>
public Exception? Exception { get; }
```

#### For `Result<T>` (generic):
```csharp
/// <summary>
/// Gets a value indicating whether the result was created from an exception (excluding cancellation).
/// </summary>
public bool IsFaulted { get; }

/// <summary>
/// Gets the exception that caused the failure, if any.
/// Contains the full stack trace for non-cancelled exceptions.
/// </summary>
public Exception? Exception { get; }
```

### 2. Update Constructors

#### For `Result`:
- Add optional `Exception? exception = null` parameter to private constructor
- Initialize `Exception` property
- Initialize `IsFaulted` property: `IsFaulted = exception is not null && exception is not OperationCanceledException`

#### For `Result<T>`:
- Add optional `Exception? exception = null` parameter to all constructors
- Initialize `Exception` property
- Initialize `IsFaulted` property: `IsFaulted = exception is not null && exception is not OperationCanceledException`

### 3. Update Factory Methods

#### For `Result`:
- Update `WithFailure(IEnumerable<string> errors)` → `WithFailure(IEnumerable<string> errors, Exception? exception = null)`
- Update `WithFailure(string[] errors)` → `WithFailure(string[] errors, Exception? exception = null)`
- Update `WithFailure(string error)` → `WithFailure(string error, Exception? exception = null)`
- Add new `WithFailure(Exception exception)` overload

#### For `Result<T>`:
- Update all `WithFailure` overloads to accept optional `Exception? exception = null` parameter
- Add new `WithFailure(Exception exception, T? value = default)` overload

### 4. Replace Validation Throws with Result Returns

#### In `ResultAsync.cs` and all async extension methods:
- Replace `ArgumentNullException.ThrowIfNull(param)` with:
  ```csharp
  if (param is null)
  {
      return Result<T>.WithFailure("Parameter cannot be null");
  }
  ```

#### In all factory methods and constructors:
- Replace `throw new ArgumentException(...)` with:
  ```csharp
  return Result<T>.WithFailure("Error message");
  ```

### 5. Update Exception Handling in Async Methods

#### In all `ResultAsync` methods:
- Update catch blocks to pass exception to `WithFailure`:
  ```csharp
  catch (Exception ex)
  {
      return Result<T>.WithFailure($"Operation failed: {ex.Message}", default, ex);
  }
  ```

### 6. Update ResultAsync.cs

- Replace all `ArgumentNullException.ThrowIfNull` calls with null checks returning `Result<T>.WithFailure`
- Update all catch blocks to use `Result<T>.WithFailure(message, default, ex)` to preserve exceptions

## Implementation Checklist

- [ ] Add `IsFaulted` and `Exception?` properties to `Result` class
- [ ] Add `IsFaulted` and `Exception?` properties to `Result<T>` class
- [ ] Update `Result` constructors to accept `Exception?` parameter
- [ ] Update `Result<T>` constructors to accept `Exception?` parameter
- [ ] Update all `Result.WithFailure` methods to accept `Exception?` parameter
- [ ] Update all `Result<T>.WithFailure` methods to accept `Exception?` parameter
- [ ] Add `Result.WithFailure(Exception)` overload
- [ ] Add `Result<T>.WithFailure(Exception, T?)` overload
- [ ] Replace `ArgumentNullException.ThrowIfNull` with null checks in `ResultAsync`
- [ ] Update all catch blocks in `ResultAsync` to pass exceptions to `WithFailure`
- [ ] Update any other validation throws to return `Result<T>.WithFailure`
- [ ] Add unit tests for `IsFaulted` property
- [ ] Add unit tests for `Exception` property with stack trace
- [ ] Verify exception stack traces are preserved
- [ ] Update NuGet package version
- [ ] Update package documentation

## Testing Requirements

### Unit Tests Needed:
1. **IsFaulted Property Tests**:
   - `IsFaulted_ShouldBeTrue_WhenCreatedFromException`
   - `IsFaulted_ShouldBeFalse_WhenCreatedFromOperationCanceledException`
   - `IsFaulted_ShouldBeFalse_WhenCreatedFromValidationFailure`

2. **Exception Property Tests**:
   - `Exception_ShouldContainFullStackTrace_WhenCreatedFromException`
   - `Exception_ShouldBeNull_WhenCreatedFromValidationFailure`
   - `Exception_ShouldBeNull_WhenCreatedFromOperationCanceledException`

3. **WithFailure(Exception) Overload Tests**:
   - `WithFailure_Exception_ShouldSetIsFaultedTrue`
   - `WithFailure_Exception_ShouldPreserveStackTrace`
   - `WithFailure_OperationCanceledException_ShouldNotSetIsFaulted`

## Migration Notes

- The changes are **backward compatible** - all existing code will continue to work
- The `Exception?` parameter is optional in all methods
- Existing `WithFailure` calls without exception parameter will work as before
- New code can optionally pass exceptions to preserve stack traces

## Reference Implementation

See the changes in:
- `ExxerRules/src/code/Analyzer/IndFusion.Analyzer/Operations/Result.cs`
- `ExxerRules/ResultAsync.cs` (if it still exists in the repo)

These files contain the complete implementation that needs to be ported to `IndQuestResult`.

## Key Implementation Details

### IsFaulted Logic
```csharp
IsFaulted = exception is not null && exception is not OperationCanceledException
```

This ensures:
- Cancellation exceptions don't set `IsFaulted = true` (they use `IsCancelled()` instead)
- Only actual faults (non-cancellation exceptions) set `IsFaulted = true`
- Validation failures (no exception) have `IsFaulted = false`

### Exception Preservation
When creating a failure from an exception:
```csharp
public static Result<T> WithFailure(Exception exception, T? value = default)
{
    if (exception is null)
    {
        return new Result<T>(false, [ResultConstants.DefaultErrorMessage], value, null);
    }

    var errorMessage = exception is OperationCanceledException
        ? ResultErrors.OperationCancelled
        : $"{exception.GetType().Name}: {exception.Message}";

    return new Result<T>(false, [errorMessage], value, exception);
}
```

This preserves:
- Full exception object with stack trace
- Exception type information
- Inner exceptions
- All exception properties

