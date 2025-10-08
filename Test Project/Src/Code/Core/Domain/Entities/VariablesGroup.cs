// <copyright file="VariablesGroup.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a variables group lookup entity for organizing PLC variables.
/// </summary>
public class VariablesGroup : ILookupEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VariablesGroup"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public VariablesGroup()
    {
        this.VariableGroupName = string.Empty;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the variable group.
    /// </summary>
    public int VariableGroupId { get; set; }

    /// <summary>
    /// Gets or sets the name of the variable group.
    /// </summary>
    public string VariableGroupName { get; set; }
}
