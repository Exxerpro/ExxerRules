using System;
using Microsoft.EntityFrameworkCore;
using IndFusion.Analyzers;
using IndFusion.Analyzers.Architecture;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Architecture;

/// <summary>
/// Tests for UseRepositoryPatternAnalyzer false-positive mitigation scenarios.
/// Note: EntityFrameworkCore is allowed in Infrastructure layer, not Application layer
/// </summary>
public class UseRepositoryPatternFalsePositiveTests
{
    //  Story 1.1: Exempt Application Layer Handlers

    /// <summary>
    /// Tests that application layer handlers using DbContext are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Application_Layer_Handlers()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace MyProject.Application.Commands
{
    public class CreateUserCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class CreateUserCommandHandler
    {
        private readonly ApplicationDbContext _context;

        public CreateUserCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User { Name = request.Name, Email = request.Email };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user.Id;
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
        public string Email { get; set; } = string.Empty;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    //

    //  Story 1.2: Exempt Infrastructure Layer

    /// <summary>
    /// Tests that infrastructure layer classes using DbContext are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Infrastructure_Layer()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace MyProject.Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }

    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User user);
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
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    //

    //  Story 1.3: Exempt Test and Fixture Classes

    /// <summary>
    /// Tests that test and fixture classes using DbContext are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Test_And_Fixture_Classes()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MyProject.Tests
{
    public class UserServiceTests
    {
        private readonly ApplicationDbContext _context;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: ""TestDb"")
                .Options;
            _context = new ApplicationDbContext(options);
        }

        [Fact]
        public void Should_Create_User_Successfully()
        {
            var user = new User { Name = ""John Doe"", Email = ""john@example.com"" };
            _context.Users.Add(user);
            _context.SaveChanges();
            Assert.NotNull(user.Id);
        }
    }

    public class DatabaseFixture : IDisposable
    {
        public ApplicationDbContext Context { get; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: ""TestDb"")
                .Options;
            Context = new ApplicationDbContext(options);
        }

