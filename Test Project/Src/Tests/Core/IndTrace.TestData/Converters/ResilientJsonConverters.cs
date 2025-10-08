namespace IndTrace.TestData.Converters;

/// <summary>
/// Factory for creating resilient enum converters.
/// </summary>
internal static class ResilientJsonConverters
{
    /// <summary>
    /// Creates JSON serializer options with resilient enum handling.
    /// </summary>
    public static JsonSerializerOptions CreateResilientOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        // Add resilient converters for common domain enums
        try
        {
            // Dynamically add converters for enum types in the Domain assembly
            var domainAssembly = typeof(IndTrace.Domain.Entities.Machine).Assembly;

            // Add converters for regular .NET enums
            var enumTypes = domainAssembly.GetTypes()
                .Where(t => t.IsEnum)
                .ToList();

            foreach (var enumType in enumTypes)
            {
                var converterType = typeof(ResilientEnumConverter<>).MakeGenericType(enumType);
                var converter = (JsonConverter)Activator.CreateInstance(converterType)!;
                options.Converters.Add(converter);
            }

            //[Fix] CLAUDE - Date: 26/08/2025
            //Reason: Add resilient converters for smart enums (EnumModel-based classes)
            // Smart enums like MachineType need special handling as they are classes, not structs

            // Add converters for smart enums (EnumModel-based classes)
            var smartEnumTypes = domainAssembly.GetTypes()
                .Where(t => t.IsClass && t.IsSubclassOf(typeof(EnumModel)) && !t.IsAbstract)
                .ToList();

            foreach (var smartEnumType in smartEnumTypes)
            {
                var converterType = typeof(ResilientSmartEnumConverter<>).MakeGenericType(smartEnumType);
                var converter = (JsonConverter)Activator.CreateInstance(converterType)!;
                options.Converters.Add(converter);
            }

            System.Diagnostics.Debug.WriteLine($"Added resilient converters for {enumTypes.Count} regular enums and {smartEnumTypes.Count} smart enums");
        }
        catch (Exception ex)
        {
            // If reflection fails, just log and continue with basic options
            System.Diagnostics.Debug.WriteLine($"Failed to add resilient enum converters: {ex.Message}");
        }

        return options;
    }
}
