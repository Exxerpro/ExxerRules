namespace IndTrace.Domain.UnitTests.WorkFlowsTests;

/// <summary>
/// Unit tests for WorkFlow domain entity
/// </summary>
public class WorkFlowTests
{
    /// <summary>
    /// Executes WorkFlow_Constructor_Default_ShouldCreateInstanceWithDefaultValues operation.
    /// </summary>
    [Fact]
    public void WorkFlow_Constructor_Default_ShouldCreateInstanceWithDefaultValues()
    {
        // Arrange & Act
        var workFlow = new WorkFlow();

        // Assert
        workFlow.ShouldNotBeNull();
        workFlow.WorkFlowId.ShouldBe(0);
        workFlow.ProductId.ShouldBe(0);
        workFlow.NextMachineId.ShouldBe(0);
        workFlow.LastMachineId.ShouldBe(0);
        workFlow.RuleId.ShouldBe(0);
        workFlow.Machine.ShouldNotBeNull();
        workFlow.Machine.ShouldBeEmpty();
        workFlow.Edges.ShouldNotBeNull();
        workFlow.Edges.ShouldBeEmpty();
    }
    /// <summary>
    /// Executes WorkFlow_WhenPropertiesAssigned_ShouldMaintainAllValues operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenPropertiesAssigned_ShouldMaintainAllValues()
    {
        // Arrange
        var workFlow = new WorkFlow();
        var workFlowId = 100;
        var productId = 200;
        var nextMachineId = 300;
        var lastMachineId = 400;
        var ruleId = 500;

        // Act
        workFlow.WorkFlowId = workFlowId;
        workFlow.ProductId = productId;
        workFlow.NextMachineId = nextMachineId;
        workFlow.LastMachineId = lastMachineId;
        workFlow.RuleId = ruleId;

        // Assert
        workFlow.WorkFlowId.ShouldBe(workFlowId);
        workFlow.ProductId.ShouldBe(productId);
        workFlow.NextMachineId.ShouldBe(nextMachineId);
        workFlow.LastMachineId.ShouldBe(lastMachineId);
        workFlow.RuleId.ShouldBe(ruleId);
    }
    /// <summary>
    /// Executes WorkFlowProperties_WhenSetToZero_ShouldAcceptZero operation.
    /// </summary>

    [Fact]
    public void WorkFlowProperties_WhenSetToZero_ShouldAcceptZero()
    {
        // Arrange
        var workFlow = new WorkFlow();

        // Act
        workFlow.WorkFlowId = 0;
        workFlow.ProductId = 0;
        workFlow.NextMachineId = 0;
        workFlow.LastMachineId = 0;
        workFlow.RuleId = 0;

        // Assert
        workFlow.WorkFlowId.ShouldBe(0);
        workFlow.ProductId.ShouldBe(0);
        workFlow.NextMachineId.ShouldBe(0);
        workFlow.LastMachineId.ShouldBe(0);
        workFlow.RuleId.ShouldBe(0);
    }
    /// <summary>
    /// Executes WorkFlowProperties_WhenSetToNegative_ShouldAcceptNegative operation.
    /// </summary>

    [Fact]
    public void WorkFlowProperties_WhenSetToNegative_ShouldAcceptNegative()
    {
        // Arrange
        var workFlow = new WorkFlow();

        // Act
        workFlow.WorkFlowId = -1;
        workFlow.ProductId = -1;
        workFlow.NextMachineId = -1;
        workFlow.LastMachineId = -1;
        workFlow.RuleId = -1;

        // Assert
        workFlow.WorkFlowId.ShouldBe(-1);
        workFlow.ProductId.ShouldBe(-1);
        workFlow.NextMachineId.ShouldBe(-1);
        workFlow.LastMachineId.ShouldBe(-1);
        workFlow.RuleId.ShouldBe(-1);
    }
    /// <summary>
    /// Executes WorkFlowProperties_WhenSetToLargeValues_ShouldAcceptLargeValues operation.
    /// </summary>

