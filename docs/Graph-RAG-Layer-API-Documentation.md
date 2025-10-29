# Graph RAG Layer API Documentation

## Overview

The Graph RAG Layer provides comprehensive graph-based retrieval and pattern analysis capabilities for the Unified Semantic RAG Standards Initiative. This layer implements three core services that enable intelligent code analysis, pattern suggestion, and graph traversal operations.

## Architecture

The Graph RAG Layer follows a clean architecture pattern with clear separation of concerns:

- **Domain Layer**: Interface contracts and domain models
- **Application Layer**: Service implementations with business logic
- **Infrastructure Layer**: External dependencies (knowledge graph, logging)
- **MCP Tools**: External API surface for agent integration

## Core Services

### 1. IGraphQueryService

Provides graph query execution and traversal capabilities.

#### Methods

##### ExecuteQueryAsync
```csharp
Task<Result<GraphQueryResult>> ExecuteQueryAsync(
    string query, 
    IReadOnlyDictionary<string, object>? parameters = null, 
    CancellationToken cancellationToken = default)
```

**Description**: Executes a graph query and returns the results.

**Parameters**:
- `query`: The graph query to execute (e.g., Cypher)
- `parameters`: Optional parameters for the query
- `cancellationToken`: Cancellation token

**Returns**: `Result<GraphQueryResult>` containing query results or failure information

**Example**:
```csharp
var result = await graphQueryService.ExecuteQueryAsync(
    "MATCH (n:CodeNode) WHERE n.type = 'Class' RETURN n",
    new Dictionary<string, object> { ["limit"] = 10 },
    cancellationToken);
```

##### GetNodesAsync
```csharp
Task<Result<IReadOnlyList<GraphNode>>> GetNodesAsync(
    string nodeType, 
    IReadOnlyDictionary<string, object>? filters = null, 
    CancellationToken cancellationToken = default)
```

**Description**: Retrieves nodes of a specific type with optional filters.

**Parameters**:
- `nodeType`: The type of nodes to retrieve
- `filters`: Optional filters to apply
- `cancellationToken`: Cancellation token

**Returns**: `Result<IReadOnlyList<GraphNode>>` containing matching nodes

##### GetRelationshipsAsync
```csharp
Task<Result<IReadOnlyList<GraphRelationship>>> GetRelationshipsAsync(
    string relationshipType, 
    IReadOnlyDictionary<string, object>? filters = null, 
    CancellationToken cancellationToken = default)
```

**Description**: Retrieves relationships of a specific type with optional filters.

**Parameters**:
- `relationshipType`: The type of relationships to retrieve
- `filters`: Optional filters to apply
- `cancellationToken`: Cancellation token

**Returns**: `Result<IReadOnlyList<GraphRelationship>>` containing matching relationships

##### TraverseAsync
```csharp
Task<Result<GraphTraversalResult>> TraverseAsync(
    string startNodeId,
    int maxDepth = 3,
    IReadOnlyList<string>? relationshipTypes = null,
    CancellationToken cancellationToken = default)
```

**Description**: Executes a traversal query starting from a specific node.

**Parameters**:
- `startNodeId`: The ID of the starting node
- `maxDepth`: Maximum traversal depth
- `relationshipTypes`: Optional relationship types to follow
- `cancellationToken`: Cancellation token

**Returns**: `Result<GraphTraversalResult>` containing traversal results

##### FindShortestPathAsync
```csharp
Task<Result<GraphPath?>> FindShortestPathAsync(
    string startNodeId,
    string endNodeId,
    int maxDepth = 10,
    CancellationToken cancellationToken = default)
```

**Description**: Finds the shortest path between two nodes.

**Parameters**:
- `startNodeId`: The ID of the starting node
- `endNodeId`: The ID of the ending node
- `maxDepth`: Maximum path length
- `cancellationToken`: Cancellation token

**Returns**: `Result<GraphPath?>` containing the shortest path or null if no path exists

##### GetStatisticsAsync
```csharp
Task<Result<GraphStatistics>> GetStatisticsAsync(CancellationToken cancellationToken = default)
```

**Description**: Gets statistics about the graph structure.

**Returns**: `Result<GraphStatistics>` containing graph statistics

