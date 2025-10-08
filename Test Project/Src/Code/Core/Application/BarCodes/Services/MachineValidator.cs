namespace IndTrace.Application.BarCodes.Services;

/// <summary>
/// Validates machine capabilities for barcode creation.
/// Implements business rule: Only Printer or InitialPrinter machines can create barcodes.
/// </summary>
public class MachineValidator : IMachineValidator
{
    private readonly IReadOnlyRepository<Machine> _machineRepository;
    private readonly ILogger<MachineValidator> _logger;

    public MachineValidator(
        IReadOnlyRepository<Machine> machineRepository,
        ILogger<MachineValidator> logger)
    {
        _machineRepository = machineRepository ?? throw new ArgumentNullException(nameof(machineRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates that the machine exists and is of printer type (Printer or InitialPrinter).
    /// Implements critical manufacturing safety rule for barcode creation authorization.
    /// </summary>
    /// <param name="machineId">The machine identifier to validate</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Success with Machine entity if valid, Failure with error message if invalid</returns>
    public async Task<Result<Machine>> ValidatePrinterMachineAsync(int machineId, CancellationToken cancellationToken)
    {
        // Early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<Machine>.WithFailure(["Operation was canceled."]);
        }

        // Null guard for dependencies
        if (_machineRepository is null)
        {
            return Result<Machine>.WithFailure(["_machineRepository cannot be null."]);
        }

        try
        {
            // Business rule: Machine ID must be positive
            if (machineId <= 0)
            {
                _logger.LogWarning("Machine validation failed: Invalid machine ID {MachineId}", machineId);
                return Result<Machine>.WithFailure([$"Machine {machineId} number invalid"]);
            }

            // Retrieve machine entity
            var machineResult = await _machineRepository.GetByIdAsync(machineId, cancellationToken);
            if (machineResult.IsFailure || machineResult.Value is null)
            {
                _logger.LogWarning("Machine validation failed: Machine {MachineId} not found", machineId);
                return Result<Machine>.WithFailure([$"Machine {machineId} does not exist or does not can create label"]);
            }

            var machine = machineResult.Value;

            // Critical business rule: Only printer-type machines can create barcodes
            if (!IsPrinterType(machine))
            {
                _logger.LogWarning("Machine validation failed: Machine {MachineId} type {MachineType} is not a printer",
                    machineId, machine.MachineType);
                return Result<Machine>.WithFailure([$"Machine {machineId} does not exist or does not can create label"]);
            }

            _logger.LogDebug("Machine validation successful: Machine {MachineId} type {MachineType}",
                machineId, machine.MachineType);
            return Result<Machine>.Success(machine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Machine validation error for MachineId={MachineId}", machineId);
            return Result<Machine>.WithFailure([$"Machine validation failed: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Determines if a machine is authorized for barcode creation.
    /// Business rule: Only Printer and InitialPrinter types are authorized.
    /// </summary>
    /// <param name="machine">The machine to check</param>
    /// <returns>True if machine can create barcodes, false otherwise</returns>
    private static bool IsPrinterType(Machine machine)
    {
        return machine.MachineType == MachineType.Printer ||
               machine.MachineType == MachineType.InitialPrinter;
    }
}
