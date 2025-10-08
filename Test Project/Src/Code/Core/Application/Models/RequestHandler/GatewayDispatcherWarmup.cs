// <copyright file="GatewayDispatcherWarmup.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.RequestHandler;

using IndTrace.Application.Cycles.Commands.Create;
using IndTrace.Application.Models.Behaviors;

public static class GatewayDispatcherWarmup
{
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Ensure warmup logic is robust and handles edge cases defensively. Validate all dependencies and handle initialization failures gracefully.
    public static void PreloadDispatcherCache()
    {
        // Preload known handler and behavior method metadata into the dispatcher caches
        var handlerTypes = new[]
        {
            typeof(IGatewayRequestHandler<CreateBarCodeCommand, TaskGatewayResponse>),
            typeof(IGatewayRequestHandler<ReadBarCodeQuery, TaskGatewayResponse>),
            typeof(IGatewayRequestHandler<CreateCyclesCommand, TaskGatewayResponse>),
            typeof(IGatewayRequestHandler<UpdateCyclesOkCommand, TaskGatewayResponse>),
            typeof(IGatewayRequestHandler<UpdateCyclesNotOkCommand, TaskGatewayResponse>),
            typeof(IGatewayRequestHandler<UpdateBarCodeCommand, TaskGatewayResponse>),
        };

        foreach (var type in handlerTypes)
        {
            _ = type.GetMethod("ProcessAsync");
        }

        // Also preload behaviors if needed
        var behaviorTypes = new[]
        {
            typeof(LoggingBehavior<,>),
            typeof(GatewayPersistenceBehavior<,>),
            typeof(ValidationBehavior<,>),
            typeof(EventLoggerBehavior<,>),
            typeof(RequestPerformanceBehaviour<,>),
            typeof(UnhandledExceptionBehaviour<,>),
        };

        foreach (var openGeneric in behaviorTypes)
        {
            var closed = openGeneric.MakeGenericType(typeof(CreateBarCodeCommand), typeof(TaskGatewayResponse));
            _ = closed.GetMethod("HandleAsync");
        }
    }
}
