namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Represents a search filter.
/// </summary>
/// <param name="Field">Field to filter on.</param>
/// <param name="Operator">Filter operator.</param>
/// <param name="Value">Value to filter by.</param>
public readonly record struct SearchFilter(string Field, FilterOperator Operator, object Value);