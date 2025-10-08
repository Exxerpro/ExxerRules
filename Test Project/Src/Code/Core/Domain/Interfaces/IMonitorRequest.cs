// <copyright file="IMonitorRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Interfaces;

// CQRS QUERY PATTERN
// The IMonitorRequest interface inherits from the IMonitorRequest interface, meaning it can be processed by a guiCommandDispatcher.
// Returns Result<TResponse> which wraps both success and failure outcomes for the guiRequest.

/// <summary>
/// Represents a marker interface for Monitor requests in the CQRS pattern.
/// </summary>
public interface IMonitorRequest : IRequest;

/// <summary>
/// Represents a Monitor request with a response type.
/// </summary>
public interface IMonitorRequest<TResponse> : IRequest<TResponse>;
