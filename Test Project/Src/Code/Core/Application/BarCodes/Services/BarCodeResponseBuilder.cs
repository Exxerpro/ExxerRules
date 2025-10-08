namespace IndTrace.Application.BarCodes.Services;

/// <summary>
/// Builds final TaskGatewayResponse with all related data for barcode creation.
/// Handles the construction of the complete response object with proper data mapping.
/// </summary>
public class BarCodeResponseBuilder : IBarCodeResponseBuilder
{
    private readonly ILogger<BarCodeResponseBuilder> _logger;

    public BarCodeResponseBuilder(ILogger<BarCodeResponseBuilder> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Builds a complete TaskGatewayResponse using all the created entities and data.
    /// Maps all properties according to the established response structure using fluent builder pattern.
    /// </summary>
    /// <param name="barCode">The created BarCode entity</param>
    /// <param name="cycle">The created Cycle entity</param>
    /// <param name="machine">The validated Machine entity</param>
    /// <param name="references">The dictionary of reference variable registers</param>
    /// <param name="partNumber">The original part number from the request</param>
    /// <returns>Complete TaskGatewayResponse ready for client consumption</returns>
    public TaskGatewayResponse BuildResponse(BarCode barCode, Cycle cycle, Machine machine,
        Dictionary<string, Register> references, string partNumber)
    {
        // Input validation
        if (barCode is null) throw new ArgumentNullException(nameof(barCode));
        if (cycle is null) throw new ArgumentNullException(nameof(cycle));
        if (machine is null) throw new ArgumentNullException(nameof(machine));
        if (references is null) throw new ArgumentNullException(nameof(references));
        if (string.IsNullOrWhiteSpace(partNumber)) throw new ArgumentException("Part number cannot be null or empty", nameof(partNumber));

        try
        {
            _logger.LogDebug("Building response for BarCodeId={BarCodeId}, CycleId={CycleId}, MachineId={MachineId}",
                barCode.BarCodeId, cycle.CycleId, machine.MachineId);

            // Build response using fluent pattern (matching existing implementation)
            var response = new TaskGatewayResponse()
                .WithMachineId(machine.MachineId)
                .WithDescription(machine.Name ?? "Unknown Machine")
                .WithName(machine.Name ?? "Unknown Machine")
                .WithBarCodeId(barCode.BarCodeId)
                .WithCycleId(cycle.CycleId)
                .WithCyclesOk(cycle.CyclesOk)
                .WithResultValidation(ResultValidation.Valid)
                .WithPartNumber(partNumber)
                .WithLastMachineId(machine.MachineId)
                .WithNextMachineId(machine.MachineId)
                .WithCycleStatus(cycle.CycleStatus)
                .WithFlowStatus(barCode.FlowStatus)
                .WithPartStatus(barCode.PartStatus)
                .WithMachineType(machine.MachineType)
                .WithWorkFlowType(machine.WorkFlowType)
                .WithCycle(cycle)
                .WithBarCode(barCode)
                .WithReferences(references);

            _logger.LogDebug("Response built successfully with {ReferenceCount} references", references.Count);

            // Apply reference values to complete the response (existing business logic)
            var applyResult = response.ApplyReferencesValuesResult();
            if (applyResult.IsFailure)
            {
                _logger.LogError("Failed to apply reference values to response: {Errors}",
                    string.Join(", ", applyResult.Errors ?? []));

                // Return response even if reference application fails (graceful degradation)
                // This matches the existing error handling pattern
            }
            else
            {
                _logger.LogDebug("Reference values applied successfully to response");
            }

            _logger.LogInformation("TaskGatewayResponse created successfully for BarCode={BarCodeLabel}", barCode.Label);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building response for BarCodeId={BarCodeId}", barCode.BarCodeId);

            // Return minimal response to prevent complete failure
            return CreateMinimalResponse(barCode, cycle, machine, partNumber);
        }
    }

    /// <summary>
    /// Creates a minimal response in case of errors during response building.
    /// Ensures graceful degradation when response building fails.
    /// </summary>
    private TaskGatewayResponse CreateMinimalResponse(BarCode barCode, Cycle cycle, Machine machine, string partNumber)
    {
        try
        {
            _logger.LogWarning("Creating minimal response due to error in full response building");

            return new TaskGatewayResponse()
                .WithMachineId(machine.MachineId)
                .WithBarCodeId(barCode.BarCodeId)
                .WithCycleId(cycle.CycleId)
                .WithResultValidation(ResultValidation.Valid)
                .WithPartNumber(partNumber)
                .WithCycleStatus(cycle.CycleStatus)
                .WithFlowStatus(barCode.FlowStatus)
                .WithPartStatus(barCode.PartStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create even minimal response");

            // Absolute fallback - return empty response
            return new TaskGatewayResponse();
        }
    }
}
