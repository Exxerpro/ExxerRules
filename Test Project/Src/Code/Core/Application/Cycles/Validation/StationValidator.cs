// <copyright file="StationValidator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Validation;

/// <summary>
/// Validates station rules for cycle creation.
/// Based on CreateCyclesCommandHandler.StationCanNotStartCycles logic.
/// Implements CLAUDE.md compliance: Result pattern, defensive validation, structured logging.
/// </summary>
public class StationValidator : IStationValidator
{
    private readonly ILogger<StationValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StationValidator"/> class.
    /// </summary>
    /// <param name="logger">Logger for recording validation operations.</param>
    public StationValidator(ILogger<StationValidator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates if station can start cycles based on machine type.
    /// Only process stations can start cycles (not Dashboard or None types).
    /// </summary>
    /// <param name="barCodeInfo">The barcode information containing machine type.</param>
    /// <returns>Result indicating if validation passed or failed with reasons.</returns>
    public Result ValidateCanStartCycles(IBarCodeResult barCodeInfo)
    {
        // CLAUDE.md compliance: defensive validation
        if (barCodeInfo is null)
        {
            _logger.LogError("BarCodeInfo cannot be null for station validation");
            return Result.WithFailure(["BarCodeInfo cannot be null."]);
        }

        // Business rule: Only process stations can start cycles
        if (Equals(barCodeInfo.MachineType, MachineType.None) ||
            Equals(barCodeInfo.MachineType, MachineType.DashBoard))
        {
            _logger.LogWarning(
                "Station validation failed: Only process stations can start cycles. MachineType: {MachineType}, MachineId: {MachineId}",
                barCodeInfo.MachineType, barCodeInfo.MachineId);

            return Result.WithFailure(["Just process station can invoke Update cycles."]);
        }

        _logger.LogInformation(
            "Station validation passed for MachineType: {MachineType}, MachineId: {MachineId}",
            barCodeInfo.MachineType, barCodeInfo.MachineId);

        return Result.Success();
    }
}
