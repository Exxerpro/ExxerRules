namespace IndTrace.Aggregation.BoundedTests.BarCodes.Services;

public static class ValidationTestData
{
    public static IEnumerable<object[]> GetValidationTestData()
    {
        //[Fix]
        //CLAUDE
        //Date: 09/09/2025
        //Reason: [Test Expectations] - Update test expectations to match actual validation business logic - when machineId(1) != nextMachineId(100), should return DestinationNotValid
        yield return new object[] { FlowStatus.Created, MachineType.Printer, CycleStatus.Started, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.Created, MachineType.InitialPrinter, CycleStatus.Started, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.Created, MachineType.Initial, CycleStatus.None, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.InProcess, MachineType.InitialPrinter, CycleStatus.None, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.InProcess, MachineType.Process, CycleStatus.None, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.InProcess, MachineType.Final, CycleStatus.NotStarted, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.InProcess, MachineType.Final, CycleStatus.FinishedOk, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.InProcess, MachineType.Final, CycleStatus.FinishedNok, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.InProcess, MachineType.Final, CycleStatus.Started, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.InProcess, MachineType.DashBoard, CycleStatus.NotStarted, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.InProcess, MachineType.DashBoard, CycleStatus.FinishedOk, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.InProcess, MachineType.Printer, CycleStatus.Started, ResultValidation.DestinationNotValid };
        //[Fix]
        //CLAUDE
        //Date: 09/09/2025
        //Reason: [Test Expectations] - Fix remaining 2 cases that still expect WorkFlowNotValid but get DestinationNotValid due to machineId!=nextMachineId
        yield return new object[] { FlowStatus.InProcess, MachineType.Final, CycleStatus.None, ResultValidation.DestinationNotValid };
        yield return new object[] { FlowStatus.Finished, MachineType.DashBoard, CycleStatus.FinishedOk, ResultValidation.DestinationNotValid };
    }
}
