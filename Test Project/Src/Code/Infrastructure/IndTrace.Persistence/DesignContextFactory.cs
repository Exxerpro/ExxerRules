using IndTrace.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IndTrace.Persistence;
/// <summary>
/// Represents the DesignContextFactory.
/// </summary>

public class DesignContextFactory : IDesignTimeDbContextFactory<IndTraceDbContext>
{
    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate design context factory logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    /// <summary>
    /// Executes CreateDbContext operation.
    /// </summary>
    /// <param name="args">The args.</param>
    /// <returns>The result of CreateDbContext.</returns>
    public IndTraceDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<IndTraceDbContext>();

        options.UseSqlServer("Server=localhost;Database=IndTraceData;Trusted_Connection=True;Encrypt=False");

        return new IndTraceDbContext(options.Options);
    }
}
