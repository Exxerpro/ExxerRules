using System.Net;
using System.Net.NetworkInformation;
using IndTrace.DataStore.ModelsComs;

namespace IndTrace.DataStore.Extensions;

/// <summary>
/// Provides network utility extension methods for PlcData objects.
/// </summary>
public static class NetworkUtils
{
    /// <summary>
    /// Determines whether the IP address assigned to the specified PlcData is present on any network interface of the host machine.
    /// </summary>
    /// <param name="plcData">The PlcData instance to check.</param>
    /// <returns>True if the IP address is assigned to a network interface; otherwise, false.</returns>
    public static bool IsIpAddressAssigned(this PlcData plcData)
    {
        // Parse the input string to an IPAddress object
        if (!IPAddress.TryParse(plcData.IpAddress, out IPAddress? addressToCheck))
        {
            throw new ArgumentException("Invalid IP address format", nameof(plcData));
        }

        // Get all network interfaces on the host machine
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        // Iterate through each network interface
        return networkInterfaces.Select(networkInterface => networkInterface.GetIPProperties())
            .Select(ipProperties =>
            {
                var unicastAddresses = ipProperties.UnicastAddresses.Select(ua => ua.Address);
                if (OperatingSystem.IsWindows())
                {
                    return unicastAddresses.Concat(ipProperties.AnycastAddresses.Select(aa => aa.Address));
                }
                return unicastAddresses;
            })
            .Any(allAddresses => allAddresses.Any(address => address.Equals(addressToCheck)));
    }
}
