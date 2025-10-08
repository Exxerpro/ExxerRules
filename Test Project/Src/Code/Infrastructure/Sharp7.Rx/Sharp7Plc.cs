// <copyright file="Sharp7Plc.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx;

using System.Buffers;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Sharp7.Rx.Basics;
using Sharp7.Rx.BatchRead;
using Sharp7.Rx.Enums;
using Sharp7.Rx.Extensions;
using Sharp7.Rx.Interfaces;
using Sharp7.Rx.Settings;
using Sharp7.Rx.Utils;

/// <summary>
/// Provides reactive PLC communication functionality for Siemens S7 PLCs using Sharp7 library.
/// Enables real-time data monitoring, batch reading, and notification-based updates for industrial automation.
/// Supports multi-variable subscriptions with automatic connection management and performance monitoring.
/// </summary>
public class Sharp7Plc : IPlc
{
    private static readonly ArrayPool<byte> ArrayPool = ArrayPool<byte>.Shared;

    private static readonly MethodInfo GetValueMethod = typeof(Sharp7Plc).GetMethods()
        .Single(m => m.Name == nameof(GetValue) && m.GetGenericArguments().Length == 1);

    private static readonly MethodInfo CreateNotificationMethod = typeof(Sharp7Plc).GetMethods()
        .Single(m => m.Name == nameof(CreateNotification) && m.GetGenericArguments().Length == 1);

    private readonly ConcurrentSubjectDictionary<string, byte[]> multiVariableSubscriptions = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly List<long> performanceCounter = new(1000);
    private readonly PlcConnectionSettings plcConnectionSettings;
    private readonly CacheVariableNameParser variableNameParser = new CacheVariableNameParser(new VariableNameParser());
    private bool disposed;
    private int initialized;

    private IDisposable notificationSubscription;

    /// <summary>
    /// Gets or sets the S7 connector for PLC communication.
    /// </summary>
    public ISharp7Connector S7Connector { get; set; }

