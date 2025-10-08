namespace IndTrace.DataStore.Models;

/// <summary>
/// Represents options for configuring dry run simulations.
/// </summary>
public class DryRunOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the run is a dry run.
    /// </summary>
    public bool IsDryRun { get; set; } = true;
    /// <summary>
    /// Gets or sets the number of runs to perform.
    /// </summary>
    public int NumberOfRun { get; set; } = 5;
    /// <summary>
    /// Gets or sets the delay in milliseconds between cycles.
    /// </summary>
    public int CycleDelayMs { get; set; } = 1000;
    /// <summary>
    /// Gets or sets the delay in milliseconds between commands.
    /// </summary>
    public int CommandDelayMs { get; set; } = 1000;
    /// <summary>
    /// Gets or sets the delay time in milliseconds between machines.
    /// </summary>
    public int DelayTimeBetweenMachines { get; set; } = 1000;
    /// <summary>
    /// Gets or sets the average cycle time in milliseconds.
    /// </summary>
    public int CycleTimeAverageTimeMs { get; set; } = 5000;
    /// <summary>
    /// Gets or sets the maximum number of retries.
    /// </summary>
    public int MaxNumberOfRetry { get; set; } = 3;
    /// <summary>
    /// Gets or sets the wait time for a response in milliseconds.
    /// </summary>
    public int WaitTimeForResponse { get; set; } = 5000;
    /// <summary>
    /// Gets or sets the delay time in milliseconds between retries.
    /// </summary>
    public int DelayTimeBetweenRetries { get; set; } = 5000;

    /// <summary>
    /// Returns a string representation of the dry run options.
    /// </summary>
    /// <returns>A string describing the dry run options.</returns>
    public override string ToString()
    {
        return $"DryRunOptions:\n" +
               $"- IsDryRun: {this.IsDryRun}\n" +
               $"- NumberOfRun: {this.NumberOfRun}\n" +
               $"- CycleDelayMs: {this.CycleDelayMs} ms\n" +
               $"- CommandDelayMs: {this.CommandDelayMs} ms\n" +
               $"- DelayTimeBetweenMachines: {this.DelayTimeBetweenMachines} ms\n" +
               $"- CycleTimeAverageTimeMs: {this.CycleTimeAverageTimeMs} ms\n" +
               $"- MaxNumberOfRetry: {this.MaxNumberOfRetry}\n" +
               $"- WaitTimeForResponse: {this.WaitTimeForResponse} ms\n" +
               $"- DelayTimeBetweenRetries: {this.DelayTimeBetweenRetries} ms";
    }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate dry run options logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
