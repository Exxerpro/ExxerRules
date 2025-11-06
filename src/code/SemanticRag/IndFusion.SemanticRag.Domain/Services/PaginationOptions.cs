namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Pagination options for search results.
/// </summary>
/// <param name="Page">Page number (1-based).</param>
/// <param name="PageSize">Number of results per page.</param>
public readonly record struct PaginationOptions(int Page = 1, int PageSize = 10);