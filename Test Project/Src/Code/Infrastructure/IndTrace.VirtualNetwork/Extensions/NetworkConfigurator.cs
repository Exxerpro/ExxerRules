namespace IndTrace.VirtualNetwork.Extensions;

/// <summary>
/// Provides utility methods for network configuration, specifically for adding IP addresses on Windows.
/// </summary>
public static class NetworkConfigurator
{
    /// <summary>
    /// Adds an IP address to a specified network interface on Windows.
    /// </summary>
    /// <param name="interfaceName">The name of the network interface.</param>
    /// <param name="ipAddress">The IP address to add.</param>
    /// <param name="subnetMask">The subnet mask for the IP address.</param>
    /// <param name="gateway">Optional: The gateway address.</param>
    /// <param name="logger">The logger instance for logging information and errors.</param>
    /// <returns><see langword="true"/> if the IP address was successfully added; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="Exception">Thrown when the IP address cannot be added.</exception>
    /// <exception cref="InvalidCredentialException">Thrown when administrator privileges are required but not available.</exception>
    public static bool AddIpAddressWindows(string interfaceName, string ipAddress, string subnetMask, string gateway, ILogger logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }
        if (string.IsNullOrWhiteSpace(interfaceName))
        {
            throw new ArgumentException("Interface name cannot be null or whitespace.", nameof(interfaceName));
        }
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            throw new ArgumentException("IP address cannot be null or whitespace.", nameof(ipAddress));
        }
        if (string.IsNullOrWhiteSpace(subnetMask))
        {
            throw new ArgumentException("Subnet mask cannot be null or whitespace.", nameof(subnetMask));
        }
        var arguments = $"interface ip add address \"{interfaceName}\" {ipAddress} {subnetMask}";
        if (!string.IsNullOrWhiteSpace(gateway))
        {
            arguments += $" {gateway}";
        }

        logger.LogInformation("Executing: netsh {Arguments}", arguments);
        var (success, output, error) = RunNetshCommand(arguments);

        if (!success)
        {
            logger.LogError("Failing error {error} adding ip {ip} on interface {interface}", error, ipAddress, interfaceName);
            throw new Exception($"Failed to add IP address: {error}");
        }

        logger.LogInformation(output);
        return true;
    }

    /// <summary>
    /// Runs a netsh command.
    /// </summary>
    /// <param name="arguments">The arguments for the netsh command.</param>
    /// <returns>A tuple indicating success, standard output, and standard error.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the netsh process fails to start.</exception>
    /// <exception cref="InvalidCredentialException">Thrown when administrator privileges are required but not available.</exception>
    private static (bool Success, string Output, string Error) RunNetshCommand(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "C:\\Windows\\System32\\netsh.exe", // Avoids relying on PATH
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = Process.Start(startInfo);
        if (process is null)
        {
            throw new InvalidOperationException("Failed to start netsh process.");
        }

        string output;
        string error;

        using (process)
        {
            output = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();
            process.WaitForExit();
        }

        if (output.Contains("administrator", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidCredentialException("Administrator privileges are required to execute this command.");
        }

        return (process.ExitCode == 0, output, error);
    }
}
