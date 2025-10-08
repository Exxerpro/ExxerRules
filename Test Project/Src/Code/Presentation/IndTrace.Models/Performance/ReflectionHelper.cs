// <copyright file="ReflectionHelper.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.UI.Models.Performance;

using System.Reflection;
using IndTrace.UI.Models.Products;
using MudBlazor;

/// <summary>
/// Provides reflection helper methods for accessing private fields in MudBlazor components.
/// </summary>
public static class ReflectionHelper
{
    // MudDropContainer<ProductMachineItem>

    /// <summary>
    /// Gets the drop zones dictionary from a MudDropContainer using reflection.
    /// </summary>
    /// <typeparam name="T">The type of items in the drop zones.</typeparam>
    /// <param name="container">The MudDropContainer instance.</param>
    /// <returns>A dictionary of drop zones keyed by their identifiers.</returns>
    /// <exception cref="ArgumentException">Thrown when the private field is not found.</exception>
    /// <exception cref="InvalidCastException">Thrown when the field is not of the expected type.</exception>
    public static Dictionary<string, MudDropZone<T>> GetDropZones<T>(this MudDropContainer<ProductMachineItem> container)
        where T : notnull
    {
        var fieldName = "_mudDropZones";

        // Get the type of the container object
        Type containerType = container.GetType();

        // Get the FieldInfo for the private field
        FieldInfo? fieldInfo = containerType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

        // Check if the field was found
        if (fieldInfo == null)
        {
            throw new ArgumentException($"Field '{fieldName}' not found in type '{containerType.FullName}'.");
        }

        // Get the value of the field
        var fieldValue = fieldInfo!.GetValue(container);

        // Cast the field value to the expected type
        if (fieldValue is Dictionary<string, MudDropZone<T>> dictionary)
        {
            return dictionary;
        }
        else
        {
            throw new InvalidCastException($"Field '{fieldName}' is not of the expected type 'Dictionary<string, MudDropZone<{typeof(T).Name}>>'.");
        }
    }

    /// <summary>
    /// Gets the indices dictionary from a MudDropZone using reflection.
    /// </summary>
    /// <typeparam name="T">The type of items in the drop zone.</typeparam>
    /// <param name="dropZone">The MudDropZone instance.</param>
    /// <returns>A dictionary mapping items to their indices.</returns>
    /// <exception cref="ArgumentException">Thrown when the private field is not found.</exception>
    /// <exception cref="InvalidCastException">Thrown when the field is not of the expected type.</exception>
    public static Dictionary<T, int> GetIndices<T>(this MudDropZone<T> dropZone)
        where T : notnull
    {
        // Get the type of the MudDropZone instance
        Type dropZoneType = dropZone.GetType();

        // Get the FieldInfo for the private _indices field
        FieldInfo? fieldInfo = dropZoneType.GetField("_indices", BindingFlags.NonPublic | BindingFlags.Instance);

        // Check if the field was found
        if (fieldInfo == null)
        {
            throw new ArgumentException($"Field '_indices' not found in type '{dropZoneType.FullName}'.");
        }

        // Get the value of the field
        var fieldValue = fieldInfo!.GetValue(dropZone);

        // Cast the field value to the expected type
        if (fieldValue is Dictionary<T, int> dictionary)
        {
            return dictionary;
        }
        else
        {
            throw new InvalidCastException($"Field '_indices' is not of the expected type 'Dictionary<{typeof(T).Name}, int>'.");
        }
    }
}

// class
