namespace IndTrace.Application.BarCodes.Services.Interfaces;

/// <summary>
/// Validates machine capabilities for barcode creation.
/// Ensures only printer-type machines can create barcodes.
/// </summary>
public interface IMachineValidator
{
    /// <summary>
    /// Validates that the machine exists and is of printer type (Printer or InitialPrinter).
    /// </summary>
    /// <param name="machineId">The machine identifier to validate</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Success with Machine entity if valid, Failure with error message if invalid</returns>
    Task<Result<Machine>> ValidatePrinterMachineAsync(int machineId, CancellationToken cancellationToken);
}
