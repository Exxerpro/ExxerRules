using IndTrace.Domain.Enum.LookUpTable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;

/// <summary>
/// Configures the WorkFlowTypeEntity for Entity Framework, including table and property mappings.
/// </summary>
public class FlowTypeConfiguration : IEntityTypeConfiguration<WorkFlowTypeEntity>
{
    /// <summary>
    /// Configures the WorkFlowTypeEntity type.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<WorkFlowTypeEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName(nameof(WorkFlowTypeEntity.Id))
            .HasColumnType("int")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasColumnName(nameof(WorkFlowTypeEntity.Name))
            .HasMaxLength(80)
            .IsUnicode()
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.DisplayName)
            .HasColumnName(nameof(WorkFlowTypeEntity.DisplayName))
            .HasMaxLength(80)
            .IsUnicode()
            .IsRequired()
            .ValueGeneratedNever();

        builder.ToTable("WorkFlowType");
    }
}
