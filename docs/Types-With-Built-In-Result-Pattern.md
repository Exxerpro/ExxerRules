# Types with Built-in Result Pattern

## Overview

This document lists types that have built-in Result pattern properties (`Success`/`IsSuccess` + `ErrorMessage`) and do NOT need a `Result<T>` wrapper. These types are returned directly from services, not wrapped in `Result<T>`.

## Important Note

**These types are NOT standardized** - they use different patterns:
- Some use `Success` (bool) + `ErrorMessage` (string?)
- Some use `IsSuccess` (computed property) + `ErrorMessage` (string?)
- Some use `Status` (enum) + `ErrorMessage` (string?)

## Types with Built-in Result Pattern

### CLI Types

1. **RefactoringResult** (`IndFusion.Tools.Cli.Core/Services/RefactoringResult.cs`)
   - Properties: `IsSuccess` (computed from `ErrorMessage`), `ErrorMessage`
   - Pattern: `IsSuccess => string.IsNullOrEmpty(ErrorMessage) && (!string.IsNullOrEmpty(Summary) || !string.IsNullOrEmpty(Preview))`

2. **AnalysisResult** (`IndFusion.Tools.Cli.Core/Services/AnalysisResult.cs`)
   - Properties: `isSuccess` (computed from `ErrorMessage`), `ErrorMessage`
   - Pattern: `isSuccess => string.IsNullOrEmpty(ErrorMessage) && Output is not null`

3. **SessionResult** (`IndFusion.Tools.Cli.Core/Commands/SessionResult.cs`)
   - Properties: `IsSuccess` (computed from `ErrorMessage`), `ErrorMessage`
   - Pattern: `IsSuccess => string.IsNullOrEmpty(ErrorMessage) && !string.IsNullOrEmpty(SuccessMessage)`

### SemanticRag Domain Types

4. **VectorSearchResponse** (`IndFusion.SemanticRag.Domain/Models/VectorSearchResponse.cs`)
   - Properties: `Success` (bool), `ErrorMessage` (string?), `IsSuccess` (computed)
   - Pattern: `IsSuccess => Success && ErrorMessage is null`

5. **GraphQueryResult** (`IndFusion.SemanticRag.Domain/Models/GraphQueryResult.cs`)
   - Properties: `Success` (bool), `ErrorMessage` (string?), `IsSuccess` (computed)
   - Pattern: `IsSuccess => Success && ErrorMessage is null`

6. **GraphQuery** (`IndFusion.SemanticRag.Domain/Models/GraphQuery.cs`)
   - Properties: `Success` (bool), `ErrorMessage` (string?), `IsSuccess` (computed)
   - Pattern: `IsSuccess => Success && ErrorMessage is null`

7. **VectorEmbedding** (`IndFusion.SemanticRag.Domain/Models/VectorEmbedding.cs`)
   - Properties: `Success` (bool), `ErrorMessage` (string?), `IsSuccess` (computed)
   - Pattern: `IsSuccess => Success && ErrorMessage is null`

8. **DocumentProcessingResult** (`IndFusion.SemanticRag.Domain/Models/DocumentProcessingResult.cs`)
   - Properties: `Status` (enum: `ProcessingStatus`), `ErrorMessage` (string?)
   - Pattern: Uses `Status` enum instead of `Success` bool
   - Note: Not standardized - uses `Status` enum instead of `Success`/`IsSuccess`

## Test Patterns

### For Result<T> Types
```csharp
result.IsSuccess.ShouldBeTrue();
result.Value.ShouldNotBeNull();
// OR
result.ShouldSucceed();
```

### For Types with Success/IsSuccess + ErrorMessage
```csharp
result.Success.ShouldBeTrue(); // or result.IsSuccess.ShouldBeTrue();
result.ErrorMessage.ShouldBeNull();
```

### For DocumentProcessingResult (Status enum)
```csharp
result.Status.ShouldBe(ProcessingStatus.Success);
result.ErrorMessage.ShouldBeNull();
// For failures:
result.Status.ShouldBe(ProcessingStatus.Failed);
result.ErrorMessage.ShouldNotBeNullOrEmpty();
```

## Cancellation Handling

### Result<T> cancellation
```csharp
result.IsFailure.ShouldBeTrue();
result.Error.ShouldContain(ErrorCodes.OperationCancelled);
// OR
result.ShouldBeCancelled(); // extension method
```

### Types with built-in Result pattern cancellation
```csharp
result.Success.ShouldBeFalse(); // or result.IsSuccess.ShouldBeFalse();
result.ErrorMessage.ShouldNotBeNullOrEmpty();
```

### DocumentProcessingResult cancellation
```csharp
result.Status.ShouldBe(ProcessingStatus.Failed);
result.ErrorMessage.ShouldNotBeNullOrEmpty();
```

## Migration Notes

When refactoring tests that incorrectly assume `Result<T>` wrapper:
1. Check the return type of the method/service
2. If it's one of the types listed above, use the appropriate test pattern
3. Do NOT wrap in `Result<T>` - these types already have their own Result pattern

