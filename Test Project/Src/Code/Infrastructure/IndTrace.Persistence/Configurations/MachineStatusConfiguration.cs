using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the MachineStatusConfiguration.
/// </summary>

public class MachineStatusConfiguration : IEntityTypeConfiguration<MachineStatus>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<MachineStatus> builder)
    {
        // IMPORTANT: Non-standard primary key name
        // StatusMachineId does not follow convention of {nameof(MachineStatus) + "RegisterId"} (MachineStatusId)
        // This should be reviewed and potentially renamed to MachineStatusId
        builder.HasKey(e => e.StatusMachineId);

        builder.Property(e => e.StatusMachineId)
            .HasColumnName(nameof(MachineStatus.StatusMachineId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(MachineStatus.MachineId))
            .HasColumnType("int")
            .IsRequired();

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core validation fixes - BreakDownTime needs HasPrecision and UpdatedOn needs datetime2
        builder.Property(e => e.BreakDownTime)
            .HasColumnName(nameof(MachineStatus.BreakDownTime))
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(e => e.UpdatedOn)
            .HasColumnName(nameof(MachineStatus.UpdatedOn))
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(d => d.MachineId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.MachineStatus.MachineId");

        builder.ToTable("MachineStatus");
    }
}
