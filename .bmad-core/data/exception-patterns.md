# .NET Exception Patterns

## Exception Handling Fundamentals

### Basic Exception Handling
```csharp
public class ExceptionHandlingExample
{
    public async Task<Result<string>> ProcessDataAsync(string input)
    {
        try
        {
            var result = await ValidateAndProcessAsync(input);
            return Result<string>.Success(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for input: {Input}", input);
            return Result<string>.WithFailure($"Validation error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error occurred");
            return Result<string>.WithFailure("Network error occurred");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred");
            return Result<string>.WithFailure("An unexpected error occurred");
        }
    }
}
```

### Custom Exception Types
```csharp
// Domain-specific exceptions
public class BusinessRuleException : Exception
{
    public string RuleName { get; }
    public object Context { get; }
    
    public BusinessRuleException(string ruleName, string message, object context = null) 
        : base(message)
    {
        RuleName = ruleName;
        Context = context;
    }
}

// Validation exceptions
public class ValidationException : Exception
{
    public IReadOnlyList<ValidationError> Errors { get; }
    
    public ValidationException(IEnumerable<ValidationError> errors) 
        : base($"Validation failed with {errors.Count()} errors")
    {
        Errors = errors.ToList().AsReadOnly();
    }
}

// Infrastructure exceptions
public class ExternalServiceException : Exception
{
    public string ServiceName { get; }
    public int StatusCode { get; }
    
    public ExternalServiceException(string serviceName, int statusCode, string message) 
        : base(message)
    {
        ServiceName = serviceName;
        StatusCode = statusCode;
    }
}
```

## Exception Handling Patterns

### Result Pattern
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public string Error { get; }
    
    private Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
    
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> WithFailure(string error) => new(false, default, error);
    
    public static implicit operator Result<T>(T value) => Success(value);
}

// Usage with Result pattern
public async Task<Result<ProcessedData>> ProcessDataAsync(RawData data)
{
    try
    {
        var validated = await ValidateDataAsync(data);
        var processed = await TransformDataAsync(validated);
        return Result<ProcessedData>.Success(processed);
    }
    catch (ValidationException ex)
    {
        return Result<ProcessedData>.WithFailure($"Validation failed: {ex.Message}");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Processing failed");
        return Result<ProcessedData>.WithFailure("Processing failed");
    }
}
```

### Try-Catch with Specific Handling
```csharp
public class SpecificExceptionHandler
{
    public async Task<Result<T>> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation, int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var result = await operation();
                return Result<T>.Success(result);
            }
            catch (HttpRequestException ex) when (attempt < maxRetries)
            {
                _logger.LogWarning(ex, "Attempt {Attempt} failed, retrying...", attempt);
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                continue;
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Operation timed out on attempt {Attempt}", attempt);
                return Result<T>.WithFailure("Operation timed out");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access on attempt {Attempt}", attempt);
                return Result<T>.WithFailure("Unauthorized access");
            }
        }
        
        return Result<T>.WithFailure("Max retries exceeded");
    }
}
```

## Exception Logging Patterns

### Structured Exception Logging
```csharp
public class StructuredExceptionLogger
{
    private readonly ILogger<StructuredExceptionLogger> _logger;
    
    public async Task<Result<T>> ExecuteWithLoggingAsync<T>(
        Func<Task<T>> operation, string operationName)
    {
        using var activity = ActivitySource.StartActivity(operationName);
        activity?.SetTag("operation.name", operationName);
        
        try
        {
            var result = await operation();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return Result<T>.Success(result);
        }
        catch (BusinessRuleException ex)
        {
            _logger.LogWarning(ex, 
                "Business rule violation in {OperationName}: {RuleName} - {Message}", 
                operationName, ex.RuleName, ex.Message);
            
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            return Result<T>.WithFailure($"Business rule violation: {ex.Message}");
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, 
                "Validation failed in {OperationName} with {ErrorCount} errors", 
                operationName, ex.Errors.Count);
            
            activity?.SetStatus(ActivityStatusCode.Error, "Validation failed");
            return Result<T>.WithFailure("Validation failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Unexpected error in {OperationName}: {Message}", 
                operationName, ex.Message);
            
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            return Result<T>.WithFailure("An unexpected error occurred");
        }
    }
}
```

### Exception Context Enrichment
```csharp
public class ExceptionContextEnricher
{
    public static void EnrichException(Exception ex, IDictionary<string, object> context)
    {
        if (ex is BusinessRuleException businessEx)
        {
            context["exception.rule_name"] = businessEx.RuleName;
            context["exception.context"] = businessEx.Context;
        }
        else if (ex is ValidationException validationEx)
        {
            context["exception.validation_errors"] = validationEx.Errors.Select(e => new
            {
                e.PropertyName,
                e.ErrorMessage,
                e.AttemptedValue
            });
        }
        else if (ex is HttpRequestException httpEx)
        {
            context["exception.http_status"] = httpEx.Data["StatusCode"];
            context["exception.service_name"] = httpEx.Data["ServiceName"];
        }
        
        // Add common context
        context["exception.type"] = ex.GetType().Name;
        context["exception.message"] = ex.Message;
        context["exception.stack_trace"] = ex.StackTrace;
    }
}
```

## Global Exception Handling

### ASP.NET Core Exception Middleware
```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            ValidationException ex => new
            {
                StatusCode = 400,
                Message = "Validation failed",
                Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            },
            BusinessRuleException ex => new
            {
                StatusCode = 422,
                Message = ex.Message,
                RuleName = ex.RuleName
            },
            UnauthorizedAccessException => new
            {
                StatusCode = 401,
                Message = "Unauthorized access"
            },
            _ => new
            {
                StatusCode = 500,
                Message = "An unexpected error occurred"
            }
        };
        
        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

