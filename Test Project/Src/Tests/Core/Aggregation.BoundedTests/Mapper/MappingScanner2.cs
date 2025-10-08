namespace IndTrace.Aggregation.BoundedTests.Mapper;
/// <summary>
/// Represents the MappingScanner2.
/// </summary>

public class MappingScanner2(params Assembly[] assemblies)
{
    /// <summary>
    /// Executes GetMappingPairs operation.
    /// </summary>
    /// <returns>The result of GetMappingPairs.</returns>
    public IEnumerable<object[]> GetMappingPairs()
    {
        return from assembly in assemblies
               from type in assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
               from method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
               where method.GetParameters().Length == 1
               where new[] { "ToDto", "ToEntity", "ToDtoList" }.Contains(method.Name)
               let sourceType = method.GetParameters()[0].ParameterType
               let returnType = method.ReturnType
               select new object[] { method.Name, method, sourceType, returnType };
    }
}

public static class MappingScanner2TestSource
{
    public static IEnumerable<object[]> MappingData =>
        new MappingScanner2(typeof(IndTrace.Application.BarCodes.Rules.CreateBarCodeExecutor).Assembly).GetMappingPairs();
}
