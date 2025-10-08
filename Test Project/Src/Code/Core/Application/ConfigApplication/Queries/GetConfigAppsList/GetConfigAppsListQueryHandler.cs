// <copyright file="GetConfigAppsListQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigApplication.Queries.GetConfigAppsList;

/// <summary>
/// Represents the GetConfigAppsListQueryHandler.
/// </summary>
public class GetConfigAppsListQueryHandler : IMonitorRequestHandler<GetConfigAppsListQuery, ConfigAppsListVm>
{
    private readonly IRepository<Domain.Entities.ConfigApp> repository;
    private readonly ILogger<GetConfigAppsListQueryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetConfigAppsListQueryHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public GetConfigAppsListQueryHandler(IRepository<Domain.Entities.ConfigApp> repository, ILogger<GetConfigAppsListQueryHandler> logger)
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
    public async Task<Result<ConfigAppsListVm>> ProcessAsync(GetConfigAppsListQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<ConfigAppsListVm>.WithFailure("request cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ConfigAppsListVm>.WithFailure("Operation was canceled.");
        }

        try
        {
            var result = await this.repository.ListAsync(cancellationToken).ConfigureAwait(false);
            if (result.IsFailure)
            {
                this.logger.LogError("Failed to get ConfigApp list: {Errors}", string.Join(", ", result.Errors ?? []));
                return Result<ConfigAppsListVm>.WithFailure(result.Errors);
            }

            var listResult = ConfigAppsDto.ToDtoList(result.Value ?? []);
            if (listResult.IsFailure || listResult.Value is null)
            {
                return Result<ConfigAppsListVm>.WithFailure(listResult.Errors);
            }

            var listVm = listResult.Value.ToList();

            var vm = new ConfigAppsListVm
            {
                ConfigApp = listVm,
                Count = listVm.Count,
            };

            return Result<ConfigAppsListVm>.Success(vm);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception in GetConfigAppsListQueryHandler");
            return Result<ConfigAppsListVm>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }
}
