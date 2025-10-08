// <copyright file="BarCodeDtoGateway.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.DTO;

/// <summary>
/// Represents the BarCodeDtoGateway.
/// </summary>
public class BarCodeDtoGateway
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BarCodeDtoGateway"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public BarCodeDtoGateway()
    {
        this.Label = string.Empty;
        this.FlowStatus = Domain.Enum.FlowStatus.None;
        this.Machine = new Machine();
    }

    /// <summary>
    /// Gets or sets the BarCodeId.
    /// </summary>
    public int BarCodeId { get; set; }

    /// <summary>
    /// Gets or sets the Label.
    /// </summary>
    public Label Label { get; set; }

    /// <summary>
    /// Gets or sets the ProductId.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the FlowStatus.
    /// </summary>
    public FlowStatus FlowStatus { get; set; }

    /// <summary>
    /// Gets or sets the CreatedOn.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the ModifiedOn.
    /// </summary>
    public DateTime ModifiedOn { get; set; }

    /// <summary>
    /// Gets or sets the Machine.
    /// </summary>
    public virtual Machine Machine { get; set; }

    /// <summary>
    /// Executes ToDto operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDto.</returns>
    public static IndQuestResults.Result<BarCodeDtoGateway> ToDto(BarCode src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCodeDtoGateway>.WithFailure("BarCode source cannot be null");
        }

        return IndQuestResults.Result<BarCodeDtoGateway>.Success(new BarCodeDtoGateway
        {
            BarCodeId = src.BarCodeId,
            Label = src.Label ?? string.Empty,
            ProductId = src.ProductId,
            MachineId = src.MachineId,
            FlowStatus = src.FlowStatus, // Assuming FlowStatus maps to FlowStatus
            CreatedOn = src.CreatedOn,
            ModifiedOn = src.ModifiedOn,
        });
    }

    /// <summary>
    /// Executes ToEntity operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToEntity.</returns>
    public static IndQuestResults.Result<BarCode> ToEntity(BarCodeDtoGateway src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<BarCode>.WithFailure("BarCodeDtoGateway source cannot be null");
        }

        return IndQuestResults.Result<BarCode>.Success(new BarCode
        {
            BarCodeId = src.BarCodeId,
            Label = src.Label,
            ProductId = src.ProductId,
            MachineId = src.MachineId,
            FlowStatus = src.FlowStatus, // Assuming FlowStatus maps to FlowStatus
            CreatedOn = src.CreatedOn,
            ModifiedOn = src.ModifiedOn,
        });
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate DTO gateway logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
