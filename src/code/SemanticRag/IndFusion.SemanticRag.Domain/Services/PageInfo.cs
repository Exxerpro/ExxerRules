namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Information about pagination.
/// </summary>
/// <param name="CurrentPage">Current page number.</param>
/// <param name="PageSize">Results per page.</param>
/// <param name="TotalPages">Total number of pages.</param>
/// <param name="TotalCount">Total number of results.</param>
public readonly record struct PageInfo(int CurrentPage, int PageSize, int TotalPages, int TotalCount);