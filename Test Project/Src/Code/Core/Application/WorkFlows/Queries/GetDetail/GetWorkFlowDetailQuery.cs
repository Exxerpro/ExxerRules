// <copyright file="GetWorkFlowDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.WorkFlows.Queries.GetDetail;

/// <summary>
/// Represents the GetWorkFlowDetailQuery.
/// </summary>
public class GetWorkFlowDetailQuery : IMonitorRequest<List<WorkFlowDetailVm>>
{
    /// <summary>
    /// Gets or sets the NoParte.
    /// </summary>
    public string? NoParte { get; set; }
}
