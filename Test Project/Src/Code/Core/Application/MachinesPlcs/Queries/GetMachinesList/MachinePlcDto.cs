// <copyright file="MachinePlcDto.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.MachinesPlcs.Queries.GetMachinesList;

/// <summary>
/// Represents the MachinePlcDto.
/// </summary>
public class MachinePlcDto
{
    /// <summary>
    /// Gets or sets the MachineId.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the PlcId.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Executes ToDto operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToDto.</returns>
    public static IndQuestResults.Result<MachinePlcDto> ToDto(MachinePlc src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<MachinePlcDto>.WithFailure("MachinePlc source cannot be null");
        }

        return IndQuestResults.Result<MachinePlcDto>.Success(new MachinePlcDto
        {
            MachineId = src.MachineId,
            PlcId = src.PlcId,
        });
    }

    /// <summary>
    /// Executes ToEntity operation.
    /// </summary>
    /// <param name="src">The src.</param>
    /// <returns>The result of ToEntity.</returns>
    public static IndQuestResults.Result<MachinePlc> ToEntity(MachinePlcDto src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<MachinePlc>.WithFailure("MachinePlcDto source cannot be null");
        }

        return IndQuestResults.Result<MachinePlc>.Success(new MachinePlc(src.MachineId, src.PlcId, 1));
    }
}
