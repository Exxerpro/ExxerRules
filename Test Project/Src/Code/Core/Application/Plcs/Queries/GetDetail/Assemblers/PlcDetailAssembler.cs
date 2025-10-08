// <copyright file="PlcDetailAssembler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.Plcs.Queries.GetDetail.DataLoaders;

namespace IndTrace.Application.Plcs.Queries.GetDetail.Assemblers;

/// <summary>
/// Assembles PlcDto from loaded context with comprehensive business rules.
/// Implements CLAUDE.md patterns: Result-T, null safety, industrial logging.
/// Eliminates magic numbers by using VariableGroupIds constants.
/// </summary>
public class PlcDetailAssembler : IPlcDetailAssembler
{
    private readonly ILogger<PlcDetailAssembler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcDetailAssembler"/> class.
    /// </summary>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public PlcDetailAssembler(ILogger<PlcDetailAssembler> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Assembles complete PLC detail view model.
    /// Preserves original business logic including variable deduplication and register transformation.
    /// </summary>
    /// <param name="context">Loaded PLC detail context.</param>
    /// <returns>Result containing assembled PLC DTO or failure reasons.</returns>
    public Result<PlcDto> AssembleDetail(PlcDetailContext context)
    {
        // 1. Parameter validation
        if (context is null)
        {
            this.logger.LogError("PlcDetailContext cannot be null");
            return Result<PlcDto>.WithFailure(["Context cannot be null."]);
        }

        if (context.Plc is null)
        {
            this.logger.LogError("PLC in context cannot be null");
            return Result<PlcDto>.WithFailure(["PLC cannot be null."]);
        }

        try
        {
            using var activity = this.logger.BeginScope("AssemblePlcDetail PlcId: {PlcId}", context.Plc.PlcId);
            var stopwatch = Stopwatch.StartNew();

            this.logger.LogInformation(
                "Starting assembly for PLC: {PlcId}, {MachinePlcCount} machine-PLCs, {VariableCount} variables",
                context.Plc.PlcId, context.MachinePlcs.Count, context.Variables.Count);

            // 2. Convert PLC to DTO using existing pattern
            var vmResult = PlcDto.ToDto(context.Plc);
            if (vmResult.IsFailure || vmResult.Value is null)
            {
                this.logger.LogWarning("Failed to convert PLC to DTO: {Errors}",
                    string.Join(", ", vmResult.Errors ?? []));
                return Result<PlcDto>.WithFailure(vmResult.Errors);
            }

            var vm = vmResult.Value;

            // 3. Calculate minimum MachineId (preserving original logic)
            var machineIds = context.MachinePlcs
                .Select(mp => mp.MachineId)
                .ToList();

            vm.MachineId = machineIds.Count > 0 ? machineIds.Min() : 0;

            // 4. Assign machines
            vm.Machines = context.Machines.ToList();

            // 5. Deduplicate variables by Name (preserving original GroupBy logic)
            var deduplicatedVariables = context.Variables
                .GroupBy(v => v.Name)
                .ToDictionary(
                    group => group.Key,
                    group => group.FirstOrDefault());

            vm.Variables = deduplicatedVariables!;

            // 6. Build variable groups dictionary
            vm.VariablesGroups = context.VariableGroups
                .ToDictionary(vg => vg.VariableGroupName);

            // 7. Filter and transform variables to registers (VariableGroupId = 128)
            var registerVariables = deduplicatedVariables
                .Where(kvp => kvp.Value != null && kvp.Value.VariableGroupId == VariableGroupIds.Registers)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            vm.Registers = TransformToRegisters(registerVariables);

            // 8. Filter and transform variables to references (VariableGroupId = 256)
            var referenceVariables = deduplicatedVariables
                .Where(kvp => kvp.Value != null && kvp.Value.VariableGroupId == VariableGroupIds.References)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            vm.References = TransformToRegisters(referenceVariables);

            stopwatch.Stop();

            this.logger.LogInformation("Assembled PlcDto for PlcId {PlcId} in {ElapsedMs}ms",
                context.Plc.PlcId, stopwatch.ElapsedMilliseconds);

            return Result<PlcDto>.Success(vm);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to assemble PlcDto for PlcId {PlcId}", context.Plc.PlcId);
            return Result<PlcDto>.WithFailure([$"Assembly failed: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Transforms variables dictionary to registers dictionary.
    /// Preserves original Register entity creation logic.
    /// </summary>
    /// <param name="variables">Dictionary of variables to transform.</param>
    /// <returns>Dictionary of registers keyed by variable name.</returns>
    private static Dictionary<string, Register> TransformToRegisters(
        Dictionary<string, Variable?> variables)
    {
        return variables.ToDictionary(
            kvp => kvp.Key,
            kvp => new Register
            {
                RegisterId = kvp.Value!.VariableId,
                Name = kvp.Value.Name,
                Description = kvp.Value.Description,
                MachineId = kvp.Value.MachineId,
                VariableId = kvp.Value.VariableId,
                CycleId = 0,
                Value = kvp.Value.Value,
                DataType = kvp.Value.NetType,
                StatusValueId = 0,
            });
    }
}
