// <copyright file="VariablesView.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeDetailMonitor;

/// <summary>
/// Represents the VariablesView.
/// </summary>
public class VariablesView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VariablesView"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public VariablesView()
    {
        this.Name = string.Empty;
        this.Description = string.Empty;
        this.Alias = string.Empty;
        this.Address = string.Empty;
        this.NetType = string.Empty;
        this.NativeType = string.Empty;
        this.Value = string.Empty;
        this.NativeAddress = string.Empty;
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
    /// Gets or sets the Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the Alias.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Gets or sets the Address.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Gets or sets the NetType.
    /// </summary>
    public string NetType { get; set; }

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
    /// Gets or sets the VariableSpecId.
    /// </summary>
    public int? VariableSpecId { get; set; }

    /// <summary>
    /// Gets or sets the TagStatus.
    /// </summary>
    public int TagStatus { get; set; }

    /// <summary>
    /// Gets or sets the NativeType.
    /// </summary>
    public string NativeType { get; set; }

    /// <summary>
    /// Gets or sets the Value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the NativeAddress.
    /// </summary>
    public string NativeAddress { get; set; }

    public string EnumValue
    {
        get
        {
            return this.Name switch
            {
                "CycleStatusPlc" => int.TryParse(this.Value, out var i1) ? EnumModel.FromValue<CycleStatus>(i1).Name : string.Empty,
                "PartStatusPlc" => int.TryParse(this.Value, out var i2) ? EnumModel.FromValue<PartStatus>(i2).Name : string.Empty,
                "FlowStatusPlc" => int.TryParse(this.Value, out var i3) ? EnumModel.FromValue<FlowStatus>(i3).Name : string.Empty,
                "MachineTypePlc" => int.TryParse(this.Value, out var i4) ? EnumModel.FromValue<MachineType>(i4).Name : string.Empty,
                "WorkFlowTypePlc" => int.TryParse(this.Value, out var i5) ? EnumModel.FromValue<WorkFlowType>(i5).Name : string.Empty,
                _ => this.Value ?? string.Empty,
            };
        }
    }

    /// <summary>
    /// Converts a <see cref="Variable"/> entity into a <see cref="VariablesView"/> using functional Result semantics.
    /// </summary>
    /// <param name="src">The source <see cref="Variable"/> entity.</param>
    /// <returns>A Result with the mapped <see cref="VariablesView"/>, or a failure when <paramref name="src"/> is null.</returns>
    public static IndQuestResults.Result<VariablesView> ToDto(Variable src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<VariablesView>.WithFailure("Variable source cannot be null");
        }

        return IndQuestResults.Result<VariablesView>.Success(new VariablesView
        {
            VariableId = src.VariableId,
            MachineId = src.MachineId,
            PlcId = src.PlcId,
            Name = src.Name,
            Description = src.Description,
            Alias = src.Alias,
            Address = src.Address,
            NetType = src.NetType,
            Length = src.Length,
            IsActive = src.IsActive,
            Direction = src.Direction,
            VariableGroupId = src.VariableGroupId,
            VariableSpecId = src.VariableSpecId,
            TagStatus = src.TagStatus,
            NativeType = src.NativeType,
            Value = src.Value,
            NativeAddress = src.NativeAddress,

            // Only map properties that exist in both classes
        });
    }

    /// <summary>
    /// Converts a collection of <see cref="Variable"/> entities into a list of <see cref="VariablesView"/>.
    /// </summary>
    /// <param name="src">Source collection of <see cref="Variable"/> entities.</param>
    /// <returns>A Result with the mapped list of <see cref="VariablesView"/>, or a failure when <paramref name="src"/> is null.</returns>
    public static IndQuestResults.Result<List<VariablesView>> ToDtoList(IEnumerable<Variable> src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<List<VariablesView>>.WithFailure("Variable collection cannot be null");
        }

        var list = src.Select(s => new VariablesView
        {
            VariableId = s.VariableId,
            MachineId = s.MachineId,
            PlcId = s.PlcId,
            Name = s.Name,
            Description = s.Description,
            Alias = s.Alias,
            Address = s.Address,
            NetType = s.NetType,
            Length = s.Length,
            IsActive = s.IsActive,
            Direction = s.Direction,
            VariableGroupId = s.VariableGroupId,
            VariableSpecId = s.VariableSpecId,
            TagStatus = s.TagStatus,
            NativeType = s.NativeType,
            Value = s.Value,
            NativeAddress = s.NativeAddress,
        }).ToList();
        return IndQuestResults.Result<List<VariablesView>>.Success(list);
    }

    /// <summary>
    /// Converts a <see cref="VariablesView"/> into a <see cref="Variable"/> entity using functional Result semantics.
    /// </summary>
    /// <param name="src">The source <see cref="VariablesView"/>.</param>
    /// <returns>A Result with the mapped <see cref="Variable"/>, or a failure when <paramref name="src"/> is null.</returns>
    public static IndQuestResults.Result<Variable> ToEntity(VariablesView src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<Variable>.WithFailure("VariablesView source cannot be null");
        }

        return IndQuestResults.Result<Variable>.Success(new Variable
        {
            VariableId = src.VariableId,
            MachineId = src.MachineId,
            PlcId = src.PlcId,
            Name = src.Name,
            Description = src.Description,
            Alias = src.Alias,
            Address = src.Address,
            NetType = src.NetType,
            Length = src.Length,
            IsActive = src.IsActive,
            Direction = src.Direction,
            VariableGroupId = src.VariableGroupId,
            VariableSpecId = src.VariableSpecId,
            TagStatus = src.TagStatus,
            NativeType = src.NativeType,
            Value = src.Value,
            NativeAddress = src.NativeAddress,

            // Only map properties that exist in both classes
        });
    }
}
