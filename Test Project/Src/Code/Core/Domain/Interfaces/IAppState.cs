// <copyright file="IAppState.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Interfaces;

/// <summary>
/// Marker interface for app-wide shared state entities (configs, app status, health, etc.).
/// Entities implementing this interface represent global application state that is shared
/// across the entire system and typically have singleton-like behavior in the database.
/// </summary>
/// <remarks>
/// Examples of app state entities:
/// - Application configuration settings
/// - System health status
/// - Global feature flags
/// - License information
/// - System maintenance status
/// - Audit settings
/// </remarks>
public interface IAppState
{
}
