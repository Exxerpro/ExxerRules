// <copyright file="Sharp7Connector.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Sharp7.Rx.Basics;
using Sharp7.Rx.Enums;
using Sharp7.Rx.Extensions;
using Sharp7.Rx.Interfaces;
using Sharp7.Rx.Settings;

/// <summary>
/// Represents the PlcBatchWriteTag.
/// </summary>

public record PlcBatchWriteTag(string Alias, Type Type, object Value);

public interface ISharp7Connector
{
    IObservable<ConnectionState> ConnectionState { get; }

    ILogger Logger { get; set; }

    TimeSpan ReconnectDelay { get; set; }

    void Dispose();

    Task<bool> Connect();

    Task Disconnect();

    /// <summary>
    /// Writes a batch of values to the PLC efficiently using S7MultiVar.
    /// Continues writing even if individual batches fail.
    /// </summary>
    /// <param name="tags">Collection of tags to write (Alias, Type, ValueString).</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>True if all writes succeeded; false if any failed.</returns>
    Task<bool> WriteBatchValuesPlcAsync(
        IEnumerable<PlcBatchWriteTag> tags,
        CancellationToken token = default);

    Task<IReadOnlyDictionary<string, byte[]>> ExecuteMultiVarRequestGateway(
        IReadOnlyList<string> variableNames, CancellationToken token = default);

    Task<IReadOnlyDictionary<string, byte[]>> ExecuteMultiVarRequest(
        IReadOnlyList<string> variableNames,
        CancellationToken token = default);

    Task DisconnectS7Client();

    Task InitializeAsync();

    Task<byte[]> ReadBytes(Operand operand, ushort startByteAddress, ushort bytesToRead, ushort dbNo, CancellationToken token);

    Task<byte[]> ReadBytesS7Client(Operand operand, ushort startByteAddress, ushort bytesToRead, ushort dbNo, CancellationToken token);

    Task WriteBit(Operand operand, ushort startByteAddress, byte bitAdress, bool value, ushort dbNo, CancellationToken token);

    Task WriteBytes(Operand operand, ushort startByteAddress, byte[] data, ushort dbNo, ushort bytesToWrite, CancellationToken token);

    void EnsureConnectionS7Valid();
}

/// <summary>
/// Implements the <see cref="ISharp7Connector"/> interface, providing concrete functionality for PLC communication using Sharp7.
/// </summary>
public class Sharp7Connector : IDisposable, ISharp7Connector
{
    private readonly BehaviorSubject<ConnectionState> connectionStateSubject = new(Enums.ConnectionState.Initial);
    private readonly int cpuSlotNr;

    private readonly CompositeDisposable disposables = [];
    private readonly string ipAddress;
    private readonly int port;
    private readonly int rackNr;
    private readonly LimitedConcurrencyLevelTaskScheduler scheduler = new(maxDegreeOfParallelism: 1);
    private readonly IVariableNameParser variableNameParser;
    private bool disposed;

    /// <summary>
    /// The underlying Sharp7 client instance.
    /// </summary>
    public S7Client Sharp7;

    /// <summary>
    /// The S7 client instance used for direct S7 communication.
    /// </summary>
    public S7Client S7Client;

    /// <summary>
    /// Initializes a new instance of the <see cref="Sharp7Connector"/> class.
    /// </summary>
    /// <param name="settings">The PLC connection settings.</param>
    /// <param name="variableNameParser">The variable name parser.</param>
    public Sharp7Connector(PlcConnectionSettings settings, IVariableNameParser variableNameParser)
    {
        this.variableNameParser = variableNameParser;
        this.ipAddress = settings.IpAddress;
        this.cpuSlotNr = settings.CpuMpiAddress;
        this.port = settings.Port;
        this.rackNr = settings.RackNumber;

        this.ReconnectDelay = TimeSpan.FromSeconds(5);
    }

    /// <summary>
    /// Gets an observable that provides the connection state of the PLC.
    /// </summary>
    public IObservable<ConnectionState> ConnectionState => this.connectionStateSubject.DistinctUntilChanged().AsObservable();

