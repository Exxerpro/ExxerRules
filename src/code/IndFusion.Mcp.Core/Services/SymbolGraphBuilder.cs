using IndQuestResults;
using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Build.Locator;
using System.Security.Cryptography;
using System.Text;

namespace IndFusion.Mcp.Core.Services;

/// <summary>
/// Service for building and maintaining symbol graphs from codebases.
/// Handles the creation and incremental updates of symbol graphs using Roslyn analysis.
/// </summary>
public class SymbolGraphBuilder : ISymbolGraphBuilder
{
	private readonly ILogger<SymbolGraphBuilder> _logger;

	/// <summary>
	/// Initializes a new instance of the SymbolGraphBuilder class.
	/// </summary>
	/// <param name="logger">Logger instance for this service.</param>
	public SymbolGraphBuilder(ILogger<SymbolGraphBuilder> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	/// <summary>
	/// Builds a complete symbol graph for the specified project path.
	/// </summary>
	/// <param name="projectPath">The path to the project to analyze.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the built symbol graph or failure information.</returns>
	public async Task<Result<SymbolGraph>> BuildAsync(
		string projectPath, 
		CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			if (string.IsNullOrWhiteSpace(projectPath))
			{
				_logger.LogWarning("Project path is null or empty");
				return Result<SymbolGraph>.WithFailure("Project path cannot be null or empty");
			}

			_logger.LogInformation("Building symbol graph for project: {ProjectPath}", projectPath);

			// Ensure MSBuild is available
			MSBuildLocator.RegisterDefaults();

			// Create MSBuild workspace
			using var workspace = MSBuildWorkspace.Create();
			
			// Open the project
			var project = await workspace.OpenProjectAsync(projectPath, progress: null, cancellationToken);
			
			// Get compilation
			var compilation = await project.GetCompilationAsync(cancellationToken);
			if (compilation == null)
			{
				_logger.LogError("Failed to get compilation for project: {ProjectPath}", projectPath);
				return Result<SymbolGraph>.WithFailure("Failed to compile project");
			}

			// Build symbol graph
			var nodes = new List<GraphNode>();
			var edges = new List<GraphEdge>();
			var nodeId = 0;

			// Process all symbols in the compilation
			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				cancellationToken.ThrowIfCancellationRequested();
				
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var root = await syntaxTree.GetRootAsync(cancellationToken);
				
				// Process all nodes in the syntax tree
				foreach (var node in root.DescendantNodes())
				{
					var symbol = semanticModel.GetDeclaredSymbol(node);
					if (symbol != null)
					{
						var graphNode = CreateGraphNode(symbol, nodeId++, syntaxTree.FilePath);
						nodes.Add(graphNode);
						
						// Create relationships
						var nodeEdges = CreateEdges(symbol, semanticModel, nodeId - 1);
						edges.AddRange(nodeEdges);
					}
				}
			}

			// Generate project hash
			var projectHash = GenerateProjectHash(projectPath, compilation);
			
			// Create symbol graph
			var symbolGraph = new SymbolGraph(
				ProjectPath: projectPath,
				ProjectHash: projectHash,
				Nodes: nodes,
				Edges: edges,
				CreatedAt: DateTime.UtcNow,
				LastUpdated: DateTime.UtcNow,
				Metadata: new Dictionary<string, object>
				{
					["TotalNodes"] = nodes.Count,
					["TotalEdges"] = edges.Count,
					["CompilationErrors"] = compilation.GetDiagnostics().Count(d => d.Severity == DiagnosticSeverity.Error)
				}
			);

			_logger.LogInformation("Built symbol graph with {NodeCount} nodes and {EdgeCount} edges for project: {ProjectPath}", 
				nodes.Count, edges.Count, projectPath);

			return Result<SymbolGraph>.Success(symbolGraph);
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Symbol graph building was cancelled for project: {ProjectPath}", projectPath);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error building symbol graph for project: {ProjectPath}", projectPath);
			return Result<SymbolGraph>.WithFailure($"Error building symbol graph: {ex.Message}");
		}
	}

	/// <summary>
	/// Updates an existing symbol graph with changes from specific files.
	/// </summary>
	/// <param name="graph">The existing symbol graph to update.</param>
	/// <param name="changedFiles">Collection of file paths that have changed.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result indicating success or failure of the update operation.</returns>
	public async Task<Result> UpdateAsync(
		SymbolGraph graph, 
		IReadOnlyCollection<string> changedFiles, 
		CancellationToken cancellationToken = default)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			if (graph == null)
			{
				_logger.LogWarning("Symbol graph is null");
				return Result.WithFailure("Symbol graph cannot be null");
			}

			if (changedFiles == null || changedFiles.Count == 0)
			{
				_logger.LogInformation("No changed files provided, skipping update");
				return Result.Success();
			}

			_logger.LogInformation("Updating symbol graph with {FileCount} changed files", changedFiles.Count);

			// For now, we'll rebuild the entire graph when files change
			// In a more sophisticated implementation, we would do incremental updates
			var rebuildResult = await BuildAsync(graph.ProjectPath, cancellationToken);
			
			if (rebuildResult.IsFailure)
			{
				_logger.LogError("Failed to rebuild symbol graph during update: {Error}", rebuildResult.Error);
				return Result.WithFailure($"Failed to update symbol graph: {rebuildResult.Error}");
			}

			_logger.LogInformation("Successfully updated symbol graph with {FileCount} changed files", changedFiles.Count);
			return Result.Success();
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Symbol graph update was cancelled");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating symbol graph");
			return Result.WithFailure($"Error updating symbol graph: {ex.Message}");
		}
	}

	/// <summary>
	/// Creates a graph node from a Roslyn symbol.
	/// </summary>
	/// <param name="symbol">The Roslyn symbol.</param>
	/// <param name="nodeId">The unique node identifier.</param>
	/// <param name="filePath">The file path where the symbol is defined.</param>
	/// <returns>A graph node representing the symbol.</returns>
	private GraphNode CreateGraphNode(ISymbol symbol, int nodeId, string filePath)
	{
		var location = symbol.Locations.FirstOrDefault();
		var sourceLocation = new Models.PatternGraph.SourceLocation(
			FilePath: filePath,
			StartLine: location?.GetLineSpan().StartLinePosition.Line + 1 ?? 0,
			StartColumn: location?.GetLineSpan().StartLinePosition.Character + 1 ?? 0,
			EndLine: location?.GetLineSpan().EndLinePosition.Line + 1 ?? 0,
			EndColumn: location?.GetLineSpan().EndLinePosition.Character + 1 ?? 0
		);

		var metadata = new Dictionary<string, object>
		{
			["SymbolKind"] = symbol.Kind.ToString(),
			["IsStatic"] = symbol.IsStatic,
			["IsAbstract"] = symbol.IsAbstract,
			["IsVirtual"] = symbol.IsVirtual,
			["IsOverride"] = symbol.IsOverride,
			["Accessibility"] = symbol.DeclaredAccessibility.ToString(),
			["ContainingSymbol"] = symbol.ContainingSymbol?.ToDisplayString() ?? "",
			["DocumentationComment"] = symbol.GetDocumentationCommentXml() ?? ""
		};

		return new GraphNode(
			Id: nodeId.ToString(),
			Type: GetSymbolType(symbol),
			Name: symbol.Name,
			FullName: symbol.ToDisplayString(),
			Location: sourceLocation,
			Metadata: metadata
		);
	}

	/// <summary>
	/// Creates edges representing relationships between symbols.
	/// </summary>
	/// <param name="symbol">The source symbol.</param>
	/// <param name="semanticModel">The semantic model for analysis.</param>
	/// <param name="sourceNodeId">The ID of the source node.</param>
	/// <returns>A collection of edges representing relationships.</returns>
	private List<GraphEdge> CreateEdges(ISymbol symbol, SemanticModel semanticModel, int sourceNodeId)
	{
		var edges = new List<GraphEdge>();
		var edgeId = 0;

		// Create inheritance relationships
		if (symbol is INamedTypeSymbol namedType)
		{
			if (namedType.BaseType != null)
			{
				edges.Add(new GraphEdge(
					Id: $"edge-{edgeId++}",
					SourceNodeId: sourceNodeId.ToString(),
					TargetNodeId: SymbolEqualityComparer.Default.GetHashCode(namedType.BaseType).ToString(),
					RelationshipType: "Inherits",
					Weight: 1.0,
					Metadata: new Dictionary<string, object> { ["RelationshipType"] = "Inheritance" }
				));
			}

			// Create interface implementation relationships
			foreach (var interfaceType in namedType.Interfaces)
			{
				edges.Add(new GraphEdge(
					Id: $"edge-{edgeId++}",
					SourceNodeId: sourceNodeId.ToString(),
					TargetNodeId: SymbolEqualityComparer.Default.GetHashCode(interfaceType).ToString(),
					RelationshipType: "Implements",
					Weight: 0.8,
					Metadata: new Dictionary<string, object> { ["RelationshipType"] = "InterfaceImplementation" }
				));
			}
		}

		// Create method call relationships (simplified)
		if (symbol is IMethodSymbol method)
		{
			// This is a simplified approach - in a real implementation, we would analyze method bodies
			// to find actual method calls and create edges for them
		}

		return edges;
	}

	/// <summary>
	/// Gets a simplified symbol type for the graph node.
	/// </summary>
	/// <param name="symbol">The Roslyn symbol.</param>
	/// <returns>A string representing the symbol type.</returns>
	private string GetSymbolType(ISymbol symbol)
	{
		return symbol.Kind switch
		{
			SymbolKind.NamedType => symbol is INamedTypeSymbol namedType ? namedType.TypeKind.ToString() : "Type",
			SymbolKind.Method => "Method",
			SymbolKind.Property => "Property",
			SymbolKind.Field => "Field",
			SymbolKind.Event => "Event",
			SymbolKind.Parameter => "Parameter",
			SymbolKind.Local => "Local",
			SymbolKind.Namespace => "Namespace",
			SymbolKind.Assembly => "Assembly",
			_ => symbol.Kind.ToString()
		};
	}

	/// <summary>
	/// Generates a hash for the project based on its content and dependencies.
	/// </summary>
	/// <param name="projectPath">The path to the project.</param>
	/// <param name="compilation">The compilation result.</param>
	/// <returns>A hash string representing the project state.</returns>
	private string GenerateProjectHash(string projectPath, Compilation compilation)
	{
		var hashInput = new StringBuilder();
		hashInput.AppendLine(projectPath);
		hashInput.AppendLine(compilation.AssemblyName);
		
		// Include file paths and their last modified times
		foreach (var syntaxTree in compilation.SyntaxTrees)
		{
			hashInput.AppendLine(syntaxTree.FilePath);
			if (File.Exists(syntaxTree.FilePath))
			{
				hashInput.AppendLine(File.GetLastWriteTime(syntaxTree.FilePath).ToString("O"));
			}
		}

		// Include referenced assembly names
		foreach (var reference in compilation.References)
		{
			if (reference is PortableExecutableReference peRef)
			{
				hashInput.AppendLine(peRef.FilePath ?? peRef.Display);
			}
		}

		using var sha256 = SHA256.Create();
		var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashInput.ToString()));
		return Convert.ToBase64String(hashBytes)[..16]; // Use first 16 characters
	}
}
