// <copyright file="IndTraceConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Configuration.Services;

/// <summary>
/// Provides configuration management for IndTrace, including caching and update tracking.
/// </summary>
public class IndTraceConfiguration
{
    /// <summary>
    /// Gets the last update time for the configuration.
    /// </summary>
    public static DateTime LastUpdateTime => lastUpdateTime;

    private static DateTime lastUpdateTime = DateTime.MinValue;

    private DateTime? cacheTimestamp;
    private readonly TimeSpan cacheDuration = TimeSpan.FromMinutes(60); // Set the cache timeout to 15 minutes
    private ApplicationConfiguration? appDetails;

    /// <summary>
    /// Sets the last update time for the configuration.
    /// </summary>
    /// <param name="timeExecuted">The time the update was executed.</param>
    public void SetUpdateTime(DateTime timeExecuted)
    {
        lastUpdateTime = timeExecuted;
    }

    /// <summary>
    /// Gets the cached application configuration details if the cache is valid; otherwise, null.
    /// </summary>
    public ApplicationConfiguration? AppDetails
    {
        get
        {
            if (this.cacheTimestamp.HasValue && (DateTime.UtcNow - this.cacheTimestamp.Value) < this.cacheDuration)
            {
                return this.appDetails;
            }

            // Cache has expired
            return null;
        }
    }

    /// <summary>
    /// Sets the cached application configuration details.
    /// </summary>
    /// <param name="details">The application configuration details to cache.</param>
    public void SetConfigDetails(ApplicationConfiguration details)
    {
        this.appDetails = details;
    }

    /// <summary>
    /// Gets a value indicating whether the cache contains valid data.
    /// </summary>
    public bool HasValidData => this.appDetails is not null;

    /// <summary>
    /// Invalidates the cached configuration details.
    /// </summary>
    public void InvalidateCache()
    {
        this.appDetails = null;
        this.cacheTimestamp = null;
    }

    /// <summary>
    /// Forces a refresh of the cached configuration details.
    /// </summary>
    /// <param name="details">The new application configuration details.</param>
    public void RefreshCache(ApplicationConfiguration details)
    {
        this.SetConfigDetails(details); // Simply calls the SetConfigDetails method
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate configuration logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
