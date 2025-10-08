// <copyright file="CreateVariableCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Variables.Commands.Create;

/// <summary>
/// Represents the CreateVariableCommand.
/// </summary>
public class CreateVariableCommand : IMonitorRequest<VariableCreatedEvent>
{
    /// <summary>
    /// Gets or sets the EntitieId.
    /// </summary>
    public int VariableId { get; set; }

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the PLC identifier.
    /// </summary>
    public string Plc { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variable name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variable address.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variable type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Length.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Gets or sets the Event.
    /// </summary>
    public int Event { get; set; }

    /// <summary>
    /// Gets or sets the Direction.
    /// </summary>
    public int Direction { get; set; }

    /// <summary>
    /// Gets or sets the VariableGroupId.
    /// </summary>
    public int VariableGroupId { get; set; }

    /// <summary>
    /// Gets or sets the model.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the transaction.
    /// </summary>
    public string Transaction { get; set; } = string.Empty;
}
