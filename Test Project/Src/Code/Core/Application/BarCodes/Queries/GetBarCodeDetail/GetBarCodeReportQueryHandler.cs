// <copyright file="GetBarCodeReportQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.BarCodes.Queries.DataLoaders;
using IndTrace.Application.BarCodes.Queries.Mappers;
using IndQuestResults.Operations;

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeDetail;

/// <summary>
/// Handles the retrieval of detailed barcode information for reporting purposes.
/// Refactored to use SRP-compliant shared services for data loading and mapping.
/// Orchestrates barcode report generation with industrial safety patterns.
/// </summary>
public class GetBarCodeReportQueryHandler : IMonitorRequestHandler<GetBarCodeDetailQuery, BarCodeDetailVm>
{
    private readonly IBarCodeDetailDataLoader _dataLoader;
    private readonly IBarCodeDetailMapper _mapper;
    private readonly IBarCodeResult _barCodeResult;
    private readonly ILogger<GetBarCodeReportQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBarCodeReportQueryHandler"/> class.
    /// Refactored constructor with reduced dependencies following SRP principles.
    /// </summary>
    /// <param name="dataLoader">Service for loading comprehensive barcode detail data.</param>
    /// <param name="mapper">Service for mapping loaded data to view models.</param>
    /// <param name="barCodeResult">Service for barcode business logic processing.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public GetBarCodeReportQueryHandler(
        IBarCodeDetailDataLoader dataLoader,
        IBarCodeDetailMapper mapper,
        IBarCodeResult barCodeResult,
        ILogger<GetBarCodeReportQueryHandler> logger)
    {
        this._dataLoader = dataLoader ?? throw new ArgumentNullException(nameof(dataLoader));
        this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this._barCodeResult = barCodeResult ?? throw new ArgumentNullException(nameof(barCodeResult));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes the barcode detail query and returns comprehensive report data.
    /// Refactored to use SRP-compliant services for data loading and view model assembly.
    /// </summary>
    /// <param name="request">The query containing barcode identification and machine information.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A result containing the detailed barcode report view model.</returns>
    public async Task<Result<BarCodeDetailVm>> ProcessAsync(GetBarCodeDetailQuery request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<BarCodeDetailVm>.WithFailure(["Operation was canceled."]);
        }

        var stopwatch = Stopwatch.StartNew();

        var result = await Result.Success(request)
            .ValidateNotNull(req => (req, nameof(request)))
            .Ensure(req => !string.IsNullOrWhiteSpace(req.BarCode) && req.BarCode.Length >= 3, "BarCode must be at least 3 characters long.")
            .Ensure(req => !string.IsNullOrWhiteSpace(req.PartNumber) && req.PartNumber.Length >= 3, "PartNumber must be at least 3 characters long.")
            .ThenAsync(req => _barCodeResult.GetBarCodeDetails(new BarCodeDetailsRequest(req.MachineId, req.BarCode, req.PartNumber), cancellationToken))
            .Ensure(info => info is not null, "BarCode not found.")
            .ThenAsync(info => _dataLoader.LoadByBarCodeIdAsync(info!.BarCodeId, cancellationToken)
                                          .ThenMap(data => new BarCodeDetailContext(info, data.Cycles, data.Registers, data.Variables)))
            .ThenAsync(context => _mapper.AssembleReportAsync(context, cancellationToken))
            .TapError(errors => _logger.LogError("Failed to process barcode report: {Errors}", string.Join(", ", errors)));

        stopwatch.Stop();
        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "Successfully processed barcode detail query for BarCode: {BarCode} in {ElapsedMs}ms, generated {RegisterVmCount} register view models",
                request.BarCode, stopwatch.ElapsedMilliseconds, result.Value?.RegistersVm.Count ?? 0);
        }

        return result;
    }
}
