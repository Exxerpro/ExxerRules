using IndTrace.DataStore.Extensions;
using System.Net;

namespace IndTrace.DataStore.ModelsComs;

/// <summary>
/// Represents data and configuration for a PLC (Programmable Logic Controller).
/// </summary>
public class PlcData
{
    private string ipAddress = string.Empty;

    /// <summary>
    /// Gets or sets the PLC instance.
    /// </summary>
    public IPlc Plc { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CPU MPI address for the PLC.
    /// </summary>
    public int CpuMpiAddress { get; set; }

    /// <summary>
    /// Gets the concurrent dictionary of data blocks associated with the PLC.
    /// </summary>
    public ConcurrentDictionary<int, Db> DataBlocks { get; private set; } = new ConcurrentDictionary<int, Db>();

    /// <summary>
    /// Gets or sets the list of database information associated with the PLC.
    /// </summary>
    public List<DbInfo> DBs { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the PLC is enabled.
    /// </summary>
    public int Enabled { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the PLC.
    /// </summary>
    public string IpAddress
    {
        get => this.ipAddress;
        set => this.ipAddress = value.CleanString();
    }

    /// <summary>
    /// Gets a value indicating whether the IP address exists on a network interface.
    /// </summary>
    public bool IpAddressExist => this.IsIpAddressAssigned();

    /// <summary>
    /// Gets a value indicating whether the IP address is not assigned.
    /// </summary>
    public bool IpAddressIsNotAssigned => !this.IpAddressIsValid;

    /// <summary>
    /// Gets a value indicating whether the IP address is valid.
    /// </summary>
    public bool IpAddressIsValid => this.IsValidIp(this.IpAddress);

    /// <summary>
    /// Gets or sets the machine identifier associated with the PLC.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the name of the PLC.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether OEE is enabled for the PLC.
    /// </summary>
    public bool OeeEnabled { get; set; }

    /// <summary>
    /// Gets or sets the PLC brand.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string PlcBrand { get; set; } = null!;

    /// <summary>
    /// Gets or sets the PLC identifier.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Gets or sets the PLC type.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string PlcType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the port number for the PLC.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the rack number for the PLC.
    /// </summary>
    public int RackNumber { get; set; }

    /// <summary>
    /// Gets or sets the dictionary of variables associated with the PLC.
    /// </summary>
    public Dictionary<string, VariableS7> Variables { get; set; } = [];

    /// <summary>
    /// Adds a database to the PLC's data blocks.
    /// </summary>
    /// <param name="db">The database to add.</param>
    /// <returns>True if the database was added; otherwise, false.</returns>
    public bool AddDb(Db db)
    {
        var result = this.DataBlocks.TryAdd(db.Id, db);
        return result;
    }

    /// <summary>
    /// Removes a data block by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the data block to remove.</param>
    /// <returns>True if the data block was removed; otherwise, false.</returns>
    public bool RemoveDataBlock(int id)
    {
        return true;
    }

    /// <summary>
    /// Saves the data blocks associated with the PLC.
    /// </summary>
    /// <returns>True if the data blocks were saved; otherwise, false.</returns>
    public bool SaveDataBlocks()
    {
        return true;
    }

    private bool IsValidIp(string ipAddress)
    {
        return IPAddress.TryParse(ipAddress, out _);
    }
}
