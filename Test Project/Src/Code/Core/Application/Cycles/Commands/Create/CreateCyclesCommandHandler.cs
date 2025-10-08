// <copyright file="CreateCyclesCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.BarCodes.Services;
using IndTrace.Application.Cycles.Policies;
using IndTrace.Application.Cycles.Services;
using IndTrace.Application.Cycles.Validation;
using IndTrace.Application.Gateway.Auditing;

namespace IndTrace.Application.Cycles.Commands.Create;

/// <summary>
/// Handles the creation of production cycles with comprehensive validation and workflow management.
/// </summary>
/// <remarks>
/// This handler is a critical component of the manufacturing execution system that manages the lifecycle
/// of production cycles, including validation of barcode status, workflow compliance, and cycle limits.
/// It implements the gateway pattern for communication with PLC systems and ensures data integrity
/// across the production tracking system.
/// </remarks>
public class CreateCyclesCommandHandler(
    ILogger<CreateCyclesCommandHandler> logger,
    IDateTimeMachine dateTimeMachine,
    IBarCodeResult barCodeResult,
    Validation.IStationValidator stationValidator,
    ICycleLimitPolicy cycleLimitPolicy,
    ICycleCreator cycleCreator,
    IBarCodeUpdater barCodeUpdater,
    IGatewayAuditFactory gatewayAuditFactory) : IGatewayRequestHandler<CreateCyclesCommand, TaskGatewayResponse>, IResettable
{
    /// <summary>
    /// Processes the creation of a production cycle with comprehensive validation and workflow management.
    /// </summary>
    /// <param name="cmd">The command containing cycle creation details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result containing the gateway response with cycle information.</returns>
    /// <remarks>
    /// This method performs critical manufacturing validations including:
    /// - Station type verification (only process stations can start cycles)
    /// - Maximum cycle count enforcement per recipe configuration
    /// - Barcode flow status validation
    /// - Part status tracking and workflow compliance.
    /// </remarks>
    public async Task<Result<TaskGatewayResponse>> ProcessAsync(CreateCyclesCommand cmd, CancellationToken cancellationToken)
    {
        if (cmd is null)
        {
            return Result<TaskGatewayResponse>.WithFailure("cmd cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TaskGatewayResponse>.WithFailure("Operation was canceled.");
        }

        try
        {
            logger.LogInformation(
                "Starting cycle creation process for MachineId: {MachineId}, BarCode: {BarCode}",
                cmd.Command.MachineId, cmd.Command.BarCode);

            var barCodeInfo = await this.GetBarCodeInformation(cmd, cancellationToken).ConfigureAwait(false);
            var request = cmd.Command;

            // Null check for barCodeInfo
            if (barCodeInfo is null)
            {
                logger.LogError("CreateCyclesCommandHandler:: Failed to retrieve barcode information. MachineId={MachineId}, BarCode={BarCode}", request.MachineId, request.BarCode);
                return Result<TaskGatewayResponse>.WithFailure("Failed to retrieve barcode information");
            }

            logger.LogInformation(
                "Retrieved barcode information: BarCodeId={BarCodeId}, PartNumber={PartNumber}, FlowStatus={FlowStatus}, MachineType={MachineType}",
                barCodeInfo.BarCodeId, barCodeInfo.PartNumber, barCodeInfo.FlowStatus, barCodeInfo.MachineType);

            // Just the process station can start cycles,  return in this point
            var stationValidationResult = stationValidator.ValidateCanStartCycles(barCodeInfo);
            if (stationValidationResult.IsFailure)
            {
                logger.LogWarning("CreateCyclesCommandHandler:: Only process stations can start cycles. MachineId={MachineId}, MachineType={MachineType}", request.MachineId, barCodeInfo.MachineType);
                return Result<TaskGatewayResponse>.WithFailure(
                    stationValidationResult.Error ?? "Station validation failed",
                    TaskGatewayResponse.ToDto(barCodeInfo));
            }

            // Check if the barcode does not have FlowStatus == FlowStatus.Restored
            if (!Equals(barCodeInfo.FlowStatus, FlowStatus.Restored))
            {
                // Evaluate cycle limits using the policy service
                var cycleLimitResult = cycleLimitPolicy.EvaluateCycleLimits(barCodeInfo, cmd);
                if (cycleLimitResult.IsFailure)
                {
                    logger.LogError("CreateCyclesCommandHandler:: Cycle limit policy evaluation failed. Error={Error}", cycleLimitResult.Error);
                    return Result<TaskGatewayResponse>.WithFailure(
                        cycleLimitResult.Error,
                        TaskGatewayResponse.ToDto(barCodeInfo));
                }

                var cycleLimitDecision = cycleLimitResult.Value;
                if (cycleLimitDecision is not null && !cycleLimitDecision.IsAllowed)
                {
                    logger.LogWarning("CreateCyclesCommandHandler:: Cycle limit policy rejected creation. MachineId={MachineId}, BarCodeId={BarCodeId}, Reason={Reason}", request.MachineId, barCodeInfo.BarCodeId, cycleLimitDecision.Reason);
                    barCodeInfo.ResultValidation = cycleLimitDecision.ValidationResult;
                    return Result<TaskGatewayResponse>.WithFailure(
                        cycleLimitDecision.Reason,
                        TaskGatewayResponse.ToDto(barCodeInfo));
                }
            }

            var cycleStatus = request.CycleStatus;
            var flowStatus = FlowStatus.InProcess;
            var partStatus = request.PartStatus;

            // Create cycle using the cycle creator service
            var cycleCreateRequest = new CycleCreateRequest(
                request.MachineId,
                barCodeInfo.BarCodeId,
                cycleStatus.Value,
                partStatus.Value,
                dateTimeMachine.Now.ToLocalTime(),
                dateTimeMachine.Now.ToLocalTime());

            var cycleCreationResult = await cycleCreator.CreateAsync(cycleCreateRequest, cancellationToken);
            if (cycleCreationResult.IsFailure)
            {
                logger.LogError("CreateCyclesCommandHandler:: Failed to create cycle. Error={Error}", cycleCreationResult.Error);
                return Result<TaskGatewayResponse>.WithFailure(
                    cycleCreationResult.Error,
                    TaskGatewayResponse.ToDto(barCodeInfo));
            }

            var cycle = cycleCreationResult.Value;
            if (cycle is null)
            {
                logger.LogError("CreateCyclesCommandHandler:: Created cycle is null");
                return Result<TaskGatewayResponse>.WithFailure(
                    "Created cycle is null",
                    TaskGatewayResponse.ToDto(barCodeInfo));
            }

            // Update barcode using the barcode updater service
            var barCodeUpdateRequest = new BarCodeUpdateRequest(
                barCodeInfo.BarCodeId,
                partStatus.Value,
                flowStatus.Value,
                request.MachineId,
                dateTimeMachine.Now.ToLocalTime());

            var barCodeUpdateResult = await barCodeUpdater.UpdateAsync(barCodeUpdateRequest, cancellationToken);
            if (barCodeUpdateResult.IsFailure)
            {
                logger.LogError("CreateCyclesCommandHandler:: Failed to update barcode. BarCodeId={BarCodeId}, Error={Error}", barCodeInfo.BarCodeId, barCodeUpdateResult.Error);
                return Result<TaskGatewayResponse>.WithFailure(
                    barCodeUpdateResult.Error ?? "Failed to update barcode",
                    TaskGatewayResponse.ToDto(barCodeInfo));
            }

            barCodeInfo.UpdateBarCodeInformationOnCycle(flowStatus, partStatus, cycleStatus);
            barCodeInfo.SetCycle(cycle);

            // Create audit entry using the gateway audit factory service
            var gatewayAuditRequest = new GatewayAuditRequest(
                barCodeInfo.MachineId,
                barCodeInfo.BarCodeId,
                cycle.CycleId,
                barCodeInfo.CycleStatus,
                barCodeInfo.PartStatus,
                barCodeInfo.FlowStatus,
                barCodeInfo.ResultValidation,
                GatewayTask.CreateCycleAsync,
                dateTimeMachine.Now.ToLocalTime());

            var auditResult = await gatewayAuditFactory.CreateAuditEntryAsync(gatewayAuditRequest, cancellationToken);
            if (auditResult.IsFailure)
            {
                logger.LogError("CreateCyclesCommandHandler:: Failed to create gateway audit entry. Error={Error}", auditResult.Error);
                return Result<TaskGatewayResponse>.WithFailure(
                    auditResult.Error,
                    TaskGatewayResponse.ToDto(barCodeInfo));
            }

            var result = TaskGatewayResponse.ToDto(barCodeInfo);
            _ = result.ApplyReferencesValuesResult();

            if (barCodeInfo.Error is not null && barCodeInfo.Error.Length > 0)
            {
                logger.LogError("CreateCyclesCommandHandler:: Barcode info contains error. MachineId={MachineId}, BarCodeId={BarCodeId}, Error={Error}", barCodeInfo.MachineId, barCodeInfo.BarCodeId, barCodeInfo.Error);
                return Result<TaskGatewayResponse>.WithFailure(barCodeInfo.Error, result);
            }

            return Result<TaskGatewayResponse>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in CreateCyclesCommandHandler");
            return Result<TaskGatewayResponse>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }


    /// <summary>
    /// Retrieves comprehensive barcode information including workflow, recipe, and cycle history.
    /// </summary>
    /// <param name="cmd">The command containing barcode and machine details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>Complete barcode information for processing decisions.</returns>
    private async Task<IBarCodeResult?> GetBarCodeInformation(CreateCyclesCommand cmd, CancellationToken cancellationToken)
    {
        var request = cmd.Command;
        var barCodeDetailsRequest = new BarCodeDetailsRequest(request.MachineId, request.BarCode, request.PartNumber);

        var result = await barCodeResult.GetBarCodeDetails(barCodeDetailsRequest, cancellationToken);

        logger.LogInformation(
            "GetBarCodeInformation:: Fetching barcode details for MachineId={MachineId}, BarCode={BarCode}, PartNumber={PartNumber}",
            request.MachineId, request.BarCode, request.PartNumber);

        logger.LogInformation("GetBarCodeInformation:: Barcode details are {BarCodeDetails}", result);

        return result;
    }


    /// <summary>
    /// Resets the handler state for reuse in pooled scenarios.
    /// </summary>
    /// <returns>True indicating successful reset.</returns>
    public bool TryReset()
    {
        return true;
    }
}
