// <copyright file="GetSettingDetailQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Settings.Queries.GetSettingDetail;

/// <summary>
/// Represents the GetSettingDetailQueryHandler.
/// </summary>
public class GetSettingDetailQueryHandler : IMonitorRequestHandler<GetSettingDetailQuery, SettingDetailVm>
{
    private readonly IRepository<Domain.Entities.Setting> repository;
    private readonly ILogger<GetSettingDetailQueryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSettingDetailQueryHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public GetSettingDetailQueryHandler(IRepository<Domain.Entities.Setting> repository, ILogger<GetSettingDetailQueryHandler> logger)
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
    public async Task<Result<SettingDetailVm>> ProcessAsync(GetSettingDetailQuery request, CancellationToken cancellationToken)
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
            var getResult = await this.repository.GetByIdAsync(request.SettingId, cancellationToken).ConfigureAwait(false);
            if (!getResult.IsSuccess || getResult.Value == null)
            {
                this.logger.LogError("Setting not found: {SettingId}", request.SettingId);
                return Result<SettingDetailVm>.WithFailure($"Setting with ID {request.SettingId} not found");
            }

            var dtoResult = SettingDetailVm.ToDto(getResult.Value);
            if (!dtoResult.IsSuccess)
            {
                this.logger.LogError("Failed to convert setting to DTO: {SettingId}", request.SettingId);
                return Result<SettingDetailVm>.WithFailure(dtoResult.Error ?? "Failed to convert setting to DTO");
            }

            return dtoResult.Value is not null
                ? Result<SettingDetailVm>.Success(dtoResult.Value)
                : Result<SettingDetailVm>.WithFailure(["DTO value is null"]);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception in GetSettingDetailQueryHandler");
            return Result<SettingDetailVm>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }
}
