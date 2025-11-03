using System;
using System.Threading.Tasks;

namespace TestProject;

/// <summary>
/// A minimal test class for linting tests.
/// </summary>
public class TestClass
{
    private string _name = "Test";

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name 
    { 
        get => _name; 
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Performs a test operation.
    /// </summary>
    /// <param name="value">The value to process.</param>
    /// <returns>The processed value.</returns>
    public int ProcessValue(int value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Value cannot be negative", nameof(value));
        }

        return value * 2;
    }

    // This method violates EXXER rules - missing CancellationToken
    public async Task<string> GetDataAsync()
    {
        await Task.Delay(100);
        return "data";
    }

    // This method violates EXXER rules - uses Console.WriteLine
    public void LogMessage(string message)
    {
        Console.WriteLine(message);
    }

    // This method violates EXXER rules - uses regions
    //  Helper Methods
    private void HelperMethod()
    {
        // Helper implementation
    }
     // 
}