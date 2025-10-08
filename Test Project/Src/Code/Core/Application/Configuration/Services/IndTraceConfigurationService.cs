// <copyright file="IndTraceConfigurationService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Configuration.Services;

/// <summary>
/// Provides methods to retrieve and manage the IndTrace application configuration using dependency injection and mediator pattern.
/// </summary>
public class IndTraceConfigurationService(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    private ApplicationConfiguration? applicationConfiguration;

    /// <summary>
    /// Asynchronously retrieves the application configuration, optionally forcing a refresh.
    /// </summary>
    /// <param name="refresh">If true, forces a refresh of the configuration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result containing the application configuration.</returns>
    public async Task<Result<ApplicationConfiguration>> GetConfigurationAsync(bool refresh = false, CancellationToken cancellationToken = default)
    {
        if (this.applicationConfiguration is not null && !refresh)
        {
            return Result<ApplicationConfiguration>.Success(this.applicationConfiguration);
        }

        var request = new GetAppDetailsMonitorRequest(refresh);

        using var scope = this.serviceProvider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMonitorRequestDispatcher>();

        var result = await mediator.ProcessAsync(request, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return result;
        }

        this.applicationConfiguration = result.Value;

        return Result<ApplicationConfiguration>.Success(this.applicationConfiguration);
    }
}

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate configuration service logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
