using System.Buffers;
using System.Text;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;

namespace IndTrace.Persistence.Converters;

/// <summary>
/// Custom HybridCache serializer that uses System.Text.Json with EnumModel converter support.
/// This fixes the critical production bug where EnumModel properties become null after caching.
/// </summary>
/// <typeparam name="T">The type to serialize/deserialize.</typeparam>
//[Fix]
//CLAUDE
//Date: 01/09/2025
//Reason: [HybridCache Bug Fix] - Create JsonCacheSerializer with EnumModel converter support
public class JsonCacheSerializer<T> : IHybridCacheSerializer<T>
{
    private static readonly JsonSerializerOptions jsonOptions;

    static JsonCacheSerializer()
    {
        jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };
        jsonOptions.Converters.Add(new EnumModelJsonConverter());
    }

    /// <summary>
    /// Deserializes the byte sequence back into a C# object.
    /// </summary>
    /// <returns></returns>
    public T Deserialize(ReadOnlySequence<byte> source)
    {
        try
        {
            var json = Encoding.UTF8.GetString(source.ToArray());
            return JsonSerializer.Deserialize<T>(json, jsonOptions)!;
        }
        catch
        {
            // Return default value if deserialization fails
            return default(T)!;
        }
    }

    /// <summary>
    /// Serializes the C# object into a byte sequence.
    /// </summary>
    public void Serialize(T value, IBufferWriter<byte> target)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, jsonOptions);
            var bytes = Encoding.UTF8.GetBytes(json);
            target.Write(bytes);
        }
        catch
        {
            // Write empty object if serialization fails
            var fallbackBytes = Encoding.UTF8.GetBytes("{}");
            target.Write(fallbackBytes);
        }
    }
}
