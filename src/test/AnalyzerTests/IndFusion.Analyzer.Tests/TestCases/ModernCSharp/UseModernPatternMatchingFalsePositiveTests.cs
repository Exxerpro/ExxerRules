using System;
using IndFusion.Analyzer.ModernCSharp;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.ModernCSharp;

/// <summary>
/// Tests for UseModernPatternMatchingAnalyzer false-positive mitigation scenarios.
/// </summary>
public class UseModernPatternMatchingFalsePositiveTests
{
    //  Story 1.1: Exempt Conditional Operator Guards

    /// <summary>
    /// Tests that conditional operator guards are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Conditional_Operator_Guards()
    {
        const string testCode = @"
using System;

namespace MyProject.Utilities
{
    public class ReflectionHelper
    {
        public string GetStringValue(object? value)
        {
            // Conditional operator with is check and cast - should not be flagged
            return value is string ? (string)value : string.Empty;
        }

        public int GetIntValue(object? value)
        {
            // Conditional operator with is not check and cast - should not be flagged
            return value is not int ? 0 : (int)value;
        }

        public bool IsString(object? value)
        {
            // Conditional operator in expression-bodied member - should not be flagged
            return value is string ? ((string)value).Length > 0 : false;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseModernPatternMatchingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.2: Exempt Reflection Property Access

    /// <summary>
    /// Tests that reflection property access patterns are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Reflection_Property_Access()
    {
        const string testCode = @"
using System;
using System.Reflection;

namespace MyProject.Reflection
{
    public class PropertyHelper
    {
        public T? GetPropertyValue<T>(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null) return default;

            var value = property.GetValue(obj);
            
            // Reflection helper with ternary expression - should not be flagged
            return value is T ? (T)value : default;
        }

        public string GetStringProperty(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null) return string.Empty;

            var value = property.GetValue(obj);
            
            // Reflection helper with cast - should not be flagged
            return value is string ? ((string)value).ToUpper() : string.Empty;
        }

        public int GetIntProperty(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null) return 0;

            var value = property.GetValue(obj);
            
            // Reflection helper with value type cast - should not be flagged
            return value is int ? (int)value : 0;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseModernPatternMatchingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.3: Exempt Type-Switched Casts

    /// <summary>
    /// Tests that type-switched casts are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Type_Switched_Casts()
    {
        const string testCode = @"
using System;

namespace MyProject.TypeHandling
{
    public class TypeConverter
    {
        public object ConvertValue(object value, Type targetType)
        {
            // Type switch with casts - should not be flagged
            return targetType switch
            {
                Type t when t == typeof(string) => (string)value,
                Type t when t == typeof(int) => (int)value,
                Type t when t == typeof(bool) => (bool)value,
                Type t when t == typeof(DateTime) => (DateTime)value,
                _ => value
            };
        }

        public T ConvertToType<T>(object value)
        {
            var targetType = typeof(T);
            
            // Type switch expression with cast - should not be flagged
            return targetType switch
            {
                Type t when t == typeof(string) => (T)(object)(string)value,
                Type t when t == typeof(int) => (T)(object)(int)value,
                Type t when t == typeof(bool) => (T)(object)(bool)value,
                _ => (T)value
            };
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseModernPatternMatchingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.4: Exempt Local Function Closures

    /// <summary>
    /// Tests that local function closures with guarded casts are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Local_Function_Closures()
    {
        const string testCode = @"
using System;

namespace MyProject.LocalFunctions
{
    public class LocalFunctionExample
    {
        public void ProcessValue(object? value)
        {
            if (value is string)
            {
                // Local function with cast - should not be flagged
                string ProcessString()
                {
                    return ((string)value).ToUpper();
                }

                var result = ProcessString();
            }

            if (value is int)
            {
                // Lambda with cast - should not be flagged
                Func<int> getInt = () => (int)value;
                var intValue = getInt();
            }
        }

        public void ProcessWithNestedFunction(object? value)
        {
            if (value is DateTime)
            {
                // Nested local function with cast - should not be flagged
                void ProcessDateTime()
                {
                    var dt = (DateTime)value;
                    Console.WriteLine(dt.ToString());
                }

                ProcessDateTime();
            }
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseModernPatternMatchingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.5: Support is not null Guard Clauses

    /// <summary>
    /// Tests that is not null guard clauses are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Is_Not_Null_Guard_Clauses()
    {
        const string testCode = @"
using System;

namespace MyProject.NullGuards
{
    public class NullGuardExample
    {
        public void ProcessValue(object? value)
        {
            // is not null guard with cast - should not be flagged
            if (value is not null)
            {
                var stringValue = (string)value;
                Console.WriteLine(stringValue);
            }

            // is not null guard with specific type - should not be flagged
            if (value is not null && value is string)
            {
                var str = (string)value;
                Console.WriteLine(str.Length);
            }
        }

        public string GetStringValue(object? value)
        {
            // is not null guard in return - should not be flagged
            if (value is not null)
            {
                return (string)value;
            }
            return string.Empty;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseModernPatternMatchingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.6: Exempt Nullable Unwrap Patterns

    /// <summary>
    /// Tests that nullable unwrap patterns are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Nullable_Unwrap_Patterns()
    {
        const string testCode = @"
using System;

namespace MyProject.NullableHandling
{
    public class NullableUnwrapExample
    {
        public void ProcessValue(object? value)
        {
            // Value type unwrap with cast - should not be flagged
            if (value is int)
            {
                var intValue = (int)value;
                Console.WriteLine(intValue);
            }

            // Value type unwrap with cast - should not be flagged
            if (value is bool)
            {
                var boolValue = (bool)value;
                Console.WriteLine(boolValue);
            }

            // Value type unwrap with cast - should not be flagged
            if (value is DateTime)
            {
                var dateValue = (DateTime)value;
                Console.WriteLine(dateValue.ToString());
            }
        }

        public int GetIntValue(object? value)
        {
            // Value type unwrap in return - should not be flagged
            if (value is int)
            {
                return (int)value;
            }
            return 0;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseModernPatternMatchingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.7: Exempt Type Equality Guards

    /// <summary>
    /// Tests that type equality guards are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Type_Equality_Guards()
    {
        const string testCode = @"
using System;

namespace MyProject.TypeEquality
{
    public class TypeEqualityExample
    {
        public void ProcessValue(object? value)
        {
            var valueType = value?.GetType();
            
            // Type equality guard with cast - should not be flagged
            if (valueType == typeof(string))
            {
                var stringValue = (string)value;
                Console.WriteLine(stringValue);
            }

            // Type equality guard with cast - should not be flagged
            if (valueType == typeof(int))
            {
                var intValue = (int)value;
                Console.WriteLine(intValue);
            }

            // Type equality guard with cast - should not be flagged
            if (valueType == typeof(DateTime))
            {
                var dateValue = (DateTime)value;
                Console.WriteLine(dateValue.ToString());
            }
        }

        public T? ConvertValue<T>(object? value)
        {
            var valueType = value?.GetType();
            var targetType = typeof(T);
            
            // Type equality guard with cast - should not be flagged
            if (valueType == targetType)
            {
                return (T)value;
            }
            return default;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseModernPatternMatchingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.8: Exempt Tuple Pattern Extraction

    /// <summary>
    /// Tests that tuple pattern extraction is not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Tuple_Pattern_Extraction()
    {
        const string testCode = @"
using System;

namespace MyProject.TupleHandling
{
    public class TupleExample
    {
        public void ProcessTuple(object? value)
        {
            if (value is (string, int))
            {
                // Tuple deconstruction with cast - should not be flagged
                var (str, num) = ((string, int))value;
                Console.WriteLine($""{str}: {num}"");
            }

            if (value is (string, int, bool))
            {
                // Tuple deconstruction with cast - should not be flagged
                var (str, num, flag) = ((string, int, bool))value;
                Console.WriteLine($""{str}: {num}: {flag}"");
            }
        }

        public (string, int) GetTupleValue(object? value)
        {
            if (value is (string, int))
            {
                // Tuple cast in return - should not be flagged
                return ((string, int))value;
            }
            return (string.Empty, 0);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseModernPatternMatchingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.9: Exempt Pattern-Matched Exception Handling

    /// <summary>
    /// Tests that pattern-matched exception handling is not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Pattern_Matched_Exception_Handling()
    {
        const string testCode = @"
using System;

namespace MyProject.ExceptionHandling
{
    public class ExceptionHandler
    {
        public void HandleException(Exception ex)
        {
            try
            {
                throw ex;
            }
            catch (Exception e) when (e is ArgumentException)
            {
                // Exception cast with when clause - should not be flagged
                var argEx = (ArgumentException)e;
                Console.WriteLine(argEx.ParamName);
            }
            catch (Exception e) when (e is InvalidOperationException)
            {
                // Exception cast with when clause - should not be flagged
                var invalidOpEx = (InvalidOperationException)e;
                Console.WriteLine(invalidOpEx.Message);
            }
            catch (Exception e) when (e is NotSupportedException)
            {
                // Exception cast with when clause - should not be flagged
                var notSupportedEx = (NotSupportedException)e;
                Console.WriteLine(notSupportedEx.Message);
            }
        }

        public string GetExceptionMessage(Exception ex)
        {
            try
            {
                throw ex;
            }
            catch (Exception e) when (e is ArgumentException)
            {
                // Exception cast in return - should not be flagged
                return ((ArgumentException)e).ParamName ?? string.Empty;
            }
            catch (Exception e) when (e is InvalidOperationException)
            {
                // Exception cast in return - should not be flagged
                return ((InvalidOperationException)e).Message;
            }
            return string.Empty;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseModernPatternMatchingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.10: Exempt Temporary Variable Reassignment

    /// <summary>
    /// Tests that temporary variable reassignment is not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Temporary_Variable_Reassignment()
    {
        const string testCode = @"
using System;

namespace MyProject.VariableReassignment
{
    public class VariableReassignmentExample
    {
        public void ProcessValue(object? value)
        {
            if (value is string)
            {
                // Variable reassignment with cast - should not be flagged
                value = (string)value;
                Console.WriteLine(value);
            }

            if (value is int)
            {
                // Variable reassignment with cast - should not be flagged
                value = (int)value;
                Console.WriteLine(value);
            }
        }

        public object TransformValue(object? value)
        {
            if (value is string)
            {
                // Variable reassignment with cast - should not be flagged
                value = ((string)value).ToUpper();
                return value;
            }

            if (value is int)
            {
                // Variable reassignment with cast - should not be flagged
                value = (int)value * 2;
                return value;
            }

            return value;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseModernPatternMatchingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Positive Control Tests

    /// <summary>
    /// Tests that actual inefficient pattern matching is still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Actual_Inefficient_Pattern_Matching()
    {
        const string testCode = @"
using System;

namespace MyProject.InefficientPatterns
{
    public class InefficientExample
    {
        public void ProcessValue(object? value)
        {
            // This should be flagged - inefficient is check followed by cast
            if (value is string)
            {
                var stringValue = (string)value;
                Console.WriteLine(stringValue);
            }

            // This should be flagged - inefficient is check followed by cast
            if (value is int)
            {
                var intValue = (int)value;
                Console.WriteLine(intValue);
            }
        }

        public string GetStringValue(object? value)
        {
            // This should be flagged - inefficient is check followed by cast
            if (value is string)
            {
                return (string)value;
            }
            return string.Empty;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseModernPatternMatchingAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.UseModernPatternMatching);
    }

     // 
}
