using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;

internal class MachinePlcsConfiguration : IEntityTypeConfiguration<MachinePlc>

{
    public void Configure(EntityTypeBuilder<MachinePlc> builder)
    {

        // Apply the base configuration
        new AuditableEntityConfiguration<MachinePlc>().Configure(builder);

        builder.HasKey(t => new { MachineId = t.MachineId, PlcId = t.PlcId });

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(pt => pt.MachineId);

        builder.HasOne<Plc>()
            .WithMany()
            .HasForeignKey(pt => pt.PlcId);

        builder.ToTable("MachinePlcs");
    }
}
