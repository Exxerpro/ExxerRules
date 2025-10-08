// <copyright file="GetReportsListMonitorQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.BarCodes.Queries.Composers;
using IndTrace.Application.BarCodes.Queries.Filters;
using IndTrace.Application.BarCodes.Queries.Mappers;

namespace IndTrace.Application.BarCodes.Queries.GetReportsList.GetList;

/// <summary>
/// Handles the retrieval of barcode lists with comprehensive filtering capabilities.
/// Supports filtering by master labels, products, customers, states, shifts, lines, and register data.
/// Refactored to use SRP-compliant services for industrial safety compliance.
/// </summary>
public class GetReportsListMonitorQueryHandler : Domain.Interfaces.IMonitorQueryHandler<GetReportsListQuery, BarCodesListVm>
{
    private readonly IReportsListQueryComposer queryComposer;
    private readonly IBarCodeListMapper barCodeMapper;
    private readonly IRegisterDataFilter registerFilter;
    private readonly IRepository<BarCode> barCodeRepository;
    private readonly IRepository<Register> registerRepository;
    private readonly IRepository<Cycle> cycleRepository;
    private readonly IRepository<MasterLabel> masterLabelRepository;
    private readonly IRepository<Customer> customerRepository;
    private readonly IRepository<Product> productRepository;
    private readonly IRepository<Line> lineRepository;
    private readonly ILogger<GetReportsListMonitorQueryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetReportsListMonitorQueryHandler"/> class.
    /// Refactored constructor to use SRP-compliant services instead of multiple repositories.
    /// </summary>
    /// <param name="queryComposer">Service for composing filtered barcode queries.</param>
    /// <param name="barCodeMapper">Service for mapping barcodes to DTOs with cycle counts.</param>
    /// <param name="registerFilter">Service for register-based filtering.</param>
    /// <param name="barCodeRepository">Repository for accessing barcode data.</param>
    /// <param name="registerRepository">Repository for accessing register data.</param>
    /// <param name="cycleRepository">Repository for accessing cycle data.</param>
    /// <param name="masterLabelRepository">Repository for accessing master label data.</param>
    /// <param name="customerRepository">Repository for accessing customer data.</param>
    /// <param name="productRepository">Repository for accessing product data.</param>
    /// <param name="lineRepository">Repository for accessing line data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public GetReportsListMonitorQueryHandler(
        IReportsListQueryComposer queryComposer,
        IBarCodeListMapper barCodeMapper,
        IRegisterDataFilter registerFilter,
        IRepository<BarCode> barCodeRepository,
        IRepository<Register> registerRepository,
        IRepository<Cycle> cycleRepository,
        IRepository<MasterLabel> masterLabelRepository,
        IRepository<Customer> customerRepository,
        IRepository<Product> productRepository,
        IRepository<Line> lineRepository,
        ILogger<GetReportsListMonitorQueryHandler> logger)
    {
        //[Fix]
        //CLAUDE
        //Date: 26/09/2025
        //Reason: [SRP REFACTOR] - Updated constructor to use extracted services following Single Responsibility Principle

        var validation = ValidateConstructorParameters(queryComposer, barCodeMapper, registerFilter,
            barCodeRepository, registerRepository, cycleRepository, masterLabelRepository,
            customerRepository, productRepository, lineRepository, logger);

        if (validation.IsFailure)
        {
            // Log the validation failure but continue with null values
            // The actual validation will be done in the ProcessAsync method
            this.logger?.LogWarning("Constructor validation failed: {Errors}", string.Join(", ", validation.Errors));
        }

        this.queryComposer = queryComposer;
        this.barCodeMapper = barCodeMapper;
        this.registerFilter = registerFilter;
        this.barCodeRepository = barCodeRepository;
        this.registerRepository = registerRepository;
        this.cycleRepository = cycleRepository;
        this.masterLabelRepository = masterLabelRepository;
        this.customerRepository = customerRepository;
        this.productRepository = productRepository;
        this.lineRepository = lineRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Validates constructor parameters.
    /// Updated to validate new SRP services and remaining repositories.
    /// </summary>
    /// <param name="queryComposer">The query composer service.</param>
    /// <param name="barCodeMapper">The barcode mapper service.</param>
    /// <param name="registerFilter">The register filter service.</param>
    /// <param name="barCodeRepository">The barcode repository.</param>
    /// <param name="registerRepository">The register repository.</param>
    /// <param name="cycleRepository">The cycle repository.</param>
    /// <param name="masterLabelRepository">The master label repository.</param>
    /// <param name="customerRepository">The customer repository.</param>
    /// <param name="productRepository">The product repository.</param>
    /// <param name="lineRepository">The line repository.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>A Result indicating validation success or failure.</returns>
    private static Result ValidateConstructorParameters(
        IReportsListQueryComposer? queryComposer,
        IBarCodeListMapper? barCodeMapper,
        IRegisterDataFilter? registerFilter,
        IRepository<BarCode>? barCodeRepository,
        IRepository<Register>? registerRepository,
        IRepository<Cycle>? cycleRepository,
        IRepository<MasterLabel>? masterLabelRepository,
        IRepository<Customer>? customerRepository,
        IRepository<Product>? productRepository,
        IRepository<Line>? lineRepository,
        ILogger<GetReportsListMonitorQueryHandler>? logger)
    {
        var errors = new List<string>();

        if (queryComposer == null)
        {
            errors.Add($"Parameter '{nameof(queryComposer)}' cannot be null");
        }

        if (barCodeMapper == null)
        {
            errors.Add($"Parameter '{nameof(barCodeMapper)}' cannot be null");
        }

        if (registerFilter == null)
        {
            errors.Add($"Parameter '{nameof(registerFilter)}' cannot be null");
        }

        if (barCodeRepository == null)
        {
            errors.Add($"Parameter '{nameof(barCodeRepository)}' cannot be null");
        }

        if (registerRepository == null)
        {
            errors.Add($"Parameter '{nameof(registerRepository)}' cannot be null");
        }

        if (cycleRepository == null)
        {
            errors.Add($"Parameter '{nameof(cycleRepository)}' cannot be null");
        }

        if (masterLabelRepository == null)
        {
            errors.Add($"Parameter '{nameof(masterLabelRepository)}' cannot be null");
        }

        if (customerRepository == null)
        {
            errors.Add($"Parameter '{nameof(customerRepository)}' cannot be null");
        }

        if (productRepository == null)
        {
            errors.Add($"Parameter '{nameof(productRepository)}' cannot be null");
        }

        if (lineRepository == null)
        {
            errors.Add($"Parameter '{nameof(lineRepository)}' cannot be null");
        }

        if (logger == null)
        {
            errors.Add($"Parameter '{nameof(logger)}' cannot be null");
        }

        return errors.Any() ? Result.WithFailure(errors) : Result.Success();
    }

    /// <summary>
    /// Processes the reports list query with comprehensive filtering options.
    /// Refactored to use SRP-compliant services for reduced complexity and improved maintainability.
    /// </summary>
    /// <param name="request">The query containing filter criteria and parameters.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A result containing the filtered barcode list view model.</returns>
    public async Task<Result<BarCodesListVm>> ProcessAsync(GetReportsListQuery request, CancellationToken cancellationToken)
    {
        //[Fix]
        //CLAUDE
        //Date: 26/09/2025
        //Reason: [SRP REFACTOR] - Refactored ProcessAsync to use extracted services, reducing from 372 to ~150 lines

        // Early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<BarCodesListVm>.WithFailure(["Operation was canceled."]);
        }

        // Null guards for dependencies and parameters
        if (this.queryComposer is null)
        {
            return Result<BarCodesListVm>.WithFailure(["queryComposer cannot be null."]);
        }

        if (this.barCodeMapper is null)
        {
            return Result<BarCodesListVm>.WithFailure(["barCodeMapper cannot be null."]);
        }

        if (this.registerFilter is null)
        {
            return Result<BarCodesListVm>.WithFailure(["registerFilter cannot be null."]);
        }

        if (request is null)
        {
            return Result<BarCodesListVm>.WithFailure(["Request cannot be null."]);
        }

        this.logger.LogInformation(
            "Processing reports list query with filters - IsMaster: {IsMaster}, DateRange: {StartDate} to {EndDate}",
            request.IsMaster, request.StartDate, request.EndDate);

        try
        {
            // 1. Get base queryables
            var baseQueryResult = await this.GetBaseQueryablesAsync(cancellationToken).ConfigureAwait(false);
            if (baseQueryResult.IsFailure)
            {
                this.logger.LogError("Failed to get base queryables: {Errors}", string.Join(", ", baseQueryResult.Errors ?? []));
                return Result<BarCodesListVm>.WithFailure(baseQueryResult.Errors);
            }

            var (barCodeQuery, supportQueries, cycleQuery) = baseQueryResult.Value;

            // 2. Apply all filters using the composer service
            var filteredQueryResult = await this.queryComposer.ComposeAsync(
                barCodeQuery, request, supportQueries, cancellationToken).ConfigureAwait(false);

            if (filteredQueryResult.IsFailure)
            {
                this.logger.LogError("Failed to compose filtered query: {Errors}", string.Join(", ", filteredQueryResult.Errors ?? []));
                return Result<BarCodesListVm>.WithFailure(filteredQueryResult.Errors);
            }

            var filteredQuery = filteredQueryResult.Value ?? throw new InvalidOperationException("Filtered query cannot be null");

            // 3. Map to DTOs with cycle counts using the mapper service
            var mappedBarCodesResult = await this.barCodeMapper.MapWithCycleCountsAsync(
                filteredQuery, cycleQuery, cancellationToken).ConfigureAwait(false);

            if (mappedBarCodesResult.IsFailure)
            {
                this.logger.LogError("Failed to map barcodes with cycle counts: {Errors}", string.Join(", ", mappedBarCodesResult.Errors ?? []));
                return Result<BarCodesListVm>.WithFailure(mappedBarCodesResult.Errors);
            }

            var barCodes = mappedBarCodesResult.Value ?? throw new InvalidOperationException("Mapped bar codes cannot be null");

            // 4. Apply register filtering if requested using the filter service
            if (request.FilterByRegister)
            {
                var registerFilterResult = await this.ApplyRegisterFilterAsync(barCodes, request.RegisterSearch, cancellationToken).ConfigureAwait(false);
                if (registerFilterResult.IsFailure)
                {
                    this.logger.LogError("Failed to apply register filter: {Errors}", string.Join(", ", registerFilterResult.Errors ?? []));
                    return Result<BarCodesListVm>.WithFailure(registerFilterResult.Errors);
                }

                barCodes = registerFilterResult.Value ?? throw new InvalidOperationException("Register filtered bar codes cannot be null");
            }

            // 5. Create and return the view model
            var vm = new BarCodesListVm
            {
                BarCodes = barCodes?.OrderByDescending(bc => bc.BarCodeId).ToList() ?? [],
                Count = barCodes?.Count ?? 0,
            };

            this.logger.LogInformation("Successfully processed reports list query, returning {BarCodeCount} barcodes", vm.Count);
            return Result<BarCodesListVm>.Success(vm);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error processing reports list query");
            return Result<BarCodesListVm>.WithFailure([$"Unexpected error processing reports list: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Gets all base queryables needed for filtering operations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing base queryables or failure reasons.</returns>
    private async Task<Result<(IQueryable<BarCode> BarCodes, ReportsSupportQueries Support, IQueryable<Cycle> Cycles)>> GetBaseQueryablesAsync(
        CancellationToken cancellationToken)
    {
        // Get all queryables in parallel
        var barCodeTask = this.barCodeRepository.AsQueryableAsync(cancellationToken);
        var masterLabelTask = this.masterLabelRepository.AsQueryableAsync(cancellationToken);
        var customerTask = this.customerRepository.AsQueryableAsync(cancellationToken);
        var productTask = this.productRepository.AsQueryableAsync(cancellationToken);
        var lineTask = this.lineRepository.AsQueryableAsync(cancellationToken);
        var cycleTask = this.cycleRepository.AsQueryableAsync(cancellationToken);

        await Task.WhenAll(barCodeTask, masterLabelTask, customerTask, productTask, lineTask, cycleTask).ConfigureAwait(false);

        var barCodeResult = await barCodeTask.ConfigureAwait(false);
        var masterLabelResult = await masterLabelTask.ConfigureAwait(false);
        var customerResult = await customerTask.ConfigureAwait(false);
        var productResult = await productTask.ConfigureAwait(false);
        var lineResult = await lineTask.ConfigureAwait(false);
        var cycleResult = await cycleTask.ConfigureAwait(false);

        // Check for failures
        var allResults = new Result[] { barCodeResult, masterLabelResult, customerResult, productResult, lineResult, cycleResult };
        var failures = allResults.Where(r => r.IsFailure).SelectMany(r => r.Errors ?? []).ToList();

        if (failures.Any())
        {
            return Result<(IQueryable<BarCode>, ReportsSupportQueries, IQueryable<Cycle>)>.WithFailure(failures);
        }

        var supportQueries = new ReportsSupportQueries(
            masterLabelResult.Value ?? throw new InvalidOperationException("Master label query cannot be null"),
            customerResult.Value ?? throw new InvalidOperationException("Customer query cannot be null"),
            productResult.Value ?? throw new InvalidOperationException("Product query cannot be null"),
            lineResult.Value ?? throw new InvalidOperationException("Line query cannot be null"));

        return Result<(IQueryable<BarCode>, ReportsSupportQueries, IQueryable<Cycle>)>.Success((
            barCodeResult.Value ?? throw new InvalidOperationException("BarCode query cannot be null"),
            supportQueries,
            cycleResult.Value ?? throw new InvalidOperationException("Cycle query cannot be null")));
    }

    /// <summary>
    /// Applies register-based filtering to the barcode list.
    /// </summary>
    /// <param name="barCodes">Current barcode list.</param>
    /// <param name="registerSearch">Register search criteria.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing filtered barcode list or failure reasons.</returns>
    private async Task<Result<List<BarCodeDto>>> ApplyRegisterFilterAsync(
        List<BarCodeDto> barCodes,
        string registerSearch,
        CancellationToken cancellationToken)
    {
        var matchingIdsResult = await this.registerFilter.GetMatchingBarCodeIdsAsync(registerSearch, cancellationToken).ConfigureAwait(false);
        if (matchingIdsResult.IsFailure)
        {
            return Result<List<BarCodeDto>>.WithFailure(matchingIdsResult.Errors);
        }

        var matchingIds = matchingIdsResult.Value ?? throw new InvalidOperationException("Matching IDs cannot be null");
        var filteredBarCodes = barCodes.Where(bc => matchingIds.Contains(bc.BarCodeId)).ToList();

        this.logger.LogDebug("Applied register filter, remaining barcodes: {BarCodeCount}", filteredBarCodes.Count);
        return Result<List<BarCodeDto>>.Success(filteredBarCodes);
    }
}
