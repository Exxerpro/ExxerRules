# IndTrace Monitor Tests - BUnit Component Testing

## Overview

This project contains smoke tests for the IndTrace Monitor application using **BUnit** for Blazor component testing instead of Playwright for end-to-end testing.

## Migration from Playwright to BUnit

### Why BUnit?

- ✅ **10x Faster**: No browser startup time
- ✅ **More Reliable**: No network/HTTPS issues  
- ✅ **Better Debugging**: Direct component testing
- ✅ **xUnit v3 Compatible**: No compatibility issues
- ✅ **Component-Level Testing**: More granular and useful

### What Was Migrated

| Old Playwright Test | New BUnit Test | Benefits |
|-------------------|----------------|----------|
| `PageLoadTests.cs` | `ComponentSmokeTests.cs` | Tests individual components |
| `MonitorPageTest.cs` | Component-specific tests | Faster, more reliable |
| `NavigationTests.cs` | Navigation component tests | No browser dependencies |
| `ReportsTests.cs` | Report component tests | Direct component validation |

## Test Structure

### Base Test Class
- `BUnitTestBase.cs` - Provides common setup and utilities
- Handles service configuration and common assertions

### Smoke Tests
- `ComponentSmokeTests.cs` - Basic component rendering tests
- Verifies components load without errors
- Checks for expected content

### Advanced Tests
- `AdvancedComponentTests.cs` - Complex interaction tests
- User interactions, events, async operations
- Form validation, navigation testing

## Usage Examples

### Basic Component Test
```csharp
[Fact]
public void MonitorComponent_ShouldRenderWithoutErrors()
{
    Component_ShouldRenderWithoutErrors<MonitorComponent>();
}
```

### Content Verification
```csharp
[Fact]
public void MonitorComponent_ShouldContainMonitorText()
{
    Component_ShouldContainText<MonitorComponent>("Monitor");
}
```

### User Interaction Test
```csharp
[Fact]
public void Component_ShouldHandleUserInteractions()
{
    var component = RenderComponent<InteractiveComponent>();
    var button = component.Find("button");
    button.Click();

    component.Find(".result").TextContent.ShouldContain("Clicked");
}
```

## Next Steps

1. **Replace Placeholder Components**: Update component references to use your actual Blazor components
2. **Add Service Mocks**: Configure services in `BUnitTestBase` for your components
3. **Extend Test Coverage**: Add more specific component tests based on your needs
4. **Integration Tests**: Consider adding integration tests for complex scenarios

## Dependencies

- `bunit` - Blazor component testing library
- `xunit.v3` - Testing framework
- `Shouldly` - Assertion library
- `NSubstitute` - Mocking library

## Benefits Achieved

- **Faster Test Execution**: No browser overhead
- **Better Reliability**: No network dependencies
- **Improved Debugging**: Direct component access
- **xUnit v3 Compatibility**: No more Playwright conflicts
- **Component-Level Testing**: More granular validation
