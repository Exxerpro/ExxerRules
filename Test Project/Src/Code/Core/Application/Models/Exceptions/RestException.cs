// <copyright file="RestException.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Exceptions;

using System.Net;

/// <summary>
/// Represents an exception for REST API errors, including HTTP status code and message.
/// </summary>
public class RestException(HttpStatusCode code, object? message = null) : Exception
{
    /// <summary>
    /// Gets the HTTP status code associated with the exception.
    /// </summary>
    public HttpStatusCode Code { get; } = code;

    /// <summary>
    /// Gets the error message associated with the exception.
    /// </summary>
    public new object Message { get; } = message ?? "No message provided";
}
