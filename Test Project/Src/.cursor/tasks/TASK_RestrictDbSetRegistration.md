# ✅ Task: Restrict DbSet<T> to Valid Domain Types

## 🎯 Objective

Prevent accidental registration of invalid types as `DbSet<T>` in the `DbContext`. Only types implementing one of the allowed marker interfaces should be registered.

## ✅ Allowed Types

| Interface         | Description                       |
|------------------|------------------------------------|
| `IEntityRoot`     | Aggregate root domain entities     |
| `ILookupEntity`   | Smart-enum-style lookup tables     |

All other types (value objects, DTOs, helpers) **must NOT** appear in `DbSet<T>`.

## 🧩 Marker Interfaces

```csharp
public interface IEntityRoot { }
public interface ILookupEntity { }
```

## 🔧 Enforcement in DbContext

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    var invalidDbSetTypes = modelBuilder.Model
        .GetEntityTypes()
        .Where(t =>
            !typeof(IEntityRoot).IsAssignableFrom(t.ClrType) &&
            !typeof(ILookupEntity).IsAssignableFrom(t.ClrType))
        .Select(t => t.ClrType.Name)
        .ToList();

    if (invalidDbSetTypes.Any())
    {
        var message = "🚫 Invalid DbSet<T> types detected:\n" + string.Join("\n", invalidDbSetTypes);
        throw new InvalidOperationException(message);
    }

    modelBuilder.EnforceModelConfiguration(); // from separate task
}
```

## 🔧 Developer Instructions

1. Review all domain entities currently in `DbSet<T>`.
2. Ensure each implements either:
   - `IEntityRoot` (if it’s a main business object), or
   - `ILookupEntity` (if it’s a lookup table for SmartEnums).
3. Remove any `DbSet<T>` for non-compliant types.
4. Fix application startup errors raised due to invalid entity registration.

## ✅ Acceptance Criteria

- No types without `IEntityRoot` or `ILookupEntity` are included in `DbSet<T>`.
- App fails on startup if any invalid types are found in the model.
