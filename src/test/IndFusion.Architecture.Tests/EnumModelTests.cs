namespace ExxerAI.Architecture.Tests;

/// <summary>
/// Unit tests for EnumModel architecture validation
/// </summary>
public class EnumModelTests
{
    /// <summary>
    /// Verifies that all enumerations in the ExxerAI.Domain.Enums namespace do not have duplicate IDs.
    /// </summary>
    [Fact]
    public void All_Enumerations_Should_Not_Have_Duplicate_Ids()
    {
        var types = Types.InAssembly(Assembly.Load("ExxerAI.Domain"))
            .That()
            .ResideInNamespace("ExxerAI.Domain.Enums")
            .And()
            .Inherit(typeof(EnumModel))
            .GetTypes();

        foreach (var type in types)
        {
            var instances = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.FieldType == type)
                .Select(f => f.GetValue(null))
                .Cast<EnumModel>()
                .ToList();

            var ids = instances.Select(e => e.Value).ToList();
            Assert.Equal(ids.Count, ids.Distinct().Count());
        }
    }

    /// <summary>
    /// Verifies that all enumerations in the ExxerAI.Domain.Enums namespace do not have duplicate names.
    /// </summary>
    [Fact]
    public void All_Enumerations_Should_Not_Have_Duplicate_Names()
    {
        var types = Types.InAssembly(Assembly.Load("ExxerAI.Domain"))
            .That()
            .ResideInNamespace("ExxerAI.Domain.Enums")
            .And()
            .Inherit(typeof(EnumModel))
            .GetTypes();

        foreach (var type in types)
        {
            var instances = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.FieldType == type)
                .Select(f => f.GetValue(null))
                .Cast<EnumModel>()
                .ToList();

            var names = instances.Select(e => e.Name).ToList();
            Assert.Equal(names.Count, names.Distinct().Count());
        }
    }
}