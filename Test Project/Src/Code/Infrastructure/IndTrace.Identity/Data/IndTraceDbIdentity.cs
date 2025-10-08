// <copyright file="IndTraceDbIdentity.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Identity.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents the Entity Framework database context for application identity management.
/// </summary>
/// <param name="options">The options to be used by the DbContext.</param>
public class IndTraceDbIdentity(DbContextOptions<IndTraceDbIdentity> options) : IdentityDbContext<ApplicationUser>(options)
{
}
