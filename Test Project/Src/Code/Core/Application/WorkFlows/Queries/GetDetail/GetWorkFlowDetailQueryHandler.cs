// <copyright file="GetWorkFlowDetailQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.WorkFlows.Queries.GetDetail;

/// <summary>
/// Represents the GetWorkFlowDetailQueryHandler.
/// </summary>
public class GetWorkFlowDetailQueryHandler : IMonitorRequestHandler<GetWorkFlowDetailQuery, List<WorkFlowDetailVm>>
{
    private readonly IRepository<Product> productRepository;
    private readonly IRepository<WorkFlow> workFlowRepository;
    private readonly ILogger<GetWorkFlowDetailQueryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWorkFlowDetailQueryHandler"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public GetWorkFlowDetailQueryHandler(
        IRepository<Product> productRepository,
        IRepository<WorkFlow> workFlowRepository,
        ILogger<GetWorkFlowDetailQueryHandler> logger)
    {
        this.productRepository = productRepository;
        this.workFlowRepository = workFlowRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Executes ProcessAsync operation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ProcessAsync.</returns>
    public async Task<Result<List<WorkFlowDetailVm>>> ProcessAsync(GetWorkFlowDetailQuery request, CancellationToken cancellationToken)
    {
        // First get the product by part number
        var getAllProductsResult = await this.productRepository.ListAsync(cancellationToken).ConfigureAwait(false);
        if (!getAllProductsResult.IsSuccess)
        {
            this.logger.LogError("Failed to retrieve Products: {Errors}", string.Join(", ", getAllProductsResult.Errors ?? []));
            return Result<List<WorkFlowDetailVm>>.WithFailure(getAllProductsResult.Errors);
        }

        var product = getAllProductsResult.Value?.FirstOrDefault(m => m.PartNumber == request.NoParte);
        if (product == null)
        {
            this.logger.LogError("Product not found with PartNumber: {PartNumber}", request.NoParte);
            return Result<List<WorkFlowDetailVm>>.WithFailure($"Product with PartNumber {request.NoParte} not found");
        }

        // Get WorkFlows for the product
        var getAllWorkFlowsResult = await this.workFlowRepository.ListAsync(cancellationToken).ConfigureAwait(false);
        if (!getAllWorkFlowsResult.IsSuccess)
        {
            this.logger.LogError("Failed to retrieve WorkFlows: {Errors}", string.Join(", ", getAllWorkFlowsResult.Errors ?? []));
            return Result<List<WorkFlowDetailVm>>.WithFailure(getAllWorkFlowsResult.Errors);
        }

        var workFlows = getAllWorkFlowsResult.Value?.Where(f => f.ProductId == product.ProductId).ToList() ?? [];
        var vmResult = WorkFlowDetailVm.ToDtoList(workFlows);
        if (vmResult.IsFailure || vmResult.Value is null)
        {
            return Result<List<WorkFlowDetailVm>>.WithFailure(vmResult.Errors);
        }

        return Result<List<WorkFlowDetailVm>>.Success(vmResult.Value.ToList());
    }
}
