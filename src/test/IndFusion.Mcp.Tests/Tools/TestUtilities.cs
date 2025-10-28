namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Helper functions for test inputs: locating the solution, creating files,
/// and providing canned sample source code for tool tests.
/// </summary>
public static class TestUtilities
{
    private static readonly Lazy<string> ExampleCodeContent = new(() =>
    {
        var projectDir = GetTestProjectDirectory();
        var calculatorPath = Path.Combine(projectDir, "Calculator.cs");
        if (File.Exists(calculatorPath))
        {
            return NormalizeToLf(File.ReadAllText(calculatorPath));
        }

        const string fallback = """
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndFusion.Mcp.Tests.Sample;

public class Calculator
{
    private List<int> numbers = new();
    private readonly string operatorSymbol;

    public Calculator(string op)
    {
        operatorSymbol = op;
    }

    public int Calculate(int a, int b)
    {
        if (a < 0 || b < 0)
        {
            throw new ArgumentException("Negative numbers not allowed");
        }

        var result = a + b;
        numbers.Add(result);
        Console.WriteLine($"Result: {result}");
        return result;
    }

    public double GetAverage()
    {
        return numbers.Sum() / (double)numbers.Count;
    }

    public string FormatResult(int value)
    {
        return $"The calculation result is: {value * 2 + 10}";
    }

    public string GetFormattedNumber(int number)
    {
        return $"{operatorSymbol}: {number}";
    }

    private string format = "Currency";

    public void SetFormat(string newFormat)
    {
        format = newFormat;
    }
}
""";

        return NormalizeToLf(fallback);
    });

    private static string ExampleCode => NormalizeToCrLf(ExampleCodeContent.Value);

    private static string ExampleCodePath => Path.Combine(GetTestProjectDirectory(), "ExampleCode.cs");

    internal static string GetTestProjectDirectory()
    {
        var currentDir = Directory.GetCurrentDirectory();

        // Try to find the test project directory by walking up from current directory
        var dir = new DirectoryInfo(currentDir);
        while (dir != null)
        {
            var testProjectDir = Path.Combine(dir.FullName, "src", "test", "IndFusion.Mcp.Tests");
            if (Directory.Exists(testProjectDir))
            {
                return testProjectDir;
            }

            // Also try without src folder structure
            testProjectDir = Path.Combine(dir.FullName, "test", "IndFusion.Mcp.Tests");
            if (Directory.Exists(testProjectDir))
            {
                return testProjectDir;
            }

            // Try direct folder
            testProjectDir = Path.Combine(dir.FullName, "IndFusion.Mcp.Tests");
            if (Directory.Exists(testProjectDir) && File.Exists(Path.Combine(testProjectDir, "IndFusion.Mcp.Tests.csproj")))
            {
                return testProjectDir;
            }

            dir = dir.Parent;
        }

        // Fallback - assume current directory is the test project directory if it contains the project file
        if (File.Exists(Path.Combine(currentDir, "IndFusion.Mcp.Tests.csproj")))
        {
            return currentDir;
        }

        // Final fallback
        return Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", "test", "IndFusion.Mcp.Tests"));
    }

