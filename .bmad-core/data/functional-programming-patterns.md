# Functional Programming Patterns in .NET

## Immutability and Immutable Data Structures

### Immutable Records
```csharp
// Immutable record with validation
public record Person(string FirstName, string LastName, int Age)
{
    public string FullName => $"{FirstName} {LastName}";
    
    public bool IsAdult => Age >= 18;
    
    public Person WithAge(int newAge) => this with { Age = newAge };
}

// Usage
var person = new Person("John", "Doe", 25);
var olderPerson = person.WithAge(26);
```

### Immutable Collections
```csharp
// Using System.Collections.Immutable
public class UserService
{
    private readonly ImmutableList<User> _users;
    
    public UserService()
    {
        _users = ImmutableList<User>.Empty;
    }
    
    public UserService AddUser(User user)
    {
        return new UserService(_users.Add(user));
    }
    
    public UserService RemoveUser(int userId)
    {
        var userToRemove = _users.FirstOrDefault(u => u.Id == userId);
        return userToRemove != null 
            ? new UserService(_users.Remove(userToRemove))
            : this;
    }
}
```

## Pure Functions

### Function Purity
```csharp
// Pure function - no side effects, deterministic
public static decimal CalculateTax(decimal amount, decimal taxRate)
{
    return amount * taxRate;
}

// Pure function with validation
public static Result<decimal> CalculateTaxSafe(decimal amount, decimal taxRate)
{
    if (amount < 0)
        return Result<decimal>.WithFailure("Amount cannot be negative");
    
    if (taxRate < 0 || taxRate > 1)
        return Result<decimal>.WithFailure("Tax rate must be between 0 and 1");
    
    return Result<decimal>.Success(amount * taxRate);
}

// Impure function - has side effects
public void LogMessage(string message)
{
    Console.WriteLine($"{DateTime.Now}: {message}"); // Side effect: console output
}
```

### Function Composition
```csharp
// Function composition
public static class FunctionComposition
{
    public static Func<T, R> Compose<T, U, R>(Func<T, U> f, Func<U, R> g)
    {
        return x => g(f(x));
    }
    
    // Usage
    public static string ProcessName(string name)
    {
        Func<string, string> trim = s => s.Trim();
        Func<string, string> toUpper = s => s.ToUpperInvariant();
        Func<string, string> addPrefix = s => $"Mr. {s}";
        
        var processName = Compose(trim, Compose(toUpper, addPrefix));
        return processName(name);
    }
}
```

## Higher-Order Functions

### Function as Parameters
```csharp
public class DataProcessor<T>
{
    public IEnumerable<T> Process(IEnumerable<T> data, Func<T, bool> predicate, Func<T, T> transform)
    {
        return data
            .Where(predicate)
            .Select(transform);
    }
    
    public async Task<IEnumerable<T>> ProcessAsync(
        IEnumerable<T> data, 
        Func<T, Task<bool>> predicate, 
        Func<T, Task<T>> transform)
    {
        var results = new List<T>();
        
        foreach (var item in data)
        {
            if (await predicate(item))
            {
                var transformed = await transform(item);
                results.Add(transformed);
            }
        }
        
        return results;
    }
}
```

### Function as Return Values
```csharp
public class ValidationBuilder<T>
{
    private readonly List<Func<T, Result>> _validators = new();
    
    public ValidationBuilder<T> AddValidator(Func<T, Result> validator)
    {
        _validators.Add(validator);
        return this;
    }
    
    public Func<T, Result> Build()
    {
        return item => _validators
            .Select(validator => validator(item))
            .FirstOrDefault(result => result.IsFailure) ?? Result.Success();
    }
}

// Usage
var userValidator = new ValidationBuilder<User>()
    .AddValidator(user => string.IsNullOrWhiteSpace(user.Name) 
        ? Result.WithFailure("Name is required") 
        : Result.Success())
    .AddValidator(user => user.Age < 0 
        ? Result.WithFailure("Age cannot be negative") 
        : Result.Success())
    .Build();
```

## Option and Result Patterns

