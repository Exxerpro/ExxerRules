namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Template for external service integration.
/// </summary>
/// <param name="TemplateName">Name of the template.</param>
/// <param name="ServiceType">Type of service the template is for.</param>
/// <param name="IntegrationSteps">Steps for integration.</param>
/// <param name="ConfigurationTemplate">Template for configuration.</param>
public record IntegrationTemplate(
    string TemplateName,
    string ServiceType,
    IEnumerable<IntegrationStep> IntegrationSteps,
    Dictionary<string, object> ConfigurationTemplate
);