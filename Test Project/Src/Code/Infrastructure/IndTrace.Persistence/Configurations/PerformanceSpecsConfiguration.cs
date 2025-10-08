using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndTrace.Persistence.Configurations;
/// <summary>
/// Represents the PerformanceSpecsConfiguration.
/// </summary>

public class PerformanceSpecsConfiguration : IEntityTypeConfiguration<PerformanceSpec>
{
    /// <summary>
    /// Executes Configure operation.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public void Configure(EntityTypeBuilder<PerformanceSpec> builder)
    {
        // IMPORTANT: Non-standard primary key name
        // RegisterId does not follow convention of {nameof(PerformanceSpec) + "RegisterId"} (PerformanceSpecId)
        // This should be reviewed and potentially renamed to PerformanceSpecId
        builder.HasKey(e => e.Id);

        builder.ToTable("PerformanceSpecs");
    }
}
