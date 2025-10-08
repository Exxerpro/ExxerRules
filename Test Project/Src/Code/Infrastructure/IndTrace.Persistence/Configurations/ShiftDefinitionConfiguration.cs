using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;

/// <summary>
/// Represents the ShiftDefinitionConfiguration.
/// </summary>
//[Fix]
//CLAUDE
//Date: 25/08/2025
//Reason: EF Core missing configuration fix - ShiftDefinition has DbSet registration but no configuration file
public class ShiftDefinitionConfiguration : IEntityTypeConfiguration<ShiftDefinition>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<ShiftDefinition> builder)
    {
        builder.HasKey(e => e.ShiftCatalogId)
            .HasName("PK_ShiftDefinition_ShiftCatalogId");

        builder.Property(e => e.ShiftCatalogId)
            .HasColumnName(nameof(ShiftDefinition.ShiftCatalogId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.PlantId)
            .HasColumnName(nameof(ShiftDefinition.PlantId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.ShiftName)
            .HasColumnName(nameof(ShiftDefinition.ShiftName))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.StartBy)
            .HasColumnName(nameof(ShiftDefinition.StartBy));

        builder.Property(e => e.Duration)
            .HasColumnName(nameof(ShiftDefinition.Duration));

        builder.Property(e => e.EndTime)
            .HasColumnName(nameof(ShiftDefinition.EndTime));

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core AuditableEntity configuration fix - ShiftDefinition inherits from AuditableEntity
        new AuditableEntityConfiguration<ShiftDefinition>().Configure(builder);

        builder.ToTable("ShiftsCatalog");
    }
}