### 2. IPatternSuggestService

Provides pattern analysis and suggestion capabilities.

#### Methods

##### SuggestPatternsAsync
```csharp
Task<Result<IReadOnlyList<PatternSuggestion>>> SuggestPatternsAsync(
    string codeContext, 
    PatternSuggestionOptions options, 
    CancellationToken cancellationToken = default)
```

**Description**: Suggests patterns based on the provided code context.

**Parameters**:
- `codeContext`: The code context to analyze
- `options`: Options for pattern suggestion
- `cancellationToken`: Cancellation token

**Returns**: `Result<IReadOnlyList<PatternSuggestion>>` containing pattern suggestions

**Example**:
```csharp
var options = new PatternSuggestionOptions(
    MaxSuggestions: 5,
    MinConfidence: 0.7f,
    Categories: new List<string> { "Design Patterns" },
    IncludeCodeExamples: true,
    IncludeEffortEstimate: true
);

var result = await patternSuggestService.SuggestPatternsAsync(
    "public class UserService { }", 
    options, 
    cancellationToken);
```

##### AnalyzePatternAsync
```csharp
Task<Result<PatternAnalysis>> AnalyzePatternAsync(
    string code, 
    string patternType, 
    CancellationToken cancellationToken = default)
```

**Description**: Analyzes a specific pattern in the provided code.

**Parameters**:
- `code`: The code to analyze
- `patternType`: The type of pattern to analyze for
- `cancellationToken`: Cancellation token

**Returns**: `Result<PatternAnalysis>` containing pattern analysis results

##### FindViolationsAsync
```csharp
Task<Result<IReadOnlyList<PatternViolation>>> FindViolationsAsync(
    string code,
    string? filePath = null,
    CancellationToken cancellationToken = default)
```

**Description**: Finds pattern violations in the provided code.

**Parameters**:
- `code`: The code to analyze
- `filePath`: Optional file path for context
- `cancellationToken`: Cancellation token

**Returns**: `Result<IReadOnlyList<PatternViolation>>` containing pattern violations

##### GetPatternDefinitionsAsync
```csharp
Task<Result<IReadOnlyList<PatternDefinition>>> GetPatternDefinitionsAsync(
    string category,
    CancellationToken cancellationToken = default)
```

**Description**: Gets pattern definitions for a specific category.

**Parameters**:
- `category`: The pattern category
- `cancellationToken`: Cancellation token

**Returns**: `Result<IReadOnlyList<PatternDefinition>>` containing pattern definitions

##### GetPatternCategoriesAsync
```csharp
Task<Result<IReadOnlyList<string>>> GetPatternCategoriesAsync(CancellationToken cancellationToken = default)
```

**Description**: Gets all available pattern categories.

**Returns**: `Result<IReadOnlyList<string>>` containing pattern categories

##### ValidatePatternDefinitionAsync
```csharp
Task<Result> ValidatePatternDefinitionAsync(
    PatternDefinition patternDefinition,
    CancellationToken cancellationToken = default)
```

**Description**: Validates a pattern definition.

**Parameters**:
- `patternDefinition`: The pattern definition to validate
- `cancellationToken`: Cancellation token

**Returns**: `Result` indicating whether the pattern is valid

### 3. IPatternGraphQueryService

Provides pattern-specific graph query operations.

#### Methods

##### QueryPatternGraphAsync
```csharp
Task<Result<PatternGraphResult>> QueryPatternGraphAsync(
    PatternGraphQuery query, 
    CancellationToken cancellationToken = default)
```

**Description**: Queries the pattern graph with a specific query.

**Parameters**:
- `query`: The pattern graph query to execute
- `cancellationToken`: Cancellation token

**Returns**: `Result<PatternGraphResult>` containing pattern graph results

**Example**:
```csharp
var query = new PatternGraphQuery(
    Query: "MATCH (p:PatternDefinition) WHERE p.category = 'Design Patterns' RETURN p",
    Parameters: null,
    MaxResults: 10,
    TimeoutMs: 30000
);

var result = await patternGraphQueryService.QueryPatternGraphAsync(query, cancellationToken);
```

