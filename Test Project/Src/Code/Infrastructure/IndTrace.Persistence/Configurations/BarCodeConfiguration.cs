using IndTrace.Domain.Entities;
using IndTrace.Domain.Entities.BarCodes;
using IndTrace.Domain.Enum;
using IndTrace.Domain.Enum.LookUpTable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the BarCodeConfiguration.
/// </summary>

public class BarCodeConfiguration : IEntityTypeConfiguration<BarCode>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<BarCode> builder)
    {
        // Configure primary key following safe refactoring pattern: {nameof(BarCode) + "RegisterId"}
        builder.HasKey(e => e.BarCodeId)
            .HasName("PK.IndTraceData.BarCodes.BarCodeId");

        builder.Property(e => e.BarCodeId)
            .HasColumnName(nameof(BarCode.BarCodeId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.ProductId)
            .HasColumnName(nameof(BarCode.ProductId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(BarCode.MachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.Label)
            .HasColumnName(nameof(BarCode.Label))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core value converter fix - Convert EnumModel smart enums to int for database storage
        builder.Property(e => e.PartStatus)
            .HasColumnName(nameof(BarCode.PartStatus))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: PartStatus → int
                v => (PartStatus)v             // From DB: int → PartStatus (implicit)
            );

        builder.Property(e => e.FlowStatus)
            .HasColumnName(nameof(BarCode.FlowStatus))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: FlowStatus → int
                v => (FlowStatus)v             // From DB: int → FlowStatus (implicit)
            );

        builder.Property(e => e.CreatedOn)
            .HasColumnName(nameof(BarCode.CreatedOn))
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(e => e.ModifiedOn)
            .HasColumnName(nameof(BarCode.ModifiedOn))
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(e => e.MachineId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.BarCodes.Machines");

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.BarCodes.Products");

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core foreign key fix - Remove foreign key relationships for smart enums to avoid type compatibility issues
        // Note: Lookup tables exist for SQL developers/admins, but EF Core foreign keys cause type mismatch
        // Smart enums provide the business logic functionality without needing EF Core relationships

        builder.HasIndex(e => e.BarCodeId)
            .HasDatabaseName("IDX.IndTraceData.BarCodes.BarCodeId")
            .IsUnique();

        builder.HasIndex(e => e.Label)
            .HasDatabaseName("IDX.IndTraceData.BarCodes.Label")
            .IsUnique();

        builder.HasIndex(r => r.CreatedOn)
            .HasDatabaseName("IDX.IndTraceData.BarCodes.CreatedOn");

        builder.ToTable("BarCodes");
    }
}
