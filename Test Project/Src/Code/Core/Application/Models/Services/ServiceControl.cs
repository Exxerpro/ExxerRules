// <copyright file="ServiceControl.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Services;

/// <summary>
/// Provides methods to control and manage system processes.
/// </summary>
public static class ServiceControl
{
    /// <summary>
    /// Determines whether a process with the specified name is currently running.
    /// </summary>
    /// <param name="processName">The name of the process to check.</param>
    /// <returns>True if the process is open; otherwise, false.</returns>
    public static bool IsProcessOpen(string processName)
    {
        return Process.GetProcesses().Any(clsProcess => clsProcess.ProcessName.Contains(processName));
    }

    /// <summary>
    /// Starts a process if it is not already running.
    /// </summary>
    /// <param name="processName">The name of the process to start.</param>
    /// <param name="fullName">The full path to the process executable (optional).</param>
    /// <param name="path">The directory path where the process executable is located (optional).</param>
    /// <returns>True if the process was started or is already running; otherwise, false.</returns>
    public static IndQuestResults.Result<bool> StartProcessIfNotOpen(string processName, string? fullName = null, string? path = null)
    {
        if (string.IsNullOrEmpty(processName))
        {
            return IndQuestResults.Result<bool>.WithFailure("processName cannot be null");
        }

        if (IsProcessOpen(processName))
        {
            return IndQuestResults.Result<bool>.Success(true);
        }

        if (string.IsNullOrEmpty(fullName) && string.IsNullOrEmpty(path))
        {
            return IndQuestResults.Result<bool>.WithFailure("a path must be provided");
        }

        if (!string.IsNullOrEmpty(fullName) && !File.Exists(fullName))
        {
            return IndQuestResults.Result<bool>.WithFailure($"process file {fullName} not found");
        }

        if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
        {
            return IndQuestResults.Result<bool>.WithFailure($"path {path} not found");
        }

        if (string.IsNullOrEmpty(path) && !File.Exists(fullName))
        {
            return IndQuestResults.Result<bool>.WithFailure($"process file {fullName} not found");
        }

        var fileName = string.IsNullOrEmpty(fullName) ? Path.Combine(path!, processName) : fullName;

        if (!File.Exists(fileName))
        {
            return IndQuestResults.Result<bool>.WithFailure($"process {fileName} not found");
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
            },
        };

        return IndQuestResults.Result<bool>.Success(process.Start());
    }
}
