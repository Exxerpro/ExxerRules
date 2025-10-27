using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Interfaces;

/// <summary>
/// Defines the contract for a command handler that returns a Result.
/// All handlers are cancellation-aware and follow functional programming principles.
/// Uses IndQuestResults for functional Result pattern implementation.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
public interface ICommandHandler<in TCommand> where TCommand : class
{
    /// <summary>
    /// Handles the specified command and returns a Result.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines the contract for a command handler that returns a Result&lt;T&gt;.
/// All handlers are cancellation-aware and follow functional programming principles.
/// Uses IndQuestResults for functional Result pattern implementation.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
/// <typeparam name="TResponse">The type of response to return.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> where TCommand : class
{
    /// <summary>
    /// Handles the specified command and returns a Result&lt;TResponse&gt;.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result&lt;TResponse&gt; containing the response or an error.</returns>
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines the contract for a query handler that returns a Result&lt;T&gt;.
/// All handlers are cancellation-aware and follow functional programming principles.
/// Uses IndQuestResults for functional Result pattern implementation.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResponse">The type of response to return.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> where TQuery : class
{
    /// <summary>
    /// Handles the specified query and returns a Result&lt;TResponse&gt;.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result&lt;TResponse&gt; containing the response or an error.</returns>
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default);
}
