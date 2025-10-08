// <copyright file="FixtureProfile.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Simulation;

using System.Text.Json;
using IndTrace.DataStore.Models;
using IndTrace.Simulator.Models.Constants;

/// <summary>
/// Represents a simulation profile for a fixture, including part numbers, machine names, test paths, and simulation parameters.
/// </summary>
public class FixtureProfile
{
    /// <summary>
    /// Gets or sets the list of part numbers to simulate.
    /// </summary>
    public List<string> PartNumbers { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of machine names involved in the simulation.
    /// </summary>
    public List<string> MachineNames { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of test path types to use in the simulation.
    /// </summary>
    public List<TestPathType> PathTypes { get; set; } = [];

    /// <summary>
    /// Gets or sets the number of barcodes per test path.
    /// </summary>
    public int BarcodesPerPath { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum number of retries for simulation steps.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the delay in milliseconds between simulation steps.
    /// </summary>
    public int DelayMs { get; set; } = 2000;

    /// <summary>
    /// Loads a <see cref="FixtureProfile"/> from a JSON file.
    /// </summary>
    /// <param name="path">The path to the JSON file. Defaults to "FixtureProfile.json".</param>
    /// <returns>The loaded <see cref="FixtureProfile"/> instance.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist at the specified path.</exception>
    public static FixtureProfile Load(string path = "FixtureProfile.json")
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Fixture profile not found at {path}");
        }

        var json = File.ReadAllText(path);
        var profile = JsonSerializer.Deserialize<FixtureProfile>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
        if (profile is null)
        {
            throw new InvalidOperationException("Failed to deserialize FixtureProfile from JSON file.");
        }

        return profile;
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate fixture profile logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
