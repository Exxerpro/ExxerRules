# Enhanced Specification Pattern

This repository contains an enhanced implementation of the Specification pattern for .NET, providing a powerful and flexible way to build complex queries with filtering, sorting, paging, includes, and projections.

## Features

### ✅ Core Features
- **Filtering**: Define complex criteria using expressions
- **Sorting**: Primary and secondary ordering (ascending/descending)
- **Paging**: Skip and take for pagination
- **Includes**: Load related entities (expression-based and string-based)
- **No-Tracking**: Optimize read-only operations
- **Caching**: Unique key generation for caching

### 🆕 Enhanced Features
- **String-based Includes**: Include related entities using string paths
- **Secondary Ordering**: ThenBy and ThenByDescending support
- **Predicate Composition**: AND/OR logic for combining specifications
- **Projection Support**: Transform query results using Select expressions

## Quick Start

### Basic Usage

```csharp
// Simple filtering
var activeProductsSpec = new Specification<Product>(p => p.IsActive);

// With includes
var productsWithCategorySpec = new Specification<Product>(p => p.IsActive)
    .AddInclude(p => p.Category)
    .AddInclude("Orders"); // String-based include

// With ordering
var orderedProductsSpec = new Specification<Product>(p => p.IsActive)
    .AddOrderBy(p => p.Category)
    .AddThenBy(p => p.Name)
    .AddInclude(p => p.Category);
```

### Advanced Usage

```csharp
// Complex specification with multiple features
var complexSpec = new Specification<Product>(p => p.IsActive && p.Price > 50)
    .AddInclude("CategoryNavigation")
    .AddInclude("Orders")
    .AddOrderBy(p => p.Category)
    .AddThenBy(p => p.Name)
    .ApplyPaging(0, 20)
    .ApplyNoTracking();

// Predicate composition
var activeSpec = new Specification<Product>(p => p.IsActive);
var expensiveSpec = new Specification<Product>(p => p.Price > 100);

var activeAndExpensive = activeSpec.And(expensiveSpec);
var activeOrExpensive = activeSpec.Or(expensiveSpec);

// Projection
var productNamesSpec = new Specification<Product>(p => p.IsActive)
    .AddSelect(p => new { p.Id, p.Name, p.Category })
    .AddOrderBy(p => p.Name);
```

## API Reference

### ISpecification<T>

#### Properties
- `Criteria`: The main filtering expression
- `Includes`: List of expression-based includes
- `IncludeStrings`: List of string-based includes
- `OrderBy`: Primary ascending order expression
- `OrderByDescending`: Primary descending order expression
- `ThenBy`: Secondary ascending order expression
- `ThenByDescending`: Secondary descending order expression
- `Select`: Projection expression
- `Key`: Unique cache key
- `Skip`: Number of items to skip
- `Take`: Number of items to take
- `IsTracking`: Whether to track entities

#### Methods
- `AddInclude(Expression<Func<T, object>>)`: Add expression-based include
- `AddInclude(string)`: Add string-based include
- `AddOrderBy(Expression<Func<T, object>>)`: Add primary ascending order
- `AddOrderByDescending(Expression<Func<T, object>>)`: Add primary descending order
- `AddThenBy(Expression<Func<T, object>>)`: Add secondary ascending order
- `AddThenByDescending(Expression<Func<T, object>>)`: Add secondary descending order
- `ApplyPaging(int skip, int take)`: Apply pagination
- `ApplyNoTracking()`: Disable entity tracking
- `And(ISpecification<T>)`: Combine with AND logic
- `Or(ISpecification<T>)`: Combine with OR logic
- `AddSelect(Expression<Func<T, object>>)`: Add projection

## Examples

### 1. Basic Filtering and Includes

```csharp
public class ProductService
{
    public async Task<List<Product>> GetActiveProductsWithCategoryAsync()
    {
        var spec = new Specification<Product>(p => p.IsActive)
            .AddInclude(p => p.Category)
            .AddOrderBy(p => p.Name);

        return await _repository.GetAsync(spec);
    }
}
```

### 2. Complex Filtering with Composition

```csharp
public async Task<List<Product>> GetFilteredProductsAsync(
    bool? isActive,
    decimal? minPrice,
    string? category)
{
    var spec = new Specification<Product>(p => true);

    if (isActive.HasValue)
    {
        var activeSpec = new Specification<Product>(p => p.IsActive == isActive.Value);
        spec = spec.And(activeSpec);
    }

    if (minPrice.HasValue)
    {
        var priceSpec = new Specification<Product>(p => p.Price >= minPrice.Value);
        spec = spec.And(priceSpec);
    }

    if (!string.IsNullOrEmpty(category))
    {
        var categorySpec = new Specification<Product>(p => p.Category == category);
        spec = spec.And(categorySpec);
    }

    return spec
        .AddInclude(p => p.Category)
        .AddOrderBy(p => p.Name)
        .ApplyPaging(0, 50);
}
```

### 3. Projection and Paging

