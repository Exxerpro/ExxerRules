// <copyright file="UpdateConfigAppCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.ConfigApplication.Commands.Update;

using IndQuestResults.Operations;

/// <summary>
/// Represents the UpdateConfigAppCommandHandler.
/// </summary>
public class UpdateConfigAppCommandHandler : IMonitorRequestHandler<UpdateConfigAppCommand, ConfigAppDto>
{
    private readonly IRepository<Domain.Entities.ConfigApp> repository;
    private readonly ILogger<UpdateConfigAppCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateConfigAppCommandHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public UpdateConfigAppCommandHandler(IRepository<Domain.Entities.ConfigApp> repository, ILogger<UpdateConfigAppCommandHandler> logger)
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
    public async Task<Result<ConfigAppDto>> ProcessAsync(UpdateConfigAppCommand request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ConfigAppDto>.WithFailure("Operation was canceled.");
        }

        return await Result.Success(request)
            .ValidateNotNull(req => (req, nameof(req)))
            .ThenAsync(req => repository.GetByIdAsync(req.AppId ?? 0, cancellationToken).ToResult("ConfigApp not found"))
            .ThenTap(entity =>
            {
                entity.AppId = request.AppId ?? entity.AppId;
                entity.Client = request.Client ?? entity.Client;
                entity.Factory = request.Factory ?? entity.Factory;
                entity.Line = request.Line ?? entity.Line;
                entity.Project = request.Project ?? entity.Project;
                entity.Version = request.Version ?? entity.Version;
            })
            .ThenAsync(entity => repository.UpdateAsync(entity, cancellationToken).Then(_ => Result.Success(entity)))
            .ThenAsync(entity => repository.CommitAsync(cancellationToken).Then(_ => Result.Success(entity)))
            .Then(ConfigAppDto.ToDto)
            .TapError(errors => logger.LogError("Failed to update ConfigApp: {Errors}", string.Join(", ", errors)));
    }
}
