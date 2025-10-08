// <copyright file="SeqApiOptions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.SeqTail;

/// <summary>
/// Represents configuration options for connecting to a Seq logging server.
/// </summary>
public class SeqApiOptions
{
    /// <summary>
    /// Gets or sets the Seq server URL.
    /// </summary>
    public string Server { get; set; } = "http://localhost:5341";

    /// <summary>
    /// Gets or sets the API key for authenticating with the Seq server.
    /// Security: This should be configured via appsettings.json or environment variables.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filter expression for Seq log queries.
    /// </summary>
    public string Filter { get; set; } = " @Level = 'Error";
}
