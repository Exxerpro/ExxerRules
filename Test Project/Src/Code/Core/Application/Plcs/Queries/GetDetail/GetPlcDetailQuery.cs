// <copyright file="GetPlcDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Plcs.Queries.GetDetail;

/// <summary>
/// Represents the GetPlcDetailQuery.
/// </summary>
public class GetPlcDetailQuery : IMonitorRequest<PlcDto>
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int Id { get; set; }
}
