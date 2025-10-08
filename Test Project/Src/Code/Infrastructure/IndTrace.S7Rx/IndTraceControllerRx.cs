// <copyright file="IndTraceControllerRx.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.S7Rx;

using IndTrace.Application.Performance.Request.Command.Create;

/// <summary>
/// Represents the IndTraceControllerRx.
/// </summary>
public class IndTraceControllerRx : IIndTraceControllerRx
{
    // Use like this to subscribe IIndTraceController.CommandChanged.Subscribe(_ => FUNC<,> );
    // be sure using ObservableExtensions using Subscribe =System.ObservableExtensions;
    private const int TimeOut = 10;

    private readonly ILogger logger = null!;
    private readonly Subject<IIndTraceControllerRx> subCommand = new();
    private readonly Subject<IIndTraceControllerRx> subHeartBeat = new();
    private Dictionary<string, Variable> eventVariables = [];
    private DateTimeMachine dateTimeMachine;

    private const int DisableTimeoutMagic = 1987;

    /// <inheritdoc/>
    public PlcDto PlcDetails { get; private set; }

    /// <inheritdoc/>
    public bool IsOeeEnabled { get; private set; }

    /// <inheritdoc/>
    public string BarCode { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public short Command { get; private set; }

    public short CommandFeedback { get; private set; }

    /// <inheritdoc/>
    public IObservable<IIndTraceControllerRx> CommandChanged => this.subCommand.AsObservable();

    /// <inheritdoc/>
    public bool Configured { get; private set; }

    public bool ConnectionTimeOut { get; private set; }

    /// <inheritdoc/>
    public short HeartBeat { get; private set; }

    /// <inheritdoc/>
    public IObservable<IIndTraceControllerRx> HeartBeatChanged => this.subHeartBeat.AsObservable();

    /// <inheritdoc/>
    public bool IsConnected { get; set; }

    /// <inheritdoc/>
    public bool IsInitialized { get; set; }

    /// <inheritdoc/>
    public int MachineId { get; private set; }

    /// <inheritdoc/>
    public string PartNumber { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string Name { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public int PlcId { get; private set; }

    /// <inheritdoc/>
    public PartStatus PartStatus { get; private set; }

    /// <inheritdoc/>
    public CycleStatus CycleStatus { get; private set; }

    /// <inheritdoc/>
    public int CyclesOk { get; private set; }

    /// <inheritdoc/>
    public IDictionary<string, Register> References { get; set; } = new Dictionary<string, Register>();

    /// <inheritdoc/>
    public IDictionary<string, Register> Registers { get; set; } = new Dictionary<string, Register>();

    public IDictionary<string, Register> Perfomances { get; set; } = new Dictionary<string, Register>();

    public IPlc Controller { get; set; } = new NoOpPlc();

    public bool EnableSimulation { get; set; }

    public IDictionary<string, IIndTraceTagRx> IndTraceTags { get; set; } = new Dictionary<string, IIndTraceTagRx>();

    private string IpAddress { get; set; } = string.Empty;

    // Precomputed list for batch read
    private List<(string Alias, Type Type, int VariableId, string Key)> preparedRegisterTagInfos = new();

    private List<(string Alias, Type Type, int VariableId, string Key)> preparedPerformancesTagInfos = new();

    // This flag will control if we need to do a readback after a write operation
    // If we are on a hotpath the readback will be avoided
    // If we are on a setup the readback can be done to verify the value was written correctly
    private bool _readback = false;

    // Precomputed mapping for fast lookup
    public Dictionary<string, string> AliasRegisterToKeyMap { get; set; } = new();

    public Dictionary<string, string> AliasPerformancesToKeyMap { get; set; } = new();

    public List<(string Alias, Type Type, int VariableId)> BatchReadRegister { get; set; } = new();

    public List<(string Alias, Type Type, int VariableId)> BatchReadPerformances { get; set; } = new();

    public IndTraceControllerRx(ILogger logger, PlcDto plcDetail, DateTimeMachine dateTimeMachine)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "logger cannot be null");
        this.dateTimeMachine = dateTimeMachine ?? throw new ArgumentNullException(nameof(dateTimeMachine), "dateTimeMachine cannot be null");
        if (plcDetail is null)
        {
            throw new ArgumentNullException(nameof(plcDetail), "PlcDetails cannot be null");
        }

        this.PlcDetails = plcDetail;
        this.IsOeeEnabled = plcDetail.HasOeeEnabled;
        this.PartStatus = PartStatus.None;
        this.CycleStatus = CycleStatus.None;
    }

    /// <inheritdoc/>
    public bool Retry { get; set; }

    public int ErrorCounter { get; set; }

    public int ThreesHold { get; set; } = 6; // ABR  26 MAY 2025 Waiting for data from wireshark and testing to decide on these cases

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Controller?.Dispose();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return
            $"PlcId: {this.PlcId} IpAddress: {this.IpAddress} MachineId: {this.MachineId} IsConnected: {this.IsConnected} IsInitialized: {this.IsInitialized} Configured: {this.Configured}" +
            this.ReferencesToString() + this.RegistersToString();
    }

    public string ReferencesToString()
    {
        if (this.References is null || this.References.Count == 0)
        {
            return "References don't exist";
        }

        var result = new StringBuilder();

        foreach (var (key, reference) in this.References)
        {
            result.Append($"Key: {key} ValueString: {reference.Value}\n");
        }

        return result.ToString();
    }

    public string PerformancesToString()
    {
        if (this.Perfomances is null || this.Perfomances.Count == 0)
        {
            return "Perfomances don't exist";
        }

        var result = new StringBuilder();

        foreach (var (key, performance) in this.Perfomances)
        {
            result.Append($"Key: {key} ValueString: {performance.Value}\n");
        }

        return result.ToString();
    }

    public string RegistersToString()
    {
        if (this.Registers is null || this.Registers.Count == 0)
        {
            return "Registers don't exist";
        }

        var result = new StringBuilder();
        foreach (var (key, register) in this.Registers)
        {
            result.Append($"Key: {key} ValueString: {register.Value}\n");
        }

        return result.ToString();
    }

