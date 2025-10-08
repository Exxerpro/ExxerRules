namespace IndTrace.Application.BarCodes.Services.Interfaces;

/// <summary>
/// Builds final TaskGatewayResponse with all related data for barcode creation.
/// Handles the construction of the complete response object with proper data mapping.
/// </summary>
public interface IBarCodeResponseBuilder
{
    /// <summary>
    /// Builds a complete TaskGatewayResponse using all the created entities and data.
    /// Maps all properties according to the established response structure.
    /// </summary>
    /// <param name="barCode">The created BarCode entity</param>
    /// <param name="cycle">The created Cycle entity</param>
    /// <param name="machine">The validated Machine entity</param>
    /// <param name="references">The dictionary of reference variable registers</param>
    /// <param name="partNumber">The original part number from the request</param>
    /// <returns>Complete TaskGatewayResponse ready for client consumption</returns>
    TaskGatewayResponse BuildResponse(BarCode barCode, Cycle cycle, Machine machine,
        Dictionary<string, Register> references, string partNumber);
}
