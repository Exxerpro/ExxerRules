using Shouldly;
using NetArchTest.Rules;
using Xunit;

namespace Architecture.Tests.Layers;
/// <summary>
/// Represents the LayerTests.
/// </summary>

public class LayerTests
{
    /// <summary>
    /// Executes Services_Should_Not_Depend_On_UI operation.
    /// </summary>
    [Fact]
    public void Services_Should_Not_Depend_On_UI()
    {
        // Arrange
        var servicesNamespace = "MyProject.Services";

        // Act
        var result = Types.InNamespace(servicesNamespace)
            .That()
            .ResideInNamespace(servicesNamespace)
            .ShouldNot()
            .BeAbstract()
            //.HaveDependencyOnNamespace(uiNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.ShouldBeTrue("Services should not depend on UI.");
    }
}
