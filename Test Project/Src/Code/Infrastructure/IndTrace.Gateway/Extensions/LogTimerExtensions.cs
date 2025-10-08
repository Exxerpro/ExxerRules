// <copyright file="LogTimerExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Extensions;

public static class LogTimerExtensions
{
    public static int TimeForShortExecution = 100;
    public static int TimeForLongExecution = 2000;
    public static int TimeForVeryLongExecution = 400;

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate log timer extension logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    // TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated extension or validation logic. Refactor for maintainability if necessary.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency log timer operations, consider optimizing data processing and memory usage.
    public static Stopwatch StartAndLog(string message, ILogger logger)
    {
        var timer = new Stopwatch();
        timer.Start();
        logger.LogInformation("Gateway Task Started {message}", message);
        return timer;
    }

    public static Stopwatch StartAndLog(IIndTraceControllerRx controller, ILogger logger)
    {
        var timer = new Stopwatch();
        timer.Start();
        logger.LogInformation("Gateway Task started for Machine {Machine} and PLC {Command}", controller.MachineId, controller.PlcId);
        return timer;
    }

    public static void StopAndLogTimer(this Stopwatch timer, IIndTraceControllerRx controller, ILogger logger, string message)
    {
        timer.Stop();
        logger.LogInformation(
            timer.ElapsedMilliseconds <= TimeForShortExecution
                ? "Gateway Task ({ElapsedMilliseconds} ms)  {Message} {Machine} and PLC {Plc}"
                : "Gateway Task ({ElapsedMilliseconds} ms) long running method  {Message} {Machine} and PLC {Plc}",
            timer.ElapsedMilliseconds, message, controller.MachineId, controller.PlcId);
    }

    public static void StopAndLogTimer(this Stopwatch timer, ILogger logger, int controllerId, int command)
    {
        timer.Stop();
        if (timer.ElapsedMilliseconds <= TimeForLongExecution)
        {
            logger.LogInformation("IndTrace Performance Request:  ({ElapsedMilliseconds} ms) for controller: {controllerId} , Request: {Request} ", timer.ElapsedMilliseconds, controllerId, command);
        }

        logger.LogWarning(
            timer.ElapsedMilliseconds <= TimeForVeryLongExecution
                ? "IndTrace Performance Request: ({ElapsedMilliseconds} ms) Long Running for controller: {controllerId} , Request: {Request}"
                : "IndTrace Performance Request: ({ElapsedMilliseconds} ms) Very Long Running  for controller: {controllerId} , Request: {Request}",
            timer.ElapsedMilliseconds, controllerId, command);
    }
}
