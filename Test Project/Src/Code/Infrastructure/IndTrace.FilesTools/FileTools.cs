// <copyright file="FileTools.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.FilesTools;

using IndTrace.FilesTools.Services;

/// <summary>
/// Simple static entry point for file tools operations.
/// Provides the easiest way to load embedded data without complex setup.
/// </summary>
public static class FileTools
{
    private static EmbeddedDataLoader? defaultLoader;
    private static readonly object Lock = new();

    /// <summary>
    /// Gets the default embedded data loader instance.
    /// </summary>
    private static EmbeddedDataLoader DefaultLoader
    {
        get
        {
            if (defaultLoader == null)
            {
                lock (Lock)
                {
                    defaultLoader ??= new EmbeddedDataLoader();
                }
            }

            return defaultLoader;
        }
    }

    /// <summary>
    /// Loads a list of objects from an embedded JSON file.
    /// This is the primary method for loading test data.
    /// </summary>
    /// <typeparam name="T">The type of objects to load.</typeparam>
    /// <param name="fileName">The JSON file name (e.g., "Rules.json", "Machines.json").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of deserialized objects, or an empty list if the file was not found.</returns>
    /// <example>
    /// <code>
    /// var rules = await FileTools.LoadListAsync&lt;Rule&gt;("Rules.json");
    /// var machines = await FileTools.LoadListAsync&lt;Machine&gt;("Machines.json");
    /// </code>
    /// </example>
    public static async Task<List<T>> LoadListAsync<T>(string fileName, CancellationToken cancellationToken = default)
        where T : class
    {
        var result = await DefaultLoader.LoadDataAsync<T>(fileName, cancellationToken);
        return result ?? new List<T>();
    }

    /// <summary>
    /// Loads a single object from an embedded JSON file.
    /// </summary>
    /// <typeparam name="T">The type of object to load.</typeparam>
    /// <param name="fileName">The JSON file name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deserialized object, or null if the file was not found.</returns>
    public static async Task<T?> LoadSingleAsync<T>(string fileName, CancellationToken cancellationToken = default)
        where T : class
    {
        return await DefaultLoader.LoadSingleAsync<T>(fileName, cancellationToken);
    }

    /// <summary>
    /// Checks if an embedded file exists.
    /// </summary>
    /// <param name="fileName">The file name to check.</param>
    /// <returns>True if the file exists; otherwise, false.</returns>
    public static async Task<bool> ExistsAsync(string fileName)
    {
        return await DefaultLoader.ExistsAsync(fileName);
    }

    /// <summary>
    /// Gets the raw content of an embedded file.
    /// </summary>
    /// <param name="fileName">The file name to read.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The file content, or null if the file was not found.</returns>
    public static async Task<string?> GetContentAsync(string fileName, CancellationToken cancellationToken = default)
    {
        return await DefaultLoader.GetRawContentAsync(fileName, cancellationToken);
    }

    /// <summary>
    /// Gets diagnostic information about the file tools system.
    /// Useful for troubleshooting embedded resource issues.
    /// </summary>
    /// <returns>Diagnostic information including available files and configuration.</returns>
    public static Dictionary<string, object> GetDiagnosticInfo()
    {
        return DefaultLoader.GetDiagnosticInfo();
    }

    /// <summary>
    /// Creates a new data loader with custom configuration.
    /// Use this for advanced scenarios where you need custom JSON options or file locators.
    /// </summary>
    /// <param name="jsonOptions">Custom JSON serializer options.</param>
    /// <returns>A new configured data loader.</returns>
    public static EmbeddedDataLoader CreateCustomLoader(JsonSerializerOptions? jsonOptions = null)
    {
        return new EmbeddedDataLoader(jsonOptions: jsonOptions);
    }

    /// <summary>
    /// Resets the default loader (useful for testing).
    /// </summary>
    internal static void ResetForTesting()
    {
        lock (Lock)
        {
            defaultLoader = null;
        }
    }
}
