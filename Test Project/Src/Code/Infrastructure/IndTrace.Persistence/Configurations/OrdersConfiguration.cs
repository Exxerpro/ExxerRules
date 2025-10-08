using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the OrdersConfiguration.
/// </summary>

public class OrdersConfiguration : IEntityTypeConfiguration<Order>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Configure primary key following safe refactoring pattern: {nameof(Order) + "RegisterId"}
        builder.HasKey(e => e.OrderId);

        builder.Property(e => e.LeaderId)
            .HasColumnName(nameof(Order.LeaderId));

        builder.Property(e => e.MachineId)
            .HasColumnName(nameof(Order.MachineId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.ToolingId)
            .HasColumnName(nameof(Order.ToolingId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.OperatorId)
            .HasColumnName(nameof(Order.OperatorId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.OrderId)
            .HasColumnName(nameof(Order.OrderId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.ProductId)
            .HasColumnName(nameof(Order.ProductId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.ProgrammerId)
            .HasColumnName(nameof(Order.ProgrammerId))
            .HasColumnType("int")
            .IsRequired();

        builder.Property(e => e.ResultsId)
            .HasColumnName(nameof(Order.ResultsId))
            .HasColumnType("int")
            .IsRequired();

        builder.HasIndex(e => e.OrderId)
            .HasDatabaseName("IDX.IndTraceData.Orders.OrderId")
            .IsUnique();

        builder.Property(e => e.LeaderId).HasColumnName(nameof(Order.LeaderId));

        builder.Property(e => e.MachineId).HasColumnName(nameof(Order.MachineId));

        builder.Property(e => e.ToolingId).HasColumnName(nameof(Order.ToolingId));

        builder.Property(e => e.OperatorId).HasColumnName(nameof(Order.OperatorId));

        builder.Property(e => e.OrderId).HasColumnName(nameof(Order.OrderId));

        builder.Property(e => e.ProductId).HasColumnName(nameof(Order.ProductId));

        builder.Property(e => e.ProgrammerId).HasColumnName(nameof(Order.ProgrammerId));

        builder.Property(e => e.ResultsId).HasColumnName(nameof(Order.ResultsId));

        //[Fix]
        //CLAUDE
        //Date: 25/08/2025
        //Reason: EF Core datetime2 type fix - OrderStart DateTime property missing configuration
        builder.Property(e => e.OrderStart)
            .HasColumnName(nameof(Order.OrderStart))
            .HasColumnType("datetime2");

        builder.Property(e => e.OrderEnd)
            .HasColumnName(nameof(Order.OrderEnd))
            .HasColumnType("datetime2");

        builder.ToTable("Orders");
    }
}
