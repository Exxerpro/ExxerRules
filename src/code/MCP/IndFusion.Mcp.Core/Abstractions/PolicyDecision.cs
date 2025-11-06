namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Policy decision made during linting process.
/// </summary>
/// <param name="RuleId">Rule ID for which the decision was made.</param>
/// <param name="Decision">Decision made (Enforce, Suppress, Custom).</param>
/// <param name="Reason">Reason for the decision.</param>
/// <param name="Timestamp">When the decision was made.</param>
public record PolicyDecision(
    string RuleId,
    string Decision,
    string Reason,
    DateTime Timestamp
);