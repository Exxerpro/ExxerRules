namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents statistics about the graph structure.
/// </summary>
/// <param name="TotalNodes">Total number of nodes in the graph.</param>
/// <param name="TotalRelationships">Total number of relationships in the graph.</param>
/// <param name="NodeTypes">Count of nodes by type.</param>
/// <param name="RelationshipTypes">Count of relationships by type.</param>
/// <param name="AverageDegree">Average degree of nodes.</param>
/// <param name="MaxDegree">Maximum degree of any node.</param>
/// <param name="ConnectedComponents">Number of connected components.</param>
/// <param name="LastUpdated">When the statistics were last updated.</param>
public readonly record struct GraphStatistics(
    int TotalNodes,
    int TotalRelationships,
    IReadOnlyDictionary<string, int> NodeTypes,
    IReadOnlyDictionary<string, int> RelationshipTypes,
    double AverageDegree,
    int MaxDegree,
    int ConnectedComponents,
    DateTimeOffset LastUpdated)
{
    /// <summary>
    /// Gets the density of the graph (relationships / possible relationships).
    /// </summary>
    public double Density
    {
        get
        {
            if (TotalNodes <= 1)
                return 0.0;

            var maxPossibleRelationships = TotalNodes * (TotalNodes - 1);
            return (double)TotalRelationships / maxPossibleRelationships;
        }
    }

    /// <summary>
    /// Gets the most common node type.
    /// </summary>
    public string? MostCommonNodeType
    {
        get
        {
            if (NodeTypes.Count == 0)
                return null;

            return NodeTypes.MaxBy(kvp => kvp.Value).Key;
        }
    }

    /// <summary>
    /// Gets the most common relationship type.
    /// </summary>
    public string? MostCommonRelationshipType
    {
        get
        {
            if (RelationshipTypes.Count == 0)
                return null;

            return RelationshipTypes.MaxBy(kvp => kvp.Value).Key;
        }
    }
}