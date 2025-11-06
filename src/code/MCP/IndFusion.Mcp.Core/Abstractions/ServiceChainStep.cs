namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// A step in a service chain.
/// </summary>
/// <param name="ServiceName">Name of the service.</param>
/// <param name="InputTransformation">Transformation to apply to inputs.</param>
/// <param name="OutputTransformation">Transformation to apply to outputs.</param>
/// <param name="ValidationRules">Validation rules for this step.</param>
/// <param name="TimeoutMs">Timeout for this step.</param>
public record ServiceChainStep(
    string ServiceName,
    string InputTransformation,
    string OutputTransformation,
    IEnumerable<ValidationRule> ValidationRules,
    int TimeoutMs = 30000
);