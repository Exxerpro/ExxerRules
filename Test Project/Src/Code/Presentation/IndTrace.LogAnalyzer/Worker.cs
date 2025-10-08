using IndTrace.Dependencies.Interceptors;

namespace IndTrace.Communications
{
    /// <summary>
    /// Defines the contract for worker services that perform timed operations.
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// Performs work for the specified duration with execution time measurement.
        /// </summary>
        /// <param name="timeInSeconds">The duration of work to perform in seconds.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [MeasureExecutionTime("Important Logic")]
        Task DoSomeWork(int timeInSeconds);
    }

    /// <summary>
    /// Implements a background service worker that performs continuous operations with random delays.
    /// </summary>
    public class Worker : BackgroundService, IWorker
    {
        private readonly ILogger<Worker> _logger;

        /// <summary>
        /// Initializes a new instance of the Worker class with the specified logger.
        /// </summary>
        /// <param name="logger">The logger instance for recording operations.</param>
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Executes the background service work continuously until cancellation is requested.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token to stop the service.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var random = new Random(323);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                // Simulate some work with a random delay
                await DoSomeWork(random.Next(1000));
            }
        }

        /// <summary>
        /// Performs work for the specified duration with execution time measurement.
        /// </summary>
        /// <param name="timeInSeconds">The duration of work to perform in seconds.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [MeasureExecutionTime("Important Logic")]
        public async Task DoSomeWork(int timeInSeconds)
        {
            await Task.Delay(timeInSeconds * 1000);
            _logger.LogInformation("Work done after {time} seconds", timeInSeconds);
        }
    }
}
