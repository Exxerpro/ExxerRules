namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// How to combine facet values.
/// </summary>
public enum FacetOperator
{
    /// <summary>
    /// OR operation - match any of the values.
    /// </summary>
    Or,

    /// <summary>
    /// AND operation - match all of the values.
    /// </summary>
    And
}