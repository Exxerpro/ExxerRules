// <copyright file="S7PlcClient.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Comms;

using System.Diagnostics;
using System.Reactive.Linq;
using IndTrace.DataStore.Interfaces;
using IndTrace.DataStore.Models;
using IndTrace.DataStore.ModelsComs;
using IndTrace.Simulator.Simulation;
using Sharp7.Rx;
using Sharp7.Rx.Enums;

/// <summary>
/// Represents the S7PlcClient.
/// </summary>
public class S7PlcClient(ILogger<S7PlcClient> logger, IFixtureDb fixtureDb) : IPlcClient
{
    private Sharp7Plc? plc; // Initialized in InitializeConnection
    private Sharp7.Rx.Enums.ConnectionState lastConnectionState;

    private Dictionary<int, PlcRecordDb> IpAddress { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the IsConnected.
    /// </summary>
    public bool IsConnected { get; set; }

    private int lastPlcContacted;

    /// <summary>
    /// Executes WriteTagsAsync operation.
    /// </summary>
    /// <param name="Dictionary<string">The Dictionary.<string.</param>
    /// <param name="tags">The tags.</param>
    /// <returns>The result of WriteTagsAsync.</returns>
    public async Task WriteTagsAsync(Dictionary<string, object> tags)
    {
        var client = this.plc ?? throw new InvalidOperationException("PLC not initialized. Call InitializeConnection first.");
        foreach (var tag in tags)
        {
            logger.LogInformation("Writing to PLC tag '{TagDataStore}' with value: {ValueInt}", tag.Key, tag.Value);
            await client.SetValue(tag.Key, tag.Value);
        }
    }

    /// <summary>
    /// Executes WriteTagsIntAsync operation.
    /// </summary>
    /// <param name="Dictionary<string">The Dictionary.<string.</param>
    /// <param name="tags">The tags.</param>
    /// <returns>The result of WriteTagsIntAsync.</returns>
    public async Task WriteTagsIntAsync(Dictionary<string, int> tags)
    {
        var client = this.plc ?? throw new InvalidOperationException("PLC not initialized. Call InitializeConnection first.");
        foreach (var tag in tags)
        {
            logger.LogInformation("Writing to PLC tag '{TagDataStore}' with value: {ValueInt}", tag.Key, tag.Value);
            await client.SetValue(tag.Key, tag.Value);
        }
    }

    /// <summary>
    /// Executes WriteTagsShortAsync operation.
    /// </summary>
    /// <param name="Dictionary<string">The Dictionary.<string.</param>
    /// <param name="tags">The tags.</param>
    /// <returns>The result of WriteTagsShortAsync.</returns>
    public async Task WriteTagsShortAsync(Dictionary<string, short> tags)
    {
        var client = this.plc ?? throw new InvalidOperationException("PLC not initialized. Call InitializeConnection first.");
        foreach (var tag in tags)
        {
            logger.LogInformation("Writing to PLC tag '{TagDataStore}' with value: {ValueInt}", tag.Key, tag.Value);
            await client.SetValue(tag.Key, tag.Value);
        }
    }

    /// <summary>
    /// Executes WriteStringTagsAsync operation.
    /// </summary>
    /// <param name="Dictionary<string">The Dictionary.<string.</param>
    /// <param name="tags">The tags.</param>
    /// <returns>The result of WriteStringTagsAsync.</returns>
    public async Task WriteStringTagsAsync(Dictionary<string, string> tags)
    {
        var client = this.plc ?? throw new InvalidOperationException("PLC not initialized. Call InitializeConnection first.");
        foreach (var tag in tags)
        {
            logger.LogInformation("Writing to PLC tag '{TagDataStore}' with value: {ValueInt}", tag.Key, tag.Value);
            await client.SetValue(tag.Key, tag.Value);
        }
    }

    /// <summary>
    /// Executes ExecuteCommandSimulation operation.
    /// </summary>
    /// <returns>The result of ExecuteCommandSimulation.</returns>
    public async Task<string> ExecuteCommandSimulation(
    Product product,
    Dictionary<string, VariableS7> tags,
    string barcode,
    ExecutionStepTask stepTask,
    int machineId,
    DryRunOptions options)
    {
        if (!await this.EnsurePlcIsConnectedAsync(machineId))
        {
            return "Failure";
        }

        var client = this.plc ?? throw new InvalidOperationException("PLC not initialized after connection attempt.");

        // Write known string tags
        await client.SetValue(tags["PartNumber"].Address, product.PartNumber);
        await client.SetValue(tags["BarCode"].Address, barcode);

        // Write known int tags
        await client.SetValue(tags["CycleStatus"].Address, 1);
        await client.SetValue(tags["ResultValidation"].Address, 0);

        await Task.Delay(199); // give PLC some breathing time

        // Send command
        await client.SetValue(tags["Command"].Address, stepTask.Command);

        // Await PLC response
        var resultAddress = tags["ResultValidation"];
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var stopwatch = Stopwatch.StartNew();

        var subscription = client.CreateNotification<int>(resultAddress.Address, TransmissionMode.OnChange)
            .Where(value => value == 1)
            .Take(1)
            .Subscribe(_ =>
            {
                stopwatch.Stop();
                logger.LogInformation("✅ Received expected value 1 on {ResultAddress} after {Elapsed} ms", resultAddress, stopwatch.ElapsedMilliseconds);
                tcs.TrySetResult(true);
            });

        logger.LogInformation("⏳ Waiting up to {Timeout} ms for PLC response on {ResultAddress}", options.WaitTimeForResponse, resultAddress);

        await Task.WhenAny(tcs.Task, Task.Delay(options.WaitTimeForResponse));
        subscription.Dispose();

        stopwatch.Stop();

        if (!tcs.Task.IsCompleted)
        {
            logger.LogWarning("⌛ Timeout after {Elapsed} ms waiting for value on {ResultAddress}", stopwatch.ElapsedMilliseconds, resultAddress);
        }

        var resultValidation = await client.GetValue<int>(tags["ResultValidation"].Address);

        if (resultValidation == 1)
        {
            var generatedBarcode = await client.GetValue<string>(tags["BarCode"].Address);
            logger.LogInformation("📦 PLC returned Barcode value {Barcode} from {BarCodeAddress}", generatedBarcode, tags["BarCode"]);
            return generatedBarcode;
        }

        logger.LogError("❌ PLC returned invalid ResultValidation {ResultValidation} from {ResultAddress}", resultValidation, tags["ResultValidation"]);
        return "Failure";
    }

    /// <summary>
    /// Executes WriteTagsAsync operation.
    /// </summary>
    /// <param name="Dictionary<string">The Dictionary.<string.</param>
    /// <param name="tags">The tags.</param>
    /// <returns>The result of WriteTagsAsync.</returns>
    public Task WriteTagsAsync(Dictionary<string, VariableS7> tags)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Executes InitializeConnection operation.
    /// </summary>
    /// <param name="ipAddress">The ipAddress.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of InitializeConnection.</returns>
    public async Task InitializeConnection(string ipAddress, CancellationToken cancellationToken)
    {
        this.plc = new Sharp7Plc(ipAddress, 0, 2, 102);
        await this.plc.InitializeConnection(cancellationToken);
        this.plc.Logger = logger;
        this.MonitorConnection();
        logger.LogInformation("PLC connection initialized.");
        logger.LogInformation("Waiting 100ms for connection to complete!");
        await Task.Delay(100, cancellationToken);

        logger.LogInformation("Initializing connection to PLC at IP address: {IpAddress}", ipAddress);
    }

    /// <summary>
    /// Executes ReadTagAsync operation.
    /// </summary>
    /// <param name="Dictionary<string">The Dictionary.<string.</param>
    /// <param name="tags">The tags.</param>
    /// <returns>The result of ReadTagAsync.</returns>
    public Task<int> ReadTagAsync(Dictionary<string, VariableS7> tags)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Executes MonitorConnection operation.
    /// </summary>
    public void MonitorConnection()
    {
        var client = this.plc ?? throw new InvalidOperationException("PLC not initialized. Call InitializeConnection first.");
        client.ConnectionState.Subscribe(state =>
        {
            this.lastConnectionState = state;
            this.IsConnected = this.lastConnectionState == Sharp7.Rx.Enums.ConnectionState.Connected;
            Console.WriteLine($"New state: {this.lastConnectionState}");
        });
    }

    public async Task<Dictionary<string, VariableS7>> ReadListIntTagsAsync<T>(Dictionary<string, VariableS7> tagsToRead, int plcId)
    {
        if (tagsToRead == null || tagsToRead.Count == 0)
        {
            logger.LogCritical("No tags to read.");
            return [];
        }

        await this.EnsurePlcIsConnectedAsync(plcId);

        // Resolve types just once
        foreach (var record in tagsToRead.Values)
        {
            if (record.Type == null)
            {
                logger.LogWarning("TagDataStore {TagDataStore} has no type defined. Attempting to resolve.", record.Name);
                var resolvedType = record.ResolveType(record.NetType);
                if (resolvedType is null)
                {
                    logger.LogWarning("Skipping tag {TagDataStore} due to unresolved type.", record.Name);
                    continue;
                }

                record.Type = resolvedType;
            }
        }

        var resultDict = new Dictionary<string, VariableS7>();

        var batchTags = new List<(string Alias, Type Type, int StatusValueId)>();
        foreach (var tag in tagsToRead.Values)
        {
            if (tag.Type is null)
            {
                logger.LogWarning("Skipping tag {TagDataStore} due to missing type.", tag.Name);
                continue;
            }

            batchTags.Add((tag.Address, tag.Type, 0));
        }

        try
        {
            var client = this.plc ?? throw new InvalidOperationException("PLC not initialized. Call InitializeConnection first.");
            var batchResults = await client.GetBatchValuesPlcAsync<T>(batchTags, CancellationToken.None);

            foreach (var result in batchResults)
            {
                var symbolicName = tagsToRead
                    .FirstOrDefault(kvp => kvp.Value.Address.Equals(result.Alias)).Key;

                if (string.IsNullOrEmpty(symbolicName))
                {
                    logger?.LogWarning("TagDataStore not found in Registers. Alias: {Alias}", result.Alias);
                    continue;
                }

                var record = tagsToRead[symbolicName];

                record.ValueInt = result.ValueInt; // Assuming Value is of type T
                record.ValueString = result.ValueString;
                if (result.Status != 1)
                {
                    logger?.LogWarning(
                        "Register read completed with warning. Alias: {Alias}, Status: {Status}, ValueString: {ValueString}",
                        result.Alias, result.Status, result.ValueString);
                }

                resultDict[symbolicName] = record;
            }

            return resultDict;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Exception occurred during ReadListTagsAsync<{Type}>. Error: {Message}",
                typeof(T).Name, ex.Message);
            return tagsToRead;
        }
    }

