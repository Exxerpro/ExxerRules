// <copyright file="IMediatorAssemblyMarker.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Mediator;

/// <summary>
/// Marker interface for identifying the application assembly for mediator registration.
/// </summary>
/// <remarks>
/// This interface is used by dependency injection containers to locate the application assembly
/// during automated registration of mediator handlers and services.
/// </remarks>
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate mediator marker interface logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
public interface IMediatorAssemblyMarker
{
}
