// <copyright file="FileLocatorFactory.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.FilesTools.Services;

using IndTrace.FilesTools.Interfaces;
using IndTrace.FilesTools.Strategies;

/// <summary>
/// Factory for creating and managing file locator instances.
/// Provides a centralized way to get the appropriate locator strategy.
/// </summary>
public static class FileLocatorFactory
{
    private static IFileLocator? defaultLocator;
    private static readonly object Lock = new();

    /// <summary>
    /// Gets the default file locator (EmbeddedResourceFileLocator).
    /// </summary>
    public static IFileLocator Default
    {
        get
        {
            if (defaultLocator == null)
            {
                lock (Lock)
                {
                    defaultLocator ??= CreateEmbeddedResourceLocator();
                }
            }

            return defaultLocator;
        }
    }

    /// <summary>
    /// Creates a new embedded resource file locator.
    /// This is the recommended strategy for maximum reliability.
    /// </summary>
    /// <param name="assembly">The assembly containing embedded resources. If null, uses calling assembly.</param>
    /// <param name="resourceNamespace">The resource namespace. If null, auto-detects.</param>
    /// <returns>A new embedded resource file locator.</returns>
    public static IFileLocator CreateEmbeddedResourceLocator(Assembly? assembly = null, string? resourceNamespace = null)
    {
        return new EmbeddedResourceFileLocator(assembly, resourceNamespace);
    }

    /// <summary>
    /// Creates a legacy file system locator (OBSOLETE - for backward compatibility only).
    /// </summary>
    /// <param name="searchPaths">Optional search paths.</param>
    /// <returns>A legacy file system locator.</returns>
    [Obsolete("Use CreateEmbeddedResourceLocator instead. File system location is unreliable.")]
    public static IFileLocator CreateLegacyFileSystemLocator(params string[] searchPaths)
    {
        return new Legacy.LegacyFileSystemLocator(searchPaths);
    }

    /// <summary>
    /// Creates a composite locator that tries multiple strategies in order.
    /// </summary>
    /// <param name="locators">The locators to try, in order of preference.</param>
    /// <returns>A composite file locator.</returns>
    public static IFileLocator CreateCompositeLocator(params IFileLocator[] locators)
    {
        return new CompositeFileLocator(locators);
    }

    /// <summary>
    /// Sets a custom default locator for testing purposes.
    /// </summary>
    /// <param name="locator">The locator to use as default.</param>
    public static void SetDefaultForTesting(IFileLocator locator)
    {
        lock (Lock)
        {
            defaultLocator = locator;
        }
    }

    /// <summary>
    /// Resets the default locator to the standard embedded resource locator.
    /// </summary>
    public static void ResetToDefault()
    {
        lock (Lock)
        {
            defaultLocator = null;
        }
    }
}
