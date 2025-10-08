using IndTrace.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the AuditableEntityConfiguration.
/// </summary>

public class AuditableEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : AuditableEntity
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.CreatedBy)
            .HasColumnName("CreatedBy")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.ModifiedBy)
            .HasColumnName("ModifiedBy")
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.CreatedOn)
            .HasColumnName("CreatedOn")
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(e => e.ModifiedOn)
            .HasColumnName("ModifiedOn")
            .HasColumnType("datetime2(7)");
    }
}
