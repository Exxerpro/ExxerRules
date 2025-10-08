namespace IndTrace.Application.BarCodes.Services.Interfaces;

/// <summary>
/// Manages rule lookup and execution for barcode generation.
/// Handles complex rule parsing and barcode label creation.
/// </summary>
public interface IRuleExecutionService
{
    /// <summary>
    /// Generates a barcode label using the appropriate rule for the machine and product combination.
    /// Finds the latest active rule, parses the rule JSON, and executes the barcode creation logic.
    /// </summary>
    /// <param name="machineId">The machine identifier</param>
    /// <param name="productId">The product identifier</param>
    /// <param name="partNumber">The part number for the product</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Success with generated barcode label string, Failure if rule not found or execution fails</returns>
    Task<Result<string>> GenerateBarCodeLabelAsync(int machineId, int productId, string partNumber, CancellationToken cancellationToken);
}
