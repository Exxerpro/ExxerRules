// <copyright file="CsvFixtureStore.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Export;

using IndTrace.DataStore.Interfaces;
using JetBrains.Annotations;

/// <summary>
/// Stores fixture results in CSV format for simulation logging.
/// </summary>
public class CsvFixtureStore : IFixtureStore
{
    private readonly string path;
    private readonly Lock @lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvFixtureStore"/> class.
    /// </summary>
    /// <param name="path">The file path for the CSV output.</param>
    public CsvFixtureStore(string path)
    {
        this.path = path;
        if (!File.Exists(this.path))
        {
            File.AppendAllText(this.path, "PartNumber,ProductId,MachineId,TaskName,Barcode,State,Timestamp,Result,RetryCount,Notes\n");
        }
    }

    /// <summary>
    /// Logs a fixture result asynchronously to the CSV file.
    /// </summary>
    /// <param name="entry">The fixture log entry to log.</param>
    /// <returns>A completed task representing the asynchronous operation.</returns>
    public Task LogResultAsync(IFixtureLogEntry entry)
    {
        lock (this.@lock)
        {
            var line = string.Join(
                ",",
                entry.PartNumber,
                entry.ProductId,
                entry.StationId,
                entry.TaskName,
                entry.Barcode,
                entry.State,
                entry.Timestamp.ToString("o"),
                entry.Result,
                entry.RetryCount,
                entry.Notes.Replace(',', ';'));
            File.AppendAllText(this.path, line + "\n");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task SaveAsync(IFixtureContext context, string pathType, int? retryCount = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SaveAsync(IFixtureContext context, string pathType)
    {
        throw new NotImplementedException();
    }
}
