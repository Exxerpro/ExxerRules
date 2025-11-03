namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Common test base providing solution paths and per-test temp directories.
/// </summary>
public abstract class TestBase : IAsyncLifetime
{
    /// <summary>
    /// Absolute path to the solution file used for locating test assets.
    /// </summary>
    protected static readonly string SolutionPath = TestUtilities.GetSolutionPath();

    private static readonly string TestProjectDirectory = TestUtilities.GetTestProjectDirectory();

    /// <summary>
    /// Absolute path to the shared example C# file used in tests.
    /// </summary>
    protected static readonly string ExampleFilePath = Path.Combine(TestProjectDirectory, "ExampleCode.cs");

    private static readonly string TestOutputRoot = Path.Combine(TestProjectDirectory, "TestOutput");

    static TestBase()
    {
        TestUtilities.EnsureExampleCodeFile();
        Directory.CreateDirectory(TestOutputRoot);
    }

    /// <summary>
    /// TestOutputPath.
    /// </summary>
    protected string TestOutputPath { get; }

    /// <summary>
    /// Initializes a new temp output directory for the test instance.
    /// </summary>
    protected TestBase()
    {
        TestOutputPath = Path.Combine(TestOutputRoot, Guid.NewGuid().ToString());
        Directory.CreateDirectory(TestOutputPath);
    }

    /// <summary>
    /// Initializes test resources asynchronously.
    /// </summary>
    public virtual ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Cleans up test resources asynchronously with retry logic for file locks.
    /// </summary>
    public virtual async ValueTask DisposeAsync()
    {
        await CleanupTestOutputAsync();
    }

    private async Task CleanupTestOutputAsync()
    {
        if (!Directory.Exists(TestOutputPath)) return;

        // Try multiple approaches to handle file locks with async delays
        for (int attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                Directory.Delete(TestOutputPath, true);
                return; // Success
            }
            catch (IOException) when (attempt < 4)
            {
                // Wait and retry - files might be locked
                await Task.Delay(200 * (attempt + 1));
            }
            catch (UnauthorizedAccessException) when (attempt < 4)
            {
                // Try to reset file attributes and retry
                try
                {
                    await ResetFileAttributesAsync(TestOutputPath);
                    await Task.Delay(100);
                }
                catch { }
            }
            catch
            {
                // Other exceptions - give up
                break;
            }
        }

        // Final attempt - force cleanup
        try
        {
            if (Directory.Exists(TestOutputPath))
            {
                await ForceDeleteDirectoryAsync(TestOutputPath);
            }
        }
        catch
        {
            // If all cleanup attempts fail, at least log the issue
            System.Diagnostics.Debug.WriteLine($"Failed to clean up test directory: {TestOutputPath}");
        }
    }

    private async Task ResetFileAttributesAsync(string directory)
    {
        try
        {
            var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    await Task.Delay(1); // Small delay to prevent overwhelming the file system
                }
                catch { }
            }
        }
        catch { }
    }

    private async Task ForceDeleteDirectoryAsync(string directory)
    {
        try
        {
            // Force delete using directory enumeration
            var subdirs = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);
            foreach (var subdir in subdirs)
            {
                await ForceDeleteDirectoryAsync(subdir);
            }

            var files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                    await Task.Delay(1);
                }
                catch { }
            }

            Directory.Delete(directory, false);
        }
        catch { }
    }
}
