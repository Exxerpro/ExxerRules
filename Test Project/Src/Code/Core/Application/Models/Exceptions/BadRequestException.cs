// <copyright file="BadRequestException.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Exceptions;

/// <summary>
/// Represents an exception that is thrown for bad requests.
/// </summary>
/// <param name="message">The error message.</param>
public class BadRequestException(string message) : Exception(message);
