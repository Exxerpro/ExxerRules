using System.Net.NetworkInformation;

namespace IndTrace.Dependencies.Services;

/// <summary>
/// Service for collecting system performance metrics including CPU, memory, and network utilization.
/// </summary>
public class SystemMetricsService
{
    private readonly PerformanceCounter? cpuCounter; // Made nullable to address CS8618
    private readonly PerformanceCounter? ramCounter; // Made nullable to address CS8618
    private long previousBytesSent;
    private long previousBytesReceived;
    private DateTime lastNetworkCheck;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemMetricsService"/> class.
    /// </summary>
    public SystemMetricsService()
    {
        if (OperatingSystem.IsWindows())
        {
            this.cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this.ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        this.previousBytesSent = this.GetTotalBytesSent();
        this.previousBytesReceived = this.GetTotalBytesReceived();
        this.lastNetworkCheck = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the current CPU usage percentage.
    /// </summary>
    /// <returns>The CPU usage as a percentage.</returns>
    public float GetCpuUsage()
    {
        return OperatingSystem.IsWindows() ? this.cpuCounter?.NextValue() ?? 0f : this.GetLinuxCpuUsage();
    }

    /// <summary>
    /// Gets the current memory usage.
    /// </summary>
    /// <returns>The memory usage in MB (Windows) or percentage (Linux).</returns>
    public float GetMemoryUsage()
    {
        if (OperatingSystem.IsWindows())
        {
            return this.ramCounter?.NextValue() ?? 0f;
        }
        return this.GetLinuxMemoryUsage();
    }

    /// <summary>
    /// Gets the network utilization in bytes per second for sent and received data.
    /// </summary>
    /// <returns>A tuple containing the bytes sent per second and bytes received per second.</returns>
    public (double sent, double received) GetNetworkUtilization()
    {
        long totalSent = this.GetTotalBytesSent();
        long totalReceived = this.GetTotalBytesReceived();
        DateTime now = DateTime.UtcNow;
        double seconds = (now - this.lastNetworkCheck).TotalSeconds;

        double sentPerSec = (totalSent - this.previousBytesSent) / seconds;
        double receivedPerSec = (totalReceived - this.previousBytesReceived) / seconds;

        this.previousBytesSent = totalSent;
        this.previousBytesReceived = totalReceived;
        this.lastNetworkCheck = now;

        return (sentPerSec, receivedPerSec);
    }

    /// <summary>
    /// Gets the CPU usage on Linux systems by reading from /proc/stat.
    /// </summary>
    /// <returns>The CPU usage as a percentage.</returns>
    private float GetLinuxCpuUsage()
    {
        string[] cpuInfo = System.IO.File.ReadAllLines("/proc/stat")[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        float idleTime = float.Parse(cpuInfo[4]);
        float totalTime = cpuInfo.Skip(1).Sum(float.Parse);
        return 100f * (1f - (idleTime / totalTime));
    }

    /// <summary>
    /// Gets the memory usage on Linux systems by reading from /proc/meminfo.
    /// </summary>
    /// <returns>The memory usage as a percentage.</returns>
    private float GetLinuxMemoryUsage()
    {
        var memInfo = System.IO.File.ReadAllLines("/proc/meminfo");
        float totalMemory = float.Parse(memInfo[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
        float availableMemory = float.Parse(memInfo[2].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
        return (1 - (availableMemory / totalMemory)) * 100;
    }

    /// <summary>
    /// Gets the total bytes sent across all active network interfaces.
    /// </summary>
    /// <returns>The total number of bytes sent.</returns>
    private long GetTotalBytesSent()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(n => n.OperationalStatus == OperationalStatus.Up)
            .Sum(n => n.GetIPv4Statistics().BytesSent);
    }

    /// <summary>
    /// Gets the total bytes received across all active network interfaces.
    /// </summary>
    /// <returns>The total number of bytes received.</returns>
    private long GetTotalBytesReceived()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(n => n.OperationalStatus == OperationalStatus.Up)
            .Sum(n => n.GetIPv4Statistics().BytesReceived);
    }
}
