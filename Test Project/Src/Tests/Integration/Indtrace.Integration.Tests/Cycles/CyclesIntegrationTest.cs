namespace Integration.Tests.Cycles
{
    /// <summary>
    /// Represents the CyclesIntegrationTest.a
    /// </summary>
    public class CyclesIntegrationTest : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
    {
        private readonly ITestOutputHelper _output;
        private readonly IServiceProvider _services;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="output">The output.</param>

        public CyclesIntegrationTest(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output)
        {
            _output = output;
            _services = fixture.Services;
        }

        /// <summary>
        /// Executes Should_Continue_Cycle_When_Last_Not_Finished operation.
        /// </summary>
        /// <returns>The result of Should_Continue_Cycle_When_Last_Not_Finished.</returns>

        public static bool ShouldSkipDb45 => TestDbGuards.ShouldSkipDb45;

        [Theory(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
        [Trait("Db", "DB45")]
        [MemberData(nameof(LastCycleCases))]
        public async Task Should_Continue_Cycle_When_Last_Not_Finished(
            string label,
            int machineId,
            int lastMachineId,
            int nextMachineId,
            int lenPartNumber,
            string partStatus,
            string flowStatus,
            string cycleStatus,
            int barCodeId,
            int cycleId,
            string machineType,
            DateTime startCycle)
        {
            // Use parameters to avoid xUnit1026 warnings

            var logger = XUnitLogger.CreateLogger<UpdateBarCodeCommandHandler>();
            //Logger scneario of test  usint all the input parameters

            logger.LogInformation("?Test for machine {LABEL}, {machineId}, {lastMachineId}, {nextMachineId}, {lenPartNumber}, {partStatus}, {flowStatus}, {cycleStatus}, {barCodeId}, {cycleId}, {machineType}, {startCycle}",
                label, machineId, lastMachineId, nextMachineId, lenPartNumber, partStatus, flowStatus, cycleStatus, barCodeId, cycleId, machineType, startCycle);

            const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
            using var scope = _services.CreateScope();
            DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(CyclesIntegrationTest));

            var cycleRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Cycle>>(dbKey);
            var barCodeRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<BarCode>>(dbKey);

            var loggerBarCodeResult = XUnitLogger.CreateLogger<BarCodeResult>();
            var cyceRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Cycle>>(dbKey);

            var machineRepo = scope.ServiceProvider.GetRequiredKeyedService<IReadOnlyRepository<Machine>>(dbKey);
            var recipeRepo = scope.ServiceProvider.GetRequiredKeyedService<IReadOnlyRepository<Recipe>>(dbKey);
            var masterLabeRepo = scope.ServiceProvider.GetRequiredKeyedService<IReadOnlyRepository<MasterLabel>>(dbKey);
            var shifRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Shift>>(dbKey);
            var workflowRepo = scope.ServiceProvider.GetRequiredKeyedService<IReadOnlyRepository<WorkFlow>>(dbKey);
            var variableRepo = scope.ServiceProvider.GetRequiredKeyedService<IReadOnlyRepository<Variable>>(dbKey);
            var productRepo = scope.ServiceProvider.GetRequiredKeyedService<IReadOnlyRepository<Product>>(dbKey);
            IDateTimeMachine deDateTimeMachine = new DateTimeMachine();
            IBarCodeValidationService barCodeValidationService = new BarCodeValidationService();

            var barCodeIS = new BarCodeResult(loggerBarCodeResult, barCodeRepo, cyceRepo, machineRepo, recipeRepo, masterLabeRepo, shifRepo, workflowRepo, variableRepo, productRepo, deDateTimeMachine, barCodeValidationService);

            var timeCycle = TimeSpan.FromSeconds(45);
            var now = startCycle.Add(timeCycle);

            var dateTimeMock = Substitute.For<IDateTimeMachine>();
            dateTimeMock.Now.Returns(now);

            var partNumber = label.Substring(3, lenPartNumber);

            var sut = new UpdateBarCodeCommandHandler(dateTimeMock, cycleRepo, barCodeIS);

            var command = new UpdateBarCodeCommand();
            command.WithData(TaskGatewayRequest.Create(machineId, label, partNumber, PartStatus.Ok));

            // Act
            var result = await sut.ProcessAsync(command, TestContext.Current.CancellationToken);

            logger.LogInformation("Result {result}", result.Value?.ToString() ?? "null");

            // Assert
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<TaskGatewayResponse>();
            result.Value.Label.ShouldBe(label);
            result.Value.BarCodeId.ShouldBe(barCodeId);
            result.Value.CycleId.ShouldBe(cycleId);
        }

        public static IEnumerable<object[]> LastCycleCases => new List<object[]>
        {
            // Real working cycle data from QA45 database
            new object[] { "QA45422290251700924", 100, 100, 100, 6, "Ok", "Created", "Started", 50924, 123418, "InitialPrinter", "2025-06-18T18:43:03.977" },
            new object[] { "QA45422310251420890", 400, 400, 400, 6, "Ok", "InProcess", "Started", 50890, 123416, "Process", "2025-05-22T05:52:34.469" },
            new object[] { "QA45422290251420595", 500, 500, 500, 6, "Ok", "Finished", "Started", 50595, 122997, "Final", "2025-05-22T04:43:04.818" },
            new object[] { "QA45422290251420639", 500, 500, 500, 6, "Ok", "Finished", "Started", 50639, 122786, "Final", "2025-05-22T02:23:33.818" },
            new object[] { "QA45422290251420702", 100, 100, 100, 6, "Ok", "Created", "Started", 50702, 123415, "InitialPrinter", "2025-05-22T01:35:26.743" },
            new object[] { "QA45422290251420695", 100, 100, 100, 6, "Ok", "Created", "Started", 50695, 123419, "InitialPrinter", "2025-05-22T01:29:54.504" },
            new object[] { "QA45422290251420599", 500, 500, 500, 6, "Ok", "Finished", "Started", 50599, 122559, "Final", "2025-05-22T01:16:09.624" },
            new object[] { "QA45422290251420672", 100, 100, 100, 6, "Ok", "Created", "Started", 50672, 123417, "InitialPrinter", "2025-05-22T01:12:34.228" },
            new object[] { "QA45422290251420602", 500, 500, 500, 6, "Ok", "Finished", "Started", 50602, 122521, "Final", "2025-05-22T01:06:48.666" },
            new object[] { "QA45422290251420658", 100, 100, 100, 6, "Ok", "Created", "Started", 50658, 123420, "InitialPrinter", "2025-05-22T01:05:31.318" },

//recipe not found
            //new object[]
            //{
            //    "QA45L823581251303305", 100, 100, 100, 7, "Ok", "Created", "Started", 13305, 28764, "InitialPrinter",
            //    "2025-05-10T10:33:47.0247793"
            //},
            //new object[]
            //{
            //    "QA45L823566251072875", 100, 100, 100, 7, "Ok", "Created", "Started", 12875, 28046, "InitialPrinter",
            //    "2025-04-17T16:12:44.2747589"
            //},

            //new object[]
            //{
            //    "LQA45L823566232372678", 500, 500, 500, 8, "Ok", "InProcess", "Started", 178, 354, "Final",
            //    "2023-08-27T17:19:03"
            //},
            //new object[]
            //{
            //    "LQA45L823566232450199", 100, 100, 100, 8, "Ok", "Created", "Started", 199, 384, "InitialPrinter",
            //    "2023-09-02T17:13:53.2516801"
            //},
            //new object[]
            //{
            //    "LQA45L823581232372604", 100, 100, 100, 8, "Ok", "Created", "Started", 104, 214, "InitialPrinter",
            //    "2023-08-27T03:41:07"
            //},
            //new object[]
            //{
            //    "LQA45L823581232372616", 400, 400, 400, 8, "Ok", "InProcess", "Started", 116, 227, "Process",
            //    "2023-08-27T12:21:09"
            //},
            //new object[]
            //{
            //    "LQA45L823581232452691", 500, 500, 500, 8, "Ok", "Finished", "Started", 193, 392, "Final",
            //    "2023-09-02T18:31:19.3287562"
            //},
            //new object[]
            //{
            //    "LQA45L823566232372664", 400, 400, 400, 8, "Ok", "InProcess", "Started", 164, 323, "Process",
            //    "2023-08-27T09:03:32"
            //},

//destination not valid
            //    new object[] { "L1A422290240740241", 100, 100, 100, 5, "None", "InProcess", "Started", 241, 440, "InitialPrinter", "2024-03-14T18:01:11.3720045" },
            //    new object[] { "QA45432060251303463", 1100, 1100, 1100, 6, "Ok", "InProcess", "Started", 13463, 29399, "InitialPrinter", "2025-05-10T12:56:06.7373807" },
            //    new object[] { "QA45431850251303439", 1100, 1100, 1100, 6, "Ok", "InProcess", "Started", 13439, 29276, "InitialPrinter", "2025-05-10T12:28:00.0781324" },
            //    new object[] { "QA45431610251303407", 1100, 1100, 1100, 6, "Ok", "InProcess", "Started", 13407, 29112, "InitialPrinter", "2025-05-10T11:50:34.4103701" },
            //    new object[] { "QA45422330251303368", 100, 100, 100, 6, "Ok", "InProcess", "Started", 13368, 28928, "InitialPrinter", "2025-05-10T11:10:03.2429310" },
            //    new object[] { "QA45422300251303342", 100, 100, 100, 6, "Ok", "Finished", "Started", 13342, 28849, "InitialPrinter", "2025-05-10T10:57:37.4573707" },
            //    new object[] { "QA45422300242281437", 500, 500, 500, 6, "Ok", "InProcess", "Started", 1437, 3273, "Final", "2024-08-15T13:32:47.7401803" },
            //    new object[] { "QA4500T100251303254", 1100, 1100, 1100, 6, "Ok", "InProcess", "Started", 13254, 28646, "InitialPrinter", "2025-05-10T10:00:23.1605399" },
            //    new object[] { "QA45422290242985643", 500, 500, 500, 6, "Ok", "Finished", "Started", 5643, 12707, "Final", "2024-10-24T11:14:28.7840922" },
            //    new object[] { "QA4500T456251303287", 100, 100, 100, 6, "Ok", "InProcess", "Started", 13287, 28745, "InitialPrinter", "2025-05-10T10:15:52.3182123" },
            //    new object[] { "QA4500T100251303487", 1200, 1200, 1200, 6, "Ok", "InProcess", "Started", 13487, 29511, "Process", "2025-05-10T16:40:14.5379034" },
            };
    };
}
