namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for pattern suggestion operations.
/// </summary>
/// <param name="ViolationId">ID of the violation to suggest patterns for.</param>
/// <param name="RuleId">Rule ID that generated the violation.</param>
/// <param name="CodeSnippet">Code snippet containing the violation.</param>
/// <param name="FilePath">Path to the file containing the violation.</param>
/// <param name="Context">Additional context about the violation.</param>
/// <param name="MaxSuggestions">Maximum number of suggestions to return.</param>
/// <param name="ConfidenceThreshold">Minimum confidence threshold for suggestions.</param>
public record PatternSuggestionRequest(
    string ViolationId,
    string RuleId,
    string CodeSnippet,
    string FilePath,
    Dictionary<string, object>? Context = null,
    int MaxSuggestions = 5,
    double ConfidenceThreshold = 0.7
);

/// <summary>
/// Request for pattern analysis operations.
/// </summary>
/// <param name="ProjectPath">Path to the project to analyze.</param>
/// <param name="PatternType">Type of patterns to analyze for.</param>
/// <param name="Scope">Analysis scope (file, directory, project).</param>
/// <param name="IncludeMetrics">Whether to include pattern metrics.</param>
/// <param name="GenerateReport">Whether to generate a detailed report.</param>
public record PatternAnalysisRequest(
    string ProjectPath,
    string PatternType,
    string Scope = "project",
    bool IncludeMetrics = true,
    bool GenerateReport = false
);

/// <summary>
/// Request for pattern graph query operations.
/// </summary>
/// <param name="Query">Graph query to execute.</param>
/// <param name="RepositoryScope">Repository scope for the query.</param>
/// <param name="MaxDepth">Maximum traversal depth.</param>
/// <param name="IncludeMetadata">Whether to include node/edge metadata.</param>
/// <param name="Filters">Additional query filters.</param>
public record PatternGraphRequest(
    string Query,
    string RepositoryScope,
    int MaxDepth = 10,
    bool IncludeMetadata = true,
    Dictionary<string, object>? Filters = null
);

/// <summary>
/// Request for pattern extraction operations.
/// </summary>
/// <param name="SourceCode">Source code to extract patterns from.</param>
/// <param name="PatternTypes">Types of patterns to extract.</param>
/// <param name="Metadata">Metadata about the source code.</param>
/// <param name="ConfidenceThreshold">Minimum confidence for extracted patterns.</param>
/// <param name="AddToKnowledgeBase">Whether to add extracted patterns to knowledge base.</param>
public record PatternExtractionRequest(
    string SourceCode,
    IEnumerable<string> PatternTypes,
    Dictionary<string, object> Metadata,
    double ConfidenceThreshold = 0.8,
    bool AddToKnowledgeBase = true
);

