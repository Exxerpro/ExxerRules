// <copyright file="MachineConfigAssembler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.Machines.Queries.GetMachinesConfig.DataLoaders;

namespace IndTrace.Application.Machines.Queries.GetMachinesConfig.Assemblers;

/// <summary>
/// Assembles MachineConfigVm from loaded context with comprehensive business rules.
/// Implements CLAUDE.md patterns: Result-T, null safety, industrial logging.
/// </summary>
public class MachineConfigAssembler : IMachineConfigAssembler
{
    private readonly ILogger<MachineConfigAssembler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MachineConfigAssembler"/> class.
    /// </summary>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public MachineConfigAssembler(ILogger<MachineConfigAssembler> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Assembles machine configuration view model from provided context.
    /// Preserves original complex PLC join logic from GetMachineConfigQueryHandler.
    /// </summary>
    /// <param name="context">Loaded machine configuration context.</param>
    /// <returns>Result containing assembled view model or failure reasons.</returns>
    public Result<MachineConfigVm> AssembleConfiguration(MachineConfigContext context)
    {
        // 1. Parameter validation
        if (context is null)
        {
            this.logger.LogError("MachineConfigContext cannot be null");
            return Result<MachineConfigVm>.WithFailure(["Context cannot be null."]);
        }

        if (context.Product is null)
        {
            this.logger.LogError("Product in context cannot be null");
            return Result<MachineConfigVm>.WithFailure(["Product cannot be null."]);
        }

        try
        {
            using var activity = this.logger.BeginScope("AssembleMachineConfig {ProductId}", context.Product.ProductId);
            var stopwatch = Stopwatch.StartNew();

            this.logger.LogInformation("Starting assembly for Product: {ProductId}, {WorkFlowCount} workflows, {MachineCount} machines",
                context.Product.ProductId, context.WorkFlows.Count, context.Machines.Count);

            var vm = new MachineConfigVm();

            // 2. Assemble machine configurations following original logic
            // Preserving the exact logic from the original handler for each machine ID
            foreach (var machineId in context.MachineIds)
            {
                // Get PLCs for this machine using the same join logic as original
                // Original: from p in plcsResult.Value join mp in machinePlcsResult.Value on p.PlcId equals mp.PlcId
                //          join m in machinesResult.Value on mp.MachineId equals m.MachineId where lm.Contains(m.MachineId)
                var machinePlcs = (from p in context.Plcs
                                   join mp in context.MachinePlcs on p.PlcId equals mp.PlcId
                                   join m in context.Machines on mp.MachineId equals m.MachineId
                                   where context.MachineIds.Contains(m.MachineId) && m.MachineId == machineId
                                   select p).ToList();

                // Get variables for this machine
                var machineVariables = context.Variables
                    .Where(v => v.MachineId == machineId)
                    .ToList();

                // Create machine configuration DTO following original pattern
                var machineConfig = new MachineConfigDto
                {
                    MachineId = machineId,
                    WorkFlows = context.WorkFlows.ToList(),
                    Plcs = machinePlcs,
                    Variables = machineVariables
                };

                vm.Machines.Add(machineConfig);

                this.logger.LogInformation("Assembled machine {MachineId}: {PlcCount} PLCs, {VariableCount} variables",
                    machineId, machinePlcs.Count, machineVariables.Count);
            }

            vm.Count = vm.Machines.Count;

            stopwatch.Stop();
            this.logger.LogInformation("Machine configuration assembly completed in {ElapsedMs}ms: {MachineCount} machines",
                stopwatch.ElapsedMilliseconds, vm.Count);

            return Result<MachineConfigVm>.Success(vm);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error assembling machine configuration");
            return Result<MachineConfigVm>.WithFailure([$"Assembly failed: {ex.Message}"]);
        }
    }
}
