// <copyright file="Register.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a register entry for a machine variable, including value, data type, and status information.
/// </summary>
public class Register : IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the register.
    /// </summary>
    public int RegisterId { get; set; }

    /// <summary>
    /// Gets or sets the name of the register.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the register.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the machine identifier associated with the register.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the variable identifier associated with the register.
    /// </summary>
    public int VariableId { get; set; }

    /// <summary>
    /// Gets or sets the cycle identifier associated with the register.
    /// </summary>
    public int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the value of the register.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data type of the register value.
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status value identifier associated with the register.
    /// </summary>
    public int StatusValueId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for the register entry.
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Returns a string representation of the Register.
    /// </summary>
    /// <returns>A string containing the register ID, name, and value.</returns>
    // [Fix]
    // CLAUDE
    // Date: 23/08/2025
    // Reason: Added ToString() implementation for better debugging and logging experience
    public override string ToString() => $"Register {this.RegisterId}: {this.Name} = {this.Value}";
}
