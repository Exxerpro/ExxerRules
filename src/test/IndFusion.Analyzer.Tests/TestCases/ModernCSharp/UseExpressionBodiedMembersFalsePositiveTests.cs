using System;
using IndFusion.Analyzers;
using IndFusion.Analyzers.ModernCSharp;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.ModernCSharp;

/// <summary>
/// Tests for UseExpressionBodiedMembersAnalyzer false-positive mitigation scenarios.
/// </summary>
public class UseExpressionBodiedMembersFalsePositiveTests
{
    #region Story 1.1: Exempt ICommandData Factory Methods

    /// <summary>
    /// Tests that ICommandData.Create factory methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_ICommandData_Factory_Methods()
    {
        const string testCode = @"
namespace TestProject
{
    public interface ICommandData
    {
        ICommandData Create();
    }

    public class CreateUserCommand : ICommandData
    {
        public string Name { get; set; } = string.Empty;
        
        public ICommandData Create()
        {
            return new CreateUserCommand { Name = ""Default"" };
        }
    }

    public class UpdateUserCommand : ICommandData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public ICommandData Create()
        {
            return new UpdateUserCommand { Id = 0, Name = ""Default"" };
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.2: Exempt Fluent TODO Stubs

    /// <summary>
    /// Tests that methods with TODO or FIXME comments are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Fluent_TODO_Stubs()
    {
        const string testCode = @"
namespace TestProject
{
    public class Service
    {
        public string GetData()
        {
            // TODO: Implement actual data retrieval
            return ""placeholder"";
        }
        
        public int CalculateValue()
        {
            // FIXME: Add proper calculation logic
            return 0;
        }
        
        public bool IsValid()
        {
            // TODO: Add validation logic
            return true;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.3: Exempt IResettable.TryReset Methods

    /// <summary>
    /// Tests that IResettable.TryReset methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_IResettable_TryReset_Methods()
    {
        const string testCode = @"
using System;

namespace TestProject
{
    public interface IResettable
    {
        bool TryReset();
    }

    public class ResettableService : IResettable
    {
        public bool TryReset()
        {
            return true;
        }
    }

    public class AnotherResettable : IResettable
    {
        public bool TryReset()
        {
            return false;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.4: Exempt Fluent WithData Methods

    /// <summary>
    /// Tests that fluent With... methods that return this are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Fluent_WithData_Methods()
    {
        const string testCode = @"
namespace TestProject
{
    public class FluentBuilder
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        
        public FluentBuilder WithName(string name)
        {
            // Future: Add validation logic
            Name = name;
            return this;
        }
        
        public FluentBuilder SetValue(int value)
        {
            // Future: Add range checking
            Value = value;
            return this;
        }
        
        public FluentBuilder WithData(string data)
        {
            // Future: Process data
            return this;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Story 1.5: Exempt Domain Entity Resetters

    /// <summary>
    /// Tests that domain entity reset methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Domain_Entity_Resetters()
    {
        const string testCode = @"
namespace TestProject
{
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Resets the user to default state.
        /// </summary>
        public bool Reset()
        {
            return true;
        }
        
        public bool ResetToDefaults()
        {
            return false;
        }
        
        /// <summary>
        /// Clears all user data and resets to initial state.
        /// </summary>
        public bool ClearAndReset()
        {
            return true;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    #endregion

    #region Positive Control Tests

    /// <summary>
    /// Tests that regular simple methods are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Regular_Simple_Methods()
    {
        const string testCode = @"
namespace TestProject
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
        
        public string GetGreeting()
        {
            return ""Hello"";
        }
        
        public bool IsEven(int number)
        {
            return number % 2 == 0;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.UseExpressionBodiedMembers);
    }

    /// <summary>
    /// Tests that simple property getters are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Simple_Property_Getters()
    {
        const string testCode = @"
namespace TestProject
{
    public class Person
    {
        private string _name = ""John"";
        
        public string Name
        {
            get
            {
                return _name;
            }
        }
        
        public int Age
        {
            get
            {
                return 25;
            }
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseExpressionBodiedMembersAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.UseExpressionBodiedMembers);
    }

    #endregion
}
