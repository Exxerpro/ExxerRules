namespace IndTrace.Aggregation.BoundedTests.Mapper;
/// <summary>
/// Represents the ApplicationWithDto.
/// </summary>

public record ApplicationWithDto(Assembly[] Assemblies, List<ScannedClass> MappingData);
