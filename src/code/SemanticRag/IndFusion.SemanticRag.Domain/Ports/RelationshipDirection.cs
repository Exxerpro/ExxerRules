namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Represents the direction of relationships in graph queries.
/// </summary>
public enum RelationshipDirection
{
    /// <summary>
    /// Outgoing relationships (from the node).
    /// </summary>
    Outgoing,

    /// <summary>
    /// Incoming relationships (to the node).
    /// </summary>
    Incoming,

    /// <summary>
    /// Both incoming and outgoing relationships.
    /// </summary>
    Both
}