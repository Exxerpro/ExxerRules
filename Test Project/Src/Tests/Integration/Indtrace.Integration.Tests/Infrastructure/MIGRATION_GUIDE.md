# Migration Guide: TestHostFixture to WebApplicationFactoryFixture

This guide explains how to migrate from the old `TestHostFixture` to the new `WebApplicationFactoryFixture`.

## Key Benefits of the New Setup

1. **Performance**: Uses pooled DbContext factories (solves "154 Context Creation Monster")
2. **Consistency**: Follows Monitor/Dependencies DI patterns
3. **Simplicity**: Extension methods reduce boilerplate
4. **Flexibility**: Easy to add new services

## Migration Steps

### 1. Update Test Class

**Old Pattern:**
```csharp
public class MyIntegrationTest : IClassFixture<TestHostFixture>
{
    private readonly TestHostFixture _fixture;

    public MyIntegrationTest(TestHostFixture fixture)
    {
        _fixture = fixture;
    }
}
```

**New Pattern:**
```csharp
public class MyIntegrationTest : IClassFixture<WebApplicationFactoryFixture>
{
    private readonly WebApplicationFactoryFixture _fixture;

    public MyIntegrationTest(WebApplicationFactoryFixture fixture)
    {
        _fixture = fixture;
    }
}
```

### 2. Update Service Resolution

**Old Pattern:**
```csharp
var repository = _fixture.Services.GetRequiredKeyedService<IRepository<Machine>>(DbProfiles.IndTraceDbContext45);
```

**New Pattern (same):**
```csharp
var repository = _fixture.GetKeyedService<IRepository<Machine>>(DbProfiles.IndTraceDbContext45);
```

### 3. Update Scoped Services

**Old Pattern:**
```csharp
using var scope = _fixture.Services.CreateScope();
var service = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Product>>(profile);
```

**New Pattern:**
```csharp
using var scope = _fixture.CreateScope();
var service = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Product>>(profile);
```

## What's Changed

1. **Context Pooling**: Now uses `AddPooledDbContextFactory` with 128 pool size
2. **Extension Methods**: Repository registration via `AddKeyedRepositoriesForProfile`
3. **WebApplicationFactory**: Full ASP.NET Core test host support
4. **Cleaner Code**: Removed manual registration boilerplate

## Adding New Services

To add new keyed services, update `KeyedServiceRegistrationExtensions.cs`:

```csharp
// In AddKeyedApplicationServicesForProfile method:
services.AddKeyedScoped<IMyService>(profile, (sp, _) =>
{
    var repository = sp.GetRequiredKeyedService<IRepository<MyEntity>>(profile);
    return new MyService(repository);
});
```

## Running Tests

The new setup is fully compatible with existing tests. Just update the fixture type and run:

```bash
dotnet test Src/Tests/Integration/Indtrace.Integration.Tests/Integration.Tests.csproj
```
