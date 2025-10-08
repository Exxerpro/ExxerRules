// <copyright file="VariableS7.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Data;

/// <summary>
/// Represents a variable in the S7 PLC system, including its value and type information.
/// </summary>
public class VariableS7
{
    /// <summary>
    /// Gets or sets the machine identifier associated with this variable.
    /// </summary>
    public int MachineId { get; set; }

    /// <summary>
    /// Gets or sets the name of the variable.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the address of the variable in the PLC.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Gets or sets the network type of the variable.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string NetType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the integer value of the variable.
    /// </summary>
    public int ValueInt { get; set; } // Assuming this is an integer value, adjust as needed

    /// <summary>
    /// Gets or sets the real (double) value of the variable.
    /// </summary>
    public double ValueReal { get; set; } // Assuming this is an double value, adjust as needed

    /// <summary>
    /// Gets or sets the boolean value of the variable (as double).
    /// </summary>
    public double ValueBool { get; set; } // Assuming this is an double value, adjust as needed

    /// <summary>
    /// Gets or sets the string value of the variable.
    /// Set by EF or by builder on runtime, consumer must check for null before accessing.
    /// </summary>
    public string ValueString { get; set; } = null!; // Assuming this is an integer value, adjust as needed

    /// <summary>
    /// Gets or sets the .NET type of the variable.
    /// </summary>
    public required Type Type { get; set; }

    /// <summary>
    /// Parses the raw value and assigns it to the appropriate property based on the variable's type.
    /// </summary>
    /// <param name="rawValue">The raw value as a string.</param>
    /// <param name="logger">The logger for error reporting.</param>
    public void ParseValue(string rawValue, ILogger logger)
    {
        try
        {
            if (this.Type == null)
            {
                throw new InvalidOperationException($"Type is not resolved for {this.Name}");
            }

            if (this.Type == typeof(int))
            {
                this.ValueInt = int.TryParse(rawValue, out var i) ? i : 0;
            }
            else if (this.Type == typeof(short))
            {
                this.ValueInt = short.TryParse(rawValue, out var s) ? s : 0;
            }
            else if (this.Type == typeof(bool))
            {
                this.ValueBool = double.TryParse(rawValue, out var b) ? b : 0;
            }
            else if (this.Type == typeof(double))
            {
                this.ValueReal = double.TryParse(rawValue, out var r) ? r : 0;
            }
            else if (this.Type == typeof(float))
            {
                this.ValueReal = float.TryParse(rawValue, out var f) ? f : 0;
            }
            else if (this.Type == typeof(string))
            {
                this.ValueString = rawValue;
            }
            else
            {
                throw new InvalidOperationException($"Unsupported type: {this.NetType}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to parse value for {Name} with raw value {RawValue}", this.Name, rawValue);
        }
    }

    /// <summary>
    /// Resolves the .NET type from the provided network type string.
    /// </summary>
    /// <param name="netType">The network type as a string.</param>
    /// <returns>The resolved <see cref="Type"/> or null if not found.</returns>
    public Type? ResolveType(string netType) =>
        Type.GetType(netType, throwOnError: false)
        ?? Type.GetType($"System.{netType}")
        ?? netType switch
        {
            "Int32" or "int" => typeof(int),
            "Int16" or "short" => typeof(short),
            "String" or "string" => typeof(string),
            "Boolean" or "bool" => typeof(bool),
            "Single" or "float" => typeof(float),
            "Double" or "double" => typeof(double),
            "Decimal" or "decimal" => typeof(decimal),
            "Byte" or "byte" => typeof(byte),
            "SByte" or "sbyte" => typeof(sbyte),
            _ => null,
        };

    /// <summary>
    /// Gets the value of the variable as the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <returns>The value converted to type <typeparamref name="T"/>.</returns>
    public T GetValue<T>() => (T)Convert.ChangeType(this.ValueInt, typeof(T));

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate variable S7 logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
