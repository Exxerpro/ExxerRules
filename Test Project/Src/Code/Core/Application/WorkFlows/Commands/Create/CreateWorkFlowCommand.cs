// <copyright file="CreateWorkFlowCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.WorkFlows.Commands.Create;

/// <summary>
/// Represents the CreateWorkFlowCommand.
/// </summary>
public class CreateWorkFlowCommand : IMonitorRequest<WorkFlowCreatedEvent>
{
    /// <summary>
    /// Gets or sets the WorkFlowId.
    /// </summary>
    public int WorkFlowId { get; set; }

    /// <summary>
    /// Gets or sets the ProductId.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the LastMachineId.
    /// </summary>
    public int LastMachineId { get; set; }

    /// <summary>
    /// Gets or sets the NextMachineId.
    /// </summary>
    public int NextMachineId { get; set; }
}
