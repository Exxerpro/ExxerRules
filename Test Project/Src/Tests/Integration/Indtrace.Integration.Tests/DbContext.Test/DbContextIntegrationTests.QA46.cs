namespace Integration.Tests.DbContext.Test;

public class DbContextIntegrationTests_QA46 : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
{
    private readonly IServiceProvider _services;
    private readonly ITestOutputHelper _output;

    public DbContextIntegrationTests_QA46(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output)
    {
        _services = fixture.Services;
        _output = output;
    }

    public static bool ShouldSkipQA46 => TestDbGuards.ShouldSkipQA46;

    [Fact(Skip = "Missing QA46 DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipQA46))]
    [Trait("Db","QA46")]
    public async Task Should_Connect_And_Ping_QA46_DbContext()
    {
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext46;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(DbContextIntegrationTests_QA46));
        var factory = scope.ServiceProvider.GetRequiredKeyedService<IIndTraceDbContextFactory>(dbKey);
        await using var db = (IndTraceDbContext)factory.CreateDbContext();

        var canConnect = await db.Database.CanConnectAsync(TestContext.Current.CancellationToken);
        canConnect.ShouldBeTrue();
    }
}
