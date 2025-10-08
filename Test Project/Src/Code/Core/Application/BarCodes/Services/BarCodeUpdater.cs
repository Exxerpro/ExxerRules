// <copyright file="BarCodeUpdater.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.Repositories;

namespace IndTrace.Application.BarCodes.Services;

/// <summary>
/// Updates BarCode entities with new status and timestamps.
/// Based on CreateCyclesCommandHandler BarCode update logic.
/// Implements CLAUDE.md compliance: Result pattern, cancellation support, defensive validation.
/// </summary>
public class BarCodeUpdater : IBarCodeUpdater
{
    private readonly IRepository<BarCode> _barCodeRepository;
    private readonly ILogger<BarCodeUpdater> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarCodeUpdater"/> class.
    /// </summary>
    /// <param name="barCodeRepository">Repository for barcode persistence.</param>
    /// <param name="logger">Logger for recording update operations.</param>
    public BarCodeUpdater(
        IRepository<BarCode> barCodeRepository,
        ILogger<BarCodeUpdater> logger)
    {
        _barCodeRepository = barCodeRepository ?? throw new ArgumentNullException(nameof(barCodeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Updates BarCode with new flow/part status and modified timestamp.
    /// </summary>
    /// <param name="request">Request containing update parameters.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result indicating success or failure with reasons.</returns>
    public async Task<Result> UpdateAsync(
        BarCodeUpdateRequest request,
        CancellationToken cancellationToken)
    {
        // CLAUDE.md compliance: early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.WithFailure(["Operation was canceled."]);
        }

        // CLAUDE.md compliance: defensive validation
        if (request is null)
        {
            _logger.LogError("BarCodeUpdateRequest cannot be null");
            return Result.WithFailure(["Request cannot be null."]);
        }

        try
        {
            _logger.LogInformation(
                "Updating BarCode {BarCodeId} with PartStatus={PartStatus}, FlowStatus={FlowStatus}, MachineId={MachineId}",
                request.BarCodeId, request.PartStatus, request.FlowStatus, request.MachineId);

            var barcodeResult = await _barCodeRepository
                .GetByIdAsync(request.BarCodeId, cancellationToken)
                .ConfigureAwait(false);

            if (barcodeResult.IsFailure)
            {
                _logger.LogError(
                    "Failed to retrieve BarCode {BarCodeId}: {Error}",
                    request.BarCodeId, barcodeResult.Error);
                return Result.WithFailure([barcodeResult.Error]);
            }

            var barcode = barcodeResult.Value;
            if (barcode is null)
            {
                _logger.LogError("BarCode {BarCodeId} entity is null", request.BarCodeId);
                return Result.WithFailure(["Barcode cannot be null"]);
            }

            // Apply updates from original CreateCyclesCommandHandler logic
            barcode.PartStatus = request.PartStatus;
            barcode.FlowStatus = request.FlowStatus;
            barcode.MachineId = request.MachineId;
            barcode.ModifiedOn = request.ModifiedOn.ToLocalTime().DateTime;

            await _barCodeRepository.UpdateAsync(barcode, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation(
                "Successfully updated BarCode {BarCodeId}: PartStatus={PartStatus}, FlowStatus={FlowStatus}, MachineId={MachineId}",
                request.BarCodeId, request.PartStatus, request.FlowStatus, request.MachineId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update BarCode {BarCodeId}", request.BarCodeId);
            return Result.WithFailure([$"Failed to update: {ex.Message}"]);
        }
    }
}
