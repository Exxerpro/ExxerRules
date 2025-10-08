// <copyright file="CreateSettingCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Settings.Commands.Create;

/// <summary>
/// Handles the creation of new system configuration settings.
/// Settings control various aspects of system behavior and operational parameters.
/// </summary>
public class CreateSettingCommandHandler : IMonitorRequestHandler<CreateSettingCommand, SettingCreatedEvent>
{
    private readonly IRepository<Domain.Entities.Setting> repository;
    private readonly ILogger<CreateSettingCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSettingCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Repository for accessing setting data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public CreateSettingCommandHandler(IRepository<Domain.Entities.Setting> repository, ILogger<CreateSettingCommandHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    /// <summary>
    /// Processes the setting creation command.
    /// </summary>
    /// <param name="request">The command containing setting data to create.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A result containing the created setting notification.</returns>
    public async Task<Result<SettingCreatedEvent>> ProcessAsync(CreateSettingCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Setting
        {
            SettingId = request.SettingId,
            MachineId = request.MachineId,
            Config = request.Setting,
        };

        var addResult = await this.repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        if (!addResult.IsSuccess)
        {
            this.logger.LogError("Failed to add Setting: {Errors}", string.Join(", ", addResult.Errors ?? []));
            return Result<SettingCreatedEvent>.WithFailure(addResult.Errors);
        }

        var commitResult = await this.repository.CommitAsync(cancellationToken).ConfigureAwait(false);
        if (!commitResult.IsSuccess)
        {
            this.logger.LogError("Failed to commit Setting creation: {Errors}", string.Join(", ", commitResult.Errors ?? []));
            return Result<SettingCreatedEvent>.WithFailure(commitResult.Errors);
        }

        var response = new SettingCreatedEvent
        {
            SettingId = entity.SettingId,
            MachineId = entity.MachineId,
            Setting = entity.Config,
        };

        return Result<SettingCreatedEvent>.Success(response);
    }
}
