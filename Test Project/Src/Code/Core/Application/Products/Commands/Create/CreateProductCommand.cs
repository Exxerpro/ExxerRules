// <copyright file="CreateProductCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.Products.Events;

namespace IndTrace.Application.Products.Commands.Create;

using IndTrace.Application.Products.Services;
using IndTrace.Application.RulesEngine.Dto;

/// <summary>
/// Command for creating a new product with its associated workflows, rules, and recipe.
/// </summary>
/// <remarks>
/// This command encapsulates all the data needed to create a complete product including:
/// - Product definition and properties
/// - Workflow sequences for machine processing
/// - Business rules for validation
/// - Manufacturing recipe specifications
/// The command automatically generates workflows based on the machine assignments.
/// </remarks>
public class CreateProductCommand(ProductCreationDto productDto) : IMonitorRequest<ProductCreatedEvent>
{
    /// <summary>
    /// Gets or sets the product information including part number, specifications, and customer details.
    /// </summary>
    /// <value>A ProductDto containing the core product data.</value>
    public ProductDto Product { get; set; } = productDto.Product;

    /// <summary>
    /// Gets or sets the collection of workflows that define the machine processing sequence for this product.
    /// </summary>
    /// <value>A list of WorkFlowDto objects representing the production flow through machines.</value>
    /// <remarks>
    /// Workflows are automatically generated from the machine assignments using the CreateWorkFlowDtos method.
    /// </remarks>
    public List<WorkFlowDto> WorkFlows { get; set; } = CreateWorkFlowDtos(productDto.Machines);

    /// <summary>
    /// Gets or sets the business rule associated with this product for validation and processing logic.
    /// </summary>
    /// <value>A RuleDto containing the rule configuration and criteria.</value>
    public RuleDto Rule { get; set; } = productDto.Rule;

    /// <summary>
    /// Gets or sets the manufacturing recipe that defines how this product should be produced.
    /// </summary>
    /// <value>A RecipeDto containing process parameters and manufacturing instructions.</value>
    public RecipeDto Recipe { get; set; } = productDto.Recipe;

    /// <summary>
    /// Creates a collection of workflow DTOs from a sequence of machine IDs, establishing the production flow.
    /// </summary>
    /// <param name="machines">The collection of machine IDs that will process this product.</param>
    /// <returns>A list of WorkFlowDto objects representing the sequential machine processing flow.</returns>
    /// <remarks>
    /// This method creates a workflow chain where:
    /// - Machines are sorted in ascending order
    /// - Each workflow connects consecutive machines (LastMachineId -> NextMachineId)
    /// - The chain starts from machine 0 (initial state) and ends at machine 0 (completion)
    /// - All workflows are assigned RuleId 2005 as the default processing rule
    ///
    /// Example: For machines [5, 3, 7], creates workflows:
    /// - 0 -> 3 (RuleId: 2005)
    /// - 3 -> 5 (RuleId: 2005)
    /// - 5 -> 7 (RuleId: 2005)
    /// - 7 -> 0 (RuleId: 2005).
    /// </remarks>
    /// <exception cref="ArgumentNullException">Not thrown, but returns empty list if machines is null.</exception>
    public static List<WorkFlowDto> CreateWorkFlowDtos(IEnumerable<int> machines)
    {
        // Step 1: Sort the machines and convert to a list
        if (machines is null || !machines.Any())
        {
            return [];
        }

        var sortedMachines = machines.Distinct().OrderBy(m => m).ToList(); // Sort machines in ascending order

        // Step 2: Use LINQ to construct the circular linked list
        var workFlowDtos = sortedMachines
            .Prepend(0) // Add 0 at the beginning
            .Append(0) // Add 0 at the end
            .Zip(sortedMachines.Append(0), (last, next) => new WorkFlowDto
            {
                LastMachineId = last,
                NextMachineId = next,
                RuleId = 2005,
            })
            .ToList();

        return workFlowDtos;
    }
}
