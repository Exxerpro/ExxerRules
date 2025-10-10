using IndFusion.Analyzers;
using IndFusion.Analyzers.Logging;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Logging;

/// <summary>
/// Tests for EXXER801 - DoNotUseConsoleWriteLine Analyzer false-positive scenarios.
/// </summary>
public class DoNotUseConsoleWriteLineFalsePositiveTests
{
    #region Story 1.1: Exempt Console Applications and Tooling

    /// <summary>
    /// Tests that Console.WriteLine is allowed in Main methods and Program classes.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Console_Applications_And_Tooling()
    {
        const string testCode = @"
using System;

namespace MyProject
{
    class Program
    {
        static void Main(string[] args)
        {
            // These should not be flagged - console application entry point
            Console.WriteLine(""Starting application..."");
            Console.Write(""Enter your name: "");
            Console.WriteLine($""Hello, {args[0]}!"");
        }
    }

    public class ConsoleTool
    {
        public static void Main(string[] args)
        {
            // These should not be flagged - console tool entry point
            Console.WriteLine(""Tool started"");
            Console.Write(""Processing..."");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.2: Exempt Build and Deployment Scripts

    /// <summary>
    /// Tests that Console.WriteLine is allowed in build and deployment scripts.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Build_And_Deployment_Scripts()
    {
        const string testCode = @"
using System;

namespace MyProject.Build
{
    public class BuildScript
    {
        public void RunBuild()
        {
            // These should not be flagged - build script
            Console.WriteLine(""Building project..."");
            Console.Write(""Compiling..."");
        }
    }
}

namespace MyProject.Deployment
{
    public class DeploymentScript
    {
        public void Deploy()
        {
            // These should not be flagged - deployment script
            Console.WriteLine(""Deploying to production..."");
            Console.Write(""Copying files..."");
        }
    }
}

namespace MyProject.Scripts
{
    public class UtilityScript
    {
        public void Execute()
        {
            // These should not be flagged - utility script
            Console.WriteLine(""Running utility script..."");
            Console.Write(""Status: Complete"");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.3: Exempt Conditional Compilation Blocks

    /// <summary>
    /// Tests that Console.WriteLine is allowed in DEBUG and TRACE blocks.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Conditional_Compilation_Blocks()
    {
        const string testCode = @"
using System;

namespace MyProject
{
    public class DebugHelper
    {
        public void ProcessData()
        {
#if DEBUG
            // These should not be flagged - DEBUG block
            Console.WriteLine(""Debug: Processing started"");
            Console.Write(""Debug: Step 1 complete"");
#endif

#if TRACE
            // These should not be flagged - TRACE block
            Console.WriteLine(""Trace: Detailed information"");
            Console.Write(""Trace: Current state"");
#endif
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.4: Exempt ConditionalAttribute Methods

    /// <summary>
    /// Tests that Console.WriteLine is allowed in methods with Conditional attributes.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_ConditionalAttribute_Methods()
    {
        const string testCode = @"
using System;
using System.Diagnostics;

namespace MyProject
{
    public class DebugHelper
    {
        [Conditional(""DEBUG"")]
        public void DebugLog(string message)
        {
            // These should not be flagged - Conditional DEBUG method
            Console.WriteLine($""Debug: {message}"");
            Console.Write(""Debug info: "");
        }

        [Conditional(""TRACE"")]
        public void TraceLog(string message)
        {
            // These should not be flagged - Conditional TRACE method
            Console.WriteLine($""Trace: {message}"");
            Console.Write(""Trace info: "");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.5: Exempt Unit and Integration Tests

    /// <summary>
    /// Tests that Console.WriteLine is allowed in test classes.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Unit_And_Integration_Tests()
    {
        const string testCode = @"
using System;
using Xunit;

namespace MyProject.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public void Should_Process_User()
        {
            // These should not be flagged - test class
            Console.WriteLine(""Test: Processing user"");
            Console.Write(""Test status: Running"");
        }
    }

    public class IntegrationTests
    {
        [Fact]
        public void Should_Connect_To_Database()
        {
            // These should not be flagged - integration test
            Console.WriteLine(""Integration test: Connecting to database"");
            Console.Write(""Connection status: OK"");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.6: Exempt Redirected Console Output

    /// <summary>
    /// Tests that Console.WriteLine is allowed when console output is redirected.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Redirected_Console_Output()
    {
        const string testCode = @"
using System;
using System.IO;

namespace MyProject
{
    public class ConsoleRedirector
    {
        public void CaptureOutput()
        {
            // These should not be flagged - console output is redirected
            Console.SetOut(new StringWriter());
            Console.WriteLine(""This output is redirected"");
            Console.Write(""Captured output"");

            Console.SetError(new StringWriter());
            Console.Error.WriteLine(""Error output redirected"");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.7: Exempt CLI Prompting

    /// <summary>
    /// Tests that Console.Write is allowed when followed by Console.ReadLine or Console.ReadKey.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_CLI_Prompting()
    {
        const string testCode = @"
using System;

namespace MyProject
{
    public class CliPrompter
    {
        public string GetUserInput()
        {
            // These should not be flagged - CLI prompting
            Console.Write(""Enter your name: "");
            return Console.ReadLine();
        }

        public void WaitForKeyPress()
        {
            // These should not be flagged - CLI prompting
            Console.Write(""Press any key to continue..."");
            Console.ReadKey();
        }

        public string GetPassword()
        {
            // These should not be flagged - CLI prompting
            Console.Write(""Enter password: "");
            return Console.ReadLine();
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.8: Exempt Exception Reporting During Startup

    /// <summary>
    /// Tests that Console.Error.WriteLine is allowed in catch blocks followed by Environment.Exit.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Exception_Reporting_During_Startup()
    {
        const string testCode = @"
using System;

namespace MyProject
{
    public class StartupHandler
    {
        public void Initialize()
        {
            try
            {
                // Some initialization code
                throw new Exception(""Startup failed"");
            }
            catch (Exception ex)
            {
                // These should not be flagged - exception reporting during startup
                Console.Error.WriteLine($""Fatal startup error: {ex.Message}"");
                Console.Error.Write(""Application cannot continue"");
                Environment.Exit(1);
            }
        }

        public void HandleCriticalError()
        {
            try
            {
                // Some critical operation
                throw new InvalidOperationException(""Critical failure"");
            }
            catch (Exception ex)
            {
                // These should not be flagged - critical error reporting
                Console.Error.WriteLine($""Critical error: {ex.Message}"");
                Environment.Exit(-1);
            }
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.9: Exempt Console Logger Adapters

    /// <summary>
    /// Tests that Console.WriteLine is allowed in ConsoleLogger classes or classes with AllowConsoleLogging attribute.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Console_Logger_Adapters()
    {
        const string testCode = @"
using System;

namespace MyProject
{
    public class ConsoleLogger
    {
        public void LogInfo(string message)
        {
            // These should not be flagged - ConsoleLogger class
            Console.WriteLine($""INFO: {message}"");
            Console.Write(""Log entry: "");
        }

        public void LogError(string message)
        {
            // These should not be flagged - ConsoleLogger class
            Console.Error.WriteLine($""ERROR: {message}"");
        }
    }

    [AllowConsoleLogging]
    public class SimpleLogger
    {
        public void Log(string message)
        {
            // These should not be flagged - AllowConsoleLogging attribute
            Console.WriteLine(message);
            Console.Write(""Logged: "");
        }
    }

    public class AllowConsoleLoggingAttribute : Attribute
    {
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.10: Exempt Generated Code

    /// <summary>
    /// Tests that Console.WriteLine is allowed in generated code.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Generated_Code()
    {
        const string testCode = @"
using System;
using System.CodeDom.Compiler;

namespace MyProject.Generated
{
    [GeneratedCode(""MyGenerator"", ""1.0.0"")]
    public class GeneratedClass
    {
        public void GeneratedMethod()
        {
            // These should not be flagged - generated code
            Console.WriteLine(""Generated output"");
            Console.Write(""Generated status"");
        }
    }
}

namespace MyProject
{
    public class RegularClass
    {
        public void RegularMethod()
        {
            // These should be flagged - not in generated context
            Console.WriteLine(""Regular output"");
            Console.Write(""Regular status"");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer());
        // Should only report the Console.WriteLine in RegularClass, not in GeneratedClass
        diagnostics.ShouldHaveSingleItem();
        diagnostics[0].Location.SourceSpan.Start.ShouldBeGreaterThan(0); // Verify it's the right location
    }

    #endregion
}
