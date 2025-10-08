namespace IndTrace.Application.BarCodes.Services.Interfaces;

/// <summary>
/// Coordinates barcode and cycle persistence operations.
/// Manages the creation of both BarCode and Cycle entities as a coordinated operation.
/// </summary>
public interface IBarCodePersistenceOrchestrator
{
    /// <summary>
    /// Creates both BarCode and Cycle entities in a coordinated manner.
    /// The Cycle entity depends on the BarCode being created first for the relationship.
    /// </summary>
    /// <param name="label">The generated barcode label</param>
    /// <param name="productId">The product identifier</param>
    /// <param name="machineId">The machine identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Success with both BarCode and Cycle entities, Failure if persistence fails</returns>
    Task<Result<(BarCode barCode, Cycle cycle)>> CreateBarCodeAndCycleAsync(
        string label, int productId, int machineId, CancellationToken cancellationToken);
}
