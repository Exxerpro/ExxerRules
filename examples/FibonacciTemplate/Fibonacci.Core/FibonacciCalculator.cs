namespace Fibonacci.Core;

/// <summary>
/// Provides Fibonacci sequence calculations with both recursive and iterative approaches.
/// </summary>
public class FibonacciCalculator : IFibonacciCalculator
{
    private const int MaxSafeTermCount = 92; // Beyond this, long overflows

    /// <summary>
    /// Calculates the Fibonacci sequence up to the specified number of terms.
    /// </summary>
    /// <param name="terms">The number of terms to calculate (must be positive).</param>
    /// <returns>A collection of Fibonacci numbers.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when terms is less than 1.</exception>
    public IEnumerable<long> CalculateSequence(int terms)
    {
        if (!IsValidTermCount(terms))
        {
            throw new ArgumentOutOfRangeException(nameof(terms), 
                $"Term count must be between 1 and {MaxSafeTermCount}. Got: {terms}");
        }

        return GenerateSequence(terms);
    }

    /// <summary>
    /// Calculates the nth Fibonacci number using the recursive approach.
    /// </summary>
    /// <param name="n">The position in the sequence (must be non-negative).</param>
    /// <returns>The nth Fibonacci number.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when n is negative.</exception>
    public long CalculateNth(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), 
                "Position must be non-negative. Got: " + n);
        }

        if (n > MaxSafeTermCount)
        {
            throw new ArgumentOutOfRangeException(nameof(n), 
                $"Position must not exceed {MaxSafeTermCount} to avoid overflow. Got: {n}");
        }

        return CalculateNthRecursive(n);
    }

    /// <summary>
    /// Calculates the nth Fibonacci number using the iterative approach (more efficient).
    /// </summary>
    /// <param name="n">The position in the sequence (must be non-negative).</param>
    /// <returns>The nth Fibonacci number.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when n is negative.</exception>
    public long CalculateNthIterative(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), 
                "Position must be non-negative. Got: " + n);
        }

        if (n > MaxSafeTermCount)
        {
            throw new ArgumentOutOfRangeException(nameof(n), 
                $"Position must not exceed {MaxSafeTermCount} to avoid overflow. Got: {n}");
        }

        if (n <= 1)
            return n;

        long a = 0, b = 1;
        for (int i = 2; i <= n; i++)
        {
            long temp = a + b;
            a = b;
            b = temp;
        }

        return b;
    }

    /// <summary>
    /// Validates if a number is a valid Fibonacci term count.
    /// </summary>
    /// <param name="terms">The number of terms to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    public bool IsValidTermCount(int terms)
    {
        return terms >= 1 && terms <= MaxSafeTermCount;
    }

    /// <summary>
    /// Gets the maximum safe term count to avoid overflow.
    /// </summary>
    /// <returns>The maximum safe term count.</returns>
    public int GetMaxSafeTermCount() => MaxSafeTermCount;

    private static IEnumerable<long> GenerateSequence(int terms)
    {
        long a = 0, b = 1;
        
        for (int i = 0; i < terms; i++)
        {
            yield return a;
            long temp = a + b;
            a = b;
            b = temp;
        }
    }

    private static long CalculateNthRecursive(int n)
    {
        if (n <= 1)
            return n;

        return CalculateNthRecursive(n - 1) + CalculateNthRecursive(n - 2);
    }
}
