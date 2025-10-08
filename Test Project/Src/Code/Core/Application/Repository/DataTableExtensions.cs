// <copyright file="DataTableExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Repository;

using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

/// <summary>
/// Provides extension methods for converting collections to <see cref="DataTable"/> objects.
/// </summary>
public static class DataTableExtensions
{
    /// <summary>
    /// Converts a collection of items to a <see cref="DataTable"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="items">The collection of items to convert.</param>
    /// <returns>A <see cref="DataTable"/> representing the collection.</returns>
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - DataTable conversion may be inefficient for large collections. Consider using streaming or batching for scalability.
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate data table extension logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    public static DataTable ToDataTable<T>(this IEnumerable<T> items)
    {
        var dataTable = new DataTable(typeof(T).Name);

        var properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetCustomAttribute<NotMappedAttribute>() == null)
            .ToArray();

        foreach (var prop in properties)
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            dataTable.Columns.Add(prop.Name, type);
        }

        foreach (var item in items)
        {
            var values = properties.Select(p => p.GetValue(item) ?? DBNull.Value).ToArray();
            dataTable.Rows.Add(values);
        }

        return dataTable;
    }
}
