// <copyright file="GetShiftDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Shifts.Queries.GetShftDetail;

/// <summary>
/// Represents the GetShiftDetailQuery.
/// </summary>
public class GetShiftDetailQuery : IMonitorRequest<ShiftDetailVm>
{
    /// <summary>
    /// Gets or sets the TimeRequest.
    /// </summary>
    public DateTime TimeRequest { get; set; }

    /// <summary>
    /// Gets or sets the ShiftId.
    /// </summary>
    public int ShiftId { get; set; }
}
