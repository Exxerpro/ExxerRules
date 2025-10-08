using IndTrace.DataStore.DataAccess;
using Microsoft.Extensions.Options;
using IndTrace.DataStore.ModelsComs;
using IndTrace.Domain.Enum;
using IndTrace.OEE.Infrastructure.Services;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace IndTrace.Oee.Tests
{
    /// <summary>
    /// Represents the PlcDbTagsRepositoryIntegrationTests.
    /// </summary>
    public class PlcDbTagsRepositoryIntegrationTests : IAsyncLifetime
    {
        private const string ConnectionString = "Server=DESKTOP-FB2ES22\\SQL2022;Database=IndTraceDataQA45;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";
        private SqlConnection _connection;
        private PlcDbTagsRepository _repository;
        private ILogger<PlcDbTagsRepository> _logger;

        private readonly ITestOutputHelper output;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="output">The output.</param>

        public PlcDbTagsRepositoryIntegrationTests(ITestOutputHelper output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output), "Test output cannot be null.");

            var options = Options.Create(new PlcDbOptions
            {
                ConnectionString = ConnectionString
            });

            _connection = new SqlConnection(ConnectionString);
            _logger = XUnitLogger.CreateLogger<PlcDbTagsRepository>(this.output);
            _repository = new PlcDbTagsRepository(options, _logger, _connection);
        }
        /// <summary>
        /// Executes InitializeAsync operation.
        /// </summary>
        /// <returns>The result of InitializeAsync.</returns>

        public async ValueTask InitializeAsync()
        {
            await _connection.OpenAsync();
        }
        /// <summary>
        /// Executes DisposeAsync operation.
        /// </summary>
        /// <returns>The result of DisposeAsync.</returns>

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
        /// <summary>
        /// Executes GetPlcsAsync_ShouldReturnPlcData operation.
        /// </summary>
        /// <returns>The result of GetPlcsAsync_ShouldReturnPlcData.</returns>

        [Fact]
        public async Task GetPlcsAsync_ShouldReturnPlcData()
        {
            // Act
            var result = await _repository.GetPlcsAsync(TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeAssignableTo<IEnumerable<PlcData>>();
        }
        /// <summary>
        /// Executes GetPlcsAsync_ShouldReturnCompleteDataForPLC100PlcData operation.
        /// </summary>
        /// <returns>The result of GetPlcsAsync_ShouldReturnCompleteDataForPLC100PlcData.</returns>

        [Fact]
        public async Task GetPlcsAsync_ShouldReturnCompleteDataForPLC100PlcData()
        {
            // Act
            var result = await _repository.GetPlcsAsync(TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeAssignableTo<IEnumerable<PlcData>>();

            var plc = result.FirstOrDefault(x => x.PlcId == 100);
            plc.ShouldNotBeNull();

            plc.PlcId.ShouldBe(100);
            plc.Enabled.ShouldBe(1); // or the expected value
            plc.MachineId.ShouldBeGreaterThan(0); // or the expected value
            plc.Name.ShouldNotBeNullOrWhiteSpace();
            plc.IpAddress.ShouldNotBeNullOrWhiteSpace();
            plc.PlcType.ShouldNotBeNullOrWhiteSpace();
            plc.PlcBrand.ShouldNotBeNullOrWhiteSpace();
            plc.RackNumber.ShouldBeGreaterThanOrEqualTo(0);
            plc.CpuMpiAddress.ShouldBeGreaterThanOrEqualTo(0);
            plc.Port.ShouldBeGreaterThan(0);
        }
        /// <summary>
        /// Executes GetDistinctDbInfosAsync_ShouldReturnDbList operation.
        /// </summary>
        /// <returns>The result of GetDistinctDbInfosAsync_ShouldReturnDbList.</returns>

        [Fact]
        public async Task GetDistinctDbInfosAsync_ShouldReturnDbList()
        {
            // Act
            var result = await _repository.GetDistinctDbInfosAsync(TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeAssignableTo<IEnumerable<Db>>();
        }
        /// <summary>
        /// Executes GetTagsGroupedByMachineAsync_NoArgs_ShouldReturnDictionary operation.
        /// </summary>
        /// <returns>The result of GetTagsGroupedByMachineAsync_NoArgs_ShouldReturnDictionary.</returns>

        [Fact]
        public async Task GetTagsGroupedByMachineAsync_NoArgs_ShouldReturnDictionary()
        {
            // Act
            var result = await _repository.GetTagsGroupedByMachineAsync(TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeAssignableTo<Dictionary<int, Dictionary<string, VariableS7>>>();
        }
        /// <summary>
        /// Executes GetTagsAsync_WithMachineId_ShouldReturnDictionary operation.
        /// </summary>
        /// <returns>The result of GetTagsAsync_WithMachineId_ShouldReturnDictionary.</returns>

        [Fact]
        public async Task GetTagsAsync_WithMachineId_ShouldReturnDictionary()
        {
            // Arrange
            // Use a valid MachineId from your test database, e.g. 1
            int machineId = 100;

            // Act
            var result = await _repository.GetTagsAsync(machineId, TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeAssignableTo<Dictionary<string, VariableS7>>();
        }
        /// <summary>
        /// Executes GetTagsAsync_WithMachineId_ShouldReturnDictionaryAndHaveMembers operation.
        /// </summary>
        /// <returns>The result of GetTagsAsync_WithMachineId_ShouldReturnDictionaryAndHaveMembers.</returns>

        [Fact]
        public async Task GetTagsAsync_WithMachineId_ShouldReturnDictionaryAndHaveMembers()
        {
            // Arrange
            // Use a valid MachineId from your test database, e.g. 1
            int machineId = 100;

            // Act
            var result = await _repository.GetTagsAsync(machineId, TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeAssignableTo<Dictionary<string, VariableS7>>();
        }
        /// <summary>
        /// Executes GetTagsAsync_WithMachineId100_ShouldReturnDictionaryAndHaveMembers operation.
        /// </summary>
        /// <returns>The result of GetTagsAsync_WithMachineId100_ShouldReturnDictionaryAndHaveMembers.</returns>

        [Fact]
        public async Task GetTagsAsync_WithMachineId100_ShouldReturnDictionaryAndHaveMembers()
        {
            // Arrange
            IEnumerable<int> machines = [100, 400, 500];
            var groupId = TagsGroups.PerformanceTags.Value;

            // Act
            var result = await _repository.GetTagsGroupedByMachineAsync(machines, groupId, TestContext.Current.CancellationToken);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeAssignableTo<Dictionary<int, Dictionary<string, VariableS7>>>();

            // Ensure key exists before accessing
            result.ContainsKey(100).ShouldBeTrue("Result should contain key 100");
            var tagsFor100 = result[100];

            foreach (var key in tagsFor100.Keys)
            {
                _logger.LogInformation("TagDataStore key for machine 100: {Key}", key);
            }
            tagsFor100.ShouldNotBeNull();
            tagsFor100.Count.ShouldBeGreaterThan(0, "Tags dictionary for machine 100 should have members");

            // Log each key
        }
    }
}
