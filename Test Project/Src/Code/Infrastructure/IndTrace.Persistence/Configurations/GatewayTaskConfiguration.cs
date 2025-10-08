using IndTrace.Domain.Enum.LookUpTable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the GatewayTaskConfiguration.
/// </summary>

public class GatewayTaskConfiguration : IEntityTypeConfiguration<GatewayTaskEntity>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<GatewayTaskEntity> builder)
    {
        builder.HasKey(e => e.Id)
            .HasName("PK.IndTraceData.GatewayTask.RecipeId");

        //builder.HasNoKey();

        builder.Property(e => e.Id)
            .HasColumnName(nameof(GatewayTaskEntity.Id))
            .HasColumnType("int")
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasColumnName(nameof(GatewayTaskEntity.Name))
            .HasMaxLength(80)
            .IsUnicode()
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.DisplayName)
            .HasColumnName(nameof(GatewayTaskEntity.DisplayName))
            .HasMaxLength(80)
            .IsUnicode()
            .IsRequired()
            .ValueGeneratedNever();

        builder.ToTable("GatewayTask");
    }
}
