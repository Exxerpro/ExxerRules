# 📝 **INDTRACE PROPERTIES REFERENCE** 📝

## 🏗️ **ARCHITECTURE PATTERNS**

### **🔄 DTO Pattern**
```csharp
// Standard DTO Structure
public class ExampleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Static conversion methods
    public static ExampleDto ToDto(Example src) { ... }
    public static Example ToEntity(ExampleDto src) { ... }
    public static IEnumerable<ExampleDto> ToDtoList(IEnumerable<Example> src) { ... }
}
```

### **🏛️ Entity Pattern**
```csharp
// Domain Entity Structure
public class Example : AuditableEntity, IEntityRoot
{
    public int ExampleId { get; set; }
    // Navigation properties, business logic
}
```

---

## 🏷️ **BARCODE ENTITIES & DTOS**

### **BarCode** (Core Traceability)
```csharp
// Entity Properties
public int BarCodeId { get; set; }
public string Code { get; set; } = string.Empty;               // "VIN:1FTFW1ET5DFC12345"
public int ProductId { get; set; }
public int MachineId { get; set; }
public PartStatus Status { get; set; } = PartStatus.None;      // EnumModel
public DateTime CreatedAt { get; set; }

// DTO Properties (Same + conversion methods)
public static BarCodeDto ToDto(BarCode src);
public static IEnumerable<BarCodeDto> ToDtoList(IEnumerable<BarCode> src);
```
**Manufacturing Examples**:
- **Automotive**: `"VIN:1FTFW1ET5DFC12345"` (Ford F-150 engine block)
- **Electronics**: `"PCB:C02YG0VZJHD4"` (iPhone motherboard)
- **Pharmaceutical**: `"BATCH:LOT-PFZ-2024-001"` (Vaccine lot)

---

## ⚙️ **MACHINE ENTITIES & DTOS**

### **Machine** (Manufacturing Equipment)
```csharp
// Entity Properties
public int MachineId { get; set; }
public string Name { get; set; } = string.Empty;               // "Robotic Welding Cell #1"
public string Description { get; set; } = string.Empty;        // "Fanuc R-2000iC/210F"
public MachineType Type { get; set; } = MachineType.None;      // EnumModel
public int LineId { get; set; }
public bool IsActive { get; set; } = true;
public string? PlcAddress { get; set; }                        // "192.168.1.100"

// DTO Properties (Same + conversion methods)
public static MachineDto ToDto(Machine src);
```
**Industrial Examples**:
- **Robotic Welding**: `"Fanuc R-2000iC/210F"` - Automotive body welding
- **CNC Machining**: `"Haas VF-4SS"` - Engine block machining
- **Assembly Robot**: `"ABB IRB 6640"` - Heavy part handling
- **Vision System**: `"Cognex In-Sight 7000"` - Quality inspection

---

## 📊 **CYCLE ENTITIES & DTOS**

### **Cycle** (Production Operations)
```csharp
// Entity Properties
public int CycleId { get; set; }
public int BarCodeId { get; set; }
public int MachineId { get; set; }
public CycleStatus Status { get; set; } = CycleStatus.None;    // EnumModel
public DateTime StartTime { get; set; }
public DateTime? EndTime { get; set; }
public TimeSpan? Duration { get; set; }                        // Calculated
public int? OperatorId { get; set; }

// DTO Properties (Same + conversion methods)
public static CycleDto ToDto(Cycle src);
```
**Manufacturing Scenarios**:
- **Machining Cycle**: 45-minute CNC operation for engine block
- **Assembly Cycle**: 12-minute robot assembly operation
- **Inspection Cycle**: 3-minute vision system check
- **Packaging Cycle**: 8-minute automated packaging

---

## 🔗 **WORKFLOW ENTITIES & DTOS**

### **WorkFlow** (Process Definition)
```csharp
// Entity Properties
public int WorkFlowId { get; set; }
public string Name { get; set; } = string.Empty;               // "F-150 Engine Assembly"
public string Description { get; set; } = string.Empty;
public WorkFlowType Type { get; set; } = WorkFlowType.None;    // EnumModel
public int LineId { get; set; }
public int Order { get; set; }                                 // Sequence number
public bool IsActive { get; set; } = true;

// DTO Properties (Same + conversion methods)
public static WorkFlowDto ToDto(WorkFlow src);
```

### **WorkFlowBarCode** (Part in Process)
```csharp
// Entity Properties
public int WorkFlowBarCodeId { get; set; }
public int WorkFlowId { get; set; }
public int BarCodeId { get; set; }
public FlowStatus Status { get; set; } = FlowStatus.None;      // EnumModel
public DateTime EntryTime { get; set; }
public DateTime? ExitTime { get; set; }

// DTO Properties (Same + conversion methods)
public static WorkFlowBarCodeDto ToDto(WorkFlowBarCode src);
```

---

## 🏭 **PRODUCTION LINE ENTITIES**

