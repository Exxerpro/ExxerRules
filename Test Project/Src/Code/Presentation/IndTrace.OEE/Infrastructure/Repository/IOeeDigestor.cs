// <copyright file="IOeeDigestor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Infrastructure.Repository
{
    using IndTrace.Domain.Entities;
    using IndTrace.OEE.Domain.Models;
    using IndTrace.UI.Models.Performance;

    /// <summary>
    /// Defines the contract for digesting and storing OEE-related data.
    /// </summary>
    public interface IOeeDigestor
    {
        /// <summary>
        /// Writes performance data asynchronously.
        /// </summary>
        /// <param name="data">The performance data to write.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WriteAsync(PerformanceData data, CancellationToken ct = default);

        /// <summary>
        /// Writes KPI OEE data asynchronously.
        /// </summary>
        /// <param name="data">The KPI OEE data to write.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WriteAsync(KpiOee data, CancellationToken ct = default);

        /// <summary>
        /// Retrieves OEE results asynchronously.
        /// </summary>
        /// <param name="logger">The logger instance for recording operations.</param>
        /// <returns>A task representing the asynchronous operation that returns a list of OEE results.</returns>
        Task<List<OeeResult>> GetOeeResultsAsync(ILogger logger);
    }
}
