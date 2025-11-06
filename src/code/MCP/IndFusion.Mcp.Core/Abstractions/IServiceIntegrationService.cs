namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Service integration contract for integrating with external systems.
/// </summary>
public interface IServiceIntegrationService
{
    /// <summary>
    /// Integrates with external linting services.
    /// </summary>
    /// <param name="request">Request for external linting.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>External linting result.</returns>
    Task<ExternalLintingResult> IntegrateExternalLintingAsync(ExternalLintingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Integrates with external code analysis services.
    /// </summary>
    /// <param name="request">Request for external analysis.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>External analysis result.</returns>
    Task<ExternalAnalysisResult> IntegrateExternalAnalysisAsync(ExternalAnalysisRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Integrates with external knowledge bases.
    /// </summary>
    /// <param name="request">Request for external knowledge integration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>External knowledge integration result.</returns>
    Task<ExternalKnowledgeResult> IntegrateExternalKnowledgeAsync(ExternalKnowledgeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the integration configuration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Integration configuration.</returns>
    Task<IntegrationConfiguration> GetIntegrationConfigurationAsync(CancellationToken cancellationToken = default);
}