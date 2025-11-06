namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Rule for service dependencies.
/// </summary>
/// <param name="RuleName">Name of the rule.</param>
/// <param name="Condition">Condition for the rule.</param>
/// <param name="Action">Action to take when condition is met.</param>
/// <param name="Priority">Priority of the rule.</param>
public record DependencyRule(
    string RuleName,
    string Condition,
    string Action,
    int Priority
);