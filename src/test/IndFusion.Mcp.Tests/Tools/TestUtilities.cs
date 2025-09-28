namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Helper functions for test inputs: locating the solution, creating files,
/// and providing canned sample source code for tool tests.
/// </summary>
public static class TestUtilities
{
    /// <summary>
    /// Finds the solution path by walking up from the current directory.
    /// </summary>
    public static string GetSolutionPath()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var dir = new DirectoryInfo(currentDir);
        while (dir != null)
        {
            var solutionFile = Path.Combine(dir.FullName, "IndFusion.Mcp.sln");
            if (File.Exists(solutionFile))
                return solutionFile;
            dir = dir.Parent;
        }
        return "./IndFusion.Mcp.sln";
    }

    /// <summary>
    /// Creates a file at the given path with the specified content.
    /// </summary>
    public static async Task CreateTestFile(string filePath, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        await File.WriteAllTextAsync(filePath, content, cancellationToken: Xunit.TestContext.Current.CancellationToken);
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
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(GetSolutionPath())!, "IndFusion.Mcp.Tests", "ExampleCode.cs"));

    /// <summary> ///  GetSampleCodeForIntroduceVariable. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForIntroduceVariable() =>
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(GetSolutionPath())!, "IndFusion.Mcp.Tests", "ExampleCode.cs"));

    /// <summary> ///  GetSampleCodeForMakeFieldReadonly. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForMakeFieldReadonly() =>
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(GetSolutionPath())!, "IndFusion.Mcp.Tests", "ExampleCode.cs"));

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
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(GetSolutionPath())!, "IndFusion.Mcp.Tests", "ExampleCode.cs"));

    /// <summary> ///  GetSampleCodeForConvertToStaticInstance. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForConvertToStaticInstance() =>
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(GetSolutionPath())!, "IndFusion.Mcp.Tests", "ExampleCode.cs"));

    /// <summary>
    /// Provides sample code used by tests that move a static method between types.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForMoveStaticMethod() =>
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(GetSolutionPath())!, "IndFusion.Mcp.Tests", "ExampleCode.cs"));

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
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(GetSolutionPath())!, "IndFusion.Mcp.Tests", "ExampleCode.cs"));

    /// <summary> ///  GetSampleCodeForConvertToExtension. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForConvertToExtension() =>
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(GetSolutionPath())!, "IndFusion.Mcp.Tests", "ExampleCode.cs"));

    /// <summary>
    /// Provides sample code used by tests that verify safe deletion of code elements.
    /// </summary>
    /// <returns>Sample C# code as a string.</returns>
    public static string GetSampleCodeForSafeDelete() =>
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(GetSolutionPath())!, "IndFusion.Mcp.Tests", "ExampleCode.cs"));

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
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(GetSolutionPath())!, "IndFusion.Mcp.Tests", "ExampleCode.cs"));

    /// <summary> ///  GetSampleCodeForRenameSymbol. /// </summary> /// <returns>A value of type string.</returns>
    public static string GetSampleCodeForRenameSymbol() =>
        File.ReadAllText(Path.Combine(Path.GetDirectoryName(GetSolutionPath())!, "IndFusion.Mcp.Tests", "ExampleCode.cs"));

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
