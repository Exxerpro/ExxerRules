// <copyright file="BarCodeInfoProvider.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Services;

/// <summary>
/// Provides bar code information retrieval functionality.
/// </summary>
public class BarCodeInfoProvider : IBarCodeInfoProvider
{
    private readonly IBarCodeResult _barCodeResult;
    private readonly ILogger<BarCodeInfoProvider> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarCodeInfoProvider"/> class.
    /// </summary>
    /// <param name="barCodeResult">The bar code result service.</param>
    /// <param name="logger">The logger instance.</param>
    public BarCodeInfoProvider(
        IBarCodeResult barCodeResult,
        ILogger<BarCodeInfoProvider> logger)
    {
        _barCodeResult = barCodeResult;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<IBarCodeResult>> GetBarCodeInfoAsync(
        int machineId,
        string barCode,
        string partNumber,
        CancellationToken cancellationToken)
    {
        // Early cancellation check with fluent validation
        var validationResult = Result<BarCodeDetailsRequest>
            .Success(new BarCodeDetailsRequest(machineId, barCode, partNumber))
            .Ensure(_ => !cancellationToken.IsCancellationRequested, "Operation was cancelled")
            .Tap(_ => _logger.LogInformation(
                "Getting bar code information for BarCode={BarCode}, MachineId={MachineId}, PartNumber={PartNumber}",
                barCode, machineId, partNumber));

        if (validationResult.IsFailure)
        {
            return cancellationToken.IsCancellationRequested
                ? ResultExtensions.Cancelled<IBarCodeResult>()
                : Result<IBarCodeResult>.WithFailure(validationResult.Errors);
        }

        try
        {
            var request = validationResult.Value!;
            var result = await _barCodeResult.GetBarCodeDetails(request, cancellationToken).ConfigureAwait(false);

            return Result<IBarCodeResult>
                .Success(result)
                .Ensure(r => r is not null, $"BarCode {barCode} not found")
                .Tap(r => _logger.LogInformation(
                    "Bar code information retrieved successfully: MachineId={MachineId}, NextMachineId={NextMachineId}, MachineType={MachineType}",
                    r!.MachineId, r.NextMachineId, r.MachineType))
                .OnFailure(errors => _logger.LogError("Bar code result validation failed: {Errors}", string.Join(", ", errors)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception getting bar code information for BarCode={BarCode}", barCode);
            return Result<IBarCodeResult>.WithFailure($"Exception getting bar code information: {ex.Message}");
        }
    }
}
