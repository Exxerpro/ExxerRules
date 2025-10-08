// <copyright file="ICycleUpdateInterfaces.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Services.Interfaces;

/// <summary>
/// Provides bar code information retrieval functionality.
/// </summary>
public interface IBarCodeInfoProvider
{
    /// <summary>
    /// Retrieves bar code information for the specified parameters.
    /// </summary>
    /// <param name="machineId">The machine identifier.</param>
    /// <param name="barCode">The bar code value.</param>
    /// <param name="partNumber">The part number.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing the bar code information if successful.</returns>
    Task<Result<IBarCodeResult>> GetBarCodeInfoAsync(
        int machineId,
        string barCode,
        string partNumber,
        CancellationToken cancellationToken);
}

/// <summary>
/// Validates station capabilities for cycle updates.
/// </summary>
public interface IStationValidator
{
    /// <summary>
    /// Validates if a station can update cycles based on business rules.
    /// </summary>
    /// <param name="machineId">The current machine identifier.</param>
    /// <param name="cycleStatus">The target cycle status.</param>
    /// <param name="barCodeInfo">The bar code information.</param>
    /// <returns>A validation result indicating if the update is allowed.</returns>
    Result<StationValidationResult> ValidateStation(
        int machineId,
        CycleStatus cycleStatus,
        IBarCodeResult barCodeInfo);
}

/// <summary>
/// Represents the result of station validation.
/// </summary>
/// <param name="CanUpdate">Indicates if the station can perform the update.</param>
/// <param name="FailureReason">The reason for validation failure if applicable.</param>
/// <param name="Validation">The validation status.</param>
public record StationValidationResult(
    bool CanUpdate,
    string? FailureReason,
    ResultValidation Validation);

/// <summary>
/// Defines the strategy for updating cycles.
/// </summary>
public interface ICycleUpdateStrategy
{
    /// <summary>
    /// Executes the cycle update according to the specific strategy.
    /// </summary>
    /// <param name="command">The update command containing request data.</param>
    /// <param name="barCodeInfo">The bar code information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing the update outcome.</returns>
    Task<Result<CycleUpdateResult>> ExecuteAsync(
        IUpdateCycleCommand command,
        IBarCodeResult barCodeInfo,
        CancellationToken cancellationToken);
}

/// <summary>
/// Represents the command data for cycle updates.
/// </summary>
public interface IUpdateCycleCommand
{
    /// <summary>
    /// Gets the machine identifier.
    /// </summary>
    int MachineId { get; }

    /// <summary>
    /// Gets the bar code value.
    /// </summary>
    string BarCode { get; }

    /// <summary>
    /// Gets the part number.
    /// </summary>
    string PartNumber { get; }

    /// <summary>
    /// Gets the part status.
    /// </summary>
    PartStatus PartStatus { get; }

    /// <summary>
    /// Gets the cycle status.
    /// </summary>
    CycleStatus CycleStatus { get; }

    /// <summary>
    /// Gets the registers to save.
    /// </summary>
    IDictionary<string, Register> Registers { get; }
}

/// <summary>
/// Represents the result of a cycle update operation.
/// </summary>
/// <param name="UpdatedCycle">The updated cycle entity.</param>
/// <param name="UpdatedBarCode">The updated bar code entity.</param>
/// <param name="RegistersSaved">The number of registers saved.</param>
/// <param name="CyclesOk">The cycles OK count if applicable.</param>
/// <param name="ShiftInfo">Optional shift information.</param>
public record CycleUpdateResult(
    Cycle UpdatedCycle,
    BarCode UpdatedBarCode,
    int RegistersSaved,
    int? CyclesOk = null,
    ShiftInfo? ShiftInfo = null);

/// <summary>
/// Represents shift information.
/// </summary>
/// <param name="ShiftId">The shift identifier.</param>
/// <param name="CyclesOk">The OK cycles count.</param>
public record ShiftInfo(int ShiftId, int CyclesOk);

/// <summary>
/// Cleans and prepares registers for persistence.
/// </summary>
public interface IRegisterCleaner
{
    /// <summary>
    /// Cleans register values and sets required metadata.
    /// </summary>
    /// <param name="registers">The registers to clean.</param>
    /// <param name="cycleId">The cycle identifier.</param>
    /// <param name="machineId">The machine identifier.</param>
    /// <param name="timestamp">The timestamp to set.</param>
    /// <returns>A result containing cleaned registers.</returns>
    Result<IEnumerable<Register>> CleanRegisters(
        IDictionary<string, Register> registers,
        int cycleId,
        int machineId,
        DateTime timestamp);
}

/// <summary>
/// Orchestrates persistence operations for cycle updates.
/// </summary>
public interface IPersistenceOrchestrator
{
    /// <summary>
    /// Persists registers, cycle, and bar code in a coordinated manner.
    /// </summary>
    /// <param name="registers">The registers to persist.</param>
    /// <param name="cycle">The cycle to update.</param>
    /// <param name="barCode">The bar code to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing persistence outcome.</returns>
    Task<Result<PersistenceResult>> PersistAsync(
        IEnumerable<Register> registers,
        Cycle cycle,
        BarCode barCode,
        CancellationToken cancellationToken);
}

/// <summary>
/// Represents the result of persistence operations.
/// </summary>
/// <param name="RegistersSaved">The number of registers saved.</param>
/// <param name="CycleUpdated">Indicates if the cycle was updated.</param>
/// <param name="BarCodeUpdated">Indicates if the bar code was updated.</param>
public record PersistenceResult(
    int RegistersSaved,
    bool CycleUpdated,
    bool BarCodeUpdated);

/// <summary>
/// Logs gateway commands for audit and tracking.
/// </summary>
public interface ICommandLogger
{
    /// <summary>
    /// Logs a gateway command execution.
    /// </summary>
    /// <param name="command">The command to log.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating logging success.</returns>
    Task<Result> LogCommandAsync(
        TaskGatewayRequest command,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a command from bar code information.
    /// </summary>
    /// <param name="barCodeInfo">The bar code information.</param>
    /// <param name="gatewayTask">The gateway task type.</param>
    /// <param name="comment">Optional comment.</param>
    /// <returns>The created command.</returns>
    TaskGatewayRequest CreateCommand(
        IBarCodeResult barCodeInfo,
        GatewayTask gatewayTask,
        string? comment = null);
}

/// <summary>
/// Factory for creating cycle update strategies.
/// </summary>
public interface ICycleUpdateStrategyFactory
{
    /// <summary>
    /// Creates the appropriate strategy for the given cycle status.
    /// </summary>
    /// <param name="cycleStatus">The target cycle status.</param>
    /// <returns>The appropriate update strategy.</returns>
    ICycleUpdateStrategy CreateStrategy(CycleStatus cycleStatus);
}