##### FindPatternRelationshipsAsync
```csharp
Task<Result<IReadOnlyList<PatternRelationship>>> FindPatternRelationshipsAsync(
    string patternId, 
    int maxDepth = 3, 
    CancellationToken cancellationToken = default)
```

**Description**: Finds pattern relationships starting from a specific pattern.

**Parameters**:
- `patternId`: The ID of the pattern to start from
- `maxDepth`: Maximum depth to traverse
- `cancellationToken`: Cancellation token

**Returns**: `Result<IReadOnlyList<PatternRelationship>>` containing pattern relationships

##### FindSimilarPatternsAsync
```csharp
Task<Result<IReadOnlyList<PatternSimilarity>>> FindSimilarPatternsAsync(
    string patternId,
    float similarityThreshold = 0.7f,
    int maxResults = 10,
    CancellationToken cancellationToken = default)
```

**Description**: Finds patterns that are similar to the specified pattern.

**Parameters**:
- `patternId`: The ID of the pattern to find similarities for
- `similarityThreshold`: Minimum similarity threshold
- `maxResults`: Maximum number of results to return
- `cancellationToken`: Cancellation token

**Returns**: `Result<IReadOnlyList<PatternSimilarity>>` containing similar patterns

##### GetPatternUsageStatisticsAsync
```csharp
Task<Result<PatternUsageStatistics>> GetPatternUsageStatisticsAsync(
    string patternId,
    CancellationToken cancellationToken = default)
```

**Description**: Gets pattern usage statistics across the codebase.

**Parameters**:
- `patternId`: The ID of the pattern to get statistics for
- `cancellationToken`: Cancellation token

**Returns**: `Result<PatternUsageStatistics>` containing pattern usage statistics

##### FindAntiPatternsAsync
```csharp
Task<Result<IReadOnlyList<AntiPatternViolation>>> FindAntiPatternsAsync(
    string? category = null,
    PatternSeverity severity = PatternSeverity.Warning,
    CancellationToken cancellationToken = default)
```

**Description**: Finds patterns that violate best practices.

**Parameters**:
- `category`: Optional pattern category to filter by
- `severity`: Minimum severity level
- `cancellationToken`: Cancellation token

**Returns**: `Result<IReadOnlyList<AntiPatternViolation>>` containing anti-pattern violations

##### GetPatternEvolutionAsync
```csharp
Task<Result<IReadOnlyList<PatternEvolution>>> GetPatternEvolutionAsync(
    string patternId,
    CancellationToken cancellationToken = default)
```

**Description**: Gets pattern evolution history for a specific pattern.

**Parameters**:
- `patternId`: The ID of the pattern to get history for
- `cancellationToken`: Cancellation token

**Returns**: `Result<IReadOnlyList<PatternEvolution>>` containing pattern evolution data

## MCP Tools

### 1. PatternSuggestTool

Provides MCP tool interface for pattern suggestion operations.

#### Tools

##### PatternSuggest
- **Description**: Suggest patterns based on code context analysis
- **Parameters**: codeContext, maxSuggestions, minConfidence, categories, includeCodeExamples, includeEffortEstimate
- **Returns**: JSON response with pattern suggestions

##### AnalyzePattern
- **Description**: Analyze a specific pattern in the provided code
- **Parameters**: code, patternType
- **Returns**: JSON response with pattern analysis

##### FindViolations
- **Description**: Find pattern violations in the provided code
- **Parameters**: code, filePath
- **Returns**: JSON response with pattern violations

### 2. PatternGraphQueryTool

Provides MCP tool interface for pattern graph query operations.

#### Tools

##### QueryPatternGraph
- **Description**: Query the pattern graph with a specific query
- **Parameters**: query, parameters, maxResults, timeoutMs
- **Returns**: JSON response with pattern graph results

##### FindPatternRelationships
- **Description**: Find pattern relationships starting from a specific pattern
- **Parameters**: patternId, maxDepth
- **Returns**: JSON response with pattern relationships

##### FindSimilarPatterns
- **Description**: Find patterns that are similar to the specified pattern
- **Parameters**: patternId, similarityThreshold, maxResults
- **Returns**: JSON response with similar patterns

