namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Defines methods for storing and logging fixture data and results.
/// </summary>
public interface IFixtureStore
{
    /// <summary>
    /// Logs a fixture result asynchronously.
    /// </summary>
    /// <param name="entry">The fixture log entry to log.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LogResultAsync(IFixtureLogEntry entry);

    /// <summary>
    /// Saves the fixture context asynchronously with an optional retry count.
    /// </summary>
    /// <param name="context">The fixture context to save.</param>
    /// <param name="pathType">The path type for saving.</param>
    /// <param name="retryCount">The optional retry count.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(IFixtureContext context, string pathType, int? retryCount = null);

    /// <summary>
    /// Saves the fixture context asynchronously.
    /// </summary>
    /// <param name="context">The fixture context to save.</param>
    /// <param name="pathType">The path type for saving.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(IFixtureContext context, string pathType);
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IFixtureStore logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
