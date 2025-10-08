// <copyright file="FlowStatusEnum.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models.Constants;

/// <summary>
/// Represents the flow status states for operations in the simulator.
/// </summary>
public enum FlowStatusEnum
{
    /// <summary>
    /// The flow has been created.
    /// </summary>
    Created = 1,

    /// <summary>
    /// The flow is currently in process.
    /// </summary>
    InProcess = 2,

    /// <summary>
    /// The flow has finished.
    /// </summary>
    Finished = 4,

    /// <summary>
    /// The flow is invalid.
    /// </summary>
    Invalid = 8,

    /// <summary>
    /// The flow has been restored.
    /// </summary>
    Restored = 16,

    /// <summary>
    /// The flow has been rejected.
    /// </summary>
    Rejected = 32,
}
