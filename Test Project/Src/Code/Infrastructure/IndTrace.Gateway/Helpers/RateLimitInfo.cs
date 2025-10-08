// <copyright file="RateLimitInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Helpers;

/// <summary>
/// Represents rate limiting information for gateway operations, including semaphore, last execution time, and interval.
/// </summary>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate rate limit info logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
/// <summary>
/// Represents the RateLimitInfo.
/// </summary>
public class RateLimitInfo
{
    /// <summary>
    /// Semaphore used to control concurrent access.
    /// </summary>
    public SemaphoreSlim Semaphore = new(1, 1);

    /// <summary>
    /// The last time the operation was executed.
    /// </summary>
    public DateTime LastExecutionTime = DateTime.MinValue;

    /// <summary>
    /// The controller identifier associated with this rate limit.
    /// </summary>
    public int Controller;

    /// <summary>
    /// The last result of the gateway operation.
    /// </summary>
    public Result<TaskGatewayResponse>? LastResult;

    /// <summary>
    /// The interval to enforce between allowed executions.
    /// </summary>
    public TimeSpan RateLimitInterval = TimeSpan.FromSeconds(0.750); // Default rate limit interval
}
