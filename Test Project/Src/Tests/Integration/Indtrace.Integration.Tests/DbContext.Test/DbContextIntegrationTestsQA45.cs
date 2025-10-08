namespace Integration.Tests.DbContext.Test
{
    /// <summary>
    /// Represents the DbContextIntegrationTestsQA45.
    /// </summary>
    public class DbContextIntegrationTestsQA45 : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
    {
        private readonly IServiceProvider _services;
        private readonly ITestOutputHelper _output;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fixture">The test host fixture.</param>
        /// <param name="output">The output.</param>

        public DbContextIntegrationTestsQA45(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output)
        {
            _services = fixture.Services;
            _output = output;
        }

        /// <summary>
        /// Executes Should_Connect_And_Ping_DbContext operation.
        /// </summary>
        /// <returns>The result of Should_Connect_And_Ping_DbContext.</returns>

        public static bool ShouldSkipDb45 => TestDbGuards.ShouldSkipDb45;

        [Fact(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
        [Trait("Db", "DB45")]
        public async Task Should_Connect_And_Ping_DbContext()
        {
            const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
            using var scope = _services.CreateScope();
            DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(DbContextIntegrationTestsQA45));
            var factory = scope.ServiceProvider.GetRequiredKeyedService<IIndTraceDbContextFactory>(dbKey);
            await using var db = (IndTraceDbContext)factory.CreateDbContext();

            var canConnect = await db.Database.CanConnectAsync(TestContext.Current.CancellationToken);
            canConnect.ShouldBeTrue();
        }
    }
}
