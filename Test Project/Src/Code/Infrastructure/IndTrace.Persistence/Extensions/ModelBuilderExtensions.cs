using IndTrace.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IndTrace.Persistence.Extensions;

/// <summary>
/// Provides extension methods for configuring Entity Framework model builders with enumerations and other utilities.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies enumeration configuration to the model builder for a given entity and enumeration type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TEnumeration">The enumeration type.</typeparam>
    /// <param name="modelBuilder">The model builder.</param>
    public static void ApplyEnumerationConfiguration<TEntity, TEnumeration>(this ModelBuilder modelBuilder)
        where TEntity : class, IEnumModel, new()
        where TEnumeration : EnumModel, new()
    {
        var enumValues = EnumModel.GetAll<TEnumeration>().ToList();

        modelBuilder.Entity<TEntity>().HasData(
            enumValues.Select(e =>
            {
                var entity = new TEntity
                {
                    Value = e.Value,
                    Name = e.Name,
                    DisplayName = e.DisplayName ?? e.Name,
                };
                return entity;
            }).ToArray()
        );
    }

    public static IEnumerable<TEnumeration> GetAll<TEnumeration>()
        where TEnumeration : EnumModel, new()
    {
        var type = typeof(TEnumeration);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        foreach (var fieldInfo in fields)
        {
            if (fieldInfo.GetValue(null) is TEnumeration instance)
            {
                yield return instance;
            }
        }
    }
}