    /// <summary>
    /// Executes ClearTagsAsync operation.
    /// </summary>
    /// <param name="Dictionary<string">The Dictionary.<string.</param>
    /// <param name="tags">The tags.</param>
    /// <returns>The result of ClearTagsAsync.</returns>
    public async Task ClearTagsAsync(Dictionary<string, VariableS7> tags)
    {
        foreach (var tag in tags.Values)
        {
            if (tag.Type == typeof(int) || tag.Type == typeof(short))
            {
                logger.LogInformation("🔄 Clearing INT tag '{TagDataStore}' with value 0", tag.Name);
                var client = this.plc ?? throw new InvalidOperationException("PLC not initialized. Call InitializeConnection first.");
                await client.SetValue<int>(tag.Address, 0);
            }
            else if (tag.Type == typeof(string))
            {
                logger.LogInformation("🔄 Clearing STRING tag '{TagDataStore}' with empty string", tag.Name);
                var client = this.plc ?? throw new InvalidOperationException("PLC not initialized. Call InitializeConnection first.");
                await client.SetValue<string>(tag.Address, string.Empty);
            }
            else
            {
                logger.LogDebug("⏭️ Skipping unsupported tag '{TagDataStore}' of type {Type}", tag.Name, tag.Type?.Name);
            }
        }
    }

