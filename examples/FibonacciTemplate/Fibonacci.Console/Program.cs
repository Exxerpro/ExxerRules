using Fibonacci.Core;

namespace Fibonacci.ConsoleApp;

/// <summary>
/// Console application for calculating and displaying Fibonacci sequences.
/// </summary>
class Program
{
    private static readonly IFibonacciCalculator Calculator = new FibonacciCalculator();

    /// <summary>
    /// Main entry point of the application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    static void Main(string[] args)
    {
        System.Console.WriteLine("=== Fibonacci Sequence Calculator ===");
        System.Console.WriteLine();

        try
        {
            if (args.Length > 0 && int.TryParse(args[0], out int terms))
            {
                // Command line argument provided
                CalculateAndDisplaySequence(terms);
            }
            else
            {
                // Interactive mode
                RunInteractiveMode();
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error: {ex.Message}");
            System.Console.WriteLine("Press any key to exit...");
            System.Console.ReadKey();
        }
    }

    /// <summary>
    /// Runs the application in interactive mode, prompting the user for input.
    /// </summary>
    private static void RunInteractiveMode()
    {
        while (true)
        {
            try
            {
                System.Console.WriteLine($"Enter the number of Fibonacci terms to calculate (1-{Calculator.GetMaxSafeTermCount()}, or 'q' to quit):");
                var input = System.Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "q")
                {
                    System.Console.WriteLine("Goodbye!");
                    break;
                }

                if (int.TryParse(input, out int terms))
                {
                    CalculateAndDisplaySequence(terms);
                }
                else
                {
                    System.Console.WriteLine("Invalid input. Please enter a valid number or 'q' to quit.");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }

            System.Console.WriteLine();
        }
    }

    /// <summary>
    /// Calculates and displays the Fibonacci sequence for the specified number of terms.
    /// </summary>
    /// <param name="terms">The number of terms to calculate.</param>
    private static void CalculateAndDisplaySequence(int terms)
    {
        System.Console.WriteLine($"Calculating Fibonacci sequence with {terms} terms...");
        System.Console.WriteLine();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var sequence = Calculator.CalculateSequence(terms).ToArray();
        stopwatch.Stop();

        DisplaySequence(sequence);
        DisplayPerformanceInfo(sequence.Length, stopwatch.Elapsed);
    }

    /// <summary>
    /// Displays the Fibonacci sequence in a formatted manner.
    /// </summary>
    /// <param name="sequence">The Fibonacci sequence to display.</param>
    private static void DisplaySequence(long[] sequence)
    {
        System.Console.WriteLine("Fibonacci Sequence:");
        System.Console.WriteLine(new string('=', 50));

        for (int i = 0; i < sequence.Length; i++)
        {
            System.Console.WriteLine($"F({i,2}) = {sequence[i],15:N0}");
        }

        System.Console.WriteLine(new string('=', 50));
    }

    /// <summary>
    /// Displays performance information about the calculation.
    /// </summary>
    /// <param name="termCount">The number of terms calculated.</param>
    /// <param name="elapsed">The time taken for the calculation.</param>
    private static void DisplayPerformanceInfo(int termCount, TimeSpan elapsed)
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Performance Information:");
        System.Console.WriteLine($"- Terms calculated: {termCount}");
        System.Console.WriteLine($"- Time elapsed: {elapsed.TotalMilliseconds:F2} ms");
        System.Console.WriteLine($"- Average time per term: {elapsed.TotalMilliseconds / termCount:F4} ms");
        
        if (termCount > 0)
        {
            System.Console.WriteLine($"- Last term (F{termCount - 1}): {GetLastTerm(termCount):N0}");
        }
    }

    /// <summary>
    /// Gets the last term in the sequence using the iterative method for comparison.
    /// </summary>
    /// <param name="termCount">The number of terms.</param>
    /// <returns>The last term in the sequence.</returns>
    private static long GetLastTerm(int termCount)
    {
        if (termCount <= 0) return 0;
        if (termCount == 1) return 0;
        if (termCount == 2) return 1;

        return Calculator.CalculateNthIterative(termCount - 1);
    }
}
