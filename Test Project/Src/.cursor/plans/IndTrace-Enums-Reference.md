# 🔢 **INDTRACE ENUMS REFERENCE** 🔢

## 📋 **ENUM ARCHITECTURE**
- **Base Class**: `EnumModel` with `IEnumModel` interface
- **Pattern**: Static readonly fields with Value, Name, DisplayName
- **Conversion**: Implicit operators for int/string conversion
- **Usage**: `PartStatus.Ok`, `MachineType.Printer`, etc.

---

## 🏭 **MANUFACTURING ENUMS**

### **🏷️ PartStatus** (Product Quality Status)
```csharp
PartStatus.Invalid    = (-1, "Invalid Value")
PartStatus.None       = (0, "None")
PartStatus.Ok         = (1, "Ok")           // ✅ Good part
PartStatus.NOk        = (2, "nOK")          // ❌ Defective part  
PartStatus.Restored   = (4, "Restored")     // 🔄 Restored to good
PartStatus.Rejected   = (8, "Rejected")     // 🚫 Permanently rejected
PartStatus.Scrap      = (512, "Scrap")      // 🗑️ Scrap material
```
**Manufacturing Context**: Ford F-150 engine blocks, Tesla battery packs, iPhone PCBs

### **🔄 CycleStatus** (Production Cycle State)
```csharp
CycleStatus.Invalid       = (-1, "Invalid Value")
CycleStatus.None          = (0, "None")
CycleStatus.NotStarted    = (1, "NotStarted")    // 🕐 Waiting to start
CycleStatus.Started       = (2, "Started")       // ▶️ In progress
CycleStatus.FinishedOk    = (4, "FinishedOk")    // ✅ Completed successfully
CycleStatus.FinishedNok   = (8, "FinishedNok")   // ❌ Completed with errors
CycleStatus.EndOfProcess  = (16, "EndOfProcess") // 🏁 Final stage
CycleStatus.Rejected      = (32, "Rejected")     // 🚫 Cycle rejected
CycleStatus.Canceled      = (64, "Canceled")     // ⏹️ Cycle canceled
```
**Manufacturing Context**: CNC machining cycles, assembly line operations, injection molding

### **🌊 FlowStatus** (Workflow Process State)
```csharp
FlowStatus.None       = (0, "None")
FlowStatus.Created    = (1, "Created")      // 🆕 Flow initiated
FlowStatus.InProcess  = (2, "InProcess")    // ⚙️ Currently processing
FlowStatus.Finished   = (4, "Finished")    // ✅ Flow completed
FlowStatus.Invalid    = (8, "Invalid")     // ❌ Invalid state
FlowStatus.Restored   = (16, "Restored")   // 🔄 Restored flow
FlowStatus.Rejected   = (32, "Rejected")   // 🚫 Flow rejected
```
**Manufacturing Context**: Automotive paint booth flows, electronics assembly workflows

---

## 🏭 **MACHINE & WORKFLOW ENUMS**

### **🤖 MachineType** (Equipment Classification)
```csharp
MachineType.Invalid        = (-1, "Invalid Value")
MachineType.None          = (0, "None")
MachineType.Printer       = (1, "Printer")         // 🖨️ Label/barcode printer
MachineType.Initial       = (2, "Initial")         // 🚀 Entry point
MachineType.InitialPrinter = (4, "InitialPrinter") // 🖨️🚀 Combined entry+print
MachineType.Process       = (8, "ProcessAsync")    // ⚙️ Manufacturing operation
MachineType.Final         = (16, "Final")          // 🏁 Exit point
MachineType.Inspection    = (32, "Inspection")     // 🔍 Quality control
MachineType.DashBoard     = (64, "DashBoard")      // 📊 Monitoring station
```
**Manufacturing Context**:
- **Printer**: Zebra ZT610 industrial label printer
- **Process**: Fanuc robotic welding cell, ABB paint robot
- **Inspection**: Cognex vision system, Keyence laser scanner

### **🔗 WorkFlowType** (Process Flow Pattern)
```csharp
WorkFlowType.Invalid  = (-1, "Invalid Value")
WorkFlowType.None     = (0, "None")
WorkFlowType.Initial  = (1, "Initial")      // 🚀 Start of line
WorkFlowType.Serial   = (2, "ProcessAsync") // ➡️ Sequential processing
WorkFlowType.Lateral  = (4, "Lateral")      // ↔️ Side branch
WorkFlowType.Diverter = (8, "Diverter")     // 🔀 Route selection
WorkFlowType.Merger   = (16, "Merger")      // 🔀 Combine flows
WorkFlowType.Final    = (32, "Final")       // 🏁 End of line
```
**Manufacturing Context**: Automotive assembly line routing, electronics PCB flow

---

## 🚪 **GATEWAY & TASK ENUMS**

### **📋 GatewayTask** (System Operations)
```csharp
GatewayTask.Invalid              = (-1, "Invalid Value")
GatewayTask.None                = (0, "None")
GatewayTask.CreateBarCodeAsync  = (4, "CreateBarCodeAsync")   // 🏷️ Generate barcode
GatewayTask.ReadBarCodeAsync    = (8, "ReadBarCodeAsync")     // 📖 Scan barcode
GatewayTask.CreateCycleAsync    = (16, "CreateCycleAsync")    // 🔄 Start cycle
GatewayTask.UpdateCycleOkAsync  = (32, "UpdateCycleOkAsync") // ✅ Mark success
GatewayTask.UpdateCycleNotOkAsync = (64, "UpdateCycleNotOkAsync") // ❌ Mark failure
GatewayTask.EndOfProcessAsync   = (128, "EndOfProcessAsync")  // 🏁 Complete process
```

