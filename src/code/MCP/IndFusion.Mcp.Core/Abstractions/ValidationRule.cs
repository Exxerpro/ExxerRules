namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// A validation rule for service chains.
/// </summary>
/// <param name="RuleName">Name of the rule.</param>
/// <param name="RuleType">Type of the rule.</param>
/// <param name="Condition">Condition for the rule.</param>
/// <param name="ErrorMessage">Error message if rule fails.</param>
public record ValidationRule(
    string RuleName,
    string RuleType,
    string Condition,
    string ErrorMessage
);