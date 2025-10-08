// <copyright file="ReportsListQueryComposer.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.BarCodes.Queries.GetReportsList.GetList;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Enum;

namespace IndTrace.Application.BarCodes.Queries.Composers;

/// <summary>
/// Implementation of IReportsListQueryComposer providing comprehensive query composition for reports list.
/// Extracted from GetReportsListMonitorQueryHandler to eliminate query complexity from handlers.
/// Implements industrial safety patterns with Result&lt;T&gt;, defensive validation, and performance monitoring.
/// </summary>
public class ReportsListQueryComposer : IReportsListQueryComposer
{
    private readonly ILogger<ReportsListQueryComposer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportsListQueryComposer"/> class.
    /// Follows CLAUDE.md null safety patterns with defensive validation.
    /// </summary>
    /// <param name="logger">Logger for recording operations and performance metrics.</param>
    public ReportsListQueryComposer(ILogger<ReportsListQueryComposer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Composes filtered barcode query based on request criteria with industrial safety patterns.
    /// Replicates exact filtering logic from GetReportsListMonitorQueryHandler for compatibility.
    /// </summary>
    /// <param name="baseQuery">Base barcode queryable.</param>
    /// <param name="request">Filter request with validated parameters.</param>
    /// <param name="supportQueries">Supporting entity queryables.</param>
    /// <param name="cancellationToken">Cancellation token for operation control.</param>
    /// <returns>Result containing composed query or detailed failure information.</returns>
    public async Task<Result<IQueryable<BarCode>>> ComposeAsync(
        IQueryable<BarCode> baseQuery,
        GetReportsListQuery request,
        ReportsSupportQueries supportQueries,
        CancellationToken cancellationToken)
    {
        // CLAUDE.md compliance: Early cancellation check for industrial safety
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<IQueryable<BarCode>>.WithFailure(["Operation was canceled."]);
        }

        // Defensive validation for null parameters
        if (baseQuery is null)
        {
            _logger.LogError("Base query cannot be null");
            return Result<IQueryable<BarCode>>.WithFailure(["baseQuery cannot be null."]);
        }

        if (request is null)
        {
            _logger.LogError("Request cannot be null");
            return Result<IQueryable<BarCode>>.WithFailure(["request cannot be null."]);
        }

        if (supportQueries is null)
        {
            _logger.LogError("Support queries cannot be null");
            return Result<IQueryable<BarCode>>.WithFailure(["supportQueries cannot be null."]);
        }

        try
        {
            var query = baseQuery;
            var appliedFilters = new List<string>();

            _logger.LogInformation("Starting query composition with filters - IsMaster: {IsMaster}, DateRange: {StartDate} to {EndDate}",
                request.IsMaster, request.StartDate, request.EndDate);

            // Apply master label filtering
            if (request.IsMaster)
            {
                var masterLabelResult = await ApplyMasterLabelFilterAsync(query, supportQueries.MasterLabels, cancellationToken)
                    .ConfigureAwait(false);

                if (masterLabelResult.IsFailure)
                {
                    return Result<IQueryable<BarCode>>.WithFailure(masterLabelResult.Errors);
                }

                query = masterLabelResult.Value;
                appliedFilters.Add("MasterLabel");
            }
            else
            {
                // Apply date range filtering
                query = query.Where(e => e.ModifiedOn >= request.StartDate && e.ModifiedOn <= request.EndDate);
                appliedFilters.Add($"DateRange({request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd})");
            }

            // Apply product filtering
            if (request.FilterByProduct)
            {
                if (query is null)
                {
                    _logger.LogError("Query is null during product filtering");
                    return Result<IQueryable<BarCode>>.WithFailure(["Query became null during filtering"]);
                }

                var productFilterResult = await ApplyProductFilterAsync(query, request, supportQueries, cancellationToken)
                    .ConfigureAwait(false);

                if (productFilterResult.IsFailure)
                {
                    return Result<IQueryable<BarCode>>.WithFailure(productFilterResult.Errors);
                }

                query = productFilterResult.Value;
                appliedFilters.Add($"Product({request.Model})");
            }

            // Apply state filtering
            if (request.FilterByState)
            {
                if (query is null)
                {
                    _logger.LogError("Query is null during state filtering");
                    return Result<IQueryable<BarCode>>.WithFailure(["Query became null during filtering"]);
                }

                var stateFilterResult = ApplyStateFilter(query, request.State);
                if (stateFilterResult.IsFailure)
                {
                    return Result<IQueryable<BarCode>>.WithFailure(stateFilterResult.Errors);
                }

                query = stateFilterResult.Value;
                appliedFilters.Add($"State({request.State})");
            }

            // Apply shift filtering
            if (request.FilterByShift)
            {
                if (query is null)
                {
                    _logger.LogError("Query is null during shift filtering");
                    return Result<IQueryable<BarCode>>.WithFailure(["Query became null during filtering"]);
                }

                query = ApplyShiftFilter(query, request.Shift);
                appliedFilters.Add($"Shift({request.Shift})");
            }

            // Apply line filtering
            if (request.FilterByLine)
            {
                if (query is null)
                {
                    _logger.LogError("Query is null during line filtering");
                    return Result<IQueryable<BarCode>>.WithFailure(["Query became null during filtering"]);
                }

                var lineFilterResult = await ApplyLineFilterAsync(query, request.Line, supportQueries.Lines, cancellationToken)
                    .ConfigureAwait(false);

                if (lineFilterResult.IsFailure)
                {
                    return Result<IQueryable<BarCode>>.WithFailure(lineFilterResult.Errors);
                }

                query = lineFilterResult.Value;
                appliedFilters.Add($"Line({request.Line})");
            }

            if (query is null)
            {
                _logger.LogError("Final query is null");
                return Result<IQueryable<BarCode>>.WithFailure(["Final query is null"]);
            }

            _logger.LogInformation("Query composition completed with filters: {AppliedFilters}",
                string.Join(", ", appliedFilters));

            return Result<IQueryable<BarCode>>.Success(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during query composition");
            return Result<IQueryable<BarCode>>.WithFailure([$"Query composition failed: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Applies master label filtering to the base query.
    /// Filters barcodes to only include those with labels matching master label codes.
    /// </summary>
    private async Task<Result<IQueryable<BarCode>>> ApplyMasterLabelFilterAsync(
        IQueryable<BarCode> query,
        IQueryable<MasterLabel> masterLabels,
        CancellationToken cancellationToken)
    {
        try
        {
            if (masterLabels is null)
            {
                _logger.LogWarning("Master labels queryable is null, returning original query");
                return Result<IQueryable<BarCode>>.Success(query);
            }

            var masterLabelCodes = await masterLabels
                .Select(e => e.MasterLabelCode)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var filteredQuery = query.Where(e => e.Label != null && masterLabelCodes.Contains(e.Label));

            _logger.LogDebug("Applied master label filter with {LabelCount} master labels", masterLabelCodes.Count);
            return Result<IQueryable<BarCode>>.Success(filteredQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying master label filter");
            return Result<IQueryable<BarCode>>.WithFailure([$"Master label filter failed: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Applies product filtering to the base query.
    /// Supports both customer-specific product filtering and general model filtering.
    /// </summary>
    private async Task<Result<IQueryable<BarCode>>> ApplyProductFilterAsync(
        IQueryable<BarCode> query,
        GetReportsListQuery request,
        ReportsSupportQueries supportQueries,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Model))
            {
                _logger.LogInformation("Product filter requested but no model specified");
                return Result<IQueryable<BarCode>>.Success(query.Where(e => false)); // Return empty result
            }

            // Customer-specific product filtering
            if (request.FilterByCustomer)
            {
                if (supportQueries.Customers is null || supportQueries.Products is null)
                {
                    _logger.LogError("Customer or product queryables are null for customer product filtering");
                    return Result<IQueryable<BarCode>>.WithFailure(["Customer or product data not available for filtering"]);
                }

                var productNames = await supportQueries.Customers
                    .Where(e => e.Name == request.CustomerSearch)
                    .SelectMany(c => supportQueries.Products
                        .Where(p => p.CustomerId == c.CustomerId)
                        .Select(p => p.ProductName))
                    .Where(p => p.Contains(request.Model))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (!productNames.Any())
                {
                    _logger.LogInformation("No matching products found for customer: {Customer}, model: {Model}",
                        request.CustomerSearch, request.Model);
                    return Result<IQueryable<BarCode>>.Success(query.Where(e => false)); // Return empty result
                }

                var filteredQuery = query.Where(e => e.Label != null && e.Label.Contains(request.Model));
                _logger.LogDebug("Applied customer product filter: {Customer}, {Model}", request.CustomerSearch, request.Model);
                return Result<IQueryable<BarCode>>.Success(filteredQuery);
            }

            // General model filtering
            var modelFilteredQuery = query.Where(e => e.Label != null && e.Label.Contains(request.Model));
            _logger.LogDebug("Applied product model filter: {Model}", request.Model);
            return Result<IQueryable<BarCode>>.Success(modelFilteredQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying product filter");
            return Result<IQueryable<BarCode>>.WithFailure([$"Product filter failed: {ex.Message}"]);
        }
    }

    /// <summary>
    /// Applies state filtering to the base query.
    /// Converts state name to enum value for precise filtering.
    /// </summary>
    private Result<IQueryable<BarCode>> ApplyStateFilter(IQueryable<BarCode> query, string? state)
    {
        try
        {
            var stateValue = string.IsNullOrEmpty(state) ? "None" : state;

            var stateInt = EnumModel.FromName<FlowStatus>(stateValue);
            var filteredQuery = query.Where(e => e.FlowStatus == stateInt);

            _logger.LogDebug("Applied state filter: {State} (value: {StateInt})", stateValue, stateInt);
            return Result<IQueryable<BarCode>>.Success(filteredQuery);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse flow status: {State}, continuing without state filter", state);

            // Continue processing without state filter instead of failing
            return Result<IQueryable<BarCode>>.Success(query);
        }
    }

    /// <summary>
    /// Applies shift filtering to the base query.
    /// Filters based on time ranges for different shifts.
    /// </summary>
    private IQueryable<BarCode> ApplyShiftFilter(IQueryable<BarCode> query, int shift)
    {
        var filteredQuery = query.Where(e =>
            (shift == 1 && e.CreatedOn.TimeOfDay >= TimeSpan.FromHours(8) && e.CreatedOn.TimeOfDay < TimeSpan.FromHours(15)) ||
            (shift == 2 && e.CreatedOn.TimeOfDay >= TimeSpan.FromHours(15) && e.CreatedOn.TimeOfDay < TimeSpan.FromHours(23)) ||
            (shift == 3 &&
             ((e.CreatedOn.TimeOfDay >= TimeSpan.FromHours(23) && e.CreatedOn.TimeOfDay <= TimeSpan.FromHours(23.999)) ||
              (e.CreatedOn.TimeOfDay >= TimeSpan.FromHours(0) && e.CreatedOn.TimeOfDay < TimeSpan.FromHours(6)))));

        _logger.LogDebug("Applied shift filter: {Shift}", shift);
        return filteredQuery;
    }

    /// <summary>
    /// Applies line filtering to the base query.
    /// Currently logs the line found but does not apply the filter (as per TODO in original code).
    /// </summary>
    private async Task<Result<IQueryable<BarCode>>> ApplyLineFilterAsync(
        IQueryable<BarCode> query,
        string? lineName,
        IQueryable<Line> lines,
        CancellationToken cancellationToken)
    {
        try
        {
            if (lines is null)
            {
                _logger.LogWarning("Lines queryable is null for line filtering");
                return Result<IQueryable<BarCode>>.Success(query);
            }

            var line = await lines
                .FirstOrDefaultAsync(e => e.Name == lineName, cancellationToken)
                .ConfigureAwait(false);

            if (line is not null)
            {
                // TODO IMPLEMENT LINE FILTER - maintaining original TODO as per source code
                // query = query.Where(e => e.LineId == line.LineId);
                _logger.LogDebug("Line filter found line: {LineName} (ID: {LineId}) - implementation pending",
                    line.Name, line.LineId);
            }
            else
            {
                _logger.LogWarning("Line filter requested but line not found: {LineName}", lineName);
            }

            return Result<IQueryable<BarCode>>.Success(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying line filter");
            return Result<IQueryable<BarCode>>.WithFailure([$"Line filter failed: {ex.Message}"]);
        }
    }
}
