// <copyright file="ValueObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Domain.Models;

/// <summary>
/// Represents a base class for value objects in domain-driven design.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Determines whether the specified object is equal to the current value object.
    /// </summary>
    /// <param name="obj">The object to compare with the current value object.</param>
    /// <returns>True if equal, otherwise false.</returns>
    public override bool Equals(object? obj) // Updated to allow nullable object
    {
        if (obj == null || obj.GetType() != this.GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        using var thisValues = this.GetAtomicValues().GetEnumerator();
        using var otherValues = other.GetAtomicValues().GetEnumerator();

        while (thisValues.MoveNext() && otherValues.MoveNext())
        {
            if (thisValues.Current is null ^ otherValues.Current is null)
            {
                return false;
            }

            if (thisValues.Current != null &&
                !thisValues.Current.Equals(otherValues.Current))
            {
                return false;
            }
        }

        return !thisValues.MoveNext() && !otherValues.MoveNext();
    }

    /// <summary>
    /// Returns a hash code for the value object.
    /// </summary>
    /// <returns>A hash code for the value object.</returns>
    public override int GetHashCode()
    {
        return this.GetAtomicValues()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate(0, (x, y) => x ^ y);
    }

    /// <summary>
    /// Checks equality between two value objects using the equality operator.
    /// </summary>
    /// <param name="left">The left value object.</param>
    /// <param name="right">The right value object.</param>
    /// <returns>True if equal, otherwise false.</returns>
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }

        return left?.Equals(right) != false;
    }

    /// <summary>
    /// Checks inequality between two value objects using the inequality operator.
    /// </summary>
    /// <param name="left">The left value object.</param>
    /// <param name="right">The right value object.</param>
    /// <returns>True if not equal, otherwise false.</returns>
    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !EqualOperator(left, right);
    }

    /// <summary>
    /// Gets the atomic values of the value object for equality comparison.
    /// </summary>
    /// <returns>An enumerable of atomic values.</returns>
    protected abstract IEnumerable<object> GetAtomicValues();
}