### **Line** (Production Line)
```csharp
// Entity Properties
public int LineId { get; set; }
public string Name { get; set; } = string.Empty;               // "F-150 Final Assembly"
public string Description { get; set; } = string.Empty;
public bool IsActive { get; set; } = true;
public int? CustomerId { get; set; }

// DTO Properties (Same + conversion methods)
public static LineDto ToDto(Line src);
```

### **Product** (Manufactured Item)
```csharp
// Entity Properties
public int ProductId { get; set; }
public string Name { get; set; } = string.Empty;               // "F-150 SuperCrew 4x4"
public string PartNumber { get; set; } = string.Empty;         // "1L3Z-6006-AA"
public string Description { get; set; } = string.Empty;
public int? CustomerId { get; set; }
public bool IsActive { get; set; } = true;

// DTO Properties (Same + conversion methods)
public static ProductDto ToDto(Product src);
```

---

## 👤 **OPERATIONAL ENTITIES**

### **Shift** (Work Schedule)
```csharp
// Entity Properties
public int ShiftId { get; set; }
public string Name { get; set; } = string.Empty;               // "Day Shift A"
public ShiftType Type { get; set; } = ShiftType.None;          // EnumModel
public TimeSpan StartTime { get; set; }                        // 06:00:00
public TimeSpan EndTime { get; set; }                          // 14:00:00
public bool IsActive { get; set; } = true;

// DTO Properties (Same + conversion methods)
public static ShiftDto ToDto(Shift src);
```

### **Customer** (Manufacturing Client)
```csharp
// Entity Properties
public int CustomerId { get; set; }
public string Name { get; set; } = string.Empty;               // "Ford Motor Company"
public string Code { get; set; } = string.Empty;               // "FORD"
public string? Contact { get; set; }
public bool IsActive { get; set; } = true;

// DTO Properties (Same + conversion methods)
public static CustomerDto ToDto(Customer src);
```

---

## 🔧 **CONFIGURATION ENTITIES**

### **ConfigApp** (Application Settings)
```csharp
// Entity Properties
public int ConfigAppId { get; set; }
public string Name { get; set; } = string.Empty;               // "MaxCycleTime"
public string Value { get; set; } = string.Empty;              // "3600"
public string? Description { get; set; }                       // "Max cycle time in seconds"
public bool IsActive { get; set; } = true;

// DTO Properties (Same + conversion methods)
public static ConfigAppDto ToDto(ConfigApp src);
```

### **ConfigStation** (Station Configuration)
```csharp
// Entity Properties
public int ConfigStationId { get; set; }
public int MachineId { get; set; }
public string Name { get; set; } = string.Empty;               // "WeldPowerSetting"
public string Value { get; set; } = string.Empty;              // "85"
public string? Description { get; set; }

// DTO Properties (Same + conversion methods)
public static ConfigStationDto ToDto(ConfigStation src);
```

---

## 📊 **OEE & PERFORMANCE ENTITIES**

### **OeeRecord** (Overall Equipment Effectiveness)
```csharp
// Entity Properties
public int OeeRecordId { get; set; }
public int MachineId { get; set; }
public DateTime StartTime { get; set; }
public DateTime EndTime { get; set; }
public decimal Availability { get; set; }                      // 0.0 - 1.0 (95% = 0.95)
public decimal Performance { get; set; }                       // 0.0 - 1.0
public decimal Quality { get; set; }                           // 0.0 - 1.0
public decimal OeeValue { get; set; }                          // Calculated: A × P × Q

// DTO Properties (Same + conversion methods)
public static OeeRecordDto ToDto(OeeRecord src);
```
**OEE Benchmarks**:
- **World Class**: 85%+ (0.85)
- **Good**: 70-85% (0.70-0.85)
- **Typical**: 50-70% (0.50-0.70)
- **Poor**: <50% (<0.50)

### **Performance** (Machine Performance)
```csharp
// Entity Properties
public int PerformanceId { get; set; }
public int MachineId { get; set; }
public DateTime Timestamp { get; set; }
public int PlannedUnits { get; set; }                          // Target production
public int ActualUnits { get; set; }                           // Actual production
public decimal PerformanceRatio { get; set; }                  // Actual/Planned

// DTO Properties (Same + conversion methods)
public static PerformanceDto ToDto(Performance src);
```

---

## 🔌 **PLC & COMMUNICATION**

### **Plc** (Programmable Logic Controller)
```csharp
// Entity Properties
public int PlcId { get; set; }
public string Name { get; set; } = string.Empty;               // "Siemens S7-1500"
public string IpAddress { get; set; } = string.Empty;          // "192.168.1.100"
public int Port { get; set; } = 102;                           // S7 default port
public string? Description { get; set; }
public bool IsActive { get; set; } = true;

// DTO Properties (Same + conversion methods)
public static PlcDto ToDto(Plc src);
```

### **Variable** (PLC Tag/Variable)
```csharp
// Entity Properties
public int VariableId { get; set; }
public string Name { get; set; } = string.Empty;               // "M001_CycleStart"
public string Address { get; set; } = string.Empty;            // "DB1.DBX0.0"
public TagsGroupsEnum Group { get; set; } = TagsGroupsEnum.None; // EnumModel
public string DataType { get; set; } = string.Empty;           // "BOOL", "INT", "REAL"
public int PlcId { get; set; }

// DTO Properties (Same + conversion methods)
public static VariableDto ToDto(Variable src);
```

