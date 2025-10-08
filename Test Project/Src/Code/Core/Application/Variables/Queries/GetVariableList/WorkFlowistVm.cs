// <copyright file="WorkFlowistVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Variables.Queries.GetVariableList;

/// <summary>
/// Represents the WorkFlowistVm.
/// </summary>
public class WorkFlowistVm
{
    /// <summary>
    /// Gets or sets set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public IList<VariableDto> Variables { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Count.
    /// </summary>
    public int Count { get; set; }
}
