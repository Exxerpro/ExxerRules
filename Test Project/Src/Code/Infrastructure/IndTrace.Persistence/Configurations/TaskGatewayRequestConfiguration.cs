using IndTrace.Domain.Entities;
using IndTrace.Domain.Enum;
using IndTrace.Domain.Enum.LookUpTable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;

/// <summary>
/// Represents the TaskGatewayRequestConfiguration.
/// </summary>
//[Fix]
//CLAUDE
//Date: 25/08/2025
//Reason: EF Core missing configuration fix - TaskGatewayRequest has DbSet registration but no configuration file, causing MaxLength validation failures
public class TaskGatewayRequestConfiguration : IEntityTypeConfiguration<TaskGatewayRequest>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<TaskGatewayRequest> builder)
    {
        builder.HasKey(e => e.CommandId)
            .HasName("PK_TaskGatewayRequest_CommandId");

        builder.Property(e => e.CommandId)
            .HasColumnName(nameof(TaskGatewayRequest.CommandId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(TaskGatewayRequest.MachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName(nameof(TaskGatewayRequest.Name))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(120);

        builder.Property(e => e.TimeStamp)
            .HasColumnName(nameof(TaskGatewayRequest.TimeStamp))
            .HasColumnType("datetime2");

        builder.Property(e => e.PartNumber)
            .HasColumnName(nameof(TaskGatewayRequest.PartNumber))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.Description)
            .HasColumnName(nameof(TaskGatewayRequest.Description))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(240);

        builder.Property(e => e.RequestTask)
            .HasColumnName(nameof(TaskGatewayRequest.RequestTask))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.Comment)
            .HasColumnName(nameof(TaskGatewayRequest.Comment))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(240);

        builder.Property(e => e.BarCode)
            .HasColumnName(nameof(TaskGatewayRequest.BarCode))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.EventStatus)
            .HasColumnName(nameof(TaskGatewayRequest.EventStatus))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.StatusColor)
            .HasColumnName(nameof(TaskGatewayRequest.StatusColor))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.Error)
            .HasColumnName(nameof(TaskGatewayRequest.Error))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(500);

        builder.Property(e => e.IsEnabled)
            .HasColumnName(nameof(TaskGatewayRequest.IsEnabled))
            .IsRequired();

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core value converter fix - Convert EnumModel smart enums to int for database storage
        builder.Property(e => e.CycleStatus)
            .HasColumnName(nameof(TaskGatewayRequest.CycleStatus))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: CycleStatus → int
                v => (CycleStatus)v            // From DB: int → CycleStatus (implicit)
            );

        builder.Property(e => e.PartStatus)
            .HasColumnName(nameof(TaskGatewayRequest.PartStatus))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: PartStatus → int
                v => (PartStatus)v             // From DB: int → PartStatus (implicit)
            );

        builder.Property(e => e.FlowStatus)
            .HasColumnName(nameof(TaskGatewayRequest.FlowStatus))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: FlowStatus → int
                v => (FlowStatus)v             // From DB: int → FlowStatus (implicit)
            );

        // Note: MachineType and GatewayTask properties are likely not persisted, will be ignored below

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core value converter fix - Add missing ResultValidation smart enum property
        builder.Property(e => e.ResultValidation)
            .HasColumnName(nameof(TaskGatewayRequest.ResultValidation))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: ResultValidation → int
                v => (ResultValidation)v       // From DB: int → ResultValidation (implicit)
            );

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core foreign key fix - Remove foreign key relationships for smart enums to avoid type compatibility issues
        // Note: Lookup tables exist for SQL developers/admins, but EF Core foreign keys cause type mismatch
        // Smart enums provide the business logic functionality without needing EF Core relationships

        builder.ToTable("TaskGatewayRequests");

        // Ignore properties that are not relevant for persistence
        builder.Ignore(e => e.MachineType);
        builder.Ignore(e => e.GatewayTask);
    }
}
