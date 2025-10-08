// <copyright file="IRegisterDataFilter.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.Filters;

/// <summary>
/// Handles register data filtering as separate concern.
/// Extracted from GetReportsListMonitorQueryHandler to follow SRP principles.
/// Implements CLAUDE.md compliance with Result pattern and null safety.
/// </summary>
public interface IRegisterDataFilter
{
    /// <summary>
    /// Gets barcode IDs matching register search criteria.
    /// Uses the existing repository method for register-based barcode filtering.
    /// </summary>
    /// <param name="registerSearch">Register search criteria.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing matching barcode IDs or failure reasons.</returns>
    Task<Result<HashSet<int>>> GetMatchingBarCodeIdsAsync(
        string registerSearch,
        CancellationToken cancellationToken);
}
