// <copyright file="BarCodeRejectedView.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Commands.Reject;

/// <summary>
/// Represents the BarCodeRejectedView.
/// </summary>
public class BarCodeRejectedView
{
    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the BarCodeId.
    /// </summary>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the Label.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the FlowStatus.
    /// </summary>
    public FlowStatus FlowStatus { get; set; }

    /// <summary>
    /// Gets or sets the PartStatus.
    /// </summary>
    public PartStatus PartStatus { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BarCodeRejectedView"/> class.
    /// </summary>
    public BarCodeRejectedView()
    {
        this.Label = string.Empty;
        this.FlowStatus = FlowStatus.None;
        this.PartStatus = PartStatus.None;
    }

    /// <summary>
    /// Executes ToDto operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDto.</returns>
    public static IndQuestResults.Result<BarCodeRejectedView> ToDto(BarCode src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCodeRejectedView>.WithFailure("BarCode source cannot be null");
        }

        return IndQuestResults.Result<BarCodeRejectedView>.Success(new BarCodeRejectedView
        {
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            Label = src.Label ?? string.Empty,
            FlowStatus = EnumModel.FromValue<FlowStatus>(src.FlowStatus),
            PartStatus = EnumModel.FromValue<PartStatus>(src.PartStatus),
        });
    }

    /// <summary>
    /// Executes ToEntity operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToEntity.</returns>
    public static IndQuestResults.Result<BarCode> ToEntity(BarCodeRejectedView src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCode>.WithFailure("BarCodeRejectedView source cannot be null");
        }

        return IndQuestResults.Result<BarCode>.Success(new BarCode
        {
            MachineId = src.MachineId,
            BarCodeId = src.BarCodeId,
            Label = src.Label ?? string.Empty,
            FlowStatus = src.FlowStatus.Value,
            PartStatus = src.PartStatus.Value,
        });
    }
}
