using IndTrace.Application.Performance.Request.Command.Create;
using IndTrace.Domain.Entities;

namespace IndTrace.DataStore.Services.OEE.Interfaces;

/// <summary>
/// Defines a contract for a service that provides an observable stream of OEE register DTOs.
/// </summary>
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IObservableService logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IObservableService
{
    /// <summary>
    /// Gets an observable stream of OEE register DTOs.
    /// </summary>
    IObservable<OeeRegisterDto> Stream { get; }
}
