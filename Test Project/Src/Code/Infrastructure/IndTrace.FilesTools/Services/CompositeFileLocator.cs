// <copyright file="CompositeFileLocator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.FilesTools.Services;

using IndTrace.FilesTools.Interfaces;

/// <summary>
/// Composite file locator that tries multiple strategies in order until one succeeds.
/// Useful for migration scenarios where you want embedded resources with fallback to legacy methods.
/// </summary>
public class CompositeFileLocator : IFileLocator
{
    private readonly IFileLocator[] locators;

    /// <summary>
    /// Gets the name of this locator strategy.
    /// </summary>
    public string StrategyName => $"Composite({string.Join(",", this.locators.Select(l => l.StrategyName))})";

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeFileLocator"/> class.
    /// Initializes a new instance of the composite file locator.
    /// </summary>
    /// <param name="locators">The locators to try, in order of preference.</param>
    public CompositeFileLocator(params IFileLocator[] locators)
    {
        this.locators = locators ?? throw new ArgumentNullException(nameof(locators));
        if (this.locators.Length == 0)
        {
            throw new ArgumentException("At least one locator must be provided.", nameof(locators));
        }
    }

    /// <summary>
    /// Determines if any locator can handle the specified file.
    /// </summary>
    /// <returns></returns>
    public bool CanHandle(string fileName)
    {
        return this.locators.Any(locator => locator.CanHandle(fileName));
    }

    /// <summary>
    /// Gets the file content as a stream using the first locator that can handle it.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Stream?> GetFileStreamAsync(string fileName, CancellationToken cancellationToken = default)
    {
        foreach (var locator in this.locators)
        {
            if (locator.CanHandle(fileName))
            {
                var stream = await locator.GetFileStreamAsync(fileName, cancellationToken);
                if (stream != null)
                {
                    return stream;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the file content as text using the first locator that can handle it.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<string?> GetFileContentAsync(string fileName, CancellationToken cancellationToken = default)
    {
        foreach (var locator in this.locators)
        {
            if (locator.CanHandle(fileName))
            {
                var content = await locator.GetFileContentAsync(fileName, cancellationToken);
                if (content != null)
                {
                    return content;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Checks if any locator can find the file.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<bool> ExistsAsync(string fileName)
    {
        foreach (var locator in this.locators)
        {
            if (await locator.ExistsAsync(fileName))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets diagnostic information about all locators.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GetDiagnosticInfo()
    {
        var info = new Dictionary<string, object>
        {
            ["LocatorCount"] = this.locators.Length,
            ["LocatorTypes"] = this.locators.Select(l => l.GetType().Name).ToArray(),
        };

        for (int i = 0; i < this.locators.Length; i++)
        {
            var locator = this.locators[i];
            var locatorInfo = new Dictionary<string, object>
            {
                ["StrategyName"] = locator.StrategyName,
                ["Type"] = locator.GetType().Name,
            };

            // Try to get diagnostic info if available
            if (locator is Strategies.EmbeddedResourceFileLocator embeddedLocator)
            {
                locatorInfo["Details"] = embeddedLocator.GetDiagnosticInfo();
            }

            // The legacy locator type is marked obsolete; this diagnostic-only check keeps backward compatibility.
            // Suppress CS0618 locally with justification to avoid behavior changes.
#pragma warning disable CS0618 // Using LegacyFileSystemLocator for diagnostics while migrating to EmbeddedResourceFileLocator
            else if (locator is Legacy.LegacyFileSystemLocator legacyLocator)
#pragma warning restore CS0618
            {
                locatorInfo["Details"] = legacyLocator.GetDiagnosticInfo();
            }

            info[$"Locator{i}"] = locatorInfo;
        }

        return info;
    }
}
