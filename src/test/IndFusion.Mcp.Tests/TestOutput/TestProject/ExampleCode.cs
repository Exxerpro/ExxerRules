using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestProject;

/// <summary>
/// Example code for metrics testing.
/// </summary>
public class ExampleCode
{
    private readonly string _value;
    private readonly List<string> _items;

    /// <summary>
    /// Initializes a new instance of the ExampleCode class.
    /// </summary>
    /// <param name="value">The value to store.</param>
    public ExampleCode(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
        _items = new List<string>();
    }

    /// <summary>
    /// Gets the stored value.
    /// </summary>
    public string Value => _value;

    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddItem(string item)
    {
        if (string.IsNullOrEmpty(item))
            throw new ArgumentException("Item cannot be null or empty", nameof(item));
        
        _items.Add(item);
    }

    /// <summary>
    /// Gets all items asynchronously.
    /// </summary>
    /// <returns>A task containing the items.</returns>
    public async Task<IEnumerable<string>> GetItemsAsync()
    {
        await Task.Delay(1); // Simulate async operation
        return _items.ToArray();
    }

    /// <summary>
    /// Calculates the sum of item lengths.
    /// </summary>
    /// <returns>The total length of all items.</returns>
    public int CalculateTotalLength()
    {
        var total = 0;
        foreach (var item in _items)
        {
            total += item.Length;
        }
        return total;
    }
}