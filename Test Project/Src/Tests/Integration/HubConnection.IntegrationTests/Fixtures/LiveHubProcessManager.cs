namespace HubConnection.IntegrationTests.Fixtures;

using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Meziantou.Extensions.Logging.Xunit.v3;
using System.IO;
using IndTrace.FilesTools;

/// <summary>
/// Starts the real Hub server out-of-process when required by tests/config.
/// Default behavior is enabled via RealHub:AutoStart = true.
/// Attempts to run: dotnet run -c Release -p {ProjectPath} --urls {BaseUrl}
/// </summary>
public static class LiveHubProcessManager
{
    private static readonly object Sync = new();
    private static Process? _process;
    private static string? _url;
    private static readonly ILogger Logger = XUnitLogger.CreateLogger<object>();

    public static async Task<bool> EnsureStartedAsync(string? hubUrl, string? projectPath, int startTimeoutSeconds, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(hubUrl) || string.IsNullOrWhiteSpace(projectPath))
        {
            return false;
        }

        // Normalize base URL (remove trailing /segment if present)
        var baseUrl = hubUrl;
        if (hubUrl.Contains('/'))
        {
            try
            {
                var uri = new Uri(hubUrl, UriKind.Absolute);
                baseUrl = $"{uri.Scheme}://{uri.Host}:{uri.Port}";
            }
            catch
            {
                // leave as-is
            }
        }

        // 1) Quick pre-check: port/path reachable already?
        if (await WaitUntilReachableAsync(hubUrl, TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false))
        {
            Logger.LogInformation("Hub already reachable at {HubUrl}; auto-start not required", hubUrl);
            return true;
        }

        // 2) Resolve project path robustly
        var fullProjectPath = await ResolveProjectPathAsync(projectPath, cancellationToken).ConfigureAwait(false);
        if (fullProjectPath is null)
        {
            Logger.LogError("Hub project not found at path: {ProjectPath}. Auto-start aborted.", projectPath);
            return false;
        }

        lock (Sync)
        {
            if (_process != null && !_process.HasExited)
            {
                _url = baseUrl;
                return true;
            }
        }

        try
        {
            Logger.LogInformation("Auto-starting Hub server: {ProjectPath} at {BaseUrl}", fullProjectPath, baseUrl);
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run -c Release -p \"{fullProjectPath}\" --urls {baseUrl}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(fullProjectPath) ?? Environment.CurrentDirectory,
            };

            var proc = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
            proc.OutputDataReceived += (_, e) => { if (e.Data is not null) Logger.LogDebug(e.Data); };
            proc.ErrorDataReceived += (_, e) => { if (e.Data is not null) Logger.LogWarning(e.Data); };

            if (!proc.Start())
            {
                Logger.LogError("Failed to start hub process");
                return false;
            }

            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            lock (Sync)
            {
                _process = proc;
                _url = baseUrl;
            }

            AppDomain.CurrentDomain.ProcessExit += (_, __) => SafeKill();

            // Wait for readiness by probing negotiate endpoint
            var ready = await WaitUntilReachableAsync(hubUrl, TimeSpan.FromSeconds(startTimeoutSeconds), cancellationToken).ConfigureAwait(false);
            if (!ready)
            {
                Logger.LogError("Hub did not become ready within {Timeout}s", startTimeoutSeconds);
                return false;
            }

            Logger.LogInformation("Hub server is ready at {HubUrl}", hubUrl);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error auto-starting Hub server");
            return false;
        }
    }

    private static async Task<string?> ResolveProjectPathAsync(string projectPath, CancellationToken cancellationToken)
    {
        try
        {
            if (File.Exists(projectPath))
            {
                return Path.GetFullPath(projectPath);
            }

            // DRY: use shared FileLocator from IndTrace.FilesTools
            var fileName = Path.GetFileName(projectPath);
            var result = await FileLocator.FindFilePathAsync(fileName, "Src", cancellationToken).ConfigureAwait(false);
            if (result.IsSuccess && result.Value is not null)
            {
                return result.Value;
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to resolve project path: {ProjectPath}", projectPath);
        }
        return null;
    }

    private static async Task<bool> WaitUntilReachableAsync(string hubUrl, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var negotiate = hubUrl.TrimEnd('/') + "/negotiate?negotiateVersion=1";
        var sw = Stopwatch.StartNew();
        using var client = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
        { Timeout = TimeSpan.FromSeconds(2) };

        while (sw.Elapsed < timeout && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var resp = await client.GetAsync(negotiate, cancellationToken).ConfigureAwait(false);
                if (resp.IsSuccessStatusCode) return true;
            }
            catch
            {
                // swallow and retry
            }

            await Task.Delay(500, cancellationToken).ConfigureAwait(false);
        }

        return false;
    }

    private static void SafeKill()
    {
        lock (Sync)
        {
            try
            {
                if (_process != null && !_process.HasExited)
                {
                    _process.Kill(true);
                }
            }
            catch
            {
                // ignore
            }
            finally
            {
                _process = null;
            }
        }
    }
}
