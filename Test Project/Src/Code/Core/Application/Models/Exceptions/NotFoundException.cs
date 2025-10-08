// <copyright file="NotFoundException.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an entity is not found.
/// </summary>
/// <param name="name">The name of the entity.</param>
/// <param name="key">The key of the entity.</param>
public class NotFoundException(string name, object? key) : Exception($"Entity \"{name}\" ({key}) was not found.");
