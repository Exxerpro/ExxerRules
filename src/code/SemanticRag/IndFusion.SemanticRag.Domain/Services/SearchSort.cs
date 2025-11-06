namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Represents a search sort option.
/// </summary>
/// <param name="Field">Field to sort by.</param>
/// <param name="Direction">Sort direction.</param>
public readonly record struct SearchSort(string Field, SortDirection Direction);