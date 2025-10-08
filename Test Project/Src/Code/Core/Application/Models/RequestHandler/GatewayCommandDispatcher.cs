// <copyright file="GatewayCommandDispatcher.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.RequestHandler
{
    using IndTrace.Application.Cycles.Commands.Create;
    using IndTrace.Application.Performance.Request.Command.Create;

    /// <summary>
    /// Dispatches gateway commands to their appropriate handlers and manages pipeline behaviors.
    /// </summary>
    public class GatewayCommandDispatcher : IGatewayCommandDispatcher
    {
        private readonly IServiceProvider provider;
        private readonly ILogger<GatewayCommandDispatcher> logger;
        private readonly Dictionary<Type, Func<object, CancellationToken, Task<object>>> handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayCommandDispatcher"/> class.
        /// </summary>
        /// <param name="provider">The service provider for resolving handlers.</param>
        /// <param name="logger">The logger for logging command dispatching.</param>
        public GatewayCommandDispatcher(IServiceProvider provider, ILogger<GatewayCommandDispatcher> logger)
        {
            this.provider = provider;
            this.logger = logger;

            this.handlers = new()
            {
                [typeof(CreateBarCodeCommand)] = async (request, token) =>
                {
                    var handler = this.provider.GetRequiredService<IGatewayRequestHandler<CreateBarCodeCommand, TaskGatewayResponse>>();
                    return await handler.ProcessAsync((CreateBarCodeCommand)request, token).ConfigureAwait(false);
                },
                [typeof(ReadBarCodeQuery)] = async (request, token) =>
                {
                    var handler = this.provider.GetRequiredService<IGatewayRequestHandler<ReadBarCodeQuery, TaskGatewayResponse>>();
                    return await handler.ProcessAsync((ReadBarCodeQuery)request, token).ConfigureAwait(false);
                },
                [typeof(CreateCyclesCommand)] = async (request, token) =>
                {
                    var handler = this.provider.GetRequiredService<IGatewayRequestHandler<CreateCyclesCommand, TaskGatewayResponse>>();
                    return await handler.ProcessAsync((CreateCyclesCommand)request, token).ConfigureAwait(false);
                },
                [typeof(UpdateCyclesOkCommand)] = async (request, token) =>
                {
                    var handler = this.provider.GetRequiredService<IGatewayRequestHandler<UpdateCyclesOkCommand, TaskGatewayResponse>>();
                    return await handler.ProcessAsync((UpdateCyclesOkCommand)request, token).ConfigureAwait(false);
                },
                [typeof(UpdateCyclesNotOkCommand)] = async (request, token) =>
                {
                    var handler = this.provider.GetRequiredService<IGatewayRequestHandler<UpdateCyclesNotOkCommand, TaskGatewayResponse>>();
                    return await handler.ProcessAsync((UpdateCyclesNotOkCommand)request, token).ConfigureAwait(false);
                },
                [typeof(UpdateBarCodeCommand)] = async (request, token) =>
                {
                    var handler = this.provider.GetRequiredService<IGatewayRequestHandler<UpdateBarCodeCommand, TaskGatewayResponse>>();
                    return await handler.ProcessAsync((UpdateBarCodeCommand)request, token).ConfigureAwait(false);
                },

                [typeof(PerformanceDataCommand)] = async (request, token) =>
                {
                    var handler = this.provider.GetRequiredService<IGatewayRequestHandler<PerformanceDataCommand, TaskGatewayResponse>>();
                    return await handler.ProcessAsync((PerformanceDataCommand)request, token).ConfigureAwait(false);
                },
            };
        }

        /// <summary>
        /// Processes a gateway request and returns the result.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The gateway request to process.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with the result of the request.</returns>
        public Task<Result<TResponse>> ProcessAsync<TResponse>(IGatewayRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var requestType = request.GetType();

            if (!this.handlers.TryGetValue(requestType, out var handlerDelegate))
            {
                this.logger.LogError("No registered handler for request type {RequestType}", requestType.Name);
                return Task.FromResult(Result<TResponse>.WithFailure($"No registered handler for request type {requestType.Name}"));
            }

            RequestFunctionalHandlerDelegate<Result<TResponse>> finalFunctionalHandler = async () =>
            {
                return (Result<TResponse>)await handlerDelegate(request, cancellationToken).ConfigureAwait(false);
            };

            return this.InvokeBehaviors(request, finalFunctionalHandler, cancellationToken);
        }

        /// <summary>
        /// Processes a gateway query and returns the result.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The gateway request to query.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with the result of the query.</returns>
        public Task<Result<TResponse>> QueryAsync<TResponse>(IGatewayRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return this.ProcessAsync(request, cancellationToken);
        }

        private Task<Result<TResponse>> InvokeBehaviors<TRequest, TResponse>(
            TRequest request,
            RequestFunctionalHandlerDelegate<Result<TResponse>> finalFunctionalHandler,
            CancellationToken cancellationToken)
        {
            var behaviors = this.provider.GetServices<IPipelineBehavior<TRequest, Result<TResponse>>>().Reverse().ToList();

            foreach (var behavior in behaviors)
            {
                var next = finalFunctionalHandler;
                finalFunctionalHandler = () => behavior.HandleAsync(request, next, cancellationToken);
            }

            return finalFunctionalHandler();
        }
    }
}
