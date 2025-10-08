using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the PlcConfiguration.
/// </summary>

public class PlcConfiguration : IEntityTypeConfiguration<Plc>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Plc> builder)
    {
        builder.HasKey(e => e.PlcId)
            .HasName("PK.IndTraceData.Plcs.PlcId");

        builder.Property(e => e.PlcId)
            .HasColumnName(nameof(Plc.PlcId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName(nameof(Plc.Name))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.PlcBrand)
            .HasColumnName(nameof(Plc.PlcBrand))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.IpAddress)
            .HasColumnName(nameof(Plc.IpAddress))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.PlcType)
            .HasColumnName(nameof(Plc.PlcType))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.Options)
            .HasColumnName(nameof(Plc.Options))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(240);

        builder.Property(e => e.CommLibrary)
            .HasColumnName(nameof(Plc.CommLibrary))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.BrandOwner)
            .HasColumnName(nameof(Plc.BrandOwner))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);


        builder.Property(e => e.Enabled)
            .HasColumnName(nameof(Plc.Enabled))
            .HasColumnType("int")
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(e => e.PlcId)
            .HasDatabaseName("IDX.IndTraceData.Plcs.PlcId")
            .IsUnique();

        builder.ToTable("Plcs");
    }
}
