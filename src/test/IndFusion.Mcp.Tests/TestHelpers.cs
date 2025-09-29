namespace IndFusion.Mcp.Tests;

/// <summary>
/// Shared test utilities for locating the solution and managing test output paths.
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Walks up from the current directory to find the repository solution path.
    /// </summary>
    public static string GetSolutionPath()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var dir = new DirectoryInfo(currentDir);
        while (dir != null)
        {
            var primary = Path.Combine(dir.FullName, "src", "IndFusion.sln");
            if (File.Exists(primary))
            {
                return primary;
            }

            var legacy = Path.Combine(dir.FullName, "IndFusion.Mcp.sln");
            if (File.Exists(legacy))
            {
                return legacy;
            }

            dir = dir.Parent;
        }

        var fallback = Path.Combine(currentDir, "src", "IndFusion.sln");
        return Path.GetFullPath(fallback);
    }

    /// <summary>
    /// Creates (or ensures) a test output directory under the test project.
    /// </summary>
    public static string CreateTestOutputDir(string subfolder)
    {
        var path = Path.Combine(Path.GetDirectoryName(GetSolutionPath())!,
            "IndFusion.Mcp.Tests", "TestOutput", subfolder);
        Directory.CreateDirectory(path);
        return path;
    }
}
