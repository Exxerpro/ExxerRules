using System;
using Calculator.Core.Models;
using Calculator.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Calculator.ConsoleApp;

/// <summary>
/// Main program class for the Calculator console application.
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using var scope = host.Services.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<CalculatorApp>();

        app.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<ICalculatorService, CalculatorService>();
                services.AddSingleton<CalculatorApp>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });
}

/// <summary>
/// Main calculator application class.
/// </summary>
public class CalculatorApp
{
    private readonly ICalculatorService _calculator;
    private readonly ILogger<CalculatorApp> _logger;

    /// <summary>
    /// Initializes a new instance of the CalculatorApp class.
    /// </summary>
    /// <param name="calculator">The calculator service</param>
    /// <param name="logger">The logger instance</param>
    public CalculatorApp(ICalculatorService calculator, ILogger<CalculatorApp> logger)
    {
        _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Runs the calculator application.
    /// </summary>
    public void Run()
    {
        _logger.LogInformation("Starting Calculator Application");

        Console.WriteLine("=== Calculator Console Application ===");
        Console.WriteLine("Commands:");
        Console.WriteLine("  add <num1> <num2>     - Add two numbers");
        Console.WriteLine("  sub <num1> <num2>     - Subtract two numbers");
        Console.WriteLine("  mul <num1> <num2>     - Multiply two numbers");
        Console.WriteLine("  div <num1> <num2>     - Divide two numbers");
        Console.WriteLine("  pow <num1> <num2>     - Raise num1 to power of num2");
        Console.WriteLine("  sqrt <num>            - Square root of number");
        Console.WriteLine("  pct <num1> <num2>     - num1 percent of num2");
        Console.WriteLine("  store <num>           - Store number in memory");
        Console.WriteLine("  recall                - Recall number from memory");
        Console.WriteLine("  clear                 - Clear memory");
        Console.WriteLine("  history               - Show calculation history");
        Console.WriteLine("  clear-history         - Clear calculation history");
        Console.WriteLine("  help                  - Show this help");
        Console.WriteLine("  exit                  - Exit application");
        Console.WriteLine();

        while (true)
        {
            try
            {
                Console.Write("calc> ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.ToLowerInvariant() == "exit")
                {
                    _logger.LogInformation("Goodbye!");
                    break;
                }

                ProcessCommand(input);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing command");
                _logger.LogInformation($"Error: {ex.Message}");
            }
        }
    }

    private void ProcessCommand(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return;

        var command = parts[0].ToLowerInvariant();

        switch (command)
        {
            case "add":
                HandleBinaryOperation(OperationType.Add, parts);
                break;

            case "sub":
                HandleBinaryOperation(OperationType.Subtract, parts);
                break;

            case "mul":
                HandleBinaryOperation(OperationType.Multiply, parts);
                break;

            case "div":
                HandleBinaryOperation(OperationType.Divide, parts);
                break;

            case "pow":
                HandleBinaryOperation(OperationType.Power, parts);
                break;

            case "sqrt":
                HandleUnaryOperation(OperationType.SquareRoot, parts);
                break;

            case "pct":
                HandleBinaryOperation(OperationType.Percentage, parts);
                break;

            case "store":
                HandleStoreCommand(parts);
                break;

            case "recall":
                HandleRecallCommand();
                break;

            case "clear":
                HandleClearCommand();
                break;

            case "history":
                HandleHistoryCommand();
                break;

            case "clear-history":
                HandleClearHistoryCommand();
                break;

            case "help":
                ShowHelp();
                break;

            default:
                _logger.LogInformation($"Unknown command: {command}. Type 'help' for available commands.");
                break;
        }
    }

    private void HandleBinaryOperation(OperationType operation, string[] parts)
    {
        if (parts.Length != 3)
        {
            _logger.LogInformation($"Usage: {parts[0]} <number1> <number2>");
            return;
        }

        if (!decimal.TryParse(parts[1], out var left) || !decimal.TryParse(parts[2], out var right))
        {
            _logger.LogInformation("Error: Invalid numbers provided");
            return;
        }

        var result = _calculator.Calculate(left, right, operation);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Result: {result.Value}");
        }
        else
        {
            _logger.LogInformation($"Error: {result.ErrorMessage}");
        }
    }

    private void HandleUnaryOperation(OperationType operation, string[] parts)
    {
        if (parts.Length != 2)
        {
            _logger.LogInformation($"Usage: {parts[0]} <number>");
            return;
        }

        if (!decimal.TryParse(parts[1], out var value))
        {
            _logger.LogInformation("Error: Invalid number provided");
            return;
        }

        var result = _calculator.Calculate(value, operation);

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Result: {result.Value}");
        }
        else
        {
            _logger.LogInformation($"Error: {result.ErrorMessage}");
        }
    }

    private void HandleStoreCommand(string[] parts)
    {
        if (parts.Length != 2)
        {
            _logger.LogInformation("Usage: store <number>");
            return;
        }

        if (!decimal.TryParse(parts[1], out var value))
        {
            _logger.LogInformation("Error: Invalid number provided");
            return;
        }

        _calculator.StoreMemory(value);
        _logger.LogInformation($"Stored {value} in memory");
    }

    private void HandleRecallCommand()
    {
        var value = _calculator.RecallMemory();
        if (value.HasValue)
        {
            _logger.LogInformation($"Memory: {value.Value}");
        }
        else
        {
            _logger.LogInformation("Memory is empty");
        }
    }

    private void HandleClearCommand()
    {
        _calculator.ClearMemory();
        _logger.LogInformation("Memory cleared");
    }

    private void HandleHistoryCommand()
    {
        var history = _calculator.GetHistory();
        if (history.Count == 0)
        {
            _logger.LogInformation("No calculation history");
            return;
        }

        _logger.LogInformation("Calculation History:");
        foreach (var entry in history)
        {
            _logger.LogInformation($"  {entry}");
        }
    }

    private void HandleClearHistoryCommand()
    {
        _calculator.ClearHistory();
        _logger.LogInformation("History cleared");
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Available commands:");
        Console.WriteLine("  add <num1> <num2>     - Add two numbers");
        Console.WriteLine("  sub <num1> <num2>     - Subtract two numbers");
        Console.WriteLine("  mul <num1> <num2>     - Multiply two numbers");
        Console.WriteLine("  div <num1> <num2>     - Divide two numbers");
        Console.WriteLine("  pow <num1> <num2>     - Raise num1 to power of num2");
        Console.WriteLine("  sqrt <num>            - Square root of number");
        Console.WriteLine("  pct <num1> <num2>     - num1 percent of num2");
        Console.WriteLine("  store <num>           - Store number in memory");
        Console.WriteLine("  recall                - Recall number from memory");
        Console.WriteLine("  clear                 - Clear memory");
        Console.WriteLine("  history               - Show calculation history");
        Console.WriteLine("  clear-history         - Clear calculation history");
        Console.WriteLine("  help                  - Show this help");
        Console.WriteLine("  exit                  - Exit application");
    }
}