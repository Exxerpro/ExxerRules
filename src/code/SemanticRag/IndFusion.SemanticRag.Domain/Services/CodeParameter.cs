namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Represents a code parameter.
/// </summary>
/// <param name="Name">Parameter name.</param>
/// <param name="Type">Parameter type.</param>
/// <param name="IsOptional">Whether the parameter is optional.</param>
/// <param name="DefaultValue">Default value (if any).</param>
public readonly record struct CodeParameter(
    string Name,
    string Type,
    bool IsOptional = false,
    string? DefaultValue = null);