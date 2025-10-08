using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;

internal class EdgeConfiguration : IEntityTypeConfiguration<Edge>
{
    public void Configure(EntityTypeBuilder<Edge> builder)
    {
        // Configure primary key following safe refactoring pattern: {nameof(Edge) + "RegisterId"}
        builder.HasKey(e => e.EdgeId)
            .HasName("PK.IndTraceData.Edges.EdgeId");

        builder.Property(e => e.EdgeId)
            .HasColumnName(nameof(Edge.EdgeId))
            .HasColumnType("int")
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(e => e.Weight)
            .HasColumnName(nameof(Edge.Weight))
            .HasColumnType("int")
            .IsRequired()
            .HasDefaultValue(1);

        builder.HasOne(e => e.FromMachine)
            .WithMany(m => m.FromEdges)
            .HasForeignKey(e => e.FromMachineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ToMachine)
            .WithMany(m => m.ToEdges)
            .HasForeignKey(e => e.ToMachineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.EdgeId)
            .HasDatabaseName("IDX.IndTraceData.Edges.EdgeId")
            .IsUnique();

        builder.ToTable("Edges");
    }
}