---

## 📋 **TESTING PATTERNS**

### **✅ DTO Testing Pattern**
```csharp
[Fact]
public void Should_CreateDto_When_ValidPropertiesProvided()
{
    // Arrange
    var dto = new BarCodeDto
    {
        BarCodeId = 1,
        Code = "VIN:1FTFW1ET5DFC12345",
        ProductId = 100,
        MachineId = 10,
        Status = PartStatus.Ok
    };

    // Act & Assert
    dto.BarCodeId.ShouldBe(1);
    dto.Code.ShouldBe("VIN:1FTFW1ET5DFC12345");
    dto.Status.ShouldBe(PartStatus.Ok);
}
```

### **🔄 Conversion Testing Pattern**
```csharp
[Fact]
public void Should_ConvertToDto_When_ValidEntityProvided()
{
    // Arrange
    var entity = new BarCode
    {
        BarCodeId = 1,
        Code = "VIN:1FTFW1ET5DFC12345",
        Status = PartStatus.Ok
    };

    // Act
    var dto = BarCodeDto.ToDto(entity);

    // Assert
    dto.BarCodeId.ShouldBe(entity.BarCodeId);
    dto.Code.ShouldBe(entity.Code);
    dto.Status.ShouldBe(entity.Status);
}
```

### **📊 Enum Property Testing**
```csharp
[Theory]
[InlineData(1, "Ok")]
[InlineData(2, "nOK")]
[InlineData(8, "Rejected")]
public void Should_SetEnumProperty_When_ValidEnumValueProvided(int value, string name)
{
    // Arrange & Act
    var dto = new BarCodeDto { Status = value };

    // Assert
    dto.Status.Value.ShouldBe(value);
    dto.Status.Name.ShouldBe(name);
}
```

---

## 🏭 **MANUFACTURING TEST DATA**

### **Automotive Industry**
```csharp
// Ford F-150 Engine Block
var fordBarCode = new BarCodeDto
{
    Code = "VIN:1FTFW1ET5DFC12345",
    ProductId = 1001, // F-150 SuperCrew 4x4
    MachineId = 101   // Robotic Welding Cell
};

// Machine Configuration
var fordMachine = new MachineDto
{
    Name = "Robotic Welding Cell #1",
    Description = "Fanuc R-2000iC/210F",
    Type = MachineType.Process,
    PlcAddress = "192.168.1.100"
};
```

### **Electronics Industry**
```csharp
// iPhone PCB Manufacturing
var appleBarCode = new BarCodeDto
{
    Code = "PCB:C02YG0VZJHD4",
    ProductId = 2001, // iPhone 15 Pro
    MachineId = 201   // SMT Pick & Place
};

// Vision Inspection System
var visionMachine = new MachineDto
{
    Name = "Cognex Vision System",
    Description = "In-Sight 7000 Series",
    Type = MachineType.Inspection,
    PlcAddress = "192.168.2.100"
};
```

### **Pharmaceutical Industry**
```csharp
// Vaccine Batch Tracking
var pfizerBarCode = new BarCodeDto
{
    Code = "BATCH:LOT-PFZ-2024-001",
    ProductId = 3001, // COVID-19 Vaccine
    MachineId = 301   // Filling Machine
};

// High-precision Filling Machine
var fillingMachine = new MachineDto
{
    Name = "Pharmaceutical Filler",
    Description = "Bosch GKF 1500",
    Type = MachineType.Process,
    PlcAddress = "192.168.3.100"
};
```

---

## 📚 **QUICK REFERENCE TABLES**

### **Common String Properties**
| Property | Example Value | Context |
|:---------|:-------------|:---------|
| **Code** | `"VIN:1FTFW1ET5DFC12345"` | Barcode identifier |
| **Name** | `"Robotic Welding Cell #1"` | Machine/Equipment name |
| **PartNumber** | `"1L3Z-6006-AA"` | Ford part number |
| **IpAddress** | `"192.168.1.100"` | PLC network address |
| **Address** | `"DB1.DBX0.0"` | PLC variable address |

### **Common Integer Properties**
| Property | Example Value | Context |
|:---------|:-------------|:---------|
| **Id** | `1001` | Primary key |
| **Port** | `102` | S7 PLC port |
| **Order** | `10` | Sequence number |
| **PlannedUnits** | `100` | Production target |
| **ActualUnits** | `95` | Actual production |

### **Common Decimal Properties**
| Property | Example Value | Context |
|:---------|:-------------|:---------|
| **Availability** | `0.95` | 95% uptime |
| **Performance** | `0.88` | 88% efficiency |
| **Quality** | `0.97` | 97% good parts |
| **OeeValue** | `0.81` | 81% overall effectiveness |

---

**🔧 Usage in Tests**: Use these property patterns and manufacturing examples for comprehensive, realistic test scenarios!
