namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Defines a method for exporting fixture context data.
/// </summary>
public interface IFixtureExporter
{
    /// <summary>
    /// Exports the specified fixture context asynchronously.
    /// </summary>
    /// <param name="context">The fixture context to export.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IFixtureExporter logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    Task ExportAsync(IFixtureContext context);
}