    /// <summary>
    /// Gets or sets the S7 connector for PLC communication (lowercase accessor for backwards compatibility).
    /// </summary>
    public ISharp7Connector s7Connector
    {
        get => S7Connector;
        set => S7Connector = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sharp7Plc"/> class for testing purposes.
    /// </summary>
    public Sharp7Plc()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sharp7Plc"/> class with the specified connection parameters.
    /// </summary>
    /// <param name="ipAddress">The IP address of the PLC.</param>
    /// <param name="rackNumber">The rack number of the PLC.</param>
    /// <param name="cpuMpiAddress">The CPU MPI address of the PLC.</param>
    /// <param name="port">The port number for communication (default: 102).</param>
    /// <param name="multiVarRequestCycleTime">
    ///     <para>
    ///         Polling interval for multi variable read from PLC.
    ///     </para>
    ///     <para>
    ///         This is the wait time between two successive reads from PLC and determines the
    ///         time resolution for all variable reads related with CreateNotification.
    ///     </para>
    ///     <para>
    ///         Default is 100 ms. The minimum supported time is 5 ms.
    ///     </para>
    /// </param>
    public Sharp7Plc(string ipAddress, int rackNumber, int cpuMpiAddress, int port = 102, TimeSpan? multiVarRequestCycleTime = null)
    {
        this.plcConnectionSettings = new PlcConnectionSettings { IpAddress = ipAddress, RackNumber = rackNumber, CpuMpiAddress = cpuMpiAddress, Port = port };
        this.S7Connector = new Sharp7Connector(this.plcConnectionSettings, this.variableNameParser);
        this.ConnectionState = this.S7Connector.ConnectionState;

        if (multiVarRequestCycleTime != null)
        {
            if (multiVarRequestCycleTime < TimeSpan.FromMilliseconds(5))
            {
                this.MultiVarRequestCycleTime = TimeSpan.FromMilliseconds(5);
            }
            else
            {
                this.MultiVarRequestCycleTime = multiVarRequestCycleTime.Value;
            }
        }
    }

    /// <summary>
    /// Gets the observable connection state of the PLC.
    /// </summary>
    public IObservable<ConnectionState> ConnectionState { get; }

    /// <summary>
    /// Gets or sets the logger for PLC communication.
    /// </summary>
    public ILogger Logger
    {
        get => this.S7Connector.Logger;
        set => this.S7Connector.Logger = value;
    }

    /// <summary>
    /// Gets the multi-variable request cycle time.
    /// </summary>
    public TimeSpan MultiVarRequestCycleTime { get; } = TimeSpan.FromSeconds(0.1);

    /// <summary>
    /// Gets or sets the maximum number of items for multi-variable requests.
    /// </summary>
    public int MultiVarRequestMaxItems { get; set; } = 16;

    /// <summary>
    /// Disposes of the PLC resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Create an Observable for a given variable. Multiple notifications are automatically combined into a multi-variable subscription to
    ///     reduce network trafic and PLC workload.
    /// </summary>
    /// <typeparam name="TValue">The type of value to observe.</typeparam>
    /// <param name="variableName">The name of the variable to observe.</param>
    /// <param name="transmissionMode">The transmission mode for notifications.</param>
    /// <returns>An observable that emits values when the variable changes.</returns>
    public IObservable<TValue> CreateNotification<TValue>(string variableName, TransmissionMode transmissionMode)
    {
        return Observable.Create<TValue>(observer =>
        {
            var address = this.ParseAndVerify(variableName, typeof(TValue));

            var disp = new CompositeDisposable();
            var disposableContainer = this.multiVariableSubscriptions.GetOrCreateObservable(variableName);
            disposableContainer.AddDisposableTo(disp);

            var observable =

                // Read variable with GetValue first.
                // This will propagate any errors due to reading from invalid addresses.
                Observable.FromAsync(() => this.GetValue<TValue>(variableName))
                    .Concat(
                        disposableContainer.Observable
                            .Select(bytes => ValueConverter.ReadFromBuffer<TValue>(bytes, address)));

            if (transmissionMode == TransmissionMode.OnChange)
            {
                observable = observable.DistinctUntilChanged();
            }

            observable.Subscribe(observer)
                .AddDisposableTo(disp);

            return disp;
        });
    }

    /// <summary>
    ///     Read PLC variable as generic variable.
    /// </summary>
    /// <typeparam name="TValue">The type of value to read.</typeparam>
    /// <param name="variableName">The name of the variable to read.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>The value read from the PLC.</returns>
    public async Task<TValue> GetValue<TValue>(string variableName, CancellationToken token = default)
    {
        var address = this.ParseAndVerify(variableName, typeof(TValue));

        var data = await this.S7Connector.ReadBytes(address.Operand, address.Start, address.BufferLength, address.DbNo, token);
        return ValueConverter.ReadFromBuffer<TValue>(data, address);
    }

    /// <summary>
    ///     Read PLC variable as generic variable using S7Client.
    /// </summary>
    /// <typeparam name="TValue">The type of value to read.</typeparam>
    /// <param name="variableName">The name of the variable to read.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>The value read from the PLC.</returns>
    public async Task<TValue> GetValueS7Client<TValue>(string variableName, CancellationToken token = default)
    {
        var address = this.ParseAndVerify(variableName, typeof(TValue));

        var data = await this.S7Connector.ReadBytesS7Client(address.Operand, address.Start, address.BufferLength, address.DbNo, token);
        return ValueConverter.ReadFromBuffer<TValue>(data, address);
    }

    /// <summary>
    ///     Read PLC variable as object.
    ///     The return type is automatically infered from the variable name.
    /// </summary>
    /// <param name="variableName">The name of the variable to read.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>The actual return type is infered from the variable name.</returns>
    public async Task<object> GetValue(string variableName, CancellationToken token = default)
    {
        var address = this.variableNameParser.Parse(variableName);
        var clrType = address.GetClrType();

        var genericGetValue = GetValueMethod!.MakeGenericMethod(clrType);

        var task = genericGetValue.Invoke(this, [variableName, token]) as Task;

        await task!;
        var taskType = typeof(Task<>).MakeGenericType(clrType);
        var propertyInfo = taskType.GetProperty(nameof(Task<object>.Result));
        var result = propertyInfo!.GetValue(task);

        return result;
    }

    /// <summary>
    ///     Write value to the PLC.
    /// </summary>
    /// <typeparam name="TValue">The type of value to write.</typeparam>
    /// <param name="variableName">The name of the variable to write to.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SetValue<TValue>(string variableName, TValue value, CancellationToken token = default)
    {
        var address = this.ParseAndVerify(variableName, typeof(TValue));

        if (typeof(TValue) == typeof(bool))
        {
            // Special handling for bools, which are written on a by-bit basis. Writing a complete byte would
            // overwrite other bits within this byte.
            await this.S7Connector.WriteBit(address.Operand, address.Start, address.Bit!.Value, (bool)(object)value, address.DbNo, token);
        }
        else
        {
            var buffer = ArrayPool.Rent(address.BufferLength);
            try
            {
                ValueConverter.WriteToBuffer(buffer, value, address);

                await this.S7Connector.WriteBytes(address.Operand, address.Start, buffer, address.DbNo, address.BufferLength, token);
            }
            finally
            {
                ArrayPool.Return(buffer);
            }
        }
    }

    ////the S7-1200 CPU does not support this function
    // public S7Client.S7BlockInfo GetAgBlockInfo(string variableName, CancellationToken token = default)
    // {
    //    //41
    //    //S7Consts.S7AreaDB 0x84 DB
    //    // S7 Consts.Block_DB DB 0x41
    //    // DB
    //    //
    //    //
    //    //int BlockType = S7Consts.S7AreaDB;//= 131   [Obsolete("Use enum S7Area.DB instead")]

    // s7Connector.EnsureConnectionS7Valid();

    // //int BlockType = (int)S7Area.DB; // DB = 132,
    //    //int BlockType = 65; // DB = 132,

    // var BlockType = 0x41;
    //    var BlockNumber = variableNameParser.Parse(variableName).DbNo;
    //    var address = variableNameParser.Parse(variableName);
    //    S7Client.S7BlockInfo blockInfo = new S7Client.S7BlockInfo();
    //    var dbSize = s7Connector.s7Client.GetAgBlockInfo(BlockType, BlockNumber, ref blockInfo);
    //    return blockInfo;
    // }

    /// <summary>
    /// Reads multiple PLC variables in batch asynchronously.
    /// </summary>
    /// <param name="tags">The collection of tags to read with their aliases, types, and status value IDs.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A collection of batch read results.</returns>
    public async Task<IEnumerable<PlcBatchReadResult>> GetBatchValuesPlcAsync(
        IEnumerable<(string Alias, Type Type, int StatusValueId)> tags,
        CancellationToken token = default)
    {
        var parsedTags = tags
            .Select(tag => (Tag: tag, Address: this.ParseAndVerify(tag.Alias, tag.Type)))
            .ToList();

        var groupedByDb = parsedTags
            .GroupBy(x => x.Address.DbNo)
            .ToDictionary(group => group.Key, group => group.ToList());

        var resultList = new List<PlcBatchReadResult>();

        foreach (var (dbNo, dbTags) in groupedByDb)
        {
            var maxOffset = dbTags
                .Max(x => x.Address.Start + x.Address.BufferLength);

            byte[] buffer;
            try
            {
                buffer = await this.GetValue<byte[]>($"Db{dbNo}.Byte0.{maxOffset}", token);
            }
            catch
            {
                resultList.AddRange(dbTags.Select(x => new PlcBatchReadResult(x.Address.ToString(), x.Tag.Alias, string.Empty, 0, -1)));
                continue;
            }

            foreach (var (tag, address) in dbTags)
            {
                try
                {
                    var data = PlcMemoryReader.ReadFromMemoryBuffer(
                        buffer, address.Operand, address.Start, address.BufferLength, address.DbNo);

                    var (_, stringValue) = BatchRead.ValueConverter.ReadFromBuffer(data, address, tag.Type);

                    resultList.Add(new PlcBatchReadResult(address.ToString(), tag.Alias, stringValue, 0, 1));
                }
                catch
                {
                    resultList.Add(new PlcBatchReadResult(address.ToString(), tag.Alias, string.Empty, 0, -1));
                }
            }
        }

        return resultList;
    }

    /// <summary>
    /// Reads multiple PLC variables in batch asynchronously with generic type support.
    /// </summary>
    /// <typeparam name="T">The generic type for batch reading.</typeparam>
    /// <param name="tags">The collection of tags to read with their aliases, types, and status value IDs.</param>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A collection of batch read results.</returns>
    public async Task<IEnumerable<PlcBatchReadResult>> GetBatchValuesPlcAsync<T>(
    IEnumerable<(string Alias, Type Type, int StatusValueId)> tags,
    CancellationToken token = default)
    {
        var parsedTags = tags
            .Select(tag => (Tag: tag, Address: this.ParseAndVerify(tag.Alias, tag.Type)))
            .ToList();

        var groupedByDb = parsedTags
            .GroupBy(x => x.Address.DbNo)
            .ToDictionary(group => group.Key, group => group.ToList());

        var resultList = new List<PlcBatchReadResult>();

        foreach (var (dbNo, dbTags) in groupedByDb)
        {
            var maxOffset = dbTags.
                Max(x => x.Address.Start + x.Address.BufferLength);

            byte[] buffer;
            try
            {
                buffer = await this.GetValue<byte[]>($"Db{dbNo}.Byte0.{maxOffset}", token);
            }
            catch
            {
                resultList.AddRange(dbTags.Select(x =>
                    new PlcBatchReadResult(x.Address.ToString(), x.Tag.Alias, string.Empty, 0, -1)));
                continue;
            }

            foreach (var (tag, address) in dbTags)
            {
                try
                {
                    var data = PlcMemoryReader.ReadFromMemoryBuffer(
                        buffer, address.Operand, address.Start, address.BufferLength, address.DbNo);

                    var (value, stringValue) = BatchRead.ValueConverter.ReadFromBuffer(data, address, tag.Type);

                    var intValue = ConvertToIntValue(value);
                    resultList.Add(new PlcBatchReadResult(address.ToString(), tag.Alias, stringValue, intValue, 1));
                }
                catch
                {
                    resultList.Add(new PlcBatchReadResult(address.ToString(), tag.Alias, string.Empty, 0, -1));
                }
            }
        }

        return resultList;
    }

    private static int ConvertToIntValue(object value)
    {
        switch (value)
        {
            case int i: return i;
            case short s: return s;
            case byte b: return b;
            case bool flag: return flag ? 1 : 0;
            case Enum e: return Convert.ToInt32(e);
            default: return 0; // For unsupported or non-int-compatible types
        }
    }

    /// <summary>
    ///     Creates an observable of object for a variable.
    ///     The return type is automatically infered from the variable name.
    /// </summary>
    /// <param name="variableName">The name of the variable to observe.</param>
    /// <param name="transmissionMode">The transmission mode for notifications.</param>
    /// <returns>The return type is infered from the variable name.</returns>
    public IObservable<object> CreateNotification(string variableName, TransmissionMode transmissionMode)
    {
        var address = this.variableNameParser.Parse(variableName);
        var clrType = address.GetClrType();

        var genericCreateNotification = CreateNotificationMethod!.MakeGenericMethod(clrType);

        var genericNotification = genericCreateNotification.Invoke(this, [variableName, transmissionMode]);

        return SignatureConverter.ConvertToObjectObservable(genericNotification, clrType);
    }

    /// <summary>
    ///     Trigger PLC connection and start notification loop.
    ///     <para>
    ///         This method returns immediately and does not wait for the connection to be established.
    ///     </para>
    /// </summary>
    /// <returns>Always true.</returns>
    [Obsolete($"Use {nameof(InitializeConnection)} or {nameof(TriggerConnection)}.")]
    public async Task<bool> InitializeAsync()
    {
        await this.TriggerConnection();
        return true;
    }

    /// <summary>
    ///     Initialize PLC connection and wait for connection to be established.
    /// </summary>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InitializeConnection(CancellationToken token = default) => await this.DoInitializeConnection(true, token);

    /// <summary>
    ///     Trigger PLC connection without waiting for connection to be established.
    /// </summary>
    /// <param name="token">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task TriggerConnection(CancellationToken token = default) => await this.DoInitializeConnection(false, token);

    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;

        if (disposing)
        {
            this.notificationSubscription?.Dispose();
            this.notificationSubscription = null;

            if (this.S7Connector != null)
            {
                this.S7Connector.Disconnect().Wait();
                this.S7Connector.Dispose();
                this.S7Connector = null;
            }

            this.multiVariableSubscriptions.Dispose();
        }
    }

