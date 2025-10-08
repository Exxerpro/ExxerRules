using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the LineConfiguration.
/// </summary>

public class LineConfiguration : IEntityTypeConfiguration<Line>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Line> builder)
    {
        // Apply the base configuration
        new AuditableEntityConfiguration<Line>().Configure(builder);

        // Configure primary key following safe refactoring pattern: {nameof(Line) + "RegisterId"}
        builder.HasKey(e => e.LineId)
            .HasName("PK.IndTraceData.Line.LineId");

        builder.Property(e => e.LineId)
            .HasColumnName(nameof(Line.LineId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName(nameof(Line.Name))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.Description)
            .HasColumnName(nameof(Line.Description))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(240);

        builder.Property(e => e.Status)
            .HasColumnName(nameof(Line.Status))
            .HasColumnType("int")
            .IsRequired();

        builder.Ignore(l => l.Products);
        builder.Ignore(l => l.Machines);

        builder.ToTable("Lines");
    }
}
