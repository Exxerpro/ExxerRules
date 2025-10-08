// <copyright file="GetShiftDetailQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Shifts.Queries.GetShftDetail;

/// <summary>
/// Represents the GetShiftDetailQueryHandler.
/// </summary>
public class GetShiftDetailQueryHandler : IMonitorRequestHandler<GetShiftDetailQuery, ShiftDetailVm>
{
    private readonly IRepository<Shift> repository;
    private readonly ILogger<GetShiftDetailQueryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetShiftDetailQueryHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public GetShiftDetailQueryHandler(IRepository<Shift> repository, ILogger<GetShiftDetailQueryHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    /// <summary>
    /// Executes ProcessAsync operation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ProcessAsync.</returns>
    public async Task<Result<ShiftDetailVm>> ProcessAsync(GetShiftDetailQuery request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<ShiftDetailVm>.WithFailure("request cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ShiftDetailVm>.WithFailure("Operation was canceled.");
        }

        try
        {
            if (request.ShiftId <= 0)
            {
                this.logger.LogError("Invalid ShiftId: {ShiftId}", request.ShiftId);
                return Result<ShiftDetailVm>.WithFailure("ShiftId must be greater than 0");
            }

            var getAllResult = await this.repository.ListAsync(cancellationToken).ConfigureAwait(false);
            if (!getAllResult.IsSuccess)
            {
                this.logger.LogError("Failed to retrieve Shifts: {Errors}", string.Join(", ", getAllResult.Errors ?? []));
                return Result<ShiftDetailVm>.WithFailure(getAllResult.Errors);
            }

            var shift = getAllResult.Value?.FirstOrDefault(p => p.ShiftId == request.ShiftId);
            if (shift == null)
            {
                this.logger.LogError("Shift not found: {ShiftId}", request.ShiftId);
                return Result<ShiftDetailVm>.WithFailure($"Shift not found {request.ShiftId}");
            }

            return ShiftDetailVm.ToDto(shift);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception in GetShiftDetailQueryHandler");
            return Result<ShiftDetailVm>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }
}
