// <copyright file="Variable.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

/// <summary>
/// Represents a variable entity, including PLC, addressing, and validation details.
/// </summary>
public class Variable : AuditableEntity, IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the variable.
    /// </summary>
    public int VariableId { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier associated with the variable.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the PLC identifier associated with the variable.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Gets or sets the name of the variable.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the variable.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alias for the variable.
    /// </summary>
    public string Alias { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the address of the variable in the PLC.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the network type of the variable.
    /// </summary>
    public string NetType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the length of the variable.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the variable is active.
    /// </summary>
    public int IsActive { get; set; }

    /// <summary>
    /// Gets or sets the direction of the variable (input/output).
    /// </summary>
    public int Direction { get; set; }

    /// <summary>
    /// Gets or sets the variable group identifier.
    /// </summary>
    public int VariableGroupId { get; set; }

    /// <summary>
    /// Gets or sets the variable specification identifier, if any.
    /// </summary>
    public int? VariableSpecId { get; set; }

    /// <summary>
    /// Gets or sets the tag status of the variable.
    /// </summary>
    public int TagStatus { get; set; }

    /// <summary>
    /// Gets or sets the native type of the variable.
    /// </summary>
    public string NativeType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value of the variable.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the native address of the variable.
    /// </summary>
    public string NativeAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the variable has been validated on the PLC.
    /// </summary>
    public bool? Validated { get; set; }
}