/// <summary>
/// Result of pattern suggestion operations.
/// </summary>
/// <param name="Success">Whether the operation succeeded.</param>
/// <param name="Suggestions">Collection of pattern suggestions.</param>
/// <param name="ConfidenceScores">Confidence scores for suggestions.</param>
/// <param name="Citations">Source citations for suggestions.</param>
/// <param name="ExecutionTimeMs">Time taken to generate suggestions.</param>
/// <param name="ErrorDetails">Error details if operation failed.</param>
public record PatternSuggestionResult(
    bool Success,
    IEnumerable<PatternSuggestion> Suggestions,
    Dictionary<string, double> ConfidenceScores,
    IEnumerable<PatternCitation> Citations,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Result of pattern analysis operations.
/// </summary>
/// <param name="Success">Whether the analysis succeeded.</param>
/// <param name="PatternAlignment">Pattern alignment analysis results.</param>
/// <param name="ImprovementSuggestions">Suggestions for pattern improvements.</param>
/// <param name="Metrics">Pattern metrics and statistics.</param>
/// <param name="Report">Detailed analysis report if requested.</param>
/// <param name="ExecutionTimeMs">Time taken for analysis.</param>
/// <param name="ErrorDetails">Error details if analysis failed.</param>
public record PatternAnalysisResult(
    bool Success,
    PatternAlignmentAnalysis PatternAlignment,
    IEnumerable<ImprovementSuggestion> ImprovementSuggestions,
    PatternMetrics Metrics,
    string? Report = null,
    long ExecutionTimeMs = 0,
    string? ErrorDetails = null
);

/// <summary>
/// Result of pattern graph query operations.
/// </summary>
/// <param name="Success">Whether the query succeeded.</param>
/// <param name="GraphResults">Graph traversal results.</param>
/// <param name="Relationships">Discovered relationships.</param>
/// <param name="PatternInsights">Pattern insights from the graph.</param>
/// <param name="QueryExecutionTimeMs">Time taken to execute the query.</param>
/// <param name="ErrorDetails">Error details if query failed.</param>
public record PatternGraphResult(
    bool Success,
    IEnumerable<GraphTraversalResult> GraphResults,
    IEnumerable<PatternRelationship> Relationships,
    IEnumerable<PatternInsight> PatternInsights,
    long QueryExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Result of pattern extraction operations.
/// </summary>
/// <param name="Success">Whether the extraction succeeded.</param>
/// <param name="ExtractedPatterns">Patterns extracted from the source code.</param>
/// <param name="KnowledgeBaseUpdates">Updates made to the knowledge base.</param>
/// <param name="ExtractionMetrics">Metrics about the extraction process.</param>
/// <param name="ExecutionTimeMs">Time taken for extraction.</param>
/// <param name="ErrorDetails">Error details if extraction failed.</param>
public record PatternExtractionResult(
    bool Success,
    IEnumerable<ExtractedPattern> ExtractedPatterns,
    IEnumerable<KnowledgeBaseUpdate> KnowledgeBaseUpdates,
    ExtractionMetrics ExtractionMetrics,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// A pattern suggestion with details and confidence.
/// </summary>
/// <param name="Id">Unique identifier for the suggestion.</param>
/// <param name="PatternType">Type of pattern suggested.</param>
/// <param name="Description">Description of the suggested pattern.</param>
/// <param name="CodeExample">Example code showing the pattern.</param>
/// <param name="Confidence">Confidence score (0.0-1.0).</param>
/// <param name="Effort">Estimated effort to implement (Low, Medium, High).</param>
/// <param name="Benefits">Benefits of applying the pattern.</param>
/// <param name="Citations">Source citations for the pattern.</param>
public record PatternSuggestion(
    string Id,
    string PatternType,
    string Description,
    string CodeExample,
    double Confidence,
    string Effort,
    IEnumerable<string> Benefits,
    IEnumerable<PatternCitation> Citations
);

/// <summary>
/// Citation for a pattern suggestion.
/// </summary>
/// <param name="Source">Source of the citation (document, code, etc.).</param>
/// <param name="Url">URL to the source if available.</param>
/// <param name="Confidence">Confidence in the citation (0.0-1.0).</param>
/// <param name="Relevance">Relevance to the current context (0.0-1.0).</param>
public record PatternCitation(
    string Source,
    string? Url,
    double Confidence,
    double Relevance
);

/// <summary>
/// Pattern alignment analysis results.
/// </summary>
/// <param name="OverallScore">Overall alignment score (0.0-1.0).</param>
/// <param name="PatternScores">Scores for individual patterns.</param>
/// <param name="AlignmentIssues">Issues found in pattern alignment.</param>
/// <param name="Recommendations">Recommendations for improvement.</param>
public record PatternAlignmentAnalysis(
    double OverallScore,
    Dictionary<string, double> PatternScores,
    IEnumerable<string> AlignmentIssues,
    IEnumerable<string> Recommendations
);

/// <summary>
/// Improvement suggestion for pattern alignment.
/// </summary>
/// <param name="Type">Type of improvement suggestion.</param>
/// <param name="Description">Description of the improvement.</param>
/// <param name="Priority">Priority level (Low, Medium, High).</param>
/// <param name="EstimatedEffort">Estimated effort to implement.</param>
/// <param name="ExpectedBenefit">Expected benefit from the improvement.</param>
public record ImprovementSuggestion(
    string Type,
    string Description,
    string Priority,
    string EstimatedEffort,
    string ExpectedBenefit
);

/// <summary>
/// Pattern metrics and statistics.
/// </summary>
/// <param name="TotalPatterns">Total number of patterns found.</param>
/// <param name="PatternDistribution">Distribution of patterns by type.</param>
/// <param name="ComplianceScore">Overall compliance score.</param>
/// <param name="QualityMetrics">Quality metrics for patterns.</param>
public record PatternMetrics(
    int TotalPatterns,
    Dictionary<string, int> PatternDistribution,
    double ComplianceScore,
    Dictionary<string, double> QualityMetrics
);

/// <summary>
/// Graph traversal result from pattern graph queries.
/// </summary>
/// <param name="NodeId">ID of the traversed node.</param>
/// <param name="NodeType">Type of the node.</param>
/// <param name="Properties">Node properties.</param>
/// <param name="Relationships">Relationships from this node.</param>
/// <param name="Depth">Depth of traversal.</param>
public record GraphTraversalResult(
    string NodeId,
    string NodeType,
    Dictionary<string, object> Properties,
    IEnumerable<string> Relationships,
    int Depth
);

/// <summary>
/// Pattern relationship discovered in the graph.
/// </summary>
/// <param name="SourcePattern">Source pattern ID.</param>
/// <param name="TargetPattern">Target pattern ID.</param>
/// <param name="RelationshipType">Type of relationship.</param>
/// <param name="Strength">Strength of the relationship (0.0-1.0).</param>
/// <param name="Context">Context of the relationship.</param>
public record PatternRelationship(
    string SourcePattern,
    string TargetPattern,
    string RelationshipType,
    double Strength,
    Dictionary<string, object> Context
);

/// <summary>
/// Pattern insight derived from graph analysis.
/// </summary>
/// <param name="InsightType">Type of insight.</param>
/// <param name="Description">Description of the insight.</param>
/// <param name="Confidence">Confidence in the insight (0.0-1.0).</param>
/// <param name="SupportingEvidence">Evidence supporting the insight.</param>
/// <param name="Recommendations">Recommendations based on the insight.</param>
public record PatternInsight(
    string InsightType,
    string Description,
    double Confidence,
    IEnumerable<string> SupportingEvidence,
    IEnumerable<string> Recommendations
);

/// <summary>
/// Extracted pattern from source code.
/// </summary>
/// <param name="Id">Unique identifier for the extracted pattern.</param>
/// <param name="PatternType">Type of the extracted pattern.</param>
/// <param name="CodeSnippet">Code snippet containing the pattern.</param>
/// <param name="Confidence">Confidence in the extraction (0.0-1.0).</param>
/// <param name="Metadata">Additional metadata about the pattern.</param>
/// <param name="SourceLocation">Location in source code where pattern was found.</param>
public record ExtractedPattern(
    string Id,
    string PatternType,
    string CodeSnippet,
    double Confidence,
    Dictionary<string, object> Metadata,
    SourceLocation SourceLocation
);

/// <summary>
/// Knowledge base update from pattern extraction.
/// </summary>
/// <param name="UpdateType">Type of update (Add, Update, Delete).</param>
/// <param name="EntityId">ID of the entity being updated.</param>
/// <param name="EntityType">Type of the entity.</param>
/// <param name="Changes">Changes made to the entity.</param>
/// <param name="Timestamp">When the update was made.</param>
public record KnowledgeBaseUpdate(
    string UpdateType,
    string EntityId,
    string EntityType,
    Dictionary<string, object> Changes,
    DateTime Timestamp
);

/// <summary>
/// Metrics about pattern extraction process.
/// </summary>
/// <param name="PatternsExtracted">Number of patterns extracted.</param>
/// <param name="ExtractionTimeMs">Time taken for extraction.</param>
/// <param name="ConfidenceDistribution">Distribution of confidence scores.</param>
/// <param name="PatternTypeDistribution">Distribution by pattern type.</param>
public record ExtractionMetrics(
    int PatternsExtracted,
    long ExtractionTimeMs,
    Dictionary<string, int> ConfidenceDistribution,
    Dictionary<string, int> PatternTypeDistribution
);

/// <summary>
/// Source location information.
/// </summary>
/// <param name="FilePath">Path to the source file.</param>
/// <param name="Line">Line number (1-based).</param>
/// <param name="Column">Column number (1-based).</param>
/// <param name="Length">Length of the code snippet.</param>
public record SourceLocation(
    string FilePath,
    int Line,
    int Column,
    int Length
);