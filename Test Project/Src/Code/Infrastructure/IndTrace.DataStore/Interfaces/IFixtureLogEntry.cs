namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Represents a log entry for a fixture operation.
/// </summary>
public interface IFixtureLogEntry : IEquatable<IFixtureLogEntry?>
{
    /// <summary>
    /// Gets the part number.
    /// </summary>
    string PartNumber { get; init; }
    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    int ProductId { get; init; }
    /// <summary>
    /// Gets the station identifier.
    /// </summary>
    int StationId { get; init; }
    /// <summary>
    /// Gets the task name.
    /// </summary>
    string TaskName { get; init; }
    /// <summary>
    /// Gets the barcode.
    /// </summary>
    string Barcode { get; init; }
    /// <summary>
    /// Gets the state of the fixture.
    /// </summary>
    string State { get; init; }
    /// <summary>
    /// Gets the timestamp of the log entry.
    /// </summary>
    DateTime Timestamp { get; init; }
    /// <summary>
    /// Gets the result of the operation.
    /// </summary>
    string Result { get; init; }
    /// <summary>
    /// Gets the retry count for the operation.
    /// </summary>
    int RetryCount { get; init; }
    /// <summary>
    /// Gets any notes associated with the log entry.
    /// </summary>
    string Notes { get; init; }

    /// <summary>
    /// Determines whether the specified log entry is equal to the current log entry.
    /// </summary>
    /// <param name="other">The log entry to compare with the current log entry.</param>
    /// <returns>True if equal, otherwise false.</returns>
    new bool Equals(IFixtureLogEntry? other);

    /// <summary>
    /// Determines whether the specified object is equal to the current log entry.
    /// </summary>
    /// <param name="other">The object to compare with the current log entry.</param>
    /// <returns>True if equal, otherwise false.</returns>
    bool Equals(object? other);

    /// <summary>
    /// Returns a hash code for the log entry.
    /// </summary>
    /// <returns>A hash code for the log entry.</returns>
    int GetHashCode();

    /// <summary>
    /// Deconstructs the log entry into its components.
    /// </summary>
    /// <param name="PartNumber">The part number.</param>
    /// <param name="ProductId">The product identifier.</param>
    /// <param name="StationId">The station identifier.</param>
    /// <param name="TaskName">The task name.</param>
    /// <param name="Barcode">The barcode.</param>
    /// <param name="State">The state of the fixture.</param>
    /// <param name="Timestamp">The timestamp of the log entry.</param>
    /// <param name="Result">The result of the operation.</param>
    /// <param name="RetryCount">The retry count for the operation.</param>
    /// <param name="Notes">Any notes associated with the log entry.</param>
    void Deconstruct(out string PartNumber, out int ProductId, out int StationId, out string TaskName, out string Barcode, out string State, out DateTime Timestamp, out string Result, out int RetryCount, out string Notes);

    /// <summary>
    /// Returns a string representation of the log entry.
    /// </summary>
    /// <returns>A string describing the log entry.</returns>
    string ToString();
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IFixtureLogEntry logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
