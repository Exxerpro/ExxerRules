// <copyright file="FixtureContext.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models;

using System.Diagnostics.CodeAnalysis;
using IndTrace.DataStore.Interfaces;
using IndTrace.DataStore.ModelsComs;

/// <summary>
/// Represents the context for a fixture simulation, including part, barcode, machine, and task information.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate fixture context logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the FixtureContext.
/// </summary>
public class FixtureContext : IFixtureContext
{
    /// <summary>
    /// Gets or sets the part number for the fixture.
    /// </summary>
    public string PartNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the barcode for the fixture.
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the machine identifier.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the expected state for the fixture.
    /// </summary>
    public string ExpectedState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the current task.
    /// </summary>
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tags associated with the fixture.
    /// </summary>
    public Dictionary<string, VariableS7> Tags { get; set; } = [];

    /// <summary>
    /// Gets or sets the current cycle step.
    /// </summary>
    public ICycleStep CycleStep { get; set; } = new CycleStep();

    /// <summary>
    /// Gets or sets the list of tasks (cycle steps) for the fixture.
    /// </summary>
    public List<ICycleStep> Tasks { get; set; } = [];

    public override string ToString()
        => $"Part={this.PartNumber}, Barcode={this.Barcode}, ProductId={this.ProductId}, MachineId={this.MachineId}, Task={this.TaskName}";

    [return: NotNull]
    string IFixtureContext.ToString() => this.ToString();
}

/// <summary>
/// Represents a single cycle step in a fixture simulation.
/// </summary>
public class CycleStep : ICycleStep
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CycleStep"/> class.
    /// </summary>
    public CycleStep()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CycleStep"/> class with the specified name, command, and state.
    /// </summary>
    /// <param name="name">The name of the cycle step.</param>
    /// <param name="command">The command code for the cycle step.</param>
    /// <param name="state">The state for the cycle step.</param>
    public CycleStep(string name, int command, string state)
    {
        this.Name = name;
        this.Command = command;
        this.State = state;
    }

    /// <summary>
    /// Gets the name of the cycle step.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the command code for the cycle step.
    /// </summary>
    public int Command { get; init; }

    /// <summary>
    /// Gets the state for the cycle step.
    /// </summary>
    public string State { get; init; } = string.Empty;

    public override string ToString() => $"{this.Name}:{this.Command}:{this.State}";

    [return: NotNull]
    string ICycleStep.ToString() => this.ToString();

    /// <inheritdoc/>
    public bool Equals(ICycleStep? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Name == other.Name &&
               this.Command == other.Command &&
               this.State == other.State;
    }

    /// <inheritdoc/>
    public void Deconstruct(out string Name, out int Command, out string State)
    {
        throw new NotImplementedException();
    }
}