        public void Dispose()
        {
            Context.Dispose();
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
        public string Email { get; set; } = string.Empty;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    //

    //  Story 1.4: Exempt Connection Wrapper Classes

    /// <summary>
    /// Tests that connection wrapper classes using connection objects are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Connection_Wrapper_Classes()
    {
        const string testCode = @"
using System.Data.SqlClient;

namespace MyProject.Infrastructure.Connections
{
    public class DatabaseConnection
    {
        private readonly IDbConnection _connection;

        public DatabaseConnection(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Open()
        {
            _connection.Open();
        }

        public void Close()
        {
            _connection.Close();
        }
    }

    public class SqlConnector
    {
        private readonly SqlConnection _connection;

        public SqlConnector(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public async Task ExecuteCommandAsync(string sql)
        {
            await _connection.OpenAsync();
            using var command = new SqlCommand(sql, _connection);
            await command.ExecuteNonQueryAsync();
        }
    }

    public class ConnectionFactory
    {
        private readonly string _connectionString;

        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    //

    //  Story 1.5: Exempt DbContextOptions and EF Services

    /// <summary>
    /// Tests that DbContextOptions and EF services injected into constructors are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_DbContextOptions_And_EF_Services()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MyProject.Infrastructure.Services
{
    public class DbContextFactory
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public DbContextFactory(DbContextOptions<ApplicationDbContext> options)
        {
            _options = options;
        }

        public ApplicationDbContext CreateContext()
        {
            return new ApplicationDbContext(_options);
        }
    }

    public class DbContextManager
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public DbContextManager(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<ApplicationDbContext> CreateContextAsync()
        {
            return await _factory.CreateDbContextAsync();
        }
    }

    public class ServiceScopeManager
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ServiceScopeManager(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public IServiceScope CreateScope()
        {
            return _scopeFactory.CreateScope();
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
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    //

    //  Story 1.6: Exempt Minimal APIs and Program.cs

    /// <summary>
    /// Tests that Program.cs and files with top-level statements using DbContext are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Minimal_APIs_And_Program_cs()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;

// Top-level statements in Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(""DefaultConnection"")));

var app = builder.Build();

app.MapGet(""/users"", async (ApplicationDbContext context) =>
{
    return await context.Users.ToListAsync();
});

app.MapPost(""/users"", async (User user, ApplicationDbContext context) =>
{
    context.Users.Add(user);
    await context.SaveChangesAsync();
    return Results.Created($""/users/{user.Id}"", user);
});

app.Run();

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; } = null!;
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    //

    //  Story 1.7: Exempt Generic Infrastructure Services

    /// <summary>
    /// Tests that generic infrastructure services using DbContext are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Generic_Infrastructure_Services()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace MyProject.Infrastructure.Services
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void BeginTransaction()
        {
            _context.Database.BeginTransaction();
        }
    }

    public class DbContextTransactionBehavior
    {
        private readonly ApplicationDbContext _context;

        public DbContextTransactionBehavior(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await operation();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public class MigrationService
    {
        private readonly ApplicationDbContext _context;

        public MigrationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task MigrateAsync()
        {
            await _context.Database.MigrateAsync();
        }
    }

    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (!_context.Users.Any())
            {
                _context.Users.Add(new User { Name = ""Admin"", Email = ""admin@example.com"" });
                await _context.SaveChangesAsync();
            }
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
        public string Email { get; set; } = string.Empty;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    //

    //  Story 1.8: Exempt Generic Repository Base Classes

    /// <summary>
    /// Tests that generic repository base classes with DbContext are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Generic_Repository_Base_Classes()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace MyProject.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TContext> where TContext : DbContext
    {
        protected readonly TContext _context;

        protected RepositoryBase(TContext context)
        {
            _context = context;
        }

        protected virtual IQueryable<T> GetQueryable<T>() where T : class
        {
            return _context.Set<T>();
        }

        protected virtual async Task<T?> FindAsync<T>(params object[] keyValues) where T : class
        {
            return await _context.Set<T>().FindAsync(keyValues);
        }
    }

    public class UserRepository : RepositoryBase<ApplicationDbContext>
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await FindAsync<User>(id);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
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
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    //

    //  Story 1.9: Exempt Domain-Specific EF Extensions

    /// <summary>
    /// Tests that domain-specific EF Core extension methods are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Domain_Specific_EF_Extensions()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace MyProject.Infrastructure.Persistence.Extensions
{
    public static class UserExtensions
    {
        public static IQueryable<User> ActiveUsers(this DbSet<User> users)
        {
            return users.Where(u => u.IsActive);
        }

        public static IQueryable<User> ByEmail(this IQueryable<User> users, string email)
        {
            return users.Where(u => u.Email == email);
        }

        public static async Task<User?> FindByEmailAsync(this DbSet<User> users, string email)
        {
            return await users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }

    public static class OrderExtensions
    {
        public static IQueryable<Order> PendingOrders(this DbSet<Order> orders)
        {
            return orders.Where(o => o.Status == OrderStatus.Pending);
        }

        public static IQueryable<Order> ByCustomer(this IQueryable<Order> orders, int customerId)
        {
            return orders.Where(o => o.CustomerId == customerId);
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Completed,
        Cancelled
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    //

    //  Story 1.10: Provide an Opt-Out Attribute

    /// <summary>
    /// Tests that classes with AllowDirectDataAccess attribute are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Classes_With_AllowDirectDataAccess_Attribute()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace MyProject.Application.Services
{
    [AllowDirectDataAccess]
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }

    [AllowDirectDataAccess]
    public class ReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetActiveUsersAsync()
        {
            return await _context.Users.Where(u => u.IsActive).ToListAsync();
        }
    }

    public class AllowDirectDataAccessAttribute : Attribute
    {
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    //

    //  Positive Control Tests

    /// <summary>
    /// Tests that regular domain services using DbContext are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Regular_Domain_Services_Using_DbContext()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace MyProject.Domain.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserAsync(int id)
        {
            return await _context.Users.FindAsync(id);
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
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseRepositoryPatternAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.UseRepositoryPattern);
    }

    //
}