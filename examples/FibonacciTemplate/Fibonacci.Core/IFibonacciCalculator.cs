namespace Fibonacci.Core;

/// <summary>
/// Defines the contract for Fibonacci sequence calculations.
/// </summary>
public interface IFibonacciCalculator
{
    /// <summary>
    /// Calculates the Fibonacci sequence up to the specified number of terms.
    /// </summary>
    /// <param name="terms">The number of terms to calculate (must be positive).</param>
    /// <returns>A collection of Fibonacci numbers.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when terms is less than 1.</exception>
    IEnumerable<long> CalculateSequence(int terms);

    /// <summary>
    /// Calculates the nth Fibonacci number using the recursive approach.
    /// </summary>
    /// <param name="n">The position in the sequence (must be non-negative).</param>
    /// <returns>The nth Fibonacci number.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when n is negative.</exception>
    long CalculateNth(int n);

    /// <summary>
    /// Calculates the nth Fibonacci number using the iterative approach (more efficient).
    /// </summary>
    /// <param name="n">The position in the sequence (must be non-negative).</param>
    /// <returns>The nth Fibonacci number.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when n is negative.</exception>
    long CalculateNthIterative(int n);

    /// <summary>
    /// Validates if a number is a valid Fibonacci term count.
    /// </summary>
    /// <param name="terms">The number of terms to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    bool IsValidTermCount(int terms);

    /// <summary>
    /// Gets the maximum safe term count to avoid overflow.
    /// </summary>
    /// <returns>The maximum safe term count.</returns>
    int GetMaxSafeTermCount();
}
