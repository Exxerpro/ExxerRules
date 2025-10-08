using Xunit.Runner.Common;

namespace IndTrace.Aggregation.BoundedTests.Services
{
    public class Sample
    {
        public Sample(ILogger<Sample> logger)
        {
            logger.LogInformation("Sample class instantiated");
        }

        public async Task<int> Execute()
        {
            await Task.Delay(1);
            return 3;
        }
    }
}
