namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Policy recommendation for a specific violation.
/// </summary>
/// <param name="Action">Recommended action (Fix, Suppress, Ignore, Escalate).</param>
/// <param name="Reason">Reason for the recommendation.</param>
/// <param name="Confidence">Confidence in the recommendation (0.0-1.0).</param>
/// <param name="AutoFixable">Whether this violation can be automatically fixed.</param>
/// <param name="EstimatedEffort">Estimated effort to fix (Low, Medium, High).</param>
public record PolicyRecommendation(
    string Action,
    string Reason,
    double Confidence,
    bool AutoFixable,
    string EstimatedEffort
);