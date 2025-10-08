namespace IndTrace.TestData.Converters;

/// <summary>
/// A resilient JSON converter for smart enums (EnumModel-based classes).
/// Handles conversion failures gracefully by falling back to Invalid values.
/// </summary>
internal sealed class ResilientSmartEnumConverter<T> : JsonConverter<T> where T : EnumModel, new()
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            // Try to read as integer first
            if (reader.TokenType == JsonTokenType.Number)
            {
                var intValue = reader.GetInt32();
                // Use the smart enum's built-in FromValue method which handles fallback to Invalid
                return EnumModel.FromValue<T>(intValue);
            }
            // Try to read as string
            else if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (!string.IsNullOrEmpty(stringValue))
                {
                    // Try parsing as integer first
                    if (int.TryParse(stringValue, out var intValue))
                    {
                        return EnumModel.FromValue<T>(intValue);
                    }
                    // Try parsing by name
                    return EnumModel.FromName<T>(stringValue);
                }
            }
        }
        catch (Exception ex)
        {
            // Log parsing errors for diagnostics (in debug builds)
            System.Diagnostics.Debug.WriteLine($"ResilientSmartEnumConverter<{typeof(T).Name}>: Failed to parse value, falling back to Invalid. Error: {ex.Message}");
        }

        // Fall back to Invalid using the smart enum's built-in mechanism
        return EnumModel.InvalidValue<T>();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        // Write smart enum as integer value
        writer.WriteNumberValue(value.Value);
    }
}
