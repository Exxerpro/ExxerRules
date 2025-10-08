// <copyright file="PlcCommandMap.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models.Constants;

/// <summary>
/// Provides mapping functionality for PLC command codes to their corresponding names.
/// </summary>
public static class PlcCommandMap
{
    /// <summary>
    /// Dictionary mapping command codes to their corresponding command names.
    /// </summary>
    public static readonly Dictionary<int, string> CommandNames = new()
    {
        { 4, "CreateBarCode" },
        { 8, "ReadBarCode" },
        { 16, "CreateCycle" },
        { 32, "UpdateCycleOk" },
        { 64, "UpdateCycleNotOk" },
    };

    /// <summary>
    /// Gets the command name for the specified command code.
    /// </summary>
    /// <param name="command">The command code to look up.</param>
    /// <returns>The command name if found; otherwise, returns "Unknown" with the command code.</returns>
    public static string GetCommandName(int command)
        => CommandNames.TryGetValue(command, out var name) ? name : $"Unknown ({command})";
}
