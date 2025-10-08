// <copyright file="BarCodesListVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetReportsList.GetList;

/// <summary>
/// Represents the BarCodesListVm.
/// </summary>
public class BarCodesListVm
{
    /// <summary>
    /// Gets or sets the BarCodes.
    /// </summary>
    public IList<BarCodeDto> BarCodes { get; set; }

    /// <summary>
    /// Gets or sets the Count.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the Shift.
    /// </summary>
    public Shift Shift { get; set; }

    /// <summary>
    /// Gets or sets the ProductionByShift.
    /// </summary>
    public int ProductionByShift { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BarCodesListVm"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public BarCodesListVm()
    {
        this.BarCodes = [];
        this.Shift = new Shift(new DateTimeMachine());
    }
}
