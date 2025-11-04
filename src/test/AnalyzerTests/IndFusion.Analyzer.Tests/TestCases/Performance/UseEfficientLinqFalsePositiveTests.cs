using System;
using System.Collections.Generic;
using System.Linq;
using IndFusion.Analyzer.Performance;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Performance;

/// <summary>
/// Tests for UseEfficientLinqAnalyzer false-positive mitigation scenarios.
/// </summary>
public class UseEfficientLinqFalsePositiveTests
{
    //  Story 1.1: Exempt Guard Patterns on ICollection and Arrays

    /// <summary>
    /// Tests that guard patterns on ICollection and arrays are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Guard_Patterns_On_ICollection_And_Arrays()
    {
        const string testCode = @"
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Services
{
    public class UserService
    {
        private readonly List<User> _users;
        private readonly User[] _userArray;
        private readonly ICollection<User> _userCollection;

        public UserService()
        {
            _users = new List<User>();
            _userArray = new User[10];
            _userCollection = new List<User>();
        }

        public bool HasUsers()
        {
            // These should not be flagged - efficient guard patterns
            return _users.Any() && _userArray.Any() && _userCollection.Any();
        }

        public int GetUserCount()
        {
            // These should not be flagged - efficient count operations
            return _users.Count() + _userArray.Count() + _userCollection.Count();
        }

        public User? GetFirstUser()
        {
            // Guard pattern - should not be flagged
            if (!_users.Any())
                return null;
            
            return _users.First();
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseEfficientLinqAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.2: Recognize Materialized Queries

    /// <summary>
    /// Tests that materialized queries are not flagged for multiple operations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Materialized_Queries()
    {
        const string testCode = @"
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Services
{
    public class DataService
    {
        private readonly List<User> _users;

        public DataService()
        {
            _users = new List<User>();
        }

        public void ProcessUsers()
        {
            // Materialized query - should not be flagged for multiple operations
            var activeUsers = _users.Where(u => u.IsActive).ToList();
            
            var count = activeUsers.Count();
            var hasAny = activeUsers.Any();
            var first = activeUsers.FirstOrDefault();
        }

        public void ProcessUsersAsArray()
        {
            // Materialized as array - should not be flagged
            var users = _users.Where(u => u.IsActive).ToArray();
            
            var count = users.Count();
            var hasAny = users.Any();
        }

        public void ProcessUsersAsHashSet()
        {
            // Materialized as HashSet - should not be flagged
            var userSet = _users.Where(u => u.IsActive).ToHashSet();
            
            var count = userSet.Count();
            var hasAny = userSet.Any();
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseEfficientLinqAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.3: Exempt IQueryable

    /// <summary>
    /// Tests that IQueryable chained operations are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_IQueryable_Chained_Operations()
    {
        const string testCode = @"
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MyProject.Infrastructure
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetActiveUsers()
        {
            // IQueryable chaining - should not be flagged
            return _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.Name)
                .Take(10);
        }

        public IQueryable<User> GetUsersByRole(string role)
        {
            // Complex IQueryable chain - should not be flagged
            return _context.Users
                .Where(u => u.Role == role)
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.CreatedDate)
                .Skip(0)
                .Take(20);
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseEfficientLinqAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.4: Exempt Set Operations

    /// <summary>
    /// Tests that set operations are not flagged as multiple enumerations.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Set_Operations()
    {
        const string testCode = @"
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Services
{
    public class SetOperationService
    {
        private readonly List<int> _list1;
        private readonly List<int> _list2;

        public SetOperationService()
        {
            _list1 = new List<int> { 1, 2, 3, 4, 5 };
            _list2 = new List<int> { 4, 5, 6, 7, 8 };
        }

        public void PerformSetOperations()
        {
            // Set operations produce new sequences - should not be flagged
            var union = _list1.Union(_list2);
            var intersection = _list1.Intersect(_list2);
            var except = _list1.Except(_list2);
            var concat = _list1.Concat(_list2);
            var distinct = _list1.Distinct();

            // Multiple operations on set operation results should not be flagged
            var unionCount = union.Count();
            var unionAny = union.Any();
            var unionFirst = union.First();
        }

        public void ChainedSetOperations()
        {
            // Chained set operations - should not be flagged
            var result = _list1
                .Union(_list2)
                .Distinct()
                .OrderBy(x => x)
                .Take(5);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseEfficientLinqAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.5: Exempt Any() Guard Followed by First() on Lists

    /// <summary>
    /// Tests that Any() guard followed by First() on lists is not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Any_Guard_Followed_By_First_On_Lists()
    {
        const string testCode = @"
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Services
{
    public class GuardPatternService
    {
        private readonly List<User> _users;
        private readonly User[] _userArray;

        public GuardPatternService()
        {
            _users = new List<User>();
            _userArray = new User[10];
        }

        public User? GetFirstActiveUser()
        {
            // Guard pattern on List - should not be flagged
            if (_users.Any())
            {
                return _users.First();
            }
            return null;
        }

        public User? GetFirstUserFromArray()
        {
            // Guard pattern on Array - should not be flagged
            if (_userArray.Any())
            {
                return _userArray.First();
            }
            return null;
        }

        public bool HasActiveUsers()
        {
            // Guard pattern in boolean expression - should not be flagged
            return _users.Any() && _users.First().IsActive;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseEfficientLinqAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.6: Exempt Async LINQ

    /// <summary>
    /// Tests that async LINQ methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Async_LINQ()
    {
        const string testCode = @"
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MyProject.Infrastructure
{
    public class AsyncUserRepository
    {
        private readonly ApplicationDbContext _context;

        public AsyncUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasActiveUsersAsync()
        {
            // Async LINQ on IQueryable - should not be flagged
            return await _context.Users.AnyAsync(u => u.IsActive);
        }

        public async Task<User?> GetFirstActiveUserAsync()
        {
            // Async LINQ chain - should not be flagged
            return await _context.Users
                .Where(u => u.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetActiveUserCountAsync()
        {
            // Async count - should not be flagged
            return await _context.Users.CountAsync(u => u.IsActive);
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseEfficientLinqAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.7: Exempt Null-Coalesced Enumerables

    /// <summary>
    /// Tests that null-coalesced enumerables are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Null_Coalesced_Enumerables()
    {
        const string testCode = @"
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Services
{
    public class NullCoalescingService
    {
        public void ProcessNullableCollection(IEnumerable<User>? users)
        {
            // Null-coalesced enumerable - should not be flagged
            var safeUsers = users ?? Enumerable.Empty<User>();
            
            var count = safeUsers.Count();
            var hasAny = safeUsers.Any();
            var first = safeUsers.FirstOrDefault();
        }

        public void ProcessNullableList(List<User>? users)
        {
            // Null-coalesced with empty list - should not be flagged
            var safeUsers = users ?? new List<User>();
            
            var count = safeUsers.Count();
            var hasAny = safeUsers.Any();
        }

        public bool HasUsers(IEnumerable<User>? users)
        {
            // Direct null-coalescing in expression - should not be flagged
            return (users ?? Enumerable.Empty<User>()).Any();
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseEfficientLinqAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.8: Exempt Expression-Bodied Properties

    /// <summary>
    /// Tests that expression-bodied properties using LINQ are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Expression_Bodied_Properties()
    {
        const string testCode = @"
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Models
{
    public class UserCollection
    {
        private readonly List<User> _users;

        public UserCollection()
        {
            _users = new List<User>();
        }

        // Expression-bodied properties - should not be flagged
        public bool HasUsers => _users.Any();
        public int UserCount => _users.Count();
        public bool HasActiveUsers => _users.Any(u => u.IsActive);
        public User? FirstUser => _users.FirstOrDefault();
        public bool IsEmpty => !_users.Any();

        public void AddUser(User user)
        {
            _users.Add(user);
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseEfficientLinqAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.9: Differentiate Query Variables Semantically

    /// <summary>
    /// Tests that different query variables are differentiated semantically.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Different_Query_Variables()
    {
        const string testCode = @"
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Services
{
    public class QueryVariableService
    {
        private readonly List<User> _users;
        private readonly List<Order> _orders;

        public QueryVariableService()
        {
            _users = new List<User>();
            _orders = new List<Order>();
        }

        public void ProcessDifferentCollections()
        {
            // Different collections - should not be flagged
            var activeUsers = _users.Where(u => u.IsActive);
            var recentOrders = _orders.Where(o => o.Date > DateTime.Now.AddDays(-30));

            var userCount = activeUsers.Count();
            var orderCount = recentOrders.Count();
        }

        public void ProcessSameCollectionDifferentQueries()
        {
            // Different queries on same collection - should not be flagged
            var activeUsers = _users.Where(u => u.IsActive);
            var adminUsers = _users.Where(u => u.Role == ""Admin"");

            var activeCount = activeUsers.Count();
            var adminCount = adminUsers.Count();
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseEfficientLinqAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Story 1.10: Provide an Opt-Out Attribute

    /// <summary>
    /// Tests that methods with AllowMultipleEnumeration attribute are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Methods_With_AllowMultipleEnumeration_Attribute()
    {
        const string testCode = @"
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Services
{
    public class OptOutService
    {
        private readonly List<User> _users;

        public OptOutService()
        {
            _users = new List<User>();
        }

        [AllowMultipleEnumeration]
        public void ProcessUsersWithOptOut()
        {
            // This should not be flagged due to the attribute
            var activeUsers = _users.Where(u => u.IsActive);
            
            var count = activeUsers.Count();
            var hasAny = activeUsers.Any();
            var first = activeUsers.FirstOrDefault();
        }

        [AllowMultipleEnumeration]
        public bool CheckUsers()
        {
            // This should not be flagged due to the attribute
            return _users.Any() && _users.First().IsActive;
        }
    }

    public class AllowMultipleEnumerationAttribute : Attribute
    {
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseEfficientLinqAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

     // 

    //  Positive Control Tests

    /// <summary>
    /// Tests that actual inefficient LINQ usage is still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Actual_Inefficient_LINQ_Usage()
    {
        const string testCode = @"
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Services
{
    public class InefficientService
    {
        private readonly IEnumerable<User> _users;

        public InefficientService()
        {
            _users = new List<User>();
        }

        public void InefficientProcessing()
        {
            // This should be flagged - multiple enumerations of IEnumerable
            var activeUsers = _users.Where(u => u.IsActive);
            
            var count = activeUsers.Count();
            var hasAny = activeUsers.Any();
            var first = activeUsers.FirstOrDefault();
        }

        public bool InefficientCheck()
        {
            // This should be flagged - multiple enumerations in expression
            return _users.Any() && _users.First().IsActive;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseEfficientLinqAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.UseEfficientLinq);
    }

     // 
}
