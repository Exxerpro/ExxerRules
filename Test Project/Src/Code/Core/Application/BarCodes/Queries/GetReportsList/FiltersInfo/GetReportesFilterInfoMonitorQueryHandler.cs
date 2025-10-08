// <copyright file="GetReportesFilterInfoMonitorQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.BarCodes.Queries.Builders;

namespace IndTrace.Application.BarCodes.Queries.GetReportsList.FiltersInfo;

/// <summary>
/// Handles retrieval of filter information for reports list functionality.
/// Refactored to use SRP-compliant services for industrial safety compliance.
/// </summary>
public class GetReportesFilterInfoMonitorQueryHandler : IMonitorQueryHandler<GetReportsFilterInfoQuery, ReportsFilterInfoVm>
{
    private readonly IReportsFilterInfoBuilder filterInfoBuilder;
    private readonly ILogger<GetReportesFilterInfoMonitorQueryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetReportesFilterInfoMonitorQueryHandler"/> class.
    /// Refactored to use SRP-compliant filter info builder service.
    /// </summary>
    /// <param name="filterInfoBuilder">Service for building comprehensive filter information.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public GetReportesFilterInfoMonitorQueryHandler(
        IReportsFilterInfoBuilder filterInfoBuilder,
        ILogger<GetReportesFilterInfoMonitorQueryHandler> logger)
    {
        //[Fix]
        //CLAUDE
        //Date: 26/09/2025
        //Reason: [SRP REFACTOR] - Updated constructor to use extracted IReportsFilterInfoBuilder service

        this.filterInfoBuilder = filterInfoBuilder ?? throw new ArgumentNullException(nameof(filterInfoBuilder));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes the reports filter info query using the SRP-compliant builder service.
    /// Refactored to delegate responsibility to IReportsFilterInfoBuilder for improved maintainability.
    /// </summary>
    /// <param name="request">The filter info query request.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing filter information or failure reasons.</returns>
    public async Task<Result<ReportsFilterInfoVm>> ProcessAsync(GetReportsFilterInfoQuery request, CancellationToken cancellationToken)
    {
        //[Fix]
        //CLAUDE
        //Date: 26/09/2025
        //Reason: [SRP REFACTOR] - Refactored ProcessAsync to use IReportsFilterInfoBuilder service, reducing from 103 to ~80 lines

        // Early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ReportsFilterInfoVm>.WithFailure(["Operation was canceled."]);
        }

        // Null guards for dependencies and parameters
        if (this.filterInfoBuilder is null)
        {
            return Result<ReportsFilterInfoVm>.WithFailure(["filterInfoBuilder cannot be null."]);
        }

        if (request is null)
        {
            return Result<ReportsFilterInfoVm>.WithFailure(["Request cannot be null."]);
        }

        this.logger.LogInformation("Processing reports filter info query");

        try
        {
            // Delegate filter information building to the specialized service
            var filterInfoResult = await this.filterInfoBuilder.BuildAsync(cancellationToken).ConfigureAwait(false);

            if (filterInfoResult.IsFailure)
            {
                this.logger.LogError("Failed to build filter information: {Errors}", string.Join(", ", filterInfoResult.Errors ?? []));
                return Result<ReportsFilterInfoVm>.WithFailure(filterInfoResult.Errors);
            }

            this.logger.LogInformation("Successfully processed reports filter info query");
            return Result<ReportsFilterInfoVm>.Success(filterInfoResult.Value ?? throw new InvalidOperationException("Filter info result cannot be null"));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error processing reports filter info query");
            return Result<ReportsFilterInfoVm>.WithFailure([$"Unexpected error processing filter info: {ex.Message}"]);
        }
    }
}
