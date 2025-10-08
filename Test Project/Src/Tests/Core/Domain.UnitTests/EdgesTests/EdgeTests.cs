namespace IndTrace.Domain.UnitTests.EdgesTests;

/// <summary>
/// Unit tests for Edge
/// </summary>
public class EdgeTests
{
    /// <summary>
    /// Executes Edge_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully operation.
    /// </summary>
    [Fact]
    public void Edge_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully()
    {
        // Arrange & Act - Test parameterless constructor
        var instance = new Edge();

        // Assert
        instance.ShouldNotBeNull();
        instance.EdgeId.ShouldBe(default(int));
        instance.FromMachine.ShouldNotBeNull();
        instance.ToMachine.ShouldNotBeNull();
        instance.FromMachineId.ShouldBe(default(int));
        instance.ToMachineId.ShouldBe(default(int));
        instance.Weight.ShouldBe(default(int));
        instance.ShouldBeAssignableTo<IEntityRoot>();

        // Arrange & Act - Test constructor with machines and weight
        var stampingMachine = new Machine { MachineId = 100001, Name = "Stamping Press A" };
        var weldingMachine = new Machine { MachineId = 100002, Name = "Welding Station B" };
        var weightedEdge = new Edge(stampingMachine, weldingMachine, 3);

        // Assert - Verify all parameters are set correctly
        weightedEdge.ShouldNotBeNull();
        weightedEdge.FromMachine.ShouldBe(stampingMachine);
        weightedEdge.ToMachine.ShouldBe(weldingMachine);
        weightedEdge.Weight.ShouldBe(3);

        // Arrange & Act - Test constructor with machines (default weight)
        var assemblyMachine = new Machine { MachineId = 100003, Name = "Assembly Line C" };
        var inspectionMachine = new Machine { MachineId = 100004, Name = "Quality Inspection D" };
        var defaultEdge = new Edge(assemblyMachine, inspectionMachine);

        // Assert - Verify default weight is 1
        defaultEdge.ShouldNotBeNull();
        defaultEdge.FromMachine.ShouldBe(assemblyMachine);
        defaultEdge.ToMachine.ShouldBe(inspectionMachine);
        defaultEdge.Weight.ShouldBe(1);
    }

    /// <summary>
    /// Executes Edge_WithInvalidConfiguration_ShouldHandleErrorsGracefully operation.
    /// </summary>

    [Fact]
    public void Edge_WithInvalidConfiguration_ShouldHandleErrorsGracefully()
    {
        // Arrange & Act - Test edge cases for manufacturing graph edges
        var validMachine = new Machine { MachineId = 2001, Name = "Valid Machine" };

        // Test null FromMachine parameter
        var nullFromEdge = new Edge(null!, validMachine, 5);
        nullFromEdge.ShouldNotBeNull();
        nullFromEdge.FromMachine.ShouldBeNull();
        nullFromEdge.ToMachine.ShouldBe(validMachine);
        nullFromEdge.Weight.ShouldBe(5);

        // Test null ToMachine parameter
        var nullToEdge = new Edge(validMachine, null!, 10);
        nullToEdge.ShouldNotBeNull();
        nullToEdge.FromMachine.ShouldBe(validMachine);
        nullToEdge.ToMachine.ShouldBeNull();
        nullToEdge.Weight.ShouldBe(10);

        // Test both null machines
        var bothNullEdge = new Edge(null!, null!, 15);
        bothNullEdge.ShouldNotBeNull();
        bothNullEdge.FromMachine.ShouldBeNull();
        bothNullEdge.ToMachine.ShouldBeNull();
        bothNullEdge.Weight.ShouldBe(15);

        // Test negative weight
        var negativeWeightEdge = new Edge(validMachine, validMachine, -1);
        negativeWeightEdge.ShouldNotBeNull();
        negativeWeightEdge.Weight.ShouldBe(-1);

        // Test zero weight
        var zeroWeightEdge = new Edge(validMachine, validMachine, 0);
        zeroWeightEdge.ShouldNotBeNull();
        zeroWeightEdge.Weight.ShouldBe(0);

        // Test maximum weight
        var maxWeightEdge = new Edge(validMachine, validMachine, int.MaxValue);
        maxWeightEdge.ShouldNotBeNull();
        maxWeightEdge.Weight.ShouldBe(int.MaxValue);

        // Test self-loop (same machine as source and destination)
        var selfLoopEdge = new Edge(validMachine, validMachine);
        selfLoopEdge.ShouldNotBeNull();
        selfLoopEdge.FromMachine.ShouldBe(validMachine);
        selfLoopEdge.ToMachine.ShouldBe(validMachine);
        selfLoopEdge.Weight.ShouldBe(1);
    }

