// <copyright file="ProductMachineItemExtensions.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.UI.Models.Products;

/// <summary>
/// Provides extension methods for working with collections of ProductMachineItem objects.
/// </summary>
public static class ProductMachineItemExtensions
{
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate ProductMachineItemExtensions logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.

    /// <summary>
    /// Removes duplicate ProductMachineItem objects from the collection based on Name, MachineId, and Status.
    /// </summary>
    /// <param name="source">The collection of ProductMachineItem objects to filter.</param>
    /// <returns>A filtered collection with duplicate items removed.</returns>
    public static IEnumerable<ProductMachineItem> RemoveDuplicates(this IEnumerable<ProductMachineItem> source)
    {
        return source
            .GroupBy(item => new { item.Name, item.MachineId, item.Status })
            .Select(group => group.First());
    }

    /// <summary>
    /// Inserts a ProductMachineItem at the specified index if it doesn't already exist in the collection.
    /// </summary>
    /// <param name="source">The collection to insert into.</param>
    /// <param name="index">The zero-based index at which to insert the item.</param>
    /// <param name="item">The ProductMachineItem to insert.</param>
    public static void InsertIfNotExist(this IList<ProductMachineItem> source, int index, ProductMachineItem item)
    {
        bool exists = source.Any(existingItem =>
            existingItem.Name == item.Name &&
            existingItem.MachineId == item.MachineId &&
            existingItem.Status == item.Status);

        if (!exists)
        {
            source.Insert(index, item);
        }
    }

    /// <summary>
    /// Adds a ProductMachineItem to the collection if it doesn't already exist.
    /// </summary>
    /// <param name="source">The collection to add to.</param>
    /// <param name="item">The ProductMachineItem to add.</param>
    public static void AddIfNotExist(this IList<ProductMachineItem> source, ProductMachineItem item)
    {
        bool exists = source.Any(existingItem =>
            existingItem.Name == item.Name &&
            existingItem.MachineId == item.MachineId &&
            existingItem.Status == item.Status);

        if (!exists)
        {
            source.Add(item);
        }
    }
}