    [Fact]
    public void WorkFlowProperties_WhenSetToLargeValues_ShouldAcceptLargeValues()
    {
        // Arrange
        var workFlow = new WorkFlow();
        var largeValue = int.MaxValue;

        // Act
        workFlow.WorkFlowId = largeValue;
        workFlow.ProductId = largeValue;
        workFlow.NextMachineId = largeValue;
        workFlow.LastMachineId = largeValue;
        workFlow.RuleId = largeValue;

        // Assert
        workFlow.WorkFlowId.ShouldBe(largeValue);
        workFlow.ProductId.ShouldBe(largeValue);
        workFlow.NextMachineId.ShouldBe(largeValue);
        workFlow.LastMachineId.ShouldBe(largeValue);
        workFlow.RuleId.ShouldBe(largeValue);
    }
    /// <summary>
    /// Executes AddNode_WithValidMachine_ShouldAddToMachineCollection operation.
    /// </summary>

    [Fact]
    public void AddNode_WithValidMachine_ShouldAddToMachineCollection()
    {
        // Arrange
        var workFlow = new WorkFlow();
        var machine = new Machine { MachineId = 100, Name = "Test Machine" };

        // Act
        workFlow.AddNode(machine);

        // Assert
        workFlow.Machine.ShouldNotBeEmpty();
        workFlow.Machine.Count.ShouldBe(1);
        workFlow.Machine.First().ShouldBe(machine);
        workFlow.Machine.First().MachineId.ShouldBe(100);
        workFlow.Machine.First().Name.ShouldBe("Test Machine");
    }
    /// <summary>
    /// Executes AddNode_WithMultipleMachines_ShouldAddAllToMachineCollection operation.
    /// </summary>

    [Fact]
    public void AddNode_WithMultipleMachines_ShouldAddAllToMachineCollection()
    {
        // Arrange
        var workFlow = new WorkFlow();
        var machine1 = new Machine { MachineId = 100, Name = "Machine 1" };
        var machine2 = new Machine { MachineId = 2, Name = "Machine 2" };
        var machine3 = new Machine { MachineId = 3, Name = "Machine 3" };

        // Act
        workFlow.AddNode(machine1);
        workFlow.AddNode(machine2);
        workFlow.AddNode(machine3);

        // Assert
        workFlow.Machine.Count.ShouldBe(3);
        workFlow.Machine.ShouldContain(machine1);
        workFlow.Machine.ShouldContain(machine2);
        workFlow.Machine.ShouldContain(machine3);
    }
    /// <summary>
    /// Executes AddNode_WithNullMachine_ShouldAddNullToMachineCollection operation.
    /// </summary>

    [Fact]
    public void AddNode_WithNullMachine_ShouldAddNullToMachineCollection()
    {
        // Arrange
        var workFlow = new WorkFlow();
        Machine? nullMachine = null;

        // Act
        workFlow.AddNode(nullMachine!);

        // Assert
        workFlow.Machine.Count.ShouldBe(1);
        workFlow.Machine.First().ShouldBeNull();
    }
    /// <summary>
    /// Executes AddEdge_WithValidEdge_ShouldAddToEdgesCollection operation.
    /// </summary>

    [Fact]
    public void AddEdge_WithValidEdge_ShouldAddToEdgesCollection()
    {
        // Arrange
        var workFlow = new WorkFlow();
        var fromMachine = new Machine { MachineId = 100, Name = "From Machine" };
        var toMachine = new Machine { MachineId = 2, Name = "To Machine" };
        var edge = new Edge(fromMachine, toMachine, 5);

        // Act
        workFlow.AddEdge(edge);

        // Assert
        workFlow.Edges.ShouldNotBeEmpty();
        workFlow.Edges.Count.ShouldBe(1);
        workFlow.Edges.First().ShouldBe(edge);
        workFlow.Edges.First().FromMachine.ShouldBe(fromMachine);
        workFlow.Edges.First().ToMachine.ShouldBe(toMachine);
        workFlow.Edges.First().Weight.ShouldBe(5);
    }
    /// <summary>
    /// Executes AddEdge_WithMultipleEdges_ShouldAddAllToEdgesCollection operation.
    /// </summary>

