namespace IndTrace.Application.Common.Services;

/// <summary>
/// Handles cross-cutting audit logging concerns for gateway operations.
/// Provides consistent audit trail creation for all gateway command failures.
/// </summary>
public class AuditLogger : IAuditLogger
{
    private readonly IRepository<TaskGatewayRequest> _requestRepository;
    private readonly IDateTimeMachine _dateTimeMachine;
    private readonly ILogger<AuditLogger> _logger;

    public AuditLogger(
        IRepository<TaskGatewayRequest> requestRepository,
        IDateTimeMachine dateTimeMachine,
        ILogger<AuditLogger> logger)
    {
        _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
        _dateTimeMachine = dateTimeMachine ?? throw new ArgumentNullException(nameof(dateTimeMachine));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Logs a gateway operation failure with appropriate result validation type.
    /// Creates TaskGatewayRequest audit record for compliance and debugging.
    /// </summary>
    /// <param name="machineId">The machine identifier where the operation failed</param>
    /// <param name="validationType">The specific validation failure type for categorization</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Result indicating success or failure of audit logging</returns>
    public async Task<Result> LogFailureAsync(int machineId, ResultValidation validationType, CancellationToken cancellationToken)
    {
        return await LogFailureAsync(machineId, string.Empty, validationType, cancellationToken);
    }

    /// <summary>
    /// Logs a gateway operation failure with custom part number context.
    /// Used when the failure is related to specific part number processing.
    /// </summary>
    /// <param name="machineId">The machine identifier where the operation failed</param>
    /// <param name="partNumber">The part number being processed when failure occurred</param>
    /// <param name="validationType">The specific validation failure type for categorization</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Result indicating success or failure of audit logging</returns>
    public async Task<Result> LogFailureAsync(int machineId, string partNumber, ResultValidation validationType, CancellationToken cancellationToken)
    {
        // Early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure(["Operation was canceled."]);
        }

        // Null guard for dependencies
        if (_requestRepository is null)
        {
            return Result.WithFailure(["_requestRepository cannot be null."]);
        }

        try
        {
            _logger.LogDebug("Creating audit log for failure: MachineId={MachineId}, PartNumber={PartNumber}, ValidationType={ValidationType}",
                machineId, partNumber, validationType);

            // Create standardized audit record
            var auditCommand = new TaskGatewayRequest
            {
                MachineId = machineId,
                PartNumber = partNumber ?? string.Empty,
                CycleStatus = CycleStatus.Started,
                FlowStatus = FlowStatus.Created,
                PartStatus = PartStatus.Ok,
                GatewayTask = GatewayTask.CreateBarCodeAsync,
                TimeStamp = _dateTimeMachine.Now.ToLocalTime(),
                ResultValidation = validationType
            };

            // Persist audit record
            var result = await _requestRepository.AddAsync(auditCommand, cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to create audit log: {Errors}", string.Join(", ", result.Errors ?? []));
                return Result.WithFailure(["Audit logging failed"]);
            }

            _logger.LogDebug("Audit log created successfully for MachineId={MachineId}, ValidationType={ValidationType}",
                machineId, validationType);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Audit logging error for MachineId={MachineId}, ValidationType={ValidationType}",
                machineId, validationType);
            return Result.WithFailure([$"Audit logging failed: {ex.Message}"]);
        }
    }
}
