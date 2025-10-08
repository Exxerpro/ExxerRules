// <copyright file="VariableRecord.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.S7Monitor;

/// <summary>
/// Represents a record for a PLC variable, including its address, value, and metadata.
/// </summary>
public class VariableRecord
{
    private object value = new();
    private volatile int valueUpdated;

    /// <summary>
    /// Gets the address of the variable.
    /// </summary>
    public string Address { get; init; } = string.Empty;

    /// <summary>
    /// Gets the PLC ID associated with this variable.
    /// </summary>
    public int PlcId { get; init; }

    /// <summary>
    /// Gets the description of this variable.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the row index of the variable in a display or collection.
    /// </summary>
    public int RowIdx { get; init; }

    /// <summary>
    /// Gets or sets the current value of the variable, with automatic change detection.
    /// </summary>
    public object Value
    {
        get => this.value;
        set
        {
            if (!this.IsEquivalent(this.value, value))
            {
                this.value = value;
                this.valueUpdated = 1;
            }
        }
    }

    /// <summary>
    /// Determines whether two values are equivalent, with special handling for byte arrays.
    /// </summary>
    /// <param name="oldValue">The old value to compare.</param>
    /// <param name="newValue">The new value to compare.</param>
    /// <returns><c>true</c> if the values are equivalent; otherwise, <c>false</c>.</returns>
    private bool IsEquivalent(object oldValue, object newValue)
    {
        // Special treatmant for byte arrays
        if (oldValue is byte[] oldArray && newValue is byte[] newArray)
        {
            if (oldArray.Length != newArray.Length)
            {
                return false;
            }

            for (var i = 0; i < oldArray.Length; i++)
            {
                if (oldArray[i] != newArray[i])
                {
                    return false;
                }
            }

            return true;
        }

        // all other types read from PLC have value compare semantics.
        return oldValue == newValue;
    }

    /// <summary>
    /// Checks if the value has been updated and retrieves the new value if so.
    /// </summary>
    /// <param name="newValue">The new value if updated; otherwise, null.</param>
    /// <returns>True if the value was updated; otherwise, false.</returns>
    public bool HasUpdate([NotNullWhen(true)] out object? newValue)
    {
        if (Interlocked.Exchange(ref this.valueUpdated, 0) == 1)
        {
            newValue = this.value;
            return true;
        }

        newValue = null;
        return false;
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate variable record logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
