using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the PerformanceDatasConfiguration.
/// </summary>

public class PerformanceDatasConfiguration : IEntityTypeConfiguration<PerformanceData>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<PerformanceData> builder)
    {
        builder.ToTable("PerformanceDatas");

        builder.HasKey(e => e.PerformanceDataId)
            .HasName("PK.IndTraceData.PerformanceDatas.PerformanceDataId");

        builder.HasIndex(e => e.TimeStamp)
            .HasDatabaseName("IX_PerformanceDatas_TimeStamp");

        builder.HasIndex(e => new { e.MachineId, e.PlcId })
            .HasDatabaseName("IX_PerformanceDatas_MachineId_PlcId");

        builder.Property(e => e.PerformanceDataId)
            .HasColumnName(nameof(PerformanceData.PerformanceDataId))
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(PerformanceData.MachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.PlcId)
            .HasColumnName(nameof(PerformanceData.PlcId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.BarCodeId)
            .HasColumnName(nameof(PerformanceData.BarCodeId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.CycleId)
            .HasColumnName(nameof(PerformanceData.CycleId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.TimeStamp)
            .HasColumnName(nameof(PerformanceData.TimeStamp))
            .HasColumnType("datetime2(7)")
            .IsRequired();

        // Integers grouped
        builder.Property(e => e.ApplicationFlag)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.EventCounter)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.CurrentTime)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.RunningTime)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.StoppedTime)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.FaultedTime)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.StatusFaultReason)
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.StatusFaultReject)
            .HasColumnType("int")
            .IsRequired();

        // Float values with explicit precision
        builder.Property(e => e.TotalProduction)
            .HasColumnType("float")
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(e => e.ProductionOk)
            .HasColumnType("float")
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Property(e => e.ProductionNoK)
            .HasColumnType("float")
            .HasPrecision(18, 6)
            .IsRequired();

        builder.Ignore(e => e.Command);

        builder.HasOne<Cycle>()
            .WithMany()
            .HasForeignKey(d => d.CycleId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.PerformanceDatas.Cycles");
    }
}
