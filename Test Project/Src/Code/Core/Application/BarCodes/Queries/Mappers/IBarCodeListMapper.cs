// <copyright file="IBarCodeListMapper.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.BarCodes.DTO;
using IndTrace.Domain.Entities;

namespace IndTrace.Application.BarCodes.Queries.Mappers;

/// <summary>
/// Maps IQueryable&lt;BarCode&gt; to enriched List&lt;BarCodeDto&gt; with cycle counts.
/// Extracted from GetReportsListMonitorQueryHandler to follow SRP principles.
/// Implements CLAUDE.md compliance with Result pattern and null safety.
/// </summary>
public interface IBarCodeListMapper
{
    /// <summary>
    /// Maps barcode query to DTOs with cycle count enrichment.
    /// Executes the query, transforms to DTOs, and enriches with cycle count data.
    /// </summary>
    /// <param name="query">Filtered barcode query ready for execution.</param>
    /// <param name="cycleQuery">Cycle queryable for count enrichment.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing mapped DTOs with cycle counts or failure reasons.</returns>
    Task<Result<List<BarCodeDto>>> MapWithCycleCountsAsync(
        IQueryable<BarCode> query,
        IQueryable<Cycle> cycleQuery,
        CancellationToken cancellationToken);
}
