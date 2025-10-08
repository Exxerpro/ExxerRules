namespace IndTrace.Application.Common.Services.Interfaces;

/// <summary>
/// Handles cross-cutting audit logging concerns for gateway operations.
/// Provides consistent audit trail creation for all gateway command failures.
/// </summary>
public interface IAuditLogger
{
    /// <summary>
    /// Logs a gateway operation failure with appropriate result validation type.
    /// Creates TaskGatewayRequest audit record for compliance and debugging.
    /// </summary>
    /// <param name="machineId">The machine identifier where the operation failed</param>
    /// <param name="validationType">The specific validation failure type for categorization</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Result indicating success or failure of audit logging</returns>
    Task<Result> LogFailureAsync(int machineId, ResultValidation validationType, CancellationToken cancellationToken);

    /// <summary>
    /// Logs a gateway operation failure with custom part number context.
    /// Used when the failure is related to specific part number processing.
    /// </summary>
    /// <param name="machineId">The machine identifier where the operation failed</param>
    /// <param name="partNumber">The part number being processed when failure occurred</param>
    /// <param name="validationType">The specific validation failure type for categorization</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Result indicating success or failure of audit logging</returns>
    Task<Result> LogFailureAsync(int machineId, string partNumber, ResultValidation validationType, CancellationToken cancellationToken);
}