    private async Task DoInitializeConnection(bool waitForConnection, CancellationToken token)
    {
        if (Interlocked.Exchange(ref this.initialized, 1) == 1)
        {
            return;
        }

        await this.S7Connector.InitializeAsync();

        // Triger connection.
        // The initial connection might fail. In this case a reconnect is initiated.
        // So we ignore any errors and wait for ConnectionState Connected afterward.
        _ = Task.Run(
            async () =>
        {
            try
            {
                await this.S7Connector.Connect();
            }
            catch (Exception e)
            {
                this.Logger?.LogError(e, "Intiial PLC connection failed.");
            }
        }, token);

        if (waitForConnection)
        {
            await this.S7Connector.ConnectionState
                .FirstAsync(c => c == Enums.ConnectionState.Connected)
                .ToTask(token);
        }

        this.StartNotificationLoop();
    }

    private async Task<Unit> GetAllValues(ISharp7Connector connector)
    {
        if (this.multiVariableSubscriptions.ExistingKeys.IsEmpty())
        {
            return Unit.Default;
        }

        var stopWatch = Stopwatch.StartNew();
        foreach (var partsOfMultiVarRequest in this.multiVariableSubscriptions.ExistingKeys.Buffer(this.MultiVarRequestMaxItems))
        {
            var multiVarRequest = await connector.ExecuteMultiVarRequest(partsOfMultiVarRequest as IReadOnlyList<string>);

            foreach (var pair in multiVarRequest)
            {
                if (this.multiVariableSubscriptions.TryGetObserver(pair.Key, out var subject))
                {
                    subject.OnNext(pair.Value);
                }
            }
        }

        stopWatch.Stop();
        this.performanceCounter.Add(stopWatch.ElapsedMilliseconds);

        this.PrintAndResetPerformanceStatistik();

        return Unit.Default;
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

    private void PrintAndResetPerformanceStatistik()
    {
        if (this.performanceCounter.Count == this.performanceCounter.Capacity)
        {
            var average = this.performanceCounter.Average();
            var min = this.performanceCounter.Min();
            var max = this.performanceCounter.Max();

            this.Logger?.LogTrace(
                "PLC {Plc} notification perf: {Elements} calls, min {Min}, max {Max}, avg {Avg}, variables {Vars}, batch size {BatchSize}",
                this.plcConnectionSettings.IpAddress,
                this.performanceCounter.Capacity, min, max, average,
                this.multiVariableSubscriptions.ExistingKeys.Count(),
                this.MultiVarRequestMaxItems);
            this.performanceCounter.Clear();
        }
    }

    private void StartNotificationLoop()
    {
        if (this.notificationSubscription != null)
        {
            // notification loop already running
            return;
        }

        var subscription =
            this.ConnectionState
                .FirstAsync(states => states == Enums.ConnectionState.Connected)
                .SelectMany(_ => this.GetAllValues(this.S7Connector))
                .RepeatAfterDelay(this.MultiVarRequestCycleTime)
                .LogAndRetryAfterDelay(this.Logger, this.MultiVarRequestCycleTime, "Error while getting batch notifications from plc")
                .Subscribe();

        if (Interlocked.CompareExchange(ref this.notificationSubscription, subscription, null) != null)
        {
            // Subscription has already been created (race condition). Dispose new subscription.
            subscription.Dispose();
        }
    }

    ~Sharp7Plc()
    {
        this.Dispose(false);
    }
}
