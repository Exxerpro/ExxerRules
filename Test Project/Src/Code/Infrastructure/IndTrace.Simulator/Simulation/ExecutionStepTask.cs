// <copyright file="ExecutionStepTask.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Simulator.Simulation;

using IndTrace.DataStore.Models;
using IndTrace.Simulator.Models.Constants;
using IndTrace.Simulator.Validation;

/// <summary>
/// Represents a task to be executed as part of a simulation step, including command, state, and timing information.
/// </summary>
public class ExecutionStepTask
{
    /// <summary>
    /// Gets or sets the name of the task.
    /// </summary>
    public string Task { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the command code associated with the task.
    /// </summary>
    public short Command { get; set; }

    /// <summary>
    /// Gets or sets the state associated with the task.
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the delay time in milliseconds between tasks.
    /// </summary>
    public int DelayTimeBetweenTasks { get; set; }

    /// <summary>
    /// Gets the list of tasks for an initial station with a successful cycle.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<ExecutionStepTask> GetTaskListInitialStationCycleOk() =>
    [
        new ExecutionStepTask { Task = nameof(GatewayTask.CreateBarCodeAsync), Command = (int)GatewayTask.CreateBarCodeAsync, State = nameof(StateStatusEnum.Created), DelayTimeBetweenTasks = 2500 },
        new ExecutionStepTask { Task = nameof(GatewayTask.UpdateCycleOkAsync), Command = (int)GatewayTask.UpdateCycleOkAsync, State = nameof(StateStatusEnum.FinishedOk), DelayTimeBetweenTasks = 2500 }
    ];

    /// <summary>
    /// Gets the list of tasks for a process station with a successful cycle.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<ExecutionStepTask> GetTaskListProcessStationCycleOk() =>
    [
        new ExecutionStepTask { Task = nameof(GatewayTask.ReadBarCodeAsync), Command = (int)GatewayTask.ReadBarCodeAsync, State = nameof(StateStatusEnum.NotStarted), DelayTimeBetweenTasks = 2500 },
        new ExecutionStepTask { Task = nameof(GatewayTask.CreateCycleAsync), Command = (int)GatewayTask.CreateCycleAsync, State = nameof(StateStatusEnum.Started), DelayTimeBetweenTasks = 2500 },
        new ExecutionStepTask { Task = nameof(GatewayTask.UpdateCycleOkAsync), Command = (int)GatewayTask.UpdateCycleOkAsync, State = nameof(StateStatusEnum.FinishedOk), DelayTimeBetweenTasks = 2500 }
    ];

    /// <summary>
    /// Gets the list of tasks for an initial station with a not OK cycle.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<ExecutionStepTask> GetTaskListInitialStationCycleNotOk() =>
    [
        new ExecutionStepTask { Task = nameof(GatewayTask.CreateBarCodeAsync), Command = (int)GatewayTask.CreateBarCodeAsync, State = nameof(StateStatusEnum.Created), DelayTimeBetweenTasks = 2500 },
        new ExecutionStepTask { Task = nameof(GatewayTask.UpdateCycleNotOkAsync), Command = (int)GatewayTask.UpdateCycleNotOkAsync, State = nameof(StateStatusEnum.FinishedNotOk), DelayTimeBetweenTasks = 2500 }
    ];

    /// <summary>
    /// Gets the list of tasks for a process station with a not OK cycle.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<ExecutionStepTask> GetTaskListCycleProcessStationNotOk() =>
    [
        new ExecutionStepTask { Task = nameof(GatewayTask.ReadBarCodeAsync), Command = (int)GatewayTask.ReadBarCodeAsync, State = nameof(StateStatusEnum.NotStarted), DelayTimeBetweenTasks = 2500 },
        new ExecutionStepTask { Task = nameof(GatewayTask.CreateCycleAsync), Command = (int)GatewayTask.CreateCycleAsync, State = nameof(StateStatusEnum.Started), DelayTimeBetweenTasks = 2500 },
        new ExecutionStepTask { Task = nameof(GatewayTask.UpdateCycleNotOkAsync), Command = (int)GatewayTask.UpdateCycleNotOkAsync, State = nameof(StateStatusEnum.FinishedNotOk), DelayTimeBetweenTasks = 2500 }
    ];

    /// <summary>
    /// Gets the list of tasks for an initial station with a started cycle.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<ExecutionStepTask> GetTaskListInitialStationCycleStarted() =>
    [
        new ExecutionStepTask { Task = nameof(GatewayTask.CreateBarCodeAsync), Command = (int)GatewayTask.CreateBarCodeAsync, State = nameof(StateStatusEnum.Created), DelayTimeBetweenTasks = 2500 },
    ];

    /// <summary>
    /// Gets the list of tasks for a process station with a started cycle.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<ExecutionStepTask> GetTaskListProcessStationCycleStarted() =>
    [
        new ExecutionStepTask { Task = nameof(GatewayTask.ReadBarCodeAsync), Command = (int)GatewayTask.ReadBarCodeAsync, State = nameof(StateStatusEnum.NotStarted), DelayTimeBetweenTasks = 2500 },
        new ExecutionStepTask { Task = nameof(GatewayTask.CreateCycleAsync), Command = (int)GatewayTask.CreateCycleAsync, State = nameof(StateStatusEnum.Started), DelayTimeBetweenTasks = 2500 },

    ];

    /// <summary>
    /// Resolves the appropriate task list based on machine position, execution flavor, and step index.
    /// </summary>
    /// <param name="initialMachine">Indicates if this is the initial machine in the sequence.</param>
    /// <param name="flavor">The execution flavor.</param>
    /// <param name="machineNumberPath">The index of the machine in the path.</param>
    /// <param name="maxSteps">The total number of steps/machines.</param>
    /// <returns>The resolved list of execution step tasks.</returns>
    public static IEnumerable<ExecutionStepTask> ResolveTaskList(
        bool initialMachine,
        ExecutionFlavor flavor,
        int machineNumberPath,
        int maxSteps)
    {
        if (initialMachine && flavor == ExecutionFlavor.FullSuccess)
        {
            return GetTaskListInitialStationCycleOk();
        }

        if (!initialMachine && flavor == ExecutionFlavor.FullSuccess)
        {
            return GetTaskListProcessStationCycleOk();
        }

        if (initialMachine && flavor == ExecutionFlavor.FinalFailure && machineNumberPath == maxSteps - 1)
        {
            return GetTaskListInitialStationCycleNotOk();
        }

        if (!initialMachine && flavor == ExecutionFlavor.FinalFailure && machineNumberPath == maxSteps - 1)
        {
            return GetTaskListInitialStationCycleNotOk();
        }

        if (initialMachine && flavor == ExecutionFlavor.CycleStarted)
        {
            return GetTaskListInitialStationCycleOk();
        }

        if (!initialMachine && flavor == ExecutionFlavor.CycleStarted)
        {
            return GetTaskListProcessStationCycleStarted();
        }

        if (initialMachine && flavor == ExecutionFlavor.CycleStarted && machineNumberPath == maxSteps - 1)
        {
            return GetTaskListInitialStationCycleStarted();
        }

        return GetTaskListProcessStationCycleOk();
    }
}
