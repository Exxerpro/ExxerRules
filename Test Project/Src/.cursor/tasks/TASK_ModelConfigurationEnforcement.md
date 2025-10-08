TASK_ModelConfigurationEnforcement.md

# ✅ Task: Enforce Explicit EF Core Model Configuration

## 🎯 Objective

Ensure all EF Core model properties are explicitly configured to avoid implicit schema defaults such as `nvarchar(max)`, `datetime`, and undefined `decimal` precision.

## 🔧 Developer Instructions

1. Clone or edit the shared `EnforceModelConfiguration` logic in `DbContextExtensions.cs` (see below).
2. Add a call to `modelBuilder.EnforceModelConfiguration()` at the end of your `OnModelCreating()` in `YourDbContext`.
3. Run the project and fix any thrown exceptions indicating improperly configured properties.

## 🔍 Rules to Enforce

| Type        | Rule |
|-------------|------|
| `string`    | Must have `.HasMaxLength(n)` |
| `decimal`   | Must have `.HasPrecision(total, scale)` |
| `DateTime`  | Must have `.HasColumnType("datetime2")` |
| `byte[]`    | Must have `.HasMaxLength(n)` or explicit `.HasColumnType(...)` |

## 🧩 Code Snippet: `DbContextExtensions.cs`

```csharp
public static class DbContextModelValidationExtensions
{
    public static void EnforceModelConfiguration(this ModelBuilder modelBuilder)
    {
        var failures = new List<string>();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var clrType = property.ClrType;
                var propertyName = $"{entityType.ClrType.Name}.{property.Name}";

                if (clrType == typeof(string) && property.GetMaxLength() == null)
                    failures.Add($"❌ Missing MaxLength → {propertyName}");

                else if (clrType == typeof(decimal) &&
                        (property.GetPrecision() == null || property.GetScale() == null))
                    failures.Add($"❌ Missing Precision/Scale → {propertyName}");

                else if (clrType == typeof(DateTime) &&
                        (string.IsNullOrWhiteSpace(property.GetColumnType()) ||
                         !property.GetColumnType().Contains("datetime2")))
                    failures.Add($"❌ Missing datetime2 type → {propertyName}");

                else if (clrType == typeof(byte[]) &&
                        property.GetMaxLength() == null &&
                        string.IsNullOrWhiteSpace(property.GetColumnType()))
                    failures.Add($"❌ Missing MaxLength or column type for byte[] → {propertyName}");
            }
        }

        if (failures.Any())
            throw new InvalidOperationException("🚫 EF Core Model Validation Failed:\n" + string.Join("\n", failures));
    }
}

 Acceptance Criteria
No application start failures from EnforceModelConfiguration().

All entity configurations explicitly declare lengths, types, or precision.

Value objects and smart enums are not affected.


---

### 📄 `TASK_RestrictDbSetRegistration.md`

```markdown
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

🔧 Enforcement in DbContext


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

 Developer Instructions
Review all domain entities currently in DbSet<T>.

Ensure each implements either:

IEntityRoot (if it’s a main business object), or

ILookupEntity (if it’s a lookup table for SmartEnums).

Remove any DbSet<T> for non-compliant types.

Fix application startup errors raised due to invalid entity registration.

---
Acceptance Criteria
No types without IEntityRoot or ILookupEntity are included in DbSet<T>.

App fails on startup if any invalid types are found in the model.