```csharp
public async Task<List<ProductSummary>> GetProductSummariesAsync(int page, int pageSize)
{
    var spec = new Specification<Product>(p => p.IsActive)
        .AddSelect(p => new ProductSummary
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryName = p.Category.Name
        })
        .AddOrderBy(p => p.Name)
        .ApplyPaging((page - 1) * pageSize, pageSize);

    return await _repository.GetAsync(spec);
}
```

### 4. String-based Includes for Complex Relationships

```csharp
public async Task<List<Order>> GetOrdersWithAllDetailsAsync()
{
    var spec = new Specification<Order>(o => o.OrderDate >= DateTime.Today.AddDays(-30))
        .AddInclude("Customer")
        .AddInclude("Customer.Address")
        .AddInclude("OrderItems")
        .AddInclude("OrderItems.Product")
        .AddInclude("OrderItems.Product.Category")
        .AddOrderByDescending(o => o.OrderDate)
        .AddThenBy(o => o.Customer.Name);

    return await _repository.GetAsync(spec);
}
```

### 5. Read-only Operations with No Tracking

```csharp
public async Task<List<Product>> GetProductsForReportingAsync()
{
    var spec = new Specification<Product>(p => p.IsActive)
        .AddInclude(p => p.Category)
        .AddInclude(p => p.Orders)
        .AddOrderBy(p => p.Category.Name)
        .AddThenBy(p => p.Name)
        .ApplyNoTracking(); // Optimize for read-only operations

    return await _repository.GetAsync(spec);
}
```

## Best Practices

### 1. Reusable Specifications
Create reusable specification classes for common patterns:

```csharp
public static class ProductSpecifications
{
    public static ISpecification<Product> Active() =>
        new Specification<Product>(p => p.IsActive);

    public static ISpecification<Product> InCategory(string category) =>
        new Specification<Product>(p => p.Category == category);

    public static ISpecification<Product> WithPriceRange(decimal min, decimal max) =>
        new Specification<Product>(p => p.Price >= min && p.Price <= max);

    public static ISpecification<Product> WithCategoryInclude() =>
        new Specification<Product>(p => true)
            .AddInclude(p => p.Category);
}
```

### 2. Composition over Complex Expressions
Use composition instead of complex expressions:

```csharp
// ❌ Avoid complex expressions
var spec = new Specification<Product>(p =>
    p.IsActive &&
    (p.Price > 100 || p.Category == "Premium") &&
    p.CreatedDate >= DateTime.Today.AddDays(-30));

// ✅ Use composition for better readability
var activeSpec = new Specification<Product>(p => p.IsActive);
var priceOrCategorySpec = new Specification<Product>(p => p.Price > 100)
    .Or(new Specification<Product>(p => p.Category == "Premium"));
var recentSpec = new Specification<Product>(p => p.CreatedDate >= DateTime.Today.AddDays(-30));

var spec = activeSpec.And(priceOrCategorySpec).And(recentSpec);
```

### 3. Performance Considerations
- Use `ApplyNoTracking()` for read-only operations
- Be selective with includes to avoid N+1 queries
- Use string-based includes for complex navigation paths
- Consider caching frequently used specifications

### 4. Caching
The specification generates a unique key that can be used for caching:

```csharp
public async Task<List<Product>> GetCachedProductsAsync(ISpecification<Product> spec)
{
    var cacheKey = $"products:{spec.Key}";

    if (_cache.TryGetValue(cacheKey, out List<Product> cached))
        return cached;

    var result = await _repository.GetAsync(spec);
    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

    return result;
}
```

## Migration Guide

### From Basic Specification
If you're migrating from a basic specification pattern:

1. **Add new properties** to your existing specifications
2. **Update method signatures** to include new parameters
3. **Replace complex expressions** with composition where beneficial
4. **Add string-based includes** for complex relationships
5. **Use projections** for performance optimization

### Example Migration

```csharp
// Before
public class ProductSpecification
{
    public Expression<Func<Product, bool>> Criteria { get; }
    public List<Expression<Func<Product, object>>> Includes { get; }
    public Expression<Func<Product, object>>? OrderBy { get; }
}

// After
public class ProductSpecification : ISpecification<Product>
{
    // All properties and methods from ISpecification<T>
    // Plus your custom logic
}
```

## Performance Tips

1. **Use projections** to select only needed fields
2. **Apply no-tracking** for read-only operations
3. **Be selective with includes** to avoid over-fetching
4. **Use string-based includes** for complex navigation
5. **Cache frequently used specifications**
6. **Combine specifications** instead of complex expressions

## Troubleshooting

### Common Issues

1. **Expression compilation errors**: Ensure all referenced properties exist
2. **Performance issues**: Use projections and no-tracking
3. **N+1 queries**: Use appropriate includes
4. **Memory issues**: Be selective with includes and use paging

### Debugging

Enable logging to see generated SQL:

```csharp
services.AddDbContext<YourDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});
```

## Contributing

When contributing to this specification pattern:

1. Follow the existing code style
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Consider performance implications
5. Maintain backward compatibility

## License

This specification pattern implementation is part of the IndTrace project and follows the same licensing terms.
