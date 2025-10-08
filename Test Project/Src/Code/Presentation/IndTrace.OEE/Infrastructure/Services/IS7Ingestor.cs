// <copyright file="IS7Ingestor.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.OEE.Infrastructure.Services
{
    using IndTrace.DataStore.ModelsComs;
    using IndTrace.Domain.Entities;

    /// <summary>
    /// Defines the contract for ingesting data from S7 PLCs.
    /// </summary>
    public interface IS7Ingestor
    {
        /// <summary>
        /// Fetches performance data from the specified PLC asynchronously.
        /// </summary>
        /// <param name="virtualPlcLC data configuration.
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns></param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation that returns performance data.</returns>
        public Task<PerformanceData> FetchPerformanceDataAsync(PlcData plc, CancellationToken cancellationToken);
    }
}
