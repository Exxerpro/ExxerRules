// <copyright file="GetSettingsListQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Settings.Queries.GetSettingsList;

using IndTrace.Application.Settings.DTO;

/// <summary>
/// Represents the GetSettingsListQueryHandler.
/// </summary>
public class GetSettingsListQueryHandler : IMonitorRequestHandler<GetSettingsListQuery, SettingsListVm>
{
    private readonly IRepository<Domain.Entities.Setting> repository;
    private readonly ILogger<GetSettingsListQueryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSettingsListQueryHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public GetSettingsListQueryHandler(IRepository<Domain.Entities.Setting> repository, ILogger<GetSettingsListQueryHandler> logger)
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
    public async Task<Result<SettingsListVm>> ProcessAsync(GetSettingsListQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<SettingsListVm>.WithFailure("request cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<SettingsListVm>.WithFailure("Operation was canceled.");
        }

        try
        {
            var getAllResult = await this.repository.ListAsync(cancellationToken).ConfigureAwait(false);
            if (!getAllResult.IsSuccess)
            {
                this.logger.LogError("Failed to retrieve Settings: {Errors}", string.Join(", ", getAllResult.Errors ?? []));
                return Result<SettingsListVm>.WithFailure(getAllResult.Errors);
            }

            var settingsEntities = getAllResult.Value ?? [];
            var settingsRes = SettingDto.ToDtoList(settingsEntities);
            if (settingsRes.IsFailure || settingsRes.Value is null)
            {
                return Result<SettingsListVm>.WithFailure(settingsRes.Errors);
            }

            var settings = settingsRes.Value.ToList();

            var vm = new SettingsListVm
            {
                Settings = settings,
                Count = settings.Count,
            };

            return Result<SettingsListVm>.Success(vm);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception in GetSettingsListQueryHandler");
            return Result<SettingsListVm>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }
}
