namespace IndTrace.Aggregation.BoundedTests.Mediatr;
/// <summary>
/// Represents the CreateItemCommandHandler.
/// </summary>

public class CreateItemCommandHandler<T> : IMonitorRequestHandler<CreateItemCommand<T>, T> where T : MonitorRequestDispatcherTests.IItem
{
    /// <summary>
    /// Executes Handle operation.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>The result of Handle.</returns>
    public T Handle(CreateItemCommand<T> message)
    {
        return Activator.CreateInstance<T>();
    }
    /// <summary>
    /// Executes ProcessAsync operation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellationToken.</param>
    /// <returns>The result of ProcessAsync.</returns>

    public async Task<Result<T>> ProcessAsync(CreateItemCommand<T> request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(Result<T>.Success(Activator.CreateInstance<T>()));
    }
}
