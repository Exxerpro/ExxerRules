// <copyright file="BarCodeRestoredView.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Commands.Restore;

/// <summary>
/// Represents the BarCodeRestoredView.
/// </summary>
public class BarCodeRestoredView
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
    /// Initializes a new instance of the <see cref="BarCodeRestoredView"/> class.
    /// </summary>
    public BarCodeRestoredView()
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
    public static IndQuestResults.Result<BarCodeRestoredView> ToDto(BarCode src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCodeRestoredView>.WithFailure("BarCode source cannot be null");
        }

        return IndQuestResults.Result<BarCodeRestoredView>.Success(new BarCodeRestoredView
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
    public static IndQuestResults.Result<BarCode> ToEntity(BarCodeRestoredView src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCode>.WithFailure("BarCodeRestoredView source cannot be null");
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
