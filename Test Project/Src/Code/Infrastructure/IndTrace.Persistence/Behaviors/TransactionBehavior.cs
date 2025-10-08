using IndTrace.Application.Models.RequestHandler;
using IndTrace.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IndTrace.Persistence.Behaviors;

/// <summary>
/// Pipeline behavior for handling transactions of requests, ensuring transactional consistency and error handling.
/// </summary>
/// <typeparam name="TRequest">Type of the request being processed.</typeparam>
/// <typeparam name="TResponse">Type of the response produced.</typeparam>
/// <summary>
/// Pipeline behavior that handles transactions for requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IIndTraceDbContext context;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    public TransactionBehavior(
        IIndTraceDbContext context,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        this.context = context;
        this.logger = logger;
    }



    /// <summary>
    /// Processes the incoming request in a transaction, committing or rolling back as needed.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">Delegate for the next action in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response of the action.</returns>
    public async Task<TResponse> HandleAsync(TRequest request, RequestFunctionalHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ITransactionRequired)
        {
            this.logger.LogInformation("Transaction not required for {CommandName} ({@Command})", typeof(TRequest).Name, request);

            var response = await next().ConfigureAwait(false);

            await this.context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return response;
        }

        try
        {
            var strategy = this.context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(
                async (cancellationToken) =>
            {
                // Check if the database supports transactions

                if (this.context.SupportsTransactions)
                {
                    await using var transaction = await this.context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

                    this.logger.LogInformation("Starting transaction for {CommandName} ({@Command})", typeof(TRequest).Name,
                        request);

                    var response = await next().ConfigureAwait(false);

                    // Save changes and commit the transaction
                    await this.context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

                    this.logger.LogInformation(
                        "Transaction committed for {CommandName} ({@Command})",
                        typeof(TRequest).Name, request);

                    return response;
                }
                else
                {
                    this.logger.LogInformation(
                        "Transaction not supported for {CommandName} ({@Command})",
                        typeof(TRequest).Name, request);

                    var response = await next().ConfigureAwait(false);

                    // Save changes without transaction
                    await this.context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    return response;
                }
            }, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Exception while transaction for  {CommandName} ({@Command})", typeof(TRequest).Name, request);

            try
            {
                //TODO CHECK IF WE CAN MATERIALIZE AN TResponse object
                if (typeof(TResponse).IsValueType)
                {
                    return default(TResponse)!;
                }

                var result = Activator.CreateInstance<TResponse>();
                // Set the 'Error' and 'Success' properties if they exist.

                if (result == null)
                {
                    this.logger.LogError("Failed to create an instance of TResponse: {TResponseType}", typeof(TResponse).Name);
                    return default(TResponse)!;
                }
                var errorProperty = result.GetType().GetProperty("Error");
                var successProperty = result.GetType().GetProperty("Success");

                errorProperty?.SetValue(result, ex.Message);
                successProperty?.SetValue(result, false);

                return result;
            }
            catch (Exception createEx)
            {
                this.logger.LogError(createEx, "Failed to create an instance of TResponse: {TResponseType}", typeof(TResponse).Name);
                // Avoid throwing to keep functional pipeline semantics; return default
                return default(TResponse)!;
            }
        }
    }


}

//TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated transaction or logging logic. Refactor for maintainability if necessary.
//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Exception handling: avoid catching general Exception, catch specific exceptions where possible. See .NET best practices for exception handling.
//TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Ensure async/await is used efficiently and avoid unnecessary context switches. Use ConfigureAwait(false) in library code.
