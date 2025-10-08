using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the CustomerConfiguration.
/// </summary>

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Apply the base configuration
        new AuditableEntityConfiguration<Customer>().Configure(builder);

        // Configure primary key following safe refactoring pattern: {nameof(Customer) + "RegisterId"}
        builder.HasKey(e => e.CustomerId)
            .HasName("PK.IndTraceData.Customer.CustomerId");

        builder.Property(e => e.CustomerId)
            .HasColumnName(nameof(Customer.CustomerId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName(nameof(Customer.Name))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.IsActive)
            .HasColumnName(nameof(Customer.IsActive))
            .HasColumnType("bool")
            .IsRequired();

        builder.ToTable("Customers");
    }
}
