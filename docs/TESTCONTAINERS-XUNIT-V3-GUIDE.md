# Testcontainers + xUnit v3 Integration Guide

**Date**: 2025-11-03
**Purpose**: Migrate ExxerAI integration tests to use Testcontainers with xUnit v3

---

## 🎯 Key Pattern: IAsyncLifetime (Not IClassFixture!)

### ❌ OLD Pattern (xUnit v2 - Won't Work!)
```csharp
public class QdrantFixture : IClassFixture<QdrantContainerFixture>
{
    // This pattern doesn't work with xUnit v3!
}
```

### ✅ NEW Pattern (xUnit v3 - Works!)
```csharp
public class QdrantContainerFixture : IAsyncLifetime
{
    private QdrantContainer? _container;

    public async Task InitializeAsync()
    {
        _container = new QdrantBuilder()
            .WithImage("qdrant/qdrant:latest")
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_container != null)
            await _container.DisposeAsync();
    }
}
```

---

## 📦 Required Packages (Already in Directory.Build.props!)

```xml
<PackageVersion Include="Testcontainers.XunitV3" Version="4.8.1" />
<PackageVersion Include="Testcontainers.Qdrant" Version="4.*" />
<PackageVersion Include="Testcontainers.Neo4j" Version="4.*" />
<PackageVersion Include="Testcontainers.PostgreSql" Version="4.*" />
```

✅ **Testcontainers.XunitV3** v4.8.1 

---

## 🔧 Container Fixture Pattern

### Complete Qdrant Example
```csharp
using Testcontainers.Qdrant;

namespace ExxerAI.Tests.Integration.Fixtures;

public class QdrantContainerFixture : IAsyncLifetime
{
    private QdrantContainer? _container;

    // Expose configuration
    public string Hostname => _container?.Hostname ?? "localhost";
    public int HttpPort => _container?.GetMappedPublicPort(6333) ?? 6333;
    public int GrpcPort => _container?.GetMappedPublicPort(6334) ?? 6334;

    public QdrantClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _container = new QdrantBuilder()
            .WithImage("qdrant/qdrant:latest")
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

        await _container.StartAsync();

        // Create client after container starts
        Client = new QdrantClient(Hostname, HttpPort);
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();

        if (_container != null)
            await _container.DisposeAsync();
    }
}
```

### Complete Neo4j Example
```csharp
using Testcontainers.Neo4j;
using Neo4j.Driver;

namespace ExxerAI.Tests.Integration.Fixtures;

public class Neo4jContainerFixture : IAsyncLifetime
{
    private Neo4jContainer? _container;

    public string Hostname => _container?.Hostname ?? "localhost";
    public int BoltPort => _container?.GetMappedPublicPort(7687) ?? 7687;
    public int HttpPort => _container?.GetMappedPublicPort(7474) ?? 7474;

    public IDriver Driver { get; private set; } = null!;
    public string ConnectionString { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _container = new Neo4jBuilder()
            .WithImage("neo4j:5.15-community")
            .WithEnvironment("NEO4J_AUTH", "neo4j/password")
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

        await _container.StartAsync();

        ConnectionString = $"bolt://{Hostname}:{BoltPort}";
        Driver = GraphDatabase.Driver(ConnectionString,
            AuthTokens.Basic("neo4j", "password"));
    }

    public async Task DisposeAsync()
    {
        Driver?.Dispose();

        if (_container != null)
            await _container.DisposeAsync();
    }
}
```

---

## 🎯 Collection Definition Pattern (Share Fixtures)

```csharp
namespace ExxerAI.Tests.Integration.Fixtures;

/// <summary>
/// Defines a test collection that shares container fixtures across all tests.
/// Containers start once per collection, shared by all test classes.
/// </summary>
[CollectionDefinition("Integration")]
public class IntegrationTestCollection :
    ICollectionFixture<QdrantContainerFixture>,
    ICollectionFixture<Neo4jContainerFixture>
{
    // No implementation needed - this is just a marker class
}
```

---

## ✅ Using Fixtures in Tests

```csharp
namespace ExxerAI.Tests.Integration.KnowledgeStore;

[Collection("Integration")]  // ← Use the collection!
public class QdrantVectorStoreIntegrationTests
{
    private readonly QdrantContainerFixture _qdrantFixture;

    // Constructor injection of fixtures
    public QdrantVectorStoreIntegrationTests(
        QdrantContainerFixture qdrantFixture)
    {
        _qdrantFixture = qdrantFixture;
    }

    [Fact(Timeout = 30_000)]
    public async Task Should_Connect_To_Qdrant_Container()
    {
        // Use the fixture's client
        var collections = await _qdrantFixture.Client
            .ListCollectionsAsync();

        collections.ShouldNotBeNull();
    }
}
```

---

## 🎨 Wait Strategies (Optional - For Custom Containers)

