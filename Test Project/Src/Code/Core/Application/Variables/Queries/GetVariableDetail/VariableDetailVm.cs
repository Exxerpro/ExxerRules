// <copyright file="VariableDetailVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Variables.Queries.GetVariableDetail;

/// <summary>
/// Represents the VariableDetailVm.
/// </summary>
public class VariableDetailVm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VariableDetailVm"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public VariableDetailVm()
    {
    }

    /// <summary>
    /// Gets or sets the EntitieId.
    /// </summary>
    public int VariableId { get; set; }

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the PlcId.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Alias { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string NetType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Length.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Gets or sets the IsActive.
    /// </summary>
    public int IsActive { get; set; }

    /// <summary>
    /// Gets or sets the Direction.
    /// </summary>
    public int Direction { get; set; }

    /// <summary>
    /// Gets or sets the VariableGroupId.
    /// </summary>
    public int VariableGroupId { get; set; }

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string NativeType { get; set; } = null!;

    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string NativeAddress { get; set; } = null!;

    /// <summary>
    /// Executes ToDto operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDto.</returns>
    public static IndQuestResults.Result<VariableDetailVm> ToDto(Variable src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<VariableDetailVm>.WithFailure("Variable source cannot be null");
        }

        return IndQuestResults.Result<VariableDetailVm>.Success(new VariableDetailVm
        {
            VariableId = src.VariableId,
            MachineId = src.MachineId,
            PlcId = src.PlcId,
            Name = src.Name,
            Address = src.Address,
            Alias = src.Alias,
            NetType = src.NetType,
            Length = src.Length,
            IsActive = src.IsActive,
            Direction = src.Direction,
            VariableGroupId = src.VariableGroupId,
            NativeType = src.NativeType,
            NativeAddress = src.NativeAddress,
        });
    }

    /// <summary>
    /// Executes ToEntity operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToEntity.</returns>
    public static IndQuestResults.Result<Variable> ToEntity(VariableDetailVm src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<Variable>.WithFailure("VariableDetailVm source cannot be null");
        }

        return IndQuestResults.Result<Variable>.Success(new Variable
        {
            VariableId = src.VariableId,
            MachineId = src.MachineId,
            PlcId = src.PlcId,
            Name = src.Name,
            Address = src.Address,
            Alias = src.Alias,
            NetType = src.NetType,
            Length = src.Length,
            IsActive = src.IsActive,
            Direction = src.Direction,
            VariableGroupId = src.VariableGroupId,
            NativeType = src.NativeType,
            NativeAddress = src.NativeAddress,
        });
    }
}