---

## ✅ **VALIDATION ENUMS**

### **🔍 ResultValidation** (Operation Results)
```csharp
// Success States
ResultValidation.None              = (0, "None")
ResultValidation.Valid             = (1, "Valid")           // ✅ All good

// Error States (Negative Values)
ResultValidation.Invalid           = (-1, "Invalid")
ResultValidation.BarCodeNotFound   = (-2, "BarCodeNotFound")   // 🏷️❌
ResultValidation.WorkFlowNotFound  = (-4, "WorkFlowNotFound")  // 🔗❌
ResultValidation.MachineNotFound   = (-8, "MachineNotFound")   // 🤖❌
ResultValidation.CycleNotFound     = (-16, "CycleNotFound")    // 🔄❌
ResultValidation.WorkFlowNotValid  = (-32, "WorkFlowNotValid") // 🔗⚠️
ResultValidation.PartNotValid      = (-64, "PartNotValid")     // 🏷️⚠️
ResultValidation.DestinationNotValid = (-128, "DestinationNotValid") // 📍⚠️
ResultValidation.PartNumberNotValid = (-256, "PartNumberNotValid")   // 🔢⚠️
ResultValidation.RecipeNotFound    = (-512, "RecipeNotFound")  // 📝❌
ResultValidation.ReferencesNotFound = (-1024, "ReferencesNotFound") // 📚❌
ResultValidation.PartRejected      = (-2048, "PieceRejected") // 🚫❌
ResultValidation.InvalidMachine    = (-4096, "InvalidMachine") // 🤖⚠️
ResultValidation.RuleNotFound      = (-8192, "RuleNotFound")   // 📋❌
ResultValidation.ProductNotFound   = (-16384, "ProductNotFound") // 📦❌
ResultValidation.ShiftInvalid      = (-32768, "ShiftInvalid")  // ⏰⚠️
```

---

## ⏰ **OPERATIONAL ENUMS**

### **🕒 ShiftType** (Work Shifts)
```csharp
ShiftType.Invalid = (-1, "Invalid Value")
ShiftType.None    = (0, "None")
ShiftType.First   = (1, "First")    // 🌅 Day shift (6AM-2PM)
ShiftType.Second  = (2, "Second")   // 🌇 Evening shift (2PM-10PM)  
ShiftType.Third   = (4, "Third")    // 🌙 Night shift (10PM-6AM)
```

### **🏷️ TagsGroupsEnum** (Variable Classification)
```csharp
TagsGroupsEnum.None             = (0, "None")
TagsGroupsEnum.EventTags        = (1, "EventTags")        // 📅 Event-driven
TagsGroupsEnum.ReadOnlyTags     = (2, "ReadOnlyTags")     // 📖 Monitor only
TagsGroupsEnum.WriteOnlyTags    = (4, "WriteOnlyTags")    // ✏️ Control only
TagsGroupsEnum.WriteAndReadTags = (8, "WriteAndReadTags") // 🔄 Bidirectional
TagsGroupsEnum.ReadCyclicTags   = (16, "ReadCyclicTags")  // 🔄📖 Periodic read
TagsGroupsEnum.WriteCyclicTags  = (32, "WriteCyclicTags") // 🔄✏️ Periodic write
TagsGroupsEnum.HeartbeatTags    = (64, "HeartbeatTags")   // 💓 Health monitoring
TagsGroupsEnum.RegisterTags     = (128, "RegisterTags")   // 📊 Data logging
TagsGroupsEnum.ReferenceTags    = (256, "ReferenceTags")  // 📚 Reference data
```

---

## 💡 **TESTING PATTERNS**

### **✅ Valid Test Cases**
```csharp
// Manufacturing Success Scenarios
[InlineData(1, "Ok")]              // PartStatus.Ok
[InlineData(4, "FinishedOk")]      // CycleStatus.FinishedOk  
[InlineData(2, "InProcess")]       // FlowStatus.InProcess
[InlineData(8, "ProcessAsync")]    // MachineType.Process
[InlineData(2, "ProcessAsync")]    // WorkFlowType.Serial
```

### **❌ Invalid Test Cases**
```csharp
[InlineData(-1, "Invalid Value")]  // Any enum Invalid
[InlineData(999, "Unknown")]       // Non-existent values
[InlineData(0, "None")]           // None/default states
```

### **🏭 Manufacturing Context Examples**
```csharp
// Automotive: Ford F-150 engine block VIN = "1FTFW1ET5DFC12345"
// Electronics: iPhone PCB Serial = "C02YG0VZJHD4"
// Pharmaceutical: Batch = "LOT-PFZ-2024-001"
// Food & Beverage: Coca-Cola bottle = "CC-ATL-240115-001"
```

---

## 🎯 **QUICK REFERENCE**

### **Common Manufacturing Flow**
1. **MachineType.Initial** → Create barcode → **PartStatus.Ok**
2. **WorkFlowType.Serial** → **CycleStatus.Started** → **FlowStatus.InProcess**
3. **MachineType.Process** → **CycleStatus.FinishedOk** → **PartStatus.Ok**
4. **MachineType.Final** → **FlowStatus.Finished** → **ResultValidation.Valid**

### **Error Handling Pattern**
- Always test **Invalid** values (-1)
- Test **None** default states (0)
- Test boundary values and non-existent enum values
- Use **ResultValidation** for operation outcomes

---

**🔧 Usage in Tests**: Use these exact enum values and manufacturing contexts for realistic industrial test scenarios!
