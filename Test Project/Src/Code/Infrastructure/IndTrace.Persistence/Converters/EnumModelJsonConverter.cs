using System.Text.Json;
using System.Text.Json.Serialization;
using IndTrace.Domain.Models;

namespace IndTrace.Persistence.Converters;

/// <summary>
/// JSON converter for EnumModel and its derived types to fix HybridCache serialization.
/// Handles the complex initialization logic and private fields of EnumModel instances.
/// </summary>
//[Fix]
//CLAUDE
//Date: 01/09/2025
//Reason: [HybridCache Bug Fix] - Fix EnumModel corruption where properties become null after cache serialization
public class EnumModelJsonConverter : JsonConverter<EnumModel>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(EnumModel).IsAssignableFrom(typeToConvert);
    }

    public override EnumModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }

        int? value = null;
        string? name = null;
        string? displayName = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected PropertyName token");
            }

            string propertyName = reader.GetString()!;
            reader.Read();

            switch (propertyName)
            {
                case "Value":
                    value = reader.GetInt32();
                    break;
                case "Name":
                    name = reader.GetString();
                    break;
                case "DisplayName":
                    displayName = reader.GetString();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        if (!value.HasValue || name == null)
        {
            // Return invalid instance if we don't have required data
            return CreateInvalidInstance(typeToConvert);
        }

        // Use FromValue to properly reconstruct the EnumModel instance
        // This leverages the cached lookup table and proper initialization logic
        var method = typeof(EnumModel).GetMethod("FromValue", new[] { typeof(int) });
        var genericMethod = method!.MakeGenericMethod(typeToConvert);

        try
        {
            var result = (EnumModel?)genericMethod.Invoke(null, new object[] { value.Value });
            return result ?? CreateInvalidInstance(typeToConvert);
        }
        catch
        {
            // Fallback to invalid instance if reconstruction fails
            return CreateInvalidInstance(typeToConvert);
        }
    }

    public override void Write(Utf8JsonWriter writer, EnumModel value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("Value", value.Value);
        writer.WriteString("Name", value.Name);
        writer.WriteString("DisplayName", value.DisplayName);

        writer.WriteEndObject();
    }

    /// <summary>
    /// Creates an invalid instance of the specified EnumModel type.
    /// </summary>
    private static EnumModel CreateInvalidInstance(Type enumType)
    {
        try
        {
            // Try to get the InvalidValue method
            var invalidMethod = typeof(EnumModel).GetMethod("InvalidValue");
            var genericInvalidMethod = invalidMethod!.MakeGenericMethod(enumType);
            return (EnumModel)genericInvalidMethod.Invoke(null, null)!;
        }
        catch
        {
            // Final fallback - create empty instance
            return (EnumModel)Activator.CreateInstance(enumType)!;
        }
    }
}
