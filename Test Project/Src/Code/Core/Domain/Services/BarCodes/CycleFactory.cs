using IndTrace.Domain.Entities;
using IndTrace.Domain.Enum;
using IndTrace.Domain.Interfaces;

namespace IndTrace.Domain.Services.BarCodes;

/// <summary>
/// Pure cycle initialization logic without external dependencies.
/// Implements business rules for initial Cycle entity creation.
/// </summary>
public class CycleFactory : ICycleFactory
{
    /// <summary>
    /// Creates a new Cycle entity with proper business rule initialization for barcode creation.
    /// Sets initial cycle status to Started as per manufacturing workflow requirements.
    /// </summary>
    /// <param name="machineId">The machine identifier</param>
    /// <param name="barCodeId">The associated barcode identifier</param>
    /// <param name="cyclesOkCount">The current cycles OK count from shift</param>
    /// <param name="timestamp">The creation timestamp</param>
    /// <returns>Initialized Cycle entity ready for persistence</returns>
    public Cycle CreateInitialCycle(int machineId, int barCodeId, int cyclesOkCount, IDateTimeMachine dateTimeMachine)
    {
        // Business Rule: All new cycles start with Started status
        return new Cycle
        {
            MachineId = machineId,
            BarCodeId = barCodeId,
            CyclesOk = cyclesOkCount,
            StartedOn = dateTimeMachine.UtcNow,
            FinishedOn = dateTimeMachine.UtcNow,
            CycleStatus = CycleStatus.Started,  // Manufacturing rule: Always start Started
            // CycleId will be set by repository during persistence
        };
    }

    public Cycle CreateInitialCycle(int machineId, int barCodeId, int cyclesOkCount, DateTimeOffset timestamp)
    {
        throw new NotImplementedException();
    }
}
