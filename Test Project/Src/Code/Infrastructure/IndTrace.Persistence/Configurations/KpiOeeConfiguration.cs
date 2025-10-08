using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the KpiOeeConfiguration.
/// </summary>

public class KpiOeeConfiguration : IEntityTypeConfiguration<KpiOee>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<KpiOee> builder)
    {
        builder.ToTable("KpiOees");

        builder.HasIndex(e => e.KpiOeeId)
            .HasDatabaseName("IDX.IndTraceData.KpiOee.KpiOeeId")
            .IsUnique();

        // Configure primary key following safe refactoring pattern: {nameof(KpiOee) + "RegisterId"}
        builder.HasKey(e => e.KpiOeeId)
            .HasName("PK.IndTraceData.KpiOee.KpiOeeId");

        builder.Property(e => e.KpiOeeId)
            .HasColumnName(nameof(KpiOee.KpiOeeId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.Oee)
            .HasColumnName(nameof(KpiOee.Oee))
            .IsRequired()
            .HasColumnType("decimal(12, 4)");

        builder.Property(e => e.Availability)
            .HasColumnName(nameof(KpiOee.Availability))
            .IsRequired()
            .HasColumnType("decimal(12, 4)");

        builder.Property(e => e.Performance)
            .HasColumnName(nameof(KpiOee.Performance))
            .IsRequired()
            .HasColumnType("decimal(12, 4)");

        builder.Property(e => e.Quality)
            .HasColumnName(nameof(KpiOee.Quality))
            .IsRequired()
            .HasColumnType("decimal(12, 4)");

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core datetime2 type fix - TimeStamp uses incorrect case "DateTime2" instead of "datetime2"
        builder.Property(e => e.TimeStamp)
            .HasColumnName(nameof(KpiOee.TimeStamp))
            .IsRequired()
            .HasColumnType("datetime2");
    }
}
