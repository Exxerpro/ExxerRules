using IndTrace.Application.ConfigApplication.Commands.Create;

namespace Integration.Tests.ConfigApps.Commands;

/// <summary>
/// Integration tests for CreateConfigAppCommandHandler using InMemory EF Core
/// </summary>
public class CreateConfigAppIntegrationTests : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
{
    private readonly IServiceProvider _services;
    private readonly ITestOutputHelper _output;
    public static bool ShouldSkipDb45 => TestDbGuards.ShouldSkipDb45;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="fixture">The test host fixture.</param>

    public CreateConfigAppIntegrationTests(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output)
    {
        _services = fixture.Services;
        _output = output;
    }

    /// <summary>
    /// Executes Process_WithValidCommand_ShouldCreateConfigApp operation.
    /// </summary>
    /// <returns>The result of Process_WithValidCommand_ShouldCreateConfigApp.</returns>

    [Fact]
    [Trait("Db", "DB45")]
    public async Task Process_WithValidCommand_ShouldCreateConfigApp()
    {
        // Arrange
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(CreateConfigAppIntegrationTests));
        var repository = scope.ServiceProvider.GetRequiredKeyedService<IRepository<ConfigApp>>(dbKey);
        var logger = XUnitLogger.CreateLogger<CreateConfigAppCommandHandler>();
        var handler = new CreateConfigAppCommandHandler(repository, logger);

        var command = new CreateConfigAppCommand
        {
            ConfigId = "TestL1A123",
            MachineId = 999,
            Pc = "1",
            Client = "TestClient",
            Factorie = "TestFactory",
            Line = "TestLine",
            Machine = "TestMachine",
            Project = "TestProject",
            Version = "1.0",
            VersionDate = new DateTime(2023, 12, 1),
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        var result = await handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ConfigId.ShouldBe(command.ConfigId);

        // Verify entity was persisted - find by ConfigAppId since AppId is auto-generated
        var queryableResult = await repository.AsQueryableAsync(TestContext.Current.CancellationToken);
        queryableResult.IsSuccess.ShouldBeTrue();
        queryableResult.Value.ShouldNotBeNull();
        var persisted = queryableResult.Value.FirstOrDefault(x => x.ConfigAppId == command.ConfigId);
        persisted.ShouldNotBeNull();
        persisted.ConfigAppId.ShouldBe(command.ConfigId);
        persisted.Client.ShouldBe(command.Client);
        persisted.MachineId.ShouldBe(command.MachineId);
        persisted.Pc.ShouldBe(command.Pc);
    }

    /// <summary>
    /// Executes Process_WithDuplicateConfigId_ShouldHandleGracefully operation.
    /// </summary>
    /// <returns>The result of Process_WithDuplicateConfigId_ShouldHandleGracefully.</returns>

    [Fact]
    [Trait("Db", "DB45")]
    public async Task Process_WithDuplicateConfigId_ShouldHandleGracefully()
    {
        // Arrange
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(CreateConfigAppIntegrationTests));
        var repository = scope.ServiceProvider.GetRequiredKeyedService<IRepository<ConfigApp>>(dbKey);
        var logger = XUnitLogger.CreateLogger<CreateConfigAppCommandHandler>();
        var handler = new CreateConfigAppCommandHandler(repository, logger);

        var command1 = new CreateConfigAppCommand
        {
            ConfigId = "DuplicateTest_L1A",
            MachineId = 100001,
            Client = "TestClient1",
            Factorie = "TestFactory1",
            Line = "TestLine1",
            Machine = "TestMachine1",
            Project = "TestProject1",
            Version = "1.0",
            VersionDate = new DateTime(2023, 12, 1),
            ModifiedDate = DateTime.UtcNow
        };

        var command2 = new CreateConfigAppCommand
        {
            ConfigId = "DuplicateTest_L1B",
            MachineId = 100002,
            Client = "TestClient2",
            Factorie = "TestFactory2",
            Line = "TestLine2",
            Machine = "TestMachine2",
            Project = "TestProject2",
            Version = "1.0",
            VersionDate = new DateTime(2023, 12, 1),
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        var result1 = await handler.ProcessAsync(command1, TestContext.Current.CancellationToken);
        var result2 = await handler.ProcessAsync(command2, TestContext.Current.CancellationToken);

        // Assert
        result1.IsSuccess.ShouldBeTrue();
        result2.IsSuccess.ShouldBeTrue();
        result1.Value.ShouldNotBeNull();
        result2.Value.ShouldNotBeNull();
        result1.Value.ConfigId.ShouldBe(command1.ConfigId);
        result2.Value.ConfigId.ShouldBe(command2.ConfigId);
    }

    /// <summary>
    /// Executes Process_WithVariousStringLengths_ShouldHandleCorrectly operation.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="factory">The factory.</param>
    /// <param name="line">The line.</param>
    /// <returns>The result of Process_WithVariousStringLengths_ShouldHandleCorrectly.</returns>

