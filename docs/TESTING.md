# Testing Guide: Hanging Tests, Timeouts, Logging, and Tracing

## Overview

This guide documents patterns for preventing and debugging hanging tests, configuring test timeouts, using structured logging, and tracing SUT (System Under Test) method calls.

## Table of Contents

1. [Hanging Test Patterns](#hanging-test-patterns)
2. [Timeout Strategies](#timeout-strategies)
3. [Logging in Tests](#logging-in-tests)
4. [SUT Tracing](#sut-tracing)
5. [Troubleshooting Guide](#troubleshooting-guide)

## Hanging Test Patterns

### Common Causes

1. **Real Client Connections**: Tests that create real client instances (e.g., `QdrantClient`, `HttpClient`) may hang if services aren't running.
2. **Missing Timeouts**: Tests without timeout attributes can run indefinitely.
3. **No Cancellation**: Using `CancellationToken.None` prevents graceful cancellation.
4. **No Visibility**: Missing logging makes it impossible to diagnose where tests hang.
5. **Real HTTP Calls**: Tests making actual HTTP requests may hang on network issues.

### Prevention Strategies

#### 1. Avoid Real Client Connections

**Problem**: Creating real clients (e.g., `QdrantClient("localhost", 6333)`) may hang if services aren't running.

**Solution**: Use environment variables or mock clients for unit tests.

```csharp
// ❌ BAD: May hang if Qdrant isn't running
_qdrantClient = new QdrantClient("localhost", 6333);

// ✅ GOOD: Check environment variable first
var qdrantEnv = Environment.GetEnvironmentVariable("QDRANT_AVAILABLE");
if (qdrantEnv == "true")
{
    _qdrantClient = new QdrantClient("localhost", 6333);
}
else
{
    // Use mock or skip for unit tests
}
```

#### 2. Use Mocked HTTP Clients

**Problem**: Real `HttpClient` instances may hang on network calls.

**Solution**: Use `MockHttpMessageHandler` for unit tests.

```csharp
// ✅ GOOD: Use mocked HttpClient
private HttpClient CreateMockHttpClient(float[] expectedEmbedding)
{
    var response = new { Embedding = expectedEmbedding };
    var json = JsonSerializer.Serialize(response);
    var handler = new MockHttpMessageHandler(json, HttpStatusCode.OK);
    return new HttpClient(handler);
}
```

#### 3. Mark Integration Tests

**Problem**: Tests requiring real services should be clearly marked.

**Solution**: Use `[Trait("Category", "Integration")]` attribute.

```csharp
[Fact(Timeout = 60000, Trait("Category", "Integration"))]
public async Task SearchAsync_WithRealQdrant_ShouldReturnResults()
{
    // Test requires real Qdrant service
}
```

## Timeout Strategies

### Timeout Guidelines

- **Unit Tests**: 5 seconds (5000ms) - Fast execution, mocked dependencies
- **Behavioral Tests**: 30 seconds (30000ms) - May involve I/O operations
- **Integration Tests**: 60 seconds (60000ms) - Real services, network calls

### Implementation

```csharp
[Fact(Timeout = 5000)]  // Unit test
public async Task TestMethod() { }

[Fact(Timeout = 30000, Trait("Category", "Behavioral"))]  // Behavioral test
public async Task BehavioralTest() { }

[Fact(Timeout = 60000, Trait("Category", "Integration"))]  // Integration test
public async Task IntegrationTest() { }
```

### Running Tests with Timeout

Use the `RunSingleTest.ps1` script to run individual tests with timeout protection:

```powershell
.\RunSingleTest.ps1 -TestFullyQualifiedName "Namespace.TestClass.TestMethod" -TimeoutSeconds 30
```

## Logging in Tests

### Meziantou XUnit Logger

Tests should use Meziantou XUnit logger for structured logging:

```csharp
public class MyTests : BaseTDDTest<MyImplementation>
{
    public MyTests(ITestOutputHelper output) : base(output)
    {
        // Logger is automatically created using Meziantou XUnit logger
    }

    [Fact(Timeout = 5000)]
    public async Task TestMethod()
    {
        Logger.LogInformation("Starting test");
        // Test implementation
    }
}
```

### Structured Logging

Use structured logging for better test output:

```csharp
Logger.LogInformation("Processing request: {RequestId}, {Method}", requestId, methodName);
Logger.LogError(ex, "Failed to process request: {RequestId}", requestId);
```

## SUT Tracing

### Using SUTTracer

The `SUTTracer` utility provides method entry/exit logging with correlation IDs:

```csharp
public class MyTests : BaseTDDTest<MyImplementation>
{
    [Fact(Timeout = 5000)]
    public async Task TestMethod()
    {
        // Tracer is automatically available via base class
        var result = await Tracer.TraceAsync(
            methodName: "MyMethod",
            parameters: new Dictionary<string, object> { { "param1", "value1" } },
            operation: async () => await Implementation.MyMethod("value1"));
    }
}
```

### Trace Output

SUTTracer generates structured trace output:

```
[14:23:45.123] [abc12345] ENTRY: MyMethod (param1=value1)
[14:23:45.456] [abc12345] EXIT: MyMethod -> ResultValue (333.33ms)
```

## Troubleshooting Guide

### Test Hangs

1. **Check Timeout**: Verify test has `[Fact(Timeout = X)]` attribute
2. **Check Dependencies**: Ensure no real client connections in unit tests
3. **Check Logs**: Review test output for hanging operations
4. **Use Tracer**: Add `Tracer.TraceAsync` around suspicious operations

### Test Times Out

1. **Increase Timeout**: For legitimate long-running tests, increase timeout
2. **Check Category**: Ensure test is marked correctly (Unit/Behavioral/Integration)
3. **Review Dependencies**: Check if test is waiting for unavailable services
4. **Use RunSingleTest**: Run test individually with detailed output

### Test Fails with Connection Error

1. **Check Environment**: Verify `QDRANT_AVAILABLE` or similar environment variables
2. **Use Mocks**: Ensure unit tests use mocked dependencies
3. **Mark Integration**: If test requires real service, mark with `[Trait("Category", "Integration")]`

### Debugging with RunSingleTest.ps1

```powershell
# Run single test with 30s timeout
.\src\scripts\RunSingleTest.ps1 `
    -TestFullyQualifiedName "IndFusion.SemanticRag.Tests.Unit.Implementations.QdrantVectorDatabaseAdapterTests.SearchAsync_WithNullQueryVector_ShouldReturnFailure" `
    -TimeoutSeconds 30
```

## Best Practices

1. **Always Use Timeouts**: Every test should have explicit timeout
2. **Mock Dependencies**: Unit tests should never use real clients
3. **Mark Categories**: Use traits to categorize tests (Fast/Behavioral/Integration)
4. **Use Structured Logging**: Leverage Meziantou logger for test output
5. **Trace SUT Calls**: Use SUTTracer for debugging complex test scenarios
6. **Environment Variables**: Use environment variables to control test behavior
7. **Fail Fast**: Tests should fail fast with clear error messages

## Configuration Files

### xunit.runner.json

Located in test project root (`ExxerRules/src/IndFusion.SemanticRag.Tests.Unit/xunit.runner.json`):

```json
{
  "$schema": "https://xunit.net/schema/current/xunit.runner.schema.json",
  "appDomain": "denied",
  "diagnosticMessages": true,
  "methodDisplay": "method",
  "parallelizeAssembly": false,
  "parallelizeTestCollections": true,
  "maxParallelThreads": 4
}
```

**Note**: Global timeout must be set via test attributes, not JSON (xUnit limitation).

## Related Documentation

- [ADR-004: Remedial Quality Proposal - Functional Methods Standardization](../adr4/ADR-004-Remedial-Quality-Proposal-Functional-Methods.md)
- [XUnit v3 Migration Guide](./XUnit-v3-Migration-Guide.md)
- [Testing Patterns](./specs/Testing-Patterns.md)

