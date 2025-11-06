using System;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Extension methods for PatternSeverity.
/// </summary>
public static class PatternSeverityExtensions
{
    /// <summary>
    /// Gets the display name for the severity level.
    /// </summary>
    /// <param name="severity">The severity level.</param>
    /// <returns>The display name.</returns>
    public static string GetDisplayName(this PatternSeverity severity) => severity switch
    {
        PatternSeverity.Info => "Information",
        PatternSeverity.Warning => "Warning",
        PatternSeverity.Error => "Error",
        PatternSeverity.Critical => "Critical",
        _ => "Unknown"
    };

    /// <summary>
    /// Gets the short name for the severity level.
    /// </summary>
    /// <param name="severity">The severity level.</param>
    /// <returns>The short name.</returns>
    public static string GetShortName(this PatternSeverity severity) => severity switch
    {
        PatternSeverity.Info => "INFO",
        PatternSeverity.Warning => "WARN",
        PatternSeverity.Error => "ERROR",
        PatternSeverity.Critical => "CRIT",
        _ => "UNKNOWN"
    };

    /// <summary>
    /// Gets the numeric value for the severity level.
    /// </summary>
    /// <param name="severity">The severity level.</param>
    /// <returns>The numeric value.</returns>
    public static int GetNumericValue(this PatternSeverity severity) => (int)severity;

    /// <summary>
    /// Checks if the severity level is at or above the specified threshold.
    /// </summary>
    /// <param name="severity">The severity level.</param>
    /// <param name="threshold">The threshold to check against.</param>
    /// <returns>True if the severity is at or above the threshold.</returns>
    public static bool IsAtLeast(this PatternSeverity severity, PatternSeverity threshold) => 
        severity >= threshold;

    /// <summary>
    /// Checks if the severity level is critical or error.
    /// </summary>
    /// <param name="severity">The severity level.</param>
    /// <returns>True if the severity is critical or error.</returns>
    public static bool IsHighSeverity(this PatternSeverity severity) => 
        severity >= PatternSeverity.Error;

    /// <summary>
    /// Checks if the severity level is warning or higher.
    /// </summary>
    /// <param name="severity">The severity level.</param>
    /// <returns>True if the severity is warning or higher.</returns>
    public static bool IsSignificant(this PatternSeverity severity) => 
        severity >= PatternSeverity.Warning;

    /// <summary>
    /// Gets the color code for the severity level (for UI display).
    /// </summary>
    /// <param name="severity">The severity level.</param>
    /// <returns>The color code.</returns>
    public static string GetColorCode(this PatternSeverity severity) => severity switch
    {
        PatternSeverity.Info => "#17a2b8",      // Info blue
        PatternSeverity.Warning => "#ffc107",   // Warning yellow
        PatternSeverity.Error => "#dc3545",     // Error red
        PatternSeverity.Critical => "#6f42c1",  // Critical purple
        _ => "#6c757d"                          // Unknown gray
    };

    /// <summary>
    /// Gets the icon for the severity level (for UI display).
    /// </summary>
    /// <param name="severity">The severity level.</param>
    /// <returns>The icon name.</returns>
    public static string GetIcon(this PatternSeverity severity) => severity switch
    {
        PatternSeverity.Info => "info-circle",
        PatternSeverity.Warning => "exclamation-triangle",
        PatternSeverity.Error => "times-circle",
        PatternSeverity.Critical => "ban",
        _ => "question-circle"
    };
}