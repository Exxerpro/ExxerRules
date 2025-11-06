namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Behavioral change detected in code.
/// </summary>
/// <param name="BehaviorType">Type of behavioral change.</param>
/// <param name="ChangeDescription">Description of the behavioral change.</param>
/// <param name="Impact">Impact of the behavioral change.</param>
/// <param name="Confidence">Confidence in detecting the change (0.0-1.0).</param>
public record BehavioralChange(
    string BehaviorType,
    string ChangeDescription,
    string Impact,
    double Confidence
);