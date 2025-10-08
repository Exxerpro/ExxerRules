namespace IndTrace.Application.Products.Services.Interfaces;

/// <summary>
/// Rule creation and linking orchestration service.
/// Application service - manages rule creation from DTOs and product association.
/// </summary>
public interface IRuleOrchestrator
{
    /// <summary>
    /// Creates and persists a rule for a product with machine association.
    /// Handles rule creation, machine assignment, and product linking.
    /// </summary>
    /// <param name="ruleDto">Rule data transfer object containing rule configuration</param>
    /// <param name="product">Product to associate the rule with</param>
    /// <param name="workflows">Workflows containing machine assignments for rule association</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>Created and persisted rule entity</returns>
    /// <remarks>
    /// Rule Creation Logic:
    /// - Converts RuleDto to Rule entity
    /// - Ensures RuleId = 0 for EF to assign new ID
    /// - Sets ProductId relationship to associate with product
    /// - Extracts MachineId from workflows (LastMachineId where > 0)
    /// - Persists rule to database
    /// - Updates product.RuleId with created rule ID
    /// - Updates product entity to maintain relationship
    ///
    /// Machine Assignment Logic:
    /// - Searches workflows for LastMachineId > 0
    /// - Uses Distinct() to avoid duplicate machine assignments
    /// - Takes FirstOrDefault() for single machine selection
    /// - This matches the original handler's machine selection logic
    /// </remarks>
    Task<Result<Rule>> CreateAndLinkRuleAsync(RuleDto ruleDto, Product product, IEnumerable<WorkFlow> workflows, CancellationToken cancellationToken);

    /// <summary>
    /// Converts RuleDto to Rule entity without persistence.
    /// Pure DTO to entity conversion for validation or preview.
    /// </summary>
    /// <param name="ruleDto">Rule DTO to convert</param>
    /// <returns>Rule entity converted from DTO</returns>
    Result<Rule> ConvertRuleDtoToEntity(RuleDto ruleDto);

    /// <summary>
    /// Determines machine assignment for rule from workflow collection.
    /// Extracts machine ID from workflows using business logic.
    /// </summary>
    /// <param name="workflows">Workflows containing machine assignments</param>
    /// <returns>Machine ID for rule association, or 0 if none found</returns>
    /// <remarks>
    /// Machine Selection Logic:
    /// - Filters workflows where LastMachineId > 0
    /// - Applies Distinct() to remove duplicates
    /// - Returns FirstOrDefault() for single machine selection
    /// - This preserves the original handler's machine assignment logic
    /// </remarks>
    int DetermineMachineIdFromWorkflows(IEnumerable<WorkFlow> workflows);

    /// <summary>
    /// Updates product entity with rule relationship.
    /// Establishes bidirectional relationship between Product and Rule.
    /// </summary>
    /// <param name="product">Product to update with rule relationship</param>
    /// <param name="rule">Rule to associate with product</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>Updated product with rule relationship established</returns>
    Task<Result<Product>> UpdateProductWithRuleAsync(Product product, Rule rule, CancellationToken cancellationToken);

    /// <summary>
    /// Validates rule configuration for product association.
    /// Ensures rule data is valid for the specific product type.
    /// </summary>
    /// <param name="ruleDto">Rule configuration to validate</param>
    /// <param name="product">Product the rule will be associated with</param>
    /// <returns>Validation result for rule-product compatibility</returns>
    Task<Result> ValidateRuleForProductAsync(RuleDto ruleDto, Product product);
}
