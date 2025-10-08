// <copyright file="CreatePerformanceDataCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Performance.Request.Command.Create;

/// <summary>
/// Handles the creation of performance data commands and manages OEE and KPI OEE registration.
/// </summary>
public class CreatePerformanceDataCommandHandler(
    ILogger<CreatePerformanceDataCommandHandler> logger,
    IRepository<OeeRegister> oeeRegisterRepo,
    IRepository<KpiOee> kpiOeeRepo) : IGatewayRequestHandler<PerformanceDataCommand, TaskGatewayResponse>, IResettable // Fixed the incorrect generic usage
{
    /// <summary>
    /// Processes the performance data command and registers OEE and KPI OEE data.
    /// </summary>
    /// <param name="request">The performance data command to process.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the task gateway response.</returns>
    public async Task<Result<TaskGatewayResponse>> ProcessAsync(PerformanceDataCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<TaskGatewayResponse>.WithFailure("Request cannot be null");
        }

        // Respect cooperative cancellation without throwing exceptions
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TaskGatewayResponse>.WithFailure("Operation was canceled.");
        }

        try
        {
            var response = TaskGatewayResponse.ToDto(request.Command);

            var performanceData = PerformanceDataCommand.ToEntity(request);

            var register = new OeeRegister();

            var resultCalculateOee = OeeRegister.CalculateOee(register, performanceData);
            if (resultCalculateOee.IsFailure && resultCalculateOee.Errors is not null)
            {
                foreach (var error in resultCalculateOee.Errors)
                {
                    logger.LogError("Erorr calculation OEE on Machine UserId {MachineId} error {error}", request.MachineId, error);
                }
            }
            else if (resultCalculateOee.HasWarnings)
            {
                // Log warnings with confidence metadata for observability; do not fail the operation
                logger.LogWarning(
                    "OEE calculation for Machine {MachineId} completed with warnings. Confidence={Confidence:0.00}, MissingDataRatio={Missing:0.00}. Warnings: {Warnings}",
                    request.MachineId,
                    resultCalculateOee.Confidence,
                    resultCalculateOee.MissingDataRatio,
                    string.Join(", ", resultCalculateOee.Warnings));
            }

            var kpiOee = OeeRegister.ToKpiOee(register);

            var result2 = await oeeRegisterRepo.AddAsync(register, cancellationToken).ConfigureAwait(false);
            var result3 = await kpiOeeRepo.AddAsync(kpiOee, cancellationToken).ConfigureAwait(false);

            // Failure if any repo add failed or invalid IDs were returned
            var id1 = result2.IsSuccess ? (result2.Value is int v1 ? v1 : 0) : 0;
            var id2 = result3.IsSuccess ? (result3.Value is int v2 ? v2 : 0) : 0;
            var isFailure = result2.IsFailure || result3.IsFailure || id1 <= 0 || id2 <= 0;

            return isFailure
                ? Result<TaskGatewayResponse>.WithFailure("Error Adding Performance Data", response)
                : Result<TaskGatewayResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in CreatePerformanceDataCommandHandler");
            return Result<TaskGatewayResponse>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }

    /// <summary>
    /// Attempts to reset the state of the command handler.
    /// </summary>
    /// <returns>True if the reset was successful; otherwise, false.</returns>
    public bool TryReset()
    {
        // Reset state for the command handler
        // This is a no-op in this case, as we don't maintain any state in this handler.
        return true;
    }
}