### Option Pattern
```csharp
public readonly struct Option<T>
{
    private readonly T _value;
    private readonly bool _hasValue;
    
    private Option(T value, bool hasValue)
    {
        _value = value;
        _hasValue = hasValue;
    }
    
    public static Option<T> Some(T value) => new(value, true);
    public static Option<T> None => new(default, false);
    
    public bool IsSome => _hasValue;
    public bool IsNone => !_hasValue;
    
    public T Value => _hasValue ? _value : throw new InvalidOperationException("Option has no value");
    
    public Option<U> Map<U>(Func<T, U> mapper)
    {
        return _hasValue ? Option<U>.Some(mapper(_value)) : Option<U>.None;
    }
    
    public Option<U> Bind<U>(Func<T, Option<U>> binder)
    {
        return _hasValue ? binder(_value) : Option<U>.None;
    }
    
    public T GetValueOrDefault(T defaultValue)
    {
        return _hasValue ? _value : defaultValue;
    }
}

// Usage
public Option<User> FindUser(int id)
{
    var user = _userRepository.GetById(id);
    return user != null ? Option<User>.Some(user) : Option<User>.None;
}
```

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
    
    public Result<U> Map<U>(Func<T, U> mapper)
    {
        return IsSuccess ? Result<U>.Success(mapper(Value)) : Result<U>.WithFailure(Error);
    }
    
    public Result<U> Bind<U>(Func<T, Result<U>> binder)
    {
        return IsSuccess ? binder(Value) : Result<U>.WithFailure(Error);
    }
    
    public T GetValueOrDefault(T defaultValue)
    {
        return IsSuccess ? Value : defaultValue;
    }
}
```

## Monadic Operations

### Maybe Monad
```csharp
public static class MaybeExtensions
{
    public static Option<T> ToOption<T>(this T? value) where T : class
    {
        return value != null ? Option<T>.Some(value) : Option<T>.None;
    }
    
    public static Option<T> ToOption<T>(this T? value) where T : struct
    {
        return value.HasValue ? Option<T>.Some(value.Value) : Option<T>.None;
    }
    
    public static Option<U> Select<T, U>(this Option<T> option, Func<T, U> selector)
    {
        return option.Map(selector);
    }
    
    public static Option<U> SelectMany<T, U>(this Option<T> option, Func<T, Option<U>> selector)
    {
        return option.Bind(selector);
    }
}

// Usage with LINQ
var result = from user in FindUser(1).ToOption()
             from profile in GetUserProfile(user.Id).ToOption()
             select new { user.Name, profile.Avatar };
```

### Either Monad
```csharp
public readonly struct Either<TLeft, TRight>
{
    private readonly TLeft _left;
    private readonly TRight _right;
    private readonly bool _isRight;
    
    private Either(TLeft left, TRight right, bool isRight)
    {
        _left = left;
        _right = right;
        _isRight = isRight;
    }
    
    public static Either<TLeft, TRight> Left(TLeft left) => new(left, default, false);
    public static Either<TLeft, TRight> Right(TRight right) => new(default, right, true);
    
    public bool IsLeft => !_isRight;
    public bool IsRight => _isRight;
    
    public TLeft LeftValue => _isRight ? throw new InvalidOperationException("Either is Right") : _left;
    public TRight RightValue => _isRight ? _right : throw new InvalidOperationException("Either is Left");
    
    public Either<TLeft, U> Map<U>(Func<TRight, U> mapper)
    {
        return _isRight ? Either<TLeft, U>.Right(mapper(_right)) : Either<TLeft, U>.Left(_left);
    }
    
    public Either<TLeft, U> Bind<U>(Func<TRight, Either<TLeft, U>> binder)
    {
        return _isRight ? binder(_right) : Either<TLeft, U>.Left(_left);
    }
}
```

## Functional Error Handling

### Railway-Oriented Programming
```csharp
public static class RailwayExtensions
{
    public static Result<T> Bind<T>(this Result<T> result, Func<T, Result<T>> func)
    {
        return result.IsSuccess ? func(result.Value) : result;
    }
    
    public static Result<U> Map<T, U>(this Result<T> result, Func<T, U> func)
    {
        return result.IsSuccess ? Result<U>.Success(func(result.Value)) : Result<U>.WithFailure(result.Error);
    }
    
    public static Result<T> Tee<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }
        return result;
    }
}

// Usage
public async Task<Result<Order>> ProcessOrderAsync(OrderRequest request)
{
    return await ValidateOrderRequest(request)
        .Bind(CalculateOrderTotal)
        .Bind(ReserveInventory)
        .Bind(ProcessPayment)
        .Tee(order => _logger.LogInformation("Order {OrderId} processed successfully", order.Id));
}
```

## Immutable Collections and Operations

### Functional List Operations
```csharp
public static class FunctionalListExtensions
{
    public static ImmutableList<T> Add<T>(this ImmutableList<T> list, T item)
    {
        return list.Add(item);
    }
    
