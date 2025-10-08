// <copyright file="GetMachinePLCDetailQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.MachinesPlcs.Queries.GetDetail;

/// <summary>
/// Handles the retrieval of detailed machine-PLC relationship information.
/// Provides comprehensive data about specific machine-PLC associations and their configurations.
/// </summary>
public class GetMachinePlcDetailQueryHandler : IMonitorRequestHandler<GetMachinePlcDetailQuery, MachinePlcDetailVm>
{
    private readonly IRepository<MachinePlc> repository;
    private readonly ILogger<GetMachinePlcDetailQueryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMachinePlcDetailQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">Repository for accessing machine-PLC relationship data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public GetMachinePlcDetailQueryHandler(IRepository<MachinePlc> repository, ILogger<GetMachinePlcDetailQueryHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    /// <summary>
    /// Processes the machine-PLC detail query and returns comprehensive relationship information.
    /// </summary>
    /// <param name="request">The query containing the machine and PLC IDs to retrieve details for.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A result containing the detailed machine-PLC relationship view model.</returns>
    public async Task<Result<MachinePlcDetailVm>> ProcessAsync(GetMachinePlcDetailQuery request, CancellationToken cancellationToken)
    {
        var getAllResult = await this.repository.ListAsync(cancellationToken);
        if (!getAllResult.IsSuccess)
        {
            this.logger.LogError("Failed to retrieve MachinePlcs: {Errors}", string.Join(", ", getAllResult.Errors ?? []));
            return Result<MachinePlcDetailVm>.WithFailure(getAllResult.Errors);
        }

        var machinePlc = getAllResult.Value?.FirstOrDefault(p => p.PlcId == request.PlcId && p.MachineId == request.MachineId);
        if (machinePlc == null)
        {
            this.logger.LogError("MachinePlc not found: PlcId={PlcId}, MachineId={MachineId}", request.PlcId, request.MachineId);
            return Result<MachinePlcDetailVm>.WithFailure($"MachinePlc with PlcId {request.PlcId} and MachineId {request.MachineId} not found");
        }

        var vm = MachinePlcDetailVm.ToDto(machinePlc);
        if (!vm.IsSuccess)
        {
            return Result<MachinePlcDetailVm>.WithFailure(vm.Errors);
        }

        if (vm.Value is null)
        {
            return Result<MachinePlcDetailVm>.WithFailure("DTO conversion returned null value");
        }

        return Result<MachinePlcDetailVm>.Success(vm.Value);
    }
}
