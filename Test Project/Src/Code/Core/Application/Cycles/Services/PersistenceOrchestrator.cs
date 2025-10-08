// <copyright file="PersistenceOrchestrator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Services;

/// <summary>
/// Orchestrates persistence operations for cycle updates.
/// </summary>
public class PersistenceOrchestrator : IPersistenceOrchestrator
{
    private readonly IRepository<Register> _registerRepository;
    private readonly IRepository<Cycle> _cycleRepository;
    private readonly IRepository<BarCode> _barCodeRepository;
    private readonly ILogger<PersistenceOrchestrator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistenceOrchestrator"/> class.
    /// </summary>
    /// <param name="registerRepository">The register repository.</param>
    /// <param name="cycleRepository">The cycle repository.</param>
    /// <param name="barCodeRepository">The bar code repository.</param>
    /// <param name="logger">The logger instance.</param>
    public PersistenceOrchestrator(
        IRepository<Register> registerRepository,
        IRepository<Cycle> cycleRepository,
        IRepository<BarCode> barCodeRepository,
        ILogger<PersistenceOrchestrator> logger)
    {
        _registerRepository = registerRepository;
        _cycleRepository = cycleRepository;
        _barCodeRepository = barCodeRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<PersistenceResult>> PersistAsync(
        IEnumerable<Register> registers,
        Cycle cycle,
        BarCode barCode,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("PersistAsync cancelled");
            return ResultExtensions.Cancelled<PersistenceResult>();
        }

        var validation = ResultExtensions.ValidateNotNull(
            (registers, nameof(registers)),
            (cycle, nameof(cycle)),
            (barCode, nameof(barCode)));

        if (validation.IsFailure)
        {
            _logger.LogError("Validation failed: {Errors}", string.Join(", ", validation.Errors));
            return Result<PersistenceResult>.WithFailure(validation.Errors);
        }

        _logger.LogInformation(
            "Persisting data for CycleId={CycleId}, BarCodeId={BarCodeId}",
            cycle.CycleId, barCode.BarCodeId);

        try
        {
            // Save registers
            var registersList = registers.ToList();
            var registerResult = await _registerRepository
                .AddRangeBulkAsync(registersList, cancellationToken)
                .ConfigureAwait(false);

            if (registerResult.IsFailure)
            {
                _logger.LogError("Failed to save registers: {Error}", registerResult.Error);
                return Result<PersistenceResult>.WithFailure($"Failed to save registers: {registerResult.Error}");
            }

            // Update cycle
            var cycleResult = await _cycleRepository
                .UpdateAsync(cycle, cancellationToken)
                .ConfigureAwait(false);

            if (cycleResult.IsFailure)
            {
                _logger.LogError("Failed to update cycle: {Error}", cycleResult.Error);
                return Result<PersistenceResult>.WithFailure($"Failed to update cycle: {cycleResult.Error}");
            }

            // Update barcode
            var barCodeResult = await _barCodeRepository
                .UpdateAsync(barCode, cancellationToken)
                .ConfigureAwait(false);

            if (barCodeResult.IsFailure)
            {
                _logger.LogError("Failed to update barcode: {Error}", barCodeResult.Error);
                return Result<PersistenceResult>.WithFailure($"Failed to update barcode: {barCodeResult.Error}");
            }

            var result = new PersistenceResult(
                RegistersSaved: registerResult.Value,
                CycleUpdated: cycleResult.IsSuccess,
                BarCodeUpdated: barCodeResult.IsSuccess);

            _logger.LogInformation(
                "Persistence completed: RegistersSaved={RegistersSaved}, CycleUpdated={CycleUpdated}, BarCodeUpdated={BarCodeUpdated}",
                result.RegistersSaved, result.CycleUpdated, result.BarCodeUpdated);

            return Result<PersistenceResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during persistence");
            return Result<PersistenceResult>.WithFailure($"Exception during persistence: {ex.Message}");
        }
    }
}