    private static string NormalizeToLf(string text)
    {
        return text.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    private static string NormalizeToCrLf(string text)
    {
        return NormalizeToLf(text).Replace("\n", "\r\n");
    }


    /// <summary>
    /// Ensures the example code file exists with the correct content.
    /// </summary>
    public static void EnsureExampleCodeFile()
    {
        var path = ExampleCodePath;
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var desired = ExampleCode;
        if (!File.Exists(path) || NormalizeToLf(File.ReadAllText(path)) != NormalizeToLf(desired))
        {
            File.WriteAllText(path, desired);
        }
    }

    private static string GetExampleCode()
    {
        EnsureExampleCodeFile();
        return ExampleCode;
    }

    /// <summary>
    /// Gets the selection range for a snippet within source text.
    /// </summary>
    /// <param name="sourceText">The source text to search in.</param>
    /// <param name="snippet">The snippet to find.</param>
    /// <param name="occurrence">The occurrence index to find (0-based).</param>
    /// <returns>The selection range in format "startLine:startColumn-endLine:endColumn".</returns>
    public static string GetSelectionRange(string sourceText, string snippet, int occurrence = 0)
    {
        if (snippet is null)
        {
            throw new ArgumentNullException(nameof(snippet));
        }

        var normalizedSource = NormalizeToLf(sourceText);
        var normalizedSnippet = NormalizeToLf(snippet);

        var searchIndex = 0;
        var matchIndex = -1;
        for (var i = 0; i <= occurrence; i++)
        {
            matchIndex = normalizedSource.IndexOf(normalizedSnippet, searchIndex, StringComparison.Ordinal);
            if (matchIndex < 0)
            {
                throw new InvalidOperationException($"Snippet '{snippet}' not found in provided source.");
            }

            searchIndex = matchIndex + 1;
        }

        var startLine = normalizedSource[..matchIndex].Count(c => c == '\n') + 1;
        var lastNewlineBeforeStart = normalizedSource.LastIndexOf('\n', matchIndex - 1);
        var startColumn = matchIndex - (lastNewlineBeforeStart >= 0 ? lastNewlineBeforeStart : -1);

        var endIndex = matchIndex + normalizedSnippet.Length - 1;
        var endLine = normalizedSource[..(endIndex + 1)].Count(c => c == '\n') + 1;
        var lastNewlineBeforeEnd = normalizedSource.LastIndexOf('\n', endIndex);
        var endColumn = endIndex - (lastNewlineBeforeEnd >= 0 ? lastNewlineBeforeEnd : -1);

        return $"{startLine}:{startColumn}-{endLine}:{endColumn}";
    }

    /// <summary>
    /// Gets a lightweight test solution path to avoid hanging on full solution load.
    /// Creates a minimal solution if it doesn't exist.
    /// </summary>
    public static string GetSolutionPath()
    {
        var testDir = GetTestProjectDirectory();
        var testSolutionPath = Path.Combine(testDir, "TestOutput", "TestSolution.sln");

        EnsureTestSolution(testSolutionPath);
        return testSolutionPath;
    }

    private static void EnsureTestSolution(string solutionPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(solutionPath)!);

        if (File.Exists(solutionPath))
            return;

        var solutionDir = Path.GetDirectoryName(solutionPath)!;
        var projectDir = Path.Combine(solutionDir, "TestProject");
        var projectPath = Path.Combine(projectDir, "TestProject.csproj");

        // Create project directory
        Directory.CreateDirectory(projectDir);

        // Create a minimal test project
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
              </PropertyGroup>
            </Project>
            """;

        File.WriteAllText(projectPath, projectContent);

        // Create a minimal C# file for the test project
        var csFilePath = Path.Combine(projectDir, "TestClass.cs");
        var csFileContent = """
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
                #region Helper Methods
                private void HelperMethod()
                {
                    // Helper implementation
                }
                #endregion
            }
            """;

        File.WriteAllText(csFilePath, csFileContent);

        // Create a minimal solution file
        var solutionContent = """
            Microsoft Visual Studio Solution File, Format Version 12.00
            # Visual Studio Version 17
            VisualStudioVersion = 17.0.31903.59
            MinimumVisualStudioVersion = 10.0.40219.1
            Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "TestProject", "TestProject\TestProject.csproj", "{12345678-1234-5678-9ABC-123456789ABC}"
            EndProject
            Global
                GlobalSection(SolutionConfigurationPlatforms) = preSolution
                    Debug|Any CPU = Debug|Any CPU
                    Release|Any CPU = Release|Any CPU
                EndGlobalSection
                GlobalSection(ProjectConfigurationPlatforms) = postSolution
                    {12345678-1234-5678-9ABC-123456789ABC}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                    {12345678-1234-5678-9ABC-123456789ABC}.Debug|Any CPU.Build.0 = Debug|Any CPU
                    {12345678-1234-5678-9ABC-123456789ABC}.Release|Any CPU.ActiveCfg = Release|Any CPU
                    {12345678-1234-5678-9ABC-123456789ABC}.Release|Any CPU.Build.0 = Release|Any CPU
                EndGlobalSection
            EndGlobal
            """;

        File.WriteAllText(solutionPath, solutionContent);
    }

    /// <summary>
    /// Creates a file at the given path with the specified content.
    /// </summary>
    public static async Task CreateTestFile(string filePath, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        var normalized = NormalizeToCrLf(content);
        await File.WriteAllTextAsync(filePath, normalized, cancellationToken: Xunit.TestContext.Current.CancellationToken);
    }

    /// <summary> ///  GetSampleCodeForExtractMethod. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForExtractMethod() => """
using System;
public class TestClass
{
    public int Calculate(int a, int b)
    {
        if (a < 0 || b < 0)
        {
            throw new ArgumentException(\"Negative numbers not allowed\");
        }

        var result = a + b;
        return result;
    }
}
""";

    /// <summary> ///  GetSampleCodeForIntroduceField. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForIntroduceField() =>
        GetExampleCode();

    /// <summary> ///  GetSampleCodeForIntroduceVariable. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForIntroduceVariable() =>
        GetExampleCode();

    /// <summary> ///  GetSampleCodeForMakeFieldReadonly. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForMakeFieldReadonly() =>
        GetExampleCode();

    /// <summary>
    /// Provides sample code for a class with a field declared but not initialized.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForMakeFieldReadonlyNoInit() => """
using System;
public class TestClass
{
    private string description;
}
""";

    /// <summary> ///  GetSampleCodeForTransformSetter. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForTransformSetter() =>
        GetExampleCode();

    /// <summary> ///  GetSampleCodeForConvertToStaticInstance. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForConvertToStaticInstance() =>
        GetExampleCode();

    /// <summary>
    /// Provides sample code used by tests that move a static method between types.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForMoveStaticMethod() =>
        GetExampleCode();

    /// <summary>
    /// Provides sample code for moving a static method where using directives are involved.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForMoveStaticMethodWithUsings() => """
using System;
using System.Collections.Generic;

public class TestClass
{
    public static void PrintList(List<int> numbers)
    {
        Console.WriteLine(string.Join(",", numbers));
    }
}

/// <summary> /// Represents the  UtilClass  class. /// </summary>
public class UtilClass { }
""";

    /// <summary> ///  GetSampleCodeForMoveInstanceMethod. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForMoveInstanceMethod() =>
        GetExampleCode();

    /// <summary> ///  GetSampleCodeForConvertToExtension. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForConvertToExtension() =>
        GetExampleCode();

    /// <summary>
    /// Provides sample code used by tests that verify safe deletion of code elements.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForSafeDelete() =>
        GetExampleCode();

    /// <summary>
    /// Provides sample code for moving an instance method that has dependencies.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForMoveInstanceMethodWithDependencies() => """
using System;
using System.Collections.Generic;

namespace Test.Domain
{
    public class OrderProcessor
    {
        private readonly string processorId;
        private List<string> log = new();

