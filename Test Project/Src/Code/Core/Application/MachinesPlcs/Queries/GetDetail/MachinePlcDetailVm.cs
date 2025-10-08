// <copyright file="MachinePlcDetailVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.MachinesPlcs.Queries.GetDetail;

/// <summary>
/// View model representing the relationship between a machine and a PLC (Programmable Logic Controller).
/// Contains details about the machine-PLC association including activation status.
/// </summary>
public class MachinePlcDetailVm
{
    /// <summary>
    /// Gets or sets the machine identifier.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the PLC (Programmable Logic Controller) identifier.
    /// </summary>
    public int PlcId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the machine-PLC association is active.
    /// A value of 1 indicates active, 0 indicates inactive.
    /// </summary>
    public int IsActive { get; set; }

    /// <summary>
    /// Converts a <see cref="MachinePlc"/> entity to a <see cref="MachinePlcDetailVm"/> view model.
    /// </summary>
    /// <param name="src">The source <see cref="MachinePlc"/> entity to convert.</param>
    /// <returns>A new <see cref="MachinePlcDetailVm"/> containing the entity's data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="src"/> is null.</exception>
    public static IndQuestResults.Result<MachinePlcDetailVm> ToDto(MachinePlc src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<MachinePlcDetailVm>.WithFailure("MachinePlc source cannot be null");
        }

        return IndQuestResults.Result<MachinePlcDetailVm>.Success(new MachinePlcDetailVm
        {
            MachineId = src.MachineId,
            PlcId = src.PlcId,
            IsActive = src.IsActive,
        });
    }

    /// <summary>
    /// Converts a <see cref="MachinePlcDetailVm"/> view model to a <see cref="MachinePlc"/> entity.
    /// </summary>
    /// <param name="src">The source <see cref="MachinePlcDetailVm"/> view model to convert.</param>
    /// <returns>A new <see cref="MachinePlc"/> entity containing the view model's data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="src"/> is null.</exception>
    public static IndQuestResults.Result<MachinePlc> ToEntity(MachinePlcDetailVm src)
    {
        if (src == null)
        {
            return IndQuestResults.Result<MachinePlc>.WithFailure("MachinePlcDetailVm source cannot be null");
        }

        return IndQuestResults.Result<MachinePlc>.Success(new MachinePlc(src.MachineId, src.PlcId, src.IsActive));
    }
}