    /// <summary>
    /// Gets or sets the logger instance for the connector.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    /// Gets or sets the delay before attempting to reconnect to the PLC.
    /// </summary>
    public TimeSpan ReconnectDelay { get; set; }

    private bool IsConnected => this.connectionStateSubject.Value == Enums.ConnectionState.Connected;

    /// <summary>
    /// Disposes the connector and its resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronously connects to the PLC.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the connection was successful; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the S7 driver is not initialized.</exception>
    public async Task<bool> Connect()
    {
        if (this.Sharp7 == null)
        {
            throw new InvalidOperationException("S7 driver is not initialized.");
        }

        try
        {
            var errorCode = await Task.Factory.StartNew(
                () => this.Sharp7.ConnectTo(this.ipAddress, this.rackNr, this.cpuSlotNr),
                CancellationToken.None, TaskCreationOptions.None, this.scheduler);
            if (errorCode == 0)
            {
                this.connectionStateSubject.OnNext(Enums.ConnectionState.Connected);
                return true;
            }
            else
            {
                var errorText = this.EvaluateErrorCode(errorCode);
                this.Logger.LogError("Failed to establish initial connection: {Error}", errorText);
            }
        }
        catch (Exception ex)
        {
            this.connectionStateSubject.OnNext(Enums.ConnectionState.ConnectionLost);
            this.Logger.LogError(ex, "Failed to establish initial connection.");
        }

        return false;
    }

    /// <summary>
    /// Asynchronously disconnects from the PLC.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Disconnect()
    {
        this.connectionStateSubject.OnNext(Enums.ConnectionState.DisconnectedByUser);
        await this.CloseConnection();
    }

    // Size reduce to 6
    // Because of the S7MultiVar limitation on S7-1200
    // The maximum number of items in a single MultiVar request is 6
    // The PLC Trigger another function who is not supported by the Driver
    // So is interpreted as an error
    private const int MaxPduSafeSize = 120;

    private const int MaxItemSize = 6;
    private const int MaxRetryDepth = 2;

    private readonly Lock s7ClientLock = new Lock();

    /// <summary>
    /// Writes a batch of values to the PLC using S7MultiVar, handling S7-1200 limitations.
    /// If the batch write fails, the method recursively splits the batch and retries up to a maximum depth.
    /// </summary>
    /// <param name="batch">The list of tag/address pairs to write in this batch.</param>
    /// <param name="depth">The current recursion depth for retry attempts.</param>
    /// <param name="token">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>True if the batch write succeeded; otherwise, false.</returns>
    /// <remarks>
    /// - The S7-1200 PLC limits the number of items in a single MultiVar request to 6.
    /// - If a write fails, the batch is split and retried recursively up to <see cref="MaxRetryDepth"/>.
    /// - If the maximum retry depth is reached or the batch cannot be split further, the method returns false.
    /// </remarks>
    private async Task<bool> WriteBatchCoreAsync(List<(PlcBatchWriteTag Tag, VariableAddress Address)> batch, int depth,
        CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        this.EnsureConnectionS7Valid();

        var multiVar = new S7MultiVar(this.S7Client);

        foreach (var (tag, address) in batch)
        {
            var buffer = new byte[address.BufferLength];

            if (!ValueConverter.TryWriteToBuffer(buffer, tag.Value, tag.Type, address))
            {
                this.Logger?.LogError("Unsupported type {Type} for writing tag {Alias}", tag.Type, tag.Alias);
                return false;
            }

            multiVar.Add(
                (int)address.Operand,
                (int)address.Type,
                address.DbNo,
                address.Start,
                buffer.Length,
                ref buffer);
        }

        int result;

        lock (this.s7ClientLock)
        {
            // s7comm.param.func == 0x05
            result = multiVar.Write();
            this.EnsureSuccessOrLog(result, $"MultiVar request failed at depth {depth}");
        }

        if (result == 0)
        {
            return true;
        }

        this.Logger?.LogWarning("Write failed at depth {Depth}, retrying. Batch size: {Count}", depth, batch.Count);

        if (depth >= MaxRetryDepth || batch.Count <= 1)
        {
            this.Logger?.LogError("Final retry failed at depth {Depth}. Giving up. Error: {Result}", depth, result);
            return false;
        }

        int mid = batch.Count / 2;
        var left = batch.GetRange(0, mid);
        var right = batch.GetRange(mid, batch.Count - mid);

        bool leftResult = await this.WriteBatchCoreAsync(left, depth + 1, token);
        bool rightResult = await this.WriteBatchCoreAsync(right, depth + 1, token);

        return leftResult && rightResult;
    }

