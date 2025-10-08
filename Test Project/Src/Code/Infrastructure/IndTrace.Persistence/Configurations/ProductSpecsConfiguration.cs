using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the ProductSpecsConfiguration.
/// </summary>

public class ProductSpecsConfiguration : IEntityTypeConfiguration<ProductSpec>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<ProductSpec> builder)
    {
        // Configure primary key following safe refactoring pattern
        builder.HasKey(e => e.ProductSpecId);

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(ProductSpec.MachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.ToolId)
            .HasColumnName(nameof(ProductSpec.ToolId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.PerformanceSpecId)
            .HasColumnName(nameof(ProductSpec.PerformanceSpecId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.PerformanceSpecsName)
            .HasColumnName(nameof(ProductSpec.PerformanceSpecsName))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.ProductSpecId)
            .HasColumnName(nameof(ProductSpec.ProductSpecId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.ProductId)
            .HasColumnName(nameof(ProductSpec.ProductId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.RecipeId)
            .HasColumnName(nameof(ProductSpec.RecipeId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.RecipeType)
            .HasColumnName(nameof(ProductSpec.RecipeType))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.HasOne(d => d.Machine)
            .WithMany()
            .HasForeignKey(d => d.MachineId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.ProductSpecs.MachineId");

        builder.HasOne(d => d.Tooling)
            .WithMany()
            .HasForeignKey(d => d.ToolId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.ProductSpecs.ToolId");

        builder.HasOne(d => d.Product)
            .WithMany()
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.ProductSpecs.ProductId");

        builder.HasOne(d => d.Recipe)
            .WithMany()
            .HasForeignKey(d => d.RecipeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.ProductSpecs.RecipeId");

        builder.ToTable("ProductSpecs");
    }
}
