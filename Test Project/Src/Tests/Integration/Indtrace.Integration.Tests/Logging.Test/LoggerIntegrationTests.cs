namespace Integration.Tests.Logging.Test
{
    /// <summary>
    /// Represents the LoggerIntegrationTests.
    /// </summary>
    public class LoggerIntegrationTests : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
    {
        private readonly IServiceProvider _services;
        private readonly ITestOutputHelper _output;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fixture">The test host fixture.</param>
        /// <param name="output">The output.</param>

        public LoggerIntegrationTests(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output)
        {
            _services = fixture.Services;
            _output = output;
        }
        /// <summary>
        /// Executes Should_Log_Message operation.
        /// </summary>
        /// <returns>The result of Should_Log_Message.</returns>

        [Fact]
        public async Task Should_Log_Message()
        {
            using var scope = _services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<LoggerIntegrationTests>>();
            logger.LogInformation("✅ This should now appear in test output.");
            await Task.CompletedTask;
        }
    }
}