        public OrderProcessor(string id)
        {
            processorId = id;
        }

        public bool ValidateOrder(decimal amount)
        {
            return amount > 0;
        }

        // This method should be moved to PaymentService
        private bool ProcessPayment(decimal amount, string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
                return false;

            log.Add($"Processing payment of {amount} for processor {processorId}");

            // Simulate payment processing
            return amount <= 1000;
        }

        public void CompleteOrder(decimal amount, string cardNumber)
        {
            if (ValidateOrder(amount) && ProcessPayment(amount, cardNumber))
            {
                log.Add("Order completed successfully");
            }
        }
    }

    /// <summary> /// Represents the  PaymentService  class. /// </summary>
    public class PaymentService
    {
        // Target class for the moved method
    }
}
""";

    /// <summary>
    /// Provides sample code for inlining a method into its call site.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForInlineMethod() => """
using System;

public class InlineSample
{
    private void Helper()
    {
        Console.WriteLine("Hi");
    }

    /// <summary> ///  Call. /// </summary>
    public void Call()
    {
        Helper();
        Console.WriteLine("Done");
    }
}
""";

    /// <summary> ///  GetSampleCodeForCleanupUsings. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForCleanupUsings() => """
using System;
using System.Text;

/// <summary> /// Represents the  CleanupSample  class. /// </summary>
public class CleanupSample
{
    public void Say() => Console.WriteLine("Hi");
}
""";

    /// <summary>
    /// Provides sample code used by tests that move a type into a dedicated file.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForMoveTypeToFile() =>
        GetExampleCode();

    /// <summary> ///  GetSampleCodeForRenameSymbol. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForRenameSymbol() =>
        GetExampleCode();

    /// <summary>
    /// Provides sample code used by tests that extract an interface from a class.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForExtractInterface() => """
public class Person
{
    public string Name { get; set; }
    public void Greet() { }
}
""";

    /// <summary>
    /// Provides sample code for feature flag usage scenarios.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForFeatureFlag() => """
using System;

public class FeatureService
{
    private readonly IFeatureFlags featureFlags;

    public FeatureService(IFeatureFlags featureFlags)
    {
        this.featureFlags = featureFlags;
    }

    public void DoWork()
    {
        if (featureFlags.IsEnabled("CoolFeature"))
        {
            Console.WriteLine("New path");
        }
        else
        {
            Console.WriteLine("Old path");
        }
    }
}

/// <summary> /// Defines the  IFeatureFlags  interface. /// </summary>
public interface IFeatureFlags
{
    bool IsEnabled(string name);
}
""";

    /// <summary> ///  GetSampleCodeForDecorator. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForDecorator() => """
public class Greeter
{
    public void Greet(string name)
    {
        Console.WriteLine("Hello {name}");
    }
}
""";

    /// <summary> ///  GetSampleCodeForAdapter. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForAdapter() => """
public class LegacyLogger
{
    public void Write(string message)
    {
        Console.WriteLine(message);
    }
}
""";

    /// <summary> ///  GetSampleCodeForObserver. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForObserver() => """
public class Counter
{
    private int _value;
    public void Update(int value)
    {
        _value = value;
    }
}
""";

    /// <summary>
    /// Provides sample code that encourages using interfaces instead of concrete types.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForUseInterface() => """
public interface IWriter { void Write(string value); }
public class FileWriter : IWriter { public void Write(string value) { } }
public class C
{
    public void DoWork(FileWriter writer)
    {
        writer.Write("hi");
    }
}
""";
}
