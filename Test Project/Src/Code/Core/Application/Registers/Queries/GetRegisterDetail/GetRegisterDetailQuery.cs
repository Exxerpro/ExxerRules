// <copyright file="GetRegisterDetailQuery.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Registers.Queries.GetRegisterDetail;

/// <summary>
/// Represents the GetRegisterDetailQuery.
/// </summary>
public class GetRegisterDetailQuery : IMonitorRequest<RegisterDto>
{
    /// <summary>
    /// Gets or sets the RegisterId.
    /// </summary>
    public int RegisterId { get; set; }
}
