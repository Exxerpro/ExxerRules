// <copyright file="GetVariableDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Variables.Queries.GetVariableDetail;

/// <summary>
/// Represents the GetVariableDetailQuery.
/// </summary>
public class GetVariableDetailQuery : IMonitorRequest<VariableDetailVm>
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int Id { get; set; }
}
