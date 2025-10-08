using IndTrace.Application.Machines.Queries.GetDetail;
using IndTrace.Application.Machines.Queries.GetMachinesList;

namespace Integration.Tests.Machines.Tests.Queries;
/// <summary>
/// Represents the GetMachineDetailQueryHandlerTest.
/// </summary>

public class GetMachineDetailQueryHandlerTest(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output) : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
{
    private readonly IServiceProvider _services = fixture.Services;
    private readonly ITestOutputHelper _output = output;
    public static bool ShouldSkipDb45 => TestDbGuards.ShouldSkipDb45;
    /// <summary>
    /// Executes GetMachineDetail operation.
    /// </summary>
    /// <returns>The result of GetMachineDetail.</returns>

    [Fact(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
    [Trait("Db", "DB45")]
    public async Task GetMachineDetail()
    {
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(GetMachineDetailQueryHandlerTest));

        var request = new GetMachineDetailQuery();
        request.Id = 100; // Using MachineId from test data (100, 400, 500)

        var machineRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(dbKey);

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetMachineDetailQueryHandler>>();

        var sut = new GetMachineDetailQueryHandler(machineRepo, logger);

        var result = await sut.ProcessAsync(request, TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        result.Value.ShouldBeOfType<MachineDto>();
        result.Value.MachineId.ShouldBe(100);
    }

    [Fact(Skip = "QA62 database may have different machine data structure")]
    [Trait("Db", "DB45")]
    public async Task GetMachineDetail_On_QA62()
    {
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext62;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(GetMachineDetailQueryHandlerTest));

        var request = new GetMachineDetailQuery { Id = 100 };
        var machineRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(dbKey);
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetMachineDetailQueryHandler>>();
        var sut = new GetMachineDetailQueryHandler(machineRepo, logger);

        var result = await sut.ProcessAsync(request, TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<MachineDto>();
    }
}
