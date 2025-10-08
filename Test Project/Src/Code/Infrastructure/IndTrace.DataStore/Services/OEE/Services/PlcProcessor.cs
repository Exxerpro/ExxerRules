using IndTrace.Domain.Models;
using IndTrace.DataStore.ModelsComs;
using IndTrace.DataStore.Services.OEE.Interfaces;
using IndTrace.Domain.Entities;
using ConnectionState = Sharp7.Rx.Enums.ConnectionState;

namespace IndTrace.DataStore.Services.OEE.Services;

/// <summary>
/// Service for fetching performance data from S7 PLCs and managing PLC connections.
/// </summary>
public class PlcProcessor : IPlcProcessor
{
    private const int TimeOut = 10;
    private readonly ILogger logger = null!;
    private DateTimeMachine dateTimeMachine = new DateTimeMachine();
    private List<(string Alias, Type Type, int VariableId, string Key)> preparedPerformancesTagInfos = [];
    private IDisposable connectionTimeoutSubscription = null!;
    /// <summary>
    /// Initializes a new instance of the <see cref="PlcProcessor"/> class.
    /// Initializes a new instance of the class.
    /// </summary>

    public PlcProcessor()
    {
        // Initialization logic for the parameterless constructor
        this.dateTimeMachine = new DateTimeMachine();
        this.preparedPerformancesTagInfos = [];
        this.AliasPerformancesToKeyMap = [];
        this.BatchReadPerformances = [];
        this.Registers = new Dictionary<string, Register>();
        this.IsConnected = false;
        this.IsInitialized = false;
        this.ConnectionTimeOut = false;

        // [TODO] //ADD A CONSTRUCTOR THAT TAKES THE PLC DATA AND PERFORMANCE TAGS [DONE] 16 JUN 2025
        ////TODO [URGENT] : Fix the sql to retrieve the aditional data from the PLCs, like the port, rack number, etc.
        ////DONE [ABR] 16 JUN 2025
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="PlcProcessor"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="plcId">The plcId.</param>
    /// <param name="plcData">The plcData.</param>
    /// <param name="IReadOnlyDictionary<string">The IReadOnlyDictionary.<string.</param>
    /// <param name="performances">The performances.</param>
    /// <param name="logger">The logger.</param>

    public PlcProcessor(int plcId, PlcData plcData, IReadOnlyDictionary<string, VariableS7>? performances, ILogger logger)
        : this()
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
        this.PlcId = plcId;
        this.PlcData = plcData;
        this.Performances = performances ?? throw new ArgumentNullException(nameof(performances), "performances cannot be null");
    }

    /// <summary>
    /// Gets or sets the mapping from performance tag aliases to register keys.
    /// </summary>
    public Dictionary<string, string> AliasPerformancesToKeyMap { get; set; }

    /// <summary>
    /// Gets or sets the list of batch read performance tag information.
    /// </summary>
    public List<(string Alias, Type Type, int VariableId)> BatchReadPerformances { get; set; }

    /// <summary>
    /// Gets a value indicating whether the connection to the PLC has timed out.
    /// </summary>
    public bool ConnectionTimeOut { get; private set; }

    /// <summary>
    /// Gets or sets the PLC controller instance.
    /// </summary>
    public IPlc Controller { get; set; } = null!;

    /// <summary>
    /// Gets or sets the IP address of the PLC.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the PLC is currently connected.
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the PLC is initialized.
    /// </summary>
    public bool IsInitialized { get; set; }

    /// <summary>
    /// Gets the machine ID associated with this processor.
    /// </summary>
    public int MachineId { get; private set; }

    /// <summary>
    /// Gets the collection of performance tags for the PLC.
    /// </summary>
    public IReadOnlyDictionary<string, VariableS7>? Performances { get; }

    /// <summary>
    /// Gets the PLC data configuration.
    /// </summary>
    public PlcData PlcData { get; } = null!;

    /// <summary>
    /// Gets the PLC ID associated with this processor.
    /// </summary>
    public int PlcId { get; private set; }

    /// <summary>
    /// Gets or sets the dictionary of register objects for the PLC.
    /// </summary>
    public IDictionary<string, Register> Registers { get; set; }
    /// <summary>
    /// Executes InitializeAsync operation.
    /// </summary>
    /// <returns>The result of InitializeAsync.</returns>

    public Task<Result> InitializeAsync()
    {
        this.Controller = new Sharp7Plc(this.PlcData.IpAddress, this.PlcData.RackNumber, this.PlcData.CpuMpiAddress, this.PlcData.Port);
        this.Controller.Logger = this.logger;

        this.Registers = ConfigureRegisterTags(this.Performances ?? throw new ArgumentNullException(nameof(this.Performances), "Performances cannot be null"));

        this.PreparePerformanceForBatchReadTagInfos(this.PlcData);

        return Task.FromResult(Result.Success());
    }
    /// <summary>
    /// Executes ReadPerformanceDataFromPlcAsync operation.
    /// </summary>
    /// <param name="PlcId">The PlcId.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ReadPerformanceDataFromPlcAsync.</returns>

