using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the SettingsConfiguration.
/// </summary>

public class SettingsConfiguration : IEntityTypeConfiguration<Setting>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.HasKey(e => e.SettingId)
            .HasName("PK.IndTraceData.Settings.SettingId");

        builder.Property(e => e.SettingId)
            .HasColumnName(nameof(Setting.SettingId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(Setting.MachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.Config)
            .HasColumnName(nameof(Setting.Config))
            .IsRequired()
            .HasMaxLength(4000)
            .IsFixedLength();

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(d => d.MachineId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.Settings.Machines");

        builder.HasIndex(e => e.SettingId)
            .HasDatabaseName("IDX.IndTraceData.Settings.SettingId")
            .IsUnique();

        builder.ToTable("Settings");
    }
}
