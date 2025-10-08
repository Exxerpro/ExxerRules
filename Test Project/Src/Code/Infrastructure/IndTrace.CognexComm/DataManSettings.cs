// <copyright file="DataManSettings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.CognexComm;

/// <summary>
/// Represents settings for connecting to a Cognex DataMan barcode reader.
/// </summary>
public class DataManSettings
{
    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string IpAddress { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string UserName { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataManSettings"/> class.
    /// </summary>
    public DataManSettings()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataManSettings"/> class with specified connection parameters.
    /// </summary>
    /// <param name="ipAddress">The IP address of the DataMan device.</param>
    /// <param name="userName">The username for authentication.</param>
    /// <param name="password">The password for authentication.</param>
    private DataManSettings(string ipAddress, string userName, string password)
    {
        // [Fix]
        // CLAUDE
        // Date: 02/09/2025
        // Reason: [Nullable Reference] - Initialize properties in private constructor to prevent warnings
        this.IpAddress = ipAddress;
        this.UserName = userName;
        this.Password = password;
    }
}