    /// <summary>
    /// Executes Edge_WhenPropertiesAssigned_ShouldMaintainAllValues operation.
    /// </summary>

    [Fact]
    public void Edge_WhenPropertiesAssigned_ShouldMaintainAllValues()
    {
        // Arrange - Test all manufacturing edge properties
        var instance = new Edge();
        var cuttingMachine = new Machine { MachineId = 3001, Name = "CNC Cutting Machine" };
        var drillingMachine = new Machine { MachineId = 3002, Name = "Drilling Station" };

        // Act & Assert - Test property setters and getters
        instance.EdgeId = 12345;
        instance.EdgeId.ShouldBe(12345);

        instance.FromMachine = cuttingMachine;
        instance.FromMachine.ShouldBe(cuttingMachine);
        instance.FromMachine.Name.ShouldBe("CNC Cutting Machine");

        instance.ToMachine = drillingMachine;
        instance.ToMachine.ShouldBe(drillingMachine);
        instance.ToMachine.Name.ShouldBe("Drilling Station");

        instance.FromMachineId = 3001;
        instance.FromMachineId.ShouldBe(3001);

        instance.ToMachineId = 3002;
        instance.ToMachineId.ShouldBe(3002);

        instance.Weight = 7;
        instance.Weight.ShouldBe(7);

        // Test property modifications
        instance.Weight = 99;
        instance.Weight.ShouldBe(99);

        instance.EdgeId = 67890;
        instance.EdgeId.ShouldBe(67890);

        // Test with different machines
        var paintingMachine = new Machine { MachineId = 3003, Name = "Painting Booth" };
        var packagingMachine = new Machine { MachineId = 3004, Name = "Packaging Line" };

        instance.FromMachine = paintingMachine;
        instance.ToMachine = packagingMachine;
        instance.FromMachineId = 3003;
        instance.ToMachineId = 3004;

        instance.FromMachine.ShouldBe(paintingMachine);
        instance.ToMachine.ShouldBe(packagingMachine);
        instance.FromMachineId.ShouldBe(3003);
        instance.ToMachineId.ShouldBe(3004);

        // Test extreme values
        instance.Weight = int.MinValue;
        instance.Weight.ShouldBe(int.MinValue);

        instance.Weight = int.MaxValue;
        instance.Weight.ShouldBe(int.MaxValue);

        instance.EdgeId = int.MaxValue;
        instance.EdgeId.ShouldBe(int.MaxValue);
    }

    /// <summary>
    /// Executes Edge_WhenMethodsInvoked_ShouldProduceExpectedOutcomes operation.
    /// </summary>

    [Fact]
    public void Edge_WhenMethodsInvoked_ShouldProduceExpectedOutcomes()
    {
        // Arrange
        var millMachine = new Machine { MachineId = 4001, Name = "Milling Machine" };
        var grindMachine = new Machine { MachineId = 4002, Name = "Grinding Station" };
        var edge1 = new Edge(millMachine, grindMachine, 2);
        var edge2 = new Edge(millMachine, grindMachine, 5); // Different weight, same machines
        var edge3 = new Edge(grindMachine, millMachine, 2); // Reverse direction

        // Act & Assert - Test ToString method
        var toStringResult = edge1.ToString();
        toStringResult.ShouldContain("Machine");

        // Test Equals method with same machines (weight doesn't affect equality)
        edge1.Equals(edge2).ShouldBeTrue(); // Same machines, different weights
        edge1.Equals(edge3).ShouldBeFalse(); // Different direction

        // Test equality operators
        (edge1 == edge2).ShouldBeTrue();
        (edge1 != edge2).ShouldBeFalse();
        (edge1 == edge3).ShouldBeFalse();
        (edge1 != edge3).ShouldBeTrue();

        // Test GetHashCode method
        var hashCode1 = edge1.GetHashCode();
        var hashCode2 = edge2.GetHashCode();
        var hashCode3 = edge3.GetHashCode();

        hashCode1.ShouldBe(hashCode2); // Same machines should have same hash
        hashCode1.ShouldNotBe(hashCode3); // Different machines should have different hash

        // Test GetType method
        var type = edge1.GetType();
        type.ShouldNotBeNull();
        type.Name.ShouldBe("Edge");
        type.Namespace.ShouldBe("IndTrace.Domain.Entities");

        // Test interface implementation
        edge1.ShouldBeAssignableTo<IEntityRoot>();

        // Test Equals with null and different types
        edge1.Equals(null).ShouldBeFalse();
        edge1.Equals("string").ShouldBeFalse();
        edge1.Equals(123).ShouldBeFalse();

        // Test reflexive equality
        edge1.Equals(edge1).ShouldBeTrue();
#pragma warning disable CS1718 // Comparison made to same variable - intentional self-comparison test
        (edge1 == edge1).ShouldBeTrue(); // Self-comparison is always true
#pragma warning restore CS1718

        // Test with null machines
        var nullEdge1 = new Edge(null!, null!, 1);
        var nullEdge2 = new Edge(null!, null!, 2);
        nullEdge1.Equals(nullEdge2).ShouldBeTrue(); // Both have null machines
        (nullEdge1 == nullEdge2).ShouldBeTrue();
    }

