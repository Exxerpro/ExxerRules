using IndTrace.Application.Products.Commands.Create;
using IndTrace.Application.WorkFlows.Dto;

namespace IndTrace.Domain.UnitTests.ProductsState;
/// <summary>
/// Represents the ProductsViewStateTests.
/// </summary>

public class ProductsViewStateTests
{
    /// <summary>
    /// Executes CreateWorkFlowDtos_ShouldReturnCorrectWorkFlowDtos_WhenMachinesAreProvided operation.
    /// </summary>
    [Fact]
    public void CreateWorkFlowDtos_ShouldReturnCorrectWorkFlowDtos_WhenMachinesAreProvided()
    {
        // Arrange
        var machines = new List<int> { 800, 500, 400, 700 }; // Example machine IDs in random order

        // Act
        var result = CreateProductCommand.CreateWorkFlowDtos(machines);

        // Assert
        result.ShouldBeEquivalentTo(new List<WorkFlowDto>
        {
            new WorkFlowDto { NextMachineId = 400, LastMachineId = 0, RuleId = 2005 },
            new WorkFlowDto { NextMachineId = 500, LastMachineId = 400, RuleId = 2005 },
            new WorkFlowDto { NextMachineId = 700, LastMachineId = 500, RuleId = 2005 },
            new WorkFlowDto { NextMachineId = 800, LastMachineId = 700, RuleId = 2005 },
            new WorkFlowDto { NextMachineId = 0, LastMachineId = 800, RuleId = 2005 }
        });
    }
    /// <summary>
    /// Executes CreateWorkFlowDtos_ShouldReturnEmptyList_WhenNoMachinesAreProvided operation.
    /// </summary>

    [Fact]
    public void CreateWorkFlowDtos_ShouldReturnEmptyList_WhenNoMachinesAreProvided()
    {
        // Arrange
        var machines = new List<int>(); // Empty list of machines

        // Act
        var result = CreateProductCommand.CreateWorkFlowDtos(machines);

        // Assert
        result.ShouldBeEmpty();
    }
    /// <summary>
    /// Executes CreateWorkFlowDtos_ShouldReturnCorrectSingleWorkFlow_WhenOneMachineIsProvided operation.
    /// </summary>

    [Fact]
    public void CreateWorkFlowDtos_ShouldReturnCorrectSingleWorkFlow_WhenOneMachineIsProvided()
    {
        // Arrange
        var machines = new List<int> { 500 }; // Only one machine

        // Act
        var result = CreateProductCommand.CreateWorkFlowDtos(machines);

        // Assert
        result.ShouldBeEquivalentTo(new List<WorkFlowDto>
        {
            new WorkFlowDto { NextMachineId = 500, LastMachineId = 0, RuleId = 2005 },
            new WorkFlowDto { NextMachineId = 0, LastMachineId = 500, RuleId = 2005 }
        });

    }
    /// <summary>
    /// Executes CreateWorkFlowDtos_ShouldRemoveDuplicates_WhenMachinesContainDuplicates operation.
    /// </summary>


    [Fact]
    public void CreateWorkFlowDtos_ShouldRemoveDuplicates_WhenMachinesContainDuplicates()
    {
        // Arrange
        var machines = new List<int> { 800, 500, 400, 500, 700, 800 }; // Machines with duplicates

        // Act
        var result = CreateProductCommand.CreateWorkFlowDtos(machines);

        // Assert
        result.ShouldBeEquivalentTo(new List<WorkFlowDto>
        {
            new WorkFlowDto { NextMachineId = 400, LastMachineId = 0, RuleId = 2005 },
            new WorkFlowDto { NextMachineId = 500, LastMachineId = 400, RuleId = 2005 },
            new WorkFlowDto { NextMachineId = 700, LastMachineId = 500, RuleId = 2005 },
            new WorkFlowDto { NextMachineId = 800, LastMachineId = 700, RuleId = 2005 },
            new WorkFlowDto { NextMachineId = 0, LastMachineId = 800, RuleId = 2005 }
        });
    }
}
