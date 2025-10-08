// <copyright file="UpdateShiftCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Shifts.Commands.Update;

using IndTrace.Application.Shifts.Queries.GetShftDetail;

/// <summary>
/// Represents the UpdateShiftCommandHandler.
/// </summary>
public class UpdateShiftCommandHandler : IMonitorRequestHandler<UpdateShiftCommand, ShiftDetailVm>
{
    private readonly IRepository<Shift> repository;
    private readonly ILogger<UpdateShiftCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateShiftCommandHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public UpdateShiftCommandHandler(IRepository<Shift> repository, ILogger<UpdateShiftCommandHandler> logger)
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
    public async Task<Result<ShiftDetailVm>> ProcessAsync(UpdateShiftCommand request, CancellationToken cancellationToken)
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
            var getResult = await this.repository.GetByIdAsync(request.ShiftId ?? 0, cancellationToken).ConfigureAwait(false);
            if (!getResult.IsSuccess || getResult.Value == null)
            {
                this.logger.LogError("Shift not found: {ShiftId}", request.ShiftId);
                return Result<ShiftDetailVm>.WithFailure($"ShiftId {request.ShiftId} does not exist");
            }

            var entity = getResult.Value;

            // Note: UpdateShiftCommand has properties that don't map to current Shift entity
            // Only updating what's available in current Shift entity
            var updateResult = await this.repository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
            if (!updateResult.IsSuccess)
            {
                this.logger.LogError("Failed to update Shift: {Errors}", string.Join(", ", updateResult.Errors ?? []));
                return Result<ShiftDetailVm>.WithFailure(updateResult.Errors);
            }

            var commitResult = await this.repository.CommitAsync(cancellationToken).ConfigureAwait(false);
            if (!commitResult.IsSuccess)
            {
                this.logger.LogError("Failed to commit Shift update: {Errors}", string.Join(", ", commitResult.Errors ?? []));
                return Result<ShiftDetailVm>.WithFailure(commitResult.Errors);
            }

            var dtoResult = ShiftDetailVm.ToDto(entity);
            if (!dtoResult.IsSuccess)
            {
                return Result<ShiftDetailVm>.WithFailure(dtoResult.Errors);
            }

            if (dtoResult.Value is null)
            {
                this.logger.LogError("DTO conversion returned null value");
                return Result<ShiftDetailVm>.WithFailure("DTO conversion returned null value");
            }

            return Result<ShiftDetailVm>.Success(dtoResult.Value);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception in UpdateShiftCommandHandler");
            return Result<ShiftDetailVm>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }
}
