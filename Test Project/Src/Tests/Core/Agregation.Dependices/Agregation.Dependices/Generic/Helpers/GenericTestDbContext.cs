namespace IndTrace.Agregation.Dependices.Generic.Helpers;

/// <summary>
/// Represents the GenericTestDbContext.
/// </summary>
public class GenericTestDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the TestEntities.
    /// </summary>
    public DbSet<GenericTestEntity> TestEntities { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase($"GenericTestDb_{Guid.NewGuid()}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GenericTestEntity>().HasKey(e => e.Id);
        base.OnModelCreating(modelBuilder);
    }
}
