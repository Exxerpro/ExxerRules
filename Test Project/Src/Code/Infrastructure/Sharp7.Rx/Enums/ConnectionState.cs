// <copyright file="ConnectionState.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Enums;

/// <summary>
/// Represents the connection state of the PLC.
/// </summary>
public enum ConnectionState
{
    /// <summary>
    /// The initial state before any connection attempts.
    /// </summary>
    Initial,

    /// <summary>
    /// The PLC is successfully connected.
    /// </summary>
    Connected,

    /// <summary>
    /// The PLC has been disconnected by the user.
    /// </summary>
    DisconnectedByUser,

    /// <summary>
    /// The connection to the PLC has been lost unexpectedly.
    /// </summary>
    ConnectionLost,

    /// <summary>
    /// The PLC connection resources have been disposed.
    /// </summary>
    Disposed,
}
