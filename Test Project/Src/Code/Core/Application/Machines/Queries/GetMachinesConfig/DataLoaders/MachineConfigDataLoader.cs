// <copyright file="MachineConfigDataLoader.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Machines.Queries.GetMachinesConfig.DataLoaders;

/// <summary>
/// Loads machine configuration data from multiple repositories with proper filtering and validation.
/// Implements CLAUDE.md patterns: Result-T, cancellation tokens, null safety, industrial logging.
/// </summary>
public class MachineConfigDataLoader : IMachineConfigDataLoader
{
    private readonly IRepository<Product> productRepository;
    private readonly IRepository<WorkFlow> workFlowRepository;
    private readonly IRepository<Machine> machineRepository;
    private readonly IRepository<Plc> plcRepository;
    private readonly IRepository<MachinePlc> machinePlcRepository;
    private readonly IRepository<Variable> variableRepository;
    private readonly ILogger<MachineConfigDataLoader> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MachineConfigDataLoader"/> class.
    /// </summary>
    /// <param name="productRepository">Repository for accessing product data.</param>
    /// <param name="workFlowRepository">Repository for accessing workflow data.</param>
    /// <param name="machineRepository">Repository for accessing machine data.</param>
    /// <param name="plcRepository">Repository for accessing PLC data.</param>
    /// <param name="machinePlcRepository">Repository for accessing machine-PLC relationships.</param>
    /// <param name="variableRepository">Repository for accessing variable data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public MachineConfigDataLoader(
        IRepository<Product> productRepository,
        IRepository<WorkFlow> workFlowRepository,
        IRepository<Machine> machineRepository,
        IRepository<Plc> plcRepository,
        IRepository<MachinePlc> machinePlcRepository,
        IRepository<Variable> variableRepository,
        ILogger<MachineConfigDataLoader> logger)
    {
        this.productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        this.workFlowRepository = workFlowRepository ?? throw new ArgumentNullException(nameof(workFlowRepository));
        this.machineRepository = machineRepository ?? throw new ArgumentNullException(nameof(machineRepository));
        this.plcRepository = plcRepository ?? throw new ArgumentNullException(nameof(plcRepository));
        this.machinePlcRepository = machinePlcRepository ?? throw new ArgumentNullException(nameof(machinePlcRepository));
        this.variableRepository = variableRepository ?? throw new ArgumentNullException(nameof(variableRepository));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Loads all related machine configuration data for a specific part number.
    /// Follows CLAUDE.md patterns for industrial safety and error handling.
    /// </summary>
    /// <param name="partNumber">Product part number for configuration lookup.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing loaded configuration context.</returns>
    public async Task<Result<MachineConfigContext>> LoadByPartNumberAsync(
        string partNumber,
        CancellationToken cancellationToken)
    {
        // 1. Early cancellation check (CLAUDE.md pattern)
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<MachineConfigContext>.WithFailure(["Operation was canceled."]);
        }

        // 2. Parameter validation (industrial safety)
        if (string.IsNullOrWhiteSpace(partNumber))
        {
            this.logger.LogWarning("PartNumber validation failed - null or empty");
            return Result<MachineConfigContext>.WithFailure(["PartNumber cannot be null or empty."]);
        }

        if (partNumber.Length < 3)
        {
            this.logger.LogWarning("PartNumber validation failed - too short: '{PartNumber}'", partNumber);
            return Result<MachineConfigContext>.WithFailure(["PartNumber must be at least 3 characters long."]);
        }

        try
        {
            using var activity = this.logger.BeginScope("LoadMachineConfig {PartNumber}", partNumber);
            var stopwatch = Stopwatch.StartNew();

            // 3. Load product by part number
            this.logger.LogInformation("Loading product for PartNumber: {PartNumber}", partNumber);
            var productsResult = await this.productRepository.ListAsync(cancellationToken).ConfigureAwait(false);

            if (productsResult.IsFailure)
            {
                this.logger.LogError("Failed to load products: {Errors}", string.Join(", ", productsResult.Errors ?? []));
                return Result<MachineConfigContext>.WithFailure(productsResult.Errors);
            }

            var product = productsResult.Value?.FirstOrDefault(p => p.PartNumber == partNumber);
            if (product is null)
            {
                this.logger.LogError("Product not found for PartNumber: {PartNumber}", partNumber);
                return Result<MachineConfigContext>.WithFailure([$"Product with PartNumber {partNumber} not found"]);
            }

            this.logger.LogInformation("Found product: ID={ProductId}, PartNumber={PartNumber}",
                product.ProductId, product.PartNumber);

            // 4. Load workflows for product
            var workFlowsResult = await this.workFlowRepository.ListAsync(cancellationToken).ConfigureAwait(false);
            if (workFlowsResult.IsFailure)
            {
                this.logger.LogError("Failed to load workflows: {Errors}", string.Join(", ", workFlowsResult.Errors ?? []));
                return Result<MachineConfigContext>.WithFailure(workFlowsResult.Errors);
            }

            var workFlows = workFlowsResult.Value?.Where(wf => wf.ProductId == product.ProductId).ToList() ?? [];
            if (!workFlows.Any())
            {
                this.logger.LogError("No workflows found for Product: {PartNumber}", partNumber);
                return Result<MachineConfigContext>.WithFailure([$"No WorkFlows found for product {partNumber}"]);
            }

            var machineIds = workFlows.Select(wf => wf.NextMachineId).Distinct().ToList();
            this.logger.LogInformation("Found {WorkFlowCount} workflows with {MachineIdCount} unique machine IDs",
                workFlows.Count, machineIds.Count);

            // 5. Load machines for workflow machine IDs
            var machinesResult = await this.machineRepository.ListAsync(cancellationToken).ConfigureAwait(false);
            if (machinesResult.IsFailure)
            {
                this.logger.LogError("Failed to load machines: {Errors}", string.Join(", ", machinesResult.Errors ?? []));
                return Result<MachineConfigContext>.WithFailure(machinesResult.Errors);
            }

            var machines = machinesResult.Value?.Where(m => machineIds.Contains(m.MachineId)).ToList() ?? [];
            this.logger.LogInformation("Loaded {MachineCount} machines", machines.Count);

            // 6. Load machine-PLC relationships
            var machinePlcsResult = await this.machinePlcRepository.ListAsync(cancellationToken).ConfigureAwait(false);
            if (machinePlcsResult.IsFailure)
            {
                this.logger.LogError("Failed to load machine-PLC relationships: {Errors}",
                    string.Join(", ", machinePlcsResult.Errors ?? []));
                return Result<MachineConfigContext>.WithFailure(machinePlcsResult.Errors);
            }

            var machinePlcs = machinePlcsResult.Value?.Where(mp => machineIds.Contains(mp.MachineId)).ToList() ?? [];
            this.logger.LogInformation("Loaded {MachinePlcCount} machine-PLC relationships", machinePlcs.Count);

            // 7. Load PLCs (need all PLCs to perform join)
            var plcsResult = await this.plcRepository.ListAsync(cancellationToken).ConfigureAwait(false);
            if (plcsResult.IsFailure)
            {
                this.logger.LogError("Failed to load PLCs: {Errors}", string.Join(", ", plcsResult.Errors ?? []));
                return Result<MachineConfigContext>.WithFailure(plcsResult.Errors);
            }

            var allPlcs = plcsResult.Value?.ToList() ?? [];
            this.logger.LogInformation("Loaded {PlcCount} total PLCs", allPlcs.Count);

            // 8. Load variables for machines
            var variablesResult = await this.variableRepository.ListAsync(cancellationToken).ConfigureAwait(false);
            if (variablesResult.IsFailure)
            {
                this.logger.LogError("Failed to load variables: {Errors}", string.Join(", ", variablesResult.Errors ?? []));
                return Result<MachineConfigContext>.WithFailure(variablesResult.Errors);
            }

            var variables = variablesResult.Value?.Where(v => machineIds.Contains(v.MachineId)).ToList() ?? [];
            this.logger.LogInformation("Loaded {VariableCount} variables", variables.Count);

            stopwatch.Stop();
            this.logger.LogInformation("Machine configuration data loading completed in {ElapsedMs}ms",
                stopwatch.ElapsedMilliseconds);

            // 9. Build and return context
            var context = new MachineConfigContext(
                Product: product,
                WorkFlows: workFlows,
                Machines: machines,
                MachinePlcs: machinePlcs,
                Plcs: allPlcs,
                Variables: variables);

            return Result<MachineConfigContext>.Success(context);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error loading machine configuration for PartNumber: {PartNumber}", partNumber);
            return Result<MachineConfigContext>.WithFailure([$"Data loading failed: {ex.Message}"]);
        }
    }
}
