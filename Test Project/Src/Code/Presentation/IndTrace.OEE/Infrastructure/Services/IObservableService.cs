// <copyright file="IObservableService.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

using IndTrace.Application.Performance.Request.Command.Create;

namespace IndTrace.OEE.Infrastructure.Services;

using IndTrace.Domain.Entities;

/// <summary>
/// Defines the contract for an observable service that provides a stream of OEE register data.
/// </summary>
public interface IObservableService
{
    /// <summary>
    /// Gets the observable stream of OEE register data.
    /// </summary>
    IObservable<OeeRegisterDto> Stream { get; }
}
