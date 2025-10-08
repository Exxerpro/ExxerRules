// <copyright file="LegacyFileSystemLocator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.FilesTools.Legacy;

using IndTrace.FilesTools.Interfaces;

/// <summary>
/// Legacy file system-based locator with multiple search strategies.
/// OBSOLETE: Use EmbeddedResourceFileLocator for reliability.
/// Preserved for reference and backward compatibility only.
/// </summary>
[Obsolete("Use EmbeddedResourceFileLocator instead. File system-based location is unreliable across different environments.")]
public class LegacyFileSystemLocator : IFileLocator
{
    private readonly string[] searchPaths;

    /// <summary>
    /// Gets the name of this locator strategy.
    /// </summary>
    public string StrategyName => "LegacyFileSystem";

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyFileSystemLocator"/> class.
    /// Initializes a new instance of the legacy file system locator.
    /// </summary>
    /// <param name="searchPaths">Optional custom search paths. If null, uses default discovery.</param>
    [Obsolete("Use EmbeddedResourceFileLocator instead.")]
    public LegacyFileSystemLocator(params string[] searchPaths)
    {
        this.searchPaths = searchPaths?.Length > 0 ? searchPaths : this.DiscoverSearchPaths();
    }

    /// <summary>
    /// Determines if this locator can handle the specified file.
    /// </summary>
    /// <returns></returns>
    public bool CanHandle(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        return this.searchPaths.Any(path =>
        {
            var fullPath = Path.Combine(path, fileName);
            return File.Exists(fullPath);
        });
    }

    /// <summary>
    /// Gets the file content as a stream.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Stream?> GetFileStreamAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = await this.FindFilePathAsync(fileName);
        if (filePath == null)
        {
            return null;
        }

        return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    /// <summary>
    /// Gets the file content as text.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<string?> GetFileContentAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = await this.FindFilePathAsync(fileName);
        if (filePath == null)
        {
            return null;
        }

        return await File.ReadAllTextAsync(filePath, cancellationToken);
    }

    /// <summary>
    /// Checks if the file exists.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<bool> ExistsAsync(string fileName)
    {
        var filePath = await this.FindFilePathAsync(fileName);
        return filePath != null;
    }

    /// <summary>
    /// Finds the full path to the specified file.
    /// </summary>
    private async Task<string?> FindFilePathAsync(string fileName)
    {
        foreach (var searchPath in this.searchPaths)
        {
            var fullPath = Path.Combine(searchPath, fileName);
            if (File.Exists(fullPath))
            {
                return await Task.FromResult(fullPath);
            }
        }

        return null;
    }

    /// <summary>
    /// Discovers search paths using various strategies.
    /// LEGACY: These strategies were problematic due to environment differences.
    /// </summary>
    private string[] DiscoverSearchPaths()
    {
        var paths = new List<string>();

        // Strategy 1: Environment variable
        var envPath = Environment.GetEnvironmentVariable("INDTRACE_DATA_PATH");
        if (!string.IsNullOrEmpty(envPath) && Directory.Exists(envPath))
        {
            paths.Add(envPath);
        }

        // Strategy 2: Current directory and parents
        var current = Directory.GetCurrentDirectory();
        paths.Add(current);
        paths.Add(Path.Combine(current, "SeedDataFiles"));
        paths.Add(Path.Combine(current, "Data"));

        var parent = Directory.GetParent(current);
        while (parent != null && paths.Count < 10) // Safety limit
        {
            paths.Add(Path.Combine(parent.FullName, "SeedDataFiles"));
            paths.Add(Path.Combine(parent.FullName, "Databases", "SeedData"));
            parent = parent.Parent;
        }

        // Strategy 3: Application base directory
        var baseDir = AppContext.BaseDirectory;
        if (!string.IsNullOrEmpty(baseDir))
        {
            paths.Add(Path.Combine(baseDir, "SeedDataFiles"));
            paths.Add(Path.Combine(baseDir, "Data"));
        }

        return paths.Where(Directory.Exists).Distinct().ToArray();
    }

    /// <summary>
    /// Gets diagnostic information about search paths.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GetDiagnosticInfo()
    {
        return new Dictionary<string, object>
        {
            ["SearchPaths"] = this.searchPaths,
            ["ValidPaths"] = this.searchPaths.Where(Directory.Exists).ToArray(),
            ["CurrentDirectory"] = Directory.GetCurrentDirectory(),
            ["BaseDirectory"] = AppContext.BaseDirectory,
        };
    }
}