    [Fact]
    public void AddEdge_WithMultipleEdges_ShouldAddAllToEdgesCollection()
    {
        // Arrange
        var workFlow = new WorkFlow();
        var fromMachine1 = new Machine { MachineId = 100, Name = "From 1" };
        var toMachine1 = new Machine { MachineId = 2, Name = "To 1" };
        var fromMachine2 = new Machine { MachineId = 2, Name = "From 2" };
        var toMachine2 = new Machine { MachineId = 3, Name = "To 2" };

        var edge1 = new Edge(fromMachine1, toMachine1, 1);
        var edge2 = new Edge(fromMachine2, toMachine2, 2);

        // Act
        workFlow.AddEdge(edge1);
        workFlow.AddEdge(edge2);

        // Assert
        workFlow.Edges.Count.ShouldBe(2);
        workFlow.Edges.ShouldContain(edge1);
        workFlow.Edges.ShouldContain(edge2);
    }
    /// <summary>
    /// Executes AddEdge_WithNullEdge_ShouldAddNullToEdgesCollection operation.
    /// </summary>

    [Fact]
    public void AddEdge_WithNullEdge_ShouldAddNullToEdgesCollection()
    {
        // Arrange
        var workFlow = new WorkFlow();
        Edge? nullEdge = null;

        // Act
        workFlow.AddEdge(nullEdge!);

        // Assert
        workFlow.Edges.Count.ShouldBe(1);
        workFlow.Edges.First().ShouldBeNull();
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowIsCreated_ShouldHaveDefaultValues operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowIsCreated_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var workFlow = new WorkFlow();

        // Assert - Verify business logic defaults
        workFlow.WorkFlowId.ShouldBe(0, "WorkFlow ID should default to 0");
        workFlow.ProductId.ShouldBe(0, "Product ID should default to 0");
        workFlow.NextMachineId.ShouldBe(0, "Next Machine ID should default to 0");
        workFlow.LastMachineId.ShouldBe(0, "Last Machine ID should default to 0");
        workFlow.RuleId.ShouldBe(0, "Rule ID should default to 0");
        workFlow.Machine.ShouldNotBeNull("Machine collection should be initialized");
        workFlow.Machine.ShouldBeEmpty("Machine collection should be empty initially");
        workFlow.Edges.ShouldNotBeNull("Edges collection should be initialized");
        workFlow.Edges.ShouldBeEmpty("Edges collection should be empty initially");
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowIsConfigured_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowIsConfigured_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            WorkFlowId = 1,
            ProductId = 5080,
            NextMachineId = 200,
            LastMachineId = 300,
            RuleId = 400
        };

