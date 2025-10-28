using IndQuestResults;
using IndFusion.Mcp.Core.Models.PatternGraph;

namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Service for building and maintaining symbol graphs from codebases.
/// Handles the creation and incremental updates of symbol graphs using Roslyn analysis.
/// </summary>
public interface ISymbolGraphBuilder
{
	/// <summary>
	/// Builds a complete symbol graph for the specified project path.
	/// </summary>
	/// <param name="projectPath">The path to the project to analyze.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result containing the built symbol graph or failure information.</returns>
	Task<Result<SymbolGraph>> BuildAsync(
		string projectPath, 
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Updates an existing symbol graph with changes from specific files.
	/// </summary>
	/// <param name="graph">The existing symbol graph to update.</param>
	/// <param name="changedFiles">Collection of file paths that have changed.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <returns>A result indicating success or failure of the update operation.</returns>
	Task<Result> UpdateAsync(
		SymbolGraph graph, 
		IReadOnlyCollection<string> changedFiles, 
		CancellationToken cancellationToken = default);
}
