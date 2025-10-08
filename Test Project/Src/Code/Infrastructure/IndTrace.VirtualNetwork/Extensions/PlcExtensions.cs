using PlcData = IndTrace.DataStore.ModelsComs.PlcData;

namespace IndTrace.VirtualNetwork.Extensions;

public static class PlcExtensions
{
    /// <summary>
    /// The name of the network interface to use, e.g., "Tailscale".
    /// </summary>
    public static string InterfaceName = "Tailscale";

    /// <summary>
    /// The subnet mask to use for IP address configuration.
    /// </summary>
    public static string SubnetMask = "255.255.0.0";

    /// <summary>
    /// The gateway address to use for IP address configuration.
    /// </summary>
    public static string GatewayAddress = "192.168.1.1";

    /// <summary>
    /// Adds the IP address of the PLC to the network interface.
    /// </summary>
    /// <param name="plcDataData">The PLC data containing the IP address.</param>
    /// <param name="logger">The logger instance.</param>
    /// <returns><see langword="true"/> if the IP address was successfully added or already exists; otherwise, <see langword="false"/>.</returns>
    public static bool AddIpAddressToNetwork(this PlcData plcDataData, ILogger logger)
    {
        if (!CheckIfNetworkInterfaceExists(InterfaceName, logger))
        {
            Debug.Fail($"Network interface '{InterfaceName}' does not exist.");
            return false;
        }

        if (plcDataData.IpAddressIsNotAssigned)
        {
            try
            {
                NetworkConfigurator.AddIpAddressWindows(InterfaceName, plcDataData.IpAddress, SubnetMask, GatewayAddress, logger);
            }
            catch (Exception ex)
            {
                Debug.Fail($"Failed to assign IP address: {ex.Message}");
                return false;
            }
        }

        return plcDataData.IpAddressExist;
    }

    /// <summary>
    /// Checks if a network interface with the specified name exists.
    /// </summary>
    /// <param name="interfaceName">The name of the network interface to check.</param>
    /// <param name="logger">The logger instance.</param>
    /// <returns><see langword="true"/> if the network interface exists; otherwise, <see langword="false"/>.</returns>
    public static bool CheckIfNetworkInterfaceExists(string interfaceName, ILogger logger)
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var networkInterface in networkInterfaces)
        {
            if (networkInterface.Name.Equals(interfaceName, StringComparison.OrdinalIgnoreCase))
            {
                // Log the details of the found network interface
                logger.LogInformation("Interface Name: {Name}", networkInterface.Name);
                logger.LogInformation("Description: {Description}", networkInterface.Description);
                logger.LogInformation("Status: {Status}", networkInterface.OperationalStatus);
                logger.LogInformation("Speed: {Speed} Mbps", networkInterface.Speed / 1_000_000);

                var ipProperties = networkInterface.GetIPProperties();
                var ipAddresses = ipProperties.UnicastAddresses.Select(ua => ua.Address.ToString()).ToArray();

                if (ipAddresses.Any())
                {
                    logger.LogInformation("IP Addresses:");
                    foreach (var ipAddress in ipAddresses) logger.LogInformation("  - {IpAddress}", ipAddress);
                }

                logger.LogInformation(new string('-', 40));

                // Return true as the network interface exists
                return true;
            }
        }

        // Return false as the network interface does not exist
        return false;
    }

    /// <summary>
    /// Adds an IP address to a specified network interface on Windows using netsh command.
    /// </summary>
    /// <param name="interfaceName">The name of the network interface.</param>
    /// <param name="ipAddress">The IP address to add.</param>
    /// <param name="subnetMask">The subnet mask for the IP address.</param>
    /// <param name="gateway">The gateway address.</param>
    /// <param name="logger">The logger instance.</param>
    /// <returns><see langword="true"/> if the IP address was successfully added; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="Exception">Thrown when the command execution fails.</exception>
    /// <exception cref="InvalidCredentialException">Thrown when administrator privileges are required but not available.</exception>
    public static bool AddIpAddressWindows(string interfaceName, string ipAddress, string subnetMask, string gateway, ILogger logger)
    {
        string addIpCommand = $"netsh interface ip add address {interfaceName} {ipAddress} {subnetMask}";
        logger.LogInformation(addIpCommand);
        RunCommand(addIpCommand, logger);

        return true;
    }

    /// <summary>
    /// Runs a shell command using cmd.exe.
    /// </summary>
    /// <param name="command">The command to run.</param>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="Exception">Thrown when the command execution results in an error.</exception>
    /// <exception cref="InvalidCredentialException">Thrown when administrator privileges are required but not available.</exception>
    private static void RunCommand(string command, ILogger logger)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        var psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        var process = Process.Start(psi);
        if (process is null)
        {
            logger.LogError("Failed to start process for command: {Command}", command);
            throw new InvalidOperationException("Failed to start process.");
        }

        using (process)
        {
            process.WaitForExit();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            if (output.Contains("administrator", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidCredentialException("you need admin privileges to run this");
            }

            logger.LogInformation(output);
        }
    }
}
