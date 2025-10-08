// <copyright file="CreateConfigAppCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigApplication.Commands.Create;

using IndQuestResults.Operations;

/// <summary>
/// Handles the creation of new application configuration entities in the system.
/// </summary>
public class CreateConfigAppCommandHandler : IMonitorRequestHandler<CreateConfigAppCommand, ConfigAppCreated>
{
    private readonly IRepository<Domain.Entities.ConfigApp> repository;
    private readonly ILogger<CreateConfigAppCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateConfigAppCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Repository for accessing application configuration data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public CreateConfigAppCommandHandler(IRepository<Domain.Entities.ConfigApp> repository, ILogger<CreateConfigAppCommandHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    /// <summary>
    /// Processes the application configuration creation command.
    /// </summary>
    /// <param name="request">The command containing application configuration data to create.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A result containing the created application configuration notification.</returns>
    public async Task<Result<ConfigAppCreated>> ProcessAsync(CreateConfigAppCommand request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ConfigAppCreated>.WithFailure("Operation was canceled.");
        }

        return await Result.Success(request)
            .ValidateNotNull(req => (req, nameof(req)))
            .ThenMap(req => new Domain.Entities.ConfigApp
            {
                ConfigAppId = req.ConfigId,
                MachineId = req.MachineId,
                Client = req.Client,
                Factory = req.Factorie,
                Machine = req.Machine,
                Line = req.Line,
                Pc = req.Pc,
                Project = req.Project,
                Version = req.Version,
            })
            .ThenAsync(entity => repository.AddAsync(entity, cancellationToken).Then(_ => Result.Success(entity)))
            .ThenAsync(entity => repository.CommitAsync(cancellationToken).Then(_ => Result.Success(entity)))
            .ThenMap(entity => new ConfigAppCreated
            {
                ConfigId = entity.ConfigAppId,
                AppId = entity.AppId,
                MachineId = entity.MachineId,
                Client = entity.Client,
                Factory = entity.Factory,
                Line = entity.Line,
                Project = entity.Project,
                Version = entity.Version,
            })
            .TapError(errors => logger.LogError("Failed to create ConfigApp: {Errors}", string.Join(", ", errors)));
    }
}
