// <copyright file="EmbeddedResourceFileLocator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.FilesTools.Strategies;

using IndTrace.FilesTools.Interfaces;

/// <summary>
/// File locator that finds files embedded as assembly resources.
/// This is the most reliable strategy - zero external dependencies, always works.
/// </summary>
public class EmbeddedResourceFileLocator : IFileLocator
{
    private readonly Assembly assembly;
    private readonly string resourceNamespace;
    private readonly Dictionary<string, string> resourceMap;

    /// <summary>
    /// Gets the name of this locator strategy.
    /// </summary>
    public string StrategyName => "EmbeddedResource";

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedResourceFileLocator"/> class.
    /// Initializes a new instance of the embedded resource file locator.
    /// </summary>
    /// <param name="assembly">The assembly containing the embedded resources. If null, uses calling assembly.</param>
    /// <param name="resourceNamespace">The namespace prefix for embedded resources. If null, auto-detects.</param>
    public EmbeddedResourceFileLocator(Assembly? assembly = null, string? resourceNamespace = null)
    {
        this.assembly = assembly ?? Assembly.GetCallingAssembly();
        this.resourceNamespace = resourceNamespace ?? this.DetectResourceNamespace();
        this.resourceMap = this.BuildResourceMap();
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

        return this.resourceMap.ContainsKey(fileName.ToLowerInvariant());
    }

    /// <summary>
    /// Gets the file content as a stream.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Stream?> GetFileStreamAsync(string fileName, CancellationToken cancellationToken = default)
    {
        if (!this.CanHandle(fileName))
        {
            return null;
        }

        var resourceName = this.resourceMap[fileName.ToLowerInvariant()];
        var stream = this.assembly.GetManifestResourceStream(resourceName);

        return await Task.FromResult(stream);
    }

    /// <summary>
    /// Gets the file content as text.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<string?> GetFileContentAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var stream = await this.GetFileStreamAsync(fileName, cancellationToken);
        if (stream == null)
        {
            return null;
        }

        using (stream)
        using (var reader = new StreamReader(stream))
        {
            return await reader.ReadToEndAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Checks if the file exists as an embedded resource.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<bool> ExistsAsync(string fileName)
    {
        return await Task.FromResult(this.CanHandle(fileName));
    }

    /// <summary>
    /// Gets all available embedded resource file names.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetAvailableFiles()
    {
        return this.resourceMap.Keys;
    }

    /// <summary>
    /// Builds a map of file names to their full resource names for fast lookup.
    /// </summary>
    private Dictionary<string, string> BuildResourceMap()
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var resourceNames = this.assembly.GetManifestResourceNames();

        foreach (var resourceName in resourceNames)
        {
            // Extract just the filename from the full resource name
            // Example: "IndTrace.FilesTools.Data.Rules.json" -> "Rules.json"
            var fileName = Path.GetFileName(resourceName);

            // Handle cases where the resource name doesn't have a clear filename
            if (string.IsNullOrEmpty(fileName) && resourceName.Contains('.'))
            {
                var parts = resourceName.Split('.');
                if (parts.Length >= 2)
                {
                    // Take last two parts as filename if it looks like "name.extension"
                    var lastTwo = string.Join(".", parts.TakeLast(2));
                    if (lastTwo.Contains('.') && !lastTwo.StartsWith('.'))
                    {
                        fileName = lastTwo;
                    }
                }
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                map[fileName.ToLowerInvariant()] = resourceName;
            }
        }

        return map;
    }

    /// <summary>
    /// Auto-detects the resource namespace from the assembly.
    /// </summary>
    private string DetectResourceNamespace()
    {
        var assemblyName = this.assembly.GetName().Name;
        return assemblyName ?? "IndTrace.FilesTools";
    }

    /// <summary>
    /// Gets diagnostic information about available resources.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GetDiagnosticInfo()
    {
        return new Dictionary<string, object>
        {
            ["Assembly"] = this.assembly.GetName().Name ?? "Unknown",
            ["ResourceNamespace"] = this.resourceNamespace,
            ["AvailableResources"] = this.assembly.GetManifestResourceNames(),
            ["MappedFiles"] = this.resourceMap.Keys.ToList(),
            ["TotalResourcesCount"] = this.assembly.GetManifestResourceNames().Length,
            ["MappedFilesCount"] = this.resourceMap.Count,
        };
    }
}
