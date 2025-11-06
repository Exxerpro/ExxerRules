namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Represents a search facet for filtering.
/// </summary>
/// <param name="Name">Name of the facet (e.g., "type", "language", "source").</param>
/// <param name="Values">Values to filter by.</param>
/// <param name="Operator">How to combine multiple values.</param>
public readonly record struct SearchFacet(
    string Name,
    IReadOnlyList<string> Values,
    FacetOperator Operator = FacetOperator.Or)
{
    /// <summary>
    /// Validates the search facet.
    /// </summary>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return Result.WithFailure("Facet name cannot be null or empty");

        if (Values.Count == 0)
            return Result.WithFailure("At least one facet value must be specified");

        return Result.Success();
    }
}