    public async Task<Result<PerformanceData>> ReadPerformanceDataFromPlcAsync(int PlcId, CancellationToken cancellationToken)
    {
        //TODO [URGENT] : Implement this method to read performance data from the PLC
        //TODO [TODAY] :
        //IMPLEMENT A BATCH READ FOR THE PERFORMANCE DATA
        //THE PEFORMANCE DATA WILL BE READ FROM THE PLC
        //THE GROUP READ IS THE
        var performance = new PerformanceData()
        {
            MachineId = PlcId,
            PlcId = PlcId,
        };

        this.Registers[nameof(this.MachineId)].Value = PlcId.ToString();
        this.Registers[nameof(PlcId)].Value = PlcId.ToString();

        if (!this.IsInitialized || !this.IsConnected)
            await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var batchResults = await this.Controller.GetBatchValuesPlcAsync(this.BatchReadPerformances, cancellationToken);

            foreach (var result in batchResults)
            {
                if (this.AliasPerformancesToKeyMap.TryGetValue(result.Alias, out var key) && this.Registers.TryGetValue(key, out var performances))
                {
                    performances.Value = result.ValueString;
                    performances.StatusValueId = result.Status;
                    performances.TimeStamp = this.dateTimeMachine.Now.ToLocalTime();

                    if (result.Status != 1)
                    {
                        this.logger?.LogWarning(
                            "Register read completed with warning. Name: {Alias}, Status: {Status}, ValueString: {ValueString}",
                            result.Alias, result.Status, result.ValueString);
                    }
                }
                else
                {
                    this.logger?.LogWarning(
                        "TagDataStore not found in Registers. Name: {Alias}",
                        result.Alias);
                }
            }

            performance = PerformanceData.FromPlc(this.Registers);

            performance.MachineId = PlcId;
            performance.PlcId = PlcId;

            await this.Controller.s7Connector.Disconnect();

            return Result<PerformanceData>.Success(performance);
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "Exception occurred during ReadRegistersBulkAsync");
            return Result<PerformanceData>.WithFailure($"Error uploading registers: {ex.Message}", performance);
        }
    }

    private static IDictionary<string, Register> ConfigureRegisterTags(IReadOnlyDictionary<string, VariableS7> variables)
    {
        var collectionTags = variables.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return collectionTags.ToDictionary(
            kvp => kvp.Value.Name,
            kvp => new Register
            {
                VariableId = kvp.Value.VariableId,
                Name = kvp.Value.Name,
                DataType = kvp.Value.NetType,
                StatusValueId = 1,
            });
    }

    private const int ConnectionCheckDelayMs = 200;

    internal async Task<bool> ConnectWithNotificationAsync(CancellationToken cancellationToken)
    {
        if (this.IsInitialized) return true;

        try
        {
            await this.InitializeControllerConnectionAsync(cancellationToken).ConfigureAwait(false);
            this.MonitorConnectionState();

            this.StartConnectionTimeoutMonitor();

            this.logger?.LogInformation("Waiting for connection to PLC {PlcId} at IP {IpAddress}", this.PlcId, this.IpAddress);

            while (!this.IsConnected && !this.ConnectionTimeOut)
            {
                await Task.Delay(ConnectionCheckDelayMs, cancellationToken).ConfigureAwait(false);
            }

            if (!this.IsConnected)
            {
                throw new TimeoutException($"Timeout connecting to PLC {this.PlcId} at IP {this.IpAddress}");
            }

            return this.IsInitialized;
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; // unreachable, required for compiler
        }
        finally
        {
            this.connectionTimeoutSubscription?.Dispose();
        }
    }

    private async Task InitializeControllerConnectionAsync(CancellationToken cancellationToken)
    {
        await this.Controller.InitializeConnection(cancellationToken).ConfigureAwait(false);
        this.IsInitialized = true;
    }

    private void MonitorConnectionState()
    {
        this.Controller.ConnectionState.Subscribe(state => this.IsConnected = state == ConnectionState.Connected);
    }

    private void StartConnectionTimeoutMonitor()
    {
        this.connectionTimeoutSubscription = Observable
            .Interval(TimeSpan.FromSeconds(TimeOut))
            .Subscribe(_ =>
            {
                if (!this.IsConnected)
                {
                    this.ConnectionTimeOut = true;
                    this.logger?.LogError("Failed to connect to controller {PlcId} at IP {IpAddress}", this.PlcId, this.IpAddress);
                }
            });
    }

    private void PreparePerformanceForBatchReadTagInfos(PlcData plcData)
    {
        _ = this.Registers ?? throw new ArgumentNullException(nameof(this.Registers), "Registers cannot be null");

        this.preparedPerformancesTagInfos = this.Registers
            .Where(pair => plcData.Variables.ContainsKey(pair.Key))
            .Select(pair =>
            {
                var variable = plcData.Variables[pair.Key];
                return (Alias: variable.Alias, Type: Type.GetType(variable.NetType) ?? typeof(object), VariableId: variable.VariableId, Key: pair.Key);
            })
            .ToList();

        this.AliasPerformancesToKeyMap = this.preparedPerformancesTagInfos.ToDictionary(x => x.Alias, x => x.Key);

        this.BatchReadPerformances = this.preparedPerformancesTagInfos
            .Select(x => (x.Alias, x.Type, x.VariableId))
            .ToList();

        this.AliasPerformancesToKeyMap = this.preparedPerformancesTagInfos.ToDictionary(x => x.Alias, x => x.Key);
    }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate PlcProcessor logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
