using IndQuestResults;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a graph query for knowledge graph operations.
/// </summary>
/// <param name="Query">The Cypher query string.</param>
/// <param name="Parameters">Parameters for the query.</param>
/// <param name="TimeoutMs">Query timeout in milliseconds.</param>
/// <param name="Description">Optional description of the query.</param>
public readonly record struct GraphQuery(
    string Query,
    IReadOnlyDictionary<string, object>? Parameters = null,
    int TimeoutMs = 30000,
    string? Description = null)
{
    /// <summary>
    /// Validates the graph query.
    /// </summary>
    /// <returns>A Result indicating whether the query is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Query))
            return Result.WithFailure("Query cannot be null or empty");

        if (TimeoutMs <= 0)
            return Result.WithFailure("Timeout must be greater than 0");

        return Result.Success();
    }
}