# Migration Example: DependenciesFactory → DependenciesFixtureBase

## Before (Inheritance-based)

```csharp
public class UpdateMachineCommandTest : DependenciesFactory
{
    public UpdateMachineCommandTest(ITestOutputHelper outputHelper)
        :base(output)
    {
    }

    [Fact]
    public async Task ShouldSendRequestAsync()
    {
        // Arrange

        var dispatcher = DpMonitorRequestDispatcher;
        // ... rest of test
    }
}
```

## After (Composition-based)

```csharp
public class UpdateMachineCommandTest : DependenciesFixtureBase
{
    public UpdateMachineCommandTest(ITestOutputHelper outputHelper)
        :base(output)
    {
    }

    [Fact]
    public async Task ShouldSendRequestAsync()
    {
        // Arrange
        var dispatcher = DpMonitorRequestDispatcher;
        // ... rest of test (no changes needed!)
    }
}
```

## Key Changes

1. **Base class**: Change from `DependenciesFactory` to `DependenciesFixtureBase`
2. **Constructor**: Remove `ITestContextAccessor` parameter (handled internally)
3. **Remove `await Initialization`**: xUnit v3 handles this via `IAsyncLifetime`
4. **Everything else stays the same**: All `Dp*` properties work identically

## Benefits

- ✅ Per-test DI container (proper isolation)
- ✅ Automatic cache partitioning (no contamination)
- ✅ Proper EF Core pooling (no exhaustion)
- ✅ State preserved between tests (by design)
- ✅ Minimal code changes (frictionless migration)
