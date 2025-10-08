using IndTrace.Domain.Services.BarCodes;
using IndTrace.Application.Shifts.Commands.Create;

namespace IndTrace.Application.BarCodes.Services;

/// <summary>
/// Coordinates barcode and cycle persistence operations.
/// Manages the creation of both BarCode and Cycle entities as a coordinated operation with shift management.
/// </summary>
public class BarCodePersistenceOrchestrator : IBarCodePersistenceOrchestrator
{
    private readonly IRepository<BarCode> _barCodeRepository;
    private readonly IRepository<Cycle> _cycleRepository;
    private readonly IRepository<TaskGatewayRequest> _requestRepository;
    private readonly IShiftService _shiftService;
    private readonly IBarCodeFactory _barCodeFactory;
    private readonly ICycleFactory _cycleFactory;
    private readonly IDateTimeMachine _dateTimeMachine;
    private readonly ILogger<BarCodePersistenceOrchestrator> _logger;

    public BarCodePersistenceOrchestrator(
        IRepository<BarCode> barCodeRepository,
        IRepository<Cycle> cycleRepository,
        IRepository<TaskGatewayRequest> requestRepository,
        IShiftService shiftService,
        IBarCodeFactory barCodeFactory,
        ICycleFactory cycleFactory,
        IDateTimeMachine dateTimeMachine,
        ILogger<BarCodePersistenceOrchestrator> logger)
    {
        _barCodeRepository = barCodeRepository ?? throw new ArgumentNullException(nameof(barCodeRepository));
        _cycleRepository = cycleRepository ?? throw new ArgumentNullException(nameof(cycleRepository));
        _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
        _shiftService = shiftService ?? throw new ArgumentNullException(nameof(shiftService));
        _barCodeFactory = barCodeFactory ?? throw new ArgumentNullException(nameof(barCodeFactory));
        _cycleFactory = cycleFactory ?? throw new ArgumentNullException(nameof(cycleFactory));
        _dateTimeMachine = dateTimeMachine ?? throw new ArgumentNullException(nameof(dateTimeMachine));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates both BarCode and Cycle entities in a coordinated manner.
    /// The Cycle entity depends on the BarCode being created first for the relationship.
    /// Includes shift management to ensure proper cycles OK count.
    /// </summary>
    /// <param name="label">The generated barcode label</param>
    /// <param name="productId">The product identifier</param>
    /// <param name="machineId">The machine identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Success with both BarCode and Cycle entities, Failure if persistence fails</returns>
    public async Task<Result<(BarCode barCode, Cycle cycle)>> CreateBarCodeAndCycleAsync(
        string label, int productId, int machineId, CancellationToken cancellationToken)
    {
        // Early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<(BarCode, Cycle)>.WithFailure(["Operation was canceled."]);
        }

        // Null guards for dependencies
        if (_barCodeRepository is null)
        {
            return Result<(BarCode, Cycle)>.WithFailure(["_barCodeRepository cannot be null."]);
        }

        if (string.IsNullOrWhiteSpace(label))
        {
            return Result<(BarCode, Cycle)>.WithFailure(["Label cannot be null or empty."]);
        }

        try
        {
            var timestamp = _dateTimeMachine.Now.ToLocalTime();
            _logger.LogDebug("Starting coordinated BarCode and Cycle creation for Label={Label}, ProductId={ProductId}, MachineId={MachineId}",
                label, productId, machineId);

            // Step 1: Create BarCode entity using domain factory
            var barCode = _barCodeFactory.CreateBarCode(label, productId, machineId, _dateTimeMachine);

            // Step 2: Persist BarCode first (required for cycle relationship)
            var barCodeResult = await PersistBarCodeAsync(barCode, cancellationToken);
            if (barCodeResult.IsFailure || barCodeResult.Value is null)
            {
                return Result<(BarCode, Cycle)>.WithFailure(barCodeResult.Errors);
            }

            var persistedBarCode = barCodeResult.Value;
            _logger.LogDebug("BarCode persisted successfully: BarCodeId={BarCodeId}", persistedBarCode.BarCodeId);

            // Step 3: Get or create shift to determine cycles OK count
            var shiftResult = await GetOrCreateShiftAsync(machineId, cancellationToken);
            if (shiftResult.IsFailure || shiftResult.Value is null)
            {
                await LogFailureAsync(machineId, cancellationToken);
                return Result<(BarCode, Cycle)>.WithFailure(shiftResult.Errors);
            }

            var shift = shiftResult.Value;
            _logger.LogDebug("Shift retrieved: CyclesOk={CyclesOk}", shift.CyclesOk);

            // Step 4: Create Cycle entity using domain factory
            var cycle = _cycleFactory.CreateInitialCycle(machineId, persistedBarCode.BarCodeId, shift!.CyclesOk, _dateTimeMachine);

            // Step 5: Persist Cycle
            var cycleResult = await PersistCycleAsync(cycle, cancellationToken);
            if (cycleResult.IsFailure || cycleResult.Value is null)
            {
                return Result<(BarCode, Cycle)>.WithFailure(cycleResult.Errors);
            }

            var persistedCycle = cycleResult.Value;
            _logger.LogInformation("BarCode and Cycle created successfully: BarCodeId={BarCodeId}, CycleId={CycleId}",
                persistedBarCode.BarCodeId, persistedCycle.CycleId);

            return Result<(BarCode, Cycle)>.Success((persistedBarCode, persistedCycle));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Coordinated persistence error for Label={Label}, ProductId={ProductId}, MachineId={MachineId}",
                label, productId, machineId);
            return Result<(BarCode, Cycle)>.WithFailure([$"Persistence failed: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Persists the BarCode entity to the repository.
    /// </summary>
    private async Task<Result<BarCode>> PersistBarCodeAsync(BarCode barCode, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Persisting BarCode with Label={Label}", barCode.Label);

        var barCodeResult = await _barCodeRepository.AddAsync(barCode, cancellationToken);

        if (barCodeResult.IsFailure)
        {
            _logger.LogError("Failed to add BarCode: {Error}", barCodeResult.Errors?.FirstOrDefault());
            return Result<BarCode>.WithFailure([$"Failed to add barcode: {barCodeResult.Errors?.FirstOrDefault()}"]);
        }

        return Result<BarCode>.Success(barCode); // Repository updates the entity with ID
    }

    /// <summary>
    /// Persists the Cycle entity to the repository.
    /// </summary>
    private async Task<Result<Cycle>> PersistCycleAsync(Cycle cycle, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Persisting Cycle for MachineId={MachineId}, BarCodeId={BarCodeId}", cycle.MachineId, cycle.BarCodeId);

        var cycleResult = await _cycleRepository.AddAsync(cycle, cancellationToken);

        if (cycleResult.IsFailure)
        {
            _logger.LogError("Failed to add Cycle: {Error}", cycleResult.Errors?.FirstOrDefault());
            return Result<Cycle>.WithFailure([$"Failed to add cycle: {cycleResult.Errors?.FirstOrDefault()}"]);
        }

        return Result<Cycle>.Success(cycle); // Repository updates the entity with ID
    }

    /// <summary>
    /// Gets or creates shift for the machine to determine cycles OK count.
    /// Critical for maintaining proper shift cycle tracking.
    /// </summary>
    private async Task<Result<ShiftCreatedEvent>> GetOrCreateShiftAsync(int machineId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Creating or retrieving shift for MachineId={MachineId}", machineId);

        var shiftResult = await _shiftService.CreateOrRetrieveShiftAndCyclesOkAsync(machineId, cancellationToken);

        if (shiftResult.IsFailure || shiftResult.Value is null)
        {
            _logger.LogError("Failed to create shift for MachineId={MachineId}", machineId);
            return Result<ShiftCreatedEvent>.WithFailure([$"Cannot create Shift {_dateTimeMachine.Now.ToLocalTime()}"]);
        }

        return Result<ShiftCreatedEvent>.Success(shiftResult.Value);
    }

    /// <summary>
    /// Logs failure to request repository for audit purposes.
    /// </summary>
    private async Task LogFailureAsync(int machineId, CancellationToken cancellationToken)
    {
        try
        {
            var command = new TaskGatewayRequest
            {
                MachineId = machineId,
                CycleStatus = CycleStatus.Started,
                FlowStatus = FlowStatus.Created,
                PartStatus = PartStatus.Ok,
                GatewayTask = GatewayTask.CreateBarCodeAsync,
                TimeStamp = _dateTimeMachine.Now.ToLocalTime(),
                ResultValidation = ResultValidation.ShiftInvalid
            };

            await _requestRepository.AddAsync(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log failure for MachineId={MachineId}", machineId);
        }
    }
}
