// <copyright file="GetAppDetailsGuiRequestHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Configuration.Services;

using IndTrace.Application.Models.CacheServices;

/// <summary>
/// Represents the GetAppDetailsMonitorRequestHandler.
/// </summary>
public class GetAppDetailsMonitorRequestHandler(
    CacheManager<ApplicationConfiguration> cacheManager,
    AppDetailsFactory appDetailsFactory,
    ILogger<GetAppDetailsMonitorRequestHandler> logger)
    : IMonitorRequestHandler<GetAppDetailsMonitorRequest, ApplicationConfiguration>
{
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate request handler logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

    /// <inheritdoc/>
    public async Task<Result<ApplicationConfiguration>> ProcessAsync(GetAppDetailsMonitorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var appDetails = await cacheManager.
                GetOrRefreshAsync(
                    () => appDetailsFactory.CreateAppDetailsAsync(cancellationToken),
                    request.Refresh, cancellationToken, logger);

            if (appDetails == null)
            {
                return Result<ApplicationConfiguration>.WithFailure("Failed to load AppDetails.");
            }

            return Result<ApplicationConfiguration>.Success(appDetails);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching AppDetails.");
            return Result<ApplicationConfiguration>.WithFailure("An error occurred while fetching AppDetails.");
        }
    }
}
