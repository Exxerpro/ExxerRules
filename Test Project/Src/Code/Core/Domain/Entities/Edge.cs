// <copyright file="Edge.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Entities;

using IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a connection (edge) between two machines in a production graph, including weight and navigation properties.
/// </summary>
public class Edge : IEntityRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the edge.
    /// </summary>
    public int EdgeId { get; set; }

    /// <summary>
    /// Gets or sets the source machine of the edge.
    /// </summary>
    public Machine FromMachine { get; set; }

    /// <summary>
    /// Gets or sets the destination machine of the edge.
    /// </summary>
    public Machine ToMachine { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the source machine (foreign key).
    /// </summary>
    public int FromMachineId { get; set; } // Foreign Key

    /// <summary>
    /// Gets or sets the identifier of the destination machine (foreign key).
    /// </summary>
    public int ToMachineId { get; set; } // Foreign Key

    /// <summary>
    /// Gets or sets the weight of the edge.
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Edge"/> class.
    /// </summary>
    public Edge()
    {
        this.FromMachine = new Machine();
        this.ToMachine = new Machine();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Edge"/> class with specified machines and weight.
    /// </summary>
    /// <param name="fromMachine">The source machine.</param>
    /// <param name="toMachine">The destination machine.</param>
    /// <param name="weight">The weight of the edge.</param>
    public Edge(Machine fromMachine, Machine toMachine, int weight)
    {
        this.FromMachine = fromMachine;
        this.ToMachine = toMachine;
        this.Weight = weight;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Edge"/> class with specified machines and a default weight of 1.
    /// </summary>
    /// <param name="fromMachine">The source machine.</param>
    /// <param name="toMachine">The destination machine.</param>
    public Edge(Machine fromMachine, Machine toMachine)
    {
        this.FromMachine = fromMachine;
        this.ToMachine = toMachine;
        this.Weight = 1;
    }

    /// <summary>
    /// Determines whether two edges are equal.
    /// </summary>
    /// <param name="left">The first edge to compare.</param>
    /// <param name="right">The second edge to compare.</param>
    public static bool operator ==(Edge left, Edge right) => left.Equals(right);

    /// <summary>
    /// Determines whether two edges are not equal.
    /// </summary>
    /// <param name="left">The first edge to compare.</param>
    /// <param name="right">The second edge to compare.</param>
    public static bool operator !=(Edge left, Edge right) => !(left == right);

    /// <summary>
    /// Returns a string representation of the edge.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{this.FromMachine} -> {this.ToMachine}";

    /// <summary>
    /// Determines whether the specified object is equal to the current edge.
    /// </summary>
    /// <param name="obj">The object to compare with the current edge.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is Edge edge)
        {
            return edge.FromMachine == this.FromMachine && edge.ToMachine == this.ToMachine;
        }

        return false;
    }

    /// <summary>
    /// Returns a hash code for the edge.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => (From: this.FromMachine, To: this.ToMachine).GetHashCode();
}