    /// <summary>
    /// Executes Edge_BusinessScenario_ShouldEnforceAllDomainRules operation.
    /// </summary>

    [Fact]
    public void Edge_BusinessScenario_ShouldEnforceAllDomainRules()
    {
        // Arrange - Manufacturing workflow graph scenarios
        var rawMaterialStation = new Machine { MachineId = 5001, Name = "Raw Material Input" };
        var stampingPress = new Machine { MachineId = 5002, Name = "Stamping Press" };
        var weldingRobot = new Machine { MachineId = 5003, Name = "Welding Robot" };
        var paintingBooth = new Machine { MachineId = 5004, Name = "Painting Booth" };
        var qualityInspection = new Machine { MachineId = 5005, Name = "Quality Inspection" };
        var packaging = new Machine { MachineId = 5006, Name = "Packaging Station" };

        // Act & Assert - Business Rule 1: Manufacturing workflow sequence
        var workflowEdges = new[]
        {
            new Edge(rawMaterialStation, stampingPress, 1),     // Step 1: Raw materials to stamping
            new Edge(stampingPress, weldingRobot, 2),           // Step 2: Stamping to welding
            new Edge(weldingRobot, paintingBooth, 3),           // Step 3: Welding to painting
            new Edge(paintingBooth, qualityInspection, 4),     // Step 4: Painting to inspection
            new Edge(qualityInspection, packaging, 5)          // Step 5: Inspection to packaging
        };

        // Verify workflow progression
        foreach (var edge in workflowEdges)
        {
            edge.ShouldNotBeNull();
            edge.FromMachine.ShouldNotBeNull();
            edge.ToMachine.ShouldNotBeNull();
            edge.Weight.ShouldBeGreaterThan(0);
        }

        // Business Rule 2: Weight represents processing time or cost
        workflowEdges[0].Weight.ShouldBe(1); // Raw material handling is fastest
        workflowEdges[4].Weight.ShouldBe(5); // Quality inspection takes longest

        // Business Rule 3: Edge uniqueness based on machine pairs
        var duplicateEdge = new Edge(rawMaterialStation, stampingPress, 10); // Different weight
        workflowEdges[0].Equals(duplicateEdge).ShouldBeTrue(); // Same machines = equal
        (workflowEdges[0] == duplicateEdge).ShouldBeTrue();

        // Business Rule 4: Reverse edges are different (directed graph)
        var reverseEdge = new Edge(stampingPress, rawMaterialStation, 1);
        workflowEdges[0].Equals(reverseEdge).ShouldBeFalse();
        (workflowEdges[0] != reverseEdge).ShouldBeTrue();

        // Business Rule 5: Self-loops are valid (maintenance, rework)
        var reworkEdge = new Edge(qualityInspection, qualityInspection, 1);
        reworkEdge.ShouldNotBeNull();
        reworkEdge.FromMachine.ShouldBe(qualityInspection);
        reworkEdge.ToMachine.ShouldBe(qualityInspection);

        // Business Rule 6: Multiple edges between same machines with different weights
        var fastPath = new Edge(stampingPress, weldingRobot, 1);    // Fast setup
        var slowPath = new Edge(stampingPress, weldingRobot, 10);   // Complex setup
        fastPath.Equals(slowPath).ShouldBeTrue(); // Same machines
        fastPath.Weight.ShouldNotBe(slowPath.Weight); // Different costs

        // Business Rule 7: Graph connectivity validation
        var allMachines = new[] { rawMaterialStation, stampingPress, weldingRobot, paintingBooth, qualityInspection, packaging };
        var machineIds = allMachines.Select(m => m.MachineId).ToList();
        machineIds.ShouldBeUnique(); // All machines should have unique IDs

        // Verify each edge connects valid machines
        foreach (var edge in workflowEdges)
        {
            machineIds.ShouldContain(edge.FromMachine.MachineId);
            machineIds.ShouldContain(edge.ToMachine.MachineId);
        }

        // Business Rule 8: Weight constraints for manufacturing
        foreach (var edge in workflowEdges)
        {
            edge.Weight.ShouldBeInRange(1, 100); // Reasonable manufacturing time range
        }

        // Business Rule 9: ToString provides human-readable representation
        var edgeRepresentation = workflowEdges[0].ToString();
        edgeRepresentation.ShouldContain("Machine");
        edgeRepresentation.ShouldContain(" -> ");

        // Business Rule 10: Parallel processing paths
        var parallelPath1 = new Edge(stampingPress, paintingBooth, 5);    // Skip welding
        var parallelPath2 = new Edge(stampingPress, qualityInspection, 8); // Skip welding and painting

        parallelPath1.ShouldNotBeNull();
        parallelPath2.ShouldNotBeNull();
        parallelPath1.Equals(parallelPath2).ShouldBeFalse(); // Different destinations
    }

