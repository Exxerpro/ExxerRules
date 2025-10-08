namespace IndTrace.DataStore.Services.OEE.Interfaces
{
    /// <summary>
    /// Defines a contract for a service that processes OEE register data reactively.
    /// </summary>
    public interface IReactiveEventService
    {
        /// <summary>
        /// Processes OEE register data asynchronously.
        /// </summary>
        /// <param name="stoppingToken">A token to observe for cancellation.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task ProcessOeeRegisterAsync(CancellationToken stoppingToken);
    }
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IReactiveEventService logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