    public async Task<bool> DownloadReferencesAsync(CancellationToken cancellationToken)
    {
        if (this.EnableSimulation)
        {
            return true;
        }

        if (!this.IsInitialized || !this.IsConnected)
        {
            await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
        }

        foreach (var (key, reference) in this.References)
        {
            if (string.IsNullOrWhiteSpace(reference.Value))
            {
                continue;
            }

            if (!this.PlcDetails.Variables.ContainsKey(key))
            {
                this.logger?.LogWarning("Variable not found in PLC definition: {Key} {Name} {EntitieId}", key, reference.Name, reference.VariableId);
                continue;
            }

            if (!this.IndTraceTags.ContainsKey(key))
            {
                this.logger?.LogWarning("Tag not found in IndTraceTags: {Key} {Name} {EntitieId}", key, reference.Name, reference.VariableId);
                continue;
            }

            try
            {
                this.IndTraceTags[key].Value = reference.Value;
                await this.IndTraceTags[key].DownloadValueAsync(this.Controller).ConfigureAwait(false);

                if (this.logger is not null)
                {
                    this.logger.LogInformation("S7 D1 Writing to tag {key} value {value}", key, reference.Value);
                }
            }
            catch (Exception ex)
            {
                if (this.logger is not null)
                {
                    this.logger.LogError(ex, "Failed to download value to tag {Key}. ValueString: {ValueString}, PLC: {PlcId}, IP: {IpAddress}",
                        key,
                        this.IndTraceTags[key]?.Value,
                        this.PlcId,
                        this.IpAddress);
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Iterates through all variables and attempts to read their values from the PLC.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for async operations.</param>
    /// <returns>A dictionary mapping variable names to their read string values.</returns>
    public async Task<Dictionary<string, string>> TryReadAllVariablesAsync(CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, string>();

        foreach (var (key, variable) in this.PlcDetails.Variables)
        {
            try
            {
                var result = await this.TryReadVariableOnPlc(variable, cancellationToken);
                results[variable.Name] = result;
                if (this.logger is not null)
                {
                    this.logger.LogInformation("Successfully read {VariableName}: {Result}", variable.Name, result);
                }
            }
            catch (Exception ex)
            {
                if (this.logger is not null)
                {
                    this.logger.LogError(ex, "Failed to read variable {VariableName}", variable.Name);
                }
                results[variable.Name] = string.Empty;
            }
        }

        return results;
    }

    /// <summary>
    /// Parses a string back into the correct .NET type based on PLC NetType.
    /// Validates that the variable exists on the PLC.
    /// Checks for the DB size on the PLC.
    /// </summary>
    private async Task<string> TryReadVariableOnPlc(Variable variable, CancellationToken cancellationToken)
    {
        var netType = variable.NetType;

        try
        {
            // var sS7BlockInfo = this.Controller.GetAgBlockInfo(variable.Address, AsQueryableAsync);
            // variable.DBSize = sS7BlockInfo.BlkLang;
            // _logger.LogInformation(
            //    "[DBValidation] Variable: {Name}, DB: {DB}, MC7Size: {MC7Size}, LoadSize: {LoadSize}",
            //    variable.Name,
            //    sS7BlockInfo.BlkNumber,
            //    sS7BlockInfo.MC7Size,
            //    sS7BlockInfo.LoadSize
            // );

            // variable.DBSize = sS7BlockInfo.MC7Size;
            switch (netType)
            {
                case "System.Int32":
                    var intVal = await this.Controller.GetValueS7Client<int>(variable.Address, cancellationToken);
                    variable.Validated = true;
                    return intVal.ToString();

                case "System.String":
                    var stringVal = await this.Controller.GetValueS7Client<string>(variable.Address, cancellationToken);
                    variable.Validated = true;
                    return stringVal;

                case "System.Int16":
                    var shortVal = await this.Controller.GetValueS7Client<short>(variable.Address, cancellationToken);
                    variable.Validated = true;
                    return shortVal.ToString();

                case "System.Boolean":
                    var boolVal = await this.Controller.GetValueS7Client<bool>(variable.Address, cancellationToken);
                    variable.Validated = true;
                    return boolVal.ToString();

                case "System.Single":
                    var floatVal = await this.Controller.GetValueS7Client<float>(variable.Address, cancellationToken);
                    variable.Validated = true;
                    return floatVal.ToString(CultureInfo.InvariantCulture);

                default:
                    this.logger?.LogWarning("Unsupported NetType {NetType}. Sending raw string.", netType);
                    variable.Validated = false;
                    return string.Empty;
            }
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "Error reading {Controller}, variable {Variable} value for NetType {NetType} from PLC at address {Address}", this.PlcId, variable.Name, netType, variable.Address);
            variable.Validated = false;
            return string.Empty;
        }
    }

    // TODO: [Critical] Consider implementing a combined approach for error recovery:
    // ABR  26 MAY 2025 Waiting for data from wireshark to decide on these cases
    // Require several consecutive successful batch reads before resetting ErrorCounter.
    // Add a time-based reset, using a configurable interval from the database.
    // Make both the error threshold and the time interval configurable via the database.
    // This will improve robustness and avoid rapid reconnect cycles.

    /// <inheritdoc/>
    public async Task<Result> DownloadReferencesBulkAsync(CancellationToken cancellationToken)
    {
        if (this.EnableSimulation)
        {
            return Result.Success();
        }

        if (this.ErrorCounter > this.ThreesHold)
        {
            await this.DisconnectWithOutNotificationAsync(cancellationToken).ConfigureAwait(false);
            this.ErrorCounter = 0;
            await Task.Delay(20, cancellationToken);
        }

        if (!this.IsInitialized || !this.IsConnected)
        {
            await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
        }

        var batchWriteTags = new List<PlcBatchWriteTag>();

        foreach (var (key, reference) in this.References)
        {
            if (string.IsNullOrWhiteSpace(reference.Value))
            {
                continue;
            }

            if (!this.PlcDetails.Variables.ContainsKey(key))
            {
                this.logger?.LogWarning("Variable not found in PLC definition: {Key} {Name} {EntitieId}", key, reference.Name, reference.VariableId);
                continue;
            }

            if (!this.IndTraceTags.TryGetValue(key, out var tag))
            {
                this.logger?.LogWarning("Tag not found in IndTraceTags: {Key} {Name} {EntitieId}", key, reference.Name,
                    reference.VariableId);
                continue;
            }

            tag.Value = reference.Value;

            batchWriteTags.Add(new PlcBatchWriteTag(tag.Variable.Alias, tag.NetType, tag.Value));
        }

        if (!batchWriteTags.Any())
        {
            this.logger?.LogInformation("No valid tags to write for PLC {PlcId}.", this.PlcId);
            return Result.Success();
        }

        bool success = false;
        try
        {
            success = await this.Controller.s7Connector.WriteBatchValuesPlcAsync(batchWriteTags, cancellationToken).ConfigureAwait(false);
            this.logger?.LogInformation("S7 Batch writing to PLC {PlcId} with {TagCount} tags.", this.PlcId, batchWriteTags.Count);
            this.ErrorCounter--;
            if (this.ErrorCounter < 0)
            {
                this.ErrorCounter = 0; // Ensure ErrorCounter does not go negative
            }
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "Exception in WriteBatchValuesPlcAsync for PLC {PlcId} IP {IpAddress}", this.PlcId, this.IpAddress);
            success = false;
        }

        if (success)
        {
            this.ErrorCounter--;
            if (this.ErrorCounter < 0)
            {
                this.ErrorCounter = 0; // Ensure ErrorCounter does not go negative
            }

            return Result.Success();
        }

        await this.IncrementErrorCounterAndFireDisconnect(cancellationToken);

        var result = await this.DownloadReferencesAsync(cancellationToken);
        this.Retry = false;
        return Result.Success();
    }

    private async Task IncrementErrorCounterAndFireDisconnect(CancellationToken cancellationToken)
    {
        if (!this.Retry)
        {
            return;
        }

        this.logger?.LogWarning("Batch operation failed for PLC {PlcId} IP {IpAddress}. Retrying...", this.PlcId, this.IpAddress);

        this.ErrorCounter++;
        if (this.ErrorCounter > 2 * this.ThreesHold)
        {
            this.ErrorCounter = this.ThreesHold;
            await this.DisconnectWithOutNotificationAsync(cancellationToken).ConfigureAwait(false);
            await Task.Delay(25, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<ControllerMonitor> GetPlcMonitorAsync(CancellationToken cancellationToken)
    {
        try
        {
            var (partNumber, barCode, cyclesOk) = await this.GetBarCodeAsync(cancellationToken).ConfigureAwait(false);

            var monitor = new ControllerMonitor
            {
                PlcId = this.PlcId,
                MachineId = this.PlcDetails.MachineId,
                Description = this.PlcDetails.Name,
                Name = this.Name,
                HeartBeat = this.HeartBeat,
                IpAddress = this.PlcDetails.IpAddress,
                PartNumber = partNumber,
                Label = barCode,
                CyclesOk = cyclesOk,
                TimeStamp = this.dateTimeMachine.Now.ToLocalTime(),
            };

            return monitor;
        }
        catch (TaskCanceledException)
        {
            // Log task cancellation
            this.logger?.LogWarning("Task was canceled while getting PLC monitor data. PLC ID: {PlcId}, IP: {IpAddress}", this.PlcId, this.IpAddress);
        }
        catch (Exception ex)
        {
            // Log the error
            this.logger?.LogError(ex, "Error while getting PLC monitor data. PLC ID: {PlcId}, IP: {IpAddress}", this.PlcId, this.IpAddress);
        }

        return new ControllerMonitor
        {
            PlcId = this.PlcId,
            MachineId = this.PlcDetails.MachineId,
            Description = this.PlcDetails.Name,
            Name = this.Name,
            HeartBeat = this.HeartBeat,
            IpAddress = this.PlcDetails.IpAddress,
            PartNumber = this.PartNumber,
            Label = this.BarCode,
            CyclesOk = this.CyclesOk,
            TimeStamp = this.dateTimeMachine.Now.ToLocalTime(),
        };
    }

    public async Task<(string, string, int)> GetBarCodeAsync(CancellationToken cancellationToken)
    {
        var result = await this.ReadBarCodePartNumberAsync(default);

        this.BarCode = result[nameof(this.BarCode)];
        this.PartNumber = result[nameof(this.PartNumber)];

        this.CyclesOk = await this.ReadIntTagAsync(nameof(this.CyclesOk), cancellationToken).ConfigureAwait(false);
        return (this.BarCode, this.PartNumber, this.CyclesOk);
    }

    /// <inheritdoc/>
    public async Task<int> SetPlcIdAsync(short value, CancellationToken cancellationToken)
    {
        // changed to Write short to avoid error on s7 library and to avoid changing every plc related class to short
        // plcmachines, variables, registers, etc
        await this.WriteShortTagAsync(nameof(this.PlcId), value, cancellationToken).ConfigureAwait(false);
        return this.PlcId;
    }

    /// <inheritdoc/>
    public async Task<int> GetPlcIdAsync(CancellationToken cancellationToken)
    {
        // changed to read short to avoid error on s7 library and to avoid changing every plc related class to short
        // plcmachines, variables, registers, etc
        this.PlcId = await this.ReadShortTagAsync(nameof(this.PlcId), cancellationToken).ConfigureAwait(false);
        return this.PlcId;
    }

    /// <inheritdoc/>
    public async Task<string> ResetCommandAsync(CancellationToken cancellationToken)
    {
        this.IndTraceTags[nameof(this.Command)].Value = (short)0;
        await this.WriteShortTagAsync(nameof(this.Command), (short)0, cancellationToken).ConfigureAwait(false);
        return string.Empty;
    }

    /// <inheritdoc/>
    public async Task<string> SetFeedBackAsync(short value, CancellationToken cancellationToken)
    {
        this.IndTraceTags[nameof(this.CommandFeedback)].Value = (short)value;
        await this.WriteShortTagAsync(nameof(this.CommandFeedback), (short)value, cancellationToken).ConfigureAwait(false);

        return string.Empty;
    }

    /// <inheritdoc/>
    public async Task<string> SetBarCodeAsync(string value, CancellationToken cancellationToken)
    {
        await this.WriteStringTagAsync(nameof(this.BarCode), value, cancellationToken).ConfigureAwait(false);
        return value;
    }

    /// <inheritdoc/>
    public Task<bool> SetUpAsync(CancellationToken cancellationToken)
    {
        _ = this.PlcDetails ?? throw new ArgumentNullException(nameof(this.PlcDetails));
        this.PlcId = this.PlcDetails.PlcId;
        this.MachineId = this.PlcDetails.MachineId;
        this.Name = this.PlcDetails.Name;

        _ = this.PlcDetails.VariablesGroups ?? throw new InvalidOperationException("PLC detail is missing required VariableGroups.");
        _ = this.PlcDetails.Variables ?? throw new InvalidOperationException("PLC detail is missing required Variables.");

        this.IsInitialized = false;
        this.Configured = false;

        this.ConfigureController();
        this.IndTraceTags = this.ConfigureTags(this.PlcDetails);

        this.References = ConfigureRegisterTags(this.PlcDetails.Variables, TagsGroups.ReferenceTags);
        this.Registers = ConfigureRegisterTags(this.PlcDetails.Variables, TagsGroups.RegisterTags);
        this.PrepareRegisterForBatchReadTagInfos();

        // If we try to read the performance tags, we will get an error if the tag is not configured on the PLC
        // So we will not configure the performance tags if the OEE is not enabled
        // [FIX] ABR 24 MAY 2025
        // We want to disable this //16 jun 2025
        // TODO: [URGENT] : Take the decision to enable or disable the OEE tags on this code or on another service
        if (this.IsOeeEnabled)
        {
            this.Perfomances = ConfigureRegisterTags(this.PlcDetails.Variables, TagsGroups.PerformanceTags);
            this.PreparePerformanceForBatchReadTagInfos();
        }

        this.ConfigureEventTags();

        this.Configured = true;
        return Task.FromResult(this.Configured);
    }

    /// <inheritdoc/>
    public async Task<bool> ValidateThatTheTagExistOnTheController(CancellationToken cancellationToken)
    {
        var result = await this.TryReadAllVariablesAsync(cancellationToken);

        foreach (var kvp in this.PlcDetails.Variables)
        {
            var name = kvp.Key;
            var variable = kvp.Value;

            this.logger?.LogInformation(
                "[ValidationResult] Name: {Name}, Address: {Address}, Validated: {Validated}",
                name,
                variable.Address,
                variable.Validated ?? false);
        }

        return this.PlcDetails.Variables.All(v => v.Value.Validated == true);
    }

    /// <inheritdoc/>
    public async Task<bool> ConnectAndCreateNotificationsAsync(CancellationToken cancellationToken)
    {
        var result = false;
        try
        {
            result = await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var capturedException = ExceptionDispatchInfo.Capture(ex);
            this.logger?.LogError(ex, "Error while connecting to the PLC {PlcId} IP {IpAddress}", this.PlcId, this.IpAddress);

            // 🔴 Fixed: Remove Console.WriteLine anti-pattern - already logged above
            capturedException.Throw();
        }

        if (result)
        {
            this.CreateNotifications(this.eventVariables);
        }

        this.EnableSimulation = this.PlcDetails.EnableSimulation;
        if (this.EnableSimulation)
        {
            return this.Configured;
        }

        return result;
    }

    // 26 APRIL 2025
    // CHANGES MADE TO UPLOAD REGISTER ON JUST ONE GO TO THE PLC
    // MAKE SURE EVERYTHING WORKS
    // TODO: [CRITICAL] MAKE SURE THE VALUES ARE STORED ON THE DATABASE
    // Validation made on the PLC side and on the database side
    // by Lisset Camacho
    // Test on 28, 29 y 30 of April 2025

    /// <inheritdoc/>
    public async Task<Result> ReadRegistersBulkAsync(CancellationToken cancellationToken)
    {
        // First clear the batch read bad register
        List<(string Alias, Type Type, int VariableId)> batchBadRegister = [];

        if (this.EnableSimulation)
        {
            return Result.Success();
        }

        if (this.ErrorCounter > this.ThreesHold)
        {
            await this.DisconnectWithOutNotificationAsync(cancellationToken).ConfigureAwait(false);
            this.ErrorCounter = 0;
            await Task.Delay(20, cancellationToken);
        }

        if (!this.IsInitialized || !this.IsConnected)
        {
            await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
        }

        try
        {
            var batchResults = await this.Controller.GetBatchValuesPlcAsync(this.BatchReadRegister, cancellationToken);

            foreach (var result in batchResults)
            {
                if (this.AliasRegisterToKeyMap.TryGetValue(result.Alias, out var key) && this.Registers.TryGetValue(key, out var register))
                {
                    register.Value = result.ValueString;
                    register.StatusValueId = result.Status;
                    register.TimeStamp = this.dateTimeMachine.Now.ToLocalTime();

                    // 🔵 Structured logging: log if Status != 1
                    if (result.Status != 1)
                    {
                        this.logger?.LogWarning(
                            "Register read completed with warning. Name: {Alias}, Status: {Status}, ValueString: {ValueString}",
                            result.Alias, result.Status, result.ValueString);
                        {
                            var badRegister = this.BatchReadRegister.FirstOrDefault(x => x.Alias == result.Alias);

                            batchBadRegister.Add(badRegister);
                        }
                    }
                }
                else
                {
                    // 🔵 Structured logging: alias not found
                    this.logger?.LogWarning("Tag not found in Registers. Name: {Alias}", result.Alias);
                }
            }

            var badTagCount = batchBadRegister.Count;
            if (badTagCount == 0)
            {
                this.ErrorCounter--;
                if (this.ErrorCounter < 0)
                {
                    this.ErrorCounter = 0; // Ensure ErrorCounter does not go negative
                }

                return Result.Success();
            }

            await this.IncrementErrorCounterAndFireDisconnect(cancellationToken);
            var failureRatio = this.CalculateFailureRate(badTagCount);

            if (failureRatio <= 0.6)
            {
                this.logger?.LogWarning("Bulk read with failures Trying second time read No tags {count} .", badTagCount);
                await this.FallBackToBulkRead(cancellationToken, failureRatio, batchBadRegister);
            }
            else
            {
                this.logger?.LogWarning("Bulk read with many failures Trying second time read No tags {count} .", badTagCount);
                await this.UploadRegistersAsync(batchBadRegister, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            // 🔴 Structured error logging
            this.logger?.LogError(ex, "Exception occurred during ReadRegistersBulkAsync");
            return Result.WithFailure($"Error uploading registers: {ex.Message}");
        }

        return Result.Success();
    }

    private async Task<Result> FallBackToBulkRead(CancellationToken cancellationToken, double failureRatio, List<(string Alias, Type Type, int VariableId)> batchBadRegister)
    {
        this.logger?.LogInformation("Retrying bulk read (only {Ratio:P1} failed).", failureRatio);
        try
        {
            var batchResults = await this.Controller.GetBatchValuesPlcAsync(batchBadRegister, cancellationToken);

            foreach (var result in batchResults)
            {
                if (this.AliasRegisterToKeyMap.TryGetValue(result.Alias, out var key) && this.Registers.TryGetValue(key, out var register))
                {
                    register.Value = result.ValueString;
                    register.StatusValueId = result.Status;
                    register.TimeStamp = this.dateTimeMachine.Now.ToLocalTime();

                    // 🔵 Structured logging: log if Status != 1
                    if (result.Status == 1)
                    {
                        continue;
                    }

                    // 🔵 Structured logging: register read completed with warning
                    this.logger?.LogWarning(
                        "Register read completed with warning.Escalating to per-register reads Name: {Alias}, Status: {Status}, ValueString: {ValueString}",
                        result.Alias, result.Status, result.ValueString);

                    register.Value = await this.ReadRegisterAsync(cancellationToken, register, result);
                    register.StatusValueId = 1;
                    register.TimeStamp = this.dateTimeMachine.Now.ToLocalTime();
                }
                else
                {
                    // 🔵 Structured logging: alias not found
                    this.logger?.LogWarning("Tag not found in Registers. Name: {Alias}", result.Alias);
                }
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "Exception during batch read for PLC {PlcId} IP {IpAddress}", this.PlcId, this.IpAddress);
        }

        return Result.Success();
    }

    private double CalculateFailureRate(int failureCount)
    {
        var totalCount = this.BatchReadRegister.Count;
        var failureRatio = (double)failureCount / totalCount;
        return failureRatio;
    }

    public async Task UploadRegistersAsync(List<(string Alias, Type Type, int VariableId)> batchBadRegister, CancellationToken cancellationToken)
    {
        if (this.EnableSimulation)
        {
            return;
        }

        if (!this.IsInitialized || !this.IsConnected)
        {
            await this.ConnectAndCreateNotificationsAsync(cancellationToken);
        }

        var badKeys = new HashSet<string>(batchBadRegister.Select(x => x.Alias));

        foreach (var (key, register) in this.Registers)
        {
            if (!badKeys.Contains(key))
            {
                continue;
            }

            if (!this.PlcDetails.Variables.TryGetValue(key, out var detailVariable))
            {
                throw new Exception(
                    $"Variables does not contains key {key} {register.Name} Variable ID {register.VariableId}");
            }

            if (!this.IndTraceTags.ContainsKey(key))
            {
                throw new Exception(
                    $"Tag does not contain key  {key} {register.Name} Variable ID {register.VariableId}");
            }

            try
            {
                await this.IndTraceTags[key].UploadValueAsync(this.Controller).ConfigureAwait(false);

                if (this.IndTraceTags[key].Value == null)
                {
                    throw new ArgumentNullException(
                        nameof(register.Value),
                        $"UploadRegistersAsync Value can not be null key {key}, address {detailVariable.Alias} ");
                }

                register.Value = this.IndTraceTags[key].Value?.ToString() ?? string.Empty;
                register.TimeStamp = this.dateTimeMachine.Now.ToLocalTime();
            }
            catch (Exception ex)
            {
                var capturedException = ExceptionDispatchInfo.Capture(ex);
                this.logger.LogError(
                    ex,
                    "Error while uploading register plc {PlcId} IP {IpAddress} TAG with key:{Name} {Address} ",
                    this.PlcId, this.IpAddress, register.Name, this.IndTraceTags[key].Variable.Alias);

                // 🔴 Fixed: Use proper logging instead of Console.WriteLine anti-pattern
                this.logger.LogError(ex, "Error while uploading register for PLC {PlcId} IP {IpAddress} TAG {Name} {Address}",
                    this.PlcId, this.IpAddress, register.Name, this.IndTraceTags[key].Variable.Alias);
            }
        }
    }

    private async Task<string> ReadRegisterAsync(CancellationToken cancellationToken, Register register, PlcBatchReadResult result)
    {
        // Attempt to read the value directly if batch read failed
        {
            try
            {
                switch (register.DataType.ToLowerInvariant())
                {
                    case "int":
                        register.Value =
                            (await this.Controller.GetValue<int>(result.Alias, cancellationToken))
                            .ToString();
                        break;

                    case "short":
                    case "word":
                        register.Value =
                            (await this.Controller.GetValue<short>(result.Alias, cancellationToken))
                            .ToString();
                        break;

                    case "float":
                    case "real":
                        register.Value =
                            (await this.Controller.GetValue<float>(result.Alias, cancellationToken))
                            .ToString(CultureInfo.InvariantCulture);
                        break;

                    case "double":
                        register.Value =
                            (await this.Controller.GetValue<double>(result.Alias, cancellationToken))
                            .ToString(CultureInfo.InvariantCulture);
                        break;

                    case "bool":
                        register.Value =
                            (await this.Controller.GetValue<bool>(result.Alias, cancellationToken))
                            .ToString();
                        break;

                    case "byte":
                        register.Value =
                            (await this.Controller.GetValue<byte>(result.Alias, cancellationToken))
                            .ToString();
                        break;

                    case "dword":
                        register.Value =
                            (await this.Controller.GetValue<uint>(result.Alias, cancellationToken))
                            .ToString();
                        break;

                    case "string":
                        register.Value =
                            await this.Controller.GetValue<string>(result.Alias, cancellationToken);
                        break;

                    default:
                        register.Value = string.Empty;
                        this.logger.LogError(
                            "Unsupported NetType {NetType}. Sending raw string.",
                            register.DataType);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Retry failed. Escalating to per-register reads");
            }

            return register.Value;
        }
    }

    private static readonly Random Random = new();

    /// <inheritdoc/>
    public async Task<string> SimulateCommandAsync(SimulatedCommand command, CancellationToken cancellationToken)
    {
        // Add logic to detect if we are running in a virtual network
        // Using the Tailscae adapter
        // If we are running in a virtual network, we are going to simulate performancde data
        // when we are on station 100
        if (this.PlcId == 100 && !CheckIfNetworkInterfaceExists("Tailscale", this.logger))
        {
            // Network interface found, so we are not in a virtual network, on machine 100
            // we are going to simulate performance data
        }

        try
        {
            try
            {
                this.logger.LogInformation("Sending information to the plc {plcid} Part Number {PartNumber}", this.PlcId, command.PartNumber);
                this.IndTraceTags[nameof(this.PartNumber)].Value = command.PartNumber;
                await this.WriteStringTagAsync(nameof(this.PartNumber), command.PartNumber, cancellationToken)
                    .ConfigureAwait(false);

                this.logger.LogInformation("Sending information to the plc {plcid} Part Number {BarCode}", this.PlcId, command.BarCode);
                this.IndTraceTags[nameof(this.BarCode)].Value = command.BarCode;
                await this.WriteStringTagAsync(nameof(this.BarCode), command.BarCode, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Exception occurred while simulation command writing string tags for PartNumber and BarCode");
            }

            try
            {
                this.logger.LogInformation("Sending information to the plc {plcid} PartStatusPlc {PartStatusPlc}", this.PlcId, command.PartStatusPlc);
                this.IndTraceTags[nameof(this.PartStatus) + "Plc"].Value = command.PartStatusPlc;
                await this.WriteShortTagAsync(nameof(this.PartStatus) + "Plc", command.PartStatusPlc, cancellationToken).ConfigureAwait(false);

                this.logger.LogInformation("Sending information to the plc {plcid} CycleStatusPlc {PartStatusPlc}", this.PlcId, command.CycleStatusPlc);
                this.IndTraceTags[nameof(this.CycleStatus) + "Plc"].Value = command.CycleStatusPlc;
                await this.WriteShortTagAsync(nameof(this.CycleStatus) + "Plc", command.CycleStatusPlc, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Exception occurred while simulation command writing short tags for PartStatusPlc and CycleStatusPlc");
            }

            try
            {
                this.logger.LogInformation("Sending information to the plc {plcid} PartStatus {PartStatusPlc}", this.PlcId, command.PartStatus);
                this.IndTraceTags[nameof(this.PartStatus)].Value = command.PartStatus;
                await this.WriteIntTagAsync(nameof(this.PartStatus), command.PartStatus, cancellationToken).ConfigureAwait(false);

                this.logger.LogInformation("Sending information to the plc {plcid} CycleStatus {PartStatusPlc}", this.PlcId, command.CycleStatus);
                this.IndTraceTags[nameof(this.CycleStatus)].Value = command.PartStatus;
                await this.WriteIntTagAsync(nameof(this.CycleStatus), command.CycleStatus, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Exception occurred while simulation command writing Int tags for PartStatus and CycleStatus");
            }

            // This is written to WathchDog to avoid the watchdog to be disabled
            // the name say ShifOk, because it was used for the shift ok command
            try
            {
                this.IndTraceTags["ShiftOk"].Value = command.WatchDog;
                await this.WriteIntTagAsync("ShiftOk", command.WatchDog, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // this will be a silent error, because the tag is not always available
                this.logger.LogError(ex, "Exception occurred while simulation command writing int tags for ShiftOk");
            }

            this.logger.LogInformation("waiting a second to send the Request {Request}", command.Command);
            await Task.Delay(100, cancellationToken).ConfigureAwait(false);

            try
            {
                this.IndTraceTags[nameof(this.Command)].Value = command.Command;
                await this.WriteShortTagAsync(nameof(this.Command), command.Command, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Exception occurred while simulation command writing short tags for Command: {Command}", command.Command);
                throw;
            }

            if (command.SimulateReference)
            {
                await this.SimulateAndDownloadRegistersAsync(cancellationToken);
            }

            this.logger.LogInformation("Information send to the plc {plcid}", this.PlcId);
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "Exception occurred while simulation command");
        }

        return string.Empty;
    }

    public static bool CheckIfNetworkInterfaceExists(string interfaceName, ILogger logger)
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var networkInterface in networkInterfaces)
        {
            if (networkInterface.Name.Equals(interfaceName, StringComparison.OrdinalIgnoreCase))
            {
                // Log the details of the found network interface
                logger.LogInformation("Interface Name: {Name}", networkInterface.Name);
                logger.LogInformation("Description: {Description}", networkInterface.Description);
                logger.LogInformation("Status: {Status}", networkInterface.OperationalStatus);
                logger.LogInformation("Speed: {Speed} Mbps", networkInterface.Speed / 1_000_000);

                var ipProperties = networkInterface.GetIPProperties();
                var ipAddresses = ipProperties.UnicastAddresses.Select(ua => ua.Address.ToString()).ToArray();

                if (ipAddresses.Any())
                {
                    logger.LogInformation("IP Addresses:");
                    foreach (var ipAddress in ipAddresses)
                    {
                        logger.LogInformation("  - {IpAddress}", ipAddress);
                    }
                }

                logger.LogInformation(new string('-', 40));

                // Return true as the network interface exists
                return true;
            }
        }

        // Return false as the network interface does not exist
        return false;
    }

    public async Task<Result> SimulateAndDownloadRegistersAsync(CancellationToken cancellationToken)
    {
        if (this.EnableSimulation)
        {
            return Result.Success();
        }

        if (!this.IsInitialized || !this.IsConnected)
        {
            await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
        }

        var batchWriteTags = new List<PlcBatchWriteTag>();

        try
        {
            foreach (var (key, register) in this.Registers)
            {
                if (register.Value == null)
                {
                    continue;
                }

                if (!this.PlcDetails.Variables.ContainsKey(key))
                {
                    this.logger?.LogWarning("Variable not found in PLC definition for Register {Key}.", key);
                    continue;
                }

                if (!this.IndTraceTags.TryGetValue(key, out var tag))
                {
                    this.logger?.LogWarning("IndTraceTag not found for Register {Key}.", key);
                    continue;
                }

                // Step 1 & 2: Simulate string value and assign to register
                var simulatedString = this.GenerateRandomStringValue(register.Value);
                register.Value = simulatedString;

                // Step 3: Parse string into correct .NET type for uploading
                var parsedValue = this.ParseStringToNetType(simulatedString, register.DataType);

                tag.Value = parsedValue ?? string.Empty;

                if (this.logger is not null)
                {
                    this.logger.LogInformation("Simulated Register {Key} with ValueString: {ValueString}", key, simulatedString);
                }

                if (tag.NetType != Type.GetType("string"))
                {
                    // Step 4: Add to batch
                    batchWriteTags.Add(new PlcBatchWriteTag(tag.Variable.Alias, tag.NetType, parsedValue));
                }
                else
                {
                    // for string type, we need to convert it to byte array
                }
            }

            if (!batchWriteTags.Any())
            {
                if (this.logger is not null)
                {
                    this.logger.LogInformation("No simulated registers to write for PLC {PlcId}.", this.PlcId);
                }
                return Result.Success();
            }

            // Step 5: Write batch
            var success = await this.Controller.s7Connector.WriteBatchValuesPlcAsync(batchWriteTags, cancellationToken).ConfigureAwait(false);

            this.logger?.LogInformation("Simulated batch writing {TagCount} registers to PLC {PlcId}.", batchWriteTags.Count, this.PlcId);

            return success ? Result.Success() : Result.WithFailure("Failed to write simulated register values.");
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "Error simulating and writing registers");
            return Result.WithFailure($"Error during simulation download: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates a believable random string value based on the current register value type.
    /// </summary>
    private string GenerateRandomStringValue(object? currentValue)
    {
        if (currentValue == null)
        {
            return Random.Next(0, 100).ToString();
        }

        return currentValue switch
        {
            int => Random.Next(0, 10_000).ToString(),
            short => ((short)Random.Next(short.MinValue, short.MaxValue)).ToString(),
            float => (Random.NextDouble() * 100.0).ToString("F2"),
            double => (Random.NextDouble() * 1000.0).ToString("F2"),
            bool => (Random.Next(0, 2) == 0).ToString(),
            string => $"00Sim_{Random.Next(1000, 9999)}",
            _ => Random.Next(0, 100).ToString(),
        };
    }

    /// <summary>
    /// Parses a string back into the correct .NET type based on PLC NetType.
    /// </summary>
    private object? ParseStringToNetType(string value, string netType)
    {
        try
        {
            switch (netType.ToLowerInvariant())
            {
                case "int":
                    return int.TryParse(value, out var intVal) ? intVal : 0;

                case "short":
                case "word":
                    return short.TryParse(value, out var shortVal) ? shortVal : (short)0;

                case "float":
                case "real":
                    return float.TryParse(value, out var floatVal) ? floatVal : 0.0f;

                case "double":
                    return double.TryParse(value, out var doubleVal) ? doubleVal : 0.0;

                case "bool":
                    return bool.TryParse(value, out var boolVal) ? boolVal : false;

                case "byte":
                    return byte.TryParse(value, out var byteVal) ? byteVal : (byte)0;

                case "dword":
                    return uint.TryParse(value, out var uintVal) ? uintVal : 0u;

                case "string":
                    return value;

                default:
                    this.logger?.LogWarning("Unsupported NetType {NetType}. Sending raw string.", netType);
                    return value;
            }
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "Error parsing simulated value {ValueString} for NetType {NetType}", value, netType);
            return value; // Fallback: send raw string
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DataFromPlc>> UploadCommandDataFromController(CancellationToken cancellationToken)
    {
        var tempResult = new DataFromPlc();

        try
        {
            if (this.EnableSimulation)
            {
                return Result<DataFromPlc>.Success(new DataFromPlc());
            }

            if (!this.IsInitialized || !this.IsConnected)
            {
                await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
            }

            // Individual reads with error handling
            // ABR change to read on just one trip to the plc
            // 27/ABRIL/2027

            // Make sure Command has the command form controller
            var result = await this.ReadBarCodePartNumberAsync(cancellationToken);

            tempResult.WatchDogTime = await this.CheckIfWatchDogIsEnabled(cancellationToken);

            this.BarCode = result[nameof(this.BarCode)];
            this.PartNumber = result[nameof(this.PartNumber)];
            tempResult.Command = this.Command;
            tempResult.MachineId = this.MachineId;
            tempResult.Command = this.Command;
            tempResult.BarCode = this.BarCode;
            tempResult.PartNumber = this.PartNumber;

            tempResult.MachineId = this.PlcId;

            return Result<DataFromPlc>.Success(tempResult);
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "Error while connecting to the PLC {PlcId} IP {IpAddress}", this.PlcId, this.IpAddress);
            return Result<DataFromPlc>.WithFailure(ex.Message, tempResult);
        }
    }

    private async Task<WatchDog> CheckIfWatchDogIsEnabled(CancellationToken cancellationToken)
    {
        // If debugger isn't attached, watchdog remains enabled
        if (!Debugger.IsAttached)
        {
            return WatchDog.Enable;
        }

        IIndTraceTagRx watchDogTag;

        // Only check the tag if it's defined
        if (!this.IndTraceTags.TryGetValue("ShiftOk", out watchDogTag!))
        {
            return WatchDog.Enable;
        }

        try
        {
            var watchDogValue = await this.ReadIntTagAsync(watchDogTag.Variable.Address, cancellationToken);

            if (watchDogValue == DisableTimeoutMagic)
            {
                this.logger.LogInformation("Watchdog disabled for debugging.");
                return WatchDog.Disable;
            }
        }
        catch (Exception ex)
        {
            this.logger?.LogWarning(ex, "Error reading WatchDog override flag — defaulting to enabled.");
        }

        return WatchDog.Enable;
    }

    /// <inheritdoc/>
    public async Task<Result<PerformanceDataCommand>> ReadPerformanceDataFromPlcAsync(CancellationToken cancellationToken)
    {
        // TODO [URGENT] : Implement this method to read performance data from the PLC
        // TODO [TODAY] :
        // IMPLEMENT A BATCH READ FOR THE PERFORMANCE DATA
        // THE PEFORMANCE DATA WILL BE READ FROM THE PLC
        // THE GROUP READ IS THE
        var performance = new PerformanceDataCommand()
        {
            MachineId = this.PlcId,
            PlcId = this.PlcId,
        };

        if (this.EnableSimulation)
        {
            this.Perfomances[nameof(this.MachineId)].Value = this.PlcId.ToString();
            this.Perfomances[nameof(this.PlcId)].Value = this.PlcId.ToString();

            performance.MachineId = this.PlcId;
            performance.PlcId = this.PlcId;
            return performance;
        }

        if (!this.IsInitialized || !this.IsConnected)
        {
            await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
        }

        try
        {
            var batchResults = await this.Controller.GetBatchValuesPlcAsync(
                this.BatchReadPerformances, cancellationToken);

            foreach (var result in batchResults)
            {
                if (this.AliasPerformancesToKeyMap.TryGetValue(result.Alias, out var key) && this.Perfomances.TryGetValue(key, out var performances))
                {
                    performances.Value = result.ValueString;
                    performances.StatusValueId = result.Status;
                    performances.TimeStamp = this.dateTimeMachine.Now.ToLocalTime();

                    // 🔵 Structured logging: log if Status != 1
                    if (result.Status != 1)
                    {
                        this.logger?.LogWarning(
                            "Register read completed with warning. Name: {Alias}, Status: {Status}, ValueString: {ValueString}",
                            result.Alias, result.Status, result.ValueString);
                    }
                }
                else
                {
                    // 🔵 Structured logging: alias not found
                    this.logger?.LogWarning(
                        "Tag not found in Registers. Name: {Alias}",
                        result.Alias);
                }
            }

            var performanceResult = PerformanceDataCommand.FromPlc(this.Perfomances);
            if (performanceResult.IsFailure)
            {
                return performanceResult;
            }

            performance = performanceResult.Value;
            if (performance is not null)
            {
                performance.MachineId = this.PlcId;
                performance.PlcId = this.PlcId;
            }

            return performance is not null
                ? Result<PerformanceDataCommand>.Success(performance)
                : Result<PerformanceDataCommand>.WithFailure("Performance data is null");
        }
        catch (Exception ex)
        {
            // 🔴 Structured error logging
            this.logger?.LogError(ex, "Exception occurred during ReadRegistersBulkAsync");
            return Result<PerformanceDataCommand>.WithFailure($"Error uploading registers: {ex.Message}", performance);
        }
    }

    private async Task<Dictionary<string, string>> ReadBarCodePartNumberAsync(CancellationToken cancellationToken)
    {
        var tagsToRead = new List<string>
        {
            nameof(this.BarCode),
            nameof(this.PartNumber),
        };

        var result = await this.ReadStringTagAsync(tagsToRead, cancellationToken);

        return result;
    }

    private static IDictionary<string, Register> ConfigureRegisterTags(IDictionary<string, Variable> variables, TagsGroups tagGroup)
    {
        var collectionTags = variables.Where((kvp) =>
                kvp.Value.VariableGroupId == tagGroup)
            .ToDictionary(
                kvp =>
                kvp.Key, kvp => kvp.Value);

        if (collectionTags is null)
        {
            throw new Exception($"This gateway need {nameof(tagGroup)} Tag");
        }

        if (collectionTags.IsNullOrEmpty())
        {
            throw new Exception($"This gateway need  {nameof(tagGroup)} Tag");
        }

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

    private static string ParseIpAddress(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
        {
            throw new ArgumentOutOfRangeException(nameof(ip), "IpAddress must not be null");
        }

        var ipTemp = Convert.ToInt16(ip.Replace(",", ".").Split('.')[0]).ToString();
        ipTemp += "." + Convert.ToInt16(ip.Replace(",", ".").Split('.')[1]);
        ipTemp += "." + Convert.ToInt16(ip.Replace(",", ".").Split('.')[2]);
        ipTemp += "." + Convert.ToInt16(ip.Replace(",", ".").Split('.')[3]);

        return IPAddress.Parse(ipTemp).ToString();
    }

    private void ConfigureController()
    {
        this.IpAddress = ParseIpAddress(this.PlcDetails.IpAddress);

        var optionsList = JsonSerializer.Deserialize<List<PlcSiemensOptions>>(this.PlcDetails.Options);
        var options = optionsList?.FirstOrDefault();

        if (options is null)
        {
            throw new Exception($"Options for plc invalid");
        }

        this.Controller ??= new Sharp7Plc(this.IpAddress, options.Rack, options.Slot);

        this.Controller.Logger = this.logger;
    }

    private void PrepareRegisterForBatchReadTagInfos()
    {
        this.preparedRegisterTagInfos = this.Registers
            .Where(pair => this.PlcDetails.Variables.ContainsKey(pair.Key))
            .Select(pair =>
            {
                var variable = this.PlcDetails.Variables[pair.Key];
                return (Alias: variable.Alias, Type: Type.GetType(variable.NetType) ?? typeof(object), VariableId: variable.VariableId, Key: pair.Key);
            })
            .ToList();

        this.AliasRegisterToKeyMap = this.preparedRegisterTagInfos.ToDictionary(x => x.Alias, x => x.Key);

        this.BatchReadRegister = this.preparedRegisterTagInfos
            .Select(x => (x.Alias, x.Type, x.VariableId))
            .ToList();
    }

    private void PreparePerformanceForBatchReadTagInfos()
    {
        this.preparedPerformancesTagInfos = this.Perfomances
            .Where(pair => this.PlcDetails.Variables.ContainsKey(pair.Key))
            .Select(pair =>
            {
                var variable = this.PlcDetails.Variables[pair.Key];
                return (Alias: variable.Alias, Type: Type.GetType(variable.NetType) ?? typeof(object), VariableId: variable.VariableId, Key: pair.Key);
            })
            .ToList();

        this.AliasPerformancesToKeyMap = this.preparedPerformancesTagInfos.ToDictionary(x => x.Alias, x => x.Key);

        this.BatchReadPerformances = this.preparedPerformancesTagInfos
            .Select(x => (x.Alias, x.Type, x.VariableId))
            .ToList();
    }

    private void ConfigureEventTags()
    {
        // Retrieve event tags
        var tagCount = this.PlcDetails.Variables.Count(kvp =>
            kvp.Value.VariableGroupId == TagsGroups.EventTags);
        if (tagCount != 4)
        {
            throw new Exception("This gateway need four event tag");
        }

        this.eventVariables = this.PlcDetails.Variables
            .Where(kvp => kvp.Value.VariableGroupId == TagsGroups.EventTags)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        if (this.eventVariables is null)
        {
            throw new Exception("Event Tag does not exist for this PLC");
        }
    }

    private void CreateNotifications(Dictionary<string, Variable> eventVariable)
    {
        // Check that the event variables are not null and have the correct count
        // Check that the event variables contain the required keys
        if (eventVariable is null)
        {
            throw new ArgumentNullException(nameof(eventVariable), "Event variables cannot be null");
        }

        if (eventVariable.Count != 4)
        {
            throw new Exception("This gateway need four event tag");
        }

        if (!eventVariable.ContainsKey(nameof(this.Command)))
        {
            throw new InvalidOperationException("Gateway is missing the Command event tag.");
        }

        if (!eventVariable.ContainsKey(nameof(this.HeartBeat)))
        {
            throw new InvalidOperationException("Gateway is missing the HeartBeat event tag.");
        }

        var eventCommandChanged = this.CreateNotificationService(eventVariable, nameof(this.Command));

        // Subscribe
        eventCommandChanged.Subscribe(e =>
        {
            this.Command = e;
            this.subCommand.OnNext(this);
        });

        // Create notification
        var eventHeartBeat = this.CreateNotificationService(eventVariable, nameof(this.HeartBeat));

        eventHeartBeat.Subscribe(
            e =>
            {
                this.HeartBeat = e;
                this.subHeartBeat.OnNext(this);
            });
    }

    private IObservable<short> CreateNotificationService(Dictionary<string, Variable> eventVariable, string nameVariable)
    {
        // abr 12 agosto 2024
        // changed the subscription mode to OnChange to avoid the recurrent notification
        // this must be the result of a wrong migration from the old version of S7Rx
        // Now is functional again
        return this.Controller.CreateNotification<short>(eventVariable[nameVariable].Alias, TransmissionMode.OnChange);
    }

    private IDictionary<string, IIndTraceTagRx> ConfigureTags(PlcDto plcDto)
    {
        var indTraceTags = new Dictionary<string, IIndTraceTagRx>();

        foreach (var (key, variable) in plcDto.Variables)
        {
            if (indTraceTags.ContainsKey(variable.Name))
            {
                this.logger.LogError(
                    "Duplicated tag detected while adding tags. RecipeId: {EntitieId}, Name: {VariableName}, Address: {VariableAddress}",
                    variable.VariableId,
                    variable.Name,
                    variable.Address);

                throw new DuplicateTagException(variable.VariableId, variable.Name, variable.Address);
            }

            indTraceTags.Add(variable.Name, new IndTraceTagRx(variable, plcDto.EnableSimulation));
        }

        return indTraceTags;
    }

    private async Task<bool> DisconnectWithOutNotificationAsync(CancellationToken cancellationToken)
    {
        if (this.EnableSimulation)
        {
            return true;
        }

        try
        {
            await this.Controller.s7Connector.DisconnectS7Client().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var capturedException = ExceptionDispatchInfo.Capture(ex);
            this.logger?.LogError(ex, "Error while disconnecting from PLC {PlcId} IP {IpAddress}", this.PlcId, this.IpAddress);

            // 🔴 Fixed: Remove Console.WriteLine anti-pattern - already logged above
            capturedException.Throw();
        }

        return Equals(this.Controller.ConnectionState, ConnectionState.Connected);
    }

    private async Task<bool> ConnectWithNotificationAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!this.IsInitialized)
            {
                await this.Controller.InitializeConnection(cancellationToken).ConfigureAwait(false);
                this.IsInitialized = true;

                this.Controller.ConnectionState.Subscribe(e => this.IsConnected = e == ConnectionState.Connected);

                Observable
                    .Interval(TimeSpan.FromSeconds(TimeOut))
                    .Subscribe(
                        x =>
                        {
                            if (this.IsConnected)
                            {
                                return;
                            }

                            this.ConnectionTimeOut = true;
                            this.logger.LogError("This controller is having troubles to connect {PlcId} IP {IpAddress}", this.PlcId, this.IpAddress);
                        });

                while (!this.IsConnected && !this.ConnectionTimeOut)
                {
                    this.logger.LogInformation($"Wait for connection to the plc {this.PlcId} IP {this.IpAddress}");
                    await Task.Delay(200, cancellationToken).ConfigureAwait(false);
                }

                // Call CreateNotifications when the connection is successful
                if (this.IsConnected)
                {
                    this.CreateNotifications(this.eventVariables);
                }
            }

            if (!this.IsInitialized || this.ConnectionTimeOut)
            {
                throw new Exception("This controller is having troubles !! :( ");
            }
        }
        catch (Exception ex)
        {
            var capturedException = ExceptionDispatchInfo.Capture(ex);
            this.logger?.LogError(ex, "Error while connecting to PLC {PlcId} IP {IpAddress}", this.PlcId, this.IpAddress);

            // 🔴 Fixed: Remove Console.WriteLine anti-pattern - already logged above
            capturedException.Throw();
        }

        return this.IsInitialized;
    }

    private void RaiseCommandChangedEvent()
    {
        this.subCommand.OnNext(this);
    }

    /// <inheritdoc/>
    public async Task<short> ReadShortTagAsync(string tagName, CancellationToken cancellationToken)
    {
        try
        {
            if (!this.IsConnected)
            {
                await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
            }

            return this.IndTraceTags!.ContainsKey(tagName)
                ? await this.Controller.GetValue<short>(this.IndTraceTags[tagName].Variable.Alias, cancellationToken).ConfigureAwait(false)
                : throw new NullReferenceException(tagName);
        }
        catch (Exception ex)
        {
            var capturedException = ExceptionDispatchInfo.Capture(ex);
            this.logger?.LogError(ex, "Error while reading tag {TagName} on PLC {PlcId} IP {IpAddress}", tagName, this.PlcId, this.IpAddress);

            // 🔴 Fixed: Remove Console.WriteLine anti-pattern - already logged above
        }

        return (short)0;
    }

    public async Task<int> ReadIntTagAsync(string tagName, CancellationToken cancellationToken)
    {
        try
        {
            if (!this.IsConnected)
            {
                await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
            }

            return this.IndTraceTags!.ContainsKey(tagName)
                ? await this.Controller.GetValue<int>(this.IndTraceTags[tagName].Variable.Alias, cancellationToken)
                    .ConfigureAwait(false)
                : 0; // throw new NullReferenceException(tagName); // this line was throwing error the tags are supposed to be checked at this point
        }
        catch (Exception ex)
        {
            var capturedException = ExceptionDispatchInfo.Capture(ex);
            this.logger?.LogError(ex, "Error while reading int tag {TagName} on PLC {PlcId} IP {IpAddress}", tagName, this.PlcId, this.IpAddress);

            // 🔴 Fixed: Remove Console.WriteLine anti-pattern - already logged above
        }

        return 0;
    }

    public async Task<Dictionary<string, string>> ReadStringTagAsync(IEnumerable<string> tagNames, CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, string>();
        string tagAlias = string.Empty;

        try
        {
            if (!this.IsConnected)
            {
                await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
            }

            var tagNamesList = tagNames as IList<string> ?? tagNames.ToList();

            var tagNameToAlias = tagNamesList
                .Where(tagName => this.IndTraceTags.TryGetValue(tagName, out var tag) && !string.IsNullOrWhiteSpace(tag.Variable?.Alias))
                .ToDictionary(tagName => tagName, tagName => this.IndTraceTags[tagName].Variable.Alias);

            if (!tagNameToAlias.Any())
            {
                return result;
            }

            var resultMultiRequest = await this.Controller.s7Connector.ExecuteMultiVarRequestGateway(tagNameToAlias.Values.ToList(), cancellationToken);

            foreach (var (tagName, alias) in tagNameToAlias)
            {
                if (!resultMultiRequest.TryGetValue(alias, out var value))
                {
                    continue;
                }

                var length = value[1];
                var stringValue = Encoding.ASCII.GetString(value, 2, length);

                result[tagName] = stringValue;
            }

            return result;
        }
        catch (Exception ex)
        {
            var capturedException = ExceptionDispatchInfo.Capture(ex);
            this.logger?.LogError(ex, "Error while reading string tags {TagAlias} on PLC {PlcId} IP {IpAddress}", tagAlias, this.PlcId, this.IpAddress);

            // 🔴 Fixed: Remove Console.WriteLine anti-pattern - already logged above
            capturedException.Throw();
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<string> ReadStringTagAsync(string tagName, CancellationToken cancellationToken)
    {
        try
        {
            if (!this.IsConnected)
            {
                await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
            }

            return this.IndTraceTags.TryGetValue(tagName, out var tag)
                 ? await this.Controller.GetValue<string>(tag.Variable.Alias, cancellationToken).ConfigureAwait(false)
                 : throw new NullReferenceException(tagName);
        }
        catch (Exception ex)
        {
            var capturedException = ExceptionDispatchInfo.Capture(ex);
            this.logger?.LogError(ex, "Error while reading string tag {TagName} on PLC {PlcId} IP {IpAddress}", tagName, this.PlcId, this.IpAddress);

            // 🔴 Fixed: Remove Console.WriteLine anti-pattern - already logged above
        }

        return string.Empty;
    }

    private async Task<int> WriteIntTagAsync(string tagName, int value, CancellationToken cancellationToken)
    {
        try
        {
            if (!this.IndTraceTags.ContainsKey(tagName))
            {
                return value;
            }

            if (!this.IsConnected)
            {
                await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
            }

            await this.Controller.SetValue<int>(this.IndTraceTags[tagName].Variable.Alias, value, cancellationToken).ConfigureAwait(false);
            return value;

            // return await Controller.GetValue<int>(IndTraceTags[tagName].Variable.Alias).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var capturedException = ExceptionDispatchInfo.Capture(ex);
            this.logger?.LogError(ex, "Error while writing int tag {TagName} on PLC {PlcId} IP {IpAddress}", tagName, this.PlcId, this.IpAddress);

            // 🔴 Fixed: Remove Console.WriteLine anti-pattern - already logged above
        }

        return value;
    }

    private async Task<short> WriteShortTagAsync(string tagName, short value, CancellationToken cancellationToken)
    {
        try
        {
            if (!this.IndTraceTags.ContainsKey(tagName))
            {
                return value;
            }

            if (!this.IsConnected)
            {
                await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
            }

            await this.Controller.SetValue<short>(this.IndTraceTags[tagName].Variable.Alias, value, cancellationToken).ConfigureAwait(false);
            return value;
        }
        catch (Exception ex)
        {
            var capturedException = ExceptionDispatchInfo.Capture(ex);
            this.logger?.LogError(ex, "Error while writing short tag {TagName} on PLC {PlcId} IP {IpAddress}", tagName, this.PlcId, this.IpAddress);

            // 🔴 Fixed: Remove Console.WriteLine anti-pattern - already logged above
        }

        return value;
    }

    private async Task<string> WriteStringTagAsync(string tagName, string value, CancellationToken cancellationToken)
    {
        try
        {
            if (!this.IndTraceTags.ContainsKey(tagName))
            {
                throw new NullReferenceException(tagName);
            }

            if (!this.IsConnected)
            {
                await this.ConnectWithNotificationAsync(cancellationToken).ConfigureAwait(false);
            }

            // Not needed anymore because the string tags where completed on the database
            // var address = $"{IndTraceTags[tagName].Variable.Alias}.{value.Length.ToString()}";
            // they when from DB249.S100 TO DB249.S100.32
            // Now we have an error when this line execute, maybe we can check if the address
            // have two or three dots to execute, but I like more have the data complete and consistent on the database
            // also this is faster to execute.
            // ABR 12 agosto 2024
            var address = this.IndTraceTags[tagName].Variable.Alias;

            await this.Controller.SetValue<string>(address, value, cancellationToken).ConfigureAwait(false);

            // [TODO] [ABR]
            // why this code is on here
            // ABR
            // SEPT 2 2025
            // This was an attemp to validate the data is written correctly
            // but for performance reason I will not do it
            // just lets add a flag to control this behavior
            // if the flag is true we will read the value after writen to validate

            if (_readback)
            {
                return await this.Controller.GetValue<string>(this.IndTraceTags[tagName].Variable.Alias, cancellationToken).ConfigureAwait(false);
            }
            return value;
        }
        catch (Exception ex)
        {
            var capturedException = ExceptionDispatchInfo.Capture(ex);
            this.logger?.LogError(ex, "Error while writing string tag {TagName} on PLC {PlcId} IP {IpAddress}", tagName, this.PlcId, this.IpAddress);

            // 🔴 Fixed: Remove Console.WriteLine anti-pattern - already logged above
        }

        return value;
    }

    /// <inheritdoc/>
    public async Task<SimulatedCommand> ReadStartUp(CancellationToken cancellationToken)
    {
        this.PartNumber = this.ReadStringTagAsync(nameof(this.PartNumber), cancellationToken).Result;
        this.BarCode = this.ReadStringTagAsync(nameof(this.BarCode), cancellationToken).Result;
        this.PartStatus = this.ReadShortTagAsync("PartStatusPlc", cancellationToken).Result;
        this.CycleStatus = this.ReadShortTagAsync("CycleStatusPlc", cancellationToken).Result;

        var command = new SimulatedCommand
        {
            MachineId = this.MachineId,
            PartNumber = this.PartNumber,
            BarCode = this.BarCode,
            Command = 8,
            PartStatus = this.PartStatus,
            CycleStatus = this.CycleStatus,
            PartStatusPlc = (short)this.PartStatus,
            CycleStatusPlc = (short)this.CycleStatus,
        };

        var result = await this.SimulateCommandAsync(command, cancellationToken);

        return command;
    }
}
