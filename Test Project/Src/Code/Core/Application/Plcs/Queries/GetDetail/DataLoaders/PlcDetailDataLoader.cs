// <copyright file="PlcDetailDataLoader.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Plcs.Queries.GetDetail.DataLoaders;

/// <summary>
/// Loads PLC detail data from multiple repositories with proper filtering and validation.
/// Implements CLAUDE.md patterns: Result-T, cancellation tokens, null safety, industrial logging.
/// </summary>
public class PlcDetailDataLoader : IPlcDetailDataLoader
{
    private readonly IRepository<Plc> plcRepository;
    private readonly IRepository<MachinePlc> machinePlcRepository;
    private readonly IRepository<Machine> machineRepository;
    private readonly IRepository<Variable> variableRepository;
    private readonly IRepository<VariablesGroup> variableGroupRepository;
    private readonly ILogger<PlcDetailDataLoader> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcDetailDataLoader"/> class.
    /// </summary>
    /// <param name="plcRepository">Repository for accessing PLC data.</param>
    /// <param name="machinePlcRepository">Repository for accessing machine-PLC relationship data.</param>
    /// <param name="machineRepository">Repository for accessing machine data.</param>
    /// <param name="variableRepository">Repository for accessing variable data.</param>
    /// <param name="variableGroupRepository">Repository for accessing variable group data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public PlcDetailDataLoader(
        IRepository<Plc> plcRepository,
        IRepository<MachinePlc> machinePlcRepository,
        IRepository<Machine> machineRepository,
        IRepository<Variable> variableRepository,
        IRepository<VariablesGroup> variableGroupRepository,
        ILogger<PlcDetailDataLoader> logger)
    {
        this.plcRepository = plcRepository ?? throw new ArgumentNullException(nameof(plcRepository));
        this.machinePlcRepository = machinePlcRepository ?? throw new ArgumentNullException(nameof(machinePlcRepository));
        this.machineRepository = machineRepository ?? throw new ArgumentNullException(nameof(machineRepository));
        this.variableRepository = variableRepository ?? throw new ArgumentNullException(nameof(variableRepository));
        this.variableGroupRepository = variableGroupRepository ?? throw new ArgumentNullException(nameof(variableGroupRepository));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Loads all related data for a PLC detail view.
    /// Follows CLAUDE.md patterns for industrial safety and error handling.
    /// </summary>
    /// <param name="plcId">The PLC ID to load data for.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing loaded PLC detail context.</returns>
    public async Task<Result<PlcDetailContext>> LoadByPlcIdAsync(
        int plcId,
        CancellationToken cancellationToken)
    {
        // 1. Early cancellation check (CLAUDE.md pattern)
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<PlcDetailContext>.WithFailure(["Operation was canceled."]);
        }

        // 2. Parameter validation (industrial safety)
        if (plcId <= 0)
        {
            this.logger.LogWarning("Invalid PlcId: {PlcId}", plcId);
            return Result<PlcDetailContext>.WithFailure(["PlcId must be positive."]);
        }

        try
        {
            using var activity = this.logger.BeginScope("LoadPlcDetail {PlcId}", plcId);
            var stopwatch = Stopwatch.StartNew();

            // 3. Load PLC entity
            this.logger.LogInformation("Loading PLC for PlcId: {PlcId}", plcId);
            var plcResult = await this.plcRepository
                .GetByIdAsync(plcId, cancellationToken)
                .ConfigureAwait(false);

            if (!plcResult.IsSuccess || plcResult.Value == null)
            {
                this.logger.LogError("Failed to get PLC details for PlcId {PlcId}: {Errors}",
                    plcId, string.Join(", ", plcResult.Errors ?? []));
                return Result<PlcDetailContext>.WithFailure(
                    plcResult.Errors ?? ["PLC not found."]);
            }

            var plc = plcResult.Value;
            this.logger.LogInformation("Loaded PLC {PlcId} in {ElapsedMs}ms",
                plcId, stopwatch.ElapsedMilliseconds);

            // 4. Load MachinePlcs with filtering
            stopwatch.Restart();
            var machinePlcsResult = await this.machinePlcRepository.ListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!machinePlcsResult.IsSuccess)
            {
                this.logger.LogError("Failed to load MachinePlcs: {Errors}",
                    string.Join(", ", machinePlcsResult.Errors ?? []));
                return Result<PlcDetailContext>.WithFailure(
                    machinePlcsResult.Errors ?? ["Failed to load MachinePlcs"]);
            }

            var machinePlcs = machinePlcsResult.Value?
                .Where(mp => mp.PlcId == plcId)
                .ToList() ?? [];

            this.logger.LogInformation("Loaded {Count} MachinePlcs for PlcId {PlcId} in {ElapsedMs}ms",
                machinePlcs.Count, plcId, stopwatch.ElapsedMilliseconds);

            // 5. Get machine IDs and load machines
            var machineIds = machinePlcs
                .Select(mp => mp.MachineId)
                .Distinct()
                .ToList();

            stopwatch.Restart();
            var machinesResult = await this.machineRepository.ListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!machinesResult.IsSuccess)
            {
                this.logger.LogError("Failed to load Machines: {Errors}",
                    string.Join(", ", machinesResult.Errors ?? []));
                return Result<PlcDetailContext>.WithFailure(
                    machinesResult.Errors ?? ["Failed to load Machines"]);
            }

            var machines = machineIds.Any()
                ? machinesResult.Value?
                    .Where(m => machineIds.Contains(m.MachineId))
                    .ToList() ?? []
                : [];

            this.logger.LogInformation("Loaded {Count} Machines in {ElapsedMs}ms",
                machines.Count, stopwatch.ElapsedMilliseconds);

            // 6. Load Variables (active only)
            stopwatch.Restart();
            var variablesResult = await this.variableRepository.ListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!variablesResult.IsSuccess)
            {
                this.logger.LogError("Failed to load Variables: {Errors}",
                    string.Join(", ", variablesResult.Errors ?? []));
                return Result<PlcDetailContext>.WithFailure(
                    variablesResult.Errors ?? ["Failed to load Variables"]);
            }

            var variables = variablesResult.Value?
                .Where(v => v.PlcId == plcId && v.IsActive == 1)
                .ToList() ?? [];

            this.logger.LogInformation("Loaded {Count} active Variables for PlcId {PlcId} in {ElapsedMs}ms",
                variables.Count, plcId, stopwatch.ElapsedMilliseconds);

            // 7. Load Variable Groups
            stopwatch.Restart();
            var groupsResult = await this.variableGroupRepository.ListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!groupsResult.IsSuccess)
            {
                this.logger.LogError("Failed to load VariableGroups: {Errors}",
                    string.Join(", ", groupsResult.Errors ?? []));
                return Result<PlcDetailContext>.WithFailure(
                    groupsResult.Errors ?? ["Failed to load VariableGroups"]);
            }

            var groups = groupsResult.Value?.ToList() ?? [];

            this.logger.LogInformation("Loaded {Count} VariableGroups in {ElapsedMs}ms",
                groups.Count, stopwatch.ElapsedMilliseconds);

            // 8. Build and return context
            var context = new PlcDetailContext(plc, machinePlcs, machines, variables, groups);

            return Result<PlcDetailContext>.Success(context);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to load PLC detail data for PlcId {PlcId}", plcId);
            return Result<PlcDetailContext>.WithFailure([$"Data loading failed: {ex.Message}"]);
        }
    }
}
