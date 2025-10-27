using System;
using System.Collections.Generic;
using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.ValueObjects;

/// <summary>
/// Represents a search query in the semantic RAG system.
/// </summary>
/// <param name="Text">The search query text.</param>
/// <param name="Limit">Maximum number of results to return.</param>
/// <param name="Threshold">Minimum similarity threshold for results.</param>
/// <param name="Filters">Additional search filters.</param>
public record SearchQuery(
    string Text,
    int Limit = 10,
    float Threshold = 0.0f,
    IReadOnlyDictionary<string, object>? Filters = null)
{
    /// <summary>
    /// Gets the search query text.
    /// </summary>
    public string Text { get; init; } = Text;

    /// <summary>
    /// Gets the maximum number of results to return.
    /// </summary>
    public int Limit { get; init; } = Limit;

    /// <summary>
    /// Gets the minimum similarity threshold.
    /// </summary>
    public float Threshold { get; init; } = Threshold;

    /// <summary>
    /// Gets the search filters.
    /// </summary>
    public IReadOnlyDictionary<string, object>? Filters { get; init; } = Filters;

    /// <summary>
    /// Validates the search query.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Text))
            return Result.WithFailure("Search query text cannot be null, empty, or whitespace");

        if (Text.Length > 1000)
            return Result.WithFailure("Search query text cannot exceed 1000 characters");

        if (Limit <= 0)
            return Result.WithFailure("Limit must be greater than zero");

        if (Limit > 1000)
            return Result.WithFailure("Limit cannot exceed 1000");

        if (Threshold < 0.0f || Threshold > 1.0f)
            return Result.WithFailure("Threshold must be between 0.0 and 1.0");

        return Result.Success();
    }

    /// <summary>
    /// Creates a new SearchQuery with validation.
    /// </summary>
    /// <param name="text">The search query text.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="threshold">Minimum similarity threshold for results.</param>
    /// <param name="filters">Additional search filters.</param>
    /// <returns>A Result containing the SearchQuery or validation error.</returns>
    public static Result<SearchQuery> Create(
        string text, 
        int limit = 10, 
        float threshold = 0.0f, 
        IReadOnlyDictionary<string, object>? filters = null)
    {
        var query = new SearchQuery(text, limit, threshold, filters);
        var validation = query.Validate();
        
        return validation.IsFailure 
            ? Result<SearchQuery>.WithFailure(validation.Error!)
            : Result<SearchQuery>.Success(query);
    }

    /// <summary>
    /// Gets a filter value by key.
    /// </summary>
    /// <typeparam name="T">The expected type of the filter value.</typeparam>
    /// <param name="key">The filter key.</param>
    /// <returns>The filter value if found, otherwise default value.</returns>
    public T? GetFilter<T>(string key)
    {
        if (Filters?.TryGetValue(key, out var value) == true && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// Checks if the query has a specific filter.
    /// </summary>
    /// <param name="key">The filter key.</param>
    /// <returns>True if the filter exists, otherwise false.</returns>
    public bool HasFilter(string key)
    {
        return Filters?.ContainsKey(key) == true;
    }

    /// <summary>
    /// Returns a string representation of the SearchQuery.
    /// </summary>
    /// <returns>A string representation.</returns>
    public override string ToString() => $"SearchQuery: \"{Text}\" (Limit: {Limit}, Threshold: {Threshold})";
}
