namespace Calculator;

/// <summary>
/// Defines the contract for calculator operations.
/// </summary>
public interface ICalculator
{
    /// <summary>
    /// Adds two numbers.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns>The sum of the two numbers.</returns>
    double Add(double a, double b);

    /// <summary>
    /// Subtracts the second number from the first.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns>The difference of the two numbers.</returns>
    double Subtract(double a, double b);

    /// <summary>
    /// Multiplies two numbers.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns>The product of the two numbers.</returns>
    double Multiply(double a, double b);

    /// <summary>
    /// Divides the first number by the second.
    /// </summary>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The quotient of the division.</returns>
    /// <exception cref="DivideByZeroException">Thrown when the divisor is zero.</exception>
    double Divide(double a, double b);

    /// <summary>
    /// Calculates the power of a number.
    /// </summary>
    /// <param name="baseNumber">The base number.</param>
    /// <param name="exponent">The exponent.</param>
    /// <returns>The result of raising the base to the power of the exponent.</returns>
    double Power(double baseNumber, double exponent);

    /// <summary>
    /// Calculates the square root of a number.
    /// </summary>
    /// <param name="number">The number to calculate the square root of.</param>
    /// <returns>The square root of the number.</returns>
    /// <exception cref="ArgumentException">Thrown when the number is negative.</exception>
    double SquareRoot(double number);
}
