using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the StatusConfigurationsConfiguration.
/// </summary>

public class StatusConfigurationsConfiguration : IEntityTypeConfiguration<StatusConfiguration>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<StatusConfiguration> builder)
    {
        // TODO: CRITICAL - Primary key design needed
        // StatusConfiguration entity lacks primary key property (StatusConfigurationId)
        // Current design suggests composite key (MachineId, Status) or synthetic key needed
        // This violates {nameof(StatusConfiguration)}RegisterId convention
        builder.HasNoKey();

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core datetime2 type fix - ModifiedOn uses incorrect case "Datetime2" instead of "datetime2"
        builder.Property(e => e.ModifiedOn)
            .HasColumnName(nameof(StatusConfiguration.ModifiedOn))
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(StatusConfiguration.MachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.Message)
            .HasColumnName(nameof(StatusConfiguration.Message))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(d => d.MachineId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.StatusConfigurations.MachineId");

        builder.ToTable("StatusConfigurations");
    }
}
