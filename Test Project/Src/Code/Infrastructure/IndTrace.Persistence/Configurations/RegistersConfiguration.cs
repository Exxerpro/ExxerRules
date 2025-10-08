using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the RegistersConfiguration.
/// </summary>

public class RegistersConfiguration : IEntityTypeConfiguration<Register>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Register> builder)
    {
        builder.HasKey(e => e.RegisterId)
            .HasName("PK.IndTraceData.Registers.RegisterId");

        builder.HasIndex(r => r.VariableId)
            .HasDatabaseName("IX_Registers_VariableID");

        builder.HasIndex(r => new { r.Name, r.MachineId })
            .HasDatabaseName("IX_Registers_Name_MachineId");

        builder.HasIndex(r => r.TimeStamp)
            .HasDatabaseName("IX_Registers_TimeStamp");

        builder.Property(e => e.RegisterId)
            .HasColumnName(nameof(Register.RegisterId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.CycleId)
            .HasColumnName(nameof(Register.CycleId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.Value)
            .HasColumnName(nameof(Register.Value))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.Name)
            .HasColumnName(nameof(Register.Name))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.StatusValueId)
            .HasColumnName(nameof(Register.StatusValueId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.VariableId)
            .HasColumnName(nameof(Register.VariableId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.DataType)
            .HasColumnName(nameof(Register.DataType))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core MaxLength constraint fix - Register.Description property missing configuration
        builder.Property(e => e.Description)
            .HasColumnName(nameof(Register.Description))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(240);

        builder.Property(e => e.TimeStamp)
            .HasColumnName(nameof(Register.TimeStamp))
            .IsRequired()
            .IsUnicode()
            .HasColumnType("datetime2(7)")
            .HasMaxLength(80);

        builder.HasOne<Variable>()
            .WithMany()
            .HasForeignKey(v => v.VariableId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.Registers.Variables");

        builder.HasOne<Cycle>()
            .WithMany()
            .HasForeignKey(d => d.CycleId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.Registers.Cycles");

        builder.HasIndex(e => e.RegisterId)
            .HasDatabaseName("IDX.IndTraceData.Registers.RegisterId")
            .IsUnique();

        builder.ToTable("Registers");
    }
}
