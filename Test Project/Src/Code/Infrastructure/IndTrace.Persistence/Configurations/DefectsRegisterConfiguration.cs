using IndTrace.Domain.Entities;
using IndTrace.Domain.Entities.BarCodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the DefectsRegisterConfiguration.
/// </summary>

public class DefectsRegisterConfiguration : IEntityTypeConfiguration<DefectRegister>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<DefectRegister> builder)
    {
        // Configure primary key following safe refactoring pattern: {nameof(DefectRegister) + "RegisterId"}
        builder.HasKey(e => e.DefectRegisterId)
            .HasName("PK.IndTraceData.DefectsRegister.DefectRegisterId");

        builder.Property(e => e.DefectRegisterId)
            .HasColumnName(nameof(DefectRegister.DefectRegisterId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.BarCodeId)
            .HasColumnName(nameof(DefectRegister.BarCodeId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.Comment)
            .HasColumnName(nameof(DefectRegister.Comment))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.DefectId)
            .HasColumnName(nameof(DefectRegister.DefectId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName(nameof(DefectRegister.Description))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(240);

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(DefectRegister.MachineId))
            .HasColumnType("int")
            .IsRequired();

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core Precision/Scale fix - Use HasPrecision instead of HasColumnType for better validation
        builder.Property(e => e.PartsQuantity)
            .HasColumnName(nameof(DefectRegister.PartsQuantity))
            .HasPrecision(18, 4);

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core byte[] configuration fix - TimeStamp needs explicit column type for rowversion
        builder.Property(e => e.TimeStamp)
            .HasColumnName(nameof(DefectRegister.TimeStamp))
            .IsRequired()
            .IsRowVersion()
            .IsConcurrencyToken()
            .HasColumnType("rowversion");

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core datetime2 type fix - DefectRegister has CreatedOn/ModifiedOn DateTime properties missing configuration
        builder.Property(e => e.CreatedOn)
            .HasColumnName(nameof(DefectRegister.CreatedOn))
            .HasColumnType("datetime2");

        builder.Property(e => e.ModifiedOn)
            .HasColumnName(nameof(DefectRegister.ModifiedOn))
            .HasColumnType("datetime2");

        builder.HasOne<BarCode>()
            .WithMany()
            .HasForeignKey(d => d.BarCodeId)
            .HasConstraintName("FK.IndTraceData.DefectsRegister.BarCodeId");

        builder.HasOne<Defect>()
            .WithMany()
            .HasForeignKey(d => d.DefectId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.DefectsRegister.DefectId");

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(d => d.MachineId)
            .HasConstraintName("FK.IndTraceData.DefectsRegister.MachineId");

        builder.ToTable("DefectsRegister");
    }
}
