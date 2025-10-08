// <copyright file="GatewayTask.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Validation;

/// <summary>
/// Defines the available gateway tasks for simulation and validation.
/// </summary>
public enum GatewayTask
{
    /// <summary>Invalid or uninitialized task.</summary>
    Invalid = -1,

    /// <summary>No task.</summary>
    None = 0,

    /// <summary>Get application information asynchronously.</summary>
    GetReadAppInfoAsync = 1,

    /// <summary>Get stations information asynchronously.</summary>
    GetReadStationsInfoAsync = 2,

    /// <summary>Create a barcode asynchronously.</summary>
    CreateBarCodeAsync = 4,

    /// <summary>Read a barcode asynchronously.</summary>
    ReadBarCodeAsync = 8,

    /// <summary>Create a cycle asynchronously.</summary>
    CreateCycleAsync = 16,

    /// <summary>Update cycle as OK asynchronously.</summary>
    UpdateCycleOkAsync = 32,

    /// <summary>Update cycle as Not OK asynchronously.</summary>
    UpdateCycleNotOkAsync = 64,

    /// <summary>End of process asynchronously.</summary>
    EndOfProcessAsync = 128,

    /// <summary>Restore part asynchronously.</summary>
    RestorePartAsync = 256,

    /// <summary>Reject part asynchronously.</summary>
    RejectPartAsync = 512,
}
