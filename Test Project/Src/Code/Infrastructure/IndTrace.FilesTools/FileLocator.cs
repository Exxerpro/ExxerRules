// <copyright file="FileLocator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.FilesTools;

using IndTrace.Domain.Models;

/// <summary>
/// Provides file location utilities for finding files using various search strategies.
/// This is a general-purpose file location utility moved from Application layer.
/// </summary>
public static class FileLocator
{
    private const string SrcMarkerFolder = "Src";
    private static readonly Dictionary<string, string?> PathCache = new();

    /// <summary>
    /// Gets or sets the logger action for diagnostic output.
    /// </summary>
    public static Action<string>? Logger { get; set; }

    /// <summary>
    /// Finds the full path to a specific file using various search strategies.
    /// </summary>
    /// <param name="fileName">The name of the file to locate.</param>
    /// <param name="searchFolder">Optional folder name to search within.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The full file path as a Result.</returns>
    public static Task<Result<string>> FindFilePathAsync(
        string fileName,
        string? searchFolder = null,
        CancellationToken cancellationToken = default) =>
        LocateAsync(fileName, searchFolder, returnDirectory: false, cancellationToken);

    /// <summary>
    /// Finds the directory path containing a specific file.
    /// </summary>
    /// <param name="fileName">The name of the file to locate.</param>
    /// <param name="searchFolder">Optional folder name to search within.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The directory path as a Result.</returns>
    public static Task<Result<string>> FindFolderPathAsync(
        string fileName,
        string? searchFolder = null,
        CancellationToken cancellationToken = default) =>
        LocateAsync(fileName, searchFolder, returnDirectory: true, cancellationToken);

    /// <summary>
    /// Clears the internal path cache.
    /// </summary>
    public static void ClearCache()
    {
        PathCache.Clear();
    }

    private static async Task<Result<string>> LocateAsync(
        string fileName,
        string? searchFolder,
        bool returnDirectory,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result<string>.WithFailure("File name must be non-empty.");
        }

        try
        {
            // Check cache
            var cacheKey = $"{fileName}|{searchFolder ?? string.Empty}";
            if (PathCache.TryGetValue(cacheKey, out var cachedPath) && cachedPath != null)
            {
                return Result<string>.Success(returnDirectory ? Path.GetDirectoryName(cachedPath)! : cachedPath);
            }

            // Try various search strategies
            var dirResult = await ResolveFileLocationAsync(fileName, searchFolder, cancellationToken).ConfigureAwait(false);
            if (dirResult.IsFailure || dirResult.Value is null)
            {
                return Result<string>.WithFailure(dirResult.Errors);
            }

            var fullPath = dirResult.Value;

            // Cache the result
            PathCache[cacheKey] = fullPath;

            return Result<string>.Success(returnDirectory ? Path.GetDirectoryName(fullPath)! : fullPath);
        }
        catch (Exception ex)
        {
            return Result<string>.WithFailure($"Error locating file '{fileName}': {ex.Message}");
        }
    }

    private static async Task<Result<string>> ResolveFileLocationAsync(
        string fileName,
        string? searchFolder,
        CancellationToken cancellationToken)
    {
        var strategies = new Func<Task<string?>>[]
        {
            // Try environment variable first
            () => TryPathAsync(Environment.GetEnvironmentVariable("FILE_SEARCH_PATH"), searchFolder, fileName),

            // Try current directory
            () => TryPathAsync(Directory.GetCurrentDirectory(), searchFolder, fileName),

            // Try app base directory
            () => TryPathAsync(AppContext.BaseDirectory, searchFolder, fileName),

            // Try recursive search from current directory
            () => TryRecursiveSearchAsync(Directory.GetCurrentDirectory(), searchFolder, fileName),

            // Try traversing up to find Src folder
            () => TryTraverseUpToFindSrcAsync(fileName, searchFolder),

            // Try common locations
            () => TryCommonLocationsAsync(fileName, searchFolder),
        };

        foreach (var strategy in strategies)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await strategy().ConfigureAwait(false);
            if (result is not null)
            {
                return Result<string>.Success(result);
            }
        }

        return Result<string>.WithFailure($"File '{fileName}' not found using any search strategy.");
    }

    private static async Task<string?> TryPathAsync(string? basePath, string? searchFolder, string fileName)
    {
        if (string.IsNullOrWhiteSpace(basePath) || !Directory.Exists(basePath))
        {
            return null;
        }

        var searchPath = searchFolder != null
            ? Path.Combine(basePath, searchFolder)
            : basePath;

        if (!Directory.Exists(searchPath))
        {
            return null;
        }

        Logger?.Invoke($"Checking path: {searchPath}");

        var filePath = Path.Combine(searchPath, fileName);
        return await Task.FromResult(File.Exists(filePath) ? filePath : null);
    }

    private static async Task<string?> TryRecursiveSearchAsync(string basePath, string? searchFolder, string fileName)
    {
        if (!Directory.Exists(basePath))
        {
            return null;
        }

        Logger?.Invoke($"Recursively searching in: {basePath}");

        try
        {
            var searchPattern = fileName;
            var files = Directory.EnumerateFiles(basePath, searchPattern, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (searchFolder == null)
                {
                    return await Task.FromResult(file);
                }

                // Check if file is within the search folder
                var directory = Path.GetDirectoryName(file);
                if (directory != null && directory.Contains(searchFolder))
                {
                    return await Task.FromResult(file);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Skip directories we can't access
        }

        return null;
    }

    private static async Task<string?> TryTraverseUpToFindSrcAsync(string fileName, string? searchFolder)
    {
        string? current = AppContext.BaseDirectory;

        while (!string.IsNullOrEmpty(current))
        {
            // Look for Src folder
            string srcPath = Path.Combine(current, SrcMarkerFolder);
            if (Directory.Exists(srcPath))
            {
                var result = await TryRecursiveSearchAsync(srcPath, searchFolder, fileName);
                if (result != null)
                {
                    Logger?.Invoke($"Found by traversing to Src: {result}");
                    return result;
                }
            }

            // Also check current directory
            if (searchFolder != null)
            {
                var searchPath = Path.Combine(current, searchFolder);
                if (Directory.Exists(searchPath))
                {
                    var filePath = Path.Combine(searchPath, fileName);
                    if (File.Exists(filePath))
                    {
                        return filePath;
                    }
                }
            }

            current = Directory.GetParent(current)?.FullName;
        }

        return null;
    }

    private static async Task<string?> TryCommonLocationsAsync(string fileName, string? searchFolder)
    {
        var commonPaths = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), searchFolder ?? string.Empty),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), searchFolder ?? string.Empty),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), searchFolder ?? string.Empty),
        };

        foreach (var basePath in commonPaths)
        {
            if (!Directory.Exists(basePath))
            {
                continue;
            }

            var filePath = Path.Combine(basePath, fileName);
            if (File.Exists(filePath))
            {
                return await Task.FromResult(filePath);
            }
        }

        return null;
    }
}

// [Fix] CLAUDE - Date: 26/08/2025
// Reason: Moved DataFileLocator to IndTrace.FilesTools as a general-purpose FileLocator utility
// Enhanced with caching and more flexible search options