Most official Testcontainers images (Qdrant, Neo4j, PostgreSQL) have **built-in readiness checks** and don't need explicit wait strategies. However, for custom containers:

```csharp
// HTTP endpoint readiness
.WithWaitStrategy(Wait.ForUnixContainer()
    .UntilHttpRequestIsSucceeded(r => r.ForPort(8080)))

// Port availability
.WithWaitStrategy(Wait.ForUnixContainer()
    .UntilPortIsAvailable(8080))

// Log message
.WithWaitStrategy(Wait.ForUnixContainer()
    .UntilMessageIsLogged("Server started"))

// Custom health check
.WithWaitStrategy(Wait.ForUnixContainer()
    .AddCustomWaitStrategy(new HealthCheckWait()))
```

---

## 📋 Migration Checklist for ExxerAI Container Tests

### Phase 1: Update Fixtures ✅
- [x] Verify `Testcontainers.XunitV3` v4.8.1 in Directory.Build.props
- [ ] Convert all `IClassFixture<T>` fixtures to `IAsyncLifetime`
- [ ] Add `InitializeAsync()` with container startup
- [ ] Add `DisposeAsync()` with cleanup
- [ ] Remove any custom lifecycle management code

### Phase 2: Create Collection Definitions
- [ ] Create `IntegrationTestCollection` class
- [ ] Add `[CollectionDefinition("Integration")]` attribute
- [ ] Implement `ICollectionFixture<T>` for each container fixture

### Phase 3: Update Test Classes
- [ ] Add `[Collection("Integration")]` to all test classes
- [ ] Change constructor injection to use fixture types directly
- [ ] Remove old `IClassFixture<T>` usage

### Phase 4: Verify Container Configurations
- [ ] Check all `.WithImage()` versions are correct
- [ ] Ensure `.WithAutoRemove(true)` for cleanup
- [ ] Ensure `.WithCleanUp(true)` for full cleanup
- [ ] Add wait strategies if needed (usually not for official images)

### Phase 5: Test & Validate
- [ ] Build integration test project
- [ ] Run individual container tests
- [ ] Verify containers start and stop properly
- [ ] Check for resource leaks (containers not stopping)

---

## 🔍 Common Issues & Solutions

### Issue: "Container not found" errors
**Solution**: Ensure Docker Desktop is running and containers have time to start.

### Issue: Port conflicts
**Solution**: Use `.WithPortBinding(hostPort, true)` for dynamic port assignment.

### Issue: Containers not cleaning up
**Solution**: Always use both `.WithAutoRemove(true)` and `.WithCleanUp(true)`.

### Issue: Tests timeout
**Solution**:
1. Increase `[Fact(Timeout = 60_000)]` for first test (container startup)
2. Add wait strategies if container takes long to become ready
3. Check Docker resource limits

### Issue: "Collection fixture" not injecting
**Solution**: Ensure:
1. `[CollectionDefinition]` class exists
2. Test class has `[Collection("Integration")]` attribute
3. Constructor parameter types match fixture types exactly

---

## 🎯 Best Practices

1. **One fixture per container type** - Don't mix multiple container types in one fixture
2. **Use collections for sharing** - Reduces container startup time across tests
3. **Dispose properly** - Always implement `DisposeAsync()` fully
4. **Expose configuration** - Make ports and connection strings accessible
5. **Create clients in fixtures** - Pre-initialize clients for tests to use
6. **Use meaningful timeouts** - First test needs more time for container startup
7. **Clean images** - Use official, tested container images when possible

---

## 📚 Reference Links

- [Testcontainers for .NET Documentation](https://dotnet.testcontainers.org/)
- [xUnit v3 Documentation](https://xunit.net/docs/getting-started/v3/cmdline)
- [IAsyncLifetime Interface](https://xunit.net/docs/shared-context#async-lifetime)
- [Collection Fixtures](https://xunit.net/docs/shared-context#collection-fixture)

---

## 🚀 Quick Start Template

```csharp
// 1. Create Fixture
public class MyContainerFixture : IAsyncLifetime
{
    private MyContainer? _container;
    public async Task InitializeAsync() { /* start container */ }
    public async Task DisposeAsync() { /* cleanup */ }
}

// 2. Create Collection
[CollectionDefinition("MyTests")]
public class MyTestCollection : ICollectionFixture<MyContainerFixture> { }

// 3. Use in Tests
[Collection("MyTests")]
public class MyIntegrationTests
{
    private readonly MyContainerFixture _fixture;
    public MyIntegrationTests(MyContainerFixture fixture) => _fixture = fixture;

    [Fact] public async Task Test() { /* use _fixture */ }
}
```

---

**Last Updated**: 2025-11-03
**Status**: Ready for implementation
**Impact**: Fixes all xUnit v3 container test issues
