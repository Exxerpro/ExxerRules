// <copyright file="IBarCodeService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Services;

/// <summary>
/// Service interface for BarCode-related operations, replacing BarCodeRepositoryExtensions.
/// Enables proper dependency injection and testability.
/// </summary>
public interface IBarCodeService
{
    /// <summary>
    /// Gets the next consecutive BarCode ID by label, wrapping at 10000.
    /// </summary>
    /// <param name="partNumber">The part number to filter by.</param>
    /// <param name="masterLabel">The list of master labels.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The next consecutive BarCode ID, or a failure result if not found.</returns>
    Task<Result<int>> GetConsecutiveByBarCodeLabelAsync(
        string partNumber,
        List<string> masterLabel,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a BarCode entity by its label.
    /// </summary>
    /// <param name="label">The label to search for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The BarCode entity if found, or a failure result.</returns>
    Task<Result<BarCode>> GetBarCodeByLabelAsync(
        string label,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a BarCode entity by its unique identifier.
    /// </summary>
    /// <param name="barCodeId">The BarCode ID to search for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The BarCode entity if found, or a failure result.</returns>
    Task<Result<BarCode>> GetBarCodeByIdAsync(
        int barCodeId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a list of BarCode DTOs by searching register and cycle data for a label.
    /// </summary>
    /// <param name="label">The label to search for.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of BarCode DTOs matching the label, or a failure result.</returns>
    Task<Result<List<BarCodeDto>>> GetBarCodeByRegisterDataAsync(
        string label,
        CancellationToken cancellationToken);
}
