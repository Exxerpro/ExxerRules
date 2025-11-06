namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Service composition contract for MCP tools.
/// Defines how individual services are composed to create MCP tools.
/// </summary>
public interface IMcpToolCompositionService
{
    /// <summary>
    /// Composes the lint_run MCP tool from individual services.
    /// </summary>
    /// <param name="request">Request for linting operations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composed linting result.</returns>
    Task<McpToolResult> ComposeLintRunToolAsync(LintingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Composes the pattern_suggest MCP tool from individual services.
    /// </summary>
    /// <param name="request">Request for pattern suggestions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composed pattern suggestion result.</returns>
    Task<McpToolResult> ComposePatternSuggestToolAsync(PatternSuggestionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Composes the fixer001_apply MCP tool from individual services.
    /// </summary>
    /// <param name="request">Request for code transformations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composed transformation result.</returns>
    Task<McpToolResult> ComposeFixer001ApplyToolAsync(Fixer001Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Composes the safe_regex_replace MCP tool from individual services.
    /// </summary>
    /// <param name="request">Request for regex transformations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composed regex transformation result.</returns>
    Task<McpToolResult> ComposeSafeRegexReplaceToolAsync(SafeRegexRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Composes the knowledge_rag MCP tool from individual services.
    /// </summary>
    /// <param name="request">Request for RAG operations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composed RAG result.</returns>
    Task<McpToolResult> ComposeKnowledgeRagToolAsync(RagQueryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the composition configuration for MCP tools.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composition configuration.</returns>
    Task<McpToolCompositionConfiguration> GetCompositionConfigurationAsync(CancellationToken cancellationToken = default);
}