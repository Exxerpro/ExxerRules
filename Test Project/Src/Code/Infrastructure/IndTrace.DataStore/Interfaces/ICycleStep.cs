namespace IndTrace.DataStore.Interfaces;

/// <summary>
/// Represents a cycle step interface with name, command, and state.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate ICycleStep logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface ICycleStep
{
    /// <summary>
    /// Gets the name of the cycle step.
    /// </summary>
    string Name { get; init; }
    /// <summary>
    /// Gets the command associated with the cycle step.
    /// </summary>
    int Command { get; init; }
    /// <summary>
    /// Gets the state of the cycle step.
    /// </summary>
    string State { get; init; }
    /// <summary>
    /// Determines whether the specified ICycleStep is equal to the current ICycleStep.
    /// </summary>
    /// <param name="other">The ICycleStep to compare with the current ICycleStep.</param>
    /// <returns>True if equal, otherwise false.</returns>
    bool Equals(ICycleStep? other);
    /// <summary>
    /// Determines whether the specified object is equal to the current ICycleStep.
    /// </summary>
    /// <param name="other">The object to compare with the current ICycleStep.</param>
    /// <returns>True if equal, otherwise false.</returns>
    bool Equals(object? other);
    /// <summary>
    /// Returns a hash code for the ICycleStep.
    /// </summary>
    /// <returns>A hash code for the ICycleStep.</returns>
    int GetHashCode();
    /// <summary>
    /// Deconstructs the cycle step into its components.
    /// </summary>
    /// <param name="Name">The name of the cycle step.</param>
    /// <param name="Command">The command associated with the cycle step.</param>
    /// <param name="State">The state of the cycle step.</param>
    void Deconstruct(out string Name, out int Command, out string State);
    /// <summary>
    /// Returns a string representation of the cycle step.
    /// </summary>
    /// <returns>A string describing the cycle step.</returns>
    string ToString();
}
