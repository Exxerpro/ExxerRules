// <copyright file="UpdateProductCommandHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Products.Commands.Update;

/// <summary>
/// Handles the updating of existing product entities in the system.
/// Products represent manufacturing items that are produced and tracked through the industrial process.
/// </summary>
public class UpdateProductCommandHandler : IMonitorRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IRepository<Product> repository;
    private readonly IMonitorRequestDispatcher monitorRequestDispatcher;
    private readonly ILogger<UpdateProductCommandHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProductCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Repository for accessing product data.</param>
    /// <param name="monitorRequestDispatcher">Command dispatcher for executing related operations.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public UpdateProductCommandHandler(
        IRepository<Product> repository,
        IMonitorRequestDispatcher monitorRequestDispatcher,
        ILogger<UpdateProductCommandHandler> logger)
    {
        this.repository = repository;
        this.monitorRequestDispatcher = monitorRequestDispatcher;
        this.logger = logger;
    }

    /// <summary>
    /// Processes the product update command.
    /// </summary>
    /// <param name="request">The command containing updated product data.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A result containing the updated product data transfer object.</returns>
    public async Task<Result<ProductDto>> ProcessAsync(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Result<ProductDto>.WithFailure("request cannot be null.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ProductDto>.WithFailure("Operation was canceled.");
        }

        try
        {
            var getResult = await this.repository.GetByIdAsync(request.ProductId ?? 0, cancellationToken).ConfigureAwait(false);
            if (!getResult.IsSuccess || getResult.Value == null)
            {
                this.logger.LogError("Product not found: {ProductId}", request.ProductId);
                return Result<ProductDto>.WithFailure($"ProductId {request.ProductId} does not exist please provide a valid ProductId");
            }

            var product = getResult.Value;
            product.AliasPartNumber = request.AliasNoParte ?? product.AliasPartNumber;
            product.CustomerPartNumber = request.CustomerPartNumber ?? product.CustomerPartNumber;
            product.Description = request.Description ?? product.Description;
            product.IsActive = request.IsActive ?? product.IsActive;
            product.PartNumber = request.NoParte ?? product.PartNumber;
            product.ProductId = request.ProductId ?? product.ProductId;
            product.ProductName = request.ProductName ?? product.ProductName;
            product.Version = request.Version ?? product.Version;

            var updateResult = await this.repository.UpdateAsync(product, cancellationToken).ConfigureAwait(false);
            if (!updateResult.IsSuccess)
            {
                this.logger.LogError("Failed to update Product: {Errors}", string.Join(", ", updateResult.Errors ?? []));
                return Result<ProductDto>.WithFailure(updateResult.Errors);
            }

            var commitResult = await this.repository.CommitAsync(cancellationToken).ConfigureAwait(false);
            if (!commitResult.IsSuccess)
            {
                this.logger.LogError("Failed to commit Product update: {Errors}", string.Join(", ", commitResult.Errors ?? []));
                return Result<ProductDto>.WithFailure(commitResult.Errors);
            }

            var result = ProductDto.ToDto(product);
            if (result.IsSuccess)
            {
                if (result.Value is null)
                {
                    this.logger.LogError("DTO conversion returned null value");
                    return Result<ProductDto>.WithFailure("DTO conversion returned null value");
                }

                return Result<ProductDto>.Success(result.Value);
            }
            else
            {
                this.logger.LogError("Failed to create ProductDto: {Errors}", string.Join(", ", result.Errors ?? []));
                return Result<ProductDto>.WithFailure(result.Errors);
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unhandled exception in UpdateProductCommandHandler");
            return Result<ProductDto>.WithFailure($"Operation finished with an exception {ex.Message}");
        }
    }
}
