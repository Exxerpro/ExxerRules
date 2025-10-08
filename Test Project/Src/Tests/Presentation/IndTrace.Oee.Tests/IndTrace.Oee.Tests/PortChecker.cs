using System.Net;
using System.Net.NetworkInformation;

namespace IndTrace.Oee.Tests;

/// <summary>
/// Utility class to check if a TCP port is closed (not being listened on).
/// </summary>
public static class PortChecker
{
    /// <summary>
    /// Checks if a given TCP port is currently in closed (being listened on) by a local process.
    /// </summary>
    /// <param name="port">The port number to check.</param>
    /// <returns>True if the port is in close, false otherwise.</returns>
    public static bool IsPortClosed(int port)
    {
        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] activeTcpListeners = ipProperties.GetActiveTcpListeners();

        // Check if any of the active TCP listeners are using the specified port
        foreach (IPEndPoint endPoint in activeTcpListeners)
        {
            if (endPoint.Port == port)
            {
                return false; // Port is open
            }
        }
        return true; // Port is closed
    }
}
