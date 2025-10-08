// <copyright file="ReportsFilterInfoBuilder.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.BarCodes.Queries.GetReportsList.FiltersInfo;
using IndTrace.Domain.Entities;

namespace IndTrace.Application.BarCodes.Queries.Builders;

/// <summary>
/// Builds filter information for reports using parallel data loading.
/// Extracted from GetReportesFilterInfoMonitorQueryHandler to follow SRP principles.
/// Implements CLAUDE.md compliance with Result pattern, null safety, and parallel optimization.
/// </summary>
public class ReportsFilterInfoBuilder : IReportsFilterInfoBuilder
{
    private readonly IRepository<FlowStatus> flowStatusRepository;
    private readonly IRepository<Customer> customerRepository;
    private readonly IRepository<Product> productRepository;
    private readonly IRepository<ShiftsCatalog> shiftCatalogRepository;
    private readonly ILogger<ReportsFilterInfoBuilder> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportsFilterInfoBuilder"/> class.
    /// </summary>
    /// <param name="flowStatusRepository">Repository for flow status data.</param>
    /// <param name="customerRepository">Repository for customer data.</param>
    /// <param name="productRepository">Repository for product data.</param>
    /// <param name="shiftCatalogRepository">Repository for shift catalog data.</param>
    /// <param name="logger">Logger for recording operations and errors.</param>
    public ReportsFilterInfoBuilder(
        IRepository<FlowStatus> flowStatusRepository,
        IRepository<Customer> customerRepository,
        IRepository<Product> productRepository,
        IRepository<ShiftsCatalog> shiftCatalogRepository,
        ILogger<ReportsFilterInfoBuilder> logger)
    {
        //[Fix]
        //CLAUDE
        //Date: 26/09/2025
        //Reason: [SRP REFACTOR] - Created ReportsFilterInfoBuilder following Single Responsibility Principle

        this.flowStatusRepository = flowStatusRepository ?? throw new ArgumentNullException(nameof(flowStatusRepository));
        this.customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        this.productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        this.shiftCatalogRepository = shiftCatalogRepository ?? throw new ArgumentNullException(nameof(shiftCatalogRepository));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Builds comprehensive filter information for reports.
    /// Uses parallel loading for optimal performance across all filter categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing filter info view model or failure reasons.</returns>
    public async Task<Result<ReportsFilterInfoVm>> BuildAsync(CancellationToken cancellationToken)
    {
        // Early cancellation check
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ReportsFilterInfoVm>.WithFailure(["Operation was canceled."]);
        }

        this.logger.LogInformation("Building reports filter information with parallel data loading");

        try
        {
            // Load all filter data in parallel for optimal performance
            var flowStatusTask = this.flowStatusRepository.ListAsync(cancellationToken);
            var customersTask = this.customerRepository.ListAsync(cancellationToken);
            var productsTask = this.productRepository.ListAsync(cancellationToken);
            var shiftsTask = this.shiftCatalogRepository.ListAsync(cancellationToken);

            await Task.WhenAll(flowStatusTask, customersTask, productsTask, shiftsTask).ConfigureAwait(false);

            var flowStatusResult = await flowStatusTask.ConfigureAwait(false);
            var customersResult = await customersTask.ConfigureAwait(false);
            var productsResult = await productsTask.ConfigureAwait(false);
            var shiftsResult = await shiftsTask.ConfigureAwait(false);

            // Check for any failures
            var allResults = new Result[] { flowStatusResult, customersResult, productsResult, shiftsResult };
            var failures = allResults.Where(r => r.IsFailure).SelectMany(r => r.Errors ?? []).ToList();

            if (failures.Any())
            {
                this.logger.LogError("Failed to load filter data: {Errors}", string.Join(", ", failures));
                return Result<ReportsFilterInfoVm>.WithFailure(failures);
            }

            // Extract data safely
            var flowStatuses = flowStatusResult.Value?.ToList() ?? [];
            var customers = customersResult.Value?.ToList() ?? [];
            var products = productsResult.Value?.ToList() ?? [];
            var shifts = shiftsResult.Value?.ToList() ?? [];

            // Build filter information
            var states = flowStatuses.Select(f => f.Name).Distinct().ToList();
            var customerNames = customers.Select(c => c.Name).ToList();
            var productPartNumbers = products.Select(p => p.PartNumber).ToList();
            var shiftPlantIds = shifts.Select(s => s.PlantId).ToList();

            // Create customer products mapping using GroupJoin for efficiency
            var customerProducts = customers
                .GroupJoin(
                    products,
                    customer => customer.CustomerId,
                    product => product.CustomerId,
                    (customer, customerProducts) => new CustomerProduct
                    {
                        CustomerId = customer.CustomerId,
                        Name = customer.Name,
                        Products = customerProducts.Select(product => new Product
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            PartNumber = product.PartNumber,
                        }).ToList(),
                    })
                .ToList();

            var vm = new ReportsFilterInfoVm
            {
                Products = productPartNumbers,
                States = states,
                Customers = customerNames,
                CustomerProducts = customerProducts,
                Shifts = shiftPlantIds,
            };

            this.logger.LogInformation(
                "Successfully built filter information: {StateCount} states, {CustomerCount} customers, {ProductCount} products, {ShiftCount} shifts",
                states.Count, customerNames.Count, productPartNumbers.Count, shiftPlantIds.Count);

            return Result<ReportsFilterInfoVm>.Success(vm);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error building reports filter information");
            return Result<ReportsFilterInfoVm>.WithFailure([$"Unexpected error building filter information: {ex.Message}"]);
        }
    }
}
