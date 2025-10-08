// <copyright file="CycleCreator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.Repositories;

namespace IndTrace.Application.Cycles.Services;

/// <summary>
/// Creates and persists new cycles with proper timestamps.
/// Based on CreateCyclesCommandHandler cycle creation logic.
/// Implements CLAUDE.md compliance: Result pattern, cancellation support, defensive validation.
/// </summary>
public class CycleCreator : ICycleCreator
{
    private readonly IRepository<Cycle> _cycleRepository;
    private readonly ILogger<CycleCreator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CycleCreator"/> class.
    /// </summary>
    /// <param name="cycleRepository">Repository for cycle persistence.</param>
    /// <param name="logger">Logger for recording cycle creation operations.</param>
    public CycleCreator(
        IRepository<Cycle> cycleRepository,
        ILogger<CycleCreator> logger)
    {
        _cycleRepository = cycleRepository ?? throw new ArgumentNullException(nameof(cycleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates and persists a new cycle with deterministic timestamps.
    /// </summary>
    /// <param name="request">Request containing cycle creation parameters.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing the created cycle or failure reasons.</returns>
    public async Task<Result<Cycle>> CreateAsync(
        CycleCreateRequest request,
        CancellationToken cancellationToken)
    {
        // CLAUDE.md compliance: early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<Cycle>.WithFailure(["Operation was canceled."]);
        }

        // CLAUDE.md compliance: defensive validation
        if (request is null)
        {
            _logger.LogError("CycleCreateRequest cannot be null");
            return Result<Cycle>.WithFailure(["Request cannot be null."]);
        }

        try
        {
            _logger.LogInformation(
                "Creating cycle for BarCodeId {BarCodeId} at MachineId {MachineId}",
                request.BarCodeId, request.MachineId);

            var cycle = new Cycle
            {
                MachineId = request.MachineId,
                BarCodeId = request.BarCodeId,
                CycleStatus = request.CycleStatus.Value,
                PartStatus = request.PartStatus.Value,
                CycleTime = 0, // Default values from original implementation
                TaktTime = 0,  // Default values from original implementation
                StartedOn = request.StartedOn.ToLocalTime().DateTime,
                FinishedOn = request.FinishedOn.ToLocalTime().DateTime,
            };

            await _cycleRepository.AddAsync(cycle, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation(
                "Successfully created cycle {CycleId} for BarCodeId {BarCodeId} at MachineId {MachineId}",
                cycle.CycleId, cycle.BarCodeId, cycle.MachineId);

            return Result<Cycle>.Success(cycle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to create cycle for BarCodeId {BarCodeId} at MachineId {MachineId}",
                request.BarCodeId, request.MachineId);
            return Result<Cycle>.WithFailure([$"Failed to create cycle: {ex.Message}"]);
        }
    }
}