### WPF Exception Handling
```csharp
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Global exception handling
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        
        base.OnStartup(e);
    }
    
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "Unhandled exception in UI thread");
        
        var result = MessageBox.Show(
            "An unexpected error occurred. Would you like to continue?",
            "Error",
            MessageBoxButton.YesNo,
            MessageBoxImage.Error);
        
        e.Handled = result == MessageBoxResult.Yes;
    }
    
    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        _logger.LogError(e.ExceptionObject as Exception, "Unhandled exception in application domain");
        
        // Log the exception and optionally show a message
        MessageBox.Show("A critical error occurred. The application will now exit.", "Critical Error");
    }
}
```

## Exception Testing Patterns

### Exception Testing with NUnit
```csharp
[Test]
public void ValidateData_WithInvalidInput_ShouldThrowValidationException()
{
    // Arrange
    var validator = new DataValidator();
    var invalidData = new RawData { Value = null };
    
    // Act & Assert
    var ex = Assert.ThrowsAsync<ValidationException>(() => validator.ValidateAsync(invalidData));
    Assert.That(ex.Errors, Has.Count.EqualTo(1));
    Assert.That(ex.Errors[0].PropertyName, Is.EqualTo(nameof(RawData.Value)));
}

[Test]
public void ProcessData_WithBusinessRuleViolation_ShouldThrowBusinessRuleException()
{
    // Arrange
    var processor = new DataProcessor();
    var data = new RawData { Value = "invalid" };
    
    // Act & Assert
    var ex = Assert.ThrowsAsync<BusinessRuleException>(() => processor.ProcessAsync(data));
    Assert.That(ex.RuleName, Is.EqualTo("ValueFormatRule"));
}
```

### Exception Testing with xUnit
```csharp
[Fact]
public async Task ValidateData_WithInvalidInput_ShouldThrowValidationException()
{
    // Arrange
    var validator = new DataValidator();
    var invalidData = new RawData { Value = null };
    
    // Act & Assert
    var exception = await Assert.ThrowsAsync<ValidationException>(() => validator.ValidateAsync(invalidData));
    Assert.Single(exception.Errors);
    Assert.Equal(nameof(RawData.Value), exception.Errors[0].PropertyName);
}
```

## Performance and Exception Handling

### Exception Performance Considerations
```csharp
public class PerformanceAwareExceptionHandler
{
    // BAD: Exception for control flow
    public bool TryGetValue(string key, out string value)
    {
        try
        {
            value = _dictionary[key];
            return true;
        }
        catch (KeyNotFoundException)
        {
            value = null;
            return false; // This is expensive!
        }
    }
    
    // GOOD: Use TryGetValue pattern
    public bool TryGetValue(string key, out string value)
    {
        return _dictionary.TryGetValue(key, out value);
    }
    
    // GOOD: Use Result pattern for expected failures
    public Result<string> GetValue(string key)
    {
        if (_dictionary.TryGetValue(key, out var value))
        {
            return Result<string>.Success(value);
        }
        return Result<string>.WithFailure("Key not found");
    }
}
```

### Exception Monitoring
```csharp
public class ExceptionMonitor
{
    private readonly IMetrics _metrics;
    private readonly ILogger<ExceptionMonitor> _logger;
    
    public void RecordException(Exception ex, string operationName)
    {
        _metrics.Counter("exceptions_total", new[] { operationName, ex.GetType().Name }).Increment();
        
        if (ex is BusinessRuleException businessEx)
        {
            _metrics.Counter("business_rule_violations_total", new[] { businessEx.RuleName }).Increment();
        }
        
        _logger.LogError(ex, "Exception recorded for operation {OperationName}", operationName);
    }
}
```

## Best Practices

### Exception Handling Guidelines
1. **Use specific exceptions**: Catch specific exception types
2. **Don't catch and ignore**: Always handle or log exceptions
3. **Use Result pattern**: For expected failures
4. **Log with context**: Include relevant context in logs
5. **Don't use exceptions for control flow**: Use return values instead
6. **Create custom exceptions**: For domain-specific errors
7. **Handle exceptions at appropriate levels**: Don't catch too early or too late
8. **Use finally blocks**: For cleanup code
9. **Test exception scenarios**: Include exception tests
10. **Monitor exception rates**: Set up alerts for high exception rates

### Common Anti-Patterns
1. **Catching Exception**: Too broad exception handling
2. **Empty catch blocks**: Swallowing exceptions
3. **Throwing generic exceptions**: Use specific exception types
4. **Using exceptions for control flow**: Expensive and unclear
5. **Not logging exceptions**: Missing important information
6. **Catching and re-throwing**: Without adding context
7. **Not disposing resources**: In finally blocks
8. **Throwing from constructors**: Can cause issues
9. **Not handling async exceptions**: Missing await in catch blocks
10. **Not testing exception paths**: Missing test coverage
