using IndTrace.Domain.Enum.LookUpTable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;

/// <summary>
/// Configures the MachineTypeEntity for Entity Framework, including table and property mappings.
/// </summary>
public class MachineTypeConfiguration : IEntityTypeConfiguration<MachineTypeEntity>
{
    /// <summary>
    /// Configures the MachineTypeEntity type.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<MachineTypeEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasColumnName(nameof(MachineTypeEntity.Name))
            .HasMaxLength(80)
            .IsUnicode()
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.DisplayName)
            .HasColumnName(nameof(MachineTypeEntity.DisplayName))
            .HasMaxLength(80)
            .IsUnicode()
            .IsRequired()
            .ValueGeneratedNever();

        builder.ToTable("MachineType");
    }
}
