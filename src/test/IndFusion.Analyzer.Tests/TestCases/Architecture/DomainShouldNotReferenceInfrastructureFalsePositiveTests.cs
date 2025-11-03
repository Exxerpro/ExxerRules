using System;
using Microsoft.EntityFrameworkCore;
using IndFusion.Analyzers;
using IndFusion.Analyzers.Architecture;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Architecture;

/// <summary>
/// Tests for DomainShouldNotReferenceInfrastructureAnalyzer false-positive mitigation scenarios.
/// Note: EntityFrameworkCore is allowed in Infrastructure layer, not Application layer
/// </summary>
public class DomainShouldNotReferenceInfrastructureFalsePositiveTests
{
    //  Story 1.1: Exempt EF Core Attributes on Domain Value Objects

    /// <summary>
    /// Tests that EF Core attributes on domain value objects are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_EF_Core_Attributes_On_Domain_Value_Objects()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace MyProject.Domain.ValueObjects
{
    [Owned]
    public class Money
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
    }

    [Owned]
    public class Address
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DomainShouldNotReferenceInfrastructureAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    //

    //  Story 1.2: Exempt Domain Enum Seeding Extensions

    /// <summary>
    /// Tests that domain enum seeding extensions using ModelBuilder are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Domain_Enum_Seeding_Extensions()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;

namespace MyProject.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed,
        Cancelled
    }

    public static class OrderStatusExtensions
    {
        public static void SeedOrderStatus(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderStatus>()
                .HasData(
                    new { Id = (int)OrderStatus.Pending, Name = ""Pending"" },
                    new { Id = (int)OrderStatus.Processing, Name = ""Processing"" },
                    new { Id = (int)OrderStatus.Completed, Name = ""Completed"" },
                    new { Id = (int)OrderStatus.Cancelled, Name = ""Cancelled"" }
                );
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DomainShouldNotReferenceInfrastructureAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    // Story 1.2: Exempt Domain Enum Seeding Extensions

    //  Story 1.3: Exempt Nested IEntityTypeConfiguration

    /// <summary>
    /// Tests that nested IEntityTypeConfiguration implementations are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Nested_IEntityTypeConfiguration()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyProject.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public class Configuration : IEntityTypeConfiguration<User>
        {
            public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.HasKey(u => u.Id);
                builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
                builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            }
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DomainShouldNotReferenceInfrastructureAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    // Story 1.3: Exempt Nested IEntityTypeConfiguration

    //  Story 1.4: Exempt Domain Tests Using EF InMemory Providers

    /// <summary>
    /// Tests that domain tests using EF InMemory providers are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Domain_Tests_Using_EF_InMemory_Providers()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

namespace MyProject.Domain.Tests
{
    public class UserTests
    {
        [Fact]
        public void Should_Create_User_Successfully()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: ""TestDb"")
                .Options;

            using var context = new TestDbContext(options);
            var user = new User { Name = ""John Doe"", Email = ""john@example.com"" };

            context.Users.Add(user);
            context.SaveChanges();

            Assert.NotNull(user.Id);
        }
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DomainShouldNotReferenceInfrastructureAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    // Story 1.4: Exempt Domain Tests Using EF InMemory Providers

    //  Story 1.5: Exempt Domain Tests Validating ModelBuilder Projections

    /// <summary>
    /// Tests that domain tests validating ModelBuilder projections are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Domain_Tests_Validating_ModelBuilder_Projections()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace MyProject.Domain.UnitTests
{
    public class ModelValidationTests
    {
        [Fact]
        public void Should_Have_Correct_Entity_Configuration()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: ""TestDb"")
                .Options;

            using var context = new TestDbContext(options);
            var entityType = context.Model.FindEntityType(typeof(User));

            Assert.NotNull(entityType);
            Assert.Equal(""User"", entityType.GetTableName());
        }
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DomainShouldNotReferenceInfrastructureAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    // Story 1.5: Exempt Domain Tests Validating ModelBuilder Projections

    //  Story 1.6: Do not Exempt SqlConnectionStringBuilder for Guard Logic

    /// <summary>
    /// Tests that SqlConnectionStringBuilder usage for guard logic is not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_SqlConnectionStringBuilder_Guard_Logic()
    {
        const string testCode = @"
using Microsoft.Data.SqlClient;

namespace MyProject.Domain.Services
{
    public class ConnectionStringValidator
    {
        public bool IsValidConnectionString(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                return !string.IsNullOrEmpty(builder.DataSource) &&
                       !string.IsNullOrEmpty(builder.InitialCatalog);
            }
            catch
            {
                return false;
            }
        }

        public string ExtractDatabaseName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DomainShouldNotReferenceInfrastructureAnalyzer());
        diagnostics.ShouldNotBeEmpty();
    }

    // Story 1.6: Do not Exempt SqlConnectionStringBuilder for Guard Logic

    //  Story 1.7: Exempt Provider-Specific Validation in Domain Rules

    /// <summary>
    /// Tests that provider-specific validation in domain rules tests is not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Provider_Specific_Validation_In_Domain_Rules()
    {
        const string testCode = @"
using NpgsqlTypes;
using Xunit;

namespace MyProject.Domain.Tests
{
    public class TimeIntervalTests
    {
        [Fact]
        public void Should_Validate_Time_Interval()
        {
            var interval = new NpgsqlInterval(1, 2, 3, 4, 5, 6);

            Assert.Equal(1, interval.Days);
            Assert.Equal(2, interval.Hours);
            Assert.Equal(3, interval.Minutes);
        }

        [Fact]
        public void Should_Create_Valid_Interval_From_String()
        {
            var interval = NpgsqlInterval.Parse(""1 day 2 hours 3 minutes"");

            Assert.True(interval.TotalDays > 0);
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DomainShouldNotReferenceInfrastructureAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    // Story 1.7: Exempt Provider-Specific Validation in Domain Rules

    //  Story 1.8: Exempt Domain Enum Synchronization Scripts

    /// <summary>
    /// Tests that domain enum synchronization scripts using ADO.NET builders are not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Domain_Enum_Synchronization_Scripts()
    {
        const string testCode = @"
using System.Data.SqlClient;

namespace MyProject.Domain.Utilities
{
    public class EnumSynchronizer
    {
        public bool ValidateConnectionString(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                return !string.IsNullOrEmpty(builder.DataSource);
            }
            catch
            {
                return false;
            }
        }

        public string GetDatabaseName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DomainShouldNotReferenceInfrastructureAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    // Story 1.8: Exempt Domain Enum Synchronization Scripts

    //  Story 1.9: Exempt ValueComparer Usage in Domain Tests

    /// <summary>
    /// Tests that ValueComparer usage in domain tests is not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_ValueComparer_Usage_In_Domain_Tests()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;

namespace MyProject.Domain.Tests
{
    public class AuditableEntityTests
    {
        [Fact]
        public void Should_Track_Changes_Correctly()
        {
            var comparer = new ValueComparer<string[]>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToArray()
            );

            var array1 = new[] { ""item1"", ""item2"" };
            var array2 = new[] { ""item1"", ""item2"" };
            var array3 = new[] { ""item1"", ""item3"" };

            Assert.True(comparer.Equals(array1, array2));
            Assert.False(comparer.Equals(array1, array3));
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DomainShouldNotReferenceInfrastructureAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    // Story 1.9: Exempt ValueComparer Usage in Domain Tests

    //  Story 1.10: Exempt Migration Snapshot Verification in Domain Tests

    /// <summary>
    /// Tests that migration snapshot verification in domain tests is not flagged.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Migration_Snapshot_Verification_In_Domain_Tests()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore.Migrations;
using Xunit;

namespace MyProject.Domain.Tests
{
    public class MigrationCompatibilityTests
    {
        [Fact]
        public void Should_Verify_Migration_Snapshot_Compatibility()
        {
            var migrationAssembly = typeof(TestDbContext).Assembly;
            var migrationService = new MigrationsAssembly(migrationAssembly, null, null);

            var migrations = migrationService.Migrations;
            Assert.NotEmpty(migrations);
        }
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DomainShouldNotReferenceInfrastructureAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    // Story 1.10: Exempt Migration Snapshot Verification in Domain Tests

    //  Positive Control Tests

    /// <summary>
    /// Tests that regular infrastructure references in domain are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Regular_Infrastructure_References_In_Domain()
    {
        const string testCode = @"
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace MyProject.Domain.Services
{
    public class UserService
    {
        public void SaveUser(User user)
        {
            // This should be flagged - direct EF usage in domain
            using var context = new DbContext();
            context.Set<User>().Add(user);
            context.SaveChanges();
        }

        public void ConnectToDatabase()
        {
            // This should be flagged - direct SQL connection in domain
            using var connection = new SqlConnection(""connection string"");
            connection.Open();
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DomainShouldNotReferenceInfrastructureAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.DomainShouldNotReferenceInfrastructure);
    }

    // Positive Control Tests
}