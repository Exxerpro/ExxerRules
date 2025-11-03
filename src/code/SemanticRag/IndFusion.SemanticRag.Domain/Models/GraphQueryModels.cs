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

/// <summary>
/// Represents the result of a graph query execution.
/// </summary>
/// <param name="Records">The query result records.</param>
/// <param name="ExecutionTimeMs">Time taken to execute the query in milliseconds.</param>
/// <param name="RecordsAffected">Number of records affected by the query.</param>
/// <param name="Success">Whether the query executed successfully.</param>
/// <param name="ErrorMessage">Error message if the query failed.</param>
public readonly record struct GraphQueryResult(
    IReadOnlyList<GraphRecord> Records,
    long ExecutionTimeMs,
    int RecordsAffected,
    bool Success,
    string? ErrorMessage = null)
{
    /// <summary>
    /// Checks if the query was successful.
    /// </summary>
    public bool IsSuccess => Success && ErrorMessage is null;

    /// <summary>
    /// Gets the number of records returned.
    /// </summary>
    public int RecordCount => Records.Count;
}

/// <summary>
/// Represents a single record from a graph query result.
/// </summary>
/// <param name="Values">The values in the record.</param>
/// <param name="Keys">The keys corresponding to the values.</param>
public readonly record struct GraphRecord(
    IReadOnlyList<object> Values,
    IReadOnlyList<string> Keys)
{
    /// <summary>
    /// Gets a value by key.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value if found, otherwise null.</returns>
    public object? GetValue(string key)
    {
        for (int i = 0; i < Keys.Count; i++)
        {
            if (Keys[i] == key)
                return i < Values.Count ? Values[i] : null;
        }
        return null;
    }

    /// <summary>
    /// Gets a typed value by key.
    /// </summary>
    /// <typeparam name="T">The expected type.</typeparam>
    /// <param name="key">The key to look up.</param>
    /// <returns>The typed value if found and convertible, otherwise default.</returns>
    public T? GetValue<T>(string key)
    {
        var value = GetValue(key);
        if (value is T typedValue)
            return typedValue;

        if (value is null)
            return default;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Checks if the record contains a specific key.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <returns>True if the key exists, otherwise false.</returns>
    public bool HasKey(string key) => Keys.Contains(key);
}

/// <summary>
/// Represents a code node in the knowledge graph.
/// </summary>
/// <param name="Id">Unique identifier for the code node.</param>
/// <param name="Type">Type of the code node (Class, Method, Property, etc.).</param>
/// <param name="Name">Name of the code element.</param>
/// <param name="FullName">Fully qualified name.</param>
/// <param name="Namespace">Namespace containing the code element.</param>
/// <param name="FilePath">Path to the file containing the code.</param>
/// <param name="LineNumber">Line number where the code element is defined.</param>
/// <param name="StartLine">Starting line number of the code.</param>
/// <param name="EndLine">Ending line number of the code.</param>
/// <param name="CodeContent">The actual code content.</param>
/// <param name="Language">Programming language of the code.</param>
/// <param name="Patterns">Code patterns detected in this node.</param>
/// <param name="Properties">Additional properties of the code node.</param>
/// <param name="Embedding">Vector embedding for semantic similarity.</param>
/// <param name="CreatedAt">When the code node was created.</param>
/// <param name="UpdatedAt">When the code node was last updated.</param>
public readonly record struct CodeNode(
    string Id,
    string Type,
    string Name,
    string FullName,
    string? Namespace,
    string? FilePath,
    int? LineNumber,
    int StartLine,
    int EndLine,
    string CodeContent,
    string Language,
    IReadOnlyList<string> Patterns,
    IReadOnlyDictionary<string, object> Properties,
    float[]? Embedding,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    /// <summary>
    /// Gets the display name for the code node.
    /// </summary>
    public string DisplayName => !string.IsNullOrWhiteSpace(Namespace) 
        ? $"{Namespace}.{Name}" 
        : Name;

    /// <summary>
    /// Gets the file name from the file path.
    /// </summary>
    public string? FileName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                return null;
            
            var fileName = System.IO.Path.GetFileName(FilePath);
            return string.IsNullOrWhiteSpace(fileName) ? null : fileName;
        }
    }

    /// <summary>
    /// Checks if the code node has an embedding.
    /// </summary>
    public bool HasEmbedding => Embedding is not null && Embedding.Length > 0;

    /// <summary>
    /// Validates the code node.
    /// </summary>
    /// <returns>A Result indicating whether the code node is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Code node ID cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(Type))
            return Result.WithFailure("Code node type cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(Name))
            return Result.WithFailure("Code node name cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(FullName))
            return Result.WithFailure("Code node full name cannot be empty or whitespace");

        if (LineNumber.HasValue && LineNumber.Value < 0)
            return Result.WithFailure("Line number cannot be negative");

        return Result.Success();
    }

    /// <summary>
    /// Gets a property value by key.
    /// </summary>
    /// <typeparam name="T">The expected type of the property value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The property value if found, otherwise default value.</returns>
    public T? GetProperty<T>(string key)
    {
        if (Properties.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// Checks if the code node has a specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>True if the property exists, otherwise false.</returns>
    public bool HasProperty(string key)
    {
        return Properties.ContainsKey(key);
    }
}