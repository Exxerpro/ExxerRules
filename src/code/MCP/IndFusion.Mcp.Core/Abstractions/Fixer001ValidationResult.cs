namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of Fixer001 validation operations.
/// </summary>
/// <param name="IsReady">Whether Fixer001 is ready to be applied.</param>
/// <param name="ReadinessScore">Readiness score from 0.0 to 1.0.</param>
/// <param name="Issues">Issues preventing application.</param>
/// <param name="Warnings">Warnings about the application.</param>
/// <param name="ValidationTimeMs">Time taken for validation.</param>
public record Fixer001ValidationResult(
    bool IsReady,
    double ReadinessScore,
    IEnumerable<Fixer001Issue> Issues,
    IEnumerable<Fixer001Warning> Warnings,
    long ValidationTimeMs
);