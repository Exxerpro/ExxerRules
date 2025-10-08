using IndTrace.Domain.Entities;
using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Domain.Enum;
using IndTrace.Domain.Enum.LookUpTable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the CyclesConfiguration.
/// </summary>

public class CyclesConfiguration : IEntityTypeConfiguration<Cycle>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Cycle> builder)
    {
        // Configure primary key following safe refactoring pattern: {nameof(Cycle) + "RegisterId"}
        builder.HasKey(e => e.CycleId)
            .HasName("PK.IndTraceData.Cycles.CycleId");

        builder.Property(e => e.CycleId)
            .HasColumnName(nameof(Cycle.CycleId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(Cycle.MachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.BarCodeId)
            .HasColumnName(nameof(Cycle.BarCodeId))
            .HasColumnType("int")
            .IsRequired();

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core value converter fix - Convert EnumModel smart enums to int for database storage
        builder.Property(e => e.CycleStatus)
            .HasColumnName(nameof(Cycle.CycleStatus))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: CycleStatus → int
                v => (CycleStatus)v            // From DB: int → CycleStatus (implicit)
            );

        builder.Property(e => e.PartStatus)
            .HasColumnName(nameof(Cycle.PartStatus))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: PartStatus → int
                v => (PartStatus)v             // From DB: int → PartStatus (implicit)
            );

        builder.Property(e => e.CycleTime)
            .HasColumnName(nameof(Cycle.CycleTime))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.TaktTime)
            .HasColumnName(nameof(Cycle.TaktTime))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.StartedOn)
            .HasColumnName(nameof(Cycle.StartedOn))
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(e => e.FinishedOn)
            .HasColumnName(nameof(Cycle.FinishedOn))
            .HasColumnType("datetime2(7)");

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(c => c.MachineId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.Cycles.Machines");

        builder.HasOne<BarCode>()
            .WithMany()
            .HasForeignKey(c => c.BarCodeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.Cycles.BarCodes");

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core foreign key fix - Remove foreign key relationships for smart enums to avoid type compatibility issues
        // Note: Lookup tables exist for SQL developers/admins, but EF Core foreign keys cause type mismatch
        // Smart enums provide the business logic functionality without needing EF Core relationships

        builder.HasIndex(e => e.CycleId)
            .HasDatabaseName("IDX.IndTraceData.Cycles.CycleId")
            .IsUnique();

        builder.HasIndex(e => e.BarCodeId)
            .HasDatabaseName("IDX.IndTraceData.Cycles.BarCodeId");

        builder.ToTable("Cycles");
    }
}
