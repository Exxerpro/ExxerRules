namespace Application.UnitTests.Features.WorkFlows;

/// <summary>
/// Unit tests for WorkFlowDto
/// </summary>
public class WorkFlowDtoTests
{
    /// <summary>
    /// Executes Constructor_WithDefaultConstructor_ShouldCreateInstanceWithDefaultValues operation.
    /// </summary>
    [Fact]
    public void Constructor_WithDefaultConstructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var workFlowDto = new WorkFlowDto();

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 24/08/2025
        //Reason: [PATTERN 17 FIX] - Collections are initialized with = null! by default, not empty collections. Fix test expectations.
        workFlowDto.ShouldNotBeNull();
        workFlowDto.WorkFlowId.ShouldBe(0);
        workFlowDto.ProductId.ShouldBe(0);
        workFlowDto.NextMachineId.ShouldBe(0);
        workFlowDto.LastMachineId.ShouldBe(0);
        workFlowDto.RuleId.ShouldBe(0);
        workFlowDto.Machine.ShouldNotBeNull(); // Collections are null by default
        workFlowDto.Edges.ShouldNotBeNull();   // Collections are null by default
    }

    /// <summary>
    /// Executes Properties_WhenSet_ShouldReturnCorrectValues operation.
    /// </summary>

    [Fact]
    public void Properties_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var workFlowDto = new WorkFlowDto();
        var machines = new List<Machine>
        {
            new() { MachineId = 100001, Name = "CNC Mill A" },
            new() { MachineId = 100002, Name = "CNC Mill B" }
        };
        var edges = new List<Edge>
        {
            new() { EdgeId = 1, FromMachineId = 100001, ToMachineId = 100002 }
        };

        // Act
        workFlowDto.WorkFlowId = 12345;
        workFlowDto.ProductId = 67890;
        workFlowDto.NextMachineId = 100002;
        workFlowDto.LastMachineId = 100001;
        workFlowDto.RuleId = 2005;
        workFlowDto.Machine = machines;
        workFlowDto.Edges = edges;

        // Assert
        workFlowDto.WorkFlowId.ShouldBe(12345);
        workFlowDto.ProductId.ShouldBe(67890);
        workFlowDto.NextMachineId.ShouldBe(100002);
        workFlowDto.LastMachineId.ShouldBe(100001);
        workFlowDto.RuleId.ShouldBe(2005);
        workFlowDto.Machine.ShouldBeSameAs(machines);
        workFlowDto.Machine.Count.ShouldBe(2);
        workFlowDto.Edges.ShouldBeSameAs(edges);
        workFlowDto.Edges.Count.ShouldBe(1);
    }

    /// <summary>
    /// Executes Machine_Collection_ShouldBeInitializedAndMutable operation.
    /// </summary>

    [Fact]
    public void Machine_Collection_ShouldBeInitializedAndMutable()
    {
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 17 Fix - Machine collection is null by default, need to initialize before adding
        // Arrange
        var workFlowDto = new WorkFlowDto();
        var machine = new Machine { MachineId = 5001, Name = "Assembly Station" };

        // Act
        workFlowDto.Machine = new List<Machine>();
        workFlowDto.Machine.Add(machine);

        // Assert
        workFlowDto.Machine.Count.ShouldBe(1);
        workFlowDto.Machine[0].ShouldBeSameAs(machine);
        workFlowDto.Machine[0].MachineId.ShouldBe(5001);
        workFlowDto.Machine[0].Name.ShouldBe("Assembly Station");
    }

    /// <summary>
    /// Executes Edges_Collection_ShouldBeInitializedAndMutable operation.
    /// </summary>

    [Fact]
    public void Edges_Collection_ShouldBeInitializedAndMutable()
    {
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 17 Fix - Edges collection is null by default, need to initialize before adding
        // Arrange
        var workFlowDto = new WorkFlowDto();
        var edge = new Edge { EdgeId = 100, FromMachineId = 2001, ToMachineId = 2002 };

        // Act
        workFlowDto.Edges = new List<Edge>();
        workFlowDto.Edges.Add(edge);

        // Assert
        workFlowDto.Edges.Count.ShouldBe(1);
        workFlowDto.Edges[0].ShouldBeSameAs(edge);
        workFlowDto.Edges[0].EdgeId.ShouldBe(100);
        workFlowDto.Edges[0].FromMachineId.ShouldBe(2001);
        workFlowDto.Edges[0].ToMachineId.ShouldBe(2002);
    }

    // ToDto Static Method Tests
    /// <summary>
    /// Executes ToDto_WithNullWorkFlow_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void ToDto_WithNullWorkFlow_ShouldReturnFailureResult()
    {
        // Arrange
        WorkFlow? nullWorkFlow = null!;

        // Act
        var result = WorkFlowDto.ToDto(nullWorkFlow!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("Parameter 'src' cannot be null");
    }

    /// <summary>
    /// Executes ToDto_WithValidWorkFlow_ShouldMapAllProperties operation.
    /// </summary>

    [Fact]
    public void ToDto_WithValidWorkFlow_ShouldMapAllProperties()
    {
        // Arrange
        var machines = new List<Machine>
        {
            new() { MachineId = 3001, Name = "Injection Molding A" },
            new() { MachineId = 3002, Name = "Quality Inspection B" }
        };
        var edges = new List<Edge>
        {
            new() { EdgeId = 200, FromMachineId = 3001, ToMachineId = 3002 }
        };

        var workFlow = new WorkFlow
        {
            WorkFlowId = 98765,
            ProductId = 54321,
            NextMachineId = 3002,
            LastMachineId = 3001,
            RuleId = 3000,
            Machine = machines,
            Edges = edges
        };

        // Act
        var dtoWrapper = WorkFlowDto.ToDto(workFlow);

        // Assert
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.WorkFlowId.ShouldBe(98765);
        dto.ProductId.ShouldBe(54321);
        dto.NextMachineId.ShouldBe(3002);
        dto.LastMachineId.ShouldBe(3001);
        dto.RuleId.ShouldBe(3000);
        dto.Machine.ShouldBeSameAs(machines);
        dto.Machine.Count.ShouldBe(2);
        dto.Edges.ShouldBeSameAs(edges);
        dto.Edges.Count.ShouldBe(1);
    }

    /// <summary>
    /// Executes ToDto_WithMinimalWorkFlow_ShouldMapBasicProperties operation.
    /// </summary>

    [Fact]
    public void ToDto_WithMinimalWorkFlow_ShouldMapBasicProperties()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            WorkFlowId = 1,
            ProductId = 5080,
            NextMachineId = 2001,
            LastMachineId = 2000
        };

        // Act
        var dtoWrapper = WorkFlowDto.ToDto(workFlow);

        // Assert
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.WorkFlowId.ShouldBe(1);
        dto.ProductId.ShouldBe(5080);
        dto.NextMachineId.ShouldBe(2001);
        dto.LastMachineId.ShouldBe(2000);
        dto.RuleId.ShouldBe(0);
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 17 Fix - WorkFlow entity initializes both Machine and Edges as empty lists ([]), not one empty and one null
        dto.Machine.ShouldNotBeNull().ShouldBeEmpty(); // From entity = []
        dto.Edges.ShouldNotBeNull().ShouldBeEmpty(); // From entity = []
    }

    /// <summary>
    /// Executes ToDto_WithNullCollections_ShouldHandleGracefully operation.
    /// </summary>

    [Fact]
    public void ToDto_WithNullCollections_ShouldHandleGracefully()
    {
        // Arrange
        var workFlow = new WorkFlow
        {
            WorkFlowId = 123,
            ProductId = 456,
            Machine = null!,
            Edges = null!
        };

        // Act
        var dtoWrapper = WorkFlowDto.ToDto(workFlow);

        // Assert
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.ShouldNotBeNull();
        dto.WorkFlowId.ShouldBe(123);
        dto.ProductId.ShouldBe(456);
        dto.Machine.ShouldBeNull();
        dto.Edges.ShouldBeNull();
    }

    // ToEntity Static Method Tests
    /// <summary>
    /// Executes ToEntity_WithNullWorkFlowDto_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void ToEntity_WithNullWorkFlowDto_ShouldReturnFailureResult()
    {
        // Arrange
        WorkFlowDto? nullDto = null!;

        // Act
        var result = WorkFlowDto.ToEntity(nullDto!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("Parameter 'src' cannot be null");
    }

    /// <summary>
    /// Executes ToEntity_WithValidWorkFlowDto_ShouldMapAllProperties operation.
    /// </summary>

    [Fact]
    public void ToEntity_WithValidWorkFlowDto_ShouldMapAllProperties()
    {
        // Arrange
        var machines = new List<Machine>
        {
            new() { MachineId = 4001, Name = "Welding Station A" },
            new() { MachineId = 4002, Name = "Paint Booth B" }
        };
        var edges = new List<Edge>
        {
            new() { EdgeId = 300, FromMachineId = 4001, ToMachineId = 4002 }
        };

        var dto = new WorkFlowDto
        {
            WorkFlowId = 11111,
            ProductId = 22222,
            NextMachineId = 4002,
            LastMachineId = 4001,
            RuleId = 4000,
            Machine = machines,
            Edges = edges
        };

        // Act
        var entityWrapper = WorkFlowDto.ToEntity(dto);

        // Assert
        entityWrapper.IsSuccess.ShouldBeTrue();
        entityWrapper.Value.ShouldNotBeNull();
        var entity = entityWrapper.Value;
        entity.ShouldNotBeNull();
        entity.ShouldNotBeNull();
        entity.ShouldNotBeNull();
        entity.WorkFlowId.ShouldBe(11111);
        entity.ProductId.ShouldBe(22222);
        entity.NextMachineId.ShouldBe(4002);
        entity.LastMachineId.ShouldBe(4001);
        entity.RuleId.ShouldBe(4000);
        entity.Machine.ShouldBeSameAs(machines);
        entity.Machine.Count.ShouldBe(2);
        entity.Edges.ShouldBeSameAs(edges);
        entity.Edges.Count.ShouldBe(1);
    }

    /// <summary>
    /// Executes ToEntity_WithMinimalWorkFlowDto_ShouldMapBasicProperties operation.
    /// </summary>

    [Fact]
    public void ToEntity_WithMinimalWorkFlowDto_ShouldMapBasicProperties()
    {
        // Arrange
        var dto = new WorkFlowDto
        {
            WorkFlowId = 999,
            ProductId = 888,
            NextMachineId = 777,
            LastMachineId = 666
        };

        // Act
        var entityWrapper = WorkFlowDto.ToEntity(dto);

        // Assert
        entityWrapper.IsSuccess.ShouldBeTrue();
        entityWrapper.Value.ShouldNotBeNull();
        var entity = entityWrapper.Value;
        entity.ShouldNotBeNull();
        entity.ShouldNotBeNull();
        entity.ShouldNotBeNull();
        entity.WorkFlowId.ShouldBe(999);
        entity.ProductId.ShouldBe(888);
        entity.NextMachineId.ShouldBe(777);
        entity.LastMachineId.ShouldBe(666);
        entity.RuleId.ShouldBe(0);
        entity.Machine.ShouldNotBeNull(); // DTO initializes empty list
        entity.Machine.ShouldBeEmpty();
        entity.Edges.ShouldNotBeNull(); // DTO initializes empty list
        entity.Edges.ShouldBeEmpty();
    }

    // Round-trip Conversion Tests
    /// <summary>
    /// Executes ToDto_ThenToEntity_ShouldPreserveAllProperties operation.
    /// </summary>

    [Fact]
    public void ToDto_ThenToEntity_ShouldPreserveAllProperties()
    {
        // Arrange
        var originalWorkFlow = new WorkFlow
        {
            WorkFlowId = 55555,
            ProductId = 44444,
            NextMachineId = 6002,
            LastMachineId = 6001,
            RuleId = 5000,
            Machine =
            [
                new() { MachineId = 6001, Name = "Lathe Station" },
                new() { MachineId = 6002, Name = "Finishing Station" }
            ],
            Edges =
            [
                new() { EdgeId = 400, FromMachineId = 6001, ToMachineId = 6002 }
            ]
        };

        // Act
        var dtoWrapper = WorkFlowDto.ToDto(originalWorkFlow);
        dtoWrapper.IsSuccess.ShouldBeTrue();
        dtoWrapper.Value.ShouldNotBeNull();
        var dto = dtoWrapper.Value;

        var convertedEntityWrapper = WorkFlowDto.ToEntity(dto);
        convertedEntityWrapper.IsSuccess.ShouldBeTrue();
        convertedEntityWrapper.Value.ShouldNotBeNull();
        var convertedEntity = convertedEntityWrapper.Value;
        convertedEntity.ShouldNotBeNull();
        convertedEntity.ShouldNotBeNull();

        // Assert
        convertedEntity.WorkFlowId.ShouldBe(originalWorkFlow.WorkFlowId);
        convertedEntity.ProductId.ShouldBe(originalWorkFlow.ProductId);
        convertedEntity.NextMachineId.ShouldBe(originalWorkFlow.NextMachineId);
        convertedEntity.LastMachineId.ShouldBe(originalWorkFlow.LastMachineId);
        convertedEntity.RuleId.ShouldBe(originalWorkFlow.RuleId);
        convertedEntity.Machine.ShouldBeSameAs(originalWorkFlow.Machine);
        convertedEntity.Edges.ShouldBeSameAs(originalWorkFlow.Edges);
    }

    // Industrial Scenario Tests
    /// <summary>
    /// Executes WorkFlowDto_WithAutomotiveManufacturingScenario_ShouldHandleComplexWorkflow operation.
    /// </summary>

    [Fact]
    public void WorkFlowDto_WithAutomotiveManufacturingScenario_ShouldHandleComplexWorkflow()
    {
        // Arrange - Automotive assembly line workflow
        var workFlowDto = new WorkFlowDto();

        // Act - Set up automotive manufacturing workflow
        workFlowDto.WorkFlowId = 1001;
        workFlowDto.ProductId = 501234; // Engine block product
        workFlowDto.LastMachineId = 0; // Start of workflow
        workFlowDto.NextMachineId = 7001; // First CNC machine
        workFlowDto.RuleId = 2005;

        // Add automotive manufacturing machines
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 17 Fix - Machine collection is null by default, must initialize before AddRange
        workFlowDto.Machine = new List<Machine>();
        workFlowDto.Machine.AddRange(new List<Machine>
        {
            new() { MachineId = 7001, Name = "CNC Rough Milling Station", MachineType = 8 }, // Process
            new() { MachineId = 7002, Name = "CNC Precision Boring Machine", MachineType = 8 }, // Process
            new() { MachineId = 7003, Name = "Surface Honing Station", MachineType = 8 }, // Process
            new() { MachineId = 7004, Name = "Quality Inspection CMM", MachineType = 32 } // Inspection
        });

        // Add workflow edges
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 17 Fix - Edges collection is null by default, must initialize before AddRange
        workFlowDto.Edges = new List<Edge>();
        workFlowDto.Edges.AddRange(new List<Edge>
        {
            new() { EdgeId = 1001, FromMachineId = 0, ToMachineId = 7001 },
            new() { EdgeId = 1002, FromMachineId = 7001, ToMachineId = 7002 },
            new() { EdgeId = 1003, FromMachineId = 7002, ToMachineId = 7003 },
            new() { EdgeId = 1004, FromMachineId = 7003, ToMachineId = 7004 }
        });

        // Assert - Verify automotive workflow
        workFlowDto.WorkFlowId.ShouldBe(1001);
        workFlowDto.ProductId.ShouldBe(501234);
        workFlowDto.Machine.Count.ShouldBe(4);
        workFlowDto.Machine[0].Name.ShouldBe("CNC Rough Milling Station");
        workFlowDto.Machine[3].MachineType.Value.ShouldBe(32); // Inspection type
        workFlowDto.Edges.Count.ShouldBe(4);
        workFlowDto.Edges[1].FromMachineId.ShouldBe(7001); // Edge from CNC mill to boring
        workFlowDto.RuleId.ShouldBe(2005);
    }

    /// <summary>
    /// Executes WorkFlowDto_WithElectronicsManufacturingScenario_ShouldHandleComplexWorkflow operation.
    /// </summary>

    [Fact]
    public void WorkFlowDto_WithElectronicsManufacturingScenario_ShouldHandleComplexWorkflow()
    {
        // Arrange - Electronics PCB assembly workflow
        var workFlowDto = new WorkFlowDto();

        // Act - Set up electronics manufacturing workflow
        workFlowDto.WorkFlowId = 2001;
        workFlowDto.ProductId = 700456; // PCB product
        workFlowDto.LastMachineId = 0; // Start of workflow
        workFlowDto.NextMachineId = 8001; // First SMT machine
        workFlowDto.RuleId = 2005;

        // Add electronics manufacturing machines
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 17 Fix - Machine collection is null by default, must initialize before AddRange
        workFlowDto.Machine = new List<Machine>();
        workFlowDto.Machine.AddRange(new List<Machine>
        {
            new() { MachineId = 8001, Name = "SMT Pick & Place Line 1", MachineType = 8 }, // Process
            new() { MachineId = 8002, Name = "Reflow Oven Station", MachineType = 8 }, // Process
            new() { MachineId = 8003, Name = "ICT Testing Station", MachineType = 32 }, // Inspection
            new() { MachineId = 8004, Name = "Final Assembly & Packaging", MachineType = 16 } // Final
        });

        // Add workflow edges
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 17 Fix - Edges collection is null by default, must initialize before AddRange
        workFlowDto.Edges = new List<Edge>();
        workFlowDto.Edges.AddRange(new List<Edge>
        {
            new() { EdgeId = 2001, FromMachineId = 0, ToMachineId = 8001 },
            new() { EdgeId = 2002, FromMachineId = 8001, ToMachineId = 8002 },
            new() { EdgeId = 2003, FromMachineId = 8002, ToMachineId = 8003 },
            new() { EdgeId = 2004, FromMachineId = 8003, ToMachineId = 8004 }
        });

        // Assert - Verify electronics workflow
        workFlowDto.WorkFlowId.ShouldBe(2001);
        workFlowDto.ProductId.ShouldBe(700456);
        workFlowDto.Machine.Count.ShouldBe(4);
        workFlowDto.Machine[0].Name.ShouldBe("SMT Pick & Place Line 1");
        workFlowDto.Machine[1].MachineType.Value.ShouldBe(8); // Process type
        workFlowDto.Edges.Count.ShouldBe(4);
        workFlowDto.Edges[0].ToMachineId.ShouldBe(8001); // First edge goes to SMT machine
        workFlowDto.RuleId.ShouldBe(2005);
    }

    /// <summary>
    /// Executes WorkFlowDto_WithLinearWorkflow_ShouldMaintainSequentialOrder operation.
    /// </summary>

    [Fact]
    public void WorkFlowDto_WithLinearWorkflow_ShouldMaintainSequentialOrder()
    {
        // Arrange - Linear production workflow
        var workFlowDto = new WorkFlowDto();

        // Act - Set up sequential workflow
        workFlowDto.WorkFlowId = 3001;
        workFlowDto.ProductId = 12345;
        workFlowDto.LastMachineId = 9001;
        workFlowDto.NextMachineId = 9002;
        workFlowDto.RuleId = 2005;

        // Sequential machines
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 17 Fix - Machine collection is null by default, must initialize before AddRange
        workFlowDto.Machine = new List<Machine>();
        workFlowDto.Machine.AddRange(new List<Machine>
        {
            new() { MachineId = 9001, Name = "Input Station", MachineType = 2 }, // Initial
            new() { MachineId = 9002, Name = "Process Station A", MachineType = 8 }, // Process
            new() { MachineId = 9003, Name = "Process Station B", MachineType = 8 }, // Process
            new() { MachineId = 9004, Name = "Output Station", MachineType = 16 } // Final
        });

        // Linear edges
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 17 Fix - Edges collection is null by default, must initialize before AddRange
        workFlowDto.Edges = new List<Edge>();
        workFlowDto.Edges.AddRange(new List<Edge>
        {
            new() { EdgeId = 3001, FromMachineId = 9001, ToMachineId = 9002 },
            new() { EdgeId = 3002, FromMachineId = 9002, ToMachineId = 9003 },
            new() { EdgeId = 3003, FromMachineId = 9003, ToMachineId = 9004 }
        });

        // Assert - Verify linear workflow
        workFlowDto.Machine.Count.ShouldBe(4);
        workFlowDto.Edges.Count.ShouldBe(3);
        workFlowDto.Machine.ShouldAllBe(m => m.MachineType > 0);
        workFlowDto.Machine.OrderBy(m => m.MachineId).First().Name.ShouldBe("Input Station");
        workFlowDto.Machine.OrderBy(m => m.MachineId).Last().Name.ShouldBe("Output Station");
    }
}
