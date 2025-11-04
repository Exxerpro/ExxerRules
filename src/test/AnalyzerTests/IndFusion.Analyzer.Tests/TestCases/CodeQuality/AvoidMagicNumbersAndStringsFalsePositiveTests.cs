using System;
using System.Globalization;
using System.Text.RegularExpressions;
using IndFusion.Analyzer.CodeQuality;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.CodeQuality;

/// <summary>
/// Tests for AvoidMagicNumbersAndStringsAnalyzer false-positive mitigation scenarios.
/// </summary>
public class AvoidMagicNumbersAndStringsFalsePositiveTests
{
    //  Story 1.1: Exempt Enum Member Values

    /// <summary>
    /// Tests that numeric literals used to define enum member values are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Enum_Member_Values()
    {
        const string testCode = @"
namespace TestProject
{
    public enum Status
    {
        Pending = 0,
        Active = 1,
        Completed = 2,
        Cancelled = 99
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.2: Exempt Bit-Flag Enum Values

    /// <summary>
    /// Tests that bit-shift operations used to define flag enum members are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Bit_Flag_Enum_Values()
    {
        const string testCode = @"
using System;

namespace TestProject
{
    [Flags]
    public enum Permissions
    {
        None = 0,
        Read = 1 << 0,
        Write = 1 << 1,
        Execute = 1 << 2,
        Admin = 1 << 3
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.3: Exempt Domain Range Guards

    /// <summary>
    /// Tests that numeric literals in domain range guard clauses are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Domain_Range_Guards()
    {
        const string testCode = @"
using System;

namespace TestProject
{
    public class User
    {
        public int Age { get; set; }
        
        public void SetAge(int age)
        {
            if (age < 0)
                throw new ArgumentOutOfRangeException(nameof(age), ""Age cannot be negative"");
            
            if (age > 150)
                throw new ArgumentOutOfRangeException(nameof(age), ""Age cannot exceed 150"");
                
            Age = age;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.4: Exempt Business Rule Thresholds

    /// <summary>
    /// Tests that numeric literals for business rule thresholds are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Business_Rule_Thresholds()
    {
        const string testCode = @"
namespace TestProject
{
    public class UserValidator
    {
        public bool ValidateName(string name)
        {
            if (name.Length < 2)
                return false;
                
            if (name.Length > 50)
                return false;
                
            return true;
        }
        
        public bool ValidateEmail(string email)
        {
            if (email.Length > 254)
                return false;
                
            return true;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.5: Exempt Exception Messages

    /// <summary>
    /// Tests that string literals used as exception messages are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Exception_Messages()
    {
        const string testCode = @"
using System;

namespace TestProject
{
    public class Service
    {
        public void ProcessData(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException(""Data cannot be null or empty"");
                
            if (data.Length > 1000)
                throw new InvalidOperationException(""Data is too large to process"");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.6: Exempt Result/Validation Messages

    /// <summary>
    /// Tests that string literals for validation messages in collections or Result objects are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Result_Validation_Messages()
    {
        const string testCode = @"
using System.Collections.Generic;

namespace TestProject
{
    public class Validator
    {
        public List<string> ValidateUser(string name, string email)
        {
            var errors = new List<string>();
            
            if (string.IsNullOrEmpty(name))
                errors.Add(""Name is required"");
                
            if (string.IsNullOrEmpty(email))
                errors.Add(""Email is required"");
                
            return errors;
        }
        
        public Result ValidateData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return Result.WithFailure(""Data is required"");
                
            return Result.Success();
        }
    }
    
    public class Result
    {
        public static Result Success() => new Result();
        public static Result WithFailure(string message) => new Result();
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.7: Exempt Regex and Pattern Literals

    /// <summary>
    /// Tests that string literals that are regular expressions or pattern-based strings are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Regex_And_Pattern_Literals()
    {
        const string testCode = @"
using System.Text.RegularExpressions;

namespace TestProject
{
    public class PatternMatcher
    {
        public bool IsValidEmail(string email)
        {
            var pattern = @""^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"";
            return Regex.IsMatch(email, pattern);
        }
        
        public bool IsValidPhone(string phone)
        {
            var regex = new Regex(@""^\d{3}-\d{3}-\d{4}$"");
            return regex.IsMatch(phone);
        }
        
        public string CleanInput(string input)
        {
            return Regex.Replace(input, @""[^a-zA-Z0-9]"", """");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.8: Exempt Culture and Locale Codes

    /// <summary>
    /// Tests that string literals for culture and locale codes are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Culture_And_Locale_Codes()
    {
        const string testCode = @"
using System.Globalization;

namespace TestProject
{
    public class LocalizationService
    {
        public string FormatCurrency(decimal amount)
        {
            var culture = new CultureInfo(""en-US"");
            return amount.ToString(""C"", culture);
        }
        
        public string FormatDate(DateTime date)
        {
            var culture = new CultureInfo(""fr-FR"");
            return date.ToString(""d"", culture);
        }
        
        public void SetCulture(string cultureCode)
        {
            var culture = new CultureInfo(cultureCode);
            CultureInfo.CurrentCulture = culture;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.9: Exempt TimeSpan and DateTime Construction

    /// <summary>
    /// Tests that numeric literals when constructing TimeSpan or DateTime objects are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_TimeSpan_And_DateTime_Construction()
    {
        const string testCode = @"
using System;

namespace TestProject
{
    public class TimeService
    {
        public TimeSpan GetTimeout()
        {
            return TimeSpan.FromSeconds(30);
        }
        
        public TimeSpan GetDelay()
        {
            return new TimeSpan(0, 0, 5);
        }
        
        public DateTime GetEpoch()
        {
            return new DateTime(1970, 1, 1);
        }
        
        public DateTime GetFutureDate()
        {
            return DateTime.Now.AddDays(7);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.10: Exempt Logging Message Templates

    /// <summary>
    /// Tests that string literals for structured logging message templates are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Logging_Message_Templates()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
    public class Service
    {
        private readonly ILogger<Service> _logger;
        
        public Service(ILogger<Service> logger)
        {
            _logger = logger;
        }
        
        public void ProcessData(string data)
        {
            _logger.LogInformation(""Processing data: {Data}"", data);
            _logger.LogWarning(""Data validation failed for user {UserId}"", 123);
            _logger.LogError(""Failed to process request after {RetryCount} attempts"", 3);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Positive Control Tests

    /// <summary>
    /// Tests that regular magic numbers are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Regular_Magic_Numbers()
    {
        const string testCode = @"
namespace TestProject
{
    public class Calculator
    {
        public int Calculate(int value)
        {
            return value * 42; // Should be flagged
        }
        
        public double GetRate()
        {
            return 3.14159; // Should be flagged
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.AvoidMagicNumbersAndStrings);
    }

    /// <summary>
    /// Tests that regular magic strings are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Regular_Magic_Strings()
    {
        const string testCode = @"
namespace TestProject
{
    public class ConfigService
    {
        public string GetConnectionString()
        {
            return ""Server=localhost;Database=MyDB;""; // Should be flagged
        }
        
        public string GetApiKey()
        {
            return ""abc123def456""; // Should be flagged
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidMagicNumbersAndStringsAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.AvoidMagicNumbersAndStrings);
    }

     // 
}
