using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the DistinctRegisterConfiguration.
/// </summary>

public class DistinctRegisterConfiguration : IEntityTypeConfiguration<DistinctRegister>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<DistinctRegister> builder)
    {
        // Configure the primary key
        builder.HasKey(dr => new { dr.Name, VariableID = dr.VariableId, dr.MachineId });

        // Configure the Name column
        builder.Property(dr => dr.Name)
            .IsRequired()
            .HasMaxLength(255);

        // Configure the VariableID column
        builder.Property(dr => dr.VariableId)
            .IsRequired();

        // Configure the MachineId column
        builder.Property(dr => dr.MachineId)
            .IsRequired();

        // Set the table name explicitly (optional, EF Core will use the class name by default)
        builder.ToTable("DistinctRegisters", "dbo");
    }
}
