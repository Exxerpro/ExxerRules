using IndTrace.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IndTrace.Persistence.Converters;

/// <summary>
/// Provides a value converter for the Label entity to and from string for Entity Framework.
/// </summary>
public class ValueConverterLabel(ConverterMappingHints? mappingHints = null) : ValueConverter<Label, string>(
    label => label.ToString(),
    value => new Label(value),
    mappingHints)
{
}