    public static ImmutableList<T> Remove<T>(this ImmutableList<T> list, T item)
    {
        return list.Remove(item);
    }
    
    public static ImmutableList<U> Map<T, U>(this ImmutableList<T> list, Func<T, U> mapper)
    {
        return list.Select(mapper).ToImmutableList();
    }
    
    public static ImmutableList<T> Filter<T>(this ImmutableList<T> list, Func<T, bool> predicate)
    {
        return list.Where(predicate).ToImmutableList();
    }
    
    public static TAccumulate Fold<T, TAccumulate>(
        this ImmutableList<T> list, 
        TAccumulate seed, 
        Func<TAccumulate, T, TAccumulate> folder)
    {
        return list.Aggregate(seed, folder);
    }
}
```

## Async Functional Patterns

### Async Option and Result
```csharp
public static class AsyncOptionExtensions
{
    public static async Task<Option<U>> MapAsync<T, U>(this Option<T> option, Func<T, Task<U>> mapper)
    {
        return option.IsSome ? Option<U>.Some(await mapper(option.Value)) : Option<U>.None;
    }
    
    public static async Task<Option<U>> BindAsync<T, U>(this Option<T> option, Func<T, Task<Option<U>>> binder)
    {
        return option.IsSome ? await binder(option.Value) : Option<U>.None;
    }
}

public static class AsyncResultExtensions
{
    public static async Task<Result<U>> MapAsync<T, U>(this Result<T> result, Func<T, Task<U>> mapper)
    {
        return result.IsSuccess 
            ? Result<U>.Success(await mapper(result.Value)) 
            : Result<U>.WithFailure(result.Error);
    }
    
    public static async Task<Result<U>> BindAsync<T, U>(this Result<T> result, Func<T, Task<Result<U>>> binder)
    {
        return result.IsSuccess ? await binder(result.Value) : Result<U>.WithFailure(result.Error);
    }
}
```

## Functional Composition and Pipelines

### Pipeline Pattern
```csharp
public static class PipelineExtensions
{
    public static T Pipe<T>(this T input, Func<T, T> func)
    {
        return func(input);
    }
    
    public static async Task<T> PipeAsync<T>(this T input, Func<T, Task<T>> func)
    {
        return await func(input);
    }
    
    public static Result<T> Pipe<T>(this T input, Func<T, Result<T>> func)
    {
        return func(input);
    }
    
    public static async Task<Result<T>> PipeAsync<T>(this T input, Func<T, Task<Result<T>>> func)
    {
        return await func(input);
    }
}

// Usage
public async Task<Result<Order>> ProcessOrder(OrderRequest request)
{
    return await request
        .Pipe(ValidateOrderRequest)
        .PipeAsync(CalculateOrderTotal)
        .PipeAsync(ReserveInventory)
        .PipeAsync(ProcessPayment)
        .PipeAsync(CreateOrder);
}
```

## Best Practices for Functional Programming in .NET

### 1. Prefer Immutability
- Use `readonly` fields and properties
- Use immutable collections (`ImmutableList`, `ImmutableDictionary`, etc.)
- Use records for data structures
- Avoid mutable state when possible

### 2. Write Pure Functions
- Functions should not have side effects
- Functions should be deterministic
- Functions should be testable in isolation
- Separate pure logic from impure operations

### 3. Use Function Composition
- Break complex operations into smaller functions
- Compose functions to build complex behavior
- Use higher-order functions for reusability
- Apply the single responsibility principle

### 4. Handle Errors Functionally
- Use `Result<T>` pattern for expected failures
- Use `Option<T>` for nullable values
- Use `Either<TLeft, TRight>` for multiple error types
- Avoid throwing exceptions for control flow

### 5. Leverage LINQ for Functional Operations
- Use `Select`, `Where`, `Aggregate` for transformations
- Use `GroupBy`, `OrderBy` for data manipulation
- Use `Take`, `Skip` for pagination
- Use `Any`, `All` for validation

### 6. Use Async Functional Patterns
- Compose async operations functionally
- Use `Task` as a monad for async operations
- Handle async errors functionally
- Use async pipelines for complex workflows

### 7. Apply Functional Design Patterns
- Use the Strategy pattern with functions
- Use the Command pattern with function composition
- Use the Observer pattern with functional callbacks
- Use the Factory pattern with function factories