##### GetPatternUsageStatistics
- **Description**: Get pattern usage statistics across the codebase
- **Parameters**: patternId
- **Returns**: JSON response with pattern usage statistics

### 3. GraphTraversalTool

Provides MCP tool interface for graph traversal operations.

#### Tools

##### ExecuteGraphQuery
- **Description**: Execute a graph query and return the results
- **Parameters**: query, parameters
- **Returns**: JSON response with graph query results

##### GetNodes
- **Description**: Retrieve nodes of a specific type with optional filters
- **Parameters**: nodeType, filters
- **Returns**: JSON response with matching nodes

##### TraverseGraph
- **Description**: Execute a traversal query starting from a specific node
- **Parameters**: startNodeId, maxDepth, relationshipTypes
- **Returns**: JSON response with traversal results

##### FindShortestPath
- **Description**: Find the shortest path between two nodes
- **Parameters**: startNodeId, endNodeId, maxDepth
- **Returns**: JSON response with shortest path

##### GetGraphStatistics
- **Description**: Get statistics about the graph structure
- **Parameters**: None
- **Returns**: JSON response with graph statistics

## Data Models

### Core Models

#### GraphQueryResult
```csharp
public readonly record struct GraphQueryResult(
    IReadOnlyList<GraphRecord> Records,
    long ExecutionTimeMs,
    int RecordsAffected,
    bool Success,
    string? ErrorMessage = null)
```

#### PatternSuggestion
```csharp
public readonly record struct PatternSuggestion(
    string Id,
    string ViolationId,
    string Title,
    string Description,
    string? CodeExample,
    float Confidence,
    SuggestionEffort Effort,
    SuggestionImpact Impact)
```

#### PatternAnalysis
```csharp
public readonly record struct PatternAnalysis(
    string PatternType,
    IReadOnlyList<PatternMatch> Matches,
    IReadOnlyList<PatternViolation> Violations,
    IReadOnlyList<PatternSuggestion> Suggestions,
    float Confidence,
    long AnalysisTimeMs)
```

#### PatternGraphResult
```csharp
public readonly record struct PatternGraphResult(
    IReadOnlyList<PatternDefinition> Patterns,
    IReadOnlyList<PatternRelationship> Relationships,
    long ExecutionTimeMs,
    int TotalResults,
    bool HasMoreResults)
```

### Enums

#### PatternSeverity
```csharp
public enum PatternSeverity
{
    Info,
    Warning,
    Error
}
```

#### SuggestionEffort
```csharp
public enum SuggestionEffort
{
    Low,
    Medium,
    High,
    VeryHigh
}
```

#### SuggestionImpact
```csharp
public enum SuggestionImpact
{
    Low,
    Medium,
    High,
    VeryHigh
}
```

#### UsageTrend
```csharp
public enum UsageTrend
{
    Unknown,
    Increasing,
    Stable,
    Decreasing
}
```

#### PatternChangeType
```csharp
public enum PatternChangeType
{
    Created,
    Updated,
    Deprecated,
    Removed
}
```

## Error Handling

All services use the `Result<T>` pattern for error handling, avoiding exceptions in normal control flow:

```csharp
var result = await service.MethodAsync(parameters, cancellationToken);
if (result.IsFailure)
{
    // Handle error
    Console.WriteLine($"Error: {result.Error}");
    return;
}

// Use success result
var data = result.Value;
```

## Performance Considerations

- All async methods use `ConfigureAwait(false)` for background operations
- Cancellation tokens are supported throughout the API
- Query timeouts are configurable for long-running operations
- Results are paginated for large datasets

## Testing

The Graph RAG Layer includes comprehensive test coverage:

- **ITDD Tests**: Interface contract validation with mocks
- **TDD Tests**: Real implementation validation
- **Integration Tests**: End-to-end workflow validation
- **Performance Tests**: Response time validation
- **Error Handling Tests**: Failure scenario validation

## Dependencies

- `IndQuestResults`: Result pattern implementation
- `Microsoft.Extensions.Logging`: Structured logging
- `Microsoft.Extensions.DependencyInjection`: Dependency injection
- `NSubstitute`: Mocking framework for tests
- `Shouldly`: Assertion library for tests
- `xUnit`: Testing framework