    /// <summary>
    /// Writes a batch of values to the PLC efficiently using S7MultiVar.
    /// Continues writing even if individual batches fail.
    /// </summary>
    /// <param name="tags">Collection of tags to write (Alias, Type, ValueString).</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>True if all writes succeeded; false if any failed.</returns>
    public async Task<bool> WriteBatchValuesPlcAsync(
        IEnumerable<PlcBatchWriteTag> tags,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(tags);
        this.EnsureConnectionS7Valid();
        var parsedTags = tags
            .Select(tag => (Tag: tag, Address: this.ParseAndVerify(tag.Alias, tag.Type)))
            .ToList();

        var batches = new List<List<(PlcBatchWriteTag Tag, VariableAddress Address)>>();
        var currentBatch = new List<(PlcBatchWriteTag, VariableAddress)>();
        var currentBatchSize = 0;
        var currentTagCount = 0;

        foreach (var (tag, address) in parsedTags)
        {
            token.ThrowIfCancellationRequested();

            int tagSize = address.BufferLength;
            var sizeLimitExceeded = currentBatchSize + tagSize > MaxPduSafeSize;
            var countLimitExceeded = currentTagCount >= MaxItemSize;

            if ((sizeLimitExceeded || countLimitExceeded) && currentBatch.Count > 0)
            {
                batches.Add(currentBatch);
                currentBatch = [];
                currentBatchSize = 0;
                currentTagCount = 0;
            }

            currentBatch.Add((tag, address));
            currentBatchSize += tagSize;
            currentTagCount++;
        }

        if (currentBatch.Count > 0)
        {
            batches.Add(currentBatch);
        }

        var allSucceeded = true;

        foreach (var batch in batches)
        {
            try
            {
                var success = await this.WriteBatchCoreAsync(batch, 0, token);
                if (!success)
                {
                    allSucceeded = false;
                }
            }
            catch (OperationCanceledException)
            {
                this.Logger?.LogInformation("Batch write operation was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "Unexpected exception during batch write.");
                allSucceeded = false;
            }
        }

        return allSucceeded;
    }

    /// <summary>
    /// Executes a multi-variable read request to the PLC via a gateway.
    /// </summary>
    /// <param name="variableNames">A read-only list of variable names to read.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only dictionary where keys are variable names and values are byte arrays representing the read data.</returns>
    public async Task<IReadOnlyDictionary<string, byte[]>> ExecuteMultiVarRequestGateway(
        IReadOnlyList<string> variableNames, CancellationToken token = default)
    {
        if (variableNames.IsEmpty())
        {
            return new Dictionary<string, byte[]>();
        }

        var s7MultiVar = new S7MultiVar(this.Sharp7);
        var buffers = variableNames
            .Select(key => new { VariableName = key, Address = this.variableNameParser.Parse(key) })
            .Select(x =>
            {
                var buffer = new byte[x.Address.BufferLength];
#pragma warning disable CS0618 // Type or member is obsolete, no matching overload.
                s7MultiVar.Add(S7Consts.S7AreaDB, S7Consts.S7WLByte, x.Address.DbNo, x.Address.Start,
                    x.Address.BufferLength, ref buffer);
#pragma warning restore CS0618
                return new { x.VariableName, Buffer = buffer };
            })
            .ToArray();

        var result = await Task.Factory.StartNew(() => s7MultiVar.Read(), CancellationToken.None,
            TaskCreationOptions.None, this.scheduler);

        this.EnsureSuccessOrThrow(result, $"Error in MultiVar request for variables: {string.Join(",", variableNames)}");

        return buffers.ToDictionary(arg => arg.VariableName, arg => arg.Buffer);
    }

    /// <summary>
    /// Executes a multi-variable read request to the PLC.
    /// </summary>
    /// <param name="variableNames">A read-only list of variable names to read.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only dictionary where keys are variable names and values are byte arrays representing the read data.</returns>
    public async Task<IReadOnlyDictionary<string, byte[]>> ExecuteMultiVarRequest(
        IReadOnlyList<string> variableNames,
        CancellationToken token = default)
    {
        if (variableNames.IsEmpty())
        {
            return new Dictionary<string, byte[]>();
        }

        var s7MultiVar = new S7MultiVar(this.Sharp7);

        var buffers = variableNames
            .Select(key => new { VariableName = key, Address = this.variableNameParser.Parse(key) })
            .Select(x =>
            {
                var buffer = new byte[x.Address.BufferLength];
#pragma warning disable CS0618 // Type or member is obsolete, no matching overload.
                s7MultiVar.Add(S7Consts.S7AreaDB, S7Consts.S7WLByte, x.Address.DbNo, x.Address.Start,
                    x.Address.BufferLength, ref buffer);
#pragma warning restore CS0618
                return new { x.VariableName, Buffer = buffer };
            })
            .ToArray();

        var result = await Task.Factory.StartNew(() => s7MultiVar.Read(), CancellationToken.None,
            TaskCreationOptions.None, this.scheduler);

        this.EnsureSuccessOrThrow(result, $"Error in MultiVar request for variables: {string.Join(",", variableNames)}");

        return buffers.ToDictionary(arg => arg.VariableName, arg => arg.Buffer);
    }

    /// <summary>
    /// Asynchronously disconnects the S7 client.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task DisconnectS7Client()
    {
        if (this.S7Client is null)
        {
            return Task.CompletedTask;
        }

        this.S7Client.Disconnect();
        this.S7Client = null;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously initializes the connector.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task InitializeAsync()
    {
        try
        {
            this.Sharp7 ??= new S7Client();
            this.Sharp7.PLCPort = this.port;

            var subscription =
                 this.ConnectionState
                     .Where(state => state == Enums.ConnectionState.ConnectionLost)
                     .Take(1)
                     .SelectMany(_ => this.Reconnect())
                     .RepeatAfterDelay(this.ReconnectDelay)
                     .LogAndRetry(this.Logger, "Error while reconnecting to S7.")
                     .Subscribe();

            this.disposables.Add(subscription);
        }
        catch (Exception ex)
        {
            this.Logger?.LogError(ex, "S7 driver could not be initialized");
        }

        return Task.FromResult(true);
    }

    /// <summary>
    /// Asynchronously reads a specified number of bytes from the PLC.
    /// </summary>
    /// <param name="operand">The memory area to read from.</param>
    /// <param name="startByteAddress">The starting byte address.</param>
    /// <param name="bytesToRead">The number of bytes to read.</param>
    /// <param name="dbNo">The data block number (if applicable).</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a byte array with the read data.</returns>
    public async Task<byte[]> ReadBytes(Operand operand, ushort startByteAddress, ushort bytesToRead, ushort dbNo, CancellationToken token)
    {
        this.EnsureConnectionValid();

        var buffer = new byte[bytesToRead];

        var result =
            await Task.Factory.StartNew(() => this.Sharp7.ReadArea(operand.ToArea(), dbNo, startByteAddress, bytesToRead, S7WordLength.Byte, buffer), token, TaskCreationOptions.None, this.scheduler);
        token.ThrowIfCancellationRequested();

        this.EnsureSuccessOrThrow(result, $"Error reading {operand}{dbNo}:{startByteAddress}->{bytesToRead}");

        return buffer;
    }

    /// <summary>
    /// Asynchronously reads a specified number of bytes from the PLC using the S7 client.
    /// </summary>
    /// <param name="operand">The memory area to read from.</param>
    /// <param name="startByteAddress">The starting byte address.</param>
    /// <param name="bytesToRead">The number of bytes to read.</param>
    /// <param name="dbNo">The data block number (if applicable).</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a byte array with the read data.</returns>
    public async Task<byte[]> ReadBytesS7Client(Operand operand, ushort startByteAddress, ushort bytesToRead, ushort dbNo, CancellationToken token)
    {
        this.EnsureConnectionS7Valid();

        var buffer = new byte[bytesToRead];

        var result =
            await Task.Factory.StartNew(() => this.S7Client.ReadArea(operand.ToArea(), dbNo, startByteAddress, bytesToRead, S7WordLength.Byte, buffer), token, TaskCreationOptions.None, this.scheduler);
        token.ThrowIfCancellationRequested();

        this.EnsureSuccessOrLog(result, $"Error reading {operand}{dbNo}:{startByteAddress}->{bytesToRead}");

        return buffer;
    }

    /// <summary>
    /// Asynchronously writes a single bit to the PLC.
    /// </summary>
    /// <param name="operand">The memory area to write to.</param>
    /// <param name="startByteAddress">The starting byte address.</param>
    /// <param name="bitAdress">The bit address within the byte (0-7).</param>
    /// <param name="value">The boolean value to write.</param>
    /// <param name="dbNo">The data block number (if applicable).</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task WriteBit(Operand operand, ushort startByteAddress, byte bitAdress, bool value, ushort dbNo, CancellationToken token)
    {
        this.EnsureConnectionValid();

        var buffer = new[] { value ? (byte)0xff : (byte)0 };

        var offsetStart = (startByteAddress * 8) + bitAdress;

        var result = await Task.Factory.StartNew(() => this.Sharp7.WriteArea(operand.ToArea(), dbNo, offsetStart, 1, S7WordLength.Bit, buffer), token, TaskCreationOptions.None, this.scheduler);
        token.ThrowIfCancellationRequested();

        this.EnsureSuccessOrThrow(result, $"Error writing {operand}{dbNo}:{startByteAddress} bit {bitAdress}");
    }

    /// <summary>
    /// Asynchronously writes a byte array to the PLC.
    /// </summary>
    /// <param name="operand">The memory area to write to.</param>
    /// <param name="startByteAddress">The starting byte address.</param>
    /// <param name="data">The byte array containing the data to write.</param>
    /// <param name="dbNo">The data block number (if applicable).</param>
    /// <param name="bytesToWrite">The number of bytes from the data array to write.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task WriteBytes(Operand operand, ushort startByteAddress, byte[] data, ushort dbNo, ushort bytesToWrite, CancellationToken token)
    {
        this.EnsureConnectionValid();

        var result = await Task.Factory.StartNew(() => this.Sharp7.WriteArea(operand.ToArea(), dbNo, startByteAddress, bytesToWrite, S7WordLength.Byte, data), token, TaskCreationOptions.None, this.scheduler);
        token.ThrowIfCancellationRequested();

        this.EnsureSuccessOrThrow(result, $"Error writing {operand}{dbNo}:{startByteAddress}.{data.Length}");
    }

    /// <summary>
    /// Disposes the connector and its resources.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.disposables.Dispose();

                if (this.Sharp7 != null)
                {
                    this.Sharp7.Disconnect();
                    this.Sharp7 = null;
                }

                this.connectionStateSubject?.OnNext(Enums.ConnectionState.Disposed);
                this.connectionStateSubject?.OnCompleted();
                this.connectionStateSubject?.Dispose();
            }

            this.disposed = true;
        }
    }

    private VariableAddress ParseAndVerify(string variableName, Type type)
    {
        var address = this.variableNameParser.Parse(variableName);
        if (!address.MatchesType(type))
        {
            throw new DataTypeMissmatchException($"Address \"{variableName}\" does not match type {type}.", type, address);
        }

        return address;
    }

    private async Task CloseConnection()
    {
        if (this.Sharp7 == null)
        {
            throw new InvalidOperationException("S7 driver is not initialized.");
        }

        await Task.Factory.StartNew(() => this.Sharp7.Disconnect(), CancellationToken.None, TaskCreationOptions.None, this.scheduler);
    }

    private void EnsureConnectionValid()
    {
        if (this.disposed)

        // throw new ObjectDisposedException(nameof(Sharp7Connector));
        {
            this.Sharp7 = new S7Client
            {
                PLCPort = this.port,
            };
        }

        this.Sharp7 ??= new S7Client();
        this.Sharp7.PLCPort = this.port;

        if (!this.IsConnected)
        {
            // throw new InvalidOperationException("Plc is not connected");
            this.InitializeAsync();
        }
    }

    /// <summary>
    /// Ensures that the S7 client connection is valid.
    /// </summary>
    public void EnsureConnectionS7Valid()
    {
        this.S7Client ??= new S7Client();
        this.S7Client.PLCPort = this.port;

        lock (this.s7ClientLock)
        {
            if (!this.S7Client.Connected)
            {
                this.S7Client.ConnectTo(this.ipAddress, this.rackNr, this.cpuSlotNr);
            }
        }
    }

    private void EnsureSuccessOrThrow(int result, string message)
    {
        if (result == 0)
        {
            return;
        }

        var errorText = this.EvaluateErrorCode(result);
        var completeMessage = $"{message}: {errorText}";

        var additionalErrorText = S7ErrorCodes.GetAdditionalErrorText(result);
        if (additionalErrorText != null)
        {
            completeMessage += Environment.NewLine + additionalErrorText;
        }

        throw new S7CommunicationException(completeMessage, result, errorText);
    }

    private void EnsureSuccessOrLog(int result, string message)
    {
        if (result == 0)
        {
            return;
        }

        var errorText = this.EvaluateErrorCodeS7(result);
        var completeMessage = $"{message}: {errorText}";

        var additionalErrorText = S7ErrorCodes.GetAdditionalErrorText(result);
        if (additionalErrorText != null)
        {
            completeMessage += Environment.NewLine + additionalErrorText;
        }

        var ex = new S7CommunicationException(completeMessage, result, errorText);
        this.Logger?.LogError(ex, completeMessage);
    }

    private string EvaluateErrorCode(int errorCode)
    {
        if (errorCode == 0)
        {
            return null;
        }

        if (this.Sharp7 == null)
        {
            throw new InvalidOperationException("S7 driver is not initialized.");
        }

        var errorText = $"0x{errorCode:X}, {this.Sharp7.ErrorText(errorCode)}";
        this.Logger?.LogError($"S7 Error {errorText}");

        if (S7ErrorCodes.AssumeConnectionLost(errorCode))
        {
            this.SetConnectionLostState();
        }

        return errorText;
    }

    private string EvaluateErrorCodeS7(int errorCode)
    {
        if (errorCode == 0)
        {
            return null;
        }

        if (this.S7Client == null)
        {
            // Create a new instance of S7Client if it is null
            this.EnsureConnectionS7Valid();
        }

        if (this.S7Client == null)
        {
            throw new InvalidOperationException("S7 driver is not initialized.");
        }

        var errorText = $"0x{errorCode:X}, {this.S7Client.ErrorText(errorCode)}";
        this.Logger?.LogError($"S7 Error {errorText}");

        if (S7ErrorCodes.AssumeConnectionLost(errorCode))
        {
            this.SetConnectionLostState();
        }

        return errorText;
    }

    private async Task<bool> Reconnect()
    {
        await this.CloseConnection();

        return await this.Connect();
    }

    private void SetConnectionLostState()
    {
        if (this.connectionStateSubject.Value == Enums.ConnectionState.ConnectionLost)
        {
            return;
        }

        this.connectionStateSubject.OnNext(Enums.ConnectionState.ConnectionLost);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Sharp7Connector"/> class.
    /// </summary>
    ~Sharp7Connector()
    {
        this.Dispose(false);
    }
}
