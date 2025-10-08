// <copyright file="GetConfigStationListQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigStations.Queries.GetConfigStationList;

using IndQuestResults.Operations;

/// <summary>
/// Represents the GetConfigStationListQueryHandler.
/// </summary>
public class GetConfigStationListQueryHandler : IMonitorRequestHandler<GetConfigStationListQuery, ApplicationConfiguration>
{
    private readonly IRepository<ConfigStation> repository;
    private readonly ILogger<GetConfigStationListQueryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetConfigStationListQueryHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public GetConfigStationListQueryHandler(IRepository<ConfigStation> repository, ILogger<GetConfigStationListQueryHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    /// <summary>
    /// Executes ProcessAsync operation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ProcessAsync.</returns>
    public async Task<Result<ApplicationConfiguration>> ProcessAsync(GetConfigStationListQuery request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ApplicationConfiguration>.WithFailure("Operation was canceled.");
        }

        return await Result.Success(request)
            .ValidateNotNull(req => (req, nameof(request)))
            .ThenAsync(_ => repository.ListAsync(cancellationToken))
            .ThenMap(configStations =>
            {
                var config = new ApplicationConfiguration();
                // TODO: Map result.Value to config.WorkFlows, Machines, etc.
                return Result<ApplicationConfiguration>.Success(config);
            })
            .TapError(errors => logger.LogError("Failed to get ConfigStation list: {Errors}", string.Join(", ", errors)));
    }
}
