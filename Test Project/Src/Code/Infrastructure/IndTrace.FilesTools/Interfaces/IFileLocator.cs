// <copyright file="IFileLocator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.FilesTools.Interfaces;

/// <summary>
/// Contract for locating files across different storage strategies.
/// </summary>
public interface IFileLocator
{
    /// <summary>
    /// Gets the name/identifier of this locator strategy.
    /// </summary>
    string StrategyName { get; }

    /// <summary>
    /// Determines if this locator can handle the specified file.
    /// </summary>
    /// <param name="fileName">The file name to check.</param>
    /// <returns>True if this locator can handle the file; otherwise, false.</returns>
    bool CanHandle(string fileName);

    /// <summary>
    /// Locates and returns the content of the specified file.
    /// </summary>
    /// <param name="fileName">The file name to locate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The file content as a stream.</returns>
    Task<Stream?> GetFileStreamAsync(string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Locates and returns the content of the specified file as text.
    /// </summary>
    /// <param name="fileName">The file name to locate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The file content as text.</returns>
    Task<string?> GetFileContentAsync(string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the specified file exists and can be located.
    /// </summary>
    /// <param name="fileName">The file name to check.</param>
    /// <returns>True if the file exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(string fileName);
}
