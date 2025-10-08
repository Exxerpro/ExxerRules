// <copyright file="GetBarCodesLabelHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeLabel;

using IndQuestResults.Operations;

/// <summary>
/// Represents the GetBarCodesLabelHandler.
/// </summary>
public class GetBarCodesLabelHandler : IMonitorRequestHandler<GetBarCodesLabelQuery, BarCodesListVm>
{
    private readonly IRepository<BarCode> repository;
    private readonly IMonitorRequestDispatcher dispatcher;
    private readonly IDateTimeMachine dateTimeMachine;
    private readonly ILogger<GetBarCodesLabelHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBarCodesLabelHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public GetBarCodesLabelHandler(
        IRepository<BarCode> repository,
        IMonitorRequestDispatcher dispatcher,
        IDateTimeMachine dateTimeMachine,
        ILogger<GetBarCodesLabelHandler> logger)
    {
        this.repository = repository;
        this.dispatcher = dispatcher;
        this.dateTimeMachine = dateTimeMachine;
        this.logger = logger;
    }

    /// <summary>
    /// Executes ProcessAsync operation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ProcessAsync.</returns>
    public async Task<Result<BarCodesListVm>> ProcessAsync(GetBarCodesLabelQuery request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<BarCodesListVm>.WithFailure("Operation was canceled.");
        }

        return await Result.Success(request)
            .ValidateNotNull(req => (req, nameof(req)))
            .ThenAsync(_ => repository.ListAsync(cancellationToken))
            .ThenMap(barcodes =>
            {
                var filtered = barcodes.Where(e => e.Label == request.Label).ToList();
                return BarCodeDto.ToDtoList(filtered);
            })
            .ThenMap(dtos =>
            {
                var vm = new BarCodesListVm
                {
                    BarCodes = dtos.ToList(),
                    Count = dtos.Count(),
                };
                return Result.Success(vm);
            })
            .TapError(errors => logger.LogError("Failed to retrieve BarCodes: {Errors}", string.Join(", ", errors)));
    }
}
