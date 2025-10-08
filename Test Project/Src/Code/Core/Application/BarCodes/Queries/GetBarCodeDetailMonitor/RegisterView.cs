// <copyright file="RegisterView.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.BarCodes.Queries.GetBarCodeDetailMonitor;

/// <summary>
/// Represents the RegisterView.
/// </summary>
public class RegisterView
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int RegisterId { get; set; }

    /// <summary>
    /// Gets or sets the Name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the EntitieId.
    /// </summary>
    public int VariableId { get; set; }

    /// <summary>
    /// Gets or sets the CycleId.
    /// </summary>
    public int CycleId { get; set; }

    /// <summary>
    /// Gets or sets the Value.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the DataType.
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the StatusValueId.
    /// </summary>
    public int StatusValueId { get; set; }

    /// <summary>
    /// Gets or sets the TimeStamp.
    /// </summary>
    public DateTime TimeStamp { get; set; }

    public string EnumValue
    {
        get
        {
            try
            {
                return this.Name switch
                {
                    "CycleStatusPlc" => EnumModel.FromValue<CycleStatus>(int.Parse(this.Value)).Name,
                    "PartStatusPlc" => EnumModel.FromValue<PartStatus>(int.Parse(this.Value)).Name,
                    "FlowStatusPlc" => EnumModel.FromValue<FlowStatus>(int.Parse(this.Value)).Name,
                    "MachineTypePlc" => EnumModel.FromValue<MachineType>(int.Parse(this.Value)).Name,
                    "WorkFlowTypePlc" => EnumModel.FromValue<WorkFlowType>(int.Parse(this.Value)).Name,
                    _ => this.Value,
                };
            }
            catch (Exception)
            {
                return "None";
            }
        }
    }

    /// <summary>
    /// Converts a <see cref="Register"/> entity to a <see cref="RegisterView"/> using functional Result semantics.
    /// </summary>
    /// <param name="src">The source <see cref="Register"/> entity.</param>
    /// <returns>A Result with the mapped <see cref="RegisterView"/>, or failure when <paramref name="src"/> is null.</returns>
    public static IndQuestResults.Result<RegisterView> ToDto(Register src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<RegisterView>.WithFailure("Register source cannot be null");
        }

        return IndQuestResults.Result<RegisterView>.Success(new RegisterView
        {
            RegisterId = src.RegisterId,
            Name = src.Name,
            Description = src.Description,
            MachineId = src.MachineId,
            VariableId = src.VariableId,
            CycleId = src.CycleId,
            Value = src.Value,
            DataType = src.DataType,
            StatusValueId = src.StatusValueId,
            TimeStamp = src.TimeStamp,
        });
    }

    /// <summary>
    /// Converts a <see cref="RegisterView"/> into a <see cref="Register"/> entity using functional Result semantics.
    /// </summary>
    /// <param name="src">The source <see cref="RegisterView"/>.</param>
    /// <returns>A Result with the mapped <see cref="Register"/>, or failure when <paramref name="src"/> is null.</returns>
    public static IndQuestResults.Result<Register> ToEntity(RegisterView src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<Register>.WithFailure("RegisterView source cannot be null");
        }

        return IndQuestResults.Result<Register>.Success(new Register
        {
            RegisterId = src.RegisterId,
            Name = src.Name,
            Description = src.Description,
            MachineId = src.MachineId,
            VariableId = src.VariableId,
            CycleId = src.CycleId,
            Value = src.Value,
            DataType = src.DataType,
            StatusValueId = src.StatusValueId,
            TimeStamp = src.TimeStamp,
        });
    }

    /// <summary>
    /// Converts a collection of <see cref="Register"/> entities into a list of <see cref="RegisterView"/>.
    /// </summary>
    /// <param name="src">Source collection of <see cref="Register"/> entities.</param>
    /// <returns>A Result with the mapped list of <see cref="RegisterView"/>, or failure when <paramref name="src"/> is null.</returns>
    public static IndQuestResults.Result<List<RegisterView>> ToDtoList(IEnumerable<Register> src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<List<RegisterView>>.WithFailure("Register collection cannot be null");
        }

        var list = src.Select(s => new RegisterView
        {
            RegisterId = s.RegisterId,
            Name = s.Name,
            Description = s.Description,
            MachineId = s.MachineId,
            VariableId = s.VariableId,
            CycleId = s.CycleId,
            Value = s.Value,
            DataType = s.DataType,
            StatusValueId = s.StatusValueId,
            TimeStamp = s.TimeStamp,
        }).ToList();
        return IndQuestResults.Result<List<RegisterView>>.Success(list);
    }
}
