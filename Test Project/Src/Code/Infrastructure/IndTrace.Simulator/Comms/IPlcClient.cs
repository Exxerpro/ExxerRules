// <copyright file="IPlcClient.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Comms;

using IndTrace.DataStore.Models;
using IndTrace.DataStore.ModelsComs;
using IndTrace.Simulator.Simulation;

/// <summary>
/// Defines methods for interacting with a PLC client for simulation and tag operations.
/// </summary>
public interface IPlcClient
{
    /// <summary>
    /// Executes a command simulation on the PLC for the specified product and step task.
    /// </summary>
    /// <param name="product">The product to simulate.</param>
    /// <param name="tags">The dictionary of PLC tags to use.</param>
    /// <param name="barcode">The barcode for the simulation.</param>
    /// <param name="stepTask">The execution step task to perform.</param>
    /// <param name="machineId">The machine identifier.</param>
    /// <param name="options">The dry run options for simulation.</param>
    /// <returns>A task that returns the resulting barcode string.</returns>
    Task<string> ExecuteCommandSimulation(Product product, Dictionary<string, VariableS7> tags, string barcode, ExecutionStepTask stepTask, int machineId, DryRunOptions options);

    /// <summary>
    /// Writes the specified tags asynchronously to the PLC.
    /// </summary>
    /// <param name="tags">The dictionary of tags to write.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteTagsAsync(Dictionary<string, VariableS7> tags);

    /// <summary>
    /// Initializes the PLC connection asynchronously.
    /// </summary>
    /// <param name="ipAddress">The IP address of the PLC.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task InitializeConnection(string ipAddress, CancellationToken cancellationToken);

    /// <summary>
    /// Reads a tag asynchronously from the PLC.
    /// </summary>
    /// <param name="tags">The dictionary of tags to read.</param>
    /// <returns>A task that returns the integer value read from the PLC.</returns>
    Task<int> ReadTagAsync(Dictionary<string, VariableS7> tags);

    /// <summary>
    /// Reads a list of integer tags asynchronously from the PLC for the specified PLC ID.
    /// </summary>
    /// <typeparam name="T">The type parameter for the tag values.</typeparam>
    /// <param name="tagsToRead">The dictionary of tags to read.</param>
    /// <param name="plcId">The PLC identifier.</param>
    /// <returns>A task that returns a dictionary of tag names and their values.</returns>
    Task<Dictionary<string, VariableS7>> ReadListIntTagsAsync<T>(Dictionary<string, VariableS7> tagsToRead, int plcId);

    /// <summary>
    /// Clears the specified tags asynchronously on the PLC.
    /// </summary>
    /// <param name="tags">The dictionary of tags to clear.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ClearTagsAsync(Dictionary<string, VariableS7> tags);
}
