// <copyright file="GetBarCodesLabelQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeLabel;

/// <summary>
/// Represents the GetBarCodesLabelQuery.
/// </summary>
public class GetBarCodesLabelQuery(string label) : IMonitorRequest<BarCodesListVm>
{
    /// <summary>
    /// Gets or sets the Label.
    /// </summary>
    public string Label { get; set; } = label;
}
