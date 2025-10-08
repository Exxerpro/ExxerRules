namespace IndTrace.Aggregation.BoundedTests.Mapper;
/// <summary>
/// Represents the ScannedClass.
/// </summary>

public record ScannedClass(string MethodName, MethodInfo Method, Type SourceType, Type DestineType);