    /// <summary>
    /// Executes Edge_WhenMachinesAndWeight_ShouldSetProperties operation.
    /// </summary>

    [Fact]
    public void Edge_WhenMachinesAndWeight_ShouldSetProperties()
    {
        // Arrange
        var from = new Machine { MachineId = 100, Name = "A" };
        var to = new Machine { MachineId = 2, Name = "B" };
        int weight = 5;

        // Act
        var edge = new Edge(from, to, weight);

        // Assert
        edge.FromMachine.ShouldBe(from);
        edge.ToMachine.ShouldBe(to);
        edge.Weight.ShouldBe(weight);
    }

    /// <summary>
    /// Executes Edge_Constructor_WithMachines_DefaultWeightIsOne operation.
    /// </summary>

    [Fact]
    public void Edge_Constructor_WithMachines_DefaultWeightIsOne()
    {
        // Arrange
        var from = new Machine { MachineId = 100, Name = "A" };
        var to = new Machine { MachineId = 2, Name = "B" };

        // Act
        var edge = new Edge(from, to);

        // Assert
        edge.FromMachine.ShouldBe(from);
        edge.ToMachine.ShouldBe(to);
        edge.Weight.ShouldBe(1);
    }

    /// <summary>
    /// Executes Equals_WithSameMachines_ShouldReturnTrue operation.
    /// </summary>

    [Fact]
    public void Equals_WithSameMachines_ShouldReturnTrue()
    {
        // Arrange
        var from = new Machine { MachineId = 100, Name = "A" };
        var to = new Machine { MachineId = 2, Name = "B" };
        var edge1 = new Edge(from, to);
        var edge2 = new Edge(from, to);

        // Act & Assert
        edge1.Equals(edge2).ShouldBeTrue();
        (edge1 == edge2).ShouldBeTrue();
        (edge1 != edge2).ShouldBeFalse();
    }

    /// <summary>
    /// Executes Equals_WithDifferentMachines_ShouldReturnFalse operation.
    /// </summary>

    [Fact]
    public void Equals_WithDifferentMachines_ShouldReturnFalse()
    {
        // Arrange
        var from = new Machine { MachineId = 100, Name = "A" };
        var to1 = new Machine { MachineId = 2, Name = "B" };
        var to2 = new Machine { MachineId = 3, Name = "C" };
        var edge1 = new Edge(from, to1);
        var edge2 = new Edge(from, to2);

        // Act & Assert
        edge1.Equals(edge2).ShouldBeFalse();
        (edge1 == edge2).ShouldBeFalse();
        (edge1 != edge2).ShouldBeTrue();
    }

    /// <summary>
    /// Executes GetHashCode_WithSameMachines_ShouldReturnSameHashCode operation.
    /// </summary>

    [Fact]
    public void GetHashCode_WithSameMachines_ShouldReturnSameHashCode()
    {
        // Arrange
        var from = new Machine { MachineId = 100, Name = "A" };
        var to = new Machine { MachineId = 2, Name = "B" };
        var edge1 = new Edge(from, to);
        var edge2 = new Edge(from, to);

        // Act & Assert
        edge1.GetHashCode().ShouldBe(edge2.GetHashCode());
    }

    /// <summary>
    /// Executes ToString_ShouldReturnExpectedFormat operation.
    /// </summary>

    [Fact]
    public void ToString_ShouldReturnExpectedFormat()
    {
        // Arrange
        var from = new Machine { MachineId = 100, Name = "A" };
        var to = new Machine { MachineId = 2, Name = "B" };
        var edge = new Edge(from, to);

        // Act
        var str = edge.ToString();

        // Assert
        str.ShouldContain("Machine");
    }
}
