// <copyright file="RequestFunctionalHandlerDelegate.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.RequestHandler;

/// <summary>
/// Represents a delegate for handling a request and returning a response asynchronously.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
public delegate Task<TResponse> RequestFunctionalHandlerDelegate<TResponse>();

public delegate Task<Result<TResponse>> RequestHandlerDelegate<TResponse>();
