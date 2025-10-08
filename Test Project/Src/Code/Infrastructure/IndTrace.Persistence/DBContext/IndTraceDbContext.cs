using Castle.Core.Logging;
using IndTrace.Application.UserService;
using IndTrace.Domain.Entities;
using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Domain.Enum;
using IndTrace.Domain.Enum.LookUpTable;
using IndTrace.Domain.Interfaces;
using IndTrace.Domain.Models;

using IndTrace.Persistence.Converters;
using IndTrace.Persistence.Extensions;
using IndTrace.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Castle.Core.Logging.ILogger;
using Rule = IndTrace.Domain.Entities.Rule;

namespace IndTrace.Persistence.DBContext;

/// <summary>
/// Represents the primary Entity Framework database context for the IndTrace manufacturing traceability system.
/// Manages database entities for production cycles, machine data, PLCs, barcode scanning, and OEE calculations.
/// Implements auditable entity tracking and provides specialized methods for SQL Server identity operations.
/// </summary>
public class IndTraceDbContext : DbContext, IIndTraceDbContext, IAsyncDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndTraceDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public IndTraceDbContext(DbContextOptions<IndTraceDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Maps to SQL Server's SOUNDEX function for phonetic string comparison in manufacturing part identification.
    /// Used for finding similar part numbers when barcode reads are partially corrupted or unclear.
    /// </summary>
    /// <param name="input">The input string to generate a SOUNDEX code for.</param>
    /// <returns>A SOUNDEX phonetic representation of the input string.</returns>
    [DbFunction(Name = "SoundEx", IsBuiltIn = true)]
    public static string SoundLike(string input)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets or sets the DbSet for ResultValidation entities.
    /// </summary>
    public DbSet<ResultValidation> ResultValidation => Set<ResultValidation>();

    /// <summary>
    /// Gets or sets the DbSet for CycleStatus entities.
    /// </summary>
    public DbSet<CycleStatusEntity> CycleStatus => Set<CycleStatusEntity>();

    /// <summary>
    /// Gets or sets the DbSet for FlowStatus entities that define production flow states.
    /// </summary>
    public DbSet<FlowStatusEntity> FlowStatus => Set<FlowStatusEntity>();

    /// <summary>
    /// Gets or sets the DbSet for PartStatus entities that define quality status of manufactured parts.
    /// </summary>
    public DbSet<PartStatusEntity> PartStatus => Set<PartStatusEntity>();

    /// <summary>
    /// Gets or sets the DbSet for MachineType entities that categorize manufacturing equipment.
    /// </summary>
    [Required]
    public DbSet<MachineTypeEntity> MachineTypes => Set<MachineTypeEntity>();

    /// <summary>
    /// Gets or sets the DbSet for TagsGroup entities used for organizing PLC variable tags.
    /// </summary>
    [Required]
    public DbSet<TagsGroupEntity> TagsGroups => Set<TagsGroupEntity>();

    /// <summary>
    /// Gets or sets the DbSet for DistinctRegister entities containing unique register data.
    /// </summary>
    [Required]
    public DbSet<DistinctRegister> DistinctRegisters => Set<DistinctRegister>();

    /// <summary>
    /// Gets or sets the DbSet for WorkFlowType entities that define manufacturing workflow categories.
    /// </summary>
    [Required]
    public DbSet<WorkFlowTypeEntity> WorkFlowTypes => Set<WorkFlowTypeEntity>();

    /// <summary>
    /// Gets or sets the DbSet for GatewayTask entities that define available PLC communication tasks.
    /// </summary>
    [Required]
    public DbSet<GatewayTaskEntity> GatewayTask => Set<GatewayTaskEntity>();

    /// <summary>
    /// Gets or sets the DbSet for TaskGatewayRequest entities used for tracking PLC communication commands.
    /// </summary>
    [Required]
    public DbSet<TaskGatewayRequest> Requests => Set<TaskGatewayRequest>();

    /// <summary>
    /// Gets or sets the DbSet for TaskGatewayResponse entities used for tracking PLC communication responses.
    /// </summary>
    [Required]
    public DbSet<TaskGatewayResponse> Responses => Set<TaskGatewayResponse>();

    /// <summary>
    /// Gets or sets the DbSet for ConfigApp entities containing application configuration settings.
    /// </summary>
    [Required]
    public DbSet<ConfigApp> ConfigApps => Set<ConfigApp>();

    /// <summary>
    /// Gets or sets the DbSet for ShiftDefinition entities that define work shift schedules.
    /// </summary>
    [Required]
    public DbSet<ShiftsCatalog> ShiftsCatalog => Set<ShiftsCatalog>();

    /// <summary>
    /// Gets or sets the DbSet for ConfigDatabaseLog entities for database logging configuration.
    /// </summary>
    [Required]
    public DbSet<ConfigDatabaseLog> ConfigDatabaseLog => Set<ConfigDatabaseLog>();

    /// <summary>
    /// Gets or sets the DbSet for ConfigDb entities containing database configuration settings.
    /// </summary>
    [Required]
    public DbSet<ConfigDb> ConfigDb => Set<ConfigDb>();

    /// <summary>
    /// Gets or sets the DbSet for KpiOee entities containing Overall Equipment Effectiveness metrics.
    /// </summary>
    [Required]
    public DbSet<KpiOee> KpiOee => Set<KpiOee>();

    /// <summary>
    /// Gets or sets the DbSet for BarCode entities representing scanned manufacturing labels.
    /// </summary>
    [Required]
    public DbSet<BarCode> BarCodes => Set<BarCode>();

    /// <summary>
    /// Gets or sets the DbSet for Cycle entities tracking individual production cycles.
    /// </summary>
    [Required]
    public DbSet<Cycle> Cycles => Set<Cycle>();

    /// <summary>
    /// Gets or sets the DbSet for Defect entities recording quality defects found during production.
    /// </summary>
    [Required]
    public DbSet<Defect> Defects => Set<Defect>();

    /// <summary>
    /// Gets or sets the DbSet for MasterLabel entities containing barcode label master data.
    /// </summary>
    [Required]
    public DbSet<MasterLabel> MasterLabel => Set<MasterLabel>();

    /// <summary>
    /// Gets or sets the DbSet for WorkFlow entities defining manufacturing process workflows.
    /// </summary>
    [Required]
    public DbSet<WorkFlow> WorkFlows => Set<WorkFlow>();

    /// <summary>
    /// Gets or sets the DbSet for Machine entities representing manufacturing equipment.
    /// </summary>
    [Required]
    public DbSet<Machine> Machines => Set<Machine>();

    /// <summary>
    /// Gets or sets the DbSet for Plc entities representing programmable logic controllers.
    /// </summary>
    [Required]
    public DbSet<Plc> Plcs => Set<Plc>();

    /// <summary>
    /// Gets or sets the DbSet for MachinePlc entities linking machines to their PLCs.
    /// </summary>
    [Required]
    public DbSet<MachinePlc> MachinePlcs => Set<MachinePlc>();

    /// <summary>
    /// Gets or sets the DbSet for Tooling entities representing manufacturing tools and fixtures.
    /// </summary>
    [Required]
    public DbSet<Tooling> Toolings => Set<Tooling>();

    /// <summary>
    /// Gets or sets the DbSet for Order entities representing manufacturing work orders.
    /// </summary>
    [Required]
    public DbSet<Order> Orders => Set<Order>();

    /// <summary>
    /// Gets or sets the DbSet for Stoppage entities recording production line stoppages.
    /// </summary>
    [Required]
    public DbSet<Stoppage> Stoppages => Set<Stoppage>();

    /// <summary>
    /// Gets or sets the DbSet for PerformanceSpec entities defining performance specifications.
    /// </summary>
    [Required]
    public DbSet<PerformanceSpec> PerformanceSpecs => Set<PerformanceSpec>();

    /// <summary>
    /// Gets or sets the DbSet for ProductSpec entities defining product specifications.
    /// </summary>
    [Required]
    public DbSet<ProductSpec> ProductSpecs => Set<ProductSpec>();

    /// <summary>
    /// Gets or sets the DbSet for Product entities.
    /// </summary>
    [Required]
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// Gets or sets the DbSet for Customer entities.
    /// </summary>
    [Required]
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>
    /// Gets or sets the DbSet for Shift entities representing work shift schedules and assignments.
    /// </summary>
    [Required]
    public DbSet<Shift> Shifts => Set<Shift>();

    /// <summary>
    /// Gets or sets the DbSet for Recipe entities containing manufacturing process recipes.
    /// </summary>
    [Required]
    public DbSet<Recipe> Recipes => Set<Recipe>();

    /// <summary>
    /// Gets or sets the DbSet for DefectRegister entities tracking recorded quality defects.
    /// </summary>
    [Required]
    public DbSet<DefectRegister> DefectRegisters => Set<DefectRegister>();

    /// <summary>
    /// Gets or sets the DbSet for StoppageRegister entities tracking recorded production stoppages.
    /// </summary>
    [Required]
    public DbSet<StoppageRegister> StoppageRegisters => Set<StoppageRegister>();

    /// <summary>
    /// Gets or sets the DbSet for Setting entities containing system configuration settings.
    /// </summary>
    [Required]
    public DbSet<Setting> Settings => Set<Setting>();

    /// <summary>
    /// Gets or sets the DbSet for ConnectionStatus entities tracking PLC connection states.
    /// </summary>
    [Required]
    public DbSet<ConnectionStatus> ConnectionStatus => Set<ConnectionStatus>();

    /// <summary>
    /// Gets or sets the DbSet for StatusConfiguration entities for system status configuration.
    /// </summary>
    [Required]
    public DbSet<StatusConfiguration> StatusConfigurations => Set<StatusConfiguration>();

    /// <summary>
    /// Gets or sets the DbSet for MachineStatus entities tracking current machine operational states.
    /// </summary>
    [Required]
    public DbSet<MachineStatus> MachineStatus => Set<MachineStatus>();

    /// <summary>
    /// Gets or sets the DbSet for VariablesGroup entities for organizing PLC variables into logical groups.
    /// </summary>
    [Required]
    public DbSet<VariablesGroup> VariablesGroups => Set<VariablesGroup>();

    /// <summary>
    /// Gets or sets the DbSet for IndTraceUser entities representing system users.
    /// </summary>
    [Required]
    public DbSet<IndTraceUser> Users => Set<IndTraceUser>();

    /// <summary>
    /// Gets or sets the DbSet for Register entities containing manufacturing data register records.
    /// </summary>
    [Required]
    public DbSet<Register> Registers => Set<Register>();

    /// <summary>
    /// Gets or sets the DbSet for Variable entities.
    /// </summary>
    [Required]
    public DbSet<Variable> Variables => Set<Variable>();

    /// <summary>
    /// Gets or sets the DbSet for PerformanceData entities containing machine performance metrics.
    /// </summary>
    [Required]
    public DbSet<Domain.Entities.PerformanceData> PerformanceDatas => Set<Domain.Entities.PerformanceData>();

    /// <summary>
    /// Gets or sets the DbSet for OeeRegister entities tracking Overall Equipment Effectiveness data.
    /// </summary>
    [Required]
    public DbSet<Domain.Entities.OeeRegister> OeeRegisters => Set<Domain.Entities.OeeRegister>();

    /// <summary>
    /// Gets or sets the DbSet for Rule entities defining business rules for manufacturing operations.
    /// </summary>
    [Required]
    public DbSet<Rule> Rules => Set<Rule>();

    /// <summary>
    /// Gets or sets the model for the context, or null if the context is disposed.
    /// </summary>
    public new IModel? Model
    {
        get
        {
            try
            {
                return base.Model;
            }
            catch (ObjectDisposedException)
            {
                // This exception occurs when the context is disposed and the model is accessed.
                // in this case, we can return null or handle it as needed.
                // For example, you might want to log the error or throw a custom exception.
                // I think we don't have a logger yet, so let's just return null.
                return null;
            }
        }
    }

    /// <summary>
    /// Gets or sets the DbSet for Line entities representing production lines in the manufacturing facility.
    /// </summary>
    [Required]
    public DbSet<Line> Lines => Set<Line>();

    /// <summary>
    /// Enables the identity insert for a specific table and saves changes asynchronously.
    /// </summary>
    /// <param name="tableName">The table name.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the number of affected rows or an error message.</returns>
    public async Task<Result<int>> SaveChangesAsync(string tableName, CancellationToken cancellationToken = default)
    {
        if (!this.Database.IsRelational())
            return Result<int>.WithFailure(["The operation is only supported for relational databases."]);

        // Create an execution strategy
        var strategy = this.Database.CreateExecutionStrategy();

        try
        {
            // Execute all operations in a single retriable unit of work
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await this.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var queryOn = $"SET IDENTITY_INSERT {tableName} ON";
                    await this.Database.ExecuteSqlRawAsync(queryOn, cancellationToken);

                    var result = await this.SaveChangesAsync(cancellationToken);

                    var queryOff = $"SET IDENTITY_INSERT {tableName} OFF";
                    await this.Database.ExecuteSqlRawAsync(queryOff, cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    return Result<int>.Success(result);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<int>.WithFailure([ex.Message]);
                }
            });
        }
        catch (Exception ex)
        {
            return Result<int>.WithFailure([ex.Message]);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the database supports transactions.
    /// </summary>
    public bool SupportsTransactions => this.Database.IsRelational();

    /// <summary>
    /// Gets a value indicating whether the database connection is active.
    /// </summary>
    public bool IsConnectionActive => this.Database.CanConnect();

    /// <summary>
    /// Gets a value indicating whether the database connection is inactive.
    /// </summary>
    public bool IsConnectionInactive => !this.Database.CanConnect();

    IModel IIndTraceDbContext.Model => base.Model;

    /// <summary>
    /// Sets testing interfaces for dependency injection during unit and integration testing.
    /// </summary>
    /// <param name="indTraceUserService">The user service instance for testing, or null to use default.</param>
    /// <param name="dateTimeMachine">The date time machine instance for testing, or null to use default.</param>
    /// <remarks>
    /// This method is intended for testing scenarios where mock implementations of services are required.
    /// If null values are provided, default implementations will be used.
    /// </remarks>
    public void SetTestingInterfaces(IIndTraceUserService indTraceUserService, IDateTimeMachine dateTimeMachine)
    {
        this.indTraceUserService = indTraceUserService ?? new IndTraceUserService();
        this.dateTimeMachine = dateTimeMachine ?? new DateTimeMachine();
    }

    private IIndTraceUserService indTraceUserService = new IndTraceUserService();

    private IDateTimeMachine dateTimeMachine = new DateTimeMachine();
    /// <summary>
    /// Executes SaveChangesAsync operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of SaveChangesAsync.</returns>

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        foreach (var entry in this.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    //TODO VERIFY NOT NULL

                    if (this.indTraceUserService is not null)
                    {
                        entry.Entity.CreatedBy = await this.indTraceUserService.CurrentUserName;
                    }
                    else
                    {
                        entry.Entity.CreatedBy = "System";
                    }

                    entry.Entity.CreatedOn = this.dateTimeMachine is not null ? this.dateTimeMachine.Now.ToLocalTime() : DateTime.Now.ToLocalTime();

                    break;

                case EntityState.Modified:

                    if (this.indTraceUserService is not null)
                    {
                        entry.Entity.ModifiedBy = await this.indTraceUserService.CurrentUserId;
                    }
                    else
                    {
                        entry.Entity.CreatedBy = "System";
                    }

                    entry.Entity.CreatedOn = this.dateTimeMachine is not null ?
                        this.dateTimeMachine.Now.ToLocalTime() : DateTime.Now.ToLocalTime();
                    break;

                case EntityState.Detached:
                    break;

                case EntityState.Unchanged:
                    break;

                case EntityState.Deleted:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core validation fix - Exclude non-persisted entities from model
        modelBuilder.Ignore<RuleFragment>();

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core validation fix - Exclude EnumModel types as only Entity versions are persisted
        modelBuilder.Ignore<CycleStatus>();
        modelBuilder.Ignore<FlowStatus>();
        modelBuilder.Ignore<GatewayTask>();
        modelBuilder.Ignore<MachineType>();
        modelBuilder.Ignore<PartStatus>();
        modelBuilder.Ignore<ResultValidation>();
        modelBuilder.Ignore<ShiftType>();
        modelBuilder.Ignore<WorkFlowType>();

        //[Fix]
        //CLAUDE
        //Date: 02/09/2025
        //Reason: [Conflict Resolution] - ShiftsCatalog DbSet exists but was ignored, causing NullReferenceException in EntityKeyResolver
        // Removed modelBuilder.Ignore<ShiftsCatalog>() to allow EF Core model recognition

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core validation fix - Exclude Result<T> pattern classes from being treated as entities
        modelBuilder.Ignore<Result>();
        modelBuilder.Ignore<Result<object>>();

        // [Integration Test Unblock]
        // Reason: Two entity types (ShiftsCatalog, ShiftDefinition) were mapped to the same table 'ShiftsCatalog'
        // via configuration scanning, causing EF Core model validation to fail. The domain uses ShiftsCatalog, and
        // there is no DbSet for ShiftDefinition. Ignore ShiftDefinition to prevent duplicate table mapping.
        modelBuilder.Ignore<ShiftDefinition>();

        modelBuilder.UseValueConverterForType<Label>(new ValueConverterLabel());

        // Ensure critical configurations are applied explicitly
        modelBuilder.ApplyConfiguration(new IndTrace.Persistence.Configurations.ShiftsCatalogConfiguration());

        // Apply the rest from this assembly, excluding ShiftDefinitionConfiguration to avoid duplicate table mapping
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(IndTraceDbContext).Assembly,
            type => type != typeof(IndTrace.Persistence.Configurations.ShiftDefinitionConfiguration));

        // Enforce explicit EF Core model configuration - Tasks 1 & 2
        modelBuilder.EnforceModelConfiguration();
    }

    /// <summary>
    /// Creates a DbSet for lookup table entities that extend EnumModel.
    /// </summary>
    /// <typeparam name="TLookupEntity">Lookup entity type that extends EnumModel.</typeparam>
    /// <returns>A set for the given lookup entity type.</returns>
    public DbSet<TLookupEntity> LookupSet<TLookupEntity>() where TLookupEntity : class
    {
        return Set<TLookupEntity>();
    }

    /// <summary>
    /// Creates a DbSet for app-wide shared objects (configs, app status, health, etc.).
    /// </summary>
    /// <typeparam name="TState">State entity type that implements IAppState.</typeparam>
    /// <returns>A set for the given state entity type.</returns>
    public DbSet<TState> SetState<TState>() where TState : class, IAppState, new()
    {
        return Set<TState>();
    }

    /// <summary>
    /// Creates a DbSet for aggregate-specific entities that implement aggregate root pattern.
    /// </summary>
    /// <typeparam name="TAggregate">Aggregate entity type that implements IAggregateRoot.</typeparam>
    /// <returns>A set for the given aggregate entity type.</returns>
    public DbSet<TAggregate> SetAggregate<TAggregate>() where TAggregate : class, IAggregateRoot
    {
        return Set<TAggregate>();
    }

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate IndTraceDbContext logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