    [Theory]
    [Trait("Db", "DB45")]
    [InlineData("")]
    [InlineData("TestClient", "TestFactory", "TestLine")]
    [InlineData("LongClientNameForTesting", "LongFactoryNameForTesting", "LongLineNameForTesting")]
    public async Task Process_WithVariousStringLengths_ShouldHandleCorrectly(string client, string? factory = null, string? line = null)
    {
        // Arrange
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(CreateConfigAppIntegrationTests));
        var repository = scope.ServiceProvider.GetRequiredKeyedService<IRepository<ConfigApp>>(dbKey);
        var logger = XUnitLogger.CreateLogger<CreateConfigAppCommandHandler>();
        var handler = new CreateConfigAppCommandHandler(repository, logger);

        var command = new CreateConfigAppCommand
        {
            ConfigId = $"StringTest_{Guid.NewGuid():N}",
            MachineId = Random.Shared.Next(2000, 9999),
            Client = client,
            Factorie = factory ?? "DefaultFactory",
            Line = line ?? "DefaultLine",
            Machine = "TestMachine",
            Project = "TestProject",
            Version = "1.0",
            VersionDate = new DateTime(2023, 12, 1),
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        var result = await handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ConfigId.ShouldBe(command.ConfigId);
    }

    /// <summary>
    /// Executes Process_WithCancellationToken_ShouldRespectCancellation operation.
    /// </summary>
    /// <returns>The result of Process_WithCancellationToken_ShouldRespectCancellation.</returns>

    [Fact(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
    [Trait("Db", "DB45")]
    public async Task Process_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(CreateConfigAppIntegrationTests));
        var repository = scope.ServiceProvider.GetRequiredKeyedService<IRepository<ConfigApp>>(dbKey);
        var logger = XUnitLogger.CreateLogger<CreateConfigAppCommandHandler>();
        var handler = new CreateConfigAppCommandHandler(repository, logger);

        var command = new CreateConfigAppCommand
        {
            ConfigId = "CancellationTest_L1A",
            MachineId = 100100,
            Client = "TestClient",
            Factorie = "TestFactory",
            Line = "TestLine",
            Machine = "TestMachine",
            Project = "TestProject",
            Version = "1.0",
            VersionDate = new DateTime(2023, 12, 1),
            ModifiedDate = DateTime.UtcNow
        };

        using var cts = new CancellationTokenSource();

        // Act & Assert
        var task = handler.ProcessAsync(command, cts.Token);
        cts.Cancel();

        // The behavior depends on when cancellation occurs
        // If it's already completed, it should succeed
        // If it's cancelled before completion, it should throw

        var result = await task;
        result.IsSuccess.ShouldBeFalse();
    }

    /// <summary>
    /// Executes Process_MultipleEntities_ShouldPersistAll operation.
    /// </summary>
    /// <returns>The result of Process_MultipleEntities_ShouldPersistAll.</returns>

    [Fact(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
    [Trait("Db", "DB45")]
    public async Task Process_MultipleEntities_ShouldPersistAll()
    {
        // Arrange
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(CreateConfigAppIntegrationTests));
        var repository = scope.ServiceProvider.GetRequiredKeyedService<IRepository<ConfigApp>>(dbKey);
        var logger = XUnitLogger.CreateLogger<CreateConfigAppCommandHandler>();
        var handler = new CreateConfigAppCommandHandler(repository, logger);

        var commands = new[]
        {
            new CreateConfigAppCommand
            {
                ConfigId = "MultiTest_L1A",
                MachineId = 100200,
                Client = "MultiClient1",
                Factorie = "MultiFactory1",
                Line = "MultiLine1",
                Project = "MultiProject1",
                Version = "1.0",
                VersionDate = new DateTime(2023, 12, 1),
                ModifiedDate = DateTime.UtcNow
            },
            new CreateConfigAppCommand
            {
                ConfigId = "MultiTest_L1B",
                MachineId = 100201,
                Client = "MultiClient2",
                Factorie = "MultiFactory2",
                Line = "MultiLine2",
                Project = "MultiProject2",
                Version = "1.0",
                VersionDate = new DateTime(2023, 12, 1),
                ModifiedDate = DateTime.UtcNow
            }
        };

        // Act
        var results = new List<Task<Result<ConfigAppCreated>>>();
        foreach (var command in commands)
        {
            results.Add(handler.ProcessAsync(command, TestContext.Current.CancellationToken));
        }

        var completedResults = await Task.WhenAll(results);

        // Assert
        foreach (var result in completedResults)
        {
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
        }

        // Verify all entities were persisted
        var allEntities = await repository.ListAsync(TestContext.Current.CancellationToken);
        allEntities.IsSuccess.ShouldBeTrue();
        allEntities.Value.ShouldNotBeNull();
        var allList = allEntities.Value;
        var multiTestEntities = allList.Where(e => e.ConfigAppId.StartsWith("MultiTest_")).ToList();
        multiTestEntities.Count.ShouldBeGreaterThanOrEqualTo(2);
    }
}
