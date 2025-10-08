using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the RulesConfiguration.
/// </summary>

public class RulesConfiguration : IEntityTypeConfiguration<Rule>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Rule> builder)
    {
        // Apply the base configuration
        new AuditableEntityConfiguration<Rule>().Configure(builder);

        builder.HasKey(e => e.RuleId)
            .HasName("PK_Rules");

        builder.Property(e => e.RuleId)
            .HasColumnName(nameof(Rule.RuleId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.RuleJson)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(8000);

        builder.Property(e => e.Name)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.Description)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(120);

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(Rule.MachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(r => r.MachineId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.Rules.Machines");

        builder.Property(e => e.ProductId)
            .HasColumnName(nameof(Rule.ProductId))
            .HasColumnType("int")
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.Rules.Products");

        builder.Property(e => e.Version)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired();
        // Ignore complex properties that are not mapped to the database
        builder.Ignore(r => r.Components);
        builder.Ignore(r => r.RuleFunction);

        builder.ToTable("Rules");
    }
}
