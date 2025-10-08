// <copyright file="UpdateShiftCommand.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Shifts.Commands.Update;

using IndTrace.Application.Shifts.Queries.GetShftDetail;

/// <summary>
/// Command to update the details of a shift.
/// </summary>
public class UpdateShiftCommand : IMonitorRequest<ShiftDetailVm>
{
#nullable enable

    /// <summary>
    /// Gets or sets the unique identifier of the shift.
    /// </summary>
    public int? ShiftId { get; set; }

    /// <summary>
    /// Gets or sets the part number associated with the shift.
    /// </summary>
    public string? PartNumber { get; set; }

    /// <summary>
    /// Gets or sets the name of the shift.
    /// </summary>
    public string? ShiftName { get; set; }

    /// <summary>
    /// Gets or sets the shift code or identifier.
    /// </summary>
    public string? Shifto { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the shift is active.
    /// </summary>
    public int? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the version of the shift.
    /// </summary>
    public int? Version { get; set; }

    /// <summary>
    /// Gets or sets the customer part number associated with the shift.
    /// </summary>
    public string? CustomerPartNumber { get; set; }

    /// <summary>
    /// Gets or sets the alias for the part number.
    /// </summary>
    public string? AliasNoParte { get; set; }

    /// <summary>
    /// Gets or sets the description of the shift.
    /// </summary>
    public string? Description { get; set; }
}
