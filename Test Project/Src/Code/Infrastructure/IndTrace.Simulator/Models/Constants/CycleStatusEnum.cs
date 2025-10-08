// <copyright file="CycleStatusEnum.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Models.Constants;

/// <summary>
/// Enumerates the possible cycle statuses for a fixture.
/// </summary>
public enum CycleStatusEnum
{
    /// <summary>No status.</summary>
    None = 0,

    /// <summary>Cycle not started.</summary>
    NotStarted = 1,

    /// <summary>Cycle started.</summary>
    Started = 2,

    /// <summary>Cycle finished successfully.</summary>
    FinishedOk = 4,

    /// <summary>Cycle finished with failure.</summary>
    FinishedNok = 8,

    /// <summary>Cycle canceled.</summary>
    Canceled = 16,

    /// <summary>Cycle rejected.</summary>
    Rejected = 32,
}
