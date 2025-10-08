using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IndTrace.Monitor.Data;

/// <summary>
/// Database context for IndTrace application with Identity framework support.
/// </summary>
/// <param name="options">The database context options.</param>
public class IndTraceDbIdentity(DbContextOptions<IndTraceDbIdentity> options) : IdentityDbContext<ApplicationUser>(options)
{
}
