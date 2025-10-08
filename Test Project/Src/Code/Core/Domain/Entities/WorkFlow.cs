// <copyright file="WorkFlow.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

/// <summary>
/// Represents a workflow entity with configuration and status information.
/// </summary>
public class WorkFlow : AuditableEntity, IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the workflow.
    /// </summary>
    public int WorkFlowId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier associated with this workflow.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the next machine identifier in the workflow sequence.
    /// </summary>
    public int NextMachineId { get; set; }

    /// <summary>
    /// Gets or sets the last machine identifier in the workflow sequence.
    /// </summary>
    public int LastMachineId { get; set; }

    /// <summary>
    /// Gets or sets the collection of machines in this workflow.
    /// </summary>
    public List<Machine> Machine { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of edges defining connections in this workflow.
    /// </summary>
    public List<Edge> Edges { get; set; } = [];

    /// <summary>
    /// Adds a machine node to the workflow graph.
    /// </summary>
    /// <param name="node">The machine to add to the workflow.</param>
    public void AddNode(Machine node) => this.Machine.Add(node);

    /// <summary>
    /// Adds an edge to the workflow graph.
    /// </summary>
    /// <param name="edge">The edge to add to the workflow.</param>
    public void AddEdge(Edge edge) => this.Edges.Add(edge);

    /// <summary>
    /// Gets or sets the rule identifier associated with this workflow.
    /// </summary>
    public int RuleId { get; set; } = default(int);
}
