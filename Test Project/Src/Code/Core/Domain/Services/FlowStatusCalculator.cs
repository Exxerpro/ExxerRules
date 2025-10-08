// <copyright file="FlowStatusCalculator.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Services;

using IndTrace.Domain.Services.Interfaces;
using IndTrace.Domain.Enum;

/// <summary>
/// Calculates flow status based on business rules.
/// </summary>
public class FlowStatusCalculator : IFlowStatusCalculator
{
    /// <inheritdoc/>
    public FlowStatus Calculate(
        MachineType machineType,
        CycleStatus cycleStatus,
        PartStatus partStatus)
    {
        return Result<(MachineType Type, CycleStatus Status, PartStatus Part)>
            .Success((machineType, cycleStatus, partStatus))
            .Map(context => IsFlowFinished(context.Type, context.Status)
                ? FlowStatus.Finished
                : FlowStatus.InProcess)
            .Value!;
    }

    private static bool IsFlowFinished(MachineType machineType, CycleStatus cycleStatus)
    {
        return machineType == MachineType.Final && cycleStatus == CycleStatus.FinishedOk;
    }
}
