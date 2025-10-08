using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the WorkFlowConfiguration.
/// </summary>

public class WorkFlowConfiguration : IEntityTypeConfiguration<WorkFlow>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<WorkFlow> builder)
    {
        // Apply the base configuration
        new AuditableEntityConfiguration<WorkFlow>().Configure(builder);


        builder.HasKey(e => e.WorkFlowId)
            .HasName("PK.IndTraceData.WorkFlows.WorkFlowId");

        builder.Property(e => e.WorkFlowId)
            .HasColumnName(nameof(WorkFlow.WorkFlowId))
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .UseIdentityColumn(1, 1)
            .IsRequired();

        builder.Property(e => e.ProductId)
            .HasColumnName(nameof(WorkFlow.ProductId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.NextMachineId)
            .HasColumnName(nameof(WorkFlow.NextMachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.LastMachineId)
            .HasColumnName(nameof(WorkFlow.LastMachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(d => d.NextMachineId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.WorkFlows.Machines.NextMachineId");

        builder.HasOne<Machine>()
            .WithMany()
            .HasForeignKey(d => d.LastMachineId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK.IndTraceData.WorkFlows.Machines.LastMachineId");

        //builder.HasOne<Rule>()
        //    .WithOne()
        //    .HasForeignKey<Rule>(r => r.RuleId)
        //    .OnDelete(DeleteBehavior.Restrict)
        //    .HasConstraintName("FK.IndTraceData.WorkFlows.Rules.RuleId");

        builder.Property(e => e.RuleId)
            .HasColumnName(nameof(WorkFlow.RuleId))
            .HasColumnType("int")
            .IsRequired();


        builder.ToTable("WorkFlows");
    }
}
