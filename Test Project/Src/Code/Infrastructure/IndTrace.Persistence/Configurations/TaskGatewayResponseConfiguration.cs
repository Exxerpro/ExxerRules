using IndTrace.Domain.Entities;
using IndTrace.Domain.Enum;
using IndTrace.Domain.Enum.LookUpTable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the TaskGatewayResponseConfiguration.
/// </summary>

public class TaskGatewayResponseConfiguration : IEntityTypeConfiguration<TaskGatewayResponse>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<TaskGatewayResponse> builder)
    {
        // Define the primary key
        builder.HasKey(e => e.ResponseId)
            .HasName("PK.IndTraceData.TaskGatewayResponses.ResponseId");

        // Configure the relevant properties
        builder.Property(e => e.ResponseId)
            .HasColumnName(nameof(TaskGatewayResponse.ResponseId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.CommandId)
            .HasColumnName(nameof(TaskGatewayResponse.CommandId))
            .HasColumnType("int")
            .IsRequired();

        builder.HasOne<TaskGatewayRequest>()
            .WithOne()
            .HasForeignKey<TaskGatewayResponse>(e => e.CommandId)
            .HasConstraintName("FK.IndTraceData.TaskGatewayResponses.CommandId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(TaskGatewayResponse.MachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.BarCodeId)
            .HasColumnName(nameof(TaskGatewayResponse.BarCodeId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.CycleId)
            .HasColumnName(nameof(TaskGatewayResponse.CycleId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.CyclesOk)
            .HasColumnName(nameof(TaskGatewayResponse.CyclesOk))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.ShiftId)
            .HasColumnName(nameof(TaskGatewayResponse.ShiftId))
            .HasColumnType("int")
            .IsRequired();

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core value converter fix - ResultValidation is a smart enum, should use .Value not (int)v
        builder.Property(e => e.ResultValidation)
            .HasColumnName(nameof(TaskGatewayResponse.ResultValidation))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: ResultValidation → int
                v => (ResultValidation)v       // From DB: int → ResultValidation (implicit)
            );

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core MaxLength constraint fix - Replace HasColumnType with HasMaxLength for validation compliance
        builder.Property(e => e.PartNumber)
            .HasColumnName(nameof(TaskGatewayResponse.PartNumber))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(50);

        builder.Property(e => e.Label)
            .HasColumnName(nameof(TaskGatewayResponse.Label))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(100);

        builder.Property(e => e.Error)
            .HasColumnName(nameof(TaskGatewayResponse.Error))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(240);

        builder.Property(e => e.LastMachineId)
            .HasColumnName(nameof(TaskGatewayResponse.LastMachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.NextMachineId)
            .HasColumnName(nameof(TaskGatewayResponse.NextMachineId))
            .HasColumnType("int")
            .IsRequired();

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core value converter fix - Update to proper smart enum conversion pattern using .Value
        builder.Property(e => e.CycleStatus)
            .HasColumnName(nameof(TaskGatewayResponse.CycleStatus))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: CycleStatus → int
                v => (CycleStatus)v            // From DB: int → CycleStatus (implicit)
            );

        builder.Property(e => e.FlowStatus)
            .HasColumnName(nameof(TaskGatewayResponse.FlowStatus))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: FlowStatus → int
                v => (FlowStatus)v             // From DB: int → FlowStatus (implicit)
            );

        builder.Property(e => e.PartStatus)
            .HasColumnName(nameof(TaskGatewayResponse.PartStatus))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: PartStatus → int
                v => (PartStatus)v             // From DB: int → PartStatus (implicit)
            );

        // Note: MachineType and WorkFlowType properties are ignored below, so no property configuration needed

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core datetime2 type fix - TimeStamp DateTime property missing configuration
        builder.Property(e => e.TimeStamp)
            .HasColumnName(nameof(TaskGatewayResponse.TimeStamp))
            .HasColumnType("datetime2");

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core foreign key fix - Remove foreign key relationships for smart enums to avoid type compatibility issues
        // Note: Lookup tables exist for SQL developers/admins, but EF Core foreign keys cause type mismatch
        // Smart enums provide the business logic functionality without needing EF Core relationships
        // Note: MachineType and WorkFlowType are ignored properties

        // Define the table name
        builder.ToTable("TaskGatewayResponses");

        // Ignore properties that are not relevant for persistence
        // builder.Ignore(e => e.TaskGatewayRequest);

        builder.Ignore(e => e.MachineType);
        builder.Ignore(e => e.Description);
        builder.Ignore(e => e.WorkFlowType);
        builder.Ignore(e => e.Recipe);
        builder.Ignore(e => e.Cycle);
        builder.Ignore(e => e.BarCode);
        builder.Ignore(e => e.MasterLabel);
        builder.Ignore(e => e.References);
        builder.Ignore(e => e.ExecutionTime);
        builder.Ignore(e => e.RequestTask);
        builder.Ignore(e => e.Parameters);
        builder.Ignore(e => e.Name);
        builder.Ignore(e => e.PlcId);
    }
}
