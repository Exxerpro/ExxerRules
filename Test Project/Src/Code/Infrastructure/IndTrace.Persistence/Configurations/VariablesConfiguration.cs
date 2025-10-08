using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the VariablesConfiguration.
/// </summary>

public class VariablesConfiguration : IEntityTypeConfiguration<Variable>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Variable> builder)
    {
        builder.HasKey(e => e.VariableId)
            .HasName("PK.IndTraceData.Variables.EntitieId");

        builder.Property(e => e.VariableId)
            .HasColumnName(nameof(Variable.VariableId))
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(Variable.MachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName(nameof(Variable.Name))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.Description)
            .HasColumnName(nameof(Variable.Description))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(240);

        builder.Property(e => e.Address)
            .HasColumnName(nameof(Variable.Address))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.Alias)
            .HasColumnName(nameof(Variable.Alias))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.NetType)
            .HasColumnName(nameof(Variable.NetType))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.Direction)
            .HasColumnName(nameof(Variable.Direction))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.VariableGroupId)
            .HasColumnName(nameof(Variable.VariableGroupId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.VariableSpecId)
            .HasColumnName(nameof(Variable.VariableSpecId))
            .HasColumnType("int")
            .IsRequired(false); // Nullable property in entity

        builder.Property(e => e.Length)
            .HasColumnName(nameof(Variable.Length))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.IsActive)
            .HasColumnName(nameof(Variable.IsActive))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.TagStatus)
            .HasColumnName(nameof(Variable.TagStatus))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.Value)
            .HasColumnName(nameof(Variable.Value))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.NativeType)
            .HasColumnName(nameof(Variable.NativeType))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.Property(e => e.NativeAddress)
            .HasColumnName(nameof(Variable.NativeAddress))
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(80);

        builder.HasIndex(e => e.VariableId)
            .HasDatabaseName("IDX.IndTraceData.Variables.EntitieId")
            .IsUnique();

        // Add a unique constraint on the combination of MachineId, PlcId, Name, Address, and VariableGroupId
        builder.HasIndex(e => new { e.MachineId, e.PlcId, e.Name, e.Address, e.VariableGroupId })
            .HasDatabaseName("UQ_MachinePlcNameAddressVariableGroup")
            .IsUnique();

        //properties added may 1 2025  to validate the register exist on the plc
        //Add ignore to the configurator on the database on the persistence layer
        //ABR MAY 1 2025

        //public bool? Validated { get; set; }

        // Ignore the properties that are not mapped to the database

        builder.Ignore(e => e.Validated);

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core AuditableEntity configuration fix - Variable inherits from AuditableEntity
        new AuditableEntityConfiguration<Variable>().Configure(builder);

        builder.ToTable("Variables");
    }
}
