using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the ToolingConfiguration.
/// </summary>

public class ToolingConfiguration : IEntityTypeConfiguration<Tooling>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Tooling> builder)
    {
        builder.HasKey(e => e.ToolId)
            .HasName("PK.IndTraceData.Toolings.ToolId");

        builder.Property(e => e.ToolId)
            .HasColumnName(nameof(Tooling.ToolId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName(nameof(Tooling.Name))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.ToTable("Toolings");



    }
}
