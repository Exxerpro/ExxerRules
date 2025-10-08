// <copyright file="Order.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents an order entity, including machine, operator, product, and timing details.
/// </summary>
public class Order : IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the order.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier associated with the order.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the operator identifier for the order.
    /// </summary>
    public int OperatorId { get; set; }

    /// <summary>
    /// Gets or sets the leader identifier for the order.
    /// </summary>
    public int LeaderId { get; set; }

    /// <summary>
    /// Gets or sets the programmer identifier for the order.
    /// </summary>
    public int ProgrammerId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier for the order.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the tooling identifier for the order.
    /// </summary>
    public int ToolingId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for the order.
    /// </summary>
    public int TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the size of the order.
    /// </summary>
    public int OrderSize { get; set; }

    /// <summary>
    /// Gets or sets the order time.
    /// </summary>
    public int OrderTime { get; set; }

    /// <summary>
    /// Gets or sets the start date and time of the order.
    /// </summary>
    public DateTime OrderStart { get; set; }

    /// <summary>
    /// Gets or sets the end date and time of the order.
    /// </summary>
    public DateTime OrderEnd { get; set; }

    /// <summary>
    /// Gets or sets the results identifier for the order.
    /// </summary>
    public int ResultsId { get; set; }

    /// <summary>
    /// Returns a string representation of the Order.
    /// </summary>
    /// <returns>A string containing the order ID, machine ID, and order start time.</returns>
    // [Fix]
    // CLAUDE
    // Date: 23/08/2025
    // Reason: Added ToString() implementation for better debugging and logging experience
    public override string ToString() => $"Order {this.OrderId} (Machine {this.MachineId}): Started {this.OrderStart:yyyy-MM-dd HH:mm}";
}
