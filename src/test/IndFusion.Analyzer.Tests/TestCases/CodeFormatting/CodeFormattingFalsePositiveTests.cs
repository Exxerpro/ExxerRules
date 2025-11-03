using IndFusion.Analyzers;
using IndFusion.Analyzers.CodeFormatting;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.CodeFormatting;

/// <summary>
/// Tests for EXXER901 - CodeFormatting Analyzer false-positive scenarios.
/// </summary>
public class CodeFormattingFalsePositiveTests
{
    //  Story 1.1: Correctly Handle LINQ Projections

    /// <summary>
    /// Tests that LINQ projection assignments are handled correctly.
    /// </summary>
    [Fact]
    public void Should_Handle_LINQ_Projections_Correctly()
    {
        const string testCode = @"
using System.Linq;

namespace MyProject
{
    public class LinqProjectionExample
    {
        public void ProcessData()
        {
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var result = numbers.Select(x => x * 2).ToList();
            var filtered = numbers.Where(x => x > 2).Select(x => x.ToString()).ToArray();
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new CodeFormattingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.2: Correctly Handle Guard Clause Mock Data Assignments

    /// <summary>
    /// Tests that assignments within debug guard clauses are handled correctly.
    /// </summary>
    [Fact]
    public void Should_Handle_Guard_Clause_Mock_Data_Assignments_Correctly()
    {
        const string testCode = @"
using System;

namespace MyProject
{
    public class GuardClauseExample
    {
        public void ProcessData()
        {
#if DEBUG
            var mockData = new { Id = 1, Name = ""Test"" };
            var debugInfo = ""Debug information"";
#endif
            var realData = GetRealData();
        }

        private object GetRealData() => new object();
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new CodeFormattingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.3: Correctly Handle Dictionary Initializations

    /// <summary>
    /// Tests that dictionary initializations are handled correctly.
    /// </summary>
    [Fact]
    public void Should_Handle_Dictionary_Initializations_Correctly()
    {
        const string testCode = @"
using System.Collections.Generic;

namespace MyProject
{
    public class DictionaryExample
    {
        public void CreateDictionaries()
        {
            var simpleDict = new Dictionary<string, int>
            {
                { ""key1"", 1 },
                { ""key2"", 2 }
            };

            var complexDict = new Dictionary<string, object>
            {
                { ""user"", new { Id = 1, Name = ""John"" } },
                { ""settings"", new { Theme = ""dark"", Language = ""en"" } }
            };
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new CodeFormattingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.4: Correctly Handle Awaited Repository Calls

    /// <summary>
    /// Tests that awaited repository calls are handled correctly.
    /// </summary>
    [Fact]
    public void Should_Handle_Awaited_Repository_Calls_Correctly()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace MyProject
{
    public class RepositoryExample
    {
        private readonly IUserRepository _userRepository;

        public RepositoryExample(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ProcessUser()
        {
            var user = await _userRepository.GetByIdAsync(1);
            var users = await _userRepository.GetAllAsync();
            var result = await _userRepository.SaveAsync(user);
        }
    }

    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User[]> GetAllAsync();
        Task<bool> SaveAsync(User user);
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new CodeFormattingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.5: Correctly Handle Projections to DTOs

    /// <summary>
    /// Tests that LINQ projections to DTOs are handled correctly.
    /// </summary>
    [Fact]
    public void Should_Handle_Projections_To_DTOs_Correctly()
    {
        const string testCode = @"
using System.Linq;

namespace MyProject
{
    public class DtoProjectionExample
    {
        public void ProjectToDtos()
        {
            var users = new[] { new User { Id = 1, Name = ""John"" }, new User { Id = 2, Name = ""Jane"" } };
            var userDtos = users.Select(u => new UserDto { Id = u.Id, Name = u.Name }).ToList();
            var summaryDtos = users.Select(u => new UserSummaryDto { Id = u.Id, DisplayName = u.Name.ToUpper() }).ToArray();
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new CodeFormattingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.6: Correctly Handle GroupBy/ToDictionary Pipelines

    /// <summary>
    /// Tests that GroupBy/ToDictionary pipelines are handled correctly.
    /// </summary>
    [Fact]
    public void Should_Handle_GroupBy_ToDictionary_Pipelines_Correctly()
    {
        const string testCode = @"
using System.Linq;

namespace MyProject
{
    public class GroupByExample
    {
        public void GroupData()
        {
            var orders = new[] 
            { 
                new Order { CustomerId = 1, Amount = 100 },
                new Order { CustomerId = 1, Amount = 200 },
                new Order { CustomerId = 2, Amount = 150 }
            };

            var customerTotals = orders
                .GroupBy(o => o.CustomerId)
                .ToDictionary(g => g.Key, g => g.Sum(o => o.Amount));

            var customerCounts = orders
                .GroupBy(o => o.CustomerId)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }

    public class Order
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new CodeFormattingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.7: Correctly Handle Fluent Result Pipelines

    /// <summary>
    /// Tests that fluent Result pipelines are handled correctly.
    /// </summary>
    [Fact]
    public void Should_Handle_Fluent_Result_Pipelines_Correctly()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace MyProject
{
    public class FluentResultExample
    {
        public async Task ProcessResult()
        {
            var result = await Result.Success(""data"")
                .Map(x => x.ToUpper())
                .Bind(x => Result.Success(x + ""_processed""));

            var anotherResult = await Result.Success(42)
                .Map(x => x * 2)
                .Bind(x => Result.Success(x.ToString()));
        }
    }

    public class Result<T>
    {
        public static Result<T> Success(T value) => new Result<T>(value);
        public Result<U> Map<U>(System.Func<T, U> mapper) => new Result<U>(mapper(Value));
        public Result<U> Bind<U>(System.Func<T, Result<U>> binder) => binder(Value);
        private Result(T value) => Value = value;
        public T Value { get; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new CodeFormattingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.8: Correctly Handle Specification Builder Assignments

    /// <summary>
    /// Tests that specification builder assignments are handled correctly.
    /// </summary>
    [Fact]
    public void Should_Handle_Specification_Builder_Assignments_Correctly()
    {
        const string testCode = @"
namespace MyProject
{
    public class SpecificationExample
    {
        public void CreateSpecifications()
        {
            var adultSpec = new Specification<Person>(p => p.Age > 18);
            var nameSpec = new Specification<Person>(p => !string.IsNullOrEmpty(p.Name));
            var complexSpec = new Specification<Person>(p => p.Age > 21 && p.Name != null && p.Name.Length > 2);
        }
    }

    public class Specification<T>
    {
        public Specification(System.Func<T, bool> predicate)
        {
            Predicate = predicate;
        }

        public System.Func<T, bool> Predicate { get; }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new CodeFormattingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.9: Correctly Handle Dictionary Materialization from Collections

    /// <summary>
    /// Tests that dictionary materialization from collections is handled correctly.
    /// </summary>
    [Fact]
    public void Should_Handle_Dictionary_Materialization_From_Collections_Correctly()
    {
        const string testCode = @"
using System.Linq;

namespace MyProject
{
    public class DictionaryMaterializationExample
    {
        public void MaterializeDictionaries()
        {
            var products = new[] 
            { 
                new Product { Id = 1, Name = ""Laptop"" },
                new Product { Id = 2, Name = ""Mouse"" }
            };

            var productDict = products.ToDictionary(p => p.Id, p => p.Name);
            var nameDict = products.ToDictionary(p => p.Name, p => p.Id);
            var complexDict = products.ToDictionary(p => p.Id, p => new { p.Name, Category = ""Electronics"" });
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new CodeFormattingAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 
}
