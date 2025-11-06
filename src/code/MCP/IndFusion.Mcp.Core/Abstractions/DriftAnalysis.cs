namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Analysis of semantic drift.
/// </summary>
/// <param name="DriftDetected">Whether drift was detected.</param>
/// <param name="DriftType">Type of drift detected.</param>
/// <param name="DriftSeverity">Severity of the drift.</param>
/// <param name="AffectedAreas">Areas affected by the drift.</param>
/// <param name="Recommendations">Recommendations to address drift.</param>
public record DriftAnalysis(
    bool DriftDetected,
    string DriftType,
    string DriftSeverity,
    IEnumerable<string> AffectedAreas,
    IEnumerable<string> Recommendations
);