        // Act & Assert
        workFlow.ShouldNotBeNull();
        workFlow.WorkFlowId.ShouldBe(1);
        workFlow.ProductId.ShouldBe(5080);
        workFlow.NextMachineId.ShouldBe(200);
        workFlow.LastMachineId.ShouldBe(300);
        workFlow.RuleId.ShouldBe(400);
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowHasSameNextAndLastMachine_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowHasSameNextAndLastMachine_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            NextMachineId = 10000,
            LastMachineId = 10000
        };

        // Act & Assert
        workFlow.NextMachineId.ShouldBe(10000);
        workFlow.LastMachineId.ShouldBe(10000);
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowHasDifferentNextAndLastMachine_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowHasDifferentNextAndLastMachine_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            NextMachineId = 10000,
            LastMachineId = 200
        };

        // Act & Assert
        workFlow.NextMachineId.ShouldBe(10000);
        workFlow.LastMachineId.ShouldBe(200);
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowHasZeroMachineIds_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowHasZeroMachineIds_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            NextMachineId = 0,
            LastMachineId = 0
        };

        // Act & Assert
        workFlow.NextMachineId.ShouldBe(0);
        workFlow.LastMachineId.ShouldBe(0);
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowHasNegativeMachineIds_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowHasNegativeMachineIds_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            NextMachineId = -1,
            LastMachineId = -2
        };

        // Act & Assert
        workFlow.NextMachineId.ShouldBe(-1);
        workFlow.LastMachineId.ShouldBe(-2);
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowHasLargeMachineIds_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowHasLargeMachineIds_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            NextMachineId = int.MaxValue,
            LastMachineId = int.MaxValue - 1
        };

        // Act & Assert
        workFlow.NextMachineId.ShouldBe(int.MaxValue);
        workFlow.LastMachineId.ShouldBe(int.MaxValue - 1);
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowHasZeroProductId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowHasZeroProductId_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            ProductId = 0
        };

        // Act & Assert
        workFlow.ProductId.ShouldBe(0);
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowHasNegativeProductId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowHasNegativeProductId_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            ProductId = -1
        };

        // Act & Assert
        workFlow.ProductId.ShouldBe(-1);
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowHasLargeProductId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowHasLargeProductId_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            ProductId = int.MaxValue
        };

        // Act & Assert
        workFlow.ProductId.ShouldBe(int.MaxValue);
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowHasZeroRuleId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowHasZeroRuleId_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            RuleId = 0
        };

        // Act & Assert
        workFlow.RuleId.ShouldBe(0);
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowHasNegativeRuleId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowHasNegativeRuleId_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            RuleId = -1
        };

        // Act & Assert
        workFlow.RuleId.ShouldBe(-1);
    }
    /// <summary>
    /// Executes WorkFlow_WhenWorkFlowHasLargeRuleId_ShouldBeValid operation.
    /// </summary>

    [Fact]
    public void WorkFlow_WhenWorkFlowHasLargeRuleId_ShouldBeValid()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            RuleId = int.MaxValue
        };

        // Act & Assert
        workFlow.RuleId.ShouldBe(int.MaxValue);
    }
    /// <summary>
    /// Executes GraphOperations_WhenAddingNodesAndEdges_ShouldBuildValidGraph operation.
    /// </summary>

    [Fact]
    public void GraphOperations_WhenAddingNodesAndEdges_ShouldBuildValidGraph()
    {
        // Arrange
        var workFlow = new WorkFlow();
        var machine1 = new Machine { MachineId = 100, Name = "Machine 1" };
        var machine2 = new Machine { MachineId = 2, Name = "Machine 2" };
        var machine3 = new Machine { MachineId = 3, Name = "Machine 3" };

        var edge1 = new Edge(machine1, machine2, 1);
        var edge2 = new Edge(machine2, machine3, 2);

        // Act
        workFlow.AddNode(machine1);
        workFlow.AddNode(machine2);
        workFlow.AddNode(machine3);
        workFlow.AddEdge(edge1);
        workFlow.AddEdge(edge2);

        // Assert
        workFlow.Machine.Count.ShouldBe(3);
        workFlow.Edges.Count.ShouldBe(2);
        workFlow.Machine.ShouldContain(machine1);
        workFlow.Machine.ShouldContain(machine2);
        workFlow.Machine.ShouldContain(machine3);
        workFlow.Edges.ShouldContain(edge1);
        workFlow.Edges.ShouldContain(edge2);
    }
    /// <summary>
    /// Executes GraphOperations_WhenAddingDuplicateNodes_ShouldAllowDuplicates operation.
    /// </summary>

    [Fact]
    public void GraphOperations_WhenAddingDuplicateNodes_ShouldAllowDuplicates()
    {
        // Arrange
        var workFlow = new WorkFlow();
        var machine = new Machine { MachineId = 100, Name = "Test Machine" };

        // Act
        workFlow.AddNode(machine);
        workFlow.AddNode(machine);

        // Assert
        workFlow.Machine.Count.ShouldBe(2);
        workFlow.Machine.ShouldContain(machine);
    }
    /// <summary>
    /// Executes GraphOperations_WhenAddingDuplicateEdges_ShouldAllowDuplicates operation.
    /// </summary>

    [Fact]
    public void GraphOperations_WhenAddingDuplicateEdges_ShouldAllowDuplicates()
    {
        // Arrange
        var workFlow = new WorkFlow();
        var fromMachine = new Machine { MachineId = 100, Name = "From" };
        var toMachine = new Machine { MachineId = 2, Name = "To" };
        var edge = new Edge(fromMachine, toMachine, 1);

        // Act
        workFlow.AddEdge(edge);
        workFlow.AddEdge(edge);

        // Assert
        workFlow.Edges.Count.ShouldBe(2);
        workFlow.Edges.ShouldContain(edge);
    }
}
