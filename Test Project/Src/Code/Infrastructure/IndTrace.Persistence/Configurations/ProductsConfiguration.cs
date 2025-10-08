using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the ProductsConfiguration.
/// </summary>

public class ProductsConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {

        // Apply the base configuration
        new AuditableEntityConfiguration<Product>().Configure(builder);

        builder.HasKey(e => e.ProductId)
            .HasName("PK.IndTraceData.Products.ProductId");

        builder.Property(e => e.ProductId)
            .HasColumnName(nameof(Product.ProductId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.PartNumber)
            .HasColumnName(nameof(Product.PartNumber))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.ProductName)
            .HasColumnName(nameof(Product.ProductName))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.IsActive)
            .HasColumnName(nameof(Product.IsActive))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.Version)
            .HasColumnName(nameof(Product.Version))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.CustomerPartNumber)
            .HasColumnName(nameof(Product.CustomerPartNumber))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.AliasPartNumber)
            .HasColumnName(nameof(Product.AliasPartNumber))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.Description)
            .HasColumnName(nameof(Product.Description))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(240);


        builder.HasIndex(e => e.ProductId)
            .HasDatabaseName("IDX.IndTraceData.Products.ProductId")
            .IsUnique();

        builder.HasIndex(e => e.CustomerId)
            .HasDatabaseName("IDX.IndTraceData.Customer.CustomerId")
            .IsUnique();

        builder.Property(e => e.CustomerName)
            .HasColumnName(nameof(Product.CustomerName))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(240);

        builder.Property(e => e.CustomerId)
            .HasColumnName(nameof(Product.CustomerId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.RuleId)
            .HasColumnName(nameof(Product.RuleId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.LineId)
            .HasColumnName(nameof(Product.LineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Ignore(e => e.Line);
        builder.Ignore(e => e.Customer);


        builder.ToTable("Products");
    }
}
