// <copyright file="JsonFixtureExporter.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Export;

using System.Text.Json;
using IndTrace.DataStore.Interfaces;

/// <summary>
/// Exports fixture context data to a JSON file for simulation result storage.
/// </summary>
/// <param name="outputPath">The output file path for the JSON export. Defaults to "fixtures.json".</param>
public class JsonFixtureExporter(string outputPath = "fixtures.json") : IFixtureExporter
{
    private readonly Lock @lock = new();

    /// <summary>
    /// Exports the specified fixture context to a JSON file.
    /// </summary>
    /// <param name="context">The fixture context to export.</param>
    /// <returns>A completed task representing the asynchronous operation.</returns>
    public Task ExportAsync(IFixtureContext context)
    {
        lock (this.@lock)
        {
            var entry = JsonSerializer.Serialize(context);
            File.AppendAllText(outputPath, entry + Environment.NewLine);
        }

        return Task.CompletedTask;
    }
}
