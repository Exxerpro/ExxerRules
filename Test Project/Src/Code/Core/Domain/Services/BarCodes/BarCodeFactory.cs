using IndTrace.Domain.Entities;
using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Domain.Enum;
using IndTrace.Domain.Interfaces;

namespace IndTrace.Domain.Services.BarCodes;

/// <summary>
/// Pure barcode creation logic without external dependencies.
/// Implements business rules for BarCode entity initialization.
/// </summary>
public class BarCodeFactory : IBarCodeFactory
{
    /// <summary>
    /// Creates a new BarCode entity with proper business rule initialization.
    /// Sets initial flow status to Created and part status to Ok as per manufacturing requirements.
    /// </summary>
    /// <param name="label">The generated barcode label</param>
    /// <param name="productId">The product identifier</param>
    /// <param name="machineId">The machine identifier</param>
    /// <param name="timestamp">The creation timestamp</param>
    /// <returns>Initialized BarCode entity ready for persistence</returns>
    public BarCode CreateBarCode(string label, int productId, int machineId, IDateTimeMachine dateTimeMachine)
    {
        // Business Rule: All new barcodes start with these statuses
        return new BarCode
        {
            Label = label ?? throw new ArgumentNullException(nameof(label)),
            ProductId = productId,
            MachineId = machineId,
            CreatedOn = dateTimeMachine.UtcNow,
            ModifiedOn = dateTimeMachine.UtcNow,
            FlowStatus = FlowStatus.Created,    // Manufacturing rule: Always start Created
            PartStatus = PartStatus.Ok,         // Manufacturing rule: Always start Ok
            // BarCodeId will be set by repository during persistence
        };
    }
}
