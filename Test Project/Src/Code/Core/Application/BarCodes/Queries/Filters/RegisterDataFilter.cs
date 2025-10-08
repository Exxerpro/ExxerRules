// <copyright file="RegisterDataFilter.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.BarCodes.DTO;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Interfaces;

namespace IndTrace.Application.BarCodes.Queries.Filters;

/// <summary>
/// Implementation of IRegisterDataFilter providing register-based barcode filtering.
/// Extracted from GetReportsListMonitorQueryHandler to eliminate filtering complexity from handlers.
/// Implements industrial safety patterns with Result&lt;T&gt;, defensive validation, and performance monitoring.
/// </summary>
public class RegisterDataFilter : IRegisterDataFilter
{
    private readonly IRepository<Register> _registerRepository;
    private readonly IRepository<Cycle> _cycleRepository;
    private readonly IRepository<BarCode> _barCodeRepository;
    private readonly ILogger<RegisterDataFilter> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterDataFilter"/> class.
    /// Follows CLAUDE.md null safety patterns with defensive validation.
    /// </summary>
    /// <param name="registerRepository">Repository for accessing register data.</param>
    /// <param name="cycleRepository">Repository for accessing cycle data.</param>
    /// <param name="barCodeRepository">Repository for accessing barcode data.</param>
    /// <param name="logger">Logger for recording operations and performance metrics.</param>
    public RegisterDataFilter(
        IRepository<Register> registerRepository,
        IRepository<Cycle> cycleRepository,
        IRepository<BarCode> barCodeRepository,
        ILogger<RegisterDataFilter> logger)
    {
        _registerRepository = registerRepository ?? throw new ArgumentNullException(nameof(registerRepository));
        _cycleRepository = cycleRepository ?? throw new ArgumentNullException(nameof(cycleRepository));
        _barCodeRepository = barCodeRepository ?? throw new ArgumentNullException(nameof(barCodeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets barcode IDs matching register search criteria with industrial safety patterns.
    /// Delegates to existing repository method and converts result to HashSet for efficient lookup.
    /// </summary>
    /// <param name="registerSearch">Register search criteria.</param>
    /// <param name="cancellationToken">Cancellation token for operation control.</param>
    /// <returns>Result containing matching barcode IDs or detailed failure information.</returns>
    public async Task<Result<HashSet<int>>> GetMatchingBarCodeIdsAsync(
        string registerSearch,
        CancellationToken cancellationToken)
    {
        // CLAUDE.md compliance: Early cancellation check for industrial safety
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<HashSet<int>>.WithFailure(["Operation was canceled."]);
        }

        // Defensive validation for null or empty search criteria
        if (string.IsNullOrWhiteSpace(registerSearch))
        {
            _logger.LogWarning("Register search criteria is null or empty");
            return Result<HashSet<int>>.Success(new HashSet<int>());
        }

        try
        {
            var sw = Stopwatch.StartNew();

            _logger.LogInformation("Starting register data filtering for search: {RegisterSearch}", registerSearch);

            // Use existing proven repository method for register-based barcode filtering
            var barCodesRegisterResult = await _registerRepository
                .GetBarCodeByRegisterDataAsync(_cycleRepository, _barCodeRepository, registerSearch, cancellationToken)
                .ConfigureAwait(false);

            if (barCodesRegisterResult.IsFailure)
            {
                _logger.LogError("Failed to get barcodes by register data: {Errors}",
                    string.Join(", ", barCodesRegisterResult.Errors ?? []));
                return Result<HashSet<int>>.WithFailure(barCodesRegisterResult.Errors);
            }

            // Convert to HashSet for efficient lookup during filtering
            var matchingBarCodes = barCodesRegisterResult.Value ?? new List<BarCodeDto>();
            var barCodeIds = new HashSet<int>(matchingBarCodes.Select(bc => bc.BarCodeId));

            sw.Stop();

            _logger.LogInformation(
                "Register data filtering completed: {MatchingCount} matching barcodes found in {ElapsedMs}ms for search: {RegisterSearch}",
                barCodeIds.Count, sw.ElapsedMilliseconds, registerSearch);

            if (!barCodeIds.Any())
            {
                _logger.LogInformation("No barcodes found matching register search criteria: {RegisterSearch}", registerSearch);
            }

            return Result<HashSet<int>>.Success(barCodeIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during register data filtering for search: {RegisterSearch}", registerSearch);
            return Result<HashSet<int>>.WithFailure([$"Register filtering failed: {ex.Message}"]);
        }
    }
}
