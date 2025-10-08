// <copyright file="IDistinctRegisterService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Registers.Services;

public interface IDistinctRegisterService
{
    Task UpdateDistinctRegistersAsync(CancellationToken cancellationToken);

    Task<IEnumerable<DistinctRegister>> GetDistinctRegistersAsync(CancellationToken cancellationToken);
}
