// <copyright file="EmbeddedDataLoader.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.FilesTools.Services;

using IndTrace.FilesTools.Interfaces;

/// <summary>
/// High-level data loader that combines file location with JSON deserialization.
/// Provides a simple API for loading typed data from embedded resources.
/// </summary>
public class EmbeddedDataLoader
{
    private readonly IFileLocator fileLocator;
    private readonly JsonSerializerOptions jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedDataLoader"/> class.
    /// Initializes a new instance of the embedded data loader.
    /// </summary>
    /// <param name="fileLocator">The file locator to use. If null, uses the default embedded resource locator.</param>
    /// <param name="jsonOptions">JSON serializer options. If null, uses default options.</param>
    public EmbeddedDataLoader(IFileLocator? fileLocator = null, JsonSerializerOptions? jsonOptions = null)
    {
        this.fileLocator = fileLocator ?? FileLocatorFactory.Default;
        this.jsonOptions = jsonOptions ?? CreateDefaultJsonOptions();
    }

    /// <summary>
    /// Loads and deserializes data from a JSON file.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="fileName">The JSON file name (e.g., "Rules.json").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deserialized data, or null if the file was not found.</returns>
    public async Task<List<T>?> LoadDataAsync<T>(string fileName, CancellationToken cancellationToken = default)
        where T : class
    {
        var jsonContent = await this.fileLocator.GetFileContentAsync(fileName, cancellationToken);
        if (string.IsNullOrEmpty(jsonContent))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<List<T>>(jsonContent, this.jsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to deserialize JSON from {fileName}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Loads and deserializes a single object from a JSON file.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="fileName">The JSON file name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deserialized object, or null if the file was not found.</returns>
    public async Task<T?> LoadSingleAsync<T>(string fileName, CancellationToken cancellationToken = default)
        where T : class
    {
        var jsonContent = await this.fileLocator.GetFileContentAsync(fileName, cancellationToken);
        if (string.IsNullOrEmpty(jsonContent))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(jsonContent, this.jsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to deserialize JSON from {fileName}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Checks if a file exists and can be loaded.
    /// </summary>
    /// <param name="fileName">The file name to check.</param>
    /// <returns>True if the file exists; otherwise, false.</returns>
    public async Task<bool> ExistsAsync(string fileName)
    {
        return await this.fileLocator.ExistsAsync(fileName);
    }

    /// <summary>
    /// Gets the raw JSON content of a file.
    /// </summary>
    /// <param name="fileName">The file name to read.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The raw JSON content, or null if the file was not found.</returns>
    public async Task<string?> GetRawContentAsync(string fileName, CancellationToken cancellationToken = default)
    {
        return await this.fileLocator.GetFileContentAsync(fileName, cancellationToken);
    }

    /// <summary>
    /// Gets diagnostic information about the data loader and its file locator.
    /// </summary>
    /// <returns>Diagnostic information.</returns>
    public Dictionary<string, object> GetDiagnosticInfo()
    {
        var info = new Dictionary<string, object>
        {
            ["FileLocatorStrategy"] = this.fileLocator.StrategyName,
            ["JsonOptions"] = new
            {
                PropertyNameCaseInsensitive = this.jsonOptions.PropertyNameCaseInsensitive,
                PropertyNamingPolicy = this.jsonOptions.PropertyNamingPolicy?.GetType().Name,
                WriteIndented = this.jsonOptions.WriteIndented,
            },
        };

        // Add file locator diagnostic info if available
        if (this.fileLocator is Strategies.EmbeddedResourceFileLocator embeddedLocator)
        {
            info["FileLocatorDetails"] = embeddedLocator.GetDiagnosticInfo();
        }

        // The legacy locator type is obsolete but still supported for compatibility; suppress CS0618 locally for diagnostics only.
#pragma warning disable CS0618 // LegacyFileSystemLocator referenced for diagnostics during migration
        else if (this.fileLocator is Legacy.LegacyFileSystemLocator legacyLocator)
#pragma warning restore CS0618
        {
            info["FileLocatorDetails"] = legacyLocator.GetDiagnosticInfo();
        }
        else if (this.fileLocator is CompositeFileLocator compositeLocator)
        {
            info["FileLocatorDetails"] = compositeLocator.GetDiagnosticInfo();
        }

        return info;
    }

    /// <summary>
    /// Creates default JSON serializer options optimized for IndTrace data.
    /// </summary>
    private static JsonSerializerOptions CreateDefaultJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };
    }
}
