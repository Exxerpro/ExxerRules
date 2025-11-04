namespace IndFusion.SemanticRag.System.Tests.Infrastructure.Utilities;

/// <summary>
/// Utility class for providing skip conditions for tests when Docker is unavailable.
/// System tests require real containers to be meaningful, so tests should skip gracefully when Docker is unavailable.
/// </summary>
public static class DockerSkipConditions
{
	/// <summary>
	/// Gets a value indicating whether tests should be skipped due to Docker unavailability.
	/// This is determined by checking if the fixture's IsAvailable property is false.
	/// </summary>
	/// <returns><c>true</c> when tests should be skipped; otherwise, <c>false</c>.</returns>
	public static bool ShouldSkipDockerTests { get; set; } = false;
}

