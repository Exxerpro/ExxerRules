using System.Collections.Concurrent;

namespace IndFusion.Mcp.Tests.TestInfrastructure;

/// <summary>
/// Factory for creating and managing shared test solutions to avoid duplication
/// and ensure proper cleanup across test runs.
/// </summary>
public static class TestSolutionFactory
{
    private static readonly Lazy<string> _testSolutionPath = new(() => CreateTestSolution());
    private static readonly ConcurrentBag<string> _tempDirectories = [];
    private static readonly object _cleanupLock = new();

    /// <summary>
    /// Gets or creates the shared test solution path.
    /// </summary>
    /// <returns>The path to the shared test solution.</returns>
    public static string GetOrCreateTestSolution()
    {
        return _testSolutionPath.Value;
    }

    /// <summary>
    /// Gets the test project directory within the shared solution.
    /// </summary>
    /// <returns>The path to the test project directory.</returns>
    public static string GetTestProjectDirectory()
    {
        var solutionPath = GetOrCreateTestSolution();
        var solutionDir = Path.GetDirectoryName(solutionPath)!;
        return Path.Combine(solutionDir, "TestProject");
    }

    /// <summary>
    /// Creates a test file within the shared test solution project.
    /// </summary>
    /// <param name="fileName">The name of the test file (without extension).</param>
    /// <param name="content">The content of the test file.</param>
    /// <returns>The full path to the created test file.</returns>
    public static string CreateTestFile(string fileName, string content)
    {
        var projectDir = GetTestProjectDirectory();
        var filePath = Path.Combine(projectDir, $"{fileName}.cs");
        
        // Ensure the project directory exists
        Directory.CreateDirectory(projectDir);
        
        File.WriteAllText(filePath, content);
        return filePath;
    }

    /// <summary>
    /// Cleans up a test file created by CreateTestFile.
    /// </summary>
    /// <param name="filePath">The path to the test file to clean up.</param>
    public static void CleanupTestFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            // Log but don't throw - cleanup failures shouldn't break tests
            Console.WriteLine($"Warning: Failed to cleanup test file {filePath}: {ex.Message}");
        }
    }

    /// <summary>
    /// Registers a temporary directory for cleanup.
    /// </summary>
    /// <param name="directoryPath">The path to the temporary directory.</param>
    public static void RegisterTempDirectory(string directoryPath)
    {
        _tempDirectories.Add(directoryPath);
    }

    /// <summary>
    /// Cleans up all registered temporary directories and disposes shared resources.
    /// </summary>
    public static void CleanupAllTempDirectories()
    {
        lock (_cleanupLock)
        {
            foreach (var directory in _tempDirectories)
            {
                try
                {
                    if (Directory.Exists(directory))
                    {
                        Directory.Delete(directory, recursive: true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to cleanup temp directory {directory}: {ex.Message}");
                }
            }
            _tempDirectories.Clear();
        }
    }

    /// <summary>
    /// Performs complete cleanup including disposing shared resources.
    /// </summary>
    public static void CompleteCleanup()
    {
        CleanupAllTempDirectories();
        
        // Dispose MetricsProvider to prevent MemoryCache disposal errors
        try
        {
            IndFusion.Mcp.Core.Tools.MetricsProvider.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to dispose MetricsProvider: {ex.Message}");
        }
    }

    private static string CreateTestSolution()
    {
        // Create a shared test solution in the TestOutput directory
        var testOutputPath = Path.Combine(Path.GetTempPath(), "IndFusion", "TestOutput", "SharedTestSolution");
        var solutionPath = Path.Combine(testOutputPath, "SharedTestSolution.sln");
        var projectDir = Path.Combine(testOutputPath, "TestProject");
        var projectPath = Path.Combine(projectDir, "TestProject.csproj");
        
        // Clean up any existing files
        if (Directory.Exists(testOutputPath))
        {
            try
            {
                Directory.Delete(testOutputPath, recursive: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not clean existing test solution: {ex.Message}");
            }
        }
        
        Directory.CreateDirectory(projectDir);
        
        // Create a minimal, clean test project
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
                <LangVersion>latest</LangVersion>
              </PropertyGroup>
            </Project>
            """;
        File.WriteAllText(projectPath, projectContent);
        
        // Create a simple test file
        var testFilePath = Path.Combine(projectDir, "TestClass.cs");
        var testFileContent = """
            using System;
            using System.Threading.Tasks;

            namespace TestProject;

            public class TestClass
            {
                public string Name { get; set; } = "Test";
                
                public int ProcessValue(int value)
                {
                    return value * 2;
                }

                public async Task<string> GetDataAsync(CancellationToken cancellationToken = default)
                {
                    await Task.Delay(100, cancellationToken);
                    return "data";
                }
            }
            """;
        File.WriteAllText(testFilePath, testFileContent);
        
        // Create a clean solution file
        var solutionContent = """
            Microsoft Visual Studio Solution File, Format Version 12.00
            # Visual Studio Version 17
            VisualStudioVersion = 17.0.31903.59
            MinimumVisualStudioVersion = 10.0.40219.1
            Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "TestProject", "TestProject\\TestProject.csproj", "{12345678-1234-5678-9ABC-123456789ABC}"
            EndProject
            Global
                GlobalSection(SolutionConfigurationPlatforms) = preSolution
                    Debug|Any CPU = Debug|Any CPU
                EndGlobalSection
                GlobalSection(ProjectConfigurationPlatforms) = postSolution
                    {12345678-1234-5678-9ABC-123456789ABC}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                    {12345678-1234-5678-9ABC-123456789ABC}.Debug|Any CPU.Build.0 = Debug|Any CPU
                EndGlobalSection
            EndGlobal
            """;
        File.WriteAllText(solutionPath, solutionContent);
        
        // Register for cleanup
        RegisterTempDirectory(testOutputPath);
        
        return solutionPath;
    }
}