    private async Task<bool> EnsurePlcIsConnectedAsync(int machineId)
    {
        if (this.IpAddress?.Count == 0)
        {
            this.IpAddress = await fixtureDb.LoadPlcAddressAsync();
        }

        if (machineId != this.lastPlcContacted)
        {
            await this.TryConnectionAsync(machineId);
        }

        if (this.IsConnected)
        {
            return true;
        }

        return await this.ReTryConnectionAsync(machineId);
    }

    private async Task<bool> ReTryConnectionAsync(int machineId)
    {
        logger.LogError("PLC is not connected. Cannot execute command simulation.");

        // Wait for connection try reconnecting
        logger.LogInformation("PLC is not connected. Trying to connect Again");

        await this.TryConnectionAsync(machineId);
        logger.LogInformation("PLC is not connected. Waiting 500ms before Trying to connect Again");
        if (!this.IsConnected)
        {
            await Task.Delay(500);
        }

        return await this.TryConnectionAsync(machineId);
    }

    private async Task<bool> TryConnectionAsync(int machineId)
    {
        if (!this.IpAddress.TryGetValue(machineId, out var plcRecord))
        {
            logger.LogError("MachineId {MachineId} not found in PLC records", machineId);
            return false;
        }

        await this.InitializeConnection(plcRecord.IpAddress, CancellationToken.None);

        this.lastPlcContacted = machineId;
        return true;
    }
}
