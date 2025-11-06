using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Interfaces;

/// <summary>
/// Defines the contract for a mediator that handles command and query dispatching.
/// This interface provides a clean abstraction for the personalized MediatR pattern implementation.
/// Commands can return Result or Result&lt;T&gt;, queries return Result&lt;T&gt;.
/// All operations are cancellation-aware and follow functional programming principles.
/// Uses IndQuestResults for functional Result pattern implementation.
/// </summary>
public interface IRequestDispatcher
{
    /// <summary>
    /// Sends a command to its handler and returns a Result.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to send.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class;

    /// <summary>
    /// Sends a command to its handler and returns a Result&lt;T&gt;.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to send.</typeparam>
    /// <typeparam name="TResponse">The type of response expected.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result&lt;TResponse&gt; containing the response or an error.</returns>
    Task<Result<TResponse>> Send<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class;

    /// <summary>
    /// Sends a query to its handler and returns a Result&lt;T&gt;.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to send.</typeparam>
    /// <typeparam name="TResponse">The type of response expected.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result&lt;TResponse&gt; containing the response or an error.</returns>
    Task<Result<TResponse>> SendQuery<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default) where TQuery : class;
}