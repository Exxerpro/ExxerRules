// <copyright file="FixtureHostedService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator;

using IndTrace.DataStore.DataAccess;
using IndTrace.DataStore.Interfaces;
using IndTrace.DataStore.Models;
using IndTrace.Simulator.Models.Constants;
using IndTrace.Simulator.Simulation;
using Microsoft.Extensions.Options;

/// <summary>
/// Background service that performs dry run simulations for all active products using different test paths and execution flavors.
/// </summary>
/// <remarks>
/// This service is intended for automated simulation/testing scenarios and runs on application startup.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="FixtureHostedService"/> class.
/// </remarks>
/// <param name="logger">The logger instance.</param>
/// <param name="productRepo">The product repository for retrieving active products.</param>
/// <param name="pathRunner">The test path runner for executing simulation paths.</param>
/// <param name="machineResolver">The machine resolver for resolving machine sequences.</param>
/// <param name="dryRunOptions">The dry run options.</param>
public class FixtureHostedService(ILogger<FixtureHostedService> logger, IProductRepository productRepo, ITestPathRunner pathRunner, MachineResolver machineResolver, IOptions<DryRunOptions> dryRunOptions) : BackgroundService
{
    private readonly DryRunOptions dryRunOptions = dryRunOptions.Value;

    /// <summary>
    /// Executes the dry run simulation for all active products and test paths.
    /// </summary>
    /// <param name="stoppingToken">A cancellation token that indicates when to stop the service.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var products = (await productRepo.GetActiveProductsAsync()).ToList();

        await this.ExecuteDryRunAsync(
            products,
            TestPathType.FullSuccess,
            ExecutionFlavor.FullSuccess,
            logger,
            pathRunner,
            machineResolver,
            this.dryRunOptions, stoppingToken);

        await this.ExecuteDryRunAsync(
            products,
            TestPathType.FullFailure,
            ExecutionFlavor.FinalFailure,
            logger,
            pathRunner,
            machineResolver,
            this.dryRunOptions, stoppingToken);

        await this.ExecuteDryRunAsync(
            products,
            TestPathType.FullFailure,
            ExecutionFlavor.CycleStarted,
            logger,
            pathRunner,
            machineResolver,
            this.dryRunOptions, stoppingToken);
    }

    /// <summary>
    /// Executes a dry run simulation for the specified products, path type, and execution flavor.
    /// </summary>
    /// <param name="products">The collection of products to simulate.</param>
    /// <param name="pathType">The test path type to use.</param>
    /// <param name="flavor">The execution flavor to use.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="pathRunner">The test path runner.</param>
    /// <param name="machineResolver">The machine resolver.</param>
    /// <param name="options">The dry run options.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    private async Task ExecuteDryRunAsync(
        IEnumerable<Product> products,
        TestPathType pathType,
        ExecutionFlavor flavor,
        ILogger logger,
        ITestPathRunner pathRunner,
        IMachineResolver machineResolver,
        DryRunOptions options, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var product in products)
            {
                var machineCount = (await machineResolver.GetMachineSequenceAsync(product.ProductId)).Count;

                foreach (var sequenceIndex in Enumerable.Range(0, options.NumberOfRun))
                {
                    for (int depthTotalMachines = machineCount; depthTotalMachines >= 1; depthTotalMachines--)
                    {
                        logger.LogInformation(
                            "Dry run No {SequenceIndex} for product {PartNumber} with depth {Depth}.",
                            sequenceIndex, product.PartNumber, depthTotalMachines);

                        await pathRunner.ExecutePathAsync(
                            product,
                            pathType,
                            sequenceIndex,
                            flavor,
                            depthTotalMachines,
                            options, cancellationToken);
                    }
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError("Error executing dry run: {Message}", e.Message);
        }
    }
}

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate fixture hosted service logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
