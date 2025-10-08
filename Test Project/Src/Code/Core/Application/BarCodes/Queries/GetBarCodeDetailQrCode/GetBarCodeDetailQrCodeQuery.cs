// <copyright file="GetBarCodeDetailQrCodeQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeDetailQrCode;

/// <summary>
/// Represents the GetBarCodeDetailQrCodeQuery.
/// </summary>
public class GetBarCodeDetailQrCodeQuery : IMonitorRequest<BarCodeDetailMonitorVm>
{
    private string barCode = string.Empty;

    /// <summary>
    /// Gets or sets the BarCode.
    /// </summary>
    // [Fix]
    // CLAUDE
    // Date: 21/08/2025
    // Reason: Added null safety conversion to ensure null values are converted to string.Empty for null safety compliance
    public string BarCode
    {
        get => this.barCode;
        set => this.barCode = value ?? string.Empty;
    }
}
