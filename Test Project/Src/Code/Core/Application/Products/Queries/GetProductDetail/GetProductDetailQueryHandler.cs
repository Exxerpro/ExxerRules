// <copyright file="GetProductDetailQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Products.Queries.GetProductDetail;

/// <summary>
/// Handles the retrieval of detailed product information including associated customer data.
/// Provides comprehensive product details for display and operational purposes.
/// </summary>
public class GetProductDetailQueryHandler : IMonitorRequestHandler<GetProductDetailQuery, ProductDto>
{
    private readonly IRepository<Product> productRepository;
    private readonly IRepository<Customer> customerRepository;
    private readonly ILogger<GetProductDetailQueryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProductDetailQueryHandler"/> class.
    /// </summary>
    /// <param name="productRepository">Repository for accessing product data.</param>
    /// <param name="customerRepository">Repository for accessing customer data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public GetProductDetailQueryHandler(
        IRepository<Product> productRepository,
        IRepository<Customer> customerRepository,
        ILogger<GetProductDetailQueryHandler> logger)
    {
        this.productRepository = productRepository;
        this.customerRepository = customerRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Processes the product detail query and returns comprehensive product information.
    /// </summary>
    /// <param name="request">The query containing the product ID to retrieve details for.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A result containing the detailed product data transfer object.</returns>
    public async Task<Result<ProductDto>> ProcessAsync(GetProductDetailQuery request, CancellationToken cancellationToken)
    {
        Product? product = null;
        Customer? customer = null;

        if (!string.IsNullOrWhiteSpace(request.ProductName))
        {
            var productsResult = await this.productRepository.ListAsync(cancellationToken);
            if (!productsResult.IsSuccess)
            {
                this.logger.LogError("Failed to retrieve Products: {Errors}", string.Join(", ", productsResult.Errors ?? []));
                return Result<ProductDto>.WithFailure(productsResult.Errors);
            }

            product = productsResult.Value?.FirstOrDefault(p => p.ProductName == request.ProductName);
            if (product == null)
            {
                this.logger.LogError("Product not found with ProductName: {ProductName}", request.ProductName);
                return Result<ProductDto>.WithFailure($"Product with name {request.ProductName} not found");
            }

            var customersResult = await this.customerRepository.ListAsync(cancellationToken);
            if (!customersResult.IsSuccess)
            {
                this.logger.LogError("Failed to retrieve Customers: {Errors}", string.Join(", ", customersResult.Errors ?? []));
                return Result<ProductDto>.WithFailure(customersResult.Errors);
            }

            customer = customersResult.Value?.FirstOrDefault(p => p.Name == product.CustomerName);
        }
        else if (request.ProductId > 0)
        {
            var getResult = await this.productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (!getResult.IsSuccess || getResult.Value == null)
            {
                this.logger.LogError("Product not found with ProductId: {ProductId}", request.ProductId);
                return Result<ProductDto>.WithFailure($"Product with ID {request.ProductId} not found");
            }

            product = getResult.Value;

            var customersResult = await this.customerRepository.ListAsync(cancellationToken);
            if (!customersResult.IsSuccess)
            {
                this.logger.LogError("Failed to retrieve Customers: {Errors}", string.Join(", ", customersResult.Errors ?? []));
                return Result<ProductDto>.WithFailure(customersResult.Errors);
            }

            customer = customersResult.Value?.FirstOrDefault(p => p.Name == product.CustomerName);
            if (customer is not null)
            {
                product.Customer = customer;
            }
        }
        else
        {
            this.logger.LogError("Invalid request: Both ProductName and ProductId are null or invalid");
            return Result<ProductDto>.WithFailure("Please provide either a valid ProductName or ProductId");
        }

        var viewModel = ProductDto.ToDto(product);
        if (viewModel.IsSuccess)
        {
            if (viewModel.Value is null)
            {
                return Result<ProductDto>.WithFailure("DTO conversion returned null value");
            }

            return Result<ProductDto>.Success(viewModel.Value);
        }
        else
        {
            this.logger.LogError("Failed to create ProductDto: {Errors}", string.Join(", ", viewModel.Errors ?? []));
            return Result<ProductDto>.WithFailure(viewModel.Errors);
        }
    }
}
