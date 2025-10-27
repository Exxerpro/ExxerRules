using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IndQuestResults;

namespace IndFusion.SemanticRag.Application.Services;

/// <summary>
/// A simple implementation of the IMediator interface that uses dependency injection to resolve handlers.
/// This provides a clean abstraction that matches the personalized MediatR pattern where:
/// - Commands can return Result or Result&lt;T&gt;
/// - Queries return Result&lt;T&gt;
/// - All errors are handled gracefully and returned as Results, never as exceptions
/// </summary>
public class SimpleMediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SimpleMediator> _logger;

    /// <summary>
    /// Initializes a new instance of the SimpleMediator class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving handlers.</param>
    /// <param name="logger">The logger instance.</param>
    public SimpleMediator(IServiceProvider serviceProvider, ILogger<SimpleMediator> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<Result> Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class
    {
        try
        {
            if (command == null)
            {
                return Result.WithFailure("Command cannot be null");
            }

            _logger.LogDebug("Sending command of type {CommandType}", typeof(TCommand).Name);

            var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
            if (handler == null)
            {
                return Result.WithFailure($"No handler found for command type {typeof(TCommand).Name}");
            }

            return await handler.Handle(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending command of type {CommandType}", typeof(TCommand).Name);
            return Result.WithFailure($"Error sending command: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<TResponse>> Send<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class
    {
        try
        {
            if (command == null)
            {
                return Result<TResponse>.WithFailure("Command cannot be null");
            }

            _logger.LogDebug("Sending command of type {CommandType} expecting response of type {ResponseType}", 
                typeof(TCommand).Name, typeof(TResponse).Name);

            var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResponse>>();
            if (handler == null)
            {
                return Result<TResponse>.WithFailure($"No handler found for command type {typeof(TCommand).Name} with response type {typeof(TResponse).Name}");
            }

            return await handler.Handle(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending command of type {CommandType} with response type {ResponseType}", 
                typeof(TCommand).Name, typeof(TResponse).Name);
            return Result<TResponse>.WithFailure($"Error sending command: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<TResponse>> SendQuery<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default) where TQuery : class
    {
        try
        {
            if (query == null)
            {
                return Result<TResponse>.WithFailure("Query cannot be null");
            }

            _logger.LogDebug("Sending query of type {QueryType} expecting response of type {ResponseType}", 
                typeof(TQuery).Name, typeof(TResponse).Name);

            var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResponse>>();
            if (handler == null)
            {
                return Result<TResponse>.WithFailure($"No handler found for query type {typeof(TQuery).Name} with response type {typeof(TResponse).Name}");
            }

            return await handler.Handle(query, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending query of type {QueryType} with response type {ResponseType}", 
                typeof(TQuery).Name, typeof(TResponse).Name);
            return Result<TResponse>.WithFailure($"Error sending query: {ex.Message}");
        }
    }
}
