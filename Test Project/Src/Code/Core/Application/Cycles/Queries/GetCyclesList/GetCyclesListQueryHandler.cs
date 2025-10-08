// <copyright file="GetCyclesListQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Cycles.Queries.GetCyclesList;

/// <summary>
/// Handles queries for retrieving lists of production cycles with optional filtering.
/// </summary>
/// <remarks>
/// This handler provides access to cycle data for monitoring and reporting purposes,
/// supporting both filtered views by barcode ID and general cycle listings with pagination.
/// It's essential for production tracking, performance analysis, and cycle audit trails.
/// </remarks>
public class GetCyclesListQueryHandler : IMonitorRequestHandler<GetCyclesListQuery, CyclesListVm>
{
    private readonly IRepository<Cycle> repository;
    private readonly ILogger<GetCyclesListQueryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCyclesListQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">Repository for accessing cycle data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public GetCyclesListQueryHandler(IRepository<Cycle> repository, ILogger<GetCyclesListQueryHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    /// <summary>
    /// Processes the cycles list query and returns filtered cycle data.
    /// </summary>
    /// <param name="request">The query containing optional filtering criteria.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A result containing the cycles list view model with cycle data and count information.</returns>
    /// <remarks>
    /// This method applies the following filtering logic:
    /// - If a specific barcode ID is provided, returns all cycles for that barcode
    /// - If no ID is provided, returns the most recent 250 cycles ordered by cycle ID
    /// This ensures both targeted analysis and general production monitoring capabilities.
    /// </remarks>
    public async Task<Result<CyclesListVm>> ProcessAsync(GetCyclesListQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<CyclesListVm>.WithFailure("request cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<CyclesListVm>.WithFailure("Operation was canceled.");
        }

        const int MaxPageSize = 250;
        try
        {
            ISpecification<Cycle> spec;
            int effectivePageSize;
            int skip = 0;

            if (request.Id > 0)
            {
                // Filter by barcode, order by CycleId descending, limit to MaxPageSize
                effectivePageSize = Math.Min(request.PageSize > 0 ? request.PageSize : MaxPageSize, MaxPageSize);
                spec = new Specification<Cycle>(c => c.BarCodeId == request.Id)
                    .AddOrderByDescending(c => c.CycleId)
                    .ApplyPaging(0, effectivePageSize);
            }
            else
            {
                // Use pagination, clamp page size
                effectivePageSize = Math.Min(request.PageSize > 0 ? request.PageSize : 50, MaxPageSize);
                int page = request.Page > 0 ? request.Page : 1;
                skip = (page - 1) * effectivePageSize;
                spec = new Specification<Cycle>(c => true)
                    .AddOrderByDescending(c => c.CycleId)
                    .ApplyPaging(skip, effectivePageSize);
            }

            var getResult = await this.repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
            if (getResult.IsFailure)
            {
                this.logger.LogError("Failed to retrieve Cycles: {Errors}", string.Join(", ", getResult.Errors ?? []));
                return Result<CyclesListVm>.WithFailure(getResult.Errors);
            }

            var cycles = getResult.Value?.ToList() ?? [];
            var cycleResult = CyclesDto.ToDtoList(cycles);
            if (cycleResult.IsFailure || cycleResult.Value is null)
            {
                return Result<CyclesListVm>.WithFailure(cycleResult.Errors);
            }

            var cycleView = cycleResult.Value.ToList();
            var vm = new CyclesListVm
            {
                Cycles = cycleView,
                Count = cycles.Count,
            };

            return Result<CyclesListVm>.Success(vm);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception in GetCyclesListQueryHandler");
            return Result<CyclesListVm>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }
}
