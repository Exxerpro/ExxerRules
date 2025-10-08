// <copyright file="VariableListVm.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.WorkFlows.Queries.GetList;

using IndTrace.Application.Variables.Queries.GetVariableList;

/// <summary>
/// ViewModel representing a list of variables and their count.
/// </summary>
public class VariableListVm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VariableListVm"/> class.
    /// Initializes a new instance of the class.
    /// </summary>
    public VariableListVm()
    {
        this.VariableList = [];
    }

    /// <summary>
    /// Gets or sets the collection of variable DTOs.
    /// </summary>
    public ICollection<VariableDto> VariableList;

    /// <summary>
    /// Gets or sets the count of variables in the list.
    /// </summary>
    public int Count { get; set; }
}
