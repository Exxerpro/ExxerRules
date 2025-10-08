// <copyright file="MockFixtureExporter.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Mocks;

using IndTrace.DataStore.Interfaces;
using IndTrace.Simulator.Export;

/// <summary>
/// Mock implementation of <see cref="IFixtureExporter"/> for dry-run and testing scenarios.
/// </summary>
/// <remarks>
/// This exporter logs export actions instead of performing real exports.
/// </remarks>
public class MockFixtureExporter(ILogger<MockFixtureExporter> logger) : IFixtureExporter
{
    /// <summary>
    /// Simulates exporting the specified fixture context by logging the export actions.
    /// </summary>
    /// <param name="context">The fixture context to export.</param>
    /// <returns>A completed task representing the asynchronous operation.</returns>
    public Task ExportAsync(IFixtureContext context)
    {
        // Simulate exporting fixture context
        logger.LogInformation("[DRY-RUN] Exporting FixtureContext for PartNumber '{PartNumber}'", context.PartNumber);
        foreach (var task in context.Tasks)
        {
            logger.LogInformation("[DRY-RUN] Exporting Task: {Task}", task);
        }

        return Task.CompletedTask;
    }
}
