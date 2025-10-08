// <copyright file="IGatewayRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Interfaces;

/// <summary>
/// Represents a base interface for gateway request operations.
/// This interface serves as a marker for all gateway request types.
/// </summary>
public interface IGatewayRequest : IRequest;

/// <summary>
/// Represents a gateway request with a response type.
/// </summary>
public interface IGatewayRequest<TResponse> : IRequest<TResponse>;
