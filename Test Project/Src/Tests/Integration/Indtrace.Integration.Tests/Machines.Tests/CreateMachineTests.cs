using Integration.Tests.Extensions;

namespace Integration.Tests.Machines.Tests
{
    /// <summary>
    /// Represents the CreateMachineTests.
    /// </summary>
    public class CreateMachineTests : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
    {
        private readonly IServiceProvider _services;
        private readonly ITestOutputHelper _output;
        public static bool ShouldSkipDb45 => TestDbGuards.ShouldSkipDb45;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fixture">The test host fixture.</param>
        /// <param name="output">The output.</param>

        public CreateMachineTests(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output)
        {
            _services = fixture.Services;
            _output = output;
        }

        /// <summary>
        /// Executes Should_Create_Machine_When_Valid operation.
        /// </summary>
        /// <returns>The result of Should_Create_Machine_When_Valid.</returns>

        [Fact(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
        [Trait("Db", "DB45")]
        public async Task Should_Create_Machine_When_Valid()
        {
            const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
            var (sut, repo, logger) = _services.BuildHandler<CreateMachineMonitorRequestHandler, Machine>(_output, dbKey,
                (r, l) => new CreateMachineMonitorRequestHandler(r, l));

            var result = Result<MachineCreated>.WithFailure("");

            var request = new CreateMachineMonitorRequest
            {
                MachineId = 10025,
                Name = "Station 125",
                Location = "TestLine",
                MachineType = 1,
                EnableAppTraceability = 1,
                EnableBypassTraceability = 0
            };

            try
            {
                result = await sut.ProcessAsync(request, TestContext.Current.CancellationToken);
            }
            catch (Exception e)
            {
                _output.WriteLine($"Exception: {e.Message}");
                throw; // Rethrow the exception to fail the test if needed
            }
            finally
            {
                // Cleanup: Remove the machine if it was created
                var machine = new Machine()
                {
                    MachineId = 10025,
                    Name = "Station 125",
                };
                var spec = new Specification<Machine>(m => m.MachineId == machine.MachineId || m.Name == machine.Name);
                var r = await repo.FirstOrDefaultAsync(spec, TestContext.Current.CancellationToken);
                r.IsSuccess.ShouldBeTrue("Because the machine was found on the database");

                if (r.IsSuccess && r.Value is not null)
                {
                    logger.LogInformation("The machine RecipeId {id} {Name} was found on the database", machine.MachineId, machine.Name);
                    await repo.DeleteAsync(machine, TestContext.Current.CancellationToken);
                }
            }
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.Name.ShouldBe("Station 125");
        }

        private (CreateMachineMonitorRequestHandler sut, IRepository<Machine> repo, ILogger<CreateMachineMonitorRequestHandler> logger) CreateSutRepoLogger()
        {
            const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
            using var scope = _services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(dbKey);
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CreateMachineMonitorRequestHandler>>();
            var sut = new CreateMachineMonitorRequestHandler(repo, logger);
            return (sut, repo, logger);
        }
    }
}
