// <copyright file="ValidationException.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Exceptions;

using FluentValidation.Results;

/// <summary>
/// Represents an exception that is thrown when one or more validation failures occur.
/// </summary>
public class ValidationException() : Exception("One or more validation failures have occurred.")
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a list of validation failures.
    /// </summary>
    /// <param name="failures">The list of validation failures.</param>
    public ValidationException(List<ValidationFailure> failures)
        : this()
    {
        var propertyNames = failures
            .Select(e => e.PropertyName)
            .Distinct();

        foreach (var propertyName in propertyNames)
        {
            var propertyFailures = failures
                .Where(e => e.PropertyName == propertyName)
                .Select(e => e.ErrorMessage)
                .ToArray();

            this.Failures.Add(propertyName, propertyFailures);
        }
    }

    /// <summary>
    /// Gets the validation failures, grouped by property name.
    /// </summary>
    public IDictionary<string, string[]> Failures { get; } = new Dictionary<string, string[]>();
}
