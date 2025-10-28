namespace Calculator;

/// <summary>
/// A basic calculator implementation that provides fundamental arithmetic operations.
/// </summary>
public class BasicCalculator : ICalculator
{
    /// <summary>
    /// Adds two numbers.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns>The sum of the two numbers.</returns>
    public double Add(double a, double b) => a + b;

    /// <summary>
    /// Subtracts the second number from the first.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns>The difference of the two numbers.</returns>
    public double Subtract(double a, double b) => a - b;

    /// <summary>
    /// Multiplies two numbers.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns>The product of the two numbers.</returns>
    public double Multiply(double a, double b) => a * b;

    /// <summary>
    /// Divides the first number by the second.
    /// </summary>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The quotient of the division.</returns>
    /// <exception cref="DivideByZeroException">Thrown when the divisor is zero.</exception>
    public double Divide(double a, double b)
    {
        if (Math.Abs(b) < double.Epsilon)
            throw new DivideByZeroException("Cannot divide by zero.");
        
        return a / b;
    }

    /// <summary>
    /// Calculates the power of a number.
    /// </summary>
    /// <param name="baseNumber">The base number.</param>
    /// <param name="exponent">The exponent.</param>
    /// <returns>The result of raising the base to the power of the exponent.</returns>
    public double Power(double baseNumber, double exponent) => Math.Pow(baseNumber, exponent);

    /// <summary>
    /// Calculates the square root of a number.
    /// </summary>
    /// <param name="number">The number to calculate the square root of.</param>
    /// <returns>The square root of the number.</returns>
    /// <exception cref="ArgumentException">Thrown when the number is negative.</exception>
    public double SquareRoot(double number)
    {
        if (number < 0)
            throw new ArgumentException("Cannot calculate square root of negative number.", nameof(number));
        
        return Math.Sqrt(number);
    }
}
