using IndTrace.DataStore.Interfaces;

namespace IndTrace.DataStore.IModelsComs;

/// <summary>
/// Provides methods for writing test tags and cycle steps to a fixture context.
/// </summary>
public interface ITestTagWriter
{
    /// <summary>
    /// Writes a cycle step asynchronously to the fixture context.
    /// </summary>
    /// <param name="context">The fixture context to write to.</param>
    /// <param name="step">The cycle step to write.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteCycleStepAsync(IFixtureContext context, ICycleStep step);

    /// <summary>
    /// Writes an invalid command asynchronously to the fixture context.
    /// </summary>
    /// <param name="context">The fixture context to write to.</param>
    /// <param name="step">The cycle step associated with the invalid command.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteInvalidCommandAsync(IFixtureContext context, ICycleStep step);

    /// <summary>
    /// Writes a faulty command and then recovers asynchronously in the fixture context.
    /// </summary>
    /// <param name="context">The fixture context to write to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteFaultyThenRecoverAsync(IFixtureContext context);

    /// <summary>
    /// Writes only the cycle start asynchronously to the fixture context.
    /// </summary>
    /// <param name="context">The fixture context to write to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteCycleStartOnlyAsync(IFixtureContext context);
}
