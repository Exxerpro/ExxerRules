namespace IndTrace.Application.Products.Services.Interfaces;

/// <summary>
/// Workflow creation and relationship orchestration service.
/// Application service - manages workflow generation from machine assignments and persistence.
/// </summary>
public interface IWorkflowOrchestrator
{
    /// <summary>
    /// Creates and persists workflows for a product based on machine assignments.
    /// Generates workflow chain from machine collection using business rules.
    /// </summary>
    /// <param name="product">Product to create workflows for</param>
    /// <param name="machineIds">Collection of machine IDs for workflow creation</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>Collection of created and persisted workflows</returns>
    /// <remarks>
    /// Workflow Creation Logic:
    /// - Sorts machines in ascending order for predictable workflow chain
    /// - Creates circular workflow: 0 → Machine1 → Machine2 → ... → 0
    /// - Assigns RuleId 2005 as default for all workflows
    /// - Sets ProductId and RuleId relationships
    /// - Persists workflows using bulk operations
    ///
    /// Example: For machines [5, 3, 7]:
    /// - 0 → 3 (RuleId: 2005)
    /// - 3 → 5 (RuleId: 2005)
    /// - 5 → 7 (RuleId: 2005)
    /// - 7 → 0 (RuleId: 2005)
    /// </remarks>
    Task<Result<IEnumerable<WorkFlow>>> CreateAndPersistWorkflowsAsync(Product product, IEnumerable<int> machineIds, CancellationToken cancellationToken);

    /// <summary>
    /// Generates workflow DTOs from machine collection without persistence.
    /// Pure workflow generation logic for validation or preview.
    /// </summary>
    /// <param name="machineIds">Collection of machine IDs</param>
    /// <returns>Generated workflow DTOs ready for entity conversion</returns>
    /// <remarks>
    /// Uses the same workflow generation algorithm as CreateProductCommand.CreateWorkFlowDtos.
    /// This method extracts the workflow generation logic for reusability.
    /// </remarks>
    Result<IEnumerable<WorkFlowDto>> GenerateWorkflowDtos(IEnumerable<int> machineIds);

    /// <summary>
    /// Converts workflow DTOs to entities and establishes relationships.
    /// Handles DTO to entity conversion with error collection.
    /// </summary>
    /// <param name="workflowDtos">Workflow DTOs to convert</param>
    /// <param name="product">Product to associate workflows with</param>
    /// <returns>Collection of workflow entities with relationships established</returns>
    Task<Result<IEnumerable<WorkFlow>>> ConvertAndLinkWorkflowsAsync(IEnumerable<WorkFlowDto> workflowDtos, Product product);

    /// <summary>
    /// Validates machine assignments for workflow creation.
    /// Ensures all machines are valid and available for product processing.
    /// </summary>
    /// <param name="machineIds">Machine IDs to validate</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>Validation result for machine assignments</returns>
    Task<Result> ValidateMachineAssignmentsAsync(IEnumerable<int> machineIds, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves existing workflows for a product.
    /// Used for workflow updates or relationship verification.
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>Existing workflows for the product</returns>
    Task<Result<IEnumerable<WorkFlow>>> GetWorkflowsForProductAsync(int productId, CancellationToken cancellationToken);
}
