namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a single record from a graph query result.
/// </summary>
/// <param name="Values">The values in the record.</param>
/// <param name="Keys">The keys corresponding to the values.</param>
public readonly record struct GraphRecord(
    IReadOnlyList<object> Values,
    IReadOnlyList<string> Keys)
{
    /// <summary>
    /// Gets a value by key.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value if found, otherwise null.</returns>
    public object? GetValue(string key)
    {
        for (int i = 0; i < Keys.Count; i++)
        {
            if (Keys[i] == key)
                return i < Values.Count ? Values[i] : null;
        }
        return null;
    }

    /// <summary>
    /// Gets a typed value by key.
    /// </summary>
    /// <typeparam name="T">The expected type.</typeparam>
    /// <param name="key">The key to look up.</param>
    /// <returns>The typed value if found and convertible, otherwise default.</returns>
    public T? GetValue<T>(string key)
    {
        var value = GetValue(key);
        if (value is T typedValue)
            return typedValue;

        if (value is null)
            return default;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Checks if the record contains a specific key.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <returns>True if the key exists, otherwise false.</returns>
    public bool HasKey(string key) => Keys.Contains(key);
}