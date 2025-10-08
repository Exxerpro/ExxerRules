using IndTrace.Domain.Models;
using Microsoft.Extensions.Caching.Hybrid;
using System.Buffers;
using System.Text;
using System.Text.Json;

namespace IndTrace.Persistence.Converters;

/// <summary>
/// Custom HybridCache serializer for EnumModel types to handle proper serialization/deserialization.
/// Fixes the corruption bug where EnumModel properties become null after cache operations.
/// </summary>
/// <typeparam name="T">The EnumModel type that inherits from EnumModel.</typeparam>
//[Fix]
//CLAUDE
//Date: 01/09/2025
//Reason: [HybridCache Bug Fix] - Create proper serializer for EnumModel types to prevent null corruption
public class EnumModelHybridCacheSerializer<T> : IHybridCacheSerializer<T>
    where T : EnumModel, new()
{
    /// <summary>
    /// Deserialize the EnumModel from cache storage.
    /// </summary>
    /// <returns></returns>
    public T Deserialize(ReadOnlySequence<byte> source)
    {
        try
        {
            // Read the integer value from the byte sequence
            var jsonBytes = source.ToArray();
            var value = JsonSerializer.Deserialize<int>(jsonBytes);

            // Convert back to EnumModel using FromValue (uses cached lookup table)
            return EnumModel.FromValue<T>(value);
        }
        catch
        {
            // Return invalid instance if deserialization fails
            return EnumModel.InvalidValue<T>();
        }
    }

    /// <summary>
    /// Serialize the EnumModel to cache storage as its integer value.
    /// </summary>
    public void Serialize(T item, IBufferWriter<byte> target)
    {
        try
        {
            // Serialize the EnumModel as its integer value
            var value = item?.Value ?? -1; // Use -1 for null (will return Invalid on deserialization)
            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(value);
            target.Write(jsonBytes);
        }
        catch
        {
            // Fallback to -1 (Invalid) if serialization fails
            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(-1);
            target.Write(jsonBytes);
        }
    }
}
