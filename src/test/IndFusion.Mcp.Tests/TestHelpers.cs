namespace IndFusion.Mcp.Tests;

public static class TestHelpers
{
    public static string GetSolutionPath()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var dir = new DirectoryInfo(currentDir);
        while (dir != null)
        {
            var sln = Path.Combine(dir.FullName, "IndFusion.Mcp.sln");
            if (File.Exists(sln)) return sln;
            dir = dir.Parent;
        }
        return "./IndFusion.Mcp.sln";
    }

    public static string CreateTestOutputDir(string subfolder)
    {
        var path = Path.Combine(Path.GetDirectoryName(GetSolutionPath())!,
            "IndFusion.Mcp.Tests", "TestOutput", subfolder);
        Directory.CreateDirectory(path);
        return path;
    }
}
