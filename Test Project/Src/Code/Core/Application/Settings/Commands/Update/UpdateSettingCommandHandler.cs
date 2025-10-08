// <copyright file="UpdateSettingCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Settings.Commands.Update;

using IndTrace.Application.Settings.Queries.GetSettingDetail;

/// <summary>
/// Represents the UpdateSettingCommandHandler.
/// </summary>
public class UpdateSettingCommandHandler : IMonitorRequestHandler<UpdateSettingCommand, SettingDetailVm>
{
    private readonly IRepository<Domain.Entities.Setting> repository;
    private readonly ILogger<UpdateSettingCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSettingCommandHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public UpdateSettingCommandHandler(IRepository<Domain.Entities.Setting> repository, ILogger<UpdateSettingCommandHandler> logger)
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
    public async Task<Result<SettingDetailVm>> ProcessAsync(UpdateSettingCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<SettingDetailVm>.WithFailure("request cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<SettingDetailVm>.WithFailure("Operation was canceled.");
        }

        try
        {
            if (string.IsNullOrEmpty(request.Config))
            {
                this.logger.LogError("Config must have value");
                return Result<SettingDetailVm>.WithFailure("Config must have value");
            }

            var getResult = await this.repository.GetByIdAsync(request.SettingId ?? 0, cancellationToken).ConfigureAwait(false);
            if (!getResult.IsSuccess || getResult.Value == null)
            {
                this.logger.LogError("Setting not found: {SettingId}", request.SettingId);
                return Result<SettingDetailVm>.WithFailure($"SettingId {request.SettingId} does not exist");
            }

            var entity = getResult.Value;
            entity.Config = request.Config;

            var updateResult = await this.repository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
            if (!updateResult.IsSuccess)
            {
                this.logger.LogError("Failed to update Setting: {Errors}", string.Join(", ", updateResult.Errors ?? []));
                return Result<SettingDetailVm>.WithFailure(updateResult.Errors);
            }

            var commitResult = await this.repository.CommitAsync(cancellationToken).ConfigureAwait(false);
            if (!commitResult.IsSuccess)
            {
                this.logger.LogError("Failed to commit Setting update: {Errors}", string.Join(", ", commitResult.Errors ?? []));
                return Result<SettingDetailVm>.WithFailure(commitResult.Errors);
            }

            var dtoResult = SettingDetailVm.ToDto(entity);
            if (dtoResult.IsFailure)
            {
                this.logger.LogError("Failed to convert Setting to DTO: {Errors}", string.Join(", ", dtoResult.Errors ?? []));
                return Result<SettingDetailVm>.WithFailure(dtoResult.Errors);
            }

            return dtoResult.Value is not null
                ? Result<SettingDetailVm>.Success(dtoResult.Value)
                : Result<SettingDetailVm>.WithFailure(["DTO value is null"]);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception in UpdateSettingCommandHandler");
            return Result<SettingDetailVm>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }
}
