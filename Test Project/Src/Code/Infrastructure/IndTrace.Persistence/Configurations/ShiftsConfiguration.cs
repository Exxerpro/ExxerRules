using IndTrace.Domain.Entities;
using IndTrace.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the ShiftsConfiguration.
/// </summary>

public class ShiftsConfiguration : IEntityTypeConfiguration<Shift>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        // Apply the base configuration
        new AuditableEntityConfiguration<Shift>().Configure(builder);

        builder.HasKey(e => e.ShiftId)
            .HasName("PK.IndTraceData.Shifts.ShiftID");

        builder.Property(e => e.ShiftId)
            .HasColumnName(nameof(Shift.ShiftId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.StartBy)
            .HasColumnName(nameof(Shift.StartBy))
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(e => e.EndTime)
            .HasColumnName(nameof(Shift.EndTime))
            .HasColumnType("datetime2(7)")
            .IsRequired();

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core MaxLength constraint fix - Shift.ShiftType property missing configuration
        builder.Property(e => e.ShiftType)
            .HasColumnName(nameof(Shift.ShiftType))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(50);

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core value converter fix - Convert ShiftType smart enum to int for database storage
        builder.Property(e => e.Type)
            .HasColumnName(nameof(Shift.Type))
            .HasColumnType("int")
            .IsRequired()
            .HasConversion(
                v => v != null ? v.Value : 0,  // To DB: ShiftType → int
                v => (ShiftType)v              // From DB: int → ShiftType (implicit)
            );

        builder.ToTable("Shifts");
    }
